namespace WCecko.Model.Map;

using Mapsui;
using Mapsui.Layers;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using WCecko.Model;
using WCecko.Model.Rating;
using WCecko.Model.User;
using Brush = Mapsui.Styles.Brush;
using Color = Mapsui.Styles.Color;

/// <summary>
/// Service for managing the map and its features.
/// </summary>
public class MapService
{
    private readonly MapDatabaseService _mapDatabaseService;
    private readonly RatingDatabaseService _ratingDatabaseService;
    private readonly UserService _userService;

    /// <summary>
    /// Name of the layer that contains the points on the map.
    /// </summary>
    public const string POINTS_LAYER_NAME = "user_points";
    private readonly MemoryLayer _pointsLayer;

    private static readonly int s_poopIconId = ImageUtils.RegisterBitmapAsync("poop.png");
    private static readonly Brush s_poopColor = new(new Color(100, 69, 40));

    /// <summary>
    /// The map control element used to display the map.
    /// </summary>
    public MapControl MapControl { get; }

    /// <summary>
    /// The map object that contains the layers and other map-related properties.
    /// </summary>
    public Mapsui.Map Map => MapControl.Map;

    /// <summary>
    /// Constructor for the <see cref="MapService"/> class.
    /// </summary>
    /// <param name="db">Service containing the connected database.</param>
    /// <param name="userService">Service for interacting with users.</param>
    public MapService(DatabaseService db, UserService userService)
    {
        _mapDatabaseService = new(db.GetConnection());
        _ratingDatabaseService = new(db.GetConnection());
        _userService = userService;

        MapControl = new MapControl();
        _pointsLayer = new MemoryLayer
        {
            Name = POINTS_LAYER_NAME,
            Style = null,
            IsMapInfoLayer = true
        };

        Map.Layers.Add(OpenStreetMap.CreateTileLayer());
        Map.Layers.Add(_pointsLayer);

        // disable map rotation (mainly useful for mobile devices)
        Map.Navigator.RotationLock = true;

        Task.Run(InitializeAsync);
    }

    private async Task InitializeAsync()
    {
        try
        {
            await AddAllPlacesToMap();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing map: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a new place on the map and in the database.
    /// </summary>
    /// <param name="mPoint">Map point where to create the place.</param>
    /// <param name="title">Title of the new place.</param>
    /// <param name="description">Description of the new place</param>
    /// <param name="image">Optional image of the new place.</param>
    /// <returns>True if the place was successfully created, False otherwise.</returns>
    public async Task<bool> CreatePlaceAsync(MPoint mPoint, string title, string description, ImageSource? image)
    {
        User? user = _userService.CurrentUser;
        if (user is null)
            return false;

        if (!user.HasPermission(UserPermission.CreatePlaces))
            return false;

        Place? newPlace = await _mapDatabaseService.CreatePlaceAsync(mPoint, user.Username, title, description, image);
        if (newPlace is null)
            return false;

        AddPointToMap(mPoint, newPlace.Id);
        return true;
    }

    /// <summary>
    /// Retrieves a place by its ID.
    /// </summary>
    /// <param name="id">ID of the place.</param>
    /// <returns>Place with the given ID if found, null otherwise</returns>
    public async Task<Place?> GetPlaceAsync(int id)
    {
        return await _mapDatabaseService.GetPlaceAsync(id);
    }

    /// <summary>
    /// Updates an existing place in the database.
    /// </summary>
    /// <param name="id">ID of the place being updated.</param>
    /// <param name="title">New title for the place.</param>
    /// <param name="description">New description for the place.</param>
    /// <param name="image">New image for the place.</param>
    /// <returns>True if the place was successfully updated, False otherwise.</returns>
    public async Task<bool> UpdatePlaceAsync(int id, string title, string description, ImageSource? image)
    {
        Place? existingPlace = await _mapDatabaseService.GetPlaceAsync(id);
        if (existingPlace is null)
            return false;

        if (!CheckModifyPermission(existingPlace.CreatedBy))
            return false;

        Place newPlace = new()
        {
            Id = id,
            Title = title,
            Description = description,
            ImagePath = image is not null ? await ImageUtils.SaveImageAsync(image, $"{Guid.NewGuid()}.jpg") : null,
            CreatedBy = existingPlace.CreatedBy,
            Location = existingPlace.Location
        };

        return await _mapDatabaseService.UpdatePlaceAsync(newPlace);
    }

    /// <summary>
    /// Deletes a place from the map and the database.
    /// Also deletes all ratings associated with the place.
    /// </summary>
    /// <param name="id">ID of the place to delete.</param>
    /// <returns>True if the place was successfully deleted, False otherwise.</returns>
    public async Task<bool> DeletePlaceAsync(int id)
    {
        Place? place = await _mapDatabaseService.GetPlaceAsync(id);
        if (place is null)
            return false;

        if (!CheckModifyPermission(place.CreatedBy))
            return false;

        bool ratingsDeleted = await _ratingDatabaseService.DeletePlaceRatingsAsync(id);
        if (!ratingsDeleted)
            return false;

        bool placeDeleted = await _mapDatabaseService.DeletePlaceAsync(id);
        if (!placeDeleted)
            return false;

        RemovePointFromMap(id);
        return true;
    }

    private bool CheckModifyPermission(string pointCreator)
    {
        User? user = _userService.CurrentUser;
        if (user is null)
            return false;

        if (user.HasPermission(UserPermission.ModifyAllPlaces))
            return true;

        if (user.Username == pointCreator && user.HasPermission(UserPermission.ModifyOwnPlaces))
            return true;

        return false;
    }

    private async Task AddAllPlacesToMap()
    {
        IReadOnlyList<Place> places = await _mapDatabaseService.GetAllPlacesAsync();
        List<IFeature> features = _pointsLayer.Features?.ToList() ?? [];

        foreach (Place place in places)
        {
            IFeature feature = CreatePoint(place.Location, place.Id);
            features.Add(feature);
        }

        _pointsLayer.Features = features;
        _pointsLayer.DataHasChanged();
        Map.Refresh();
    }

    private void AddPointToMap(MPoint mapPosition, int id)
    {
        IFeature newPoint = CreatePoint(mapPosition, id);
        List<IFeature> features = _pointsLayer.Features?.ToList() ?? [];

        features.Add(newPoint);
        _pointsLayer.Features = features;
        _pointsLayer.DataHasChanged();
        Map.Refresh();
    }

    private void RemovePointFromMap(int id)
    {
        List<IFeature> features = _pointsLayer.Features?.ToList() ?? [];
        IFeature? featureToRemove = features.FirstOrDefault(f => f["ID"] is int featureId && featureId == id);
        if (featureToRemove is null)
            return;

        features.Remove(featureToRemove);
        _pointsLayer.Features = features;
        _pointsLayer.DataHasChanged();
        Map.Refresh();
    }

    private static IFeature CreatePoint(MPoint mPoint, int id)
    {
        PointFeature feature = new(mPoint);
        feature["ID"] = id;

        if (s_poopIconId != -1)
        {
            feature.Styles.Add(new SymbolStyle
            {
                BitmapId = s_poopIconId,
                SymbolScale = 0.25,
                SymbolOffset = new Offset(0, 64),
            });
        }
        else
        {
            feature.Styles.Add(new SymbolStyle
            {
                SymbolType = SymbolType.Triangle,
                SymbolRotation = 180,
                SymbolScale = 1,
                Fill = s_poopColor,
                SymbolOffset = new Offset(0, 16)
            });
        }

        return feature;
    }
}

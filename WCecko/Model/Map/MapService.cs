namespace WCecko.Model.Map;

using Mapsui;
using Mapsui.Layers;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using System.Diagnostics;
using WCecko.Model.Rating;
using WCecko.Model.User;
using Brush = Mapsui.Styles.Brush;
using Color = Mapsui.Styles.Color;


public class MapService
{
    public const string POINTS_LAYER_NAME = "user_points";

    private readonly MapDatabaseService _mapDatabaseService;
    private readonly RatingDatabaseService _ratingDatabaseService;
    private readonly UserService _userService;

    public MapControl MapControl { get; }
    public Mapsui.Map Map => MapControl.Map;

    private readonly MemoryLayer _pointsLayer;

    private static readonly int s_iconId = RegisterBitmapAsync("poop.png");

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

        // disable map rotation (mainly useful for mobile)
        Map.Navigator.RotationLock = true;

        Task.Run(AddAllPlacesToMap);
    }

    private static int RegisterBitmapAsync(string imageName)
    {
        try
        {
            string resourceName = $"WCecko.Resources.Images.{imageName}";
            System.Reflection.Assembly assembly = typeof(MapService).Assembly;

            Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is null)
            {
                Debug.WriteLine($"Failed to load embedded resource: {resourceName}");
                return -1;
            }

            return BitmapRegistry.Instance.Register(stream);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error registering bitmap: {ex.Message}");
            return -1;
        }
    }

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


    public async Task<Place?> GetPlaceAsync(int id)
    {
        return await _mapDatabaseService.GetPlaceAsync(id);
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

        bool result = await _mapDatabaseService.UpdatePlaceAsync(newPlace);
        if (!result)
            return false;

        return true;
    }

    public async Task AddAllPlacesToMap()
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

    public void AddPointToMap(MPoint mapPosition, int id)
    {
        IFeature newPoint = CreatePoint(mapPosition, id);
        List<IFeature> features = _pointsLayer.Features?.ToList() ?? [];

        features.Add(newPoint);
        _pointsLayer.Features = features;
        _pointsLayer.DataHasChanged();
        Map.Refresh();
    }

    public void RemovePointFromMap(int id)
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

    public static IFeature CreatePoint(MPoint mPoint, int id)
    {
        PointFeature feature = new(mPoint);
        feature["ID"] = id;

        Brush poopColor = new(new Color(100, 69, 40));


        if (s_iconId != -1)
        {
            feature.Styles.Add(new SymbolStyle
            {
                BitmapId = s_iconId,
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
                Fill = poopColor,
                SymbolOffset = new Offset(0, 16)
            });
        }

        return feature;
    }
}

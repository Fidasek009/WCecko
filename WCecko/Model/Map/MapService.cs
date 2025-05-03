using Mapsui;
using Mapsui.Layers;
using Mapsui.Styles;
using Color = Mapsui.Styles.Color;
using Brush = Mapsui.Styles.Brush;

using WCecko.Model.User;
using Mapsui.Tiling;
using Mapsui.UI.Maui;

namespace WCecko.Model.Map;

public class MapService
{
    public const string POINTS_LAYER_NAME = "user_points";
    public const int IMAGE_MAX_HEIGHT = 256;
    public const int IMAGE_MAX_WIDTH = 512;

    private readonly MapDatabaseService _mapDatabaseService;
    private readonly UserService _userService;
    
    public MapControl MapControl { get; }
    public Mapsui.Map Map => MapControl.Map;

    private readonly MemoryLayer _pointsLayer;


    public MapService(MapDatabaseService mapDatabaseService, UserService userService)
    {
        _mapDatabaseService = mapDatabaseService;
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

        Task.Run(AddAllPointsToMap);
    }


    public async Task<bool> CreateMapPointAsync(MPoint mPoint, string title, string description, ImageSource? image)
    {
        var user = _userService.CurrentUser;
        if (user == null)
            return false;

        if (!user.HasPermission(UserPermission.CreatePoints))
            return false;

        var newPointId = await _mapDatabaseService.CreateMapPointAsync(mPoint, user.Username, title, description, image);
        if (newPointId == null)
            return false;

        AddPointToMap(mPoint, newPointId.Value);
        return true;
    }


    public async Task<MapPoint?> GetMapPointAsync(int id)
    {
        return await _mapDatabaseService.GetMapPointAsync(id);
    }

    public async Task AddAllPointsToMap()
    {
        var mapPoints = await _mapDatabaseService.GetAllMapPointsAsync();
        var features = _pointsLayer.Features?.ToList() ?? [];

        foreach (var point in mapPoints)
        {
            var feature = CreatePoint(point.Location, point.Id);
            features.Add(feature);
        }

        _pointsLayer.Features = features;
        _pointsLayer.DataHasChanged();
        Map.Refresh();
    }

    public void AddPointToMap(MPoint mapPosition, int id)
    {
        var newPoint = CreatePoint(mapPosition, id);
        var features = _pointsLayer.Features?.ToList() ?? [];

        features.Add(newPoint);
        _pointsLayer.Features = features;
        _pointsLayer.DataHasChanged();
        Map.Refresh();
    }

    public static IFeature CreatePoint(MPoint mPoint, int id)
    {
        var feature = new PointFeature(mPoint);
        feature["ID"] = id;

        var poopColor = new Brush(new Color(100, 69, 40));

        feature.Styles.Add(new SymbolStyle
        {
            SymbolType = SymbolType.Triangle,
            SymbolRotation = 180,
            SymbolScale = 1,
            Fill = poopColor,
            SymbolOffset = new Offset(0, 16)
        });

        return feature;
    }
}

using Mapsui.Layers;
using Mapsui.Styles;
using Mapsui;

using Color = Mapsui.Styles.Color;
using Brush = Mapsui.Styles.Brush;
using Map = Mapsui.Map;
using Mapsui.UI.Maui;

namespace WCecko.Model;

class MapModel
{
    public static IFeature CreatePoint(MPoint mPoint)
    {
        var feature = new PointFeature(mPoint);
        feature["ID"] = $"{mPoint.X};{mPoint.Y}";

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

    public static void AddPointToMap(Map map, MPoint mapPosition)
    {
        var pointsLayer = map.Layers.FindLayer("user_points").First() as MemoryLayer;
        var newPoint = CreatePoint(mapPosition);
        var features = pointsLayer!.Features?.ToList() ?? new List<IFeature>();

        features.Add(newPoint);
        pointsLayer.Features = features;
        pointsLayer.DataHasChanged();
        map.Refresh();
    }
}

using Mapsui.Layers;
using Mapsui.Styles;
using Mapsui;

using Color = Mapsui.Styles.Color;
using Brush = Mapsui.Styles.Brush;

namespace WCecko.Model
{
    class MapModel
    {
        public static IFeature CreatePoint(MPoint mPoint)
        {
            var feature = new PointFeature(mPoint);

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
}

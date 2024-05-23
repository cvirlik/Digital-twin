using Digital_twin.Dataset.Types.Secondary;
using System.Windows.Controls;
using System.Windows;

namespace Digital_twin.Dataset.Support
{
    public class ShapeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SegmentTemplate { get; set; }
        public DataTemplate PolygonTemplate { get; set; }
        public DataTemplate PointTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Segment)
                return SegmentTemplate;

            if (item is Polygon)
                return PolygonTemplate;

            if (item is Types.Primary.Point)
                return PointTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}

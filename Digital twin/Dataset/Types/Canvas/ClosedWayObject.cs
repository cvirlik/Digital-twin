using Digital_twin.Dataset.Types.Primary;
using Digital_twin.Dataset.Types.Secondary;
using Digital_twin.Draw_tools;
using System.Collections.ObjectModel;
using OsmSharp;

namespace Digital_twin.Dataset.Types.Canvas
{
    // Class for rooms/buildings
    public class ClosedWayObject : CanvasObject
    {
        // OSM DATA
        protected ObservableCollection<Node> nodes { get; set; }
        protected Way way { get; set; }

        // POST-OSM DATA
        protected ObservableCollection<Segment> walls { get; set; } = new ObservableCollection<Segment>();
        protected Polygon innerFill;

        public ClosedWayObject(ObservableCollection<Node> _nodes, Way _way, bool isInner)
        {
            way = _way;
            foreach (var tag in way.Tags)
            {
                Tags.Add(new Tag { Key = tag.Key, Value = tag.Value });
            }
            nodes = _nodes;
            innerFill = new Polygon(isInner);
            innerFill.obj = this;
            DrawingTools.SplitToSegments(nodes, walls, isInner);
            DrawingTools.CreatePolygons(nodes, innerFill);
            shapes = new ObservableCollection<IShape>(walls);
            foreach(var shape in shapes)
            {
                shape.obj = this;
            }
            shapes.Insert(0, innerFill);
        }


        public Way Way { get { return way; } }
        public ObservableCollection<Node> Nodes { get { return nodes; } }
        public ObservableCollection<Segment> Walls { get { return walls; } }
    }
}
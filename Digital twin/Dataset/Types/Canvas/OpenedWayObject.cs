using Digital_twin.Dataset.Types.Primary;
using Digital_twin.Dataset.Types.Secondary;
using Digital_twin.Draw_tools;
using System.Collections.ObjectModel;
using OsmSharp;
using System.ServiceModel.PeerResolvers;

namespace Digital_twin.Dataset.Types.Canvas
{
    // Class for pathways
    public class OpenedWayObject : CanvasObject
    {
        // OSM DATA
        protected ObservableCollection<Node> nodes { get; set; }
        protected Way way { get; set; }


        // POST-OSM DATA
        protected ObservableCollection<Segment> segments { get; set; } = new ObservableCollection<Segment>();

        public OpenedWayObject(ObservableCollection<Node> _nodes, Way _way, bool isInner)
        {
            way = _way;
            foreach (var tag in way.Tags)
            {
                Tags.Add(new Tag { Key = tag.Key, Value = tag.Value });
            }
            nodes = _nodes;
            DrawingTools.SplitToSegments(nodes, segments, isInner);
            shapes = new ObservableCollection<IShape>(segments);
            foreach (var shape in shapes)
            {
                shape.obj = this;
            }
        }


        public Way Way { get { return way; } }
        public ObservableCollection<Node> Nodes { get { return nodes; } }
        public ObservableCollection<Segment> Segments { get { return segments; } }

        public void UpdateSegments(Segment segment)
        {
            segment.obj = this;
            segments.Add(segment);  
            shapes.Add(segment);
        }
    }
}

using Digital_twin.Dataset.Types.Primary;
using Digital_twin.Dataset.Types.Secondary;
using Digital_twin.Draw_tools;
using System.Collections.ObjectModel;
using OsmSharp;

namespace Digital_twin.Dataset.Types.Canvas
{
    // Class for pathways
    public class OpenedWayObject : CanvasObject
    {
        // OSM DATA
        protected ObservableCollection<Node> nodes { get; set; }
        protected Way way { get; set; }
        private bool _isInner;
        

        // POST-OSM DATA
        protected ObservableCollection<Segment> segments { get; set; } = new ObservableCollection<Segment>();

        public OpenedWayObject(ObservableCollection<Node> _nodes, Way _way, bool isInner)
        {
            way = _way;
            foreach (var tag in way.Tags)
            {
                Tags.Add(new Tag(tag.Key, tag.Value));
            }
            nodes = _nodes;
            _isInner = isInner;
            DrawingTools.SplitToSegments(nodes, segments, isInner, _way);
            shapes = new ObservableCollection<IShape>(segments);
            foreach (var shape in shapes)
            {
                shape.obj = this;
            }
        }


        public Way Way { get { return way; } 
            set {
                way = value;
                foreach(Segment segment in segments)
                {
                    segment.way = way;
                }
            } }
        public ObservableCollection<Node> Nodes { get { return nodes; } }
        public ObservableCollection<Segment> Segments { get { return segments; } }

        public void UpdateSegments(Segment segment)
        {
            segment.obj = this;
            segments.Add(segment);  
            shapes.Add(segment);
        }

        public void ReloadSegments(ObservableCollection<Node> _nodes)
        {
            Segments.Clear();
            nodes = _nodes;
            DrawingTools.SplitToSegments(nodes, Segments, _isInner, way);
            shapes = new ObservableCollection<IShape>(segments);
            foreach (var shape in shapes)
            {
                shape.obj = this;
            }
        }
    }
}

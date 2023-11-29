using System.Collections.ObjectModel;
using OsmSharp;
using Digital_twin.Draw_tools;
using Digital_twin.Dataset.Types.Secondary;

namespace Digital_twin.Dataset.Types
{
    public class Building
    {
        private Way way { get; set; }
        private ObservableCollection<Node> nodes { get; set; }
        private ObservableCollection<Segment> segmentsOuter { get; set; } = new ObservableCollection<Segment>(); // Building wall outline

        // Used as a starting point
        private double maxLatitude;
        private double minLongitude;
        public Building(Way _way, ObservableCollection<Node> _nodes, double _maxLatitude, double _minLongitude)
        {
            way = _way;
            nodes = _nodes;
            /*maxLatitude = nodes.Max(node => (double)node.Latitude);
            minLongitude = nodes.Min(node => (double)node.Longitude);*/
            maxLatitude = _maxLatitude;
            minLongitude = _minLongitude;
            DrawingTools.SplitToSegments(nodes, segmentsOuter, maxLatitude, minLongitude, false);
        }
        public string Name
        {
            get
            {
                if (way.Tags.ContainsKey("name"))
                    return way.Tags["name"];
                else
                    return "Unnamed building";
            }
        }
        public string LevelsCount
        {
            get
            {
                if (way.Tags.ContainsKey("building:levels"))
                    return way.Tags["building:levels"];
                else
                    return "1";
            }
        }

        public string MaxLevel
        {
            get
            {
                if (way.Tags.ContainsKey("max_level"))
                    return way.Tags["max_level"];
                else
                    return "1";
            }
        }

        public string MinLevel
        {
            get
            {
                if (way.Tags.ContainsKey("min_level"))
                    return way.Tags["min_level"];
                else
                    return "1";
            }
        }

        public Way Way { get { return way; } }
        public ObservableCollection<Node> Nodes { get { return nodes; } }
        public ObservableCollection<Segment> SegmentsOuter { get { return segmentsOuter; } }

        public double MaxLatitude { get { return maxLatitude; } }
        public double MinLongitude { get { return minLongitude; } }
    }
}

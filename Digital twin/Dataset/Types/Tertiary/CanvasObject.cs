using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Digital_twin.Dataset.Types.Primary;
using Digital_twin.Dataset.Types.Secondary;
using Digital_twin.Draw_tools;
using OsmSharp;

namespace Digital_twin.Dataset.Types.Tertiary
{
    public abstract class CanvasObject
    {
        // OSM DATA
        protected ObservableCollection<Node> nodes { get; set; }
        protected Way way { get; set; }
        public ObservableCollection<Tag> Tags { get; set; } = new ObservableCollection<Tag>();

        // POST-OSM DATA
        protected ObservableCollection<Segment> walls { get; set; } = new ObservableCollection<Segment>();
        protected Polygon innerFill;


        protected ObservableCollection<IShape> shapes;


        // DRAWING DATA
        protected double maxLatitude;
        protected double minLongitude;

        public CanvasObject(ObservableCollection<Node> _nodes, Way _way, bool isInner) {
            way = _way;
            foreach (var tag in way.Tags)
            {
                Tags.Add(new Tag { Key = tag.Key, Value = tag.Value });
            }
            nodes = _nodes;
            maxLatitude = DataManager.maxLatitude;
            minLongitude = DataManager.minLongitude;
            innerFill = new Polygon(this, isInner);
            DrawingTools.SplitToSegments(nodes, walls, maxLatitude, minLongitude, isInner, this);
            DrawingTools.CreatePolygons(nodes, innerFill, maxLatitude, minLongitude);
            shapes = new ObservableCollection<IShape>(walls);
            shapes.Insert(0, innerFill);
        }


        public Way Way { get { return way; } }
        public ObservableCollection<Node> Nodes { get { return nodes; } }
        public ObservableCollection<Segment> Walls { get { return walls; } }

        public ObservableCollection<IShape> Shapes
        {
            get {
                return shapes;
            }
        }
    }
}

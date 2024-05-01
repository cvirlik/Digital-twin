using Digital_twin.Dataset.Support;
using Digital_twin.Dataset.Types.Secondary;
using System.Collections.ObjectModel;
using OsmSharp;
using Digital_twin.Dataset.Types.Primary;
using System;
using Digital_twin.Dataset.Types.Canvas;
using Digital_twin.Dataset;
using System.Linq;
using OsmSharp.Tags;
using System.Collections.Generic;

namespace Digital_twin.Draw_tools
{
    public class DrawingTools
    {
        public static Tuple<int, int> CenterOffset(ObservableCollection<Node> nodes)
        {
            var mostLeft = nodes.OrderBy(p => p.Longitude).First();
            var mostRight = nodes.OrderByDescending(p => p.Longitude).First();
            var uppermost = nodes.OrderBy(p => p.Latitude).Last();
            var bottommost = nodes.OrderBy(p => p.Latitude).First();
            double xl, yl, hl;
            GpsUtils.GeodeticToEnu((double)mostLeft.Latitude, (double)mostLeft.Longitude, 0, 
                DataManager.maxLatitude, DataManager.minLongitude, 0,
                out xl, out yl, out hl);
            double xr, yr, hr;
            GpsUtils.GeodeticToEnu((double)mostRight.Latitude, (double)mostRight.Longitude, 0, 
                DataManager.maxLatitude, DataManager.minLongitude, 0,
                out xr, out yr, out hr);
            double xu, yu, hu;
            GpsUtils.GeodeticToEnu((double)uppermost.Latitude, (double)uppermost.Longitude, 0, 
                DataManager.maxLatitude, DataManager.minLongitude, 0,
                out xu, out yu, out hu);
            double xb, yb, hb;
            GpsUtils.GeodeticToEnu((double)bottommost.Latitude, (double)bottommost.Longitude, 0, 
                DataManager.maxLatitude, DataManager.minLongitude, 0,
                out xb, out yb, out hb);
            return new Tuple<int, int>(500/2 - ((int)xr + (int)xl) / 2 * 3, (400 / 2 - ((int)xu + (int)xb) / 2 * 3));
        }
        public static void SplitToSegments(ObservableCollection<Node> nodes, 
                                           ObservableCollection<Segment> segments,
                                           bool isInner, Way way)
        { 
            double previousX = -1;
            double previousY = -1;
            Node previousNode = null;

            foreach (var node in nodes)
            {
                double x, y, h;
                GpsUtils.GeodeticToEnu((double)(node as Node).Latitude, (double)(node as Node).Longitude, 0,
                    DataManager.maxLatitude, DataManager.minLongitude, 0,
                    out x, out y, out h);
                (x, y) = GpsUtils.LatLonToMeters((double)(node as Node).Latitude, (double)(node as Node).Longitude);
                double cx, cy;
                (cx, cy) = GpsUtils.MetersToCanvas(x, y);
                if (previousX != -1 && previousY != -1)
                {
                    Segment s = new Segment(previousX, previousY, previousNode, cx, cy, node, isInner);
                    s.way = way;
                    segments.Add(s);
                }
                previousX = cx; previousY = cy; previousNode = node;
            }
        }

        public static Point CreatePointFromNode(Node node)
        {
            double x, y, h;
            GpsUtils.GeodeticToEnu((double)(node as Node).Latitude, (double)(node as Node).Longitude, 0,
                DataManager.maxLatitude, DataManager.minLongitude, 0,
                out x, out y, out h);
            (x, y) = GpsUtils.LatLonToMeters((double)(node as Node).Latitude, (double)(node as Node).Longitude);
            double cx, cy;
            (cx, cy) = GpsUtils.MetersToCanvas(x, y);

            return new Point(cx, cy, node);
        }

        public static void CreatePolygons(ObservableCollection<Node> nodes, Polygon polygon)
        {
            foreach (var node in nodes) 
            {
                polygon.AddVertex(CreatePointFromNode(node));
            }
        }

        public static void AddNode(double X, double Y, DataManager dataManager)
        {
            Node n = new Node();
            n.Id = dataManager._idGenerator.GenerateIdNode();
            (X, Y) = GpsUtils.CanvasToMeters(X, Y);
            (n.Latitude, n.Longitude) = GpsUtils.MetersToLatLon(X, Y);
            n.Tags = new TagsCollection()
            {
                new OsmSharp.Tags.Tag("level", dataManager.SelectedLevel.Name)
            };
            NodeObject nodeObject = new NodeObject(n);
            dataManager.SelectedLevel.AddObjects(nodeObject, true);
            dataManager.Nodes.Add(n);
        }

        
        public static void AddWay(double X, double Y, double previousX, 
                           double previousY, bool removedStart, DataManager dataManager)
        {
            // Create new control point
            Node n = new Node();
            n.Id = dataManager._idGenerator.GenerateIdNode();
            (X, Y) = GpsUtils.CanvasToMeters(X, Y);
            (n.Latitude, n.Longitude) = GpsUtils.MetersToLatLon(X, Y);
            n.Tags = new TagsCollection();
            (X, Y) = GpsUtils.MetersToCanvas(X, Y);
            dataManager.Nodes.Add(n);


            if (previousX == -1 && previousY == -1) // If it start control point -- create placeholder NodeObject
            {
                Point point = new Point(X, Y, n);
                dataManager.SelectedLevel.addedShapes.Add(point);
                dataManager.startPoint = n;
            }
            else // If it at least one segment
            {
                if (!removedStart)
                {
                    dataManager.SelectedLevel.AddedShapes.RemoveAt(dataManager.SelectedLevel.AddedShapes.Count - 1);
                }
                if (dataManager.currentActiveObject == null)
                {
                    ObservableCollection<Node> _nodes = new ObservableCollection<Node>
                    {
                        dataManager.startPoint, n
                    };

                    Way way = new Way();
                    way.Id = dataManager._idGenerator.GenerateIdWay();
                    way.Tags = new TagsCollection
                    {
                        new OsmSharp.Tags.Tag("level", dataManager.SelectedLevel.Name)
                    };
                    dataManager.Ways.Add(way); 
                    List<long> nodes = new List<long>
                    {
                        (long)dataManager.startPoint.Id,
                        (long)n.Id
                    };

                    way.Nodes = nodes.ToArray();
                    dataManager.currentActiveObject = new OpenedWayObject(_nodes, way, true);
                    foreach (IShape segment in dataManager.currentActiveObject.Shapes)
                    {
                        dataManager.SelectedLevel.AddedShapes.Add(segment);
                    }
                }
                else
                {
                    Node previousPoint = ((OpenedWayObject)dataManager.currentActiveObject).Nodes.Last();
                    Segment addedSegment = new Segment(previousX, previousY, previousPoint, X, Y, n, true);
                    addedSegment.way = ((OpenedWayObject)dataManager.currentActiveObject).Way;
                    addedSegment.obj = dataManager.currentActiveObject;
                    ((OpenedWayObject)dataManager.currentActiveObject).UpdateSegments(addedSegment);
                    ((OpenedWayObject)dataManager.currentActiveObject).Nodes.Add(n);
                    List<long> nodes = ((OpenedWayObject)dataManager.currentActiveObject).Way.Nodes.ToList();
                    nodes.Add((long)n.Id);
                    ((OpenedWayObject)dataManager.currentActiveObject).Way.Nodes = nodes.ToArray();

                    dataManager.SelectedLevel.AddedShapes.Add(addedSegment);
                }
            }

        }

        public static void FinishWay(DataManager dataManager)
        {
            dataManager.SelectedLevel.AddObjects(dataManager.currentActiveObject, true);
            dataManager.currentActiveObject = null;
            dataManager.startPoint = null;
            dataManager.SelectedLevel.AddedShapes.Clear();
            dataManager.SelectedLevel.addedElements.Clear();
        }
        public static void CloseWay(double X, double Y, DataManager dataManager)
        {
            Node n = ((OpenedWayObject)dataManager.currentActiveObject).Nodes.Last();

            double sx, sy;
            (sx, sy) = GpsUtils.LatLonToMeters((double)dataManager.startPoint.Latitude, (double)dataManager.startPoint.Longitude);
            (sx, sy) = GpsUtils.MetersToCanvas(sx, sy);

            Segment addedSegment = new Segment(sx, sy, dataManager.startPoint, X, Y, n, true);
            addedSegment.way = ((OpenedWayObject)dataManager.currentActiveObject).Way;
            addedSegment.obj = dataManager.currentActiveObject;
            ((OpenedWayObject)dataManager.currentActiveObject).Segments.Add(addedSegment);
            ((OpenedWayObject)dataManager.currentActiveObject).Nodes.Add(dataManager.startPoint);

            List<long> nodes = ((OpenedWayObject)dataManager.currentActiveObject).Way.Nodes.ToList();
            nodes.Add((long)dataManager.startPoint.Id);
            ((OpenedWayObject)dataManager.currentActiveObject).Way.Nodes = nodes.ToArray();

            ClosedWayObject closedWayObject = new ClosedWayObject(((OpenedWayObject)dataManager.currentActiveObject).Nodes,
                ((OpenedWayObject)dataManager.currentActiveObject).Way, true);
            dataManager.SelectedLevel.AddObjects(closedWayObject, true);
            dataManager.currentActiveObject = null;
            dataManager.startPoint = null;
            dataManager.SelectedLevel.AddedShapes.Clear();
            dataManager.SelectedLevel.addedElements.Clear();
        }
    }
}

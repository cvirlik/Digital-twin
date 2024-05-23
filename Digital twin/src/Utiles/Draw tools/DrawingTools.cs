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
using Digital_twin.Dataset.Support.Actions;
using Digital_twin.Dataset.Support.Actions.AddActions;
using Digital_twin.UserControls;

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
            double xl, yl;
            (xl, yl) = GpsUtils.LatLonToMeters((double)mostLeft.Latitude, (double)mostLeft.Longitude);
            (xl, yl) = GpsUtils.MetersToCanvas(xl, yl);
            double xr, yr;
            (xr, yr) = GpsUtils.LatLonToMeters((double)mostRight.Latitude, (double)mostRight.Longitude);
            (xr, yr) = GpsUtils.MetersToCanvas(xr, yr);
            double xu, yu;
            (xu, yu) = GpsUtils.LatLonToMeters((double)uppermost.Latitude, (double)uppermost.Longitude);
            (xu, yu) = GpsUtils.MetersToCanvas(xu, yu);
            double xb, yb;
            (xb, yb) = GpsUtils.LatLonToMeters((double)bottommost.Latitude, (double)bottommost.Longitude);
            (xb, yb) = GpsUtils.MetersToCanvas(xb, yb);
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
                double x, y;
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
            double x, y;
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
            dataManager.actionList.AddAction(new DrawAction(new AddNodeAction(nodeObject, dataManager)));
        }

        
        public static void AddWay(double X, double Y, double previousX, 
                           double previousY, bool removedStart, DataManager dataManager, LineCanvas canvas)
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
                dataManager.actionList.AddAction(new DrawAction(new RemovePlaceholder(point, dataManager, canvas)));
            }
            else // If it at least one segment
            {
                if (!removedStart)
                {
                    dataManager.SelectedLevel.AddedShapes.RemoveAt(dataManager.SelectedLevel.AddedShapes.Count - 1);
                    canvas.SetRemoveStart(true);
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
                    dataManager.SelectedLevel.addedElements.Add(dataManager.currentActiveObject);

                    foreach (IShape segment in dataManager.currentActiveObject.Shapes)
                    {
                        dataManager.SelectedLevel.AddedShapes.Add(segment);
                        dataManager.actionList.AddAction(new DrawAction(new DrawWayAction(dataManager, dataManager.currentActiveObject,
                            segment, previousX, previousY, canvas)));
                    }
                }
                else
                {
                    Node previousPoint = dataManager.currentActiveObject.Nodes.Last();
                    Segment addedSegment = new Segment(previousX, previousY, previousPoint, X, Y, n, true);
                    addedSegment.way = dataManager.currentActiveObject.Way;
                    addedSegment.obj = dataManager.currentActiveObject;
                    dataManager.currentActiveObject.UpdateSegments(addedSegment);
                    dataManager.currentActiveObject.Nodes.Add(n);
                    List<long> nodes = dataManager.currentActiveObject.Way.Nodes.ToList();
                    nodes.Add((long)n.Id);
                    dataManager.currentActiveObject.Way.Nodes = nodes.ToArray();

                    dataManager.SelectedLevel.AddedShapes.Add(addedSegment);
                    dataManager.actionList.AddAction(new DrawAction(new DrawWayAction(dataManager, dataManager.currentActiveObject,
                        addedSegment, previousX, previousY, canvas)));
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
        public static void CloseWay(double X, double Y, DataManager dataManager, LineCanvas canvas)
        {
            Node n = dataManager.currentActiveObject.Nodes.Last();

            double sx, sy;
            (sx, sy) = GpsUtils.LatLonToMeters((double)dataManager.startPoint.Latitude, (double)dataManager.startPoint.Longitude);
            (sx, sy) = GpsUtils.MetersToCanvas(sx, sy);

            Segment addedSegment = new Segment(sx, sy, dataManager.startPoint, X, Y, n, true);

            List<long> nodes = dataManager.currentActiveObject.Way.Nodes.ToList();
            nodes.Add((long)dataManager.startPoint.Id);
            dataManager.currentActiveObject.Way.Nodes = nodes.ToArray();

            ClosedWayObject closedWayObject = new ClosedWayObject(Parser.getNodes(dataManager.Nodes, nodes.ToArray()),
                dataManager.currentActiveObject.Way, true);

            dataManager.actionList.AddAction(new DrawAction(new CloseWayAction(dataManager, dataManager.currentActiveObject, 
                closedWayObject, dataManager.startPoint, X, Y, canvas)));

            dataManager.SelectedLevel.AddObjects(closedWayObject, true);
            dataManager.currentActiveObject = null;
            dataManager.startPoint = null;
            dataManager.SelectedLevel.AddedShapes.Clear();
            dataManager.SelectedLevel.addedElements.Clear();
        }
    }
}

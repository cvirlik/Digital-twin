using Digital_twin.Dataset.Support;
using Digital_twin.Dataset.Types.Secondary;
using System.Collections.ObjectModel;
using OsmSharp;
using Digital_twin.Dataset.Types.Primary;
using System;
using Digital_twin.Dataset.Types.Tertiary;
using Digital_twin.Dataset;
using System.Linq;

namespace Digital_twin.Draw_tools
{
    public class DrawingTools
    {
        public static Tuple<int, int> CenterOffset(ObservableCollection<Node> nodes, double latitude0, double longitude0)
        {
            var mostLeft = nodes.OrderBy(p => p.Longitude).First();
            var mostRight = nodes.OrderByDescending(p => p.Longitude).First();
            var uppermost = nodes.OrderBy(p => p.Latitude).Last();
            var bottommost = nodes.OrderBy(p => p.Latitude).First();
            double xl, yl, hl;
            GpsUtils.GeodeticToEnu((double)mostLeft.Latitude, (double)mostLeft.Longitude, 0, latitude0, longitude0, 0,
                out xl, out yl, out hl);
            double xr, yr, hr;
            GpsUtils.GeodeticToEnu((double)mostRight.Latitude, (double)mostRight.Longitude, 0, latitude0, longitude0, 0,
                out xr, out yr, out hr);
            double xu, yu, hu;
            GpsUtils.GeodeticToEnu((double)uppermost.Latitude, (double)uppermost.Longitude, 0, latitude0, longitude0, 0,
                out xu, out yu, out hu);
            double xb, yb, hb;
            GpsUtils.GeodeticToEnu((double)bottommost.Latitude, (double)bottommost.Longitude, 0, latitude0, longitude0, 0,
                out xb, out yb, out hb);
            return new Tuple<int, int>(500/2 - ((int)xr + (int)xl) / 2 * 3, (400 / 2 - ((int)xu + (int)xb) / 2 * 3));
        }
        public static void SplitToSegments(ObservableCollection<Node> nodes, ObservableCollection<Segment> segments,
                                           double latitude0, double longitude0, bool isInner, CanvasObject refObject)
        { 
            double previousX = -1;
            double previousY = -1;
            double previousLat = -1;
            double previousLon = -1;

            foreach (var node in nodes)
            {
                double x, y, h;
                GpsUtils.GeodeticToEnu((double)(node as Node).Latitude, (double)(node as Node).Longitude, 0, latitude0, longitude0, 0,
                    out x, out y, out h);
                //TODO: Zoom scale.
                x = (x*3 + DataManager.central_offset.Item1);
                y = (1 - y) * 3 + DataManager.central_offset.Item2;
                if (previousX != -1 && previousY != -1)
                {
                    segments.Add(new Segment(previousX, previousY, previousLat, previousLon, x, y, 
                                            (double)(node as Node).Latitude, (double)(node as Node).Longitude, isInner, refObject));
                }
                previousX = x; previousY = y; previousLat = (double)(node as Node).Latitude; previousLon = (double)(node as Node).Longitude;
            }
        }

        public static Point CreatePointFromNode(Node node, double latitude0, double longitude0)
        {
            double x, y, h;
            GpsUtils.GeodeticToEnu((double)(node as Node).Latitude, (double)(node as Node).Longitude, 0, latitude0, longitude0, 0,
                out x, out y, out h);
            //TODO: Zoom scale.
            x = (x*3 + DataManager.central_offset.Item1);
            y = (1 - y) * 3 + DataManager.central_offset.Item2;

            return new Point(x, y, latitude0, longitude0);
        }

        public static void CreatePolygons(ObservableCollection<Node> nodes, Polygon polygon, double latitude0, double longitude0)
        {
            foreach (var node in nodes) 
            {
                polygon.AddVertex(CreatePointFromNode(node, latitude0, longitude0));
            }
        }
    }
}

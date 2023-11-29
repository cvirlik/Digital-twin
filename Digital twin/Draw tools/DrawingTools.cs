using Digital_twin.Dataset.Support;
using Digital_twin.Dataset.Types.Secondary;
using System.Collections.ObjectModel;
using OsmSharp;
using Digital_twin.Dataset.Types.Primary;
using System;

namespace Digital_twin.Draw_tools
{
    public class DrawingTools
    {
        public static void SplitToSegments(ObservableCollection<Node> nodes, ObservableCollection<Segment> segments,
                                           double latitude0, double longitude0, bool isInner)
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
                x = x * 3;
                y = (1 - y) * 3;
                if (previousX != -1 && previousY != -1)
                {
                    segments.Add(new Segment(previousX, previousY, previousLat, previousLon, x, y, 
                                            (double)(node as Node).Latitude, (double)(node as Node).Longitude, isInner));
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
            x = x * 3;
            y = (1 - y) * 3;
            return new Point(x, y, latitude0, longitude0);
        }

        public static void CreatePolygons(ObservableCollection<Node> nodes, Polygon polygon, double latitude0, double longitude0)
        {
            foreach(var node in nodes) 
            {
                polygon.AddVertex(CreatePointFromNode(node, latitude0, longitude0));
            }
        }
    }
}

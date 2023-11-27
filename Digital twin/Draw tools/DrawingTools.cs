using Digital_twin.Dataset.Support;
using Digital_twin.Dataset.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsmSharp;

namespace Digital_twin.Draw_tools
{
    internal class DrawingTools
    {
        public static void SplitToSegments(ObservableCollection<Node> nodes, ObservableCollection<Segment> segments,
                                           double latitude0, double longitude0, bool isInner)
        {
            
            double previousX = -1;
            double previousY = -1;

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
                    segments.Add(new Segment(previousX, previousY, x, y, isInner));
                }
                previousX = x; previousY = y;
            }
        }
    }
}

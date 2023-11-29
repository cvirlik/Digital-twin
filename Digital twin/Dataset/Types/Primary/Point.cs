using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_twin.Dataset.Types.Primary
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Point(double x, double y, double latitude, double longitude)
        {
            X = x;
            Y = y;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}

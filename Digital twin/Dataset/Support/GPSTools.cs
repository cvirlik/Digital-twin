using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_twin.Dataset.Support
{
    // Taken from https://gist.github.com/govert/1b373696c9a27ff4c72a
    public class GpsUtils
    {
        // WGS-84 geodetic constants
        const double a = 6378137;           // WGS-84 Earth semimajor axis (m)
        const double b = 6356752.3142;      // WGS-84 Earth semiminor axis (m)
        const double f = (a - b) / a;       // Ellipsoid Flatness
        const double e_sq = f * (2 - f);    // Square of Eccentricity

        // Converts WGS-84 Geodetic point (lat, lon, h) to the 
        // Earth-Centered Earth-Fixed (ECEF) coordinates (x, y, z).
        public static void GeodeticToEcef(double lat, double lon, double h,
                                          out double x, out double y, out double z)
        {
            // Convert to radians in notation consistent with the paper:
            var lambda = DegreeToRadian(lat);
            var phi = DegreeToRadian(lon);
            var s = Math.Sin(lambda);
            var N = a / Math.Sqrt(1 - e_sq * s * s);

            var sin_lambda = Math.Sin(lambda);
            var cos_lambda = Math.Cos(lambda);
            var cos_phi = Math.Cos(phi);
            var sin_phi = Math.Sin(phi);

            x = (h + N) * cos_lambda * cos_phi;
            y = (h + N) * cos_lambda * sin_phi;
            z = (h + (1 - e_sq) * N) * sin_lambda;
        }

        // Converts the Earth-Centered Earth-Fixed (ECEF) coordinates (x, y, z) to 
        // East-North-Up coordinates in a Local Tangent Plane that is centered at the 
        // (WGS-84) Geodetic point (lat0, lon0, h0).
        public static void EcefToEnu(double x, double y, double z,
                                     double lat0, double lon0, double h0,
                                     out double xEast, out double yNorth, out double zUp)
        {
            // Convert to radians in notation consistent with the paper:
            var lambda = DegreeToRadian(lat0);
            var phi = DegreeToRadian(lon0);
            var s = Math.Sin(lambda);
            var N = a / Math.Sqrt(1 - e_sq * s * s);

            var sin_lambda = Math.Sin(lambda);
            var cos_lambda = Math.Cos(lambda);
            var cos_phi = Math.Cos(phi);
            var sin_phi = Math.Sin(phi);

            double x0 = (h0 + N) * cos_lambda * cos_phi;
            double y0 = (h0 + N) * cos_lambda * sin_phi;
            double z0 = (h0 + (1 - e_sq) * N) * sin_lambda;

            double xd, yd, zd;
            xd = x - x0;
            yd = y - y0;
            zd = z - z0;

            // This is the matrix multiplication
            xEast = -sin_phi * xd + cos_phi * yd;
            yNorth = -cos_phi * sin_lambda * xd - sin_lambda * sin_phi * yd + cos_lambda * zd;
            zUp = cos_lambda * cos_phi * xd + cos_lambda * sin_phi * yd + sin_lambda * zd;
        }

        // Converts the geodetic WGS-84 coordinated (lat, lon, h) to 
        // East-North-Up coordinates in a Local Tangent Plane that is centered at the 
        // (WGS-84) Geodetic point (lat0, lon0, h0).
        public static void GeodeticToEnu(double lat, double lon, double h,
                                         double lat0, double lon0, double h0,
                                         out double xEast, out double yNorth, out double zUp)
        {
            double x, y, z;
            GeodeticToEcef(lat, lon, h, out x, out y, out z);
            EcefToEnu(x, y, z, lat0, lon0, h0, out xEast, out yNorth, out zUp);
        }


        private static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private const double originShift = 2 * Math.PI * 6378137 / 2.0;
        private const double initialResolution = 2 * Math.PI * 6378137 / 256;
        public static (double, double) LatLonToMeters(double lat, double lon)
        {
            double mx = lon * originShift / 180.0;
            double my = Math.Log(Math.Tan((90 + lat) * Math.PI / 360.0)) / (Math.PI / 180.0);
            my = my * originShift / 180.0;
            return (mx, my);
        }

        public static (double, double) MetersToLatLon(double mx, double my)
        {
            double lon = (mx / originShift) * 180.0;
            double lat = (my / originShift) * 180.0;
            lat = 180 / Math.PI * (2 * Math.Atan(Math.Exp(lat * Math.PI / 180.0)) - Math.PI / 2.0);
            return (lat, lon);
        }

        private const double earthCircumferenceMeters = 2 * Math.PI * 6378137;
        // TODO: adaptive
        private static double canvasWidth = 500;
        private static double canvasHeight = 400;

        public static (double, double) MetersToCanvas(double mx, double my)
        {
            double x = ((mx - DataManager.minX) / (DataManager.maxX - DataManager.minX)) * canvasWidth;
            double y = canvasHeight - ((my - DataManager.minY) / (DataManager.maxY - DataManager.minY)) * canvasHeight;
            return (x, y);
        }

        public static (double, double) CanvasToMeters(double x, double y)
        {
            double mx = x / canvasWidth * (DataManager.maxX - DataManager.minX) + DataManager.minX;
            double my = (canvasHeight - y) / canvasHeight * (DataManager.maxY - DataManager.minY) + DataManager.minY;
            return (mx, my);
        }
    }
}

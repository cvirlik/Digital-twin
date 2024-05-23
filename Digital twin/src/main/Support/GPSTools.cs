using System;

namespace Digital_twin.Dataset.Support
{
    public class GpsUtils
    {
        private const double originShift = 2 * Math.PI * 6378137 / 2.0;
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

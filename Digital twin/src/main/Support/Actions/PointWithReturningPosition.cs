using Digital_twin.Dataset.Types.Primary;

namespace Digital_twin.Dataset.Support.Actions
{
    public class PointWithReturningPosition
    {
        public Point point;
        public double pX;
        public double pY;

        public PointWithReturningPosition(Point p, double X, double Y) 
        { 
            point = p;
            pX = X;
            pY = Y;
        } 
    }
}

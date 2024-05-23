using Digital_twin.Dataset.Types.Canvas;
using Digital_twin.Dataset.Types.Primary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Digital_twin.Dataset.Support.Actions
{
    public class MoveAction : Action
    {
        ObservableCollection<PointWithReturningPosition> points = new ObservableCollection<PointWithReturningPosition>();

        private List<int> idxPolygon = new List<int>();
        private ObservableCollection<PointWithReturningPosition> pointsPolygon = new ObservableCollection<PointWithReturningPosition>();
        private ClosedWayObject wayObject;
        private bool polygon = false;
        private bool finishCycle = false;
        public MoveAction() : base()
        {
            ActionType = "Move";
        }

        public void AddEvent(Point p, double X, double Y)
        {
            if (points.FirstOrDefault(t => t.point == p) == null)
            {
                points.Add(new PointWithReturningPosition(p, X, Y));
            }
        }
        public void SetPolygonInfo(ClosedWayObject obj, int i, double X, double Y)
        {
            if (!finishCycle)
            {
                Console.WriteLine("Write");
                idxPolygon.Add(i);
                wayObject = obj;
                pointsPolygon.Add(new PointWithReturningPosition(null, X, Y));
                polygon = true;
            
            }
        }

        public void SetFinish()
        {
            finishCycle = true;
        }

        public override void Undo()
        {
            foreach (PointWithReturningPosition point in points)
            {
                point.point.X = point.pX;
                point.point.Y = point.pY;
                point.point.UpdateNodeCoordinates();
            }
            if(polygon)
            {
                for(int i = 0; i < pointsPolygon.Count; i++)
                {
                    wayObject.Polygon.UpdateVertex(idxPolygon[i], pointsPolygon[i].pX, pointsPolygon[i].pY);
                }
            }
        }
    }
}

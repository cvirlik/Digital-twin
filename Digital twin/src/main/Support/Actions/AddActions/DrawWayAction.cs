using Digital_twin.Dataset.Types.Canvas;
using Digital_twin.Dataset.Types.Primary;
using Digital_twin.UserControls;
using System;
using System.Collections.Generic;

namespace Digital_twin.Dataset.Support.Actions.AddActions
{
    public class DrawWayAction : ObjectTransform
    {
        private DataManager dataManager;
        private IShape segment;
        private LineCanvas lineCanvas;
        private OpenedWayObject wayObject;
        private double X;
        private double Y;
        public DrawWayAction(DataManager _dataManager, OpenedWayObject _wayObject, IShape _segment, double x, double y, LineCanvas canvas)
        {
            segment = _segment;
            dataManager = _dataManager;
            lineCanvas = canvas;
            wayObject = _wayObject;
            X = x;
            Y = y;  
        }

        public override void ReturnObject()
        {
            List<long> points = new List<long>(wayObject.Way.Nodes);
            points.RemoveAt(points.Count - 1);
            wayObject.Way.Nodes = new List<long>(points).ToArray();
            wayObject.ReloadSegments(Parser.getNodes(dataManager.Nodes, points.ToArray()));

            if (dataManager.SelectedLevel.addedElements.Contains(wayObject))
            {
                Console.WriteLine("Delete");
                dataManager.SelectedLevel.AddedShapes.Remove(segment);
                lineCanvas.SetXY(X, Y);
                if(points.Count == 0)
                {
                    dataManager.SelectedLevel.DeleteActiveObject(wayObject);
                    lineCanvas.ResetDrawing();
                }
            } else if (dataManager.SelectedLevel.canvasObjects.Contains(wayObject))
            {
                dataManager.SelectedLevel.shapes.Remove(segment);
                if (points.Count == 0)
                {
                    dataManager.SelectedLevel.DeleteObject(wayObject);
                    dataManager.Ways.Remove(wayObject.Way);
                }
            }
        }
    }
}

using Digital_twin.Dataset.Types.Canvas;
using Digital_twin.UserControls;
using OsmSharp;

namespace Digital_twin.Dataset.Support.Actions.AddActions
{
    public class CloseWayAction : ObjectTransform
    {
        private DataManager dataManager;
        private LineCanvas lineCanvas;
        private OpenedWayObject openWayObject;
        private ClosedWayObject closedWayObject;
        private Node n;
        private double X;
        private double Y;
        public CloseWayAction(DataManager _dataManager, OpenedWayObject _openWayObject, ClosedWayObject _closedWayObject,
                              Node startPoint, double x, double y, LineCanvas canvas)
        {
            dataManager = _dataManager;
            lineCanvas = canvas;
            openWayObject = _openWayObject;
            closedWayObject = _closedWayObject; 
            n = startPoint;
            X = x;
            Y = y;
        }

        public override void ReturnObject()
        {

            if (dataManager.currentActiveObject == null)
            {
                lineCanvas.SetXY(X, Y);
                dataManager.currentActiveObject = openWayObject;  
                dataManager.startPoint = n;
            }

            dataManager.Ways.Remove(closedWayObject.Way);
            dataManager.Ways.Add(openWayObject.Way);

            dataManager.SelectedLevel.DeleteObject(closedWayObject);
            dataManager.SelectedLevel.AddObjects(openWayObject, true);
        }
    }
}

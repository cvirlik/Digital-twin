using Digital_twin.Dataset.Types.Primary;
using Digital_twin.UserControls;

namespace Digital_twin.Dataset.Support.Actions.AddActions
{
    public class RemovePlaceholder : ObjectTransform
    {
        private Point point;
        private DataManager dataManager;
        private LineCanvas lineCanvas;
        public RemovePlaceholder(Point p, DataManager _dataManager, LineCanvas canvas)
        {
            point = p;
            dataManager = _dataManager;
            lineCanvas = canvas;
        }

        public override void ReturnObject()
        {
            dataManager.SelectedLevel.addedShapes.Remove(point);
            dataManager.startPoint = null;
            lineCanvas.ResetDrawing();
        }
    }
}

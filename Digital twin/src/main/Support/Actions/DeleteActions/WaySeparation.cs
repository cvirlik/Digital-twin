using Digital_twin.Dataset.Types.Canvas;

namespace Digital_twin.Dataset.Support.Actions.Transform
{
    public class WaySeparation : ObjectTransform
    {
        private DataManager dataManager;
        private CanvasObject previousWay;
        private CanvasObject newWayFirst;
        private CanvasObject newWaySecond;
        public WaySeparation(CanvasObject _previousWay, CanvasObject _newWayFirst, CanvasObject _newWaySecond, DataManager _dataManager)
        {
            previousWay = _previousWay;
            newWayFirst = _newWayFirst;
            newWaySecond = _newWaySecond;
            dataManager = _dataManager;
        }
        public override void ReturnObject()
        {
            dataManager.SelectedLevel.DeleteObject(newWayFirst);
            if(newWaySecond != null) { dataManager.SelectedLevel.DeleteObject(newWaySecond); }
            dataManager.SelectedLevel.AddObjects(previousWay, true);
        }
    }
}

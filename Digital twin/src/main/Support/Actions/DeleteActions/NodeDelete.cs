using Digital_twin.Dataset.Types.Primary;

namespace Digital_twin.Dataset.Support.Actions.Transform
{
    public class NodeDelete : ObjectTransform
    {
        private Point point;
        private DataManager dataManager;
        public NodeDelete(Point p, DataManager _dataManager) 
        {
            point = p;
            dataManager = _dataManager;
        }

        public override void ReturnObject()
        {
            dataManager.Nodes.Add(point.node);
            dataManager.SelectedLevel.AddObjects(point.obj, true);
        }
    }
}

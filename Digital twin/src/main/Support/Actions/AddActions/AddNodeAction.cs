using Digital_twin.Dataset.Types.Canvas;

namespace Digital_twin.Dataset.Support.Actions.AddActions
{
    public class AddNodeAction : ObjectTransform
    {
        private DataManager dataManager;
        private NodeObject createdNodeObject;
        public AddNodeAction(NodeObject obj, DataManager _dataManager) 
        {
            dataManager = _dataManager;
            createdNodeObject = obj;
        }
        public override void ReturnObject()
        {
            dataManager.SelectedLevel.DeleteObject(createdNodeObject);
            dataManager.Nodes.Remove(createdNodeObject.Node);
        }
    }
}

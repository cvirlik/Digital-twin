using Digital_twin.Dataset.Types.Canvas;
using System.Collections.Generic;

namespace Digital_twin.Dataset.Support.Actions.Transform
{
    public class WayCut : ObjectTransform
    {
        private DataManager dataManager;
        private OpenedWayObject obj;
        private List<long> originalNodes;
        public WayCut(OpenedWayObject _changed, List<long> _originalNodes, DataManager _dataManager)
        {
            dataManager = _dataManager;
            obj = _changed;
            originalNodes = _originalNodes;
        }
        public override void ReturnObject()
        {
            dataManager.SelectedLevel.DeleteObject(obj);
            obj.Way.Nodes = originalNodes.ToArray();
            obj.ReloadSegments(Parser.getNodes(dataManager.Nodes, obj.Way.Nodes));
            dataManager.SelectedLevel.AddObjects(obj, true);
        }
    }
}

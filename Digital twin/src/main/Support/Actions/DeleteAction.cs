namespace Digital_twin.Dataset.Support.Actions
{
    public class DeleteAction : Action
    {
        private ObjectTransform Object;
        public DeleteAction(ObjectTransform obj) : base()
        {
            Object = obj;
            ActionType = "Delete";
        }
        public override void Undo()
        {
            Object.ReturnObject();
        }
    }
}

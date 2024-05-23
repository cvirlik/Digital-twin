namespace Digital_twin.Dataset.Support.Actions
{
    public class DrawAction : Action
    {
        private ObjectTransform Object;
        public DrawAction(ObjectTransform obj) : base()
        {
            Object = obj;
            ActionType = "Draw";
        }
        public override void Undo()
        {
            Object.ReturnObject();
        }
    }
}

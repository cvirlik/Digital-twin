namespace Digital_twin.Dataset.Support.Actions
{
    public abstract class Action : ViewModelBase
    {
        private string _actionType;
        public Action() 
        {
        }

        public string ActionType
        {
            get { return _actionType; }
            set { 
                _actionType = value; 
                OnPropertyChanged(nameof(ActionType));
            }
        }
        public abstract void Undo();
    }
}

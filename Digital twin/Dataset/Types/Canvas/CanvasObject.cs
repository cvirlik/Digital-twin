using System.Collections.ObjectModel;
using Digital_twin.Dataset.Types.Primary;
using Digital_twin.Dataset.Types.Secondary;

namespace Digital_twin.Dataset.Types.Canvas
{
    public abstract class CanvasObject : ViewModelBase
    {
        public ObservableCollection<Tag> Tags { get; set; } = new ObservableCollection<Tag>();
        protected ObservableCollection<IShape> shapes = new ObservableCollection<IShape>();

        private bool _IsSelected = false;

        public bool IsSelected
        {
            get { return _IsSelected; }
            set { 
                _IsSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }



        public ObservableCollection<IShape> Shapes
        {
            get
            {
                return shapes;
            }
        }
    }
}

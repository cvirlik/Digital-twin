using Digital_twin.Dataset.Types.Primary;
using Digital_twin.Dataset.Types.Tertiary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Digital_twin.Dataset.Types.Secondary
{
    public class Polygon : ViewModelBase, IShape
    {
        public List<Point> vertices { get; set; }
        public PointCollection Vertices { get; set; }
        public bool IsInner { get; set; }
        private CanvasObject _canvasObject;
        private bool _isSelected;

        public Polygon(CanvasObject refObject, bool isInner)
        {
            vertices = new List<Point>();
            Vertices = new PointCollection();
            _canvasObject = refObject;
            IsInner = isInner;
        }
        public ObservableCollection<Tag> Tags
        {
            get { return _canvasObject.Tags; }
            set { }
        }

        public void AddVertex(Point point)
        {
            vertices.Add(point);
            Vertices.Add(new System.Windows.Point(point.X, point.Y));
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }
    }
}

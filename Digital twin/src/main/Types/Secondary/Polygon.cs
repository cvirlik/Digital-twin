using Digital_twin.Dataset.Types.Primary;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Digital_twin.Dataset.Types.Secondary
{
    public class Polygon : IShape
    {
        public List<Point> vertices { get; set; }

        private ObservableCollection<System.Windows.Point> _vertices;
        public ObservableCollection<System.Windows.Point> Vertices
        {
            get { return _vertices; }
            set
            {
                _vertices = value;
                OnPropertyChanged(nameof(Vertices));
            }
        }
        public void UpdateVertices()
        {
            OnPropertyChanged(nameof(Vertices));
        }
        public bool IsInner { get; set; }
        private bool _isSelected;

        public Polygon(bool isInner)
        {
            vertices = new List<Point>();
            Vertices = new ObservableCollection<System.Windows.Point>();
            IsInner = isInner;
        }
        
        public ObservableCollection<Tag> Tags
        {
            get { return obj.Tags; }
            set { }
        }

        public void AddVertex(Point point)
        {
            vertices.Add(point);
            Vertices.Add(new System.Windows.Point(point.X, point.Y));
        }

        public void UpdateVertex(int i, double x, double y)
        {
            Vertices[i] = new System.Windows.Point(x, y);
            UpdateVertices();
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

using Digital_twin.Dataset.Types.Primary;
using System.Collections.Generic;
using System.Windows.Media;

namespace Digital_twin.Dataset.Types.Secondary
{
    public class Polygon : ViewModelBase, IShape
    {
        public List<Point> vertices { get; set; }
        public PointCollection Vertices { get; set; }
        private bool _isSelected;

        public Polygon()
        {
            vertices = new List<Point>();
            Vertices = new PointCollection();
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

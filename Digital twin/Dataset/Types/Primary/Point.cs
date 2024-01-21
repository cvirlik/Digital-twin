using Digital_twin.Dataset.Types.Canvas;
using Digital_twin.Dataset.Types.Secondary;
using OsmSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_twin.Dataset.Types.Primary
{
    public class Point : ViewModelBase, IShape
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Node node { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public CanvasObject obj { get; set; }
        public ObservableCollection<Tag> Tags
        {
            get { return obj.Tags; }
            set { }
        }
            

        public Point(double x, double y, Node _node)
        {
            X = x;
            Y = y;
            node = _node;
            Latitude = (double)node.Latitude;
            Longitude = (double)node.Longitude;
        }
    }
}

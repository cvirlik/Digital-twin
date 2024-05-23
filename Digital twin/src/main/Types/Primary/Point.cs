using Digital_twin.Dataset.Support;
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
    public class Point : IShape
    {
        private double x;
        public double X
        {
            get { return x; }
            set
            {
                x = value; 
                OnPropertyChanged(nameof(X));
            }
        }
        private double y;
        public double Y
        {
            get { return y; }
            set
            {
                y = value; 
                OnPropertyChanged(nameof(Y));
            }
        }
        public void UpdateNodeCoordinates()
        {
            double mx, my;
            (mx, my) = GpsUtils.CanvasToMeters(x, y);
            (node.Latitude, node.Longitude) = GpsUtils.MetersToLatLon(mx, my);
        }
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

        public ObservableCollection<Tag> Tags
        {
            get { return obj.Tags; }
            set { }
        }
            

        public Point(double _x, double _y, Node _node)
        {
            x = _x;
            y = _y;
            node = _node;
            Latitude = (double)node.Latitude;
            Longitude = (double)node.Longitude;
        }
    }
}

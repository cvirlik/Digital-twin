using Digital_twin.Dataset.Types.Primary;
using Digital_twin.Dataset.Types.Canvas;
using OsmSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_twin.Dataset.Types.Secondary
{
    public class Segment : IShape
    {
        public Point Point1 { get; set; }
        public Point Point2 { get; set; }
        public bool IsInner { get; set; }
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

        public Way way { get; set; }

        public ObservableCollection<Tag> Tags
        {
            get { return obj.Tags; }
            set { }
        }

        public Segment(double x1, double y1, Node node1,
                       double x2, double y2, Node node2, bool isInner)
        {
            Point1 = new Point(x1, y1, node1);
            Point2 = new Point(x2, y2, node2);
            IsInner = isInner;
        }
    }
}

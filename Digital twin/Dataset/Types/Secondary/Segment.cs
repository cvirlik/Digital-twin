﻿using Digital_twin.Dataset.Types.Primary;
using Digital_twin.Dataset.Types.Tertiary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_twin.Dataset.Types.Secondary
{
    public class Segment : ViewModelBase, IShape
    {
        public Point Point1 { get; set; }
        public Point Point2 { get; set; }
        public bool IsInner { get; set; }
        private CanvasObject _canvasObject;
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
            get { return _canvasObject.Tags; }
            set { }
        }

        public Segment(double x1, double y1, 
                       double lat1, double lon1,
                       double x2, double y2,
                       double lat2, double lon2, bool isInner, CanvasObject refObject)
        {
            Point1 = new Point(x1, y1, lat1, lon1);
            Point2 = new Point(x2, y2, lat2, lon2);
            IsInner = isInner;
            _canvasObject = refObject;
        }
    }
}

﻿using Digital_twin.Dataset.Types.Secondary;
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
        public ObservableCollection<Tag> Tags { get; set; }
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

        public Point(double x, double y, double latitude, double longitude)
        {
            X = x;
            Y = y;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}

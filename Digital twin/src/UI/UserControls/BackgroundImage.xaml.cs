﻿using Digital_twin.Dataset;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Digital_twin.UserControls
{
    public partial class BackgroundImage : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
         "Angle",
         typeof(double),
         typeof(BackgroundImage),
         new PropertyMetadata(default(double), OnAngleChanged));

        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var backgroundImage = (BackgroundImage)d;
            backgroundImage.OnPropertyChanged("FinalRotateAngle");
        }
        public double FinalRotateAngle
        {
            get {
                return Angle + PrimaryAngle; }
            set
            {
                PrimaryAngle = value;
                OnPropertyChanged("FinalRotateAngle");
            }
        }
        private double PrimaryAngle;
        public BackgroundImage()
        {
            InitializeComponent();
            GridHeight = 400;
            GridWidth = 500;
            ImageHeight = 400;
            ImageWidth = 500;
        }

        private Point _startPoint;
        private Point _startSize;
        private Point _mouseOffset;
        private double _previousAngle;

        private bool _isResizing = false;
        private double[] ratio = new double[2];
        private TranslateTransform _activeTransform;

        private void Image_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(MainGrid);
            _startSize = new Point(ImageWidth, ImageHeight);
            _mouseOffset = e.GetPosition(MainImage);
            _isResizing = true;
            ratio[0] = ImageWidth <= ImageHeight ? 1 : ImageWidth / ImageHeight;
            ratio[1] = ImageHeight <= ImageWidth ? 1 : ImageHeight / ImageWidth;
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            var currentPoint = e.GetPosition(MainGrid);
            if (_isResizing && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
            {
                double diffX = Math.Abs(_startPoint.X - currentPoint.X);
                double diffY = Math.Abs(_startPoint.Y - currentPoint.Y);
                double diff = Math.Max(diffX, diffY);

                double change = Math.Sqrt(2 * Math.Pow(diff, 2));
                double changeX = change * ratio[0];
                double changeY = change * ratio[1];

                Point center = new Point(GridWidth / 2, GridHeight / 2);

                double initialCenterDiff = Math.Sqrt(Math.Pow(_startPoint.X - center.X, 2) + Math.Pow(_startPoint.Y - center.Y, 2));
                double currentCenterDiff = Math.Sqrt(Math.Pow(currentPoint.X - center.X, 2) + Math.Pow(currentPoint.Y - center.Y, 2));


                int sign = currentCenterDiff > initialCenterDiff ? 1 : -1;
                //double limit = Math.Min(GridWidth, GridHeight);

                ImageWidth = Math.Min(GridWidth, _startSize.X + changeX * sign);
                ImageHeight = Math.Min(GridHeight, _startSize.Y + changeY * sign);
            }
            if (_isResizing && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                double newX = currentPoint.X - ImageWidth / 2;
                double newY = currentPoint.Y - ImageHeight / 2;
                if (_activeTransform == null)
                {
                    _activeTransform = new TranslateTransform(newX, newY);

                    if (MainImage.RenderTransform is TransformGroup group)
                    {
                        group.Children.Add(_activeTransform);
                    }
                    else
                    {
                        TransformGroup newGroup = new TransformGroup();
                        newGroup.Children.Add(MainImage.RenderTransform);
                        newGroup.Children.Add(_activeTransform);
                        MainImage.RenderTransform = newGroup;
                    }
                }
                else
                {
                    // Update the existing TranslateTransform
                    _activeTransform.X = newX;
                    _activeTransform.Y = newY;
                }
            }
            if (_isResizing && Keyboard.IsKeyDown(Key.R))
            {
                var vectorStart = Point.Subtract(_startPoint, new Point(GridWidth / 2, GridHeight / 2));
                var vectorEnd = Point.Subtract(currentPoint, new Point(GridWidth / 2, GridHeight / 2));

                double angle = Vector.AngleBetween(vectorStart, vectorEnd);

                if (MainImage.RenderTransform is TransformGroup group)
                {

                    // Find the RotateTransform in the group
                    foreach (var child in group.Children)
                    {
                        if (child is RotateTransform rotate)
                        {
                            angle += rotate.Angle - Angle;
                            break;
                        }
                    }
                }

                FinalRotateAngle = angle;
               
                _previousAngle = angle;
                _startPoint = currentPoint;
            }
        }

        private void StopResizing()
        {
            _isResizing = false;
            _previousAngle = 0;
        }
        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            StopResizing();
        }


        private double _gridHeight;
        public double GridHeight
        {
            get { return _gridHeight; }
            set
            {
                _gridHeight = value;
                OnPropertyChanged("GridHeight");
            }
        }

        private double _gridWidth;
        public double GridWidth
        {
            get { return _gridWidth; }
            set
            {
                _gridWidth = value;
                OnPropertyChanged("GridWidth");
            }
        }

        private double _imageHeight;
        public double ImageHeight
        {
            get { return _imageHeight; }
            set
            {
                _imageHeight = value;
                OnPropertyChanged("ImageHeight");
            }
        }

        private double _imageWidth;
        public double ImageWidth
        {
            get { return _imageWidth; }
            set
            {
                _imageWidth = value;
                OnPropertyChanged("ImageWidth");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            StopResizing();
        }
    }
}

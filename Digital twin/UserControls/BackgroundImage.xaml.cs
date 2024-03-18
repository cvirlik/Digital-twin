using Digital_twin.Dataset;
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
        public BackgroundImage()
        {
            InitializeComponent();
            GridHeight = 400;
            GridWidth = 500;
            ImageHeight = 400;
            ImageWidth = 500;
        }

        private Point _startPoint;
        private Point _mouseOffset;
        private double _previousAngle;

        private bool _isResizing;
        private double[] ratio = new double[2];

        private void Image_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(MainGrid);
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
                double diffX = _startPoint.X - currentPoint.X;
                double diffY = _startPoint.Y - currentPoint.Y;
                double diff = Math.Abs(Math.Min(diffX, diffY));
                double change = Math.Sqrt(2 * Math.Pow(diff, 2)) / 100;
                double changeX = change * ratio[0];
                double changeY = change * ratio[1];

                Point center = new Point(GridWidth / 2, GridHeight / 2);

                double initialCenterDiff = Math.Sqrt(Math.Pow(_startPoint.X - center.X, 2) + Math.Pow(_startPoint.Y - center.Y, 2));
                double currentCenterDiff = Math.Sqrt(Math.Pow(currentPoint.X - center.X, 2) + Math.Pow(currentPoint.Y - center.Y, 2));


                int sign = currentCenterDiff > initialCenterDiff ? 1 : -1;

                ImageWidth = Math.Max(10, Math.Min(ImageWidth + changeX * sign, GridWidth));
                ImageHeight = Math.Max(10, Math.Min(ImageHeight + changeY * sign, GridHeight));
            }
            if (_isResizing && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                double newX = currentPoint.X - ImageWidth / 2;
                double newY = currentPoint.Y - ImageHeight / 2;
                MainImage.Margin = new Thickness(newX, newY, 0, 0);
            }
            if (_isResizing && Keyboard.IsKeyDown(Key.R))
            {
                var vectorStart = Point.Subtract(_startPoint, new Point(GridWidth / 2, GridHeight / 2));
                var vectorEnd = Point.Subtract(currentPoint, new Point(GridWidth / 2, GridHeight / 2));

                double angle = Vector.AngleBetween(vectorStart, vectorEnd);
                double scale = 0.01;
                angle *= scale;

                RotateTransform rotateTransform = MainImage.RenderTransform as RotateTransform;
                if (rotateTransform != null)
                {
                    angle += rotateTransform.Angle;
                }
                MainImage.RenderTransformOrigin = new Point(0.5, 0.5);
                MainImage.RenderTransform = new RotateTransform(angle);
                _previousAngle = angle;
            }
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isResizing = false;
            _previousAngle = 0;
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

        
    }
}

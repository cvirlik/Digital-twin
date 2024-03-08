using Digital_twin.Dataset;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Digital_twin.UserControls
{
    public partial class LineCanvas : UserControl
    {
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
        "Angle", typeof(double), typeof(LineCanvas), new PropertyMetadata(default(double)));

        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        DataManager dataManager;
        public LineCanvas()
        {
            InitializeComponent();
        }
        double pX = -1; double pY = -1;
        double? startX, startY;
        private void DrawWay(DataManager dataManager, Point position)
        {
            if (!startX.HasValue)
            {
                startX = position.X;
                startY = position.Y;
            }

            if (pX == startX && pY == startY)
            {
                dataManager.AddWay(position.X, position.Y, pX, pY, false);
                pX = position.X;
                pY = position.Y;
            }
            else
            {
                dataManager.AddWay(position.X, position.Y, pX, pY, true);
                pX = position.X;
                pY = position.Y;
            }
        }
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(CanvasListBox);

            if (dataManager == null) dataManager = this.DataContext as DataManager;

            if (dataManager == null)
            {
                Console.WriteLine("Cannot access DataManager");
            }
            else
            {
                DrawWay(dataManager, position);
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == UIElement.VisibilityProperty && e.NewValue is Visibility visibility && visibility == Visibility.Collapsed)
            {
                OnVisibilityCollapsed();
            }
        }

        private void OnVisibilityCollapsed()
        {
            pX = -1; pY = -1;
            startX = null; startY = null;
        }
    }
}

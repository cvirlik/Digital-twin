using Digital_twin.Dataset;
using Digital_twin.Draw_tools;
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
                DrawingTools.AddWay(position.X, position.Y, pX, pY, false, dataManager);
                pX = position.X;
                pY = position.Y;
            }
            else
            {
                DrawingTools.AddWay(position.X, position.Y, pX, pY, true, dataManager);
                pX = position.X;
                pY = position.Y;
            }
        }

        private void Line_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Click on closing line");
            DrawingTools.CloseWay(pX, pY, dataManager);
            pX = -1; pY = -1;
            startX = null; startY = null;
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
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                pX = -1; pY = -1;
                startX = null; startY = null;
            }
        }
        private void TheGrid_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (theGrid.IsVisible)
            {
                theGrid.Focus();
            }
        }
    }
}

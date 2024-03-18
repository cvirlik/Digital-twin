using Digital_twin.Dataset.Types.Primary;
using Digital_twin.Dataset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Digital_twin.Dataset.Support;

namespace Digital_twin.UserControls
{
    public partial class MoveCanvas : UserControl
    {
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
       "Angle", typeof(double), typeof(MoveCanvas), new PropertyMetadata(default(double)));
        public IShape editingShape
        {
            get;
            set;
        }

        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }
        public MoveCanvas()
        {
            InitializeComponent();
        }
        DataManager dataManager;
        private bool isDragging = false;
        private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {

            DependencyObject parent = VisualTreeHelper.GetParent(child);
            if (parent == null) return null;

            if (parent is T parentOfType)
                return parentOfType;

            return FindVisualParent<T>(parent);
        }

        private void UpdateEditingItem(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem clickedItem = FindVisualParent<ListBoxItem>((DependencyObject)e.OriginalSource);

            if (clickedItem != null)
            {
                editingShape = (IShape)clickedItem.Content;
            }
            else
            {
                editingShape = null;
            }
        }
        private bool addedControlPoints;
        private void DropControlPoints()
        {
            if (addedControlPoints)
            {
                dataManager.SelectedLevel.Shapes.RemoveAt(dataManager.SelectedLevel.Shapes.Count - 1);
                dataManager.SelectedLevel.Shapes.RemoveAt(dataManager.SelectedLevel.Shapes.Count - 1);
            }
            addedControlPoints = false;
        }
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (dataManager == null) dataManager = this.DataContext as DataManager;
            UpdateEditingItem(sender, e);
            DropControlPoints();
            if (editingShape is Dataset.Types.Secondary.Segment segment)
            {
                addedControlPoints = true;
                dataManager.SelectedLevel.Shapes.Add(segment.Point1);
                dataManager.SelectedLevel.Shapes.Add(segment.Point2);
            }
            isDragging = true;
            e.Handled = true;

            
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            if (editingShape is Dataset.Types.Primary.Point point)
            {
                double X, Y;
                (X, Y) = GpsUtils.CanvasToMeters(point.X, point.Y);
                (point.node.Latitude, point.node.Longitude) = GpsUtils.MetersToLatLon(X, Y);
            }
            editingShape = null;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                if (editingShape is Dataset.Types.Primary.Point point)
                {
                    var position = e.GetPosition(CanvasListBox);
                    point.X = position.X - 5;
                    point.Y = position.Y - 5;
                }
            }
        }
    }
}

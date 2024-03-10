using Digital_twin.Dataset;
using Digital_twin.Dataset.Types.Primary;
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

namespace Digital_twin.UserControls
{
    public partial class EditCanvas : UserControl
    {
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
        "Angle", typeof(double), typeof(EditCanvas), new PropertyMetadata(default(double)));


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

        public EditCanvas()
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
            } else
            {
               editingShape = null;
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UpdateEditingItem(sender, e);
            Console.WriteLine("Mouse Down");
            isDragging = true;
            

            //Mouse.Capture(CanvasListBox);

            if (dataManager == null) dataManager = this.DataContext as DataManager;
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Mouse Up");
            isDragging = false;
            editingShape = null;
            //Mouse.Capture(null);
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

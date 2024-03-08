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

namespace Digital_twin.UserControls
{
    /// <summary>
    /// Interaction logic for EditCanvas.xaml
    /// </summary>
    public partial class EditCanvas : UserControl
    {
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
        "Angle", typeof(double), typeof(EditCanvas), new PropertyMetadata(default(double)));

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
        private Point lastPosition;



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
                isDragging = true;
                lastPosition = e.GetPosition(CanvasListBox);
                Mouse.Capture(CanvasListBox);
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            Mouse.Capture(null);
        }

        // TODO: Finish movement
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
            }
        }
    }
}

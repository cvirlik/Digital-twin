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
    /// Interaction logic for PointCanvas.xaml
    /// </summary>
    public partial class PointCanvas : UserControl
    {
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
        "Angle", typeof(double), typeof(PointCanvas), new PropertyMetadata(default(double)));

        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        DataManager dataManager;
        public PointCanvas()
        {
            InitializeComponent();
        }
        private void DrawPoint(DataManager dataManager, Point position)
        {
            dataManager.AddNode(position.X, position.Y);
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
                DrawPoint(dataManager, position);
            }
        }
    }
}

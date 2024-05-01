using Digital_twin.Dataset;
using Digital_twin.Dataset.Types.Primary;
using Digital_twin.Draw_tools;
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

        DataManager dataManager;
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }
        public EditCanvas()
        {
            InitializeComponent();
        }
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (dataManager == null) dataManager = this.DataContext as DataManager;
                dataManager.DeleteElement();
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

using Digital_twin.Dataset;
using System.Windows;
using System.Windows.Controls;

namespace Digital_twin.UserControls
{
    /// <summary>
    /// Interaction logic for ImageTransform.xaml
    /// </summary>
    public partial class ImageTransform : UserControl
    {
        DataManager dataManager;
        public ImageTransform()
        {
            InitializeComponent();
        }
        private void Transform(object sender, RoutedEventArgs e)
        {
            if (dataManager == null) dataManager = this.DataContext as DataManager;
            dataManager.State = "ImageTransform";
        }
    }
}

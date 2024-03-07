using Digital_twin.Dataset.Support;
using Microsoft.Win32;
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
    public partial class ImageControl : UserControl
    {
        public static readonly DependencyProperty EnableCommandProperty =
        DependencyProperty.Register("EnableCommand", typeof(ICommand), typeof(ImageControl), new PropertyMetadata(null));

        public ICommand EnableCommand
        {
            get { return (ICommand)GetValue(EnableCommandProperty); }
            set { SetValue(EnableCommandProperty, value); }
        }

        public ImageControl()
        {
            InitializeComponent();
        }

        private void Enable(object sender, RoutedEventArgs e)
        {
            if (EnableCommand != null && EnableCommand.CanExecute(null))
            {
                EnableCommand.Execute(null);
            }
        }
    }
}

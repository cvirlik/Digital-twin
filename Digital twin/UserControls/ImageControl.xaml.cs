using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

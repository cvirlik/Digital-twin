using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace Digital_twin.UserControls
{
    /// <summary>
    /// Interaction logic for Search.xaml
    /// </summary>
    public partial class Search : UserControl
    {
        public Search()
        {
            InitializeComponent();
        }
        private void Browse(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                FileName = openFileDialog.FileName;
            }
        }

        public string FileName
        {
            get
            {
                return (string)GetValue(FileNameProperty);
            }
            set
            {
                SetValue(FileNameProperty, value);
            }
        }

        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register(nameof(FileName), typeof(string), typeof(Search));

        private void InputChanged(object sender, TextChangedEventArgs e)
        {
            e.Handled = true;
            RoutedEventArgs args = new RoutedEventArgs(FileNameChangedEvent);
            RaiseEvent(args);
        }

        public event RoutedEventHandler FileNameChanged;

        public static readonly RoutedEvent FileNameChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(FileNameChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Search));
    }
}

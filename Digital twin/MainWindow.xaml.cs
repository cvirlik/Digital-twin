using System;
using System.Collections;
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
using Digital_twin.Dataset;
using Digital_twin.Dataset.Support.Actions;

namespace Digital_twin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataManager dataManager;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Undo(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (dataManager == null) dataManager = this.DataContext as DataManager;
                dataManager.actionList.DeleteAction();
            }
        }
    }
}

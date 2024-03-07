using Digital_twin.Dataset;
using Digital_twin.Dataset.Types.Secondary;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Digital_twin.UserControls
{
    public partial class BuildingView : UserControl
    {
        private bool isDragging = false;
        private Point lastPosition;
        DataManager dataManager;
        public BuildingView()
        {
            InitializeComponent();
        }


        double pX = -1; double pY = -1;
        double? startX, startY;
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
                string state = dataManager.State;
                switch (state)
                {
                    case "Edit":
                        pX = -1; pY = -1;
                        startX = null; startY = null;
                        isDragging = true;
                        lastPosition = e.GetPosition(CanvasListBox);
                        Mouse.Capture(CanvasListBox);
                        break;
                    case "Line":
                        DrawWay(dataManager, position); 
                        break;
                    case "Point":
                        pX = -1; pY = -1;
                        startX = null; startY = null;
                        DrawPoint(dataManager, position);
                        break;
                }
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

        private void DrawPoint(DataManager dataManager, Point position)
        {
            dataManager.AddNode(position.X, position.Y);
        }

        private void DrawWay(DataManager dataManager, Point position)
        {
            if (!startX.HasValue)
            {
                startX = position.X;
                startY = position.Y;
            }

            if (pX == startX && pY == startY)
            {
                dataManager.AddWay(position.X, position.Y, pX, pY, false);
                pX = position.X;
                pY = position.Y;
            }
            else
            {
                dataManager.AddWay(position.X, position.Y, pX, pY, true);
                pX = position.X;
                pY = position.Y;
            }
        }

    }
}

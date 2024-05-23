using Digital_twin.Dataset.Types.Primary;
using Digital_twin.Dataset;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Digital_twin.Dataset.Support;
using Digital_twin.Dataset.Types.Canvas;
using Digital_twin.Dataset.Types.Secondary;
using Digital_twin.Dataset.Support.Actions;
namespace Digital_twin.UserControls
{
    public partial class MoveCanvas : UserControl
    {
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
       "Angle", typeof(double), typeof(MoveCanvas), new PropertyMetadata(default(double)));
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
        public MoveCanvas()
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
            }
            else
            {
                editingShape = null;
            }
        }
        private bool addedControlPoints;
        private void DropControlPoints()
        {
            if (addedControlPoints)
            {
                dataManager.SelectedLevel.Shapes.RemoveAt(dataManager.SelectedLevel.Shapes.Count - 1);
                dataManager.SelectedLevel.Shapes.RemoveAt(dataManager.SelectedLevel.Shapes.Count - 1);
            }
            addedControlPoints = false;
        }

        private CanvasObject obj;
        private Segment lastSegment;
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (dataManager == null) dataManager = this.DataContext as DataManager;
            UpdateEditingItem(sender, e);
            DropControlPoints();
            if (editingShape is Segment segment)
            {
                addedControlPoints = true;
                obj = segment.obj;
                lastSegment = segment;
                dataManager.SelectedLevel.Shapes.Add(segment.Point1);
                dataManager.SelectedLevel.Shapes.Add(segment.Point2);
            } 
            isDragging = true;
            e.Handled = true;

            
        }

        private MoveAction moveAction = null;
        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            if (editingShape is Dataset.Types.Primary.Point point)
            {
                double X, Y;
                (X, Y) = GpsUtils.CanvasToMeters(point.X, point.Y);
                (point.node.Latitude, point.node.Longitude) = GpsUtils.MetersToLatLon(X, Y);
            } 
            if(moveAction != null)
            {
                dataManager.actionList.AddAction(moveAction);
                moveAction = null;
            }
            editingShape = null;
        }

        private bool ComparePoints(Dataset.Types.Primary.Point point1, Dataset.Types.Primary.Point point2)
        {
            if((int)point1.X == (int)point2.X && (int)point1.Y == (int)point2.Y) return true;
            return false;
        }


        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                if (editingShape is Dataset.Types.Primary.Point point)
                {
                    if(moveAction == null)
                    {
                        moveAction = new MoveAction();
                    }
                    var position = e.GetPosition(CanvasListBox);
                    if(obj != null)
                    {
                        List<Segment> segmentsToUpdate1 = new List<Segment>();
                        List<Segment> segmentsToUpdate2 = new List<Segment>();
                        foreach( var item in obj.Shapes)
                        {
                            if(item is Segment)
                            {
                                if (ComparePoints(((Segment)item).Point1, point))
                                {
                                    segmentsToUpdate1.Add((Segment)item);
                                } else if (ComparePoints(((Segment)item).Point2, point))
                                {
                                    segmentsToUpdate2.Add((Segment)item);
                                }
                            }
                        }
                        if(obj is ClosedWayObject closed)
                        {
                            for (int i = 0; i < closed.Polygon.Vertices.Count; i++)
                            {
                                if ((int)closed.Polygon.Vertices[i].X == (int)point.X && (int)closed.Polygon.Vertices[i].Y == (int)point.Y)
                                {
                                    moveAction.SetPolygonInfo(closed, i, closed.Polygon.Vertices[i].X, closed.Polygon.Vertices[i].Y);
                                    closed.Polygon.UpdateVertex(i, position.X - 5, position.Y - 5);
                                }
                            }
                            moveAction.SetFinish();
                        }
                        foreach(Segment segment in segmentsToUpdate1)
                        {
                            moveAction.AddEvent(segment.Point1, segment.Point1.X, segment.Point1.Y);
                            segment.Point1.X = position.X - 5;
                            segment.Point1.Y = position.Y - 5;
                            segment.Point1.UpdateNodeCoordinates();
                        }
                        foreach (Segment segment in segmentsToUpdate2)
                        {
                            moveAction.AddEvent(segment.Point2, segment.Point2.X, segment.Point2.Y);
                            segment.Point2.X = position.X - 5;
                            segment.Point2.Y = position.Y - 5;
                            segment.Point2.UpdateNodeCoordinates();
                        }
                    }
                    moveAction.AddEvent(point, point.X, point.Y);
                    point.X = position.X - 5;
                    point.Y = position.Y - 5;
                    point.UpdateNodeCoordinates();
                }
            }
        }
    }
}

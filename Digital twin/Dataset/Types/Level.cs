using Digital_twin.Dataset.Types.Primary;
using Digital_twin.Dataset.Types.Canvas;
using System;
using System.Collections.ObjectModel;

namespace Digital_twin.Dataset.Types
{
    public class Level: ViewModelBase
    {
        public ObservableCollection<CanvasObject> canvasObjects { get; set; } = new ObservableCollection<CanvasObject>();
        public ObservableCollection<IShape> shapes { get; set; } = new ObservableCollection<IShape>();
        public ObservableCollection<CanvasObject> addedElements { get; set; } = new ObservableCollection<CanvasObject>();
        public ObservableCollection<IShape> addedShapes { get; set; } = new ObservableCollection<IShape>();

        private int level;
        private int lastCount = -1;

        public Level(int _level)
        {
            level = _level;
        }
        public ObservableCollection<IShape> Shapes
        {
            get
            {
                return shapes;
            }
        }

        public ObservableCollection<IShape> AddedShapes
        {
            get
            {
                return addedShapes;
            }
        }

        public void AddObjects(CanvasObject obj, bool finished)
        {
            if (finished) { 
                canvasObjects.Add(obj);
                foreach (IShape shape in obj.Shapes)
                {
                    shapes.Add(shape);
                }
            }
            else
            {
                addedElements.Add(obj);
                foreach (IShape shape in obj.Shapes)
                {
                    addedShapes.Add(shape);
                }
            }
        }

        public int LevelNum { get { return level; } }  
        public string Name { get { return level.ToString(); } }  
    }
}

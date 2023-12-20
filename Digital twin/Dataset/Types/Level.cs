using Digital_twin.Dataset.Types.Primary;
using Digital_twin.Dataset.Types.Tertiary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace Digital_twin.Dataset.Types
{
    public class Level: ViewModelBase
    {
        public ObservableCollection<CanvasObject> canvasObjects { get; set; } = new ObservableCollection<CanvasObject>();
        public ObservableCollection<IShape> shapes { get; set; } = new ObservableCollection<IShape>();

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

        public void AddObjects(CanvasObject obj)
        {
            canvasObjects.Add(obj);
            foreach (IShape shape in obj.Shapes)
            {
                shapes.Add(shape);
            }
        }
        public void AddObjects(IShape obj)
        {
            shapes.Add(obj);
        }

        public int LevelNum { get { return level; } }  
        public string Name { get { return level.ToString(); } }  
    }
}

using Digital_twin.Dataset.Support;
using Digital_twin.Dataset.Types;
using Digital_twin.Dataset.Types.Canvas;
using Digital_twin.Dataset.Types.Primary;
using Digital_twin.Dataset.Types.Secondary;
using Digital_twin.Dataset.Types.Tertiary;
using Digital_twin.Draw_tools;
using Digital_twin.File_tools;
using OsmSharp;
using OsmSharp.Tags;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Digital_twin.Dataset
{
    public class DataManager : ViewModelBase
    {
        private Level _selectedLevel;
        private IShape _selectedShape;
        public static double maxLatitude;
        public static double minLongitude;

        public static double maxX;
        public static double maxY;
        public static double minX;
        public static double minY;
        public static Tuple<int, int> central_offset;
        private RelayCommand _addCommand;
        private RelayCommand _removeCommand;
        private RelayCommand _saveCommand;
        int levelMax;
        int levelMin;

        private string state;
        public bool FileReaded
        {
            get
            {
                return readed;
            }
            set
            {
                readed = value;
                OnPropertyChanged(nameof(FileReaded));
            }
        }
        public string CurrentTipText
        {
            get
            {
                switch (state)
                {
                    case "Edit":
                        return "Click on item to change its properties";
                    case "Point":
                        return "Click anywhere on a canvas to draw an element. To close a line, click on its first part. To end line press Enter.";
                    case "Line":
                        return "Click anywhere on a canvas to draw an element. To close a line, click on its first part. To end line press Enter.";
                    case "Move":
                        return "Select item and use anchors to move it";
                    case "ImageTransform":
                        return "Hold Ctrl+click to scale or R+click to rotate, then use Alt+click to move.";
                    default:
                        return "Click `Browse OSM file to select file to work with.";
                }
            }
        }
        public string State
        {
            get { return state; }
            set
            {
                state = value;
                
                if (SelectedLevel != null)
                {
                    if(currentActiveObject != null)
                    {
                        SelectedLevel.AddObjects(currentActiveObject, true);
                        SelectedLevel.AddedShapes.Clear();
                        SelectedLevel.addedElements.Clear();
                    }
                }
                currentActiveObject = null;
                startPoint = null;
                Console.WriteLine("Current state: " + state);
                OnPropertyChanged("State");
                OnPropertyChanged("CurrentTipText");
            }
        }

        private ImageSource image;

        private string currentImage;

        private double _imageOpacity = 1.0;
        public double ImageOpacity
        {
            get { return _imageOpacity; }
            set
            {
                _imageOpacity = value;
                OnPropertyChanged("ImageOpacity");
            }
        }

        private double _shapeOpacity = 1.0;
        public double ShapeOpacity
        {
            get => _shapeOpacity;
            set 
            { 
                _shapeOpacity = value;
                OnPropertyChanged(nameof(ShapeOpacity));
            }
        }
        public string CurrentImage
        {
            get { return currentImage; }
            set
            {
                currentImage = value;
                Image = ImageLoader.LoadImage(currentImage);
                OnPropertyChanged(nameof(CurrentImage));
            }
        }

        public ImageSource Image
        {
            get { return image; }
            set { 
                image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        private string _keyText;
        private string _valueText;

        private string filename;

        public ObservableCollection<Node> Nodes { get; set; } = new ObservableCollection<Node>();
        public ObservableCollection<Way> Ways { get; set; } = new ObservableCollection<Way>();
        public ObservableCollection<Relation> Relations { get; set; } = new ObservableCollection<Relation>();
        public ObservableCollection<Building> Buildings { get; set; } = new ObservableCollection<Building>();
        public ObservableCollection<Level> Levels { get; set; } = new ObservableCollection<Level>();

        private bool readed;

        private void ReadOSMFile(string filename)
        {
            if(SelectedLevel != null) SelectedLevel.Shapes.Clear();
            Levels.Clear();
            Nodes.Clear();
            Ways.Clear();
            Relations.Clear();
            ReaderOSM.ReadOSM(filename, Nodes, Ways, Relations);
            var result = Parser.getBuildings(Ways, Relations);
            FileReaded = true;
            State = "Edit";
            // TEMP SOLUTION
            // TODO: Fix it
            ObservableCollection<Node> builldingNodes = new ObservableCollection<Node>();

            foreach (var building in result)
            {
                foreach (var item in Parser.getNodes(Nodes, building.Nodes))
                {
                    builldingNodes.Add(item);
                }

            }

            maxX = double.MinValue; minX = double.MaxValue; maxY = double.MinValue; minY = double.MaxValue;
            double x, y;

            foreach (Node node in builldingNodes)
            {
                (x, y) = GpsUtils.LatLonToMeters((double)node.Latitude, (double)node.Longitude);
                maxX = Math.Max(maxX, x);
                minX = Math.Min(minX, x);
                maxY = Math.Max(maxY, y);
                minY = Math.Min(minY, y);
            }


            maxLatitude = builldingNodes.Max(node => (double)node.Latitude);
            minLongitude = builldingNodes.Min(node => (double)node.Longitude);

            central_offset = DrawingTools.CenterOffset(builldingNodes);

            foreach (var building in result)
            {
                Buildings.Add(new Building(Parser.getNodes(Nodes, building.Nodes), building, false));
            }

            levelMax = Buildings.Max(building => int.Parse(building.MaxLevel));
            levelMin = Buildings.Min(building => int.Parse(building.MinLevel));


            for (int i = levelMin; i <= levelMax; i++)
            {
                Level level = new Level(i);
                foreach (var building in Buildings)
                {
                    level.AddObjects(building, true);
                }

                foreach (var room in Parser.getIndoorRoomLayout(Nodes, Ways, i.ToString()))
                {
                    level.AddObjects(room, true);
                }

                foreach (var way in Parser.getIndoorWayLayout(Nodes, Ways, i.ToString()))
                {
                    level.AddObjects(way, true);
                }

                foreach (var point in Parser.getDoorsLevel(Nodes, i.ToString()))
                {
                    level.AddObjects(point, true);
                }

                Levels.Add(level);
            }
            SelectedLevel = Levels.First();

        }
        private bool ValidateFileType(string path)
        {
            string extension = System.IO.Path.GetExtension(path);

            if (extension == null || extension.ToLower() != ".osm")
            {
                MessageBox.Show("The selected file is not an OSM file.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            else
            {
                return true;
            }
        }
        public ICommand ChangeOpacityCommand { get; set; }

        public DataManager()
        {
            FileReaded = false;
            State = "Default";
            ChangeOpacityCommand = new RelayCommand(ChangeOpacity);
        }

        private void ChangeOpacity(object obj)
        {
            ImageOpacity = (ImageOpacity == 0.0) ? 1.0 : 0.0;
        }

        
        public string Filepath
        { 
            get { return filename; }
            set {
                if (ValidateFileType(value))
                {
                    filename = System.IO.Path.GetFileName(value);
                    OnPropertyChanged(nameof(Filepath));
                    ReadOSMFile(value);
                }
            }
        }

        public Level SelectedLevel
        {
            get { return _selectedLevel; }
            set
            {
                _selectedLevel = value;
                OnPropertyChanged(nameof(SelectedLevel));
            }
        }
        public IShape SelectedShape
        {
            get {
                return _selectedShape; }
            set
            {
                _selectedShape = value;
                OnPropertyChanged(nameof(SelectedShape));
            }
        }

        private Types.Secondary.Tag _selectedTag;
        public Types.Secondary.Tag SelectedTag
        {
            get { return _selectedTag; }
            set
            {
                _selectedTag = value;
                if (value != null)
                {
                    KeyText = _selectedTag.Key;
                    ValueText = _selectedTag.Value;
                }
                OnPropertyChanged(nameof(SelectedTag));
            }
        }
        
        public string KeyText
        {
            get { return _keyText; }
            set
            {
                _keyText = value;
                OnPropertyChanged(nameof(KeyText));
            }
        }
        public string ValueText
        {
            get { return _valueText; }
            set
            {
                _valueText = value;
                OnPropertyChanged(nameof(ValueText));
            }
        }

        public RelayCommand AddCommand
        {
            get { return _addCommand ?? (_addCommand = new RelayCommand(AddTag, AddTagCanExecute)); }
        }
        private void AddTag(object obj)
        {
            var key = KeyText;
            var value = ValueText;

            var existingTag = SelectedShape.obj.Tags.FirstOrDefault(t => t.Key == key);
            if (existingTag != null)
            {
                existingTag.Value = value;
            }
            else
            {
                SelectedShape.obj.Tags.Add(new Types.Secondary.Tag { Key = key, Value = value });
            }

            KeyText = string.Empty;
            ValueText = string.Empty;
        }
        private bool AddTagCanExecute(object obj)
        {
            return !string.IsNullOrWhiteSpace(KeyText) && !string.IsNullOrWhiteSpace(ValueText) && readed;
        }

        public RelayCommand RemoveCommand
        {
            get { return _removeCommand ?? (_removeCommand = new RelayCommand(DeleteTag, DeleteTagCanExecute)); }
        }
        private void DeleteTag(object obj)
        {
            SelectedShape.obj.Tags.Remove(_selectedTag);
        }
        private bool DeleteTagCanExecute(object obj)
        {
            return _selectedTag != null;
        }

        public RelayCommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new RelayCommand(Save, SaveCanExecute)); }
        }
        private void Save(object obj)
        {
            WriterOSM.WriteOSM("new.osm", MergeTools.mergeNodes(Levels, Nodes), MergeTools.mergeWays(Levels, Ways), Relations);
            MessageBox.Show("Your work was saved!", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private bool SaveCanExecute(object obj)
        {
            return readed;
        }

        public void AddNode(double X, double Y)
        {
            Node n = new Node();
            (X, Y) = GpsUtils.CanvasToMeters(X, Y);
            (n.Latitude, n.Longitude)= GpsUtils.MetersToLatLon(X, Y);
            n.Tags = new TagsCollection();
            NodeObject nodeObject = new NodeObject(n);
            SelectedLevel.AddObjects(nodeObject, true);
        }

        CanvasObject currentActiveObject;
        private Node startPoint;
        public void AddWay(double X, double Y, double previousX, double previousY, bool removedStart)
        {
            // Create new control point
            Node n = new Node();
            (X, Y) = GpsUtils.CanvasToMeters(X, Y);
            (n.Latitude, n.Longitude) = GpsUtils.MetersToLatLon(X, Y);
            n.Tags = new TagsCollection();
            (X, Y) = GpsUtils.MetersToCanvas(X, Y);


            if (previousX == -1 && previousY == -1) // If it start control point -- create placeholder NodeObject
            {
                Types.Primary.Point point = new Types.Primary.Point(X, Y, n);
                SelectedLevel.addedShapes.Add(point);
                startPoint = n;
            }
            else // If it at least one segment
            {
                if (!removedStart)
                {
                    SelectedLevel.AddedShapes.RemoveAt(SelectedLevel.AddedShapes.Count - 1);
                }
                if (currentActiveObject == null)
                {
                    ObservableCollection<Node> _nodes = new ObservableCollection<Node>
                    {
                        startPoint, n
                    };
                    //TODO: add nodes?
                    Way way = new Way();
                    way.Tags = new TagsCollection();
                    currentActiveObject = new OpenedWayObject(_nodes, way, true);
                    foreach (IShape segment in currentActiveObject.Shapes)
                    {
                        SelectedLevel.AddedShapes.Add(segment);
                    }
                }
                else
                {
                    Node previousPoint = ((OpenedWayObject)currentActiveObject).Nodes.Last();
                    Segment addedSegment = new Segment(previousX, previousY, previousPoint, X, Y, n, true);
                    addedSegment.obj = currentActiveObject;
                    ((OpenedWayObject)currentActiveObject).UpdateSegments(addedSegment);
                    ((OpenedWayObject)currentActiveObject).Nodes.Add(n);

                    SelectedLevel.AddedShapes.Add(addedSegment);
                }
            }

        }
        public void CloseWay(double X, double Y)
        {
            Node n = new Node();
            (X, Y) = GpsUtils.CanvasToMeters(X, Y);
            (n.Latitude, n.Longitude) = GpsUtils.MetersToLatLon(X, Y);
            n.Tags = new TagsCollection();
            (X, Y) = GpsUtils.MetersToCanvas(X, Y);

            double sx, sy;
            (sx, sy) = GpsUtils.LatLonToMeters((double)startPoint.Latitude, (double)startPoint.Longitude);
            (sx, sy) = GpsUtils.MetersToCanvas(sx, sy);

            Segment addedSegment = new Segment(sx, sy, startPoint, X, Y, n, true);
            ((OpenedWayObject)currentActiveObject).Segments.Add(addedSegment);
            ((OpenedWayObject)currentActiveObject).Nodes.Add(n);
            ((OpenedWayObject)currentActiveObject).Nodes.Add(startPoint);


            ClosedWayObject closedWayObject = new ClosedWayObject(((OpenedWayObject)currentActiveObject).Nodes,
                ((OpenedWayObject)currentActiveObject).Way, true);
            SelectedLevel.AddObjects(closedWayObject, true);
            currentActiveObject = null;
            startPoint = null;
            SelectedLevel.AddedShapes.Clear();
            SelectedLevel.addedElements.Clear();
            Console.WriteLine("Finish");
        }

        RelayCommand _addLevelCommand;
        public RelayCommand AddLevelCommand
        {
            get { return _addLevelCommand ?? (_addLevelCommand = new RelayCommand(AddLevel, AddLevelCanExecute)); }
        }
        private void AddLevel(object obj)
        {
            Level level = new Level(levelMax + 1);
            levelMax++;
            foreach (var building in Buildings)
            {
                level.AddObjects(building, true);
            }
            Levels.Add(level);
        }
        private bool AddLevelCanExecute(object obj)
        {
            return readed;
        }

        RelayCommand _moveLevelUpCommand;
        public RelayCommand MoveLevelUpCommand
        {
            get { return _moveLevelUpCommand ?? (_moveLevelUpCommand = new RelayCommand(MoveLevelUp, MoveLevelUpCanExecute)); }
        }
        private void MoveLevelUp(object obj)
        {
            if (SelectedLevel != null && Levels.Count > 1)
            {
                int currentIndex = Levels.IndexOf(SelectedLevel);

                if (currentIndex > 0)
                {
                    string name = Levels[currentIndex - 1].Name;
                    Levels[currentIndex - 1].Name = SelectedLevel.Name;
                    SelectedLevel.Name = name;
                    Level previousItem = Levels[currentIndex - 1];
                    Levels.Move(currentIndex, currentIndex - 1);
                }
            }
        }
        private bool MoveLevelUpCanExecute(object obj)
        {
            return (SelectedLevel != null) && (SelectedLevel.Name != levelMin.ToString());
        }

        RelayCommand _moveLevelDownCommand;
        public RelayCommand MoveLevelDownCommand
        {
            get { return _moveLevelDownCommand ?? (_moveLevelDownCommand = new RelayCommand(MoveLevelDown, MoveLevelDownCanExecute)); }
        }
        private void MoveLevelDown(object obj)
        {
            if (SelectedLevel != null && Levels.Count > 1)
            {
                int currentIndex = Levels.IndexOf(SelectedLevel);
                if (currentIndex < Levels.Count - 1)
                {
                    string name = Levels[currentIndex + 1].Name;
                    Console.WriteLine(Levels[currentIndex + 1].Name);
                    Levels[currentIndex + 1].Name = SelectedLevel.Name;
                    Console.WriteLine(Levels[currentIndex + 1].Name);
                    SelectedLevel.Name = name;
                    Levels.Move(currentIndex, currentIndex + 1);
                }
            }
        }
        private bool MoveLevelDownCanExecute(object obj)
        {
            return (SelectedLevel != null) && (SelectedLevel.Name != levelMax.ToString());
        }
    }
}

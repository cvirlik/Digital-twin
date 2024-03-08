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
        public static Tuple<int, int> central_offset;
        private RelayCommand _addCommand;
        private RelayCommand _removeCommand;
        private RelayCommand _saveCommand;

        private string state;
        public string State
        {
            get { return state; }
            set
            {
                state = value;
                if (SelectedLevel != null)
                {
                    foreach(IShape shape in SelectedLevel.AddedElements)
                    {
                        SelectedLevel.Shapes.Add(shape);
                    }
                    SelectedLevel.AddedElements.Clear();
                }
                Console.WriteLine("Current state: " + state);
                OnPropertyChanged("State");
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

        private bool readed = false;

        private void ReadOSMFile(string filename)
        {
            Levels.Clear();
            Nodes.Clear();
            Ways.Clear();
            Relations.Clear();
            ReaderOSM.ReadOSM(filename, Nodes, Ways, Relations);
            var result = Parser.getBuildings(Ways);
            readed = true;
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
            maxLatitude = builldingNodes.Max(node => (double)node.Latitude);
            minLongitude = builldingNodes.Min(node => (double)node.Longitude);

            central_offset = DrawingTools.CenterOffset(builldingNodes);

            foreach (var building in result)
            {
                Buildings.Add(new Building(Parser.getNodes(Nodes, building.Nodes), building, false));
            }

            int levelMax = Buildings.Max(building => int.Parse(building.MaxLevel));
            int levelMin = Buildings.Min(building => int.Parse(building.MinLevel));


            for (int i = levelMin; i <= levelMax; i++)
            {
                Level level = new Level(i);
                foreach (var building in Buildings)
                {
                    level.AddObjects(building);
                }

                foreach (var room in Parser.getIndoorRoomLayout(Nodes, Ways, i.ToString()))
                {
                    level.AddObjects(room);
                }

                foreach (var way in Parser.getIndoorWayLayout(Nodes, Ways, i.ToString()))
                {
                    level.AddObjects(way);
                }

                foreach (var point in Parser.getDoorsLevel(Nodes, i.ToString()))
                {
                    level.AddObjects(point);
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
            State = "Edit";
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
            return !string.IsNullOrWhiteSpace(KeyText) && !string.IsNullOrWhiteSpace(ValueText);
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
            n.Latitude = -1; n.Longitude = -1;
            n.Tags = new TagsCollection();
            Types.Primary.Point point = new Types.Primary.Point(X, Y, n);
            point.obj = new NodeObject(n);
            SelectedLevel.AddedElements.Add(point);
        }
        public void AddWay(double X, double Y, double previousX, double previousY, bool removedStart)
        {
            Node n = new Node();
            n.Latitude = -1; n.Longitude = -1;
            n.Tags = new TagsCollection();

            if (previousX == -1 && previousY == -1)
            {
                Console.WriteLine("Place first point");
                Types.Primary.Point point = new Types.Primary.Point(X, Y, n);
                point.obj = new NodeObject(n);
                SelectedLevel.AddedElements.Add(point);
            }
            else
            {
                if (!removedStart)
                {
                    Console.WriteLine("Remove Start Point");
                    SelectedLevel.AddedElements.RemoveAt(SelectedLevel.AddedElements.Count - 1);
                }
                Node np = new Node();
                np.Latitude = -1; np.Longitude = -1;
                np.Tags = new TagsCollection();
                Segment segment = new Segment(previousX, previousY, np, X, Y, n, true);
                segment.obj = new NodeObject(n); //TODO: Fix objects.
                SelectedLevel.AddedElements.Add(segment);
            }
        }
    }
}

using Digital_twin.Dataset.Support;
using Digital_twin.Dataset.Types;
using Digital_twin.Dataset.Types.Canvas;
using Digital_twin.Dataset.Types.Primary;
using Digital_twin.Dataset.Types.Tertiary;
using Digital_twin.Draw_tools;
using Digital_twin.File_tools;
using Digital_twin.RelayButtons;
using OsmSharp;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace Digital_twin.Dataset
{
    public class DataManager : ViewModelBase
    {
        private Level _selectedLevel;
        private IShape _selectedShape;
        public static double maxX = double.MinValue;
        public static double maxY = double.MinValue;
        public static double minX = double.MaxValue;
        public static double minY = double.MaxValue;

        public static double maxLatitude;
        public static double minLongitude;

        public void SetCanvasVariables(ObservableCollection<Node> builldingNodes)
        {
            double x, y;
            foreach (Node node in builldingNodes)
            {
                (x, y) = GpsUtils.LatLonToMeters((double)node.Latitude, (double)node.Longitude);
                maxX = Math.Max(maxX, x);
                minX = Math.Min(minX, x);
                maxY = Math.Max(maxY, y);
                minY = Math.Min(minY, y);
            }  
            central_offset = DrawingTools.CenterOffset(builldingNodes);
            maxLatitude = builldingNodes.Max(node => (double)node.Latitude);
            minLongitude = builldingNodes.Min(node => (double)node.Longitude);
        }
        public static Tuple<int, int> central_offset;
        public AddTag AddCommand { get; }
        public RemoveTag RemoveCommand { get; }
        public MoveLevelDown MoveLevelDownCommand { get; }
        public MoveLevelUp MoveLevelUpCommand { get; }
        public AddLevel AddLevelCommand { get; }
        public RelayCommand SaveCommand { get; }

        public int levelMax;
        public int levelMin;

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
                        return "Hold Alt+click to scale or R+click to rotate, then use Ctrl+click to move.";
                    default:
                        return "Click `Browse OSM file` to select file to work with.";
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
                ImageSource newImage = ImageLoader.LoadImage(currentImage);
                if (newImage != null)
                {
                    Image = newImage;
                }
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
        public bool Readed
        {
            get => readed;
        }

        public ICommand ChangeOpacityCommand { get; set; }

        public DataManager()
        {
            FileReaded = false;
            State = "Default";
            ChangeOpacityCommand = new RelayCommand(ChangeOpacity);
            AddCommand = new AddTag(this);
            RemoveCommand = new RemoveTag(this);
            MoveLevelDownCommand = new MoveLevelDown(this);
            MoveLevelUpCommand = new MoveLevelUp(this);
            AddLevelCommand = new AddLevel(this);
            SaveCommand = new SaveComand(this);
        }

        private void ChangeOpacity(object obj)
        {
            ImageOpacity = (ImageOpacity == 0.0) ? 1.0 : 0.0;
        }
        
        public string Filepath
        { 
            get { return filename; }
            set {
                if (SupportingFileTools.ValidateOSMFileType(value))
                {
                    filename = System.IO.Path.GetFileName(value);
                    OnPropertyChanged(nameof(Filepath));
                    ReaderOSM.DataPartitioning(this, value);

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

        public CanvasObject currentActiveObject;
        public Node startPoint;
    }
}

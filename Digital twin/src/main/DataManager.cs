using Digital_twin.Dataset.Support;
using Digital_twin.Dataset.Support.Actions;
using Digital_twin.Dataset.Support.Actions.Transform;
using Digital_twin.Dataset.Types;
using Digital_twin.Dataset.Types.Canvas;
using Digital_twin.Dataset.Types.Primary;
using Digital_twin.Dataset.Types.Secondary;
using Digital_twin.Dataset.Types.Tertiary;
using Digital_twin.Draw_tools;
using Digital_twin.File_tools;
using Digital_twin.RelayButtons;
using OsmSharp;
using System;
using System.Collections.Generic;
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
        public static double maxX = double.MinValue;
        public static double maxY = double.MinValue;
        public static double minX = double.MaxValue;
        public static double minY = double.MaxValue;
        public IdGenerator _idGenerator = new IdGenerator();
        public ActionList actionList = new ActionList(10);
        public ObservableCollection<Tag> _recommendedTags;
        public ObservableCollection<Tag> RecommendedTags
        {
            get { return _recommendedTags; }
            set
            {
                if (_recommendedTags != value)
                {
                    _recommendedTags = value;
                    OnPropertyChanged(nameof(RecommendedTags));
                }
            }
        }
        
        
        public Cursor CurrentCursor
        {
            get
            {
                switch (State)
                {
                    case "Edit":
                    case "Default":
                    case "ImageTransform":
                        return Cursors.Arrow;
                    case "Point":
                    case "Line":
                        return Cursors.Pen;
                    case "Move":
                        return Cursors.SizeAll;
                    default:
                        return Cursors.Arrow;
                }
            }
        }

        public static double maxLatitude;
        public static double minLongitude;

        public void SetCanvasVariables(ObservableCollection<Node> builldingNodes)
        {
            maxX = double.MinValue;
            maxY = double.MinValue;
            minX = double.MaxValue;
            minY = double.MaxValue;
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
        public RelayCommand AddRecommendedTag { get; }

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
                        return "Click anywhere on majorAxis canvas to draw an element. To close majorAxis line, click on its first part. To end line press Enter.";
                    case "Line":
                        return "Click anywhere on majorAxis canvas to draw an element. To close majorAxis line, click on its first part. To end line press Enter.";
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
                    SelectedShape = null;
                }
                currentActiveObject = null;
                startPoint = null;
                OnPropertyChanged("State");
                OnPropertyChanged("CurrentCursor");
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
            AddRecommendedTag = new AddRecommendedTag(this);
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
                if(_selectedShape != null)
                {
                    _selectedShape.IsObjectSelected = false;
                    RecommendedTags = null;
                }
                _selectedShape = value;
                if (_selectedShape != null)
                {
                    _selectedShape.IsObjectSelected = true;
                    if(SelectedShape.obj is OpenedWayObject)
                    {
                        RecommendedTags = new ObservableCollection<Tag>()
                        {
                            new Tag("indoor", "pathway")
                        };
                    } else if(SelectedShape.obj is ClosedWayObject)
                    {
                        RecommendedTags = new ObservableCollection<Tag>()
                        {
                            new Tag("indoor", "room"),
                            new Tag("indoor", "corridor")
                        };
                    }
                    else if(SelectedShape.obj is NodeObject)
                    {
                        RecommendedTags = new ObservableCollection<Tag>()
                        {
                            new Tag("door", "hinged")
                        };
                    }
                    else
                    {
                        RecommendedTags = null;
                    }
                }
                OnPropertyChanged(nameof(SelectedShape));
            }
        }

        public void DeleteElement()
        {
            if(SelectedShape != null)
            {
                if (SelectedShape is Types.Primary.Point point)
                {
                    actionList.AddAction(new DeleteAction(new NodeDelete(point, this)));
                    Console.WriteLine("Add NodeDelete. Current long: " + actionList.CurrentLenght());
                    Nodes.Remove(point.node);
                    SelectedLevel.DeleteObject(point.obj);
                } 
                if (SelectedShape is Segment segment)
                {
                    Console.WriteLine(segment.obj.GetType().Name);
                    List<long> nodes = segment.way.Nodes.ToList();
                    List<int> indices = nodes.Select((b, i) => b == (long)segment.Point1.node.Id ? i : -1).Where(i => i != -1).ToList();

                    if(segment.obj is ClosedWayObject)
                    {
                        List<long> newWayObject;
                        foreach (int i in indices)
                        {
                            if (nodes[i + 1] == (long)segment.Point2.node.Id)
                            {
                                List<long> firstPart = nodes.Take(i+1).ToList();
                                List<long> secondPart = nodes.Skip(i+1).ToList();

                                firstPart.Reverse();
                                secondPart.Reverse();

                                if (secondPart.Count > 0 && firstPart.Count > 0 && secondPart[0] == firstPart[firstPart.Count - 1])
                                {
                                    secondPart.RemoveAt(0);
                                }


                                newWayObject = firstPart.Concat(secondPart).ToList();
                                Way way = new Way();
                                way.Nodes = new List<long>(segment.way.Nodes).ToArray();
                                way.Tags = segment.way.Tags;
                                way.Id = segment.way.Id;

                                way.Nodes = newWayObject.ToArray();

                                OpenedWayObject openedWayObject =
                                    new OpenedWayObject(Parser.getNodes(Nodes, newWayObject.ToArray()), way, segment.IsInner);
                                actionList.AddAction(new DeleteAction(new WaySeparation(segment.obj, openedWayObject, null, this)));
                                Console.WriteLine("Add WaySeparation. Current long: " + actionList.CurrentLenght());
                                SelectedLevel.DeleteObject(segment.obj);
                                SelectedLevel.AddObjects(openedWayObject, true);
                                break;
                            }
                        }
                    }
                    if (segment.obj is OpenedWayObject openedWay)
                    {
                        foreach (int i in indices)
                        {
                            if (nodes[i + 1] == (long)segment.Point2.node.Id)
                            {
                                if(i == 0){
                                    nodes.RemoveAt(0);
                                } else if (i + 1 == nodes.Count() - 1){
                                    nodes.RemoveAt(i+1);
                                }
                                else
                                {
                                    List<long> firstPart = nodes.GetRange(0, i + 1);
                                    List<long> secondPart = nodes.GetRange(i + 1, nodes.Count - (i + 1));

                                    Way firstWay = new Way();
                                    firstWay.Id = _idGenerator.GenerateIdWay();
                                    firstWay.Nodes = firstPart.ToArray();
                                    firstWay.Tags = segment.way.Tags;

                                    OpenedWayObject firstOpenedWayObject =
                                    new OpenedWayObject(Parser.getNodes(Nodes, firstWay.Nodes), firstWay, segment.IsInner);

                                    Way secondWay = new Way();
                                    secondWay.Id = _idGenerator.GenerateIdWay();
                                    secondWay.Nodes = secondPart.ToArray();
                                    secondWay.Tags = segment.way.Tags;

                                    OpenedWayObject secondOpenedWayObject =
                                    new OpenedWayObject(Parser.getNodes(Nodes, secondWay.Nodes), secondWay, segment.IsInner);
                                    actionList.AddAction(new DeleteAction(new WaySeparation(openedWay, firstOpenedWayObject, 
                                        secondOpenedWayObject, this)));
                                    Console.WriteLine("Add WaySeparation. Current long: " + actionList.CurrentLenght());
                                    SelectedLevel.DeleteObject(openedWay);
                                    SelectedLevel.AddObjects(firstOpenedWayObject, true);
                                    SelectedLevel.AddObjects(secondOpenedWayObject, true);
                                    break;
                                }

                                actionList.AddAction(new DeleteAction(new WayCut(openedWay, new List<long>(segment.way.Nodes), this)));
                                Console.WriteLine("Add WayCut. Current long: " + actionList.CurrentLenght());
                                SelectedLevel.DeleteObject(openedWay);
                                openedWay.Way.Nodes = nodes.ToArray();
                                openedWay.ReloadSegments(Parser.getNodes(Nodes, openedWay.Way.Nodes));
                                SelectedLevel.AddObjects(openedWay, true);
                                break;
                            }
                        }

                    }
                }
            }
            SelectedShape = null;
        }

        private Tag _selectedTag;
        public Tag SelectedTag
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

        private Tag _selectedRecommendedTag;
        public Tag SelectedRecommendedTag
        {
            get { return _selectedRecommendedTag; }
            set
            {
                _selectedRecommendedTag = value;
                OnPropertyChanged(nameof(SelectedRecommendedTag));
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

        public OpenedWayObject currentActiveObject;
        public Node startPoint;
        
    }
}

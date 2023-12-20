using Digital_twin.Dataset.Support;
using Digital_twin.Dataset.Types;
using Digital_twin.Dataset.Types.Primary;
using Digital_twin.Dataset.Types.Secondary;
using Digital_twin.Dataset.Types.Tertiary;
using Digital_twin.Draw_tools;
using Digital_twin.File_tools;
using OsmSharp;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Digital_twin.Dataset
{
    internal class DataManager : ViewModelBase
    {
        private Level _selectedLevel;
        private IShape _selectedShape;
        public static double maxLatitude;
        public static double minLongitude;
        public static Tuple<int, int> central_offset;
        private RelayCommand _addCommand;

        public ObservableCollection<Node> Nodes { get; set; } = new ObservableCollection<Node>();
        public ObservableCollection<Way> Ways { get; set; } = new ObservableCollection<Way>();
        public ObservableCollection<Relation> Relations { get; set; } = new ObservableCollection<Relation>();
        public ObservableCollection<Building> Buildings { get; set; } = new ObservableCollection<Building>();
        public ObservableCollection<Level> Levels { get; set; } = new ObservableCollection<Level>();
        public Canvas canvas { get; set; }

        public DataManager() {
            ReaderOSM.ReadOSM(@"OSM files\map.osm", Nodes, Ways, Relations);
            var result = Parser.getBuildings(Ways);
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
            central_offset = DrawingTools.CenterOffset(builldingNodes, maxLatitude, minLongitude);

            // building fix
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
                    Console.WriteLine("Add way");
                    level.AddObjects(way);
                }

                foreach (var point in Parser.getDoorsLevel(Nodes, i.ToString()))
                {
                    point.X -= 2.5;
                    point.Y -= 2.5;
                    level.AddObjects(point);
                }

                Levels.Add(level);
            } 
            _selectedLevel = Levels.First();
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
                OnPropertyChanged("SelectedShape");
            }
        }

        private string _keyText;
        public string KeyText
        {
            get { return _keyText; }
            set
            {
                _keyText = value;
                OnPropertyChanged("KeyText");
            }
        }

        private string _valueText;
        public string ValueText
        {
            get { return _valueText; }
            set
            {
                _valueText = value;
                OnPropertyChanged("ValueText");
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

            var existingTag = SelectedShape.Tags.FirstOrDefault(t => t.Key == key);
            if (existingTag != null)
            {
                existingTag.Value = value;
            }
            else
            {
                SelectedShape.Tags.Add(new Tag { Key = key, Value = value });
            }

            KeyText = string.Empty;
            ValueText = string.Empty;
        }

        private bool AddTagCanExecute(object obj)
        {
            return !string.IsNullOrWhiteSpace(KeyText) && !string.IsNullOrWhiteSpace(ValueText);
        }
    }
}

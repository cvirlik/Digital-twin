using Digital_twin.Dataset.Types;
using Digital_twin.File_tools;
using OsmSharp;
using OsmSharp.Tags;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Digital_twin.Dataset
{
    internal class DataManager : ViewModelBase
    {
        private Building _selectedBuilding;
        private Level _selectedLevel;
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
            double maxLatitude = builldingNodes.Max(node => (double)node.Latitude);
            double minLongitude = builldingNodes.Min(node => (double)node.Longitude);
            //

            foreach (var building in result) 
            {
                Buildings.Add(new Building(building, Parser.getNodes(Nodes, building.Nodes), maxLatitude, minLongitude));
            }
            _selectedBuilding = Buildings.First();
            
            int levelMax =  Buildings.Max(building => int.Parse(building.MaxLevel));
            int levelMin =  Buildings.Min(building => int.Parse(building.MinLevel));
            for(int i = levelMin; i <= levelMax; i++)
            {
                var rooms = Parser.getIndoorRoomLayout(Nodes, Ways, i.ToString(), _selectedBuilding);
                Levels.Add(new Level(i, rooms, Buildings));
            }
            _selectedLevel = Levels.First();
        }

        public Building SelectedBuilding
        {
            get { return _selectedBuilding; }
            set
            {
                _selectedBuilding = value;
                OnPropertyChanged(nameof(SelectedBuilding));
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


    }
}

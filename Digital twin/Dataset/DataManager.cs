using Digital_twin.Dataset.Types;
using Digital_twin.File_tools;
using OsmSharp;
using OsmSharp.Tags;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Digital_twin.Dataset
{
    internal class DataManager
    {
        public ObservableCollection<Node> Nodes { get; set; } = new ObservableCollection<Node>();
        public ObservableCollection<Way> Ways { get; set; } = new ObservableCollection<Way>();
        public ObservableCollection<Relation> Relations { get; set; } = new ObservableCollection<Relation>();
        public ObservableCollection<Building> Buildings { get; set; } = new ObservableCollection<Building>();

        public DataManager() {
            ReaderOSM.ReadOSM(@"OSM files\map.osm", Nodes, Ways, Relations);
            var result = Parser.getBuilding(Ways);

            foreach(var building in result) 
            {
                Buildings.Add(new Building(building, Parser.getNodes(Nodes, building.Nodes)));
            }
        }
    }
}

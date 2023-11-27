using Digital_twin.Dataset.Types;
using Digital_twin.File_tools;
using OsmSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_twin.Dataset
{
    public class Parser
    {
        public static ObservableCollection<Node> getNodes(ObservableCollection<Node> Nodes, long[] nodesList)
        {
            ObservableCollection<Node> BuildingNodes = new ObservableCollection<Node>();
            foreach (long nodeId in nodesList)
            {
                Node node = Nodes.FirstOrDefault(n => n.Id == nodeId);
                if (node != null)
                {
                    BuildingNodes.Add(node);
                }
            }
            return BuildingNodes;
        }
        public static ObservableCollection<Way> getBuildings(ObservableCollection<Way> Ways)
        {
            ObservableCollection<Way> Buildings = new ObservableCollection<Way>();
            
            foreach (Way way in Ways)
            {
                if (way.Tags != null && way.Tags.ContainsKey("building"))
                {
                    Buildings.Add(way);
                }
            }
            return Buildings;
        }
        public static ObservableCollection<Room> getIndoorRoomLayout(ObservableCollection<Node> Nodes, 
                                                                     ObservableCollection<Way> Ways, string level, Building currentBuilding) 
        {
            ObservableCollection<Room> IndoorLayout = new ObservableCollection<Room>();

            foreach (Way way in Ways)
            {
                if (way.Tags != null && way.Tags.Contains("indoor", "room") && way.Tags.Contains("level", level))
                {
                    IndoorLayout.Add(new Room(way, getNodes(Nodes, way.Nodes), currentBuilding));
                }
            }
            return IndoorLayout;
        }
    }
}

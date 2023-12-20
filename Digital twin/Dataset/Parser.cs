using Digital_twin.Dataset.Types.Primary;
using Digital_twin.Dataset.Types.Secondary;
using Digital_twin.Dataset.Types.Tertiary;
using Digital_twin.Draw_tools;
using OsmSharp;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Digital_twin.Dataset
{
    public class Parser
    {
        public static ObservableCollection<Node> getNodes(ObservableCollection<Node> Nodes, long[] nodesList)
        {
            ObservableCollection<Node> OSMNodes = new ObservableCollection<Node>();
            foreach (long nodeId in nodesList)
            {
                Node node = Nodes.FirstOrDefault(n => n.Id == nodeId);
                if (node != null)
                {
                    OSMNodes.Add(node);
                }
            }
            return OSMNodes;
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
            ObservableCollection<Way> Ways, string level) 
        {
            ObservableCollection<Room> IndoorLayout = new ObservableCollection<Room>();

            foreach (Way way in Ways)
            {
                if (way.Tags != null && (way.Tags.Contains("indoor", "room") || way.Tags.Contains("indoor", "corridor")) && 
                    way.Tags.Contains("level", level))
                {
                    IndoorLayout.Add(new Room(getNodes(Nodes, way.Nodes), way, true));
                }
            }
            return IndoorLayout;
        }

        public static ObservableCollection<Segment> getIndoorWayLayout(ObservableCollection<Node> Nodes,
            ObservableCollection<Way> Ways, string level)
        {
            ObservableCollection<Segment> IndoorLayout = new ObservableCollection<Segment>();

            foreach (Way way in Ways)
            {
                if (way.Tags != null && way.Tags.Contains("indoor", "pathway"))
                {
                    ObservableCollection<Node> nodes = getNodes(Nodes, way.Nodes);
                    //ObservableCollection<Segment> layout = new ObservableCollection<Segment>();
                    DrawingTools.SplitToSegments(nodes, IndoorLayout, DataManager.maxLatitude, DataManager.minLongitude, true, null);
                }
            }
            return IndoorLayout;
        }

        public static ObservableCollection<Point> getDoorsLevel(ObservableCollection<Node> Nodes, string level)
        {
            ObservableCollection<Point> Doors = new ObservableCollection<Point>();

            foreach (Node node in Nodes)
            {
                if (node.Tags != null && node.Tags.ContainsKey("door") && node.Tags.Contains("level", level))
                {
                    Doors.Add(DrawingTools.CreatePointFromNode(node, DataManager.maxLatitude, DataManager.minLongitude));
                }
            }
            return Doors;
        }
    }
}

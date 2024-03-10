using Digital_twin.Dataset.Types.Canvas;
using Digital_twin.Dataset.Types.Tertiary;
using OsmSharp;
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
        public static ObservableCollection<ClosedWayObject> getIndoorRoomLayout(ObservableCollection<Node> Nodes, 
            ObservableCollection<Way> Ways, string level) 
        {
            ObservableCollection<ClosedWayObject> IndoorLayout = new ObservableCollection<ClosedWayObject>();

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

        public static ObservableCollection<OpenedWayObject> getIndoorWayLayout(ObservableCollection<Node> Nodes,
            ObservableCollection<Way> Ways, string level)
        {
            ObservableCollection<OpenedWayObject> IndoorLayout = new ObservableCollection<OpenedWayObject>();

            foreach (Way way in Ways)
            {
                if (way.Tags != null && way.Tags.Contains("indoor", "pathway") && way.Tags.GetValue("level").Contains(level))
                {
                    IndoorLayout.Add(new Stairs(getNodes(Nodes, way.Nodes), way, true));
                }
            }
            return IndoorLayout;
        }

        public static ObservableCollection<NodeObject> getDoorsLevel(ObservableCollection<Node> Nodes, string level)
        {
            ObservableCollection<NodeObject> Doors = new ObservableCollection<NodeObject>();

            foreach (Node node in Nodes)
            {
                if (node.Tags != null && node.Tags.ContainsKey("door") && node.Tags.Contains("level", level))
                {
                    Doors.Add(new Door(node));
                }
            }
            return Doors;
        }
        public static NodeObject createNodeObject(Node node)
        {
            return new NodeObject(node);
        }
    }
}

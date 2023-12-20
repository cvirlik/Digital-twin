using System;
using System.Collections.ObjectModel;
using Digital_twin.Dataset.Types.Primary;
using Digital_twin.Draw_tools;
using OsmSharp;

namespace Digital_twin.Dataset.Types.Tertiary
{
    public class Room : CanvasObject
    {
        public Room(ObservableCollection<Node> _nodes, Way _way, bool isInner) : base(_nodes, _way, isInner){}

        public string Name
        {
            get
            {
                if (way.Tags.ContainsKey("ref"))
                    return way.Tags["ref"];
                else
                    return "Unnamed room";
            }
        }
    }
}

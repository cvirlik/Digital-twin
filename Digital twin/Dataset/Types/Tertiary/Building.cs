using System.Collections.ObjectModel;
using OsmSharp;
using Digital_twin.Draw_tools;
using Digital_twin.Dataset.Types.Secondary;

namespace Digital_twin.Dataset.Types.Tertiary
{
    public class Building : CanvasObject
    {
        public Building(ObservableCollection<Node> _nodes, Way _way, bool isInner) : base(_nodes, _way, isInner){}

        public string Name
        {
            get
            {
                if (way.Tags.ContainsKey("name"))
                    return way.Tags["name"];
                else
                    return "Unnamed building";
            }
        }
        public string LevelsCount
        {
            get
            {
                if (way.Tags.ContainsKey("building:levels"))
                    return way.Tags["building:levels"];
                else
                    return "1";
            }
        }
        public string MaxLevel
        {
            get
            {
                if (way.Tags.ContainsKey("max_level"))
                    return way.Tags["max_level"];
                else
                    return "1";
            }
        }
        public string MinLevel
        {
            get
            {
                if (way.Tags.ContainsKey("min_level"))
                    return way.Tags["min_level"];
                else
                    return "1";
            }
        }
    }
}

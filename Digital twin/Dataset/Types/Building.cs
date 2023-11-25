using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsmSharp;

namespace Digital_twin.Dataset.Types
{
    internal class Building
    {
        private Way way { get; set; }
        private ObservableCollection<Node> nodes { get; set; }

        public Building(Way _way, ObservableCollection<Node> _nodes)
        {
            way = _way;
            nodes = _nodes;
        }
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

        public Way Way { get { return way; } }
        public ObservableCollection<Node> Nodes { get { return nodes; } }
    }
}

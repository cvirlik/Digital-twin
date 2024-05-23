using Digital_twin.Dataset.Types.Canvas;
using OsmSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_twin.Dataset.Types.Tertiary
{
    public class Stairs : OpenedWayObject
    {
        public Stairs(ObservableCollection<Node> _nodes, Way _way, bool isInner) : base(_nodes, _way, isInner)
        {
        }
    }
}

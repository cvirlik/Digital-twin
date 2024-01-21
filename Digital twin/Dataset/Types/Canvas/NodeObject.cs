using System.Collections.ObjectModel;
using OsmSharp;
using Digital_twin.Dataset.Types.Primary;
using Digital_twin.Draw_tools;
using Digital_twin.Dataset.Types.Secondary;

namespace Digital_twin.Dataset.Types.Canvas
{
    public abstract class NodeObject : CanvasObject
    {
        public Node node { get; set; }
        public NodeObject(Node _node)
        {
            this.node = _node;
            foreach (var tag in node.Tags)
            {
                Tags.Add(new Tag { Key = tag.Key, Value = tag.Value });
            }
            Point shape = DrawingTools.CreatePointFromNode(node);
            shape.X -= 2.5; shape.Y -= 2.5;
            shape.obj = this;
            shapes.Add(shape);
        }

        public Node Node { get { return node; } }
    }
}

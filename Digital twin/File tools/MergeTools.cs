using Digital_twin.Dataset.Types;
using Digital_twin.Dataset.Types.Canvas;
using System.Collections.ObjectModel;
using OsmSharp;
using System.Collections.Generic;
using System.Windows.Shapes;
using Digital_twin.Dataset.Types.Primary;
using Sec = Digital_twin.Dataset.Types.Secondary;
using OsmSharp.Tags;
using System.Linq;
using System;

namespace Digital_twin.File_tools
{
    static class MergeTools
    {
        public static ObservableCollection<Node> mergeNodes(ObservableCollection<Level> levels, ObservableCollection<Node> nodes)
        {
            ObservableCollection<Node> updatedNodes = new ObservableCollection<Node>();

            foreach (Level level in levels)
            {
                foreach (CanvasObject obj in level.canvasObjects){
                    if (obj is NodeObject)
                    {
                        NodeObject upd = (NodeObject)obj;
                        Node node = upd.node;
                        var osmSharpTags = new TagsCollection();

                        ObservableCollection<Sec.Tag> updatedTags = new ObservableCollection<Sec.Tag>();
                        foreach(Sec.Tag tag in upd.Tags)
                        {
                            osmSharpTags.Add(tag.Key, tag.Value);
                        }
                        node.Tags = osmSharpTags;

                        updatedNodes.Add(node);
                    }
                }
            }
            Console.WriteLine(updatedNodes.Count());
            foreach (var updatedNode in updatedNodes)
            {
                var originalNode = nodes.FirstOrDefault(n => n.Id == updatedNode.Id);
                if (originalNode != null)
                {
                    nodes.Remove(nodes.Where(i => i.Id == originalNode.Id).Single());
                }
                nodes.Add(updatedNode);
            }

            return nodes;
        }

        public static ObservableCollection<Way> mergeWays(ObservableCollection<Level> levels, ObservableCollection<Way> ways)
        {
            ObservableCollection<Way> updatedWays = new ObservableCollection<Way>();

            foreach (Level level in levels)
            {
                foreach (CanvasObject obj in level.canvasObjects)
                {
                    if (obj is ClosedWayObject)
                    {
                        ClosedWayObject upd = (ClosedWayObject)obj;
                        Way way = upd.Way;
                        var osmSharpTags = new TagsCollection();
                        foreach (Sec.Tag tag in upd.Tags)
                        {
                            osmSharpTags.Add(tag.Key, tag.Value);
                        }
                        updatedWays.Add(way);
                    }
                    else if (obj is OpenedWayObject)
                    {
                        OpenedWayObject upd = (OpenedWayObject)obj;
                        Way way = upd.Way;
                        var osmSharpTags = new TagsCollection();
                        foreach (Sec.Tag tag in upd.Tags)
                        {
                            osmSharpTags.Add(tag.Key, tag.Value);
                        }
                        updatedWays.Add(way);
                    }
                }
            }
            Console.WriteLine(updatedWays.Count());
            foreach (var updatedWay in updatedWays)
            {
                var originalWay = ways.FirstOrDefault(w => w.Id == updatedWay.Id);
                if (originalWay != null)
                {
                    ways.Remove(ways.Where(i => i.Id == originalWay.Id).Single());
                }
                ways.Add(updatedWay);
            }

            return ways;
        }
    }
}

using OsmSharp.Streams;
using OsmSharp;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;

namespace Digital_twin.File_tools
{
    public static class WriterOSM
    {
        public static void WriteOSM(string filepath, ObservableCollection<Node> nodes, 
            ObservableCollection<Way> ways, 
            ObservableCollection<Relation> relations) {
            using (var fileStream = new FileInfo(filepath).Open(FileMode.Create))
            {
                var target = new XmlOsmStreamTarget(fileStream);
                target.Initialize();
                foreach (Node node in nodes)
                {
                    target.AddNode(node);
                }
                foreach (Way w in ways)
                {
                    target.AddWay(w);
                }
                foreach (Relation r in relations)
                {
                    target.AddRelation(r);
                }
                target.Flush();
                target.Close();
            }
        }
        
     }
}
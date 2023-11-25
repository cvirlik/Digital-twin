using OsmSharp.Streams;
using OsmSharp;
using System.Collections.Generic;
using System.IO;

namespace Digital_twin.File_tools
{
    public class WriterOSM
    {
        public static void WriteOSM(string filepath, List<OsmGeo> geodata) {
            using (var fileStream = new FileInfo(filepath).Open(FileMode.Create))
            {
                var target = new XmlOsmStreamTarget(fileStream);
                target.Initialize();
                foreach (var osmElement in geodata)
                {
                    switch (osmElement)
                    {
                        case Node n:
                            target.AddNode(n);
                            break;
                        case Way w:
                            target.AddWay(w);
                            break;
                        case Relation r:
                            target.AddRelation(r);
                            break;
                    }
                }
                target.Flush();
                target.Close();
            }
        }
        
     }
}
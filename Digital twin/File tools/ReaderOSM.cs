using OsmSharp.Streams;
using OsmSharp;
using System;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;

namespace Digital_twin.File_tools
{
    public class ReaderOSM
    {
        private static void AddIfNotExists<T, TKey>(ObservableCollection<T> collection, T item, Func<T, TKey> keySelector)
        {
            if (!collection.Any(existingItem => keySelector(existingItem).Equals(keySelector(item))))
            {
                collection.Add(item);
            } else
            {
                Console.Error.WriteLine("Current element (id: {0}) alredy was added to collection! Check if dataset was correct!", keySelector(item).ToString());
            }
        }
        public static void ReadOSM(string filepath, 
            ObservableCollection<Node> Nodes, 
            ObservableCollection<Way> Ways, 
            ObservableCollection<Relation> Relations)
        {
            Console.WriteLine("Start reading file {0}...", filepath);

            using (var fileStream = new FileInfo(filepath).OpenRead())
            {
                var source = new XmlOsmStreamSource(fileStream); // Create an OSM stream source

                foreach (var element in source) // Go through objects
                {
         
                    Console.WriteLine($"Type: {element.Type}, Id: {element.Id}");

                    switch (element)
                    {
                        case Node node:
                            AddIfNotExists(Nodes, node, Node => Node.Id);
                            break;

                        case Way way:
                            AddIfNotExists(Ways, way, Way => Way.Id);
                            break;

                        case Relation relation:
                            AddIfNotExists(Relations, relation, Relation => Relation.Id);
                            break;
                    }
                }
            }
        }
    }
}

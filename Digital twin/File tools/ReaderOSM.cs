using OsmSharp.Streams;
using OsmSharp;
using System;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;
using Digital_twin.Dataset;
using Digital_twin.Dataset.Support;
using Digital_twin.Dataset.Types.Tertiary;
using Digital_twin.Dataset.Types;
using Digital_twin.Draw_tools;

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
        private static void ReadOSM(string filepath, 
            ObservableCollection<Node> Nodes, 
            ObservableCollection<Way> Ways, 
            ObservableCollection<Relation> Relations)
        {
            using (var fileStream = new FileInfo(filepath).OpenRead())
            {
                var source = new XmlOsmStreamSource(fileStream);

                foreach (var element in source)
                {
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
        public static void DataPartitioning(DataManager dataManager, string filename)
        {
            SupportingFileTools.ClearData(dataManager);
            ReadOSM(filename, dataManager.Nodes, dataManager.Ways, dataManager.Relations);
            var result = Parser.getBuildings(dataManager.Ways, dataManager.Relations);
            dataManager.FileReaded = true;
            dataManager.State = "Edit";

            ObservableCollection<Node> builldingNodes = new ObservableCollection<Node>();

            foreach (var building in result)
            {
                foreach (var item in Parser.getNodes(dataManager.Nodes, building.Nodes))
                {
                    builldingNodes.Add(item);
                }

            }

            dataManager.SetCanvasVariables(builldingNodes);

            foreach (var building in result)
            {
                dataManager.Buildings.Add(new Building(Parser.getNodes(dataManager.Nodes, building.Nodes), building, false));
            }

            dataManager.levelMax = dataManager.Buildings.Max(building => int.Parse(building.MaxLevel));
            dataManager.levelMin = dataManager.Buildings.Min(building => int.Parse(building.MinLevel));


            for (int i = dataManager.levelMin; i <= dataManager.levelMax; i++)
            {
                Level level = new Level(i);
                foreach (var building in dataManager.Buildings)
                {
                    level.AddObjects(building, true);
                }

                foreach (var room in Parser.getIndoorRoomLayout(dataManager.Nodes, dataManager.Ways, i.ToString()))
                {
                    level.AddObjects(room, true);
                }

                foreach (var way in Parser.getIndoorWayLayout(dataManager.Nodes, dataManager.Ways, i.ToString()))
                {
                    level.AddObjects(way, true);
                }

                foreach (var point in Parser.getDoorsLevel(dataManager.Nodes, i.ToString()))
                {
                    level.AddObjects(point, true);
                }

                dataManager.Levels.Add(level);
            }
            dataManager.SelectedLevel = dataManager.Levels.First();
        }
    }    
}

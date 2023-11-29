using Digital_twin.Dataset.Types.Primary;
using Digital_twin.Dataset.Types.Secondary;
using System;
using System.Collections.ObjectModel;

namespace Digital_twin.Dataset.Types
{
    public class Level
    {
        private ObservableCollection<Room> rooms { get; set; } = new ObservableCollection<Room>();
        private ObservableCollection<Building> buildings { get; set; } = new ObservableCollection<Building>();
        
        private int level;

        public Level(int _level, ObservableCollection<Room> _rooms, ObservableCollection<Building> _buildings)
        {
            rooms = _rooms;
            buildings = _buildings;
            level = _level;
        }
        public ObservableCollection<IShape> AllShapes
        {
            get
            {
                var allSegments = new ObservableCollection<IShape>();
                foreach (var room in Rooms)
                { 
                    allSegments.Add(room.Polygon);
                }
                foreach (var room in Rooms)
                {
                    foreach (var segment in room.SegmentsOuter)
                    {
                        allSegments.Add(segment);
                    }
                }
                foreach (var building in buildings)
                {
                    foreach (var segment in building.SegmentsOuter)
                    {
                        allSegments.Add(segment);
                    }
                }
                return allSegments;
            }
        }

        public int LevelNum { get { return level; } }  
        public string Name { get { return level.ToString(); } }  
        public ObservableCollection<Room> Rooms { get {  return rooms; } }
        public ObservableCollection<Building > Buildings { get { return buildings;} }
    }
}

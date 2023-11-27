using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_twin.Dataset.Types
{
    public class Level
    {
        private ObservableCollection<Room> rooms { get; set; } = new ObservableCollection<Room>();
        private ObservableCollection<Building> buildings { get; set; } = new ObservableCollection<Building>();
        // To Do: building lon0 lat0 point to rooms
        
        private int level;

        public Level(int _level, ObservableCollection<Room> _rooms, ObservableCollection<Building> _buildings)
        {
            rooms = _rooms;
            buildings = _buildings;
            level = _level;
        }
        public ObservableCollection<Segment> AllSegments
        {
            get
            {
                var allSegments = new ObservableCollection<Segment>();
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

using Digital_twin.Dataset;
using Digital_twin.Dataset.Support;
using Digital_twin.Dataset.Types;
using Digital_twin.Dataset.Types.Tertiary;
using System.Windows.Input;

namespace Digital_twin.RelayButtons
{
    public class AddLevel : RelayCommand
    {
        public AddLevel(DataManager dataManager) : base(obj => AddLevelCommand(obj, dataManager), obj => AddLevelCanExecute(obj, dataManager))
        {}

        private static void AddLevelCommand(object obj, DataManager dataManager)
        {
            Level level = new Level(dataManager.levelMax + 1);
            dataManager.levelMax++;
            foreach(Building b in dataManager.Buildings)
            {
                if (b.Way.Tags.ContainsKey("max_level"))
                    b.Way.Tags["max_level"] = dataManager.levelMax.ToString();
                else
                    b.Way.Tags.Add(new OsmSharp.Tags.Tag { Key = "max_level", Value = dataManager.levelMax.ToString() });
                if (b.Way.Tags.ContainsKey("min_level"))
                    b.Way.Tags["min_level"] = dataManager.levelMin.ToString();
                else
                    b.Way.Tags.Add(new OsmSharp.Tags.Tag { Key = "min_level", Value = dataManager.levelMin.ToString() });
            }
            foreach (var building in dataManager.Buildings)
            {
                level.AddObjects(building, true);
            }
            dataManager.Levels.Add(level);
        }

        private static bool AddLevelCanExecute(object obj, DataManager dataManager)
        {
            return dataManager.Readed;
        }
    }
}

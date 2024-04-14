using Digital_twin.Dataset;
using Digital_twin.Dataset.Support;
using Digital_twin.Dataset.Types;

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

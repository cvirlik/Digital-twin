using Digital_twin.Dataset.Support;
using Digital_twin.Dataset;

namespace Digital_twin.RelayButtons
{
    public class MoveLevelDown : RelayCommand
    {
        public MoveLevelDown(DataManager dataManager) : base(obj => MoveLevelDownCommand(obj, dataManager), obj => MoveLevelDownCanExecute(obj, dataManager))
        {}

        private static void MoveLevelDownCommand(object obj, DataManager dataManager)
        {
            if (dataManager.SelectedLevel != null && dataManager.Levels.Count > 1)
            {
                int currentIndex = dataManager.Levels.IndexOf(dataManager.SelectedLevel);
                if (currentIndex < dataManager.Levels.Count - 1)
                {
                    string name = dataManager.Levels[currentIndex + 1].Name;
                    dataManager.Levels[currentIndex + 1].Name = dataManager.SelectedLevel.Name;
                    dataManager.SelectedLevel.Name = name;
                    dataManager.Levels.Move(currentIndex, currentIndex + 1);
                }
            }
        }

        private static bool MoveLevelDownCanExecute(object obj, DataManager dataManager)
        {
            return (dataManager.SelectedLevel != null) && (dataManager.SelectedLevel.Name != dataManager.levelMax.ToString());
        }
    }
}

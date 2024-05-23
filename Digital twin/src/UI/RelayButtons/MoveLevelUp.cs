using Digital_twin.Dataset;
using Digital_twin.Dataset.Support;

namespace Digital_twin.RelayButtons
{
    public class MoveLevelUp : RelayCommand
    {
        public MoveLevelUp(DataManager dataManager) : base(obj => MoveLevelUpCommand(obj, dataManager), obj => MoveLevelUpCanExecute(obj, dataManager))
        {}

        private static void MoveLevelUpCommand(object obj, DataManager dataManager)
        {
            if (dataManager.SelectedLevel != null && dataManager.Levels.Count > 1)
            {
                int currentIndex = dataManager.Levels.IndexOf(dataManager.SelectedLevel);

                if (currentIndex > 0)
                {
                    string name = dataManager.Levels[currentIndex - 1].Name;
                    dataManager.Levels[currentIndex - 1].Name = dataManager.SelectedLevel.Name;
                    dataManager.SelectedLevel.Name = name;
                    dataManager.Levels.Move(currentIndex, currentIndex - 1);
                }
            }
        }

        private static bool MoveLevelUpCanExecute(object obj, DataManager dataManager)
        {
            return (dataManager.SelectedLevel != null) && (dataManager.SelectedLevel.Name != dataManager.levelMin.ToString());
        }
    }
}

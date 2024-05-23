using Digital_twin.Dataset;
using Digital_twin.Dataset.Support;

namespace Digital_twin.RelayButtons
{
    public class RemoveTag : RelayCommand
    {
        public RemoveTag(DataManager dataManager) : base(obj => RemoveTagCommand(obj, dataManager), obj => RemoveTagCanExecute(obj, dataManager))
        {}

        private static void RemoveTagCommand(object obj, DataManager dataManager)
        {
            dataManager.SelectedShape.obj.Tags.Remove(dataManager.SelectedTag);
        }

        private static bool RemoveTagCanExecute(object obj, DataManager dataManager)
        {
            return dataManager.SelectedTag != null;
        }
    }
}

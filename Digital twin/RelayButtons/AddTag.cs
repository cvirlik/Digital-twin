using Digital_twin.Dataset;
using Digital_twin.Dataset.Support;
using Digital_twin.Dataset.Types.Secondary;
using System.Linq;

namespace Digital_twin.RelayButtons
{
    public class AddTag : RelayCommand
    {
        public AddTag(DataManager dataManager) : base(obj => AddTagCommand(obj, dataManager), obj => AddTagCanExecute(obj, dataManager))
        {}

        private static void AddTagCommand(object obj, DataManager dataManager)
        {
            var key = dataManager.KeyText;
            var value = dataManager.ValueText;

            var existingTag = dataManager.SelectedShape.obj.Tags.FirstOrDefault(t => t.Key == key);
            if (existingTag != null)
            {
                existingTag.Value = value;
            }
            else
            {
                dataManager.SelectedShape.obj.Tags.Add(new Tag { Key = key, Value = value });
            }

            dataManager.KeyText = string.Empty;
            dataManager.ValueText = string.Empty;
        }

        private static bool AddTagCanExecute(object obj, DataManager dataManager)
        {
            return !string.IsNullOrWhiteSpace(dataManager.KeyText) && !string.IsNullOrWhiteSpace(dataManager.ValueText) && dataManager.Readed;
        }
    }
}

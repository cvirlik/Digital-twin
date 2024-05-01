using Digital_twin.Dataset.Support;
using Digital_twin.Dataset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Digital_twin.Dataset.Types.Secondary;

namespace Digital_twin.RelayButtons
{
    public class AddRecommendedTag : RelayCommand
    {
        public AddRecommendedTag(DataManager dataManager) : base(obj => AddTagCommand(obj, dataManager), obj => AddTagCanExecute(obj, dataManager))
        { }

        private static void AddTagCommand(object obj, DataManager dataManager)
        {
            var tag = obj as Tag;

            if (tag != null)
            {
                var existingTag = dataManager.SelectedShape.obj.Tags.FirstOrDefault(t => t.Key == tag.Key);
                if (existingTag != null)
                {
                    existingTag.Value = tag.Value;
                }
                else
                {
                    dataManager.SelectedShape.obj.Tags.Add(tag);
                }
            }
        }

        private static bool AddTagCanExecute(object obj, DataManager dataManager)
        {
            return obj is Tag;
        }
    }
}

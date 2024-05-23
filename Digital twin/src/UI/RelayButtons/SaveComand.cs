using Digital_twin.Dataset;
using Digital_twin.Dataset.Support;
using Digital_twin.Dataset.Types;
using Digital_twin.File_tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Digital_twin.RelayButtons
{
    public class SaveComand : RelayCommand
    {
        public SaveComand(DataManager dataManager) : base(obj => SavingCommand(obj, dataManager), obj => SavingCanExecute(obj, dataManager))
        {}

        private static void SavingCommand(object obj, DataManager dataManager)
        {
           WriterOSM.WriteOSM("new.osm", MergeTools.mergeNodes(dataManager.Levels, dataManager.Nodes), 
                MergeTools.mergeWays(dataManager.Levels, dataManager.Ways), dataManager.Relations);
            
            //WriterOSM.WriteOSM("new.osm", dataManager.Nodes, dataManager.Ways, dataManager.Relations);
            MessageBox.Show("Your work was saved!", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private static bool SavingCanExecute(object obj, DataManager dataManager)
        {
            return dataManager.Readed;
        }
    }
}

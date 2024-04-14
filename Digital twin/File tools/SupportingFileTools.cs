using Digital_twin.Dataset;
using System;
using System.Windows;

namespace Digital_twin.File_tools
{
    public static class SupportingFileTools
    {
        public static bool ValidateOSMFileType(string path)
        {
            string extension = System.IO.Path.GetExtension(path);

            if (extension == null || extension.ToLower() != ".osm")
            {
                MessageBox.Show("The selected file is not an OSM file.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool ValidateImageFileType(string path)
        {
            string extension = System.IO.Path.GetExtension(path);
            Console.WriteLine(extension.ToLower());
            Console.WriteLine(extension.ToLower() == ".png");
            if (extension == null || (extension.ToLower() != ".png" && extension.ToLower() != ".jpg"
                && extension.ToLower() != ".jpeg"))
            {
                MessageBox.Show("The selected file is not an valid image file.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            else
            {
                return true;
            }
        }
        public static void ClearData(DataManager dataManager)
        {
            dataManager.Levels.Clear();
            dataManager.Buildings.Clear();
            dataManager.Relations.Clear();
            dataManager.Ways.Clear();
            dataManager.Nodes.Clear();
            dataManager.SelectedLevel = null;
            dataManager.SelectedShape = null;
            dataManager.SelectedTag = null;
        }
    }
}

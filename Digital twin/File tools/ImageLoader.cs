using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Digital_twin.File_tools
{
    public static class ImageLoader
    {
        public static BitmapImage LoadImage(string filepath)
        {
            if (SupportingFileTools.ValidateImageFileType(filepath))
            {
                BitmapImage bitmapImage = new BitmapImage();

                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(filepath); 
                bitmapImage.EndInit();

                return bitmapImage;
            }
            else
            {
                return null;
            }
        }
    }
}

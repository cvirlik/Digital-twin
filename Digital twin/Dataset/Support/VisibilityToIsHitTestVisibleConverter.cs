using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Digital_twin.Dataset.Support
{
    public class VisibilityToIsHitTestVisibleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var state = values.Last() as string;

            if (state == "ImageTransform")
            {
                return false;
            }

            if (values.Take(values.Length - 1).Cast<Visibility>().Any(visibility => visibility == Visibility.Visible))
            {
                return false;
            }

            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

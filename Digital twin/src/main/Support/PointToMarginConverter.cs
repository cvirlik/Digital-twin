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
    public class PointToMarginConverter : IMultiValueConverter
    {
        // TODO: rewrite to width/2 offset
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is double left && values[1] is double top)
            {
                return new Thickness(left, top, 0, 0);
            }
            return new Thickness();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

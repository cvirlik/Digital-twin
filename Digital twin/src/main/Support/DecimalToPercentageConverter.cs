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
    public class DecimalToPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double decimalValue)
            {
                return $"{(int)(decimalValue * 100)}%";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            if (strValue != null)
            {
                double result;
                if (double.TryParse(strValue.Replace("%", ""), out result))
                {
                    return result / 100;
                }
            }
            return DependencyProperty.UnsetValue;
        }
    }
}

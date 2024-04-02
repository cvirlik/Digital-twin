using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Digital_twin.Dataset.Support
{
    public class ObservableCollectionToPointCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var points = value as ObservableCollection<Point>;
            if (points != null)
            {
                var pointCollection = new PointCollection();
                foreach (var point in points)
                {
                    pointCollection.Add(point);
                }
                return pointCollection;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

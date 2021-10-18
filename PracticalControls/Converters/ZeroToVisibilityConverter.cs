using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PracticalControls.Converters
{
    public class ZeroToVisibilityConverter : IValueConverter
    {
        public bool IsHidden { get; set; }

        public static ZeroToVisibilityConverter Instance { get; set; } = new ZeroToVisibilityConverter();
        public static ZeroToVisibilityConverter InstanceWidthHidden { get; set; } = new ZeroToVisibilityConverter() { IsHidden = true };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int i = (int)value;

            if (IsHidden)
                return i == 0 ? Visibility.Hidden : Visibility.Visible;
            else
                return i == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

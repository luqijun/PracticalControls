using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PracticalControls.Converters
{
    public class LevelToWidthConverter : IValueConverter
    {
        public static LevelToWidthConverter Instance { get; set; } = new LevelToWidthConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int width = (int)value;

            return width * 12;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

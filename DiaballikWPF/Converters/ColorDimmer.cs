using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DiaballikWPF.Converters {
    public class ColorDimmer : IValueConverter {

        public byte Percent { get; set; } = 70;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            byte Dim(byte x) {
                return (byte) (x * Percent / 100);
            }

            if (value is Color c) {
                return Color.FromRgb(Dim(c.R), Dim(c.G), Dim(c.B));
            }

            return value;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }
}
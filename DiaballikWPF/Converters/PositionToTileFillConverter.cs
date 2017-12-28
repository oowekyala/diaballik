using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Diaballik.Core;

namespace DiaballikWPF.Converters {
    public class PositionToTileFillConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Position2D p) {
                return p.X % 2 == p.Y % 2
                    ? new SolidColorBrush(Colors.Bisque)
                    : new SolidColorBrush(Colors.Peru);
            }

            return new SolidColorBrush(Colors.Bisque);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }
}
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Diaballik.Core;
using static DiaballikWPF.Converters.MediaColorToDrawingColorConverter;
using Color = System.Drawing.Color;

namespace DiaballikWPF.Converters {
    public class PositionToTileFillConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Position2D p) {
                return p.X % 2 == p.Y % 2
                    ? new SolidColorBrush(DrawingToMedia(Color.Bisque))
                    : new SolidColorBrush(DrawingToMedia(Color.Peru));
            }

            return new SolidColorBrush(DrawingToMedia(Color.Bisque));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }
}
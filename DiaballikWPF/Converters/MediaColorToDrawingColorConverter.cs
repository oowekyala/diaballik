using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace DiaballikWPF.Converters {
    public class MediaColorToDrawingColorConverter : IValueConverter {

        private static System.Drawing.Color MediaToDrawing(System.Windows.Media.Color val) {
            return System.Drawing.Color.FromArgb(val.A, val.R, val.G, val.B);
        }

        private static System.Windows.Media.Color DrawingToMedia(System.Drawing.Color val) {
            return System.Windows.Media.Color.FromArgb(val.A, val.R, val.G, val.B);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            switch (value) {
                case System.Windows.Media.Color val:
                    return MediaToDrawing(val);
                case System.Drawing.Color d:
                    return DrawingToMedia(d);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture) {
            return null;
        }
    }
}
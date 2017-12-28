using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DiaballikWPF.Converters {
    /// <summary>
    ///     Dims a color by a specific percentage.
    /// </summary>
    public class ColorDimmer : IValueConverter {
        /// Conversion ratio
        public byte Ratio { get; set; } = 70;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Color c) {
                return ConvertUtil.DimColor(Ratio, c);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }
}
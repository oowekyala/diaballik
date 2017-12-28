using System;
using System.Globalization;
using System.Windows.Data;

namespace DiaballikWPF.Converters {
    public class RatioConverter : IValueConverter {

        /// Percentage with which the value is multiplied, 100% is 100 
        public double Ratio { get; set; } = 100;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return ((double) value) * Ratio / 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }
}
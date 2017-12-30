using System.Windows.Media;
using static System.Math;

namespace DiaballikWPF.Converters {
    /// <summary>
    ///     Extension methods for QuickConverter.
    /// </summary>
    public static class ConvertUtil {
        /// <summary>
        ///     Returns the same color dimmed by percent%.
        /// </summary>
        public static Color DimColor(this Color c, int percent) {
            byte Dim(byte x) {
                return (byte) Min(256, Max(0, x * percent / 100));
            }

            return Color.FromRgb(Dim(c.R), Dim(c.G), Dim(c.B));
        }

        public static int ToArgb(this Color color) {
            return (color.A << 24) | (color.R << 16) | (color.G << 8) | color.B;
        }

        public static Color GetComplement(this Color c) {
            return Color.FromRgb((byte) ~c.R, (byte) ~c.G, (byte) ~c.B);
        }
    }
}
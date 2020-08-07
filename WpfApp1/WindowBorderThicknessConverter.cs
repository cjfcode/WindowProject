using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace WpfApp1
{
    /// <summary>
    /// Special converter used in custom Window styles that use WindowChrome.
    /// </summary>
    [ValueConversion(typeof(Thickness), typeof(Thickness))]
    public class WindowBorderThicknessConverter : MarkupExtension, IValueConverter
    {
        private static WindowBorderThicknessConverter converter;

        /// <summary>
        /// Creates a new instance of the <see cref="WindowBorderThicknessConverter"/> class.
        /// </summary>
        public WindowBorderThicknessConverter()
        {
        }

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return converter ??= new WindowBorderThicknessConverter();
        }

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;

            var thickness = (Thickness)value;

            thickness.Bottom /= 2;

            return thickness;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
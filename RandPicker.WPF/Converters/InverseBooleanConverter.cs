using System;
using System.Globalization;
using System.Windows.Data;

namespace RandPicker.WPF.Converters
{
    /// <summary>
    /// 布尔值反转转换器
    /// </summary>
    public class InverseBooleanConverter : IValueConverter
    {
        /// <summary>
        /// 将布尔值反转
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return value;
        }

        /// <summary>
        /// 将布尔值反转
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return value;
        }
    }
}
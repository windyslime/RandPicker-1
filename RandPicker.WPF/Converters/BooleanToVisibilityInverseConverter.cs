using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RandPicker.WPF.Converters
{
    /// <summary>
    /// 布尔值到可见性的反转转换器
    /// </summary>
    public class BooleanToVisibilityInverseConverter : IValueConverter
    {
        /// <summary>
        /// 将布尔值反转后转换为可见性
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        /// <summary>
        /// 将可见性转换为反转后的布尔值
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility != Visibility.Visible;
            }
            return false;
        }
    }
}
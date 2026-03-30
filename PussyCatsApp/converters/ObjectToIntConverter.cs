using Microsoft.UI.Xaml.Data;
using System;

namespace PussyCatsApp.converters
{
    public class ObjectToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int intValue && parameter is string paramStr && int.TryParse(paramStr, out int paramInt))
            {
                return intValue == paramInt;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isChecked && isChecked && parameter is string paramStr && int.TryParse(paramStr, out int paramInt))
            {
                return paramInt;
            }
            return null;
        }
    }
}

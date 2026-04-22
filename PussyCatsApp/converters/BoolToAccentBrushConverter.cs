using System;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace PussyCatsApp.Converters
{
    public class BoolToAccentBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isSelected && isSelected)
            {
                // Return accent brush for selected state
                return (Brush)Application.Current.Resources["AccentFillColorDefaultBrush"];
            }
            // Return default border brush for unselected state
            return (Brush)Application.Current.Resources["DividerStrokeColorDefaultBrush"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace PussyCatsApp.Converters
{
    public class BoolToHighlightBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isSelected && isSelected)
            {
                // Return a subtle highlight background for selected state
                return (Brush)Application.Current.Resources["AccentFillColorTertiaryBrush"];
            }
            // Return default card background for unselected state
            return (Brush)Application.Current.Resources["CardBackgroundFillColorDefaultBrush"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

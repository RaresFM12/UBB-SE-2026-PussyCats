using Microsoft.UI.Xaml.Data;
using System;

namespace PussyCatsApp.converters
{
    public class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Parameter should be in format "disabledOpacity|enabledOpacity"
            // e.g., "0.6|1.0"
            string[] opacities = parameter?.ToString().Split('|') ?? new[] { "0.5", "1.0" };

            double disabledOpacity = double.TryParse(opacities[0], out var d) ? d : 0.5;
            double enabledOpacity = opacities.Length > 1 && double.TryParse(opacities[1], out var e) ? e : 1.0;

            if (value is bool boolValue)
            {
                return boolValue ? disabledOpacity : enabledOpacity;
            }
            return enabledOpacity;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

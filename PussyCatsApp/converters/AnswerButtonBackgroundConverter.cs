using Microsoft.UI.Xaml.Data;
using System;

namespace PussyCatsApp.converters
{
    public class AnswerButtonOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int selectedAnswer && parameter is string paramStr && int.TryParse(paramStr, out int paramInt))
            {
                if (selectedAnswer == paramInt)
                {
                    return 1.0; // Full opacity for selected
                }
            }
            return 0.5; // Half opacity for unselected
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

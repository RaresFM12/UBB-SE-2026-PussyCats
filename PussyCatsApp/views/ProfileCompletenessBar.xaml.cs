using Microsoft.UI.Xaml.Controls;
namespace PussyCatsApp.Views
{
    /// <summary>
    /// User control that displays a visual progress bar indicating profile completeness percentage,
    /// along with a prompt suggesting the next field to complete.
    /// </summary>
    public sealed partial class ProfileCompletenessBar : UserControl
    {
        public ProfileCompletenessBar()
        {
            this.InitializeComponent();
        }
        public void Update(int percentage, string promptText)
        {
            barProgress.Value = percentage;
            lblPercentage.Text = $"{percentage}%";
            lblPrompt.Text = promptText;
        }
    }
}
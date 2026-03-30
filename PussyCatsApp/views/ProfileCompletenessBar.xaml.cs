using Microsoft.UI.Xaml.Controls;
namespace PussyCatsApp.views
{
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
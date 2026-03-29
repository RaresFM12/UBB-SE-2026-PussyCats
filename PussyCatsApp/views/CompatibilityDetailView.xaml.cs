using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.models;
using PussyCatsApp.viewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PussyCatsApp.views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CompatibilityDetailView : Page
    {
        private CompatibilityDetailViewModel viewModel;

        public CompatibilityDetailView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            viewModel = (CompatibilityDetailViewModel)e.Parameter;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDetail();
        }

        private void LoadDetail()
        {
            lblRoleName.Text = viewModel.GetRoleName();

            double score = viewModel.GetMatchScore();
            if (score == -1)
                lblMatchScore.Text = "Score: Insufficient Data";
            else
                lblMatchScore.Text = "Match Score: " + Math.Round(score, 1) + "%";

            ShowSuggestions();
        }

        private void ShowSuggestions()
        {
            List<Suggestion> suggestions = viewModel.GetSuggestions();

            if (suggestions == null || suggestions.Count == 0)
            {
                lstSuggestions.Visibility = Visibility.Collapsed;
                lblNoSuggestions.Visibility = Visibility.Visible;
                return;
            }

            List<object> displayItems = new List<object>();
            foreach (Suggestion s in suggestions)
            {
                displayItems.Add(new
                {
                    s.SkillName,
                    s.GroupName,
                    GainDisplay = "Potential gain: +" + Math.Round(s.GainScore, 1) + "%"
                });
            }

            lstSuggestions.ItemsSource = displayItems;
            lstSuggestions.Visibility = Visibility.Visible;
            lblNoSuggestions.Visibility = Visibility.Collapsed;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

        public async Task ShowError(string message)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Error";
            dialog.Content = message;
            dialog.CloseButtonText = "OK";
            dialog.XamlRoot = this.XamlRoot;
            await dialog.ShowAsync();
        }

        public async Task ShowMessage(string message)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Information";
            dialog.Content = message;
            dialog.CloseButtonText = "OK";
            dialog.XamlRoot = this.XamlRoot;
            await dialog.ShowAsync();
        }
    }
}

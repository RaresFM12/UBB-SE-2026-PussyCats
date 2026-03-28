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
    public sealed partial class CompatibilityOverviewView : Page
    {
        private CompatibilityOverviewViewModel viewModel;

        public CompatibilityOverviewView(CompatibilityOverviewViewModel viewModel)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.LoadAllRoles();

            if (!string.IsNullOrEmpty(viewModel.GetErrorMessage()))
            {
                await ShowError(viewModel.GetErrorMessage());
                return;
            }

            List<RoleResult> results = viewModel.GetRoleResults();
            List<object> displayItems = new List<object>();

            foreach (RoleResult result in results)
            {
                displayItems.Add(new
                {
                    Result = result,
                    DisplayName = FormatRoleName(result.JobRole.ToString()),
                    DisplayScore = result.MatchScore == -1 ? 0 : result.MatchScore,
                    DisplayPercentage = result.MatchScore == -1 ? "Insufficient Data" : Math.Round(result.MatchScore, 1) + "%"
                });
            }

            lstRoles.ItemsSource = displayItems;
        }

        private string FormatRoleName(string raw)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (char c in raw)
            {
                if (char.IsUpper(c) && sb.Length > 0)
                    sb.Append(' ');
                sb.Append(c);
            }
            return sb.ToString();
        }

        private void lstRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstRoles.SelectedItem == null) return;

            dynamic selected = lstRoles.SelectedItem;
            RoleResult result = selected.Result;

            viewModel.OnRoleSelected(result.JobRole);
            NavigateToDetail(viewModel.GetSelectedResult());
        }

        private void NavigateToDetail(RoleResult result)
        {
            CompatibilityDetailViewModel detailVm = new CompatibilityDetailViewModel();
            detailVm.LoadResult(result);
            Frame.Navigate(typeof(CompatibilityDetailView), detailVm);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //Window window = (Window)this.XamlRoot.Content;
            //window.Close();
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

        public async void ShowInsufficientData()
        {
            await ShowMessage("Role data is unavailable and the score cannot be calculated for this role.");
        }
    }
}

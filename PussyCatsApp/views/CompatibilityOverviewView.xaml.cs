using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.models;
using PussyCatsApp.viewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PussyCatsApp.views
{
    public sealed partial class CompatibilityOverviewView : Page
    {
        private CompatibilityOverviewViewModel viewModel;

        public CompatibilityOverviewView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            viewModel = e.Parameter as CompatibilityOverviewViewModel;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (viewModel == null)
            {
                await ShowError("CompatibilityOverviewViewModel was not provided.");
                return;
            }

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
                string formattedName = "";
                if (result.JobRole == JobRole.UIUXDesigner)
                    formattedName = "UI/UX Designer";
                else if (result.JobRole == JobRole.AIMLEngineer)
                    formattedName = "AI/ML Engineer";
                else
                    formattedName = FormatRoleName(result.JobRole.ToString());

                displayItems.Add(new
                {
                    Result = result,
                    DisplayName = FormatRoleName(formattedName),
                    DisplayScore = result.MatchScore == -1 ? 0 : result.MatchScore,
                    DisplayPercentage = result.MatchScore == -1
                        ? "Insufficient Data"
                        : Math.Round(result.MatchScore, 1) + "%"
                });
            }

            lstRoles.ItemsSource = displayItems;
        }

        private string FormatRoleName(string raw)
        {
            var sb = new System.Text.StringBuilder();

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
            if (lstRoles.SelectedItem == null || viewModel == null)
                return;

            dynamic selected = lstRoles.SelectedItem;
            RoleResult result = selected.Result;

            viewModel.OnRoleSelected(result.JobRole);
            NavigateToDetail(viewModel.GetSelectedResult());
        }

        private void NavigateToDetail(RoleResult result)
        {
            if (result == null)
                return;

            CompatibilityDetailViewModel detailVm = new CompatibilityDetailViewModel();
            detailVm.LoadResult(result);
            Frame.Navigate(typeof(CompatibilityDetailView), detailVm);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (Frame != null && Frame.CanGoBack)
                Frame.GoBack();
        }

        public async Task ShowError(string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();
        }

        public async Task ShowMessage(string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Information",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();
        }

        public async void ShowInsufficientData()
        {
            await ShowMessage("Role data is unavailable and the score cannot be calculated for this role.");
        }
    }
}
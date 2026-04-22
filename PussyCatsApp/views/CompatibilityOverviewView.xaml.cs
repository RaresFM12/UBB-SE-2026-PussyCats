using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.services;
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
            int userId = (int)e.Parameter;

            IUserSkillRepository userSkillRepo = new UserSkillRepository(DatabaseConfiguration.GetConnectionString());
            ISkillGroupRepository skillGroupRepository = new SkillGroupRepository();
            ICompatibilityService compatibilityService = new CompatibilityService(userSkillRepo, skillGroupRepository);
            viewModel = new CompatibilityOverviewViewModel(userId, compatibilityService);
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
            var newString = new System.Text.StringBuilder();

            foreach (char c in raw)
            {
                if (char.IsUpper(c) && newString.Length > 0)
                    newString.Append(' ');

                newString.Append(c);
            }

            return newString.ToString();
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

            Frame.Navigate(typeof(CompatibilityDetailView), result);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (Frame != null && Frame.CanGoBack)
                Frame.GoBack();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
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
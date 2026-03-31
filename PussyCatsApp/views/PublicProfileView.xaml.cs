using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.models;
using PussyCatsApp.viewModels;
using PussyCatsApp.services;
using PussyCatsApp.repositories;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PussyCatsApp.views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PublicProfileView : Page
    {
        private readonly PublicProfileViewModel viewModel;
        public PublicProfileView()
        {
            this.InitializeComponent();

            var skillTestRepo = new SkillTestRepository();
            var userProfileRepo = new UserProfileRepository();
            var userProfileService = new UserProfileService(userProfileRepo, skillTestRepo);
            viewModel = new PublicProfileViewModel(userProfileService);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is UserProfile profile)
            {
                viewModel.LoadPublicProfileCommand(profile.UserId);
                if (viewModel.IsAvailable)
                {
                    ProfileContentPanel.Visibility = Visibility.Visible;
                    ProfileUnavailableTextBlock.Visibility = Visibility.Collapsed;
                    UpdateUI(profile);
                }
                else
                {
                    ProfileContentPanel.Visibility = Visibility.Collapsed;
                    ProfileUnavailableTextBlock.Visibility = Visibility.Visible;
                }
              
            }
        }

        private void UpdateUI(UserProfile profile)
        {
            
            FirstNameLabel.Text = profile.FirstName;
            LastNameLabel.Text = profile.LastName;
            LevelLabel.Text = $"Level {profile.UserLevel.Title}";

            EmailLabel.Text = profile.Email;
            PhoneLabel.Text = profile.PhoneNumber;
            GenderLabel.Text = $"Gender: {profile.Gender}";

            UniversityLabel.Text = $"University: {profile.University}";
            GradYearLabel.Text = $"Graduation Year: {profile.ExpectedGraduationYear}";
            CountryLabel.Text = $"Country:  {profile.Country}";
            AddressLabel.Text = $"Address: {profile.Address}";

            GithubLink.NavigateUri = new System.Uri(profile.GitHub ?? "https://github.com");
            LinkedinLink.NavigateUri = new System.Uri(profile.LinkedIn ?? "https://linkedin.com");

            SkillTestsContainer.Children.Clear();
            foreach (var test in viewModel.Tests)
            {
                var row = new TextBlock
                {
                    Text = $"• {test.Name}: {test.Score}%",
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White),
                    Margin = new Thickness(0, 5, 0, 5)
                };
                SkillTestsContainer.Children.Add(row);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }
    }
}

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
using PussyCatsApp.Repositories;

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
            var userProfileService = new UserProfileService(skillTestRepo, userProfileRepo);
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

        private Uri GetValidUri(string url, string fallback)
        {
            if (string.IsNullOrWhiteSpace(url))
                return new Uri(fallback);

            if (Uri.TryCreate(url, UriKind.Absolute, out Uri result) &&
                (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps))
            {
                return result;
            }

            return new Uri(fallback);
        }
        private void UpdateUI(UserProfile profile)
        {
            
            FirstNameLabel.Text = profile.FirstName;
            LastNameLabel.Text = profile.LastName;
            LevelLabel.Text = $"Level {profile.UserLevel.Title}";

            EmailLabel.Text = profile.Email;
            PhoneLabel.Text = profile.PhoneNumber;
            GenderLabel.Text = $"{profile.Gender}";

            UniversityLabel.Text = $"{profile.University}";
            GradYearLabel.Text = $"{profile.ExpectedGraduationYear}";
            CountryLabel.Text = $"{profile.Country}";
            AddressLabel.Text = $"{profile.Address}";

            GithubLink.NavigateUri = GetValidUri(profile.GitHub, "https://github.com");
            LinkedinLink.NavigateUri = GetValidUri(profile.LinkedIn, "https://linkedin.com");

            if (!string.IsNullOrEmpty(viewModel.Profile.ProfilePicture))
            {
                ProfilePhoto.Source =
                    new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(
                        new Uri(viewModel.Profile.ProfilePicture));
            }
            else
            {
                ProfilePhoto.Source = null;
            }

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

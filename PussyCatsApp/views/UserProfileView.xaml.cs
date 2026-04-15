using System;
using System.Diagnostics;
using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.Models;
using PussyCatsApp.ViewModels;

namespace PussyCatsApp.Views
{
    /// <summary>
    /// Page that displays the user's profile overview, including personal information, avatar,
    /// account status, profile completeness, XP/level progress, and navigation to related features
    /// such as editing profile, viewing CV, taking personality tests, and accessing matchmaking history.
    /// </summary>
    public sealed partial class UserProfileView : Page
    {
        private int currentUserId = 1;
        public UserProfileViewModel ViewModel { get; private set; }
        private bool isBinding = false;

        public UserProfileView()
        {
            this.InitializeComponent();

            ViewModel = new UserProfileViewModel();

            ViewModel.OnLevelUpdated += RenderLevelDisplay;
            this.DataContext = ViewModel;

            btnEdit.Click += OnEditProfileClick;
            btnOldTests.Click += OnGoToOldTestsClick;
            btnPublicProfile.Click += OnSeePublicProfileClick;
            btnViewDocuments.Click += OnViewDocumentsClick;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            BindData();
        }

        private async void BindData()
        {
            isBinding = true;

            int dummyUserId = 1;
            await ViewModel.LoadUserAsync(dummyUserId);
            if (!string.IsNullOrEmpty(ViewModel.ErrorMessage))
            {
                lblError.Text = ViewModel.ErrorMessage;
                isBinding = false;
                return;
            }

            if (ViewModel.UserProfilePublic != null)
            {
                lblFirstName.Text = $"First Name: {ViewModel.UserProfilePublic.FirstName}";
                lblLastName.Text = $"Last Name: {ViewModel.UserProfilePublic.LastName}";
                lblEmail.Text = $"Email: {ViewModel.UserProfilePublic.Email}";
                lblPhone.Text = $"Phone: {ViewModel.UserProfilePublic.PhoneNumber}";
                lblGithubAccount.Text = $"GitHub: {ViewModel.UserProfilePublic.GitHub}";
                lblLinkedinAccount.Text = $"LinkedIn: {ViewModel.UserProfilePublic.LinkedIn}";

                string displayGender = ViewModel.UserProfilePublic.Gender;
                if (string.IsNullOrEmpty(displayGender) || (displayGender != "Male" && displayGender != "Female"))
                {
                    displayGender = "Not specified";
                }

                Debug.WriteLine($"[BindData] ErrorMessage: '{ViewModel.ErrorMessage}'");
                Debug.WriteLine($"[BindData] userProfilePrivate is null: {ViewModel.UserProfilePublic == null}");

                lblGender.Text = $"Gender: {displayGender}";
                lblUniversity.Text = $"University: {ViewModel.UserProfilePublic.University}";
                lblCountry.Text = $"Country: {ViewModel.UserProfilePublic.Country}";
                // lblAddress.Text = $"Address: {ViewModel.userProfilePrivate.Address}";
                lblCity.Text = $"City: {ViewModel.UserProfilePublic.City}";
                lblGraduationYear.Text = $"Graduation Year: {ViewModel.UserProfilePublic.ExpectedGraduationYear}";
                lblFreshness.Text = ViewModel.FreshnessText;

                string testResultDisplay = "Not taken yet";
                if (!string.IsNullOrEmpty(ViewModel.UserProfilePublic.PersonalityTestResult))
                {
                    // Try parsing the string to the JobRole enum
                    if (Enum.TryParse<JobRole>(ViewModel.UserProfilePublic.PersonalityTestResult, out var jobRole))
                    {
                        var converter = new Converters.JobRoleToDisplayNameConverter();

                        // Convert the enum value to the display string
                        testResultDisplay = converter.Convert(jobRole, typeof(string), null, string.Empty).ToString();
                    }
                    else
                    {
                        // Fallback if the string couldn't be parsed
                        testResultDisplay = ViewModel.UserProfilePublic.PersonalityTestResult;
                    }
                }
                lblPersonalityTestResult.Text = $"Personality Test Result: {testResultDisplay}";

                LevelTitleText.Text = "Level 2 - Apprentice";
                XpProgressBar.Maximum = 250;
                XpProgressBar.Value = 150;
                XpCountText.Text = "150 / 250 XP";

                chkAccountStatus.IsOn = ViewModel.UserProfilePublic.ActiveAccount;

                if (!string.IsNullOrEmpty(ViewModel.UserProfilePublic.ProfilePicture))
                {
                    pbAvatar.ProfilePicture =
                        new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(
                            new Uri(ViewModel.UserProfilePublic.ProfilePicture));
                }
                else
                {
                    pbAvatar.ProfilePicture = null;
                }

                if (!string.IsNullOrEmpty(ViewModel.UserProfilePublic.PersonalityTestResult))
                {
                    btnPersonalityTest.Content = "Retake Personality Test";
                }
                else
                {
                    btnPersonalityTest.Content = "Take Personality Test";
                }

                completenessBar.Update(ViewModel.CompletenessPercentage, ViewModel.NextEmptyFieldPrompt);

                ViewModel.RecalculateLevelCommand();
                RenderLevelDisplay();
            }
            else
            {
                return;
            }

            isBinding = false;
        }

        private void RenderLevelDisplay()
        {
            if (ViewModel.UserProfilePublic == null)
            {
                return;
            }
            if (ViewModel.UserProfilePublic.UserLevel == null)
            {
                return;
            }
            LevelTitleText.Text = $"Level {ViewModel.UserProfilePublic.UserLevel.LevelNumber} — {ViewModel.UserProfilePublic.UserLevel.Title}";

            XpProgressBar.Value = ViewModel.UserProfilePublic.UserLevel.GetProgressPercent(ViewModel.TotalXP);

            int xpToNext = ViewModel.UserProfilePublic.UserLevel.GetXPToNextLevel(ViewModel.TotalXP);
            XpCountText.Text = xpToNext > 0
                ? $"{ViewModel.TotalXP} XP — {xpToNext} XP needed for next level"
                : $"{ViewModel.TotalXP} XP — Max level reached!";
        }

        private async void OnAvatarUploadClick(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainAppWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                using (var stream = await file.OpenStreamForReadAsync())
                {
                    ViewModel.UploadAvatarCommand(stream, file.Name);
                    BindData();
                }
            }
        }

        private void OnAvatarRemoveClick(object sender, RoutedEventArgs e)
        {
            ViewModel.RemoveAvatarCommand();
            BindData();
        }

        private void OnStatusToggle(object sender, RoutedEventArgs e)
        {
            if (isBinding)
            {
                return;
            }

            if (ViewModel?.UserProfilePublic != null)
            {
                ViewModel.ToggleAccountStatusCommand();
                BindData();
            }
        }

        private void OnEditProfileClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.UserProfilePublic != null)
            {
                Frame.Navigate(typeof(ProfileFormView), ViewModel.UserProfilePublic);
            }
            else
            {
                Frame.Navigate(typeof(ProfileFormView));
            }
        }

        private void OnPreviewCVClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.UserProfilePublic != null)
            {
                Frame.Navigate(typeof(ExportCVView), ViewModel.UserProfilePublic.UserId);
            }
        }

        private void OnGoToOldTestsClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.UserProfilePublic == null)
            {
                return;
            }

            // ViewModel.userProfilePrivate.userId = currentUserId;
            this.Frame.Navigate(typeof(TestDashboardView), ViewModel.UserProfilePublic);
        }

        private void OnSeePublicProfileClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.UserProfilePublic == null)
            {
                return;
            }
            this.Frame.Navigate(typeof(PublicProfileView), ViewModel.UserProfilePublic);
        }

        private void OnCompatibilityAnalyzerClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CompatibilityOverviewView), currentUserId);
        }

        private void OnPersonalityTestClick(object sender, RoutedEventArgs e)
        {
            ViewModel.TakePersonalityTestCommand();
        }

        private void OnViewDocumentsClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DocumentsListView));
        }

        private void OnMatchmakingHistoryClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MatchHistoryView));
        }
    }
}
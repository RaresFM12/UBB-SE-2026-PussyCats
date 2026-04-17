using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.models;
using PussyCatsApp.viewModels;
using System;
using System.Diagnostics;
using System.IO;

namespace PussyCatsApp.views
{
    public sealed partial class UserProfileView : Page
    {
        private int currentUserId = 1;
        public UserProfileViewModel viewModel { get; private set; }
        private bool _isBinding = false;

        public UserProfileView()
        {
            this.InitializeComponent();

            viewModel = new UserProfileViewModel();

            viewModel.OnLevelUpdated += renderLevelDisplay;
            this.DataContext = viewModel;

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
            _isBinding = true;

            int dummyUserId = 1;
            await viewModel.LoadUserAsync(dummyUserId);

            if (!string.IsNullOrEmpty(viewModel.ErrorMessage))
            {
                lblError.Text = viewModel.ErrorMessage;
                _isBinding = false;
                return;
            }

            if (viewModel.UserProfile != null)
            {
                lblFirstName.Text = $"First Name: {viewModel.UserProfile.FirstName}";
                lblLastName.Text = $"Last Name: {viewModel.UserProfile.LastName}";
                lblEmail.Text = $"Email: {viewModel.UserProfile.Email}";
                lblPhone.Text = $"Phone: {viewModel.UserProfile.PhoneNumber}";
                lblGithubAccount.Text = $"GitHub: {viewModel.UserProfile.GitHub}";
                lblLinkedinAccount.Text = $"LinkedIn: {viewModel.UserProfile.LinkedIn}";

                string displayGender = viewModel.UserProfile.Gender;
                if (string.IsNullOrEmpty(displayGender) || (displayGender != "Male" && displayGender != "Female"))
                    displayGender = "Not specified";

                Debug.WriteLine($"[BindData] ErrorMessage: '{viewModel.ErrorMessage}'");
                Debug.WriteLine($"[BindData] userProfile is null: {viewModel.UserProfile == null}");

                lblGender.Text = $"Gender: {displayGender}";
                lblUniversity.Text = $"University: {viewModel.UserProfile.University}";
                lblCountry.Text = $"Country: {viewModel.UserProfile.Country}";
                //lblAddress.Text = $"Address: {viewModel.UserProfile.Address}";
                lblCity.Text = $"City: {viewModel.UserProfile.City}";
                lblGraduationYear.Text = $"Graduation Year: {viewModel.UserProfile.ExpectedGraduationYear}";
                lblFreshness.Text = viewModel.FreshnessText;

                string testResultDisplay = "Not taken yet";
                if (!string.IsNullOrEmpty(viewModel.UserProfile.PersonalityTestResult))
                {
                    // Try parsing the string to the JobRole enum
                    if (Enum.TryParse<JobRole>(viewModel.UserProfile.PersonalityTestResult, out var jobRole))
                    {
                        var converter = new converters.JobRoleToDisplayNameConverter();

                        // Convert the enum value to the display string
                        testResultDisplay = converter.Convert(jobRole, typeof(string), null, string.Empty).ToString();
                    }
                    else
                    {
                        // Fallback if the string couldn't be parsed 
                        testResultDisplay = viewModel.UserProfile.PersonalityTestResult;
                    }
                }
                lblPersonalityTestResult.Text = $"Personality Test Result: {testResultDisplay}";

                LevelTitleText.Text = "Level 2 - Apprentice";
                XpProgressBar.Maximum = 250;
                XpProgressBar.Value = 150;
                XpCountText.Text = "150 / 250 XP";

                chkAccountStatus.IsOn = viewModel.UserProfile.ActiveAccount;

                if (!string.IsNullOrEmpty(viewModel.UserProfile.ProfilePicture))
                {
                    pbAvatar.ProfilePicture =
                        new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(
                            new Uri(viewModel.UserProfile.ProfilePicture));
                }
                else
                {
                    pbAvatar.ProfilePicture = null;
                }

                if (!string.IsNullOrEmpty(viewModel.UserProfile.PersonalityTestResult))
                {
                    btnPersonalityTest.Content = "Retake Personality Test";
                }
                else
                {
                    btnPersonalityTest.Content = "Take Personality Test";
                }

                completenessBar.Update(viewModel.CompletenessPercentage, viewModel.NextEmptyFieldPrompt);

                viewModel.RecalculateLevelCommand();
                renderLevelDisplay();
            }
            else
            {
                
                return;
            }

            _isBinding = false;
        }

        private void renderLevelDisplay()
        {
            if (viewModel.UserProfile == null) return;          
            if (viewModel.UserProfile.UserLevel == null) return;
            LevelTitleText.Text = $"Level {viewModel.UserProfile.UserLevel.LevelNumber} — {viewModel.UserProfile.UserLevel.Title}";

            XpProgressBar.Value = viewModel.UserProfile.UserLevel.getProgressPercent(viewModel.TotalXP);

            int xpToNext = viewModel.UserProfile.UserLevel.getXPToNextLevel(viewModel.TotalXP);
            XpCountText.Text = xpToNext > 0
                ? $"{viewModel.TotalXP} XP — {xpToNext} XP needed for next level"
                : $"{viewModel.TotalXP} XP — Max level reached!";
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
                    viewModel.UploadAvatarCommand(stream, file.Name);
                    BindData();
                }
            }
        }

        private void OnAvatarRemoveClick(object sender, RoutedEventArgs e)
        {
            viewModel.RemoveAvatarCommand();
            BindData();
        }

        private void OnStatusToggle(object sender, RoutedEventArgs e)
        {
            if (_isBinding) return;

            if (viewModel?.UserProfile != null)
            {
                viewModel.ToggleAccountStatusCommand();
                BindData();
            }
        }

        private void OnEditProfileClick(object sender, RoutedEventArgs e)
        {
            if (viewModel.UserProfile != null)
            {
                Frame.Navigate(typeof(ProfileFormView), viewModel.UserProfile);
            }
            else
            {
                Frame.Navigate(typeof(ProfileFormView));
            }
        }

        private void OnPreviewCVClick(object sender, RoutedEventArgs e)
        {
            if (viewModel.UserProfile != null)
            {
                Frame.Navigate(typeof(ExportCVView), viewModel.UserProfile.UserId);
            }
        }

        private void OnGoToOldTestsClick(object sender, RoutedEventArgs e)
        {
            if (viewModel.UserProfile == null)
                return;

            //viewModel.UserProfile.UserId = currentUserId;
            this.Frame.Navigate(typeof(TestDashboardView), viewModel.UserProfile);
        }

        private void OnSeePublicProfileClick(object sender, RoutedEventArgs e)
        {
            if (viewModel.UserProfile == null)
                return;
            this.Frame.Navigate(typeof(PublicProfileView), viewModel.UserProfile);
        }
        
        private void OnCompatibilityAnalyzerClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CompatibilityOverviewView), currentUserId);
        }

        private void OnPersonalityTestClick(object sender, RoutedEventArgs e)
        {
            viewModel.TakePersonalityTestCommand();
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
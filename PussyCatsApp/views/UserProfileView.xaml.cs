using Microsoft.Data.SqlClient;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.models;
using PussyCatsApp.repositories;
using PussyCatsApp.services;
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
        private SqlConnection connection;
        private CompatibilityOverviewViewModel compatibilityViewModel;

        public UserProfileView()
        {
            this.InitializeComponent();

            var userProfileRepository = new UserProfileRepository();
            var skillTestRepository = new SkillTestRepository();

            compatibilityViewModel = new CompatibilityOverviewViewModel(currentUserId);

            viewModel = new UserProfileViewModel(
                new UserProfileService(userProfileRepository, skillTestRepository),
                new ImageStorageService(),
                null,
                new CvUploadService(),
                new CompletenessService()
            );

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

            if (viewModel._userProfile != null)
            {
                lblFirstName.Text = $"First Name: {viewModel._userProfile.FirstName}";
                lblLastName.Text = $"Last Name: {viewModel._userProfile.LastName}";
                lblEmail.Text = $"Email: {viewModel._userProfile.Email}";
                lblPhone.Text = $"Phone: {viewModel._userProfile.PhoneNumber}";
                lblGithubAccount.Text = $"GitHub: {viewModel._userProfile.GitHub}";
                lblLinkedinAccount.Text = $"LinkedIn: {viewModel._userProfile.LinkedIn}";

                string displayGender = viewModel._userProfile.Gender;
                if (string.IsNullOrEmpty(displayGender) || (displayGender != "Male" && displayGender != "Female"))
                    displayGender = "Not specified";

                Debug.WriteLine($"[BindData] ErrorMessage: '{viewModel.ErrorMessage}'");
                Debug.WriteLine($"[BindData] userProfile is null: {viewModel._userProfile == null}");

                lblGender.Text = $"Gender: {displayGender}";
                lblUniversity.Text = $"University: {viewModel._userProfile.University}";
                lblCountry.Text = $"Country: {viewModel._userProfile.Country}";
                lblAddress.Text = $"Address: {viewModel._userProfile.Address}";
                lblGraduationYear.Text = $"Graduation Year: {viewModel._userProfile.ExpectedGraduationYear}";
                lblFreshness.Text = viewModel.FreshnessText;

                LevelTitleText.Text = "Level 2 - Apprentice";
                XpProgressBar.Maximum = 250;
                XpProgressBar.Value = 150;
                XpCountText.Text = "150 / 250 XP";

                chkAccountStatus.IsOn = viewModel._userProfile.ActiveAccount;

                if (!string.IsNullOrEmpty(viewModel._userProfile.ProfilePicture))
                {
                    pbAvatar.ProfilePicture =
                        new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(
                            new Uri(viewModel._userProfile.ProfilePicture));
                }
                else
                {
                    pbAvatar.ProfilePicture = null;
                }

                completenessBar.Update(viewModel.CompletenessPercentage, viewModel.NextEmptyFieldPrompt);

                viewModel.recalculateLevelCommand();
                renderLevelDisplay();
            }
            else
            {
                // No user in DB — go straight to the profile form
                //Frame.Navigate(typeof(ProfileFormPage));
                return;
            }

            _isBinding = false;
        }

        private void renderLevelDisplay()
        {
            if (viewModel._userProfile == null) return;          
            if (viewModel._userProfile.UserLevel == null) return;
            LevelTitleText.Text = $"Level {viewModel._userProfile.UserLevel.LevelNumber} — {viewModel._userProfile.UserLevel.Title}";

            XpProgressBar.Value = viewModel._userProfile.UserLevel.getProgressPercent(viewModel.TotalXP);

            int xpToNext = viewModel._userProfile.UserLevel.getXPToNextLevel(viewModel.TotalXP);
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

            if (viewModel?._userProfile != null)
            {
                viewModel.ToggleAccountStatusCommand();
                BindData();
            }
        }

        private void OnEditProfileClick(object sender, RoutedEventArgs e)
        {
            if (viewModel._userProfile != null)
            {
                Frame.Navigate(typeof(ProfileFormPage), viewModel._userProfile);
            }
            else
            {
                Frame.Navigate(typeof(ProfileFormPage));
            }
        }

        private void OnPreviewCVClick(object sender, RoutedEventArgs e)
        {
            if (viewModel._userProfile != null)
            {
                Frame.Navigate(typeof(ExportCVTestPage), viewModel._userProfile.UserId);
            }
        }

        private void OnGoToOldTestsClick(object sender, RoutedEventArgs e)
        {
            if (viewModel._userProfile == null)
                return;

            this.Frame.Navigate(typeof(TestDashboardView), viewModel._userProfile);
        }

        private void OnSeePublicProfileClick(object sender, RoutedEventArgs e)
        {
            if (viewModel._userProfile == null)
                return;
            this.Frame.Navigate(typeof(PublicProfileView), viewModel._userProfile);
        }
        
        private void OnCompatibilityAnalyzerClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CompatibilityOverviewView), compatibilityViewModel);
        }

        private void OnPersonalityTestClick(object sender, RoutedEventArgs e)
        {
            viewModel.TakePersonalityTestCommand();
        }

        private void OnViewDocumentsClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DocumentsPage));
        }
    }
}
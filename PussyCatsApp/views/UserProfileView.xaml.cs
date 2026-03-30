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
        private readonly UserProfileViewModel viewModel;
        private bool isBinding = false;
        private SqlConnection connection;

        public UserProfileView()
        {
            this.InitializeComponent();

            var userProfileRepository = new UserProfileRepository();
            var skillTestRepository = new SkillTestRepository();
            IUserProileRepository user = new UserProfileRepository();
            WebView2 view = new WebView2();

            viewModel = new UserProfileViewModel(
                new UserProfileService(userProfileRepository, skillTestRepository),
                new ImageStorageService(),
                new PdfExportService(view, user),
                new CvUploadService(),
                new CompletenessService()
            );

            viewModel.OnLevelUpdated += renderLevelDisplay;


            // Wire up Edit button to navigate to ProfileFormPage
            btnEdit.Click += OnEditProfileClick;

            btnOldTests.Click += OnGoToOldTestsClick;

            btnPublicProfile.Click += OnSeePublicProfileClick;

            BindData();

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
            await viewModel.LoadUserAsync(dummyUserId);

            if (!string.IsNullOrEmpty(viewModel.ErrorMessage))
            {
                lblError.Text = viewModel.ErrorMessage;
                isBinding = false;
                return;
            }

            if (viewModel.userProfile != null)
            {
                lblFirstName.Text = $"First Name: {viewModel.userProfile.FirstName}";
                lblLastName.Text = $"Last Name: {viewModel.userProfile.LastName}";
                lblEmail.Text = $"Email: {viewModel.userProfile.Email}";
                lblPhone.Text = $"Phone: {viewModel.userProfile.PhoneNumber}";
                lblGithubAccount.Text = $"GitHub: {viewModel.userProfile.GitHub}";
                lblLinkedinAccount.Text = $"LinkedIn: {viewModel.userProfile.LinkedIn}";

                string displayGender = viewModel.userProfile.Gender;
                if (string.IsNullOrEmpty(displayGender) || (displayGender != "Male" && displayGender != "Female"))
                    displayGender = "Not specified";

                Debug.WriteLine($"[BindData] ErrorMessage: '{viewModel.ErrorMessage}'");
                Debug.WriteLine($"[BindData] userProfile is null: {viewModel.userProfile == null}");

                lblGender.Text = $"Gender: {displayGender}";
                lblUniversity.Text = $"University: {viewModel.userProfile.University}";
                lblCountry.Text = $"Country: {viewModel.userProfile.Country}";
                lblGraduationYear.Text = $"Graduation Year: {viewModel.userProfile.ExpectedGraduationYear}";
                lblFreshness.Text = viewModel.FreshnessText;

                LevelTitleText.Text = "Level 2 - Apprentice";
                XpProgressBar.Maximum = 250;
                XpProgressBar.Value = 150;
                XpCountText.Text = "150 / 250 XP";

                chkAccountStatus.IsOn = viewModel.userProfile.ActiveAccount;

                if (!string.IsNullOrEmpty(viewModel.userProfile.ProfilePicture))
                {
                    pbAvatar.ProfilePicture =
                        new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(
                            new Uri(viewModel.userProfile.ProfilePicture));
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
                Frame.Navigate(typeof(ProfileFormPage));
                return;
            }

            isBinding = false;
        }

        private void renderLevelDisplay()
        {
            if (viewModel.userProfile == null) return;          
            if (viewModel.userProfile.UserLevel == null) return;
            LevelTitleText.Text = $"Level {viewModel.userProfile.UserLevel.LevelNumber} — {viewModel.userProfile.UserLevel.Title}";

            XpProgressBar.Value = viewModel.userProfile.UserLevel.getProgressPercent(viewModel.TotalXP);

            int xpToNext = viewModel.userProfile.UserLevel.getXPToNextLevel(viewModel.TotalXP);
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
            if (isBinding)
                return;

            if (viewModel.userProfile != null)
            {
                viewModel.ToggleAccountStatusCommand();
                BindData();
            }
        }

        private void OnEditProfileClick(object sender, RoutedEventArgs e)
        {
            if (viewModel.userProfile != null)
            {
                Frame.Navigate(typeof(ProfileFormPage), viewModel.userProfile);
            }
            else
            {
                Frame.Navigate(typeof(ProfileFormPage));
            }
        }

        private void OnGoToOldTestsClick(object sender, RoutedEventArgs e)
        {
            if (viewModel.userProfile == null)
                return;

            this.Frame.Navigate(typeof(TestDashboardView), viewModel.userProfile);
        }

        private void OnSeePublicProfileClick(object sender, RoutedEventArgs e)
        {
            if (viewModel.userProfile == null)
                return;
            this.Frame.Navigate(typeof(PublicProfileView), viewModel.userProfile);
        }
        
        private void OnCompatibilityAnalyzerClick(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=DESKTOP-SCP6QST;Initial Catalog=UserManagementDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30";

            UserSkillRepository userSkillRepo = new UserSkillRepository(connectionString);
            SkillGroupRepository skillGroupRepo = new SkillGroupRepository();
            CompatibilityService service = new CompatibilityService(userSkillRepo, skillGroupRepo);
            CompatibilityOverviewViewModel vm = new CompatibilityOverviewViewModel(service, 2); // currentUserId

            Frame.Navigate(typeof(CompatibilityOverviewView), vm);
        }

        private void OnPersonalityTestClick(object sender, RoutedEventArgs e)
        {
            viewModel.TakePersonalityTestCommand();
        }
    }
}
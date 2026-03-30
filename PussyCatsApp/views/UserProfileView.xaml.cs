using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PussyCatsApp.repositories;
using PussyCatsApp.services;
using PussyCatsApp.viewModels;
using System;
using System.IO;

namespace PussyCatsApp.views
{
    public sealed partial class UserProfileView : Page
    {
        private readonly UserProfileViewModel viewModel;
        private bool isBinding = false;

        public UserProfileView()
        {
            this.InitializeComponent();

            var userProfileRepository = new UserProfileRepository();
            var skillTestRepository = new SkillTestRepository();

            viewModel = new UserProfileViewModel(
                new UserProfileService(userProfileRepository, skillTestRepository),
                new ImageStorageService(),
                new PdfExportService(),
                new CvUploadService(),
                new CompletenessService()
            );

            viewModel.OnLevelUpdated += RenderLevelDisplay;

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
                lblFirstName.Text = $"First Name: {viewModel.userProfile.firstName}";
                lblLastName.Text = $"Last Name: {viewModel.userProfile.lastName}";
                lblEmail.Text = $"Email: {viewModel.userProfile.emailAddress}";
                lblPhone.Text = $"Phone: {viewModel.userProfile.phoneNumber}";
                lblGithubAccount.Text = $"GitHub: {viewModel.userProfile.githubAccount}";
                lblLinkedinAccount.Text = $"LinkedIn: {viewModel.userProfile.linkedinAccount}";

                string displayGender = "Not specified";
                if (viewModel.userProfile.gender == 'M')
                {
                    displayGender = "Male";
                }
                else if (viewModel.userProfile.gender == 'F')
                {
                    displayGender = "Female";
                }

                lblGender.Text = $"Gender: {displayGender}";
                lblCountry.Text = $"Country: {viewModel.userProfile.country}";
                lblCity.Text = $"City: {viewModel.userProfile.city}";
                lblGraduationYear.Text = $"Graduation Year: {viewModel.userProfile.graduationYear}";

                chkAccountStatus.IsOn = viewModel.userProfile.accountStatus.ToString() == "ACTIVE";

                if (!string.IsNullOrEmpty(viewModel.userProfile.profilePicture))
                {
                    pbAvatar.ProfilePicture =
                        new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(
                            new Uri(viewModel.userProfile.profilePicture));
                }
                else
                {
                    pbAvatar.ProfilePicture = null;
                }

                RenderLevelDisplay();
            }
            else
            {
                lblError.Text = "User profile not found.";
            }

            isBinding = false;
        }

        private void RenderLevelDisplay()
        {
            if (viewModel.CurrentLevel == null)
                return;

            LevelTitleText.Text =
                $"Level {viewModel.CurrentLevel.LevelNumber} — {viewModel.CurrentLevel.Title}";

            XpProgressBar.Value =
                viewModel.CurrentLevel.getProgressPercent(viewModel.TotalXP);

            int xpToNext = viewModel.CurrentLevel.getXPToNextLevel();
            XpCountText.Text = xpToNext > 0
                ? $"{viewModel.TotalXP} XP — {xpToNext} XP to {viewModel.CurrentLevel.Title}"
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
    }
}
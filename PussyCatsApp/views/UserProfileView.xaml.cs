using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PussyCatsApp.services;
using PussyCatsApp.viewModels;
using PussyCatsApp.repositories;
using System;
using System.IO;

namespace PussyCatsApp.views
{
    public sealed partial class UserProfileView : Page
    {
        private UserProfileViewModel viewModel;
        private bool isBinding = false;

        public UserProfileView()
        {
            this.InitializeComponent();

            var repository = new UserProfileRepository();
            viewModel = new UserProfileViewModel(
                new UserProfileService(repository),
                new ImageStorageService(),
                new PdfExportService(),
                new CvUploadService(),
                new CompletenessService()
            );

            bindData();
        }

        private async void bindData()
        {
            isBinding = true;

            int dummyUserId = 1;
            await viewModel.LoadUserAsync(dummyUserId);

            if (!string.IsNullOrEmpty(viewModel.ErrorMessage))
            {
                lblError.Text = viewModel.ErrorMessage;
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

                chkAccountStatus.IsOn = (viewModel.userProfile.accountStatus.ToString() == "ACTIVE");

                if (!string.IsNullOrEmpty(viewModel.userProfile.profilePicture))
                {
                    pbAvatar.ProfilePicture = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri(viewModel.userProfile.profilePicture));
                }
                else
                {
                    pbAvatar.ProfilePicture = null;
                }
            }
            else
            {
                lblError.Text = "User profile not found.";
            }
            isBinding = false;
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
                    bindData();
                }
            }
        }

        private void OnAvatarRemoveClick(object sender, RoutedEventArgs e)
        {
            viewModel.RemoveAvatarCommand();
            bindData();
        }

        private void OnStatusToggle(object sender, RoutedEventArgs e)
        {
            if (isBinding) return;

            if (viewModel.userProfile != null)
            {
                viewModel.ToggleAccountStatusCommand();
                bindData();
            }
        }
    }
}
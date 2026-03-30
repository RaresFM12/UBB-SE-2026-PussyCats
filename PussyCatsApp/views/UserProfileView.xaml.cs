using Microsoft.Data.SqlClient;
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
        private SqlConnection connection;

        public UserProfileView()
        {
            this.InitializeComponent();

            var userProfileRepository = new UserProfileRepository();
            var skillTestRepository = new SkillTestRepository(connection);
            IUserProileRepository user = new UserProfileRepository();
            WebView2 view = new WebView2();

            viewModel = new UserProfileViewModel(
                new UserProfileService(userProfileRepository, skillTestRepository),
                new ImageStorageService(),
                new PdfExportService(view, user),
                new CvUploadService(),
                new CompletenessService()
            );

            

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

                string displayGender = "Not specified";
                if (viewModel.userProfile.Gender == "M")
                {
                    displayGender = "Male";
                }
                else if (viewModel.userProfile.Gender == "F")
                {
                    displayGender = "Female";
                }

                lblGender.Text = $"Gender: {displayGender}";
                lblCountry.Text = $"Country: {viewModel.userProfile.Country}";
                lblCity.Text = $"City: {viewModel.userProfile.City}";
                lblGraduationYear.Text = $"Graduation Year: {viewModel.userProfile.ExpectedGraduationYear}";

                chkAccountStatus.IsOn = viewModel.userProfile.ActiveAccount.ToString() == "ACTIVE";

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
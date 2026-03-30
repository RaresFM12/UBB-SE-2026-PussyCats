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
        public UserProfileViewModel viewModel { get; private set; }
        private bool _isBinding = false;
        private SqlConnection connection;

        public UserProfileView()
        {
            this.InitializeComponent();

            var userProfileRepository = new UserProfileRepository();
            var skillTestRepository = new SkillTestRepository(connection);

            viewModel = new UserProfileViewModel(
                new UserProfileService(userProfileRepository, skillTestRepository),
                new ImageStorageService(),
                null, // PdfExportService is now handled by the ExportCVTestPage
                new CvUploadService(),
                new CompletenessService()
            );

            this.DataContext = viewModel;

            // Wire up Edit button
            btnEdit.Click += OnEditProfileClick;

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

                lblGender.Text = $"Gender: {displayGender}";
                lblUniversity.Text = $"University: {viewModel._userProfile.University}";
                lblCountry.Text = $"Country: {viewModel._userProfile.Country}";
                lblAddress.Text = $"Address: {viewModel._userProfile.Address}";
                lblGraduationYear.Text = $"Graduation Year: {viewModel._userProfile.ExpectedGraduationYear}";

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
            }

            _isBinding = false;
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

        private void OnCompatibilityAnalyzerClick(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=DESKTOP-SCP6QST;Initial Catalog=UserManagementDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30";

            UserSkillRepository userSkillRepo = new UserSkillRepository(connectionString);
            SkillGroupRepository skillGroupRepo = new SkillGroupRepository();
            CompatibilityService service = new CompatibilityService(userSkillRepo, skillGroupRepo);
            CompatibilityOverviewViewModel vm = new CompatibilityOverviewViewModel(service, 2);

            Frame.Navigate(typeof(CompatibilityOverviewView), vm);
        }
    }
}
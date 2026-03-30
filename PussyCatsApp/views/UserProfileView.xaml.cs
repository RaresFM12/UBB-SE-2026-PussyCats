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

            // Wire up Edit button to navigate to ProfileFormPage
            btnEdit.Click += OnEditProfileClick;

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

                lblGender.Text = $"Gender: {displayGender}";
                lblUniversity.Text = $"University: {viewModel.userProfile.University}";
                lblCountry.Text = $"Country: {viewModel.userProfile.Country}";
                lblAddress.Text = $"Address: {viewModel.userProfile.Address}";
                lblGraduationYear.Text = $"Graduation Year: {viewModel.userProfile.ExpectedGraduationYear}";

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
            }
            else
            {
                // No user in DB — go straight to the profile form
                //Frame.Navigate(typeof(ProfileFormPage));
                return;
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
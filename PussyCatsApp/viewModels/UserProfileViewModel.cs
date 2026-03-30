using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PussyCatsApp.models;
using PussyCatsApp.services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PussyCatsApp.viewModels
{
    public partial class UserProfileViewModel : ObservableObject
    {
        private UserProfileService profileSerivice;
        private ImageStorageService imageStorageService;
        private CvUploadService cvUploadService;
        private CompletenessService completenessService;

        // Nested Export ViewModel
        public ExportCVViewModel ExportVM { get; }

        private UserProfile? __userProfile;
        public UserProfile? _userProfile
        {
            get => __userProfile;
            set => SetProperty(ref __userProfile, value);
        }

        public bool _isLoading { get; set; }
        public int CompletenessPercentage { get; set; }
        public string NextEmptyFieldPrompt { get; set; } = "";
        public List<string> MissingFieldWarnings { get; set; } = new List<string>();
        public string ErrorMessage = string.Empty;
        public string FreshnessText = string.Empty;

        public UserProfileViewModel(UserProfileService userProfileService, ImageStorageService imageStorageService, PdfExportService pdfExportService, CvUploadService cvUploadService, CompletenessService completenessService)
        {
            this.profileSerivice = userProfileService;
            this.imageStorageService = imageStorageService;
            this.cvUploadService = cvUploadService;
            this.completenessService = completenessService;

            // Initialize nested ViewModel
            this.ExportVM = new ExportCVViewModel(pdfExportService);
        }

        public void recalculateLevelCommand()
        {
        }

        public async Task LoadUserAsync(int userId)
        {
            _isLoading = true;
            try
            {
                _userProfile = await Task.Run(() => profileSerivice.GetProfile(userId));

                if (_userProfile != null)
                {
                    ExportVM.UserId = _userProfile.UserId;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading user profile: {ex.Message}";
            }
            finally
            {
                _isLoading = false;
            }
        }

        public void ToggleAccountStatusCommand()
        {
            if (_userProfile == null) return;
            string currentStatusStr = _userProfile.ActiveAccount ? "ACTIVE" : "INACTIVE";
            profileSerivice.ToggleAccountStatus(_userProfile.UserId, currentStatusStr);
            _userProfile.ActiveAccount = !_userProfile.ActiveAccount;
        }

        public void UploadAvatarCommand(Stream fileStream, string fileName)
        {
            if (_userProfile == null) return;
            try
            {
                string newPath = imageStorageService.SaveImage(fileStream, fileName);
                profileSerivice.UpdateAvatarPath(_userProfile.UserId, newPath);
                _userProfile.ProfilePicture = newPath;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error uploading avatar: {ex.Message}";
                return;
            }
        }

        public void RemoveAvatarCommand()
        {
            if (_userProfile?.ProfilePicture != null)
            {
                imageStorageService.DeleteImage(_userProfile.ProfilePicture);
                profileSerivice.RemoveAvatarPath(_userProfile.UserId);
                _userProfile.ProfilePicture = null;
            }
        }

        public string GetPersonalityButtonText()
        {
            if (_userProfile != null && string.IsNullOrEmpty(_userProfile.PersonalityTestResult))
                return "TAKE PERSONALITY TEST";
            return "RETAKE PERSONALITY TEST";
        }

        public void EditProfileCommand() { }
        public void TakePersonalityTestCommand() { }
        public void ViewDocumentsCommand() { }
        public void MatchHistoryCommand() { }
        public void GoToSkillTestCommand() { }
    }
}
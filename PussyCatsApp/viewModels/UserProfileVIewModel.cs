using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.services;
using PussyCatsApp.views;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using PussyCatsApp.Repositories;

namespace PussyCatsApp.viewModels
{
    public partial class UserProfileViewModel : ObservableObject
    {
        private IUserProfileService profileService;
        private IImageStorageService imageStorageService;
        private CvUploadService cvUploadService;
        private ICompletenessService completenessService;

        // Nested Export ViewModel
        public ExportCVViewModel ExportVM { get; }

        private UserProfile? userProfile;
        public UserProfile? UserProfile
        {
            get => userProfile;
            set => SetProperty(ref userProfile, value);
        }

        public bool IsLoading { get; set; }
        public int CompletenessPercentage { get; set; }
        public string NextEmptyFieldPrompt { get; set; } = "";
        public List<string> MissingFieldWarnings { get; set; } = new List<string>();
        public string ErrorMessage { get; set; } = "";
        public string FreshnessText { get; set; } = "";
        public int TotalXP { get; private set; } = 0;

        public UserProfileViewModel(IUserProfileService userProfileService, IImageStorageService imageStorageService, ICompletenessService completenessService)
        {
            this.profileService = userProfileService;
            this.imageStorageService = imageStorageService;
            this.cvUploadService = new CvUploadService();
            this.completenessService = completenessService;
        }

        public event Action OnLevelUpdated;
        public void RecalculateLevelCommand()
        {
            if (UserProfile == null) return;

            try
            {
                TotalXP = profileService.RecalculateLevel(UserProfile); 
                OnLevelUpdated?.Invoke();

            }
            catch(Exception ex)
            {
                ErrorMessage = $"Error recalculating user level: {ex.Message}";
            }
            
        }
        

        public async Task LoadUserAsync(int userId)
        {
            ErrorMessage = "";
            IsLoading = true;
            try
            {
                UserProfile = await Task.Run(() => profileService.GetProfile(userId));

                if (UserProfile != null)
                {
                    FreshnessText = utilities.TimeFormatter.CalculateFreshnessLabel(UserProfile.LastUpdated);

                    CompletenessPercentage = completenessService.CalculateCompleteness(UserProfile);
                    NextEmptyFieldPrompt = completenessService.GetNextEmptyFieldPrompt(UserProfile);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading user profile: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void ToggleAccountStatusCommand()
        {
            if (UserProfile == null) return;
            string currentStatusStr = UserProfile.ActiveAccount ? "ACTIVE" : "INACTIVE";
            profileService.ToggleAccountStatus(UserProfile.UserId, currentStatusStr);
            UserProfile.ActiveAccount = !UserProfile.ActiveAccount;
        }

        public void UploadAvatarCommand(Stream fileStream, string fileName)
        {
            if (UserProfile == null) return;
            try
            {
                string newPath = imageStorageService.SaveImage(fileStream, fileName);
                profileService.UpdateAvatarPath(UserProfile.UserId, newPath);
                UserProfile.ProfilePicture = newPath;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error uploading avatar: {ex.Message}";
                return;
            }
        }

        public void RemoveAvatarCommand()
        {
            if (!string.IsNullOrEmpty(UserProfile?.ProfilePicture))
            {
                imageStorageService.DeleteImage(UserProfile.ProfilePicture);
                profileService.RemoveAvatarPath(UserProfile.UserId);
                UserProfile.ProfilePicture = null;
            }
        }

        public string GetPersonalityButtonText()
        {
            if (UserProfile != null && string.IsNullOrEmpty(UserProfile.PersonalityTestResult))
                return "TAKE PERSONALITY TEST";
            return "RETAKE PERSONALITY TEST";
        }
        public void TakePersonalityTestCommand()
        {
            if (App.MainAppWindow is MainWindow mainWindow)
            {
                mainWindow.NavigationFrame.Navigate(typeof(PersonalityTestView), 1);
            }
        }
    }
}
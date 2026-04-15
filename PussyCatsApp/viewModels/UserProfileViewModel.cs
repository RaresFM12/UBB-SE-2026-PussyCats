using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using PussyCatsApp.Repositories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PussyCatsApp.Models;
using PussyCatsApp.Services;
using PussyCatsApp.Views;

namespace PussyCatsApp.ViewModels
{
    /// <summary>
    /// ViewModel for managing the user profile overview, including avatar upload, account status,
    /// profile completeness calculation, XP/level tracking, and navigation to related features.
    /// </summary>
    public partial class UserProfileViewModel : ObservableObject
    {
        private UserProfileService profileSerivice;
        private ImageStorageService imageStorageService;
        private CvUploadService cvUploadService;
        private CompletenessService completenessService;

        // Nested Export ViewModel
        public ExportCVViewModel ExportVM { get; }

        private UserProfile? userProfilePrivate;
        public UserProfile? UserProfilePublic
        {
            get => userProfilePrivate;
            set => SetProperty(ref userProfilePrivate, value);
        }

        public bool IsLoading { get; set; }
        public int CompletenessPercentage { get; set; }
        public string NextEmptyFieldPrompt { get; set; } = string.Empty;
        public List<string> MissingFieldWarnings { get; set; } = new List<string>();
        public string ErrorMessage { get; set; } = string.Empty;
        public string FreshnessText { get; set; } = string.Empty;
        public int TotalXP { get; private set; } = 0;

        public UserProfileViewModel()
        {
            this.profileSerivice = new UserProfileService();
            this.imageStorageService = new ImageStorageService();
            this.cvUploadService = new CvUploadService();
            this.completenessService = new CompletenessService();
        }

        public static UserProfileViewModel Create()
        {
            return new UserProfileViewModel();
        }

        public event Action OnLevelUpdated;
        public void RecalculateLevelCommand()
        {
            if (UserProfilePublic == null)
            {
                return;
            }

            try
            {
                TotalXP = profileSerivice.RecalculateLevel(UserProfilePublic);
                OnLevelUpdated?.Invoke();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error recalculating user level: {ex.Message}";
            }
        }

        public async Task LoadUserAsync(int userId)
        {
            ErrorMessage = string.Empty;
            IsLoading = true;
            try
            {
                UserProfilePublic = await Task.Run(() => profileSerivice.GetProfile(userId));

                if (UserProfilePublic != null)
                {
                    FreshnessText = Utilities.TimeFormatter.CalculateFreshnessLabel(UserProfilePublic.LastUpdated);

                    CompletenessPercentage = completenessService.CalculateCompleteness(UserProfilePublic);
                    NextEmptyFieldPrompt = completenessService.GetNextEmptyFieldPrompt(UserProfilePublic);
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
            if (UserProfilePublic == null)
            {
                return;
            }
            string currentStatusStr = UserProfilePublic.ActiveAccount ? "ACTIVE" : "INACTIVE";
            profileSerivice.ToggleAccountStatus(UserProfilePublic.UserId, currentStatusStr);
            UserProfilePublic.ActiveAccount = !UserProfilePublic.ActiveAccount;
        }

        public void UploadAvatarCommand(Stream fileStream, string fileName)
        {
            if (UserProfilePublic == null)
            {
                return;
            }
            try
            {
                string newPath = imageStorageService.SaveImage(fileStream, fileName);
                profileSerivice.UpdateAvatarPath(UserProfilePublic.UserId, newPath);
                UserProfilePublic.ProfilePicture = newPath;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error uploading avatar: {ex.Message}";
                return;
            }
        }

        public void RemoveAvatarCommand()
        {
            if (!string.IsNullOrEmpty(UserProfilePublic?.ProfilePicture))
            {
                imageStorageService.DeleteImage(UserProfilePublic.ProfilePicture);
                profileSerivice.RemoveAvatarPath(UserProfilePublic.UserId);
                UserProfilePublic.ProfilePicture = null;
            }
        }

        public string GetPersonalityButtonText()
        {
            if (UserProfilePublic != null && string.IsNullOrEmpty(UserProfilePublic.PersonalityTestResult))
            {
                return "TAKE PERSONALITY TEST";
            }
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
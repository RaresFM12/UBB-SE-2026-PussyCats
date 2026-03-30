using PussyCatsApp.models;
using PussyCatsApp.services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.viewModels
{
    public class UserProfileViewModel
    {
        private UserProfileService profileSerivice;
        private ImageStorageService imageStorageService;
        private PdfExportService pdfExportService;
        private CvUploadService cvUploadService;
        private CompletenessService completenessService;

        public UserProfile? userProfile { get; set; }
        public bool isLoading { get; set; }
        public int CompletenessPercentage { get; set; }
        public string NextEmptyFieldPrompt { get; set; } = "";
        public List<string> MissingFieldWarnings { get; set; } = new List<string>();
        public string ErrorMessage { get; set; } = "";
        public string FreshnessText { get; set; } = "";

        

        public UserProfileViewModel(UserProfileService userProfileService, ImageStorageService imageStorageService,PdfExportService pdfExportService,CvUploadService cvUploadService, CompletenessService completenessService)
        {
            
            this.profileSerivice = userProfileService;
            this.imageStorageService = imageStorageService;
            this.pdfExportService = pdfExportService;
            this.cvUploadService = cvUploadService;
            this.completenessService = completenessService;
        }

        public void recalculateLevelCommand()
        {
            
        }

        public async Task LoadUserAsync(int userId)
        {
            isLoading = true;
            try
            {
                userProfile = await Task.Run(() => profileSerivice.GetProfile(userId)); 
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading user profile: {ex.Message}";
            }
            finally
            {
                isLoading = false;
            }
        }

        public void ToggleAccountStatusCommand()
        {
            string currentStatusStr = userProfile.ActiveAccount ? "ACTIVE" : "INACTIVE";

            profileSerivice.ToggleAccountStatus(userProfile.UserId, currentStatusStr);

            userProfile.ActiveAccount = !userProfile.ActiveAccount;

        }

        public void UploadAvatarCommand(Stream fileStream, string fileName)
        {
            try
            {
                string newPath = imageStorageService.SaveImage(fileStream, fileName);
                profileSerivice.UpdateAvatarPath(userProfile.UserId, newPath);
                userProfile.ProfilePicture = newPath;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error uploading avatar: {ex.Message}";
                return;
            }
            

        }

        public void RemoveAvatarCommand()
        {
            if(userProfile.ProfilePicture != null)
            {
                imageStorageService.DeleteImage(userProfile.ProfilePicture);
                profileSerivice.RemoveAvatarPath(userProfile.UserId);
                userProfile.ProfilePicture = null;
            }
        }

        public void ExportToPdfCommand()
        {
            isLoading = true;
            pdfExportService.ExportProfileToPdfAsync(userProfile.UserId);
            isLoading = false;
        }

        public string GetPersonalityButtonText()
        {
            if(string.IsNullOrEmpty(userProfile.PersonalityTestResult))
                return "TAKE PERSONALITY TEST";
            return "RETAKE PERSONALITY TEST";

        }

        public void EditProfileCommand()
        {
            //TODO Navigations
        }
        public void TakePersonalityTestCommand()
        {
            
        }

        public void ViewDocumentsCommand()
        { 
        }

        public void MatchHistoryCommand()
        {
        }

        public void GoToSkillTestCommand()
        {
        }





        }
}

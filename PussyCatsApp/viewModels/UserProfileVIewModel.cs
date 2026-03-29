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
    internal class UserProfileViewModel
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
            profileSerivice.ToggleAccountStatus(userProfile.userID, userProfile.accountStatus.ToString());

        }

        public void UploadAvatarCommand(Stream fileStream, string fileName)
        {
            try
            {
                string newPath = imageStorageService.SaveImage(fileStream, fileName);
                profileSerivice.UpdateAvatarPath(userProfile.userID, newPath);
                userProfile.profilePicture = newPath;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error uploading avatar: {ex.Message}";
                return;
            }
            

        }

        public void RemoveAvatarCommand()
        {
            if(userProfile.profilePicture != null)
            {
                imageStorageService.DeleteImage(userProfile.profilePicture);
                profileSerivice.RemoveAvatarPath(userProfile.userID);
                userProfile.profilePicture = null;
            }
        }

        public void ExportToPdfCommand()
        {
            isLoading = true;
            pdfExportService.export();
            isLoading = false;
        }

        public string GetPersonalityButtonText()
        {
            if(string.IsNullOrEmpty(userProfile.personalityResult))
                return "TAKE PERSONALITY TEST";
            return "RETAKE PERSONALITY TEST";

        }

        public void EditProfileCommand()
        {
            //TODO
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

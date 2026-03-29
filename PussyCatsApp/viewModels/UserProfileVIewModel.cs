using PussyCatsApp.models;
using PussyCatsApp.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.viewModels
{
    internal class UserProfileVIewModel
    {
        private UserProfileService profileSerivice;
        private ImageStorageService imageStorageService;
        private PdfExportService pdfExportService;
        private CvUploadService cvUploadService;
        private CompletenessService completenessService;

        public UserProfile userProfile { get; set; }
        public bool isLoading { get; set; }
        public int CompletenessPercentage { get; set; }
        public string NextEmptyFieldPrompt { get; set; }
        public List<string> MissingFieldWarnings { get; set; } = new List<string>();
        public string ErrorMessage { get; set; }
        public string FreshnessText { get; set; }

        public UserProfileVIewModel(UserProfileService userProfileService, ImageStorageService imageStorageService,PdfExportService pdfExportService,CvUploadService cvUploadService, CompletenessService completenessService)
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







    }
}

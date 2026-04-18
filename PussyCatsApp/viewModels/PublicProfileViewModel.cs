using PussyCatsApp.Models;
using PussyCatsApp.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.viewModels
{
    public class PublicProfileViewModel
    {
        private readonly UserProfileService userProfileService;

        public UserProfile? Profile { get; private set; }
        public List<SkillTest> Tests { get; private set; } = new();

        public bool IsAvailable { get; private set; }
        public string ErrorMessage { get; private set; } = "";

        public PublicProfileViewModel(UserProfileService userProfileService)
        {
            this.userProfileService = userProfileService;
        }

        public void LoadPublicProfileCommand(int userId)
        {
            try
            {
                IsAvailable = userProfileService.IsProfileAvailable(userId);

                if (!IsAvailable)
                {
                    Profile = null;
                    Tests.Clear();
                    return;
                }

                Profile = userProfileService.GetProfile(userId);
                Tests = userProfileService.GetSkillTestsForUser(userId);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading public profile: {ex.Message}";
            }
        }
        public string GetAvailabilityMessage()
        {
            return IsAvailable ? "" : "Profile Unavailable";
        }

    }
}

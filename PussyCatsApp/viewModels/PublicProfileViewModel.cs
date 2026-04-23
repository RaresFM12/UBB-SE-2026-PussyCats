using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models;
using PussyCatsApp.Services;

namespace PussyCatsApp.ViewModels
{
    public class PublicProfileViewModel
    {
        private readonly UserProfileService userProfileService;

        public UserProfile? Profile { get; private set; }
        public List<SkillTest> Tests { get; private set; } = new ();

        public bool IsAvailable { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;

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
            catch (Exception exception)
            {
                ErrorMessage = $"Error loading public profile: {exception.Message}";
            }
        }
        public string GetAvailabilityMessage()
        {
            if (IsAvailable)
            {
                return string.Empty;
            }
            return "Profile Unavailable";
        }
    }
}

using PussyCatsApp.models;
using PussyCatsApp.repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.services
{
    internal class UserProfileService
    {

        private UserProfileRepository userProfileRepository = new UserProfileRepository();

        public UserProfileService(UserProfileRepository userProfileRepository)
        {
            this.userProfileRepository = userProfileRepository;

        }

        public UserProfile GetProfile(int userId)
        {
            return userProfileRepository.getProfileById(userId);
        }

        public void ToggleAccountStatus(int userId, string currentStatus)
        {
            string newStatus = currentStatus == "ACTIVE" ? "INACTIVE" : "ACTIVE";
            userProfileRepository.updateAccountStatus(userId, newStatus);

            userProfileRepository.updateProfileLastModified(userId, DateTime.Now);

        }

        public void UpdateAvatarPath(int userId, string newPath)
        {
            userProfileRepository.updateProfilePicture(userId, newPath);

            userProfileRepository.updateProfileLastModified(userId, DateTime.Now);

        }

        public void RemoveAvatarPath(int userId)
        {
            userProfileRepository.updateProfilePicture(userId, null);

            userProfileRepository.updateProfileLastModified(userId, DateTime.Now);
        }

    }
}

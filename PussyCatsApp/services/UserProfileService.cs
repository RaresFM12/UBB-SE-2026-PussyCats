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
        SkillTestRepository skillTestRepository;
        private UserProfileRepository userProfileRepository;
        public List<SkillTest> GetSkillTestsForUser(int userId)
        {
            return skillTestRepository.GetSkillTestsByUserId(userId);
        }

        public bool isProfileAvailable(int userId)
        {
            UserProfile userProfile = userProfileRepository.getProfileById(userId);

            if (userProfile == null)
                throw new Exception($"No profile found for ID {userId}");

            return userProfile.isActive();
        }
    }
}

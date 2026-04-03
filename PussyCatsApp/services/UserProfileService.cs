using PussyCatsApp.models;
using PussyCatsApp.repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.services
{
    public class UserProfileService
    {
        private readonly SkillTestRepository skillTestRepository;
        private readonly UserProfileRepository userProfileRepository;

        public UserProfileService()
        {
            this.userProfileRepository = new UserProfileRepository();
            this.skillTestRepository = new SkillTestRepository();
        }

        public UserProfile GetProfile(int userId)
        {
            return userProfileRepository.getProfileById(userId);
        }

        public List<SkillTest> GetSkillTestsForUser(int userId)
        {
            return skillTestRepository.GetSkillTestsByUserId(userId);
        }

        public bool IsProfileAvailable(int userId)
        {
            UserProfile userProfile = userProfileRepository.getProfileById(userId);

            if (userProfile == null)
                throw new Exception($"No profile found for ID {userId}");

            return userProfile.ActiveAccount;
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

        public string GenerateParsedCVText(UserProfile profile)
        {
            if (profile == null) return string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine($"{profile.FirstName} {profile.LastName}".Trim());
            sb.AppendLine(profile.University ?? string.Empty);
            sb.AppendLine(string.Join(", ", profile.Skills ?? new List<string>()));
            return sb.ToString().TrimEnd();
        }
        public void SaveProfile(int userId, UserProfile profile)
        {
            profile.ParsedCV = GenerateParsedCVText(profile);
            userProfileRepository.save(userId, profile);
            userProfileRepository.updateProfileLastModified(userId, DateTime.Now);
        }

        public int RecalculateLevel(UserProfile profile)
        {
            if (profile == null) return 0;
            int totalXP = 0;

           
            List<SkillTest> tests = GetSkillTestsForUser(profile.UserId);
            foreach (SkillTest test in tests)
            {
                totalXP += test.getXP();
            }

            profile.UserLevel = UserLevel.calculateLevel(totalXP);

            return totalXP;
            
        }
    }
}
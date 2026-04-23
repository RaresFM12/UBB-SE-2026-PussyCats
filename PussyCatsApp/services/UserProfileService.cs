using System;
using System.Collections.Generic;
using System.Text;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;

namespace PussyCatsApp.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly ISkillTestRepository skillTestRepository;
        private readonly IUserProfileRepository userProfileRepository;

        public UserProfileService(ISkillTestRepository skillTestRepository, IUserProfileRepository userProfileRepository)
        {
            this.skillTestRepository = skillTestRepository;
            this.userProfileRepository = userProfileRepository;
        }

        public UserProfile GetProfile(int userId)
        {
            return userProfileRepository.GetProfileById(userId);
        }

        public List<SkillTest> GetSkillTestsForUser(int userId)
        {
            return skillTestRepository.GetSkillTestsByUserId(userId);
        }

        public bool IsProfileAvailable(int userId)
        {
            UserProfile userProfile = userProfileRepository.GetProfileById(userId);

            if (userProfile == null)
            {
                throw new Exception($"No profile found for ID {userId}");
            }

            return userProfile.ActiveAccount;
        }

        public void ToggleAccountStatus(int userId, string currentStatus)
        {
            string newStatus;

            if (currentStatus == AccountStatus.Active.ToString().ToUpper())
            {
                newStatus = AccountStatus.Inactive.ToString().ToUpper();
            }
            else
            {
                newStatus = AccountStatus.Active.ToString().ToUpper();
            }
            userProfileRepository.UpdateAccountStatus(userId, newStatus);
            userProfileRepository.UpdateProfileLastModified(userId, DateTime.Now);
        }

        public void UpdateAvatarPath(int userId, string newPath)
        {
            userProfileRepository.UpdateProfilePicture(userId, newPath);
            userProfileRepository.UpdateProfileLastModified(userId, DateTime.Now);
        }

        public void RemoveAvatarPath(int userId)
        {
            userProfileRepository.UpdateProfilePicture(userId, null);
            userProfileRepository.UpdateProfileLastModified(userId, DateTime.Now);
        }

        public string GenerateParsedCVText(UserProfile profile)
        {
            if (profile == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            sb.AppendLine($"{profile.FirstName} {profile.LastName}".Trim());
            sb.AppendLine(profile.University ?? string.Empty);
            sb.AppendLine(string.Join(", ", profile.Skills ?? new List<string>()));
            return sb.ToString().TrimEnd();
        }
        public void SaveProfile(int userId, UserProfile profile)
        {
            profile.ParsedCV = GenerateParsedCVText(profile);
            userProfileRepository.Save(userId, profile);
            userProfileRepository.UpdateProfileLastModified(userId, DateTime.Now);
        }

        public int RecalculateLevel(UserProfile profile)
        {
            if (profile == null)
            {
                return 0;
            }

            int totalXP = 0;

            List<SkillTest> tests = GetSkillTestsForUser(profile.UserId);
            foreach (SkillTest test in tests)
            {
                totalXP += test.GetExperiencePoints();
            }

            profile.UserLevel = UserLevel.CalculateLevel(totalXP);

            return totalXP;
        }
    }
}
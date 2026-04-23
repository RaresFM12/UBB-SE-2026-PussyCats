using System;
using PussyCatsApp.Models;

namespace PussyCatsApp.Services
{
    public class CompletenessService : ICompletenessService
    {
        private const int TotalFields = 21;

        private static readonly string[] Labels =
        {
            "First Name", "Last Name", "Age", "Gender", "Country",
            "Phone Number", "Email", "University", "Graduation Year", "GitHub",
            "LinkedIn", "Address", "Profile Picture", "Skills", "Motivation",
            "Work Experience", "Projects", "Activities", "Preferred Roles",
            "Work Mode", "Location Preference"
        };
        private bool IsFieldFilled(int index, UserProfile p)
        {
            switch (index)
            {
                case 0: return !string.IsNullOrWhiteSpace(p.FirstName);
                case 1: return !string.IsNullOrWhiteSpace(p.LastName);
                case 2: return p.Age > 0;
                case 3: return !string.IsNullOrWhiteSpace(p.Gender);
                case 4: return !string.IsNullOrWhiteSpace(p.Country);
                case 5: return !string.IsNullOrWhiteSpace(p.PhoneNumber);
                case 6: return !string.IsNullOrWhiteSpace(p.Email);
                case 7: return !string.IsNullOrWhiteSpace(p.University);
                case 8: return p.ExpectedGraduationYear > 0;
                case 9: return !string.IsNullOrWhiteSpace(p.GitHub);
                case 10: return !string.IsNullOrWhiteSpace(p.LinkedIn);
                case 11: return !string.IsNullOrWhiteSpace(p.Address);
                case 12: return !string.IsNullOrWhiteSpace(p.ProfilePicture);
                case 13: return p.Skills != null && p.Skills.Count > 0;
                case 14: return !string.IsNullOrWhiteSpace(p.Motivation);
                case 15: return p.WorkExperiences != null && p.WorkExperiences.Count > 0;
                case 16: return p.Projects != null && p.Projects.Count > 0;
                case 17: return p.ExtraCurricularActivities != null && p.ExtraCurricularActivities.Count > 0;
                case 18: return p.PreferredJobRoles != null && p.PreferredJobRoles.Count > 0;
                case 19: return !string.IsNullOrWhiteSpace(p.WorkModePreference);
                case 20: return !string.IsNullOrWhiteSpace(p.LocationPreference);
                default: return false;
            }
        }
        private int CalculatePercentage(int filledFields)
        {
            return (int)Math.Round((double)filledFields / TotalFields * 100);
        }
        private int CountFilledFields(UserProfile profile)
        {
            int filledFields = 0;

            for (int i = 0; i < TotalFields; i++)
            {
                if (IsFieldFilled(i, profile))
                {
                    filledFields++;
                }
            }

            return filledFields;
        }

        public int CalculateCompleteness(UserProfile profile)
        {
            if (profile == null)
            {
                return 0;
            }
            int filledFields = CountFilledFields(profile);
            return CalculatePercentage(filledFields);
        }

        public string GetNextEmptyFieldPrompt(UserProfile profile)
        {
            if (profile == null)
            {
                return string.Empty;
            }

            int filledFields = CountFilledFields(profile);

            for (int i = 0; i < TotalFields; i++)
            {
                if (!IsFieldFilled(i, profile))
                {
                    int nextPercentage = CalculatePercentage(filledFields + 1);
                    return $"Add your {Labels[i]} to reach {nextPercentage}% completeness!";
                }
            }

            return "Your profile is 100% complete!";
        }
    }
}
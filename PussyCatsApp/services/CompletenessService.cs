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

        public int CalculateCompleteness(UserProfile profile)
        {
            if (profile == null)
            {
                return 0;
            }

            int filled = 0;

            for (int i = 0; i < TotalFields; i++)
            {
                if (IsFieldFilled(i, profile))
                {
                    filled++;
                }
            }

            return CalculatePercentage(filled);
        }

        public string GetNextEmptyFieldPrompt(UserProfile profile)
        {
            if (profile == null)
            {
                return string.Empty;
            }

            int filled = 0;

            for (int i = 0; i < TotalFields; i++)
            {
                if (IsFieldFilled(i, profile))
                {
                    filled++;
                }
            }

            for (int i = 0; i < TotalFields; i++)
            {
                if (!IsFieldFilled(i, profile))
                {
                    int nextPercentage = CalculatePercentage(filled + 1);
                    return $"Add your {Labels[i]} to reach {nextPercentage}% completeness!";
                }
            }

            return "Your profile is 100% complete!";
        }
    }
}

/*using System;
using System.Collections.Generic;
using System.Linq;
using PussyCatsApp.Models;

namespace PussyCatsApp.Services
{
    public class CompletenessService : ICompletenessService
    {
        private static readonly List<(string Label, Func<UserProfile, bool> IsFilled)> ProfileFields = new ()
        {
            ("First Name",        p => !string.IsNullOrWhiteSpace(p.FirstName)),
            ("Last Name",         p => !string.IsNullOrWhiteSpace(p.LastName)),
            ("Age",               p => p.Age > 0),
            ("Gender",            p => !string.IsNullOrWhiteSpace(p.Gender)),
            ("Country",           p => !string.IsNullOrWhiteSpace(p.Country)),
            ("Phone Number",      p => !string.IsNullOrWhiteSpace(p.PhoneNumber)),
            ("Email",             p => !string.IsNullOrWhiteSpace(p.Email)),
            ("University",        p => !string.IsNullOrWhiteSpace(p.University)),
            ("Graduation Year",   p => p.ExpectedGraduationYear > 0),
            ("GitHub",            p => !string.IsNullOrWhiteSpace(p.GitHub)),
            ("LinkedIn",          p => !string.IsNullOrWhiteSpace(p.LinkedIn)),
            ("Address",           p => !string.IsNullOrWhiteSpace(p.Address)),
            ("Profile Picture",   p => !string.IsNullOrWhiteSpace(p.ProfilePicture)),
            ("Skills",            p => p.Skills != null && p.Skills.Count > 0),
            ("Motivation",        p => !string.IsNullOrWhiteSpace(p.Motivation)),
            ("Work Experience",   p => p.WorkExperiences != null && p.WorkExperiences.Count > 0),
            ("Projects",          p => p.Projects != null && p.Projects.Count > 0),
            ("Activities",        p => p.ExtraCurricularActivities != null && p.ExtraCurricularActivities.Count > 0),
            ("Preferred Roles",   p => p.PreferredJobRoles != null && p.PreferredJobRoles.Count > 0),
            ("Work Mode",         p => !string.IsNullOrWhiteSpace(p.WorkModePreference)),
            ("Location Pref.",    p => !string.IsNullOrWhiteSpace(p.LocationPreference)),
        };

        /// <summary>
        /// R39 — Calculates completeness as (filled / total) × 100.
        /// </summary>
        public int CalculateCompleteness(UserProfile profile)
        {
            if (profile == null)
            {
                return 0;
            }

            int filled = ProfileFields.Count(f => f.IsFilled(profile));
            return (int)Math.Round((double)filled / ProfileFields.Count * 100);
        }

        /// <summary>
        /// R41 — Returns the friendly label of the first empty field
        /// and the percentage the user would reach by filling it.
        /// </summary>
        public string GetNextEmptyFieldPrompt(UserProfile profile)
        {
            if (profile == null)
            {
                return string.Empty;
            }

            int currentFilled = ProfileFields.Count(f => f.IsFilled(profile));
            int total = ProfileFields.Count;

            var nextEmpty = ProfileFields.FirstOrDefault(f => !f.IsFilled(profile));
            if (nextEmpty.Label == null)
            {
                return "Your profile is 100% complete!";
            }

            int nextPercentage = (int)Math.Round((double)(currentFilled + 1) / total * 100);
            return $"Add your {nextEmpty.Label} to reach {nextPercentage}% completeness!";
        }
    }
}
*/

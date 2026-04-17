using PussyCatsApp.models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PussyCatsApp.services
{
    public class CompletenessService: ICompletenessService
    {
        private static readonly List<(string Label, Func<UserProfile, bool> IsFilled)> ProfileFields = new()
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
            if (profile == null) return 0;

            int filled = ProfileFields.Count(f => f.IsFilled(profile));
            return (int)Math.Round((double)filled / ProfileFields.Count * 100);
        }

        /// <summary>
        /// R41 — Returns the friendly label of the first empty field
        /// and the percentage the user would reach by filling it.
        /// </summary>
        public string GetNextEmptyFieldPrompt(UserProfile profile)
        {
            if (profile == null) return "";

            int currentFilled = ProfileFields.Count(f => f.IsFilled(profile));
            int total = ProfileFields.Count;

            var nextEmpty = ProfileFields.FirstOrDefault(f => !f.IsFilled(profile));
            if (nextEmpty.Label == null) return "Your profile is 100% complete!";

            int nextPercentage = (int)Math.Round((double)(currentFilled + 1) / total * 100);
            return $"Add your {nextEmpty.Label} to reach {nextPercentage}% completeness!";
        }
    }
}

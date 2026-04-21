using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using System.Xml.Serialization;
using PussyCatsApp.models;
using PussyCatsApp.Repositories;
using PussyCatsApp.services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PussyCatsApp.utilities;

namespace PussyCatsApp.viewModels
{
    public partial class ProfileFormViewModel : ObservableObject
    {
        private readonly IUserProfileService profileService;
        private readonly ICVParsingService cvParsingService;
        private UserProfile userProfile;

        // Required Fields
        [ObservableProperty] private string firstName = string.Empty;
        [ObservableProperty] private string lastName = string.Empty;
        [ObservableProperty] private double age;
        [ObservableProperty] private string gender = string.Empty;
        [ObservableProperty] private string email = string.Empty;
        [ObservableProperty] private string phonePrefix = string.Empty;
        [ObservableProperty] private string phoneNumber = string.Empty;
        [ObservableProperty] private string gitHub = string.Empty;
        [ObservableProperty] private string linkedIn = string.Empty;
        [ObservableProperty] private string country = string.Empty;
        [ObservableProperty] private string city = string.Empty;
        [ObservableProperty] private string university = string.Empty;
        [ObservableProperty] private string address = string.Empty;
        [ObservableProperty] private int expectedGraduationYear;
        [ObservableProperty] private string motivation = string.Empty;
        [ObservableProperty] private bool hasDisabilities;

        // Status
        [ObservableProperty] private string errorMessage = string.Empty;
        [ObservableProperty] private string cvStatusText = string.Empty;
        [ObservableProperty] private string infoBarMessage = string.Empty;
        [ObservableProperty] private int infoBarSeverity; // 0=Info, 1=Success, 2=Warning, 3=Error
        [ObservableProperty] private bool isInfoBarOpen;

        public ObservableCollection<string> Skills { get; } = new ();
        public ObservableCollection<WorkExperience> WorkExperiences { get; } = new ();
        public ObservableCollection<Project> Projects { get; } = new ();
        public ObservableCollection<ExtraCurricularActivity> ExtraCurricularActivities { get; } = new ();

        public List<int> GraduationYears { get; } = new ();

        public ProfileFormViewModel(IUserProfileService profileService, ICVParsingService cvParsingService)
        {
            this.profileService = profileService;
            this.cvParsingService = cvParsingService;

            int currentYear = DateTime.Now.Year;
            for (int year = currentYear; year <= currentYear + 10; year++)
            {
                GraduationYears.Add(year);
            }
        }

        public static ProfileFormViewModel Create()
        {
            var userProfileRepo = new UserProfileRepository();
            var skillTestRepo = new SkillTestRepository();
            var profileService = new UserProfileService(skillTestRepo, userProfileRepo);
            var cvParsingService = new CVParsingService();
            return new ProfileFormViewModel(profileService, cvParsingService);
        }

        public void LoadProfile(UserProfile profile)
        {
            userProfile = profile ?? new UserProfile();

            FirstName = userProfile.FirstName;
            LastName = userProfile.LastName;
            Age = userProfile.Age;
            Gender = userProfile.Gender;
            Email = userProfile.Email;
            GitHub = userProfile.GitHub;
            LinkedIn = userProfile.LinkedIn;
            University = userProfile.University;
            Address = userProfile.Address;
            Motivation = userProfile.Motivation;
            Country = userProfile.Country;
            City = userProfile.City;
            ExpectedGraduationYear = userProfile.ExpectedGraduationYear;
            HasDisabilities = userProfile.HasDisabilities;

            // Extract phone prefix and number
            if (!string.IsNullOrEmpty(userProfile.PhoneNumber))
            {
                var parts = PhoneNumberHelper.ExtractPhonePrefixAndNumber(userProfile.PhoneNumber);
                PhonePrefix = parts.prefix;
                PhoneNumber = parts.number;
            }

            Skills.Clear();
            foreach (var skill in userProfile.Skills)
                Skills.Add(skill);

            WorkExperiences.Clear();
            foreach (var we in userProfile.WorkExperiences)
                WorkExperiences.Add(we);

            Projects.Clear();
            foreach (var project in userProfile.Projects)
                Projects.Add(project);

            ExtraCurricularActivities.Clear();
            foreach (var activity in userProfile.ExtraCurricularActivities)
                ExtraCurricularActivities.Add(activity);
        }

        public void AddSkill(string skill)
        {
            if (string.IsNullOrWhiteSpace(skill))
                return;

            skill = skill.Trim();

            if (Skills.Any(s => s.Equals(skill, StringComparison.OrdinalIgnoreCase)))
            {
                ShowInfoBar("This skill has already been added.", 2);
                return;
            }

            if (Skills.Count >= 30)
            {
                ShowInfoBar("Maximum of 30 skills allowed.", 2);
                return;
            }

            if (skill.Length > 60)
            {
                ShowInfoBar("Skill name must be less than 60 characters.", 2);
                return;
            }

            Skills.Add(skill);
        }

        public void RemoveSkill(string skill)
        {
            Skills.Remove(skill);
        }

        public void AddWorkExperience()
        {
            if (WorkExperiences.Count >= 10)
            {
                ShowInfoBar("Maximum of 10 work experiences allowed.", 2);
                return;
            }

            WorkExperiences.Add(new WorkExperience
            {
                StartDate = DateTimeOffset.Now,
                CurrentlyWorking = false
            });
        }

        public void RemoveWorkExperience(WorkExperience we)
        {
            WorkExperiences.Remove(we);
        }

        public void AddProject()
        {
            if (Projects.Count >= 10)
            {
                ShowInfoBar("Maximum of 10 projects allowed.", 2);
                return;
            }

            Projects.Add(new Project());
        }

        public void RemoveProject(Project project)
        {
            Projects.Remove(project);
        }

        public void AddExtraCurricularActivity()
        {
            if (ExtraCurricularActivities.Count >= 10)
            {
                ShowInfoBar("Maximum of 10 extra-curricular activities allowed.", 2);
                return;
            }

            ExtraCurricularActivities.Add(new ExtraCurricularActivity());
        }

        public void RemoveExtraCurricularActivity(ExtraCurricularActivity activity)
        {
            ExtraCurricularActivities.Remove(activity);
        }

        public bool SaveProfile()
        {
            var errors = ProfileFormValidator.ValidateForm(FirstName, LastName, Age, Gender, Email, PhonePrefix, PhoneNumber, Country, City, University, ExpectedGraduationYear, WorkExperiences.ToList());
            if (errors.Any())
            {
                ShowInfoBar($"Please fill in required fields: {string.Join(", ", errors)}", 3);
                return false;
            }

            UpdateProfileFromForm();

            try
            {
                profileService.SaveProfile(userProfile.UserId, userProfile);
                ShowInfoBar("Profile saved successfully!", 1);
                return true;
            }
            catch (Exception ex)
            {
                ShowInfoBar($"Error saving profile: {ex.Message}", 3);
                return false;
            }
        }

        public UserProfile GetUpdatedProfile()
        {
            UpdateProfileFromForm();
            return userProfile;
        }

        private void UpdateProfileFromForm()
        {
            userProfile.FirstName = FirstName.Trim();
            userProfile.LastName = LastName.Trim();
            userProfile.Age = (int)Age;
            userProfile.Gender = Gender;
            userProfile.Email = Email.Trim().ToLowerInvariant();
            userProfile.PhoneNumber = PhonePrefix + PhoneNumber.Trim();
            userProfile.GitHub = GitHub.Trim();
            userProfile.LinkedIn = LinkedIn.Trim();
            userProfile.Country = Country;
            userProfile.City = City.Trim();
            userProfile.University = University.Trim();
            userProfile.ExpectedGraduationYear = ExpectedGraduationYear;
            userProfile.Address = Address.Trim();
            userProfile.Motivation = Motivation.Trim();
            userProfile.HasDisabilities = HasDisabilities;
            userProfile.Skills = Skills.ToList();
            userProfile.WorkExperiences = WorkExperiences.ToList();
            userProfile.Projects = Projects.ToList();
            userProfile.ExtraCurricularActivities = ExtraCurricularActivities.ToList();
            userProfile.LastUpdated = DateTime.Now;
        }

        public void ProcessCVFile(string content, string fileType)
        {
            try
            {
                var parsedProfile = cvParsingService.ParseCVFile(content, fileType);
                PopulateFromParsedProfile(parsedProfile);
                CvStatusText = "CV loaded successfully!";
                ShowInfoBar("CV data has been loaded. Please review and complete any missing fields.", 1);
            }
            catch (Exception ex)
            {
                var errorMsg = ex.InnerException?.Message ?? ex.Message;
                ShowInfoBar($"Error processing CV file: {errorMsg}", 3);
            }
        }

        private void PopulateFromParsedProfile(UserProfile parsed)
        {
            if (!string.IsNullOrEmpty(parsed.FirstName)) FirstName = parsed.FirstName;
            if (!string.IsNullOrEmpty(parsed.LastName)) LastName = parsed.LastName;
            if (parsed.Age > 0) Age = parsed.Age;
            if (!string.IsNullOrEmpty(parsed.Gender)) Gender = parsed.Gender;
            if (!string.IsNullOrEmpty(parsed.Email)) Email = parsed.Email;
            if (!string.IsNullOrEmpty(parsed.PhoneNumber))
            {
                var parts = PhoneNumberHelper.ExtractPhonePrefixAndNumber(parsed.PhoneNumber);
                PhonePrefix = parts.prefix;
                PhoneNumber = parts.number;
            }
            if (!string.IsNullOrEmpty(parsed.GitHub)) GitHub = parsed.GitHub;
            if (!string.IsNullOrEmpty(parsed.LinkedIn)) LinkedIn = parsed.LinkedIn;
            if (!string.IsNullOrEmpty(parsed.Country)) Country = parsed.Country;
            if (!string.IsNullOrEmpty(parsed.City)) City = parsed.City;
            if (!string.IsNullOrEmpty(parsed.University)) University = parsed.University;
            if (parsed.ExpectedGraduationYear > 0) ExpectedGraduationYear = parsed.ExpectedGraduationYear;
            if (!string.IsNullOrEmpty(parsed.Address)) Address = parsed.Address;
            if (!string.IsNullOrEmpty(parsed.Motivation)) Motivation = parsed.Motivation;

            // Clear existing collections before loading new CV data
            Skills.Clear();
            WorkExperiences.Clear();
            Projects.Clear();
            ExtraCurricularActivities.Clear();

            // Add skills with duplicate detection (R20)
            if (parsed.Skills != null)
            {
                foreach (var skill in parsed.Skills)
                {
                    if (!Skills.Any(s => s.Equals(skill, StringComparison.OrdinalIgnoreCase)) && Skills.Count < 30)
                        Skills.Add(skill);
                }
            }

            if (parsed.WorkExperiences != null)
            {
                foreach (var we in parsed.WorkExperiences)
                {
                    if (WorkExperiences.Count < 10)
                        WorkExperiences.Add(we);
                }
            }

            if (parsed.Projects != null)
            {
                foreach (var proj in parsed.Projects)
                {
                    if (Projects.Count < 10)
                        Projects.Add(proj);
                }
            }

            if (parsed.ExtraCurricularActivities != null)
            {
                foreach (var activity in parsed.ExtraCurricularActivities)
                {
                    if (ExtraCurricularActivities.Count < 10)
                        ExtraCurricularActivities.Add(activity);
                }
            }

            // List missing fields (R18)
            var missingFields = new List<string>();
            if (string.IsNullOrEmpty(FirstName)) missingFields.Add("First Name");
            if (string.IsNullOrEmpty(LastName)) missingFields.Add("Last Name");
            if (Age == 0) missingFields.Add("Age");
            if (string.IsNullOrEmpty(Gender)) missingFields.Add("Gender");
            if (string.IsNullOrEmpty(Email)) missingFields.Add("Email");
            if (string.IsNullOrEmpty(PhoneNumber)) missingFields.Add("Phone Number");
            if (string.IsNullOrEmpty(Country)) missingFields.Add("Country");
            if (string.IsNullOrEmpty(City)) missingFields.Add("City");
            if (string.IsNullOrEmpty(University)) missingFields.Add("University");
            if (ExpectedGraduationYear == 0) missingFields.Add("Expected Graduation Year");

            if (missingFields.Any())
            {
                ShowInfoBar($"Missing fields: {string.Join(", ", missingFields)}", 2);
            }
        }

        public List<string> FilterUniversities(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return new List<string>();

            var splitText = query.ToLower().Split(' ');
            return ProfileFormData.UniversityList.Where(uni =>
                splitText.All(key => uni.ToLower().Contains(key))
            ).ToList();
        }

        public List<string> FilterSkillSuggestions(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return new List<string>();

            var searchText = query.ToLower();
            return ProfileFormData.SkillSuggestions.Where(skill =>
                skill.ToLower().Contains(searchText) &&
                !Skills.Any(s => s.Equals(skill, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        private void ShowInfoBar(string message, int severity)
        {
            InfoBarMessage = message;
            InfoBarSeverity = severity;
            IsInfoBarOpen = true;
        }
    }
}

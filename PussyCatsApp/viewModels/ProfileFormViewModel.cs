using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PussyCatsApp.Models;
using PussyCatsApp.repositories;
using PussyCatsApp.services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace PussyCatsApp.viewModels
{
    public partial class ProfileFormViewModel : ObservableObject
    {
        private readonly UserProfileService _profileService;
        private readonly CVParsingService _cvParsingService;
        private UserProfile _userProfile;

        // Required Fields
        [ObservableProperty] private string _firstName = string.Empty;
        [ObservableProperty] private string _lastName = string.Empty;
        [ObservableProperty] private double _age;
        [ObservableProperty] private string _gender = string.Empty;
        [ObservableProperty] private string _email = string.Empty;
        [ObservableProperty] private string _phonePrefix = string.Empty;
        [ObservableProperty] private string _phoneNumber = string.Empty;
        [ObservableProperty] private string _gitHub = string.Empty;
        [ObservableProperty] private string _linkedIn = string.Empty;
        [ObservableProperty] private string _country = string.Empty;
        [ObservableProperty] private string _city = string.Empty;
        [ObservableProperty] private string _university = string.Empty;
        [ObservableProperty] private string _address = string.Empty;
        [ObservableProperty] private int _expectedGraduationYear;
        [ObservableProperty] private string _motivation = string.Empty;
        [ObservableProperty] private bool _hasDisabilities;

        // Status
        [ObservableProperty] private string _errorMessage = string.Empty;
        [ObservableProperty] private string _cvStatusText = string.Empty;
        [ObservableProperty] private string _infoBarMessage = string.Empty;
        [ObservableProperty] private int _infoBarSeverity; // 0=Info, 1=Success, 2=Warning, 3=Error
        [ObservableProperty] private bool _isInfoBarOpen;

        public ObservableCollection<string> Skills { get; } = new();
        public ObservableCollection<WorkExperience> WorkExperiences { get; } = new();
        public ObservableCollection<Project> Projects { get; } = new();
        public ObservableCollection<ExtraCurricularActivity> ExtraCurricularActivities { get; } = new();

        public List<string> UniversityList { get; } = new()
        {
            "Babes-Bolyai University",
            "Technical University of Cluj-Napoca",
            "University of Bucharest",
            "Politehnica University of Bucharest",
            "Alexandru Ioan Cuza University",
            "West University of Timisoara",
            "University of Medicine and Pharmacy Cluj-Napoca",
            "Academy of Economic Studies Bucharest"
        };

        public List<string> SkillSuggestions { get; } = new()
        {
            // Languages
            "JavaScript", "TypeScript", "Python", "Java", "C#", "Go", "R", "Julia",

            // Frontend
            "HTML", "CSS", "SCSS", "Tailwind",

            // Frameworks & Libraries
            "React", "Angular", "Vue.js", "Svelte", "Node.js", "Spring Boot", "ASP.NET", "Django", "FastAPI",

            // Build Tools
            "Webpack", "Vite", "Parcel",

            // Version Control
            "Git", "GitHub",

            // Testing
            "Jest", "Cypress", "Selenium", "JUnit", "NUnit", "pytest",

            // APIs & Communication
            "REST", "GraphQL", "gRPC", "Postman",

            // Databases
            "SQL", "PostgreSQL", "MySQL", "MongoDB", "Redis", "BigQuery",

            // DevOps / Cloud
            "Docker", "Podman", "Kubernetes", "Docker Swarm", "OpenShift",
            "Jenkins", "GitHub Actions", "GitLab CI", "CircleCI",
            "AWS", "Azure", "Google Cloud",
            "Terraform", "Ansible", "Pulumi",
            "Prometheus", "Grafana", "Datadog",

            // Design & UX
            "Figma", "Adobe XD", "Zeplin", "Sketch", "InVision", "Marvel", "Axure",
            "Figma Prototyping", "Typography", "Color Theory", "Grid Systems",
            "Storybook",

            // Research & UX Methods
            "Interviews", "Surveys", "Usability Testing",

            // Analytics & BI
            "Google Analytics", "Hotjar", "Mixpanel",
            "Power BI", "Tableau", "Looker",
            "Excel", "Google Sheets", "OpenRefine",

            // Data Science & AI
            "Pandas", "NumPy", "TensorFlow", "PyTorch", "scikit-learn", "Keras",
            "Apache Spark", "MLflow", "Hugging Face",
            "OpenCV", "NLTK", "spaCy",
            "Descriptive Statistics", "Regression", "Hypothesis Testing",
            "Linear Algebra", "Calculus", "Probability", "Statistics",

            // Security
            "Firewalls", "VPN", "IDS/IPS", "TCP/IP",
            "Metasploit", "Burp Suite", "Nmap",
            "Splunk", "IBM QRadar", "Microsoft Sentinel",
            "AES", "RSA", "PKI", "TLS/SSL",
            "ISO 27001", "GDPR", "NIST", "SOC 2",
            "Forensics", "Malware Analysis", "DFIR",

            // Project Management
            "Scrum", "Kanban", "Agile", "Waterfall",
            "Jira", "Trello", "Asana",
            "Risk Assessment", "Mitigation Planning",
            "Stakeholder Management", "Reporting",
            "Presentations", "Cost Estimation", "Budget Tracking",
            "MS Project"
        };

        public List<int> GraduationYears { get; } = new();

        public ProfileFormViewModel(UserProfileService profileService, CVParsingService cvParsingService)
        {
            _profileService = profileService;
            _cvParsingService = cvParsingService;

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
            var profileService = new UserProfileService();
            var cvParsingService = new CVParsingService();
            return new ProfileFormViewModel(profileService, cvParsingService);
        }

        public void LoadProfile(UserProfile profile)
        {
            _userProfile = profile ?? new UserProfile();

            FirstName = _userProfile.FirstName;
            LastName = _userProfile.LastName;
            Age = _userProfile.Age;
            Gender = _userProfile.Gender;
            Email = _userProfile.Email;
            GitHub = _userProfile.GitHub;
            LinkedIn = _userProfile.LinkedIn;
            University = _userProfile.University;
            Address = _userProfile.Address;
            Motivation = _userProfile.Motivation;
            Country = _userProfile.Country;
            City = _userProfile.City;
            ExpectedGraduationYear = _userProfile.ExpectedGraduationYear;
            HasDisabilities = _userProfile.HasDisabilities;

            // Extract phone prefix and number
            if (!string.IsNullOrEmpty(_userProfile.PhoneNumber))
            {
                var parts = ExtractPhonePrefixAndNumber(_userProfile.PhoneNumber);
                PhonePrefix = parts.prefix;
                PhoneNumber = parts.number;
            }

            Skills.Clear();
            foreach (var skill in _userProfile.Skills)
                Skills.Add(skill);

            WorkExperiences.Clear();
            foreach (var we in _userProfile.WorkExperiences)
                WorkExperiences.Add(we);

            Projects.Clear();
            foreach (var project in _userProfile.Projects)
                Projects.Add(project);

            ExtraCurricularActivities.Clear();
            foreach (var activity in _userProfile.ExtraCurricularActivities)
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

        public List<string> ValidateForm()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(FirstName))
                errors.Add("First Name");
            if (string.IsNullOrWhiteSpace(LastName))
                errors.Add("Last Name");
            if (Age < 16 || Age > 60)
                errors.Add("Age (must be between 16-60)");
            if (string.IsNullOrWhiteSpace(Gender))
                errors.Add("Gender");
            if (string.IsNullOrWhiteSpace(Email) || !IsValidEmail(Email))
                errors.Add("Valid Email");
            if (string.IsNullOrWhiteSpace(PhonePrefix) || string.IsNullOrWhiteSpace(PhoneNumber))
                errors.Add("Phone Number");
            if (string.IsNullOrWhiteSpace(Country))
                errors.Add("Country");
            if (string.IsNullOrWhiteSpace(City))
                errors.Add("City");
            if (string.IsNullOrWhiteSpace(University))
                errors.Add("University");
            if (ExpectedGraduationYear == 0)
                errors.Add("Expected Graduation Year");

            foreach (var we in WorkExperiences)
            {
                if (!we.CurrentlyWorking && we.EndDate.HasValue && we.EndDate.Value < we.StartDate)
                    errors.Add($"Work Experience \"{we.Company}\": End date is before start date");
            }

            return errors;
        }

        public bool SaveProfile()
        {
            var errors = ValidateForm();
            if (errors.Any())
            {
                ShowInfoBar($"Please fill in required fields: {string.Join(", ", errors)}", 3);
                return false;
            }

            UpdateProfileFromForm();

            try
            {
                _profileService.SaveProfile(_userProfile.UserId, _userProfile);
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
            return _userProfile;
        }

        private void UpdateProfileFromForm()
        {
            _userProfile.FirstName = FirstName.Trim();
            _userProfile.LastName = LastName.Trim();
            _userProfile.Age = (int)Age;
            _userProfile.Gender = Gender;
            _userProfile.Email = Email.Trim().ToLowerInvariant();
            _userProfile.PhoneNumber = PhonePrefix + PhoneNumber.Trim();
            _userProfile.GitHub = GitHub.Trim();
            _userProfile.LinkedIn = LinkedIn.Trim();
            _userProfile.Country = Country;
            _userProfile.City = City.Trim();
            _userProfile.University = University.Trim();
            _userProfile.ExpectedGraduationYear = ExpectedGraduationYear;
            _userProfile.Address = Address.Trim();
            _userProfile.Motivation = Motivation.Trim();
            _userProfile.HasDisabilities = HasDisabilities;
            _userProfile.Skills = Skills.ToList();
            _userProfile.WorkExperiences = WorkExperiences.ToList();
            _userProfile.Projects = Projects.ToList();
            _userProfile.ExtraCurricularActivities = ExtraCurricularActivities.ToList();
            _userProfile.LastUpdated = DateTime.Now;
        }

        public void ProcessCVFile(string content, string fileType)
        {
            try
            {
                var parsedProfile = _cvParsingService.ParseCVFile(content, fileType);
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
                var parts = ExtractPhonePrefixAndNumber(parsed.PhoneNumber);
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
            return UniversityList.Where(uni =>
                splitText.All(key => uni.ToLower().Contains(key))
            ).ToList();
        }

        public List<string> FilterSkillSuggestions(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return new List<string>();

            var searchText = query.ToLower();
            return SkillSuggestions.Where(skill =>
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

        private static (string prefix, string number) ExtractPhonePrefixAndNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return ("", "");

            var prefixes = new[] {
                "+40", "+44", "+49", "+33", "+39", "+34", "+31", "+48", "+43", "+32",
                "+46", "+351", "+420", "+36", "+359", "+30", "+45", "+358", "+353", "+385",
                "+421", "+370", "+371", "+372", "+386", "+352", "+356", "+357",
                "+1", "+61",
                "+47", "+41", "+90", "+380", "+381", "+373", "+387", "+382", "+389", "+355", "+375", "+7"
            };
            // Sort by length descending so longer prefixes match first (e.g. +351 before +3)
            foreach (var prefix in prefixes.OrderByDescending(p => p.Length))
            {
                if (phoneNumber.StartsWith(prefix))
                    return (prefix, phoneNumber.Substring(prefix.Length));
            }
            return ("", phoneNumber);
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}

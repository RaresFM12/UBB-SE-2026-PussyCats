using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;
using System.Xml.Serialization;
using PussyCatsApp.Models;

namespace PussyCatsApp.Services
{
    public class CVParsingService : ICVParsingService
    {
        private const string JsonExtension = ".json";
        private const string XmlExtension = ".xml";
        private const string ParseErrorMessage = "Failed to parse CV file: ";

        private const int MaxSkills = 30;
        private const int MaxSkillLength = 60;
        private const int MaxWorkExperiences = 10;
        private const int MaxProjects = 10;
        private const int MaxActivities = 10;
        private const int MaxTechnologiesPerProject = 10;

        private const int MaxFirstNameLength = 50;
        private const int MaxLastNameLength = 60;
        private const int MaxCountryLength = 100;
        private const int MaxCityLength = 100;
        private const int MaxUniversityLength = 200;
        private const int MaxGitHubLength = 200;
        private const int MaxLinkedInLength = 200;
        private const int MaxAddressLength = 500;
        private const int MaxMotivationLength = 1000;
        private const int MaxEmailLength = 254;
        private const int MaxPhoneLength = 15;
        private const int MaxCompanyLength = 150;
        private const int MaxJobTitleLength = 100;
        private const int MaxWorkDescriptionLength = 500;
        private const int MaxProjectNameLength = 100;
        private const int MaxProjectDescriptionLength = 600;
        private const int MaxProjectUrlLength = 200;
        private const int MaxActivityNameLength = 150;
        private const int MaxOrganizationLength = 100;
        private const int MaxRoleLength = 80;
        private const int MaxPeriodLength = 60;
        private const int MaxActivityDescriptionLength = 300;

        private const int MinAge = 16;
        private const int MaxAge = 60;
        private const int InvalidAge = 0;
        private const int InvalidYear = 0;
        private const int MaxYearsAheadForGraduation = 10;

        private const string GenderMale = "Male";
        private const string GenderFemale = "Female";
        private const string GenderMaleShort = "M";
        private const string GenderFemaleShort = "F";

        private static readonly DateTime MinValidDate = new DateTime(1980, 1, 1);
        private const int MaxYearsAheadForDate = 1;

        /// <summary>
        /// Parses a CV file (JSON or XML) and returns a UserProfile object
        /// </summary>
        public UserProfile ParseCVFile(string content, string fileType)
        {
            try
            {
                if (string.Equals(fileType, JsonExtension, StringComparison.OrdinalIgnoreCase))
                {
                    var cvData = JsonSerializer.Deserialize<CVData>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return MapCVDataToProfile(cvData);
                }

                if (string.Equals(fileType, XmlExtension, StringComparison.OrdinalIgnoreCase))
                {
                    var xDoc = XDocument.Parse(content);
                    var rootName = xDoc.Root?.Name.LocalName ?? "CVData";

                    using var reader = new StringReader(content);

                    var serializer = new XmlSerializer(typeof(CVData), new XmlRootAttribute(rootName));
                    var cvData = (CVData)serializer.Deserialize(reader);

                    return MapCVDataToProfile(cvData);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ParseErrorMessage + ex.Message, ex);
            }

            return new UserProfile();
        }
        /*public UserProfile ParseCVFile(string content, string fileType)
        {
            UserProfile profile = new UserProfile();

            try
            {
                if (fileType.ToLower() == ".json")
                {
                    var cvData = JsonSerializer.Deserialize<CVData>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return MapCVDataToProfile(cvData);
                }
                else if (fileType.ToLower() == ".xml")
                {
                    var xDoc = XDocument.Parse(content);
                    var rootName = xDoc.Root?.Name.LocalName ?? "CVData";
                    using (var reader = new StringReader(content))
                    {
                        var serializer = new XmlSerializer(typeof(CVData), new XmlRootAttribute(rootName));
                        var cvData = (CVData)serializer.Deserialize(reader);
                        return MapCVDataToProfile(cvData);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to parse CV file: {ex.InnerException?.Message ?? ex.Message}", ex);
            }

            return profile;
        }*/

        /// <summary>
        /// Maps CV data structure to UserProfile
        /// </summary>
        private UserProfile MapCVDataToProfile(CVData cvData)
        {
            if (cvData == null)
            {
                return new UserProfile();
            }

            return new UserProfile
            {
                FirstName = SanitizeString(cvData.FirstName, MaxFirstNameLength),
                LastName = SanitizeString(cvData.LastName, MaxLastNameLength),
                Age = ValidateAge(cvData.Age),
                Gender = ValidateGender(cvData.Gender),
                Email = SanitizeEmail(cvData.Email),
                PhoneNumber = FormatPhoneNumber(cvData.PhoneNumber),
                Country = SanitizeString(cvData.Country, MaxCountryLength),
                City = SanitizeString(cvData.City, MaxCityLength),
                University = SanitizeString(cvData.University, MaxUniversityLength),
                ExpectedGraduationYear = ValidateGraduationYear(cvData.ExpectedGraduationYear),

                GitHub = SanitizeString(cvData.GitHub, MaxGitHubLength),
                LinkedIn = SanitizeString(cvData.LinkedIn, MaxLinkedInLength),
                Address = SanitizeString(cvData.Address, MaxAddressLength),
                Motivation = SanitizeString(cvData.Motivation, MaxMotivationLength),

                Skills = ProcessSkills(cvData.Skills),
                WorkExperiences = ProcessWorkExperiences(cvData.WorkExperiences),
                Projects = ProcessProjects(cvData.Projects),
                ExtraCurricularActivities = ProcessActivities(cvData.ExtraCurricularActivities)
            };
        }
        /*private UserProfile MapCVDataToProfile(CVData cvData)
        {
            if (cvData == null)
            {
                return new UserProfile();
            }

            var profile = new UserProfile
            {
                // Map required fields
                FirstName = SanitizeString(cvData.FirstName, 50),
                LastName = SanitizeString(cvData.LastName, 60),
                Age = ValidateAge(cvData.Age),
                Gender = ValidateGender(cvData.Gender),
                Email = SanitizeEmail(cvData.Email),
                PhoneNumber = FormatPhoneNumber(cvData.PhoneNumber),
                Country = SanitizeString(cvData.Country, 100),
                City = SanitizeString(cvData.City, 100),
                University = SanitizeString(cvData.University, 200),
                ExpectedGraduationYear = ValidateGraduationYear(cvData.ExpectedGraduationYear),

                // Map optional fields
                GitHub = SanitizeString(cvData.GitHub, 200),
                LinkedIn = SanitizeString(cvData.LinkedIn, 200),
                Address = SanitizeString(cvData.Address, 500),
                Motivation = SanitizeString(cvData.Motivation, 1000),

                // Map collections with validation
                Skills = ProcessSkills(cvData.Skills),
                WorkExperiences = ProcessWorkExperiences(cvData.WorkExperiences),
                Projects = ProcessProjects(cvData.Projects),
                ExtraCurricularActivities = ProcessActivities(cvData.ExtraCurricularActivities)
            };

            return profile;
        }*/

        /// <summary>
        /// Processes and validates skills, removing duplicates
        /// </summary>
        private List<string> ProcessSkills(List<string> skills)
        {
            var result = new List<string>();

            if (skills == null)
            {
                return result;
            }

            var addedSkills = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            int count = 0;

            foreach (var skill in skills)
            {
                if (count >= MaxSkills)
                {
                    break;
                }

                var sanitized = SanitizeString(skill, MaxSkillLength);

                if (!string.IsNullOrWhiteSpace(sanitized) && addedSkills.Add(sanitized))
                {
                    result.Add(sanitized);
                    count++;
                }
            }

            return result;
        }

        /// <summary>
        /// Processes work experiences with validation
        /// </summary>
        private List<WorkExperience> ProcessWorkExperiences(List<WorkExperience> experiences)
        {
            var result = new List<WorkExperience>();

            if (experiences == null)
            {
                return result;
            }

            int count = 0;

            foreach (var we in experiences)
            {
                if (count >= MaxWorkExperiences)
                {
                    break;
                }

                var processed = new WorkExperience
                {
                    Company = SanitizeString(we.Company, MaxCompanyLength),
                    JobTitle = SanitizeString(we.JobTitle, MaxJobTitleLength),
                    StartDate = ValidateDate(we.StartDate),
                    EndDate = we.CurrentlyWorking ? null : ValidateDate(we.EndDate),
                    CurrentlyWorking = we.CurrentlyWorking,
                    Description = SanitizeString(we.Description, MaxWorkDescriptionLength)
                };

                if (!string.IsNullOrEmpty(processed.Company) &&
                    !string.IsNullOrEmpty(processed.JobTitle))
                {
                    result.Add(processed);
                    count++;
                }
            }

            return result;
        }
        /*
        private List<WorkExperience> ProcessWorkExperiences(List<WorkExperience> experiences)
        {
            if (experiences == null || !experiences.Any())
            {
                return new List<WorkExperience>();
            }

            return experiences.Take(10) // Max 10 experiences
                .Select(we => new WorkExperience
                {
                    Company = SanitizeString(we.Company, 150),
                    JobTitle = SanitizeString(we.JobTitle, 100),
                    StartDate = ValidateDate(we.StartDate),
                    EndDate = we.CurrentlyWorking ? null : ValidateDate(we.EndDate),
                    CurrentlyWorking = we.CurrentlyWorking,
                    Description = SanitizeString(we.Description, 500)
                })
                .Where(we => !string.IsNullOrEmpty(we.Company) && !string.IsNullOrEmpty(we.JobTitle))
                .ToList();
        }
        */

        /// <summary>
        /// Processes projects with validation
        /// </summary>
        private List<Project> ProcessProjects(List<Project> projects)
        {
            var result = new List<Project>();

            if (projects == null)
            {
                return result;
            }

            int count = 0;

            foreach (var p in projects)
            {
                if (count >= MaxProjects)
                {
                    break;
                }

                var proj = new Project
                {
                    Name = SanitizeString(p.Name, MaxProjectNameLength),
                    Description = SanitizeString(p.Description, MaxProjectDescriptionLength),
                    Url = SanitizeString(p.Url, MaxProjectUrlLength),
                    Technologies = new List<string>()
                };

                if (p.Technologies != null)
                {
                    int techCount = 0;
                    foreach (var t in p.Technologies)
                    {
                        if (techCount >= MaxTechnologiesPerProject)
                        {
                            break;
                        }

                        proj.Technologies.Add(SanitizeString(t, MaxSkillLength));
                        techCount++;
                    }
                }

                if (!string.IsNullOrEmpty(proj.Name))
                {
                    result.Add(proj);
                    count++;
                }
            }

            return result;
        }
        /*private List<Project> ProcessProjects(List<Project> projects)
        {
            if (projects == null || !projects.Any())
            {
                return new List<Project>();
            }

            return projects.Take(10) // Max 10 projects
                .Select(p => new Project
                {
                    Name = SanitizeString(p.Name, 100),
                    Description = SanitizeString(p.Description, 600),
                    Technologies = p.Technologies?.Take(10).Select(t => SanitizeString(t, 60)).ToList() ?? new List<string>(),
                    Url = SanitizeString(p.Url, 200)
                })
                .Where(p => !string.IsNullOrEmpty(p.Name))
                .ToList();
        }*/

        /// <summary>
        /// Processes extracurricular activities with validation
        /// </summary>
        /*
        private List<ExtraCurricularActivity> ProcessActivities(List<ExtraCurricularActivity> activities)
        {
            if (activities == null || !activities.Any())
            {
                return new List<ExtraCurricularActivity>();
            }

            return activities.Take(10) // Max 10 activities
                .Select(a => new ExtraCurricularActivity
                {
                    ActivityName = SanitizeString(a.ActivityName, 150),
                    Organization = SanitizeString(a.Organization, 100),
                    Role = SanitizeString(a.Role, 80),
                    Period = SanitizeString(a.Period, 60),
                    Description = SanitizeString(a.Description, 300)
                })
                .Where(a => !string.IsNullOrEmpty(a.ActivityName))
                .ToList();
        }*/
        private List<ExtraCurricularActivity> ProcessActivities(List<ExtraCurricularActivity> activities)
        {
            var result = new List<ExtraCurricularActivity>();

            if (activities == null)
            {
                return result;
            }

            int count = 0;

            foreach (var a in activities)
            {
                if (count >= MaxActivities)
                    {
                        break;
                    }

                var activity = new ExtraCurricularActivity
                {
                    ActivityName = SanitizeString(a.ActivityName, MaxActivityNameLength),
                    Organization = SanitizeString(a.Organization, MaxOrganizationLength),
                    Role = SanitizeString(a.Role, MaxRoleLength),
                    Period = SanitizeString(a.Period, MaxPeriodLength),
                    Description = SanitizeString(a.Description, MaxActivityDescriptionLength)
                };

                if (!string.IsNullOrEmpty(activity.ActivityName))
                {
                    result.Add(activity);
                    count++;
                }
            }

            return result;
        }

        /// <summary>
        /// Sanitizes and validates a string field
        /// </summary>
        private string SanitizeString(string input, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            input = input.Trim();

            if (input.Length > maxLength)
            {
                input = input.Substring(0, maxLength);
            }

            return input;
        }

        /// <summary>
        /// Validates and sanitizes email
        /// </summary>
        private string SanitizeEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return string.Empty;
            }

            email = email.Trim().ToLowerInvariant();

            if (email.Length > MaxEmailLength)
            {
                return string.Empty;
            }

            // Basic email validation
            if (!email.Contains('@') || !email.Contains('.'))
            {
                return string.Empty;
            }

            return email;
        }

        /// <summary>
        /// Validates age range
        /// </summary>
        private int ValidateAge(int age)
        {
            if (age < MinAge || age > MaxAge)
            {
                return InvalidAge;
            }
            return age;
        }

        /// <summary>
        /// Validates gender value
        /// </summary>
        private string ValidateGender(string gender)
        {
            if (string.IsNullOrWhiteSpace(gender))
            {
                return string.Empty;
            }

            gender = gender.Trim();

            if (gender.Equals(GenderMale, StringComparison.OrdinalIgnoreCase) ||
        gender.Equals(GenderMaleShort, StringComparison.OrdinalIgnoreCase))
            {
                return GenderMale;
            }

            if (gender.Equals(GenderFemale, StringComparison.OrdinalIgnoreCase) ||
                gender.Equals(GenderFemaleShort, StringComparison.OrdinalIgnoreCase))
            {
                return GenderFemale;
            }

            return string.Empty;
        }

        /// <summary>
        /// Formats phone number with validation
        /// </summary>
        private string FormatPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return string.Empty;
            }

            // Remove all non-digit characters except + at the beginning
            phoneNumber = phoneNumber.Trim();

            if (phoneNumber.Length > MaxPhoneLength)
            {
                return string.Empty;
            }

            return phoneNumber;
        }

        /// <summary>
        /// Validates graduation year
        /// </summary>
        private int ValidateGraduationYear(int year)
        {
            int currentYear = DateTime.Now.Year;

            if (year < currentYear || year > currentYear + 10)
            {
                return 0;
            }

            return year;
        }

        /// <summary>
        /// Validates date values
        /// </summary>
        private DateTimeOffset ValidateDate(DateTimeOffset? date)
        {
            if (!date.HasValue)
            {
                return DateTimeOffset.Now;
            }

            var minDate = new DateTimeOffset(MinValidDate);
            var maxDate = DateTimeOffset.Now.AddYears(MaxYearsAheadForDate);

            if (date < minDate || date > maxDate)
            {
                return DateTimeOffset.Now;
            }
            return date.Value;
        }
    }

    /// <summary>
    /// CV Data structure for deserialization
    /// </summary>
    public class CVData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string University { get; set; }
        public int ExpectedGraduationYear { get; set; }
        public string GitHub { get; set; }
        public string LinkedIn { get; set; }
        public string Address { get; set; }
        public string Motivation { get; set; }
        public bool HasDisabilities { get; set; }
        public List<string> Skills { get; set; }
        public List<WorkExperience> WorkExperiences { get; set; }
        public List<Project> Projects { get; set; }
        public List<ExtraCurricularActivity> ExtraCurricularActivities { get; set; }
    }
}
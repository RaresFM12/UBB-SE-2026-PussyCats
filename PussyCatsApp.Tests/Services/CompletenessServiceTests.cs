using PussyCatsApp.Models;
using PussyCatsApp.Services;

namespace PussyCatsApp.Tests.Services
{
    [TestClass]
    public class CompletenessServiceTest
    {
        [TestMethod]
        public void CalculateCompleteness_NullProfile_ReturnsZero()
        {
            var service = new CompletenessService();

            var result = service.CalculateCompleteness(null);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void CalculateCompleteness_FullProfile_Returns100()
        {
            var service = new CompletenessService();

            var profile = new UserProfile
            {
                FirstName = "Ana",
                LastName = "Pop",
                Age = 22,
                Gender = "Female",
                Country = "Romania",
                PhoneNumber = "0712345678",
                Email = "ana@email.com",
                University = "UBB",
                ExpectedGraduationYear = 2025,
                GitHub = "github.com/ana",
                LinkedIn = "linkedin.com/ana",
                Address = "Cluj",
                ProfilePicture = "pic.jpg",
                Skills = new List<string> { "React" },
                Motivation = "I love coding",
                WorkExperiences = new List<WorkExperience> { new WorkExperience() },
                Projects = new List<Project> { new Project() },
                ExtraCurricularActivities = new List<ExtraCurricularActivity> { new ExtraCurricularActivity() },
                PreferredJobRoles = new List<string> { "Backend" },
                WorkModePreference = "Remote",
                LocationPreference = "Bucuresti"
            };

            var result = service.CalculateCompleteness(profile);

            Assert.AreEqual(100, result);
        }
        [TestMethod]
        public void GetNextEmptyFieldPrompt_NullProfile_ReturnsEmpty()
        {
            var service = new CompletenessService();

            var result = service.GetNextEmptyFieldPrompt(null);

            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void GetNextEmptyFieldPrompt_AllFilled_ReturnsCompleteMessage()
        {
            var service = new CompletenessService();

            var profile = new UserProfile
            {
                FirstName = "Ana",
                LastName = "Pop",
                Age = 22,
                Gender = "Female",
                Country = "Romania",
                PhoneNumber = "0712345678",
                Email = "ana@email.com",
                University = "UBB",
                ExpectedGraduationYear = 2025,
                GitHub = "github.com/ana",
                LinkedIn = "linkedin.com/ana",
                Address = "Cluj",
                ProfilePicture = "pic.jpg",
                Skills = new List<string> { "React" },
                Motivation = "I love coding",
                WorkExperiences = new List<WorkExperience> { new WorkExperience() },
                Projects = new List<Project> { new Project() },
                ExtraCurricularActivities = new List<ExtraCurricularActivity> { new ExtraCurricularActivity() },
                PreferredJobRoles = new List<string> { "Backend" },
                WorkModePreference = "Remote",
                LocationPreference = "Bucuresti"
            };

            var result = service.GetNextEmptyFieldPrompt(profile);

            Assert.AreEqual("Your profile is 100% complete!", result);
        }
        [TestMethod]
        public void GetNextEmptyFieldPrompt_MissingField_ReturnsPromptWithPercentage()
        {
            var service = new CompletenessService();

            var profile = new UserProfile
            {
                FirstName = "Ana",
                LastName = "Pop"
            };

            var result = service.GetNextEmptyFieldPrompt(profile);
            Assert.IsTrue(result.StartsWith("Add your"));
            Assert.IsTrue(result.EndsWith("completeness!"));
        }
    }
}

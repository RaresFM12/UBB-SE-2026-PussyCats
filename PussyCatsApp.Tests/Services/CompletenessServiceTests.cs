using PussyCatsApp.Models;
using PussyCatsApp.Services;

namespace PussyCatsApp.Tests.Services
{
    [TestClass]
    public class CompletenessServiceTest
    {
        private CompletenessService service;
        
        [TestInitialize]
        public void Initialize()
        {
            service = new CompletenessService();
        }
        /// <summary>
        /// Verifies that CalculateCompleteness returns 0 for a null profile.
        /// </summary>
        [TestMethod]
        public void CalculateCompleteness_NullProfile_ReturnsZero()
        {
            //Act
            var result = service.CalculateCompleteness(null);
            //Assert
            Assert.AreEqual(0, result);
        }
        /// <summary>
        /// Verifies that CalculateCompleteness returns 100 for a fully populated UserProfile.
        /// </summary>
        [TestMethod]
        public void CalculateCompleteness_FullProfile_Returns100()
        {
            //Arrange
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
            //Act
            var result = service.CalculateCompleteness(profile);
            //Assert
            Assert.AreEqual(100, result);
        }
        /// <summary>
        /// Verifies GetNextEmptyFieldPrompt returns an empty string when given a null profile.
        /// </summary>
        [TestMethod]
        public void GetNextEmptyFieldPrompt_NullProfile_ReturnsEmpty()
        {
            //Act
            var result = service.GetNextEmptyFieldPrompt(null);
            //Assert
            Assert.AreEqual(string.Empty, result);
        }
        /// <summary>
        /// Verifies that GetNextEmptyFieldPrompt returns the completion message when all UserProfile fields are
        /// populated.
        /// </summary>
        [TestMethod]
        public void GetNextEmptyFieldPrompt_AllFilled_ReturnsCompleteMessage()
        {
            
            //Arrange
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
            //Act
            var result = service.GetNextEmptyFieldPrompt(profile);
            //Assert
            Assert.AreEqual("Your profile is 100% complete!", result);
        }
        /// <summary>
        /// Verifies GetNextEmptyFieldPrompt returns a prompt that indicates a missing profile field and includes a completeness percentage.
        /// </summary>
        [TestMethod]
        public void GetNextEmptyFieldPrompt_MissingField_ReturnsPromptWithPercentage()
        {
            //Arrange
            var profile = new UserProfile
            {
                FirstName = "Ana",
                LastName = "Pop"
            };
            //Act
            var result = service.GetNextEmptyFieldPrompt(profile);
            //Assert
            Assert.IsTrue(result.StartsWith("Add your"));
            Assert.IsTrue(result.EndsWith("14% completeness!"));
        }
    }
}

using PussyCatsApp.Models;
using PussyCatsApp.utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.Tests.ViewModels.UtilitiesTests
{
    [TestClass]
    public class ProfileFormValidatorTests
    {
        [TestMethod]
        public void IsValidEmail_ValidEmail_ReturnsTrue()
        {
            // Arrange
            string validEmail1 = "validemail@gmail.com";
            string validEmail2 = "also_a1_valid_email@gmail.com";
            Assert.AreEqual(true, ProfileFormValidator.IsValidEmail(validEmail1));
            Assert.AreEqual(true, ProfileFormValidator.IsValidEmail(validEmail2));
        }

        [TestMethod]
        public void IsValidEmail_InvalidEmail_ReturnsFalse()
        {
            string invalidEmail1 = "invalidemail";
            string invalidEmail2 = "invalidemail@";
            string invalidEmail3 = "invalidemail.com";
            string invalidEmail4 = "invalidemail@.com";
            Assert.AreEqual(false, ProfileFormValidator.IsValidEmail(invalidEmail1));
            Assert.AreEqual(false, ProfileFormValidator.IsValidEmail(invalidEmail2));
            Assert.AreEqual(false, ProfileFormValidator.IsValidEmail(invalidEmail3));
            Assert.AreEqual(false, ProfileFormValidator.IsValidEmail(invalidEmail4));
        }

        [TestMethod]
        public void ValidateForm_MissingFields_ReturnsErrorList()
        {
            // Arrange
            string firstName = string.Empty;
            string lastName = "Doe";
            double age = 25;
            string gender = string.Empty;
            string email = string.Empty;
            string phonePrefix = string.Empty;
            string phoneNumber = string.Empty;
            string country = string.Empty;
            string city = string.Empty;
            string university = string.Empty;
            int expectedGraduationYear = 0;
            List<WorkExperience> workExperiences = new List<WorkExperience>();
            List<string> errors = ProfileFormValidator.ValidateForm(firstName, lastName, age, gender, email, phonePrefix, phoneNumber, country, city, university, expectedGraduationYear, workExperiences);
            // phone prefix and phone number are combined into one error message so they count as one error instead of two
            int numberOfExpectedErrors = 8;

            Assert.AreEqual(numberOfExpectedErrors, errors.Count);
            Assert.IsTrue(errors.Contains("First Name"));
            Assert.IsTrue(errors.Contains("Expected Graduation Year"));
        }

        [TestMethod]
        public void ValidateForm_Validates_LastName_And_Age()
        {
            // Arrange
            string firstName = "John";
            string gender = "Male";
            string email = "validemail@gmail.com";
            string phonePrefix = "+40";
            string phoneNumber = "12345";
            string country = "Romania";
            string city = "Cluj-Napoca";
            string university = "UBB";
            int expectedGraduationYear = 2027;
            List<WorkExperience> workExperiences = new List<WorkExperience>();
            string lastName = string.Empty;

            double minimumAge = 16, maximumAge = 60;
            double ageUnderLimit = 10;
            List<string> errorsUnderage = ProfileFormValidator.ValidateForm(firstName, lastName, ageUnderLimit, gender, email, phonePrefix, phoneNumber, country, city, university, expectedGraduationYear, workExperiences);

            int numberOfExpectedErrors = 2; // Last Name and Age are the only fields that should be invalid

            Assert.AreEqual(numberOfExpectedErrors, errorsUnderage.Count);
            Assert.IsTrue(errorsUnderage.Contains("Last Name"));
            Assert.IsTrue(errorsUnderage.Contains($"Age (must be between {minimumAge}-{maximumAge})"));

            double ageOverLimit = 100;
            List<string> errorsOverAge = ProfileFormValidator.ValidateForm(firstName, lastName, ageOverLimit, gender, email, phonePrefix, phoneNumber, country, city, university, expectedGraduationYear, workExperiences);

            Assert.AreEqual(numberOfExpectedErrors, errorsOverAge.Count);
            Assert.IsTrue(errorsOverAge.Contains("Last Name"));
            Assert.IsTrue(errorsUnderage.Contains($"Age (must be between {minimumAge}-{maximumAge})"));
        }

        [TestMethod]
        public void Validator_Catches_Experiences_With_StartDate_After_EndDate()
        {
            // Arrange
            string firstName = "John";
            string lastName = "Doe";
            double age = 25;
            string gender = "Male";
            string email = "validemail@gmail.com";
            string phonePrefix = "+40";
            string phoneNumber = "12345";
            string country = "Romania";
            string city = "Cluj-Napoca";
            string university = "UBB";
            int expectedGraduationYear = 2027;
            List<WorkExperience> workExperiences = new List<WorkExperience>();

            WorkExperience workExperienceWrong = new WorkExperience
            {
                Company = "Company A",
                JobTitle = "Developer",
                StartDate = new DateTimeOffset(new DateTime(2020, 1, 1)),
                EndDate = new DateTimeOffset(new DateTime(2019, 1, 1)),
                Description = "Worked on various projects.",
                CurrentlyWorking = false
            };

            WorkExperience workExperienceRight = new WorkExperience
            {
                Company = "Company A",
                JobTitle = "Developer",
                StartDate = new DateTimeOffset(new DateTime(2019, 1, 1)),
                EndDate = new DateTimeOffset(new DateTime(2020, 1, 1)),
                Description = "Worked on various projects.",
                CurrentlyWorking = false
            };

            workExperiences.Add(workExperienceWrong);

            List<string> errorsWithWrongWorkExperience = ProfileFormValidator.ValidateForm(firstName, lastName, age, gender, email, phonePrefix, phoneNumber, country, city, university, expectedGraduationYear, workExperiences);

            Assert.AreEqual(1, errorsWithWrongWorkExperience.Count);
            Assert.IsTrue(errorsWithWrongWorkExperience.Contains($"Work Experience \"{workExperienceWrong.Company}\": End date is before start date"));

            workExperiences.Clear();
            workExperiences.Add(workExperienceRight);

            List<string> errorsWithRightWorkExperience = ProfileFormValidator.ValidateForm(firstName, lastName, age, gender, email, phonePrefix, phoneNumber, country, city, university, expectedGraduationYear, workExperiences);
            Assert.AreEqual(0, errorsWithRightWorkExperience.Count); /// Everything should be fine.
        }
    }
}

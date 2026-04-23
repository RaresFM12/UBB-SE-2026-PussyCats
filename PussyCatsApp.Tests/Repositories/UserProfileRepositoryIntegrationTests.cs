using Microsoft.VisualStudio.TestTools.UnitTesting;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.Tests.Infrastructure;
using System;
using Microsoft.Data.SqlClient;

namespace PussyCatsApp.Tests.Repositories
{
    [TestClass]
    public class UserProfileRepositoryIntegrationTests
    {
        private UserProfileRepository Repository;

        [TestInitialize]
        public void SetUp()
        {
            TestDatabaseHelper.ClearAllTables();
            Repository = new UserProfileRepository(TestDatabaseHelper.ConnectionString);
        }

        [TestMethod]
        public void GetProfileById_NonExistentUser_ExpectsNull()
        {
            int nonExistentUserId = 10876;

            UserProfile result = Repository.GetProfileById(nonExistentUserId);

            Assert.IsNull(result,"Expected null when querying a user that does not exist.");
        }

        [TestMethod]
        public void GetProfileById_ExistingUser_ExpectsNotNull()
        {
            int userId = TestDatabaseHelper.InsertUser();

            UserProfile result = Repository.GetProfileById(userId);

            Assert.IsNotNull(result, "Expected a non-null profile for an existing user.");
        }

        [TestMethod]
        public void GetProfileById_ExistingUser_ExpectsCorrectUserId()
        {
            int userId = TestDatabaseHelper.InsertUser();

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(userId, result.UserId, $"Expected userId {userId} but got {result.UserId}.");
        }

        [TestMethod]
        public void GetProfileById_ExistingUser_ExpectsCorrectFirstName()
        {
            int userId = TestDatabaseHelper.InsertUser(firstName: "Ioana");

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual("Ioana", result.FirstName, $"Expected first name 'Ioana' but got '{result.FirstName}'.");
        }

        [TestMethod]
        public void GetProfileById_ExistingUser_ExpectsCorrectLastName()
        {
            int userId = TestDatabaseHelper.InsertUser(lastName: "Gavrila");

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual("Gavrila", result.LastName, $"Expected last name 'Gavrila' but got '{result.LastName}'.");
        }

        [TestMethod]
        public void GetProfileById_ExistingUser_ExpectsCorrectEmail()
        {
            string email = "ioana@test.com";
            int userId = TestDatabaseHelper.InsertUser(email: email);

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(email, result.Email, $"Expected email '{email}' but got '{result.Email}'.");
        }

        [TestMethod]
        public void GetProfileById_ExistingUser_ExpectsCorrectAge()
        {
            int userId = TestDatabaseHelper.InsertUser(age: 22);

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(22, result.Age, $"Expected age 22 but got {result.Age}.");
        }

        [TestMethod]
        public void GetProfileById_UserWithFemaleGender_ExpectsCorrectGender()
        {
            int userId = TestDatabaseHelper.InsertUser(gender: "Female");

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual("Female", result.Gender, "Expected gender 'Female' to be returned as-is.");
        }


        [TestMethod]
        public void GetProfileById_UserWithCity_ExpectsCorrectCity()
        {
            int userId = TestDatabaseHelper.InsertUser(city: "Cluj-Napoca");

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual("Cluj-Napoca", result.City, $"Expected city 'Cluj-Napoca' but got '{result.City}'.");
        }

     
        [TestMethod]
        public void GetProfileById_UserWithMotivation_ExpectsCorrectMotivation()
        {
            int userId = TestDatabaseHelper.InsertUser(motivation: "I love coding.");

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual("I love coding.", result.Motivation, $"Expected motivation 'I love coding.' but got '{result.Motivation}'.");
        }

        [TestMethod]
        public void GetProfileById_UserWithGraduationYear_ExpectsCorrectYear()
        {
            int userId = TestDatabaseHelper.InsertUser(graduationYear: 2026);

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(2026, result.ExpectedGraduationYear, $"Expected graduation year 2026 but got {result.ExpectedGraduationYear}.");
        }


        [TestMethod]
        public void GetProfileById_UserHasNoCertificates_ExpectsEmptyCertificatesList()
        {
            int userId = TestDatabaseHelper.InsertUser();

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(0, result.RelevantCertificates.Count, "Expected empty certificates list for user with no documents.");
        }

        [TestMethod]
        public void GetProfileById_UserWithOneCertificate_ExpectsOneCertificate()
        {
            int userId = TestDatabaseHelper.InsertUser();
            TestDatabaseHelper.InsertDocument(userId, "AWS Certificate");

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(1, result.RelevantCertificates.Count, "Expected exactly one certificate.");
        }

        [TestMethod]
        public void GetProfileById_UserHasTwoCertificates_ExpectsTwoCertificates()
        {
            int userId = TestDatabaseHelper.InsertUser();
            TestDatabaseHelper.InsertDocument(userId, "AWS Certificate");
            TestDatabaseHelper.InsertDocument(userId, "Azure Certificate");

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(2, result.RelevantCertificates.Count, "Expected two certificates.");
        }



        [TestMethod]
        public void GetProfileById_UserWithWorkModePreference_ExpectsCorrectWorkMode()
        {
            int userId = TestDatabaseHelper.InsertUser();
            TestDatabaseHelper.InsertPreference(userId, "WorkMode", "Remote");

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual("Remote", result.WorkModePreference, "Expected work mode preference 'Remote'.");
        }


        [TestMethod]
        public void GetProfileById_UserWithNoPreferences_ExpectsEmptyJobRolesList()
        {
            int userId = TestDatabaseHelper.InsertUser();

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(0, result.PreferredJobRoles.Count,
                "Expected empty preferred job roles list.");
        }


        [TestMethod]
        public void UpdateAccountStatus_SetToActive_ExpectsAccountBeingActive()
        {
            int userId = TestDatabaseHelper.InsertUser(activeAccount: false);

            Repository.UpdateAccountStatus(userId, "ACTIVE");
            UserProfile result = Repository.GetProfileById(userId);

            Assert.IsTrue(result.ActiveAccount, "Expected ActiveAccount to be true after setting status to ACTIVE.");
        }


        [TestMethod]
        public void UpdateAccountStatus_NonExistentUser_ExpectsGracefulHandling()
        {
            int nonExistentUserId = 10876;

            try
            {
                Repository.UpdateAccountStatus(nonExistentUserId, "ACTIVE");
                Assert.IsTrue(true, "Expected no exception for non-existent user.");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected no exception but got: {ex.Message}");
            }
        }


        [TestMethod]
        public void UpdateProfilePicture_ValidPath_ExpectsNewProfilePicture()
        {
            int userId = TestDatabaseHelper.InsertUser();
            string newPath = "uploads/avatars/new_picture.jpg";

            Repository.UpdateProfilePicture(userId, newPath);
            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(newPath, result.ProfilePicture, $"Expected profile picture '{newPath}' but got '{result.ProfilePicture}'.");
        }

        [TestMethod]
        public void UpdateProfilePicture_NonExistentUser_ExpectsGracefulHandling()
        {
            int nonExistentUserId = 10876;

            try
            {
                Repository.UpdateProfilePicture(nonExistentUserId, "uploads/test.jpg");
                Assert.IsTrue(true, "Expected no exception for non-existent user.");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected no exception but got: {ex.Message}");
            }
        }


        [TestMethod]
        public void UpdateProfileLastModified_ValidTimestamp_ExpectsNewDate()
        {
            int userId = TestDatabaseHelper.InsertUser();
            DateTime newTimestamp = new DateTime(2026, 4, 21, 10, 30, 0);

            Repository.UpdateProfileLastModified(userId, newTimestamp);
            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(newTimestamp, result.LastUpdated, $"Expected LastUpdated '{newTimestamp}' but got '{result.LastUpdated}'.");
        }

        [TestMethod]
        public void UpdateProfileLastModified_NonExistentUser_ExpectsGracefulHandling()
        {
            int nonExistentUserId = 10876;
            try
            {
                Repository.UpdateProfileLastModified(nonExistentUserId, DateTime.Now);
                Assert.IsTrue(true, "Expected no exception for non-existent user.");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected no exception but got: {ex.Message}");
            }
        }


        [TestMethod]
        public void Save_NewUser_ExpectsNewProfileAdded()
        {
            UserProfile newProfile = new UserProfile
            {
                FirstName = "Amalia",
                LastName = "Antici",
                Gender = "Female",
                Age = 20,
                Email = "amalia@test.com",
                ActiveAccount = true
            };
            int newUserId = 1;

            Repository.Save(newUserId, newProfile);
            UserProfile result = Repository.GetProfileById(newUserId);

            Assert.IsNotNull(result, "Expected profile to be inserted and retrievable.");
        }

        [TestMethod]
        public void Save_ExistingUser_ExpectsNewFirstName()
        {
            int userId = TestDatabaseHelper.InsertUser(firstName: "OldName");
            UserProfile updatedProfile = new UserProfile
            {
                FirstName = "NewAmalia",
                LastName = "User",
                Gender = "Female",
                Age = 20,
                Email = "newAmalia@test.com",
                ActiveAccount = true
            };

            Repository.Save(userId, updatedProfile);
            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual("NewAmalia", result.FirstName, $"Expected updated first name 'NewName' but got '{result.FirstName}'.");
        }

        [TestMethod]
        public void Save_ExistingUser_ExpectsNewMotivation()
        {
            int userId = TestDatabaseHelper.InsertUser(motivation: "Old motivation");
            UserProfile updatedProfile = new UserProfile
            {
                FirstName = "Test",
                LastName = "User",
                Gender = "Female",
                Age = 20,
                Email = "test@test.com",
                Motivation = "New motivation",
                ActiveAccount = true
            };

            Repository.Save(userId, updatedProfile);
            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual("New motivation", result.Motivation, $"Expected motivation 'New motivation' but got '{result.Motivation}'.");
        }

        [TestMethod]
        public void GetProfileById_InvalidConnectionString_ExpectsConnectionException()
        {
            var badRepo = new UserProfileRepository("Server=NonExistentServer;Database=FakeDB;Trusted_Connection=True;Connect Timeout=1;");

            var result = badRepo.GetProfileById(1);

            Assert.IsNull(result, "Should return null when connection fails.");
        }

        [TestMethod]
        public void GetProfileById_MalformedSql_ExpectsSqlConnectionException()
        {

            var repo = new UserProfileRepository(TestDatabaseHelper.ConnectionString + ";Max Pool Size=1;");

            var result = repo.GetProfileById(-1); 
        }

        [TestMethod]
        public void LoadFormData_MalformedJson_ExpectsJsonExceptionHandled()
        {
            int userId = TestDatabaseHelper.InsertUser();
            using (var connection = new SqlConnection(TestDatabaseHelper.ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand("UPDATE Users SET formDataJson = '{ invalid: json }' WHERE userID = @id", connection);
                command.Parameters.AddWithValue("@id", userId);
                command.ExecuteNonQuery();
            }

            var profile = Repository.GetProfileById(userId);

            Assert.IsNotNull(profile.Skills);
        }

        [TestMethod]
        public void UpdateAccountStatus_InvalidId_ExpectsNoRowsAffected()
        {
            Repository.UpdateAccountStatus(99999, "ACTIVE");
        }

        [TestMethod]
        public void UpdateProfilePicture_ExpectsSqlConnectionError()
        {
            var badRepo = new UserProfileRepository("Server=localhost;Database=NonExistent;Connect Timeout=1;");

            badRepo.UpdateProfilePicture(1, "path/to/pic.png");
        }

        [TestMethod]
        public void GetProfileById_UserWithMaleGender_ReturnsGenderDisplayValue()
        {
            int userId = TestDatabaseHelper.InsertUser(gender: "Male");

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual("Male", result.Gender, "Expected gender 'Male' to be returned as-is.");
        }
    }
}
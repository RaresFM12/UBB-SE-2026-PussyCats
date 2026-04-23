using Microsoft.VisualStudio.TestTools.UnitTesting;
using PussyCatsApp.Repositories;
using PussyCatsApp.Models;
using PussyCatsApp.Tests.Infrastructure;
using System.Collections.Generic;

namespace PussyCatsApp.Tests.Repositories
{
    [TestClass]
    public class PreferenceRepositoryIntegrationTests
    {
        private PreferenceRepository Repository;

        [TestInitialize]
        public void SetUp()
        {
            TestDatabaseHelper.ClearAllTables();
            Repository = new PreferenceRepository(TestDatabaseHelper.ConnectionString);
        }


        [TestMethod]
        public void GetPreferencesByUserId_UserHasNoPreferences_ExpectsZeroPreferences()
        {
            int userId = TestDatabaseHelper.InsertUser();

            var result = Repository.GetPreferencesByUserId(userId);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetPreferencesByUserId_UserHasTwoPreferences_ExpectsTwoPreferences()
        {
            int userId = TestDatabaseHelper.InsertUser();
            TestDatabaseHelper.InsertPreference(userId, "Theme", "Dark");
            TestDatabaseHelper.InsertPreference(userId, "Notifications", "Enabled");

            var result = Repository.GetPreferencesByUserId(userId);

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void AddPreference_ValidPreference_SavedToDatabase()
        {
            int userId = TestDatabaseHelper.InsertUser();
            var newPref = new Preference
            {
                UserId = userId,
                PreferenceType = "Language",
                Value = "English"
            };

            Repository.AddPreference(newPref);

            var result = Repository.GetPreferencesByUserId(userId);
            Assert.AreEqual("Language", result[0].PreferenceType);
   
        }


        [TestMethod]
        public void RemovePreference_PreferenceExists_ExpectsOnlyThatPreferenceRemoved()
        {
            int userId = TestDatabaseHelper.InsertUser();
            int prefId1 = TestDatabaseHelper.InsertPreference(userId, "A", "1");
            int prefId2 = TestDatabaseHelper.InsertPreference(userId, "B", "2");

            Repository.RemovePreference(prefId1);

            var result = Repository.GetPreferencesByUserId(userId);
            Assert.AreEqual("B", result[0].PreferenceType, "The wrong preference was deleted.");
        }


        [TestMethod]
        public void DeleteAllByUserId_UserHasMultiplePreferences_ExpectsAllPreferencesClearedForThatUser()
        {
           
            int userId1 = TestDatabaseHelper.InsertUser(email: "user1@test.com");

            TestDatabaseHelper.InsertPreference(userId1, "Color", "Red");
            TestDatabaseHelper.InsertPreference(userId1, "Font", "Arial");

            Repository.DeleteAllByUserId(userId1);

            Assert.AreEqual(0, Repository.GetPreferencesByUserId(userId1).Count, "User 1 should have 0 prefs.");
        }
    }
}
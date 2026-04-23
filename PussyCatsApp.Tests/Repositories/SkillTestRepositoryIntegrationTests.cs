using PussyCatsApp.Repositories;
using PussyCatsApp.Tests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models;

namespace PussyCatsApp.Tests.Repositories
{
    [TestClass]
    public class SkillTestRepositoryIntegrationTests
    {
        private SkillTestRepository Repository;

        [TestInitialize]
        public void SetUp()
        {
            TestDatabaseHelper.ClearAllTables();
            Repository = new SkillTestRepository(TestDatabaseHelper.ConnectionString);
        }


        [TestMethod]
        public void Load_ExistingSkill_ExpectsCorrectSkillName()
        {
            int userId = TestDatabaseHelper.InsertUser();
            int skillId = TestDatabaseHelper.InsertSkill(userId, "C# Programming", 5, DateTime.Now);

            SkillTest resultFromDb = Repository.Load(skillId);

            Assert.AreEqual("C# Programming", resultFromDb.Name);
        }

        [TestMethod]
        public void Save_UpdateUserSkill_ExpectsNewSkillReturn()
        {
            int userId = TestDatabaseHelper.InsertUser();
            int skillId = TestDatabaseHelper.InsertSkill(userId, "SQL", 10, DateTime.Now);
            SkillTest newSkill = new SkillTest(skillId, userId, "SQL", 100, new DateOnly(2026, 4, 21));

            Repository.Save(skillId, newSkill);
            SkillTest updated = Repository.Load(skillId);

            Assert.AreEqual(100, updated.Score);
        }

        [TestMethod]
        public void GetSkillTestsByUserId_UserHasTwoTests_ExpectsCorrectCountOfTwo()
        {
            int userId = TestDatabaseHelper.InsertUser();
            TestDatabaseHelper.InsertSkill(userId, "First test", 60, DateTime.Now);
            TestDatabaseHelper.InsertSkill(userId, "Second test", 70, DateTime.Now);

            List<SkillTest> result = Repository.GetSkillTestsByUserId(userId);

            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void UpdateSkillTestScore_UserHasNewScore_ExpectsTheNewScore()
        {
            int userId=TestDatabaseHelper.InsertUser();
            int skillId=TestDatabaseHelper.InsertSkill(userId, "Java", 20, DateTime.Now);

            Repository.UpdateSkillTestScore(skillId, 80);

            SkillTest result=Repository.Load(skillId);

            Assert.AreEqual(80, result.Score);
        }


        [TestMethod]
        public void UpdateAchivedDate_UserHasNewDateForTest_ExpectsNewDate()
        {
            int userId=TestDatabaseHelper.InsertUser();
            int skillId = TestDatabaseHelper.InsertSkill(userId, "SQL", 65, DateTime.Now);
            DateOnly newDate=new DateOnly(2026, 4, 21);

            Repository.UpdateAchievedDate(skillId, newDate);

            SkillTest result=Repository.Load(skillId);

            Assert.AreEqual(newDate, result.AchievedDate);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Load_SkillDoesNotExist_ExpectsThrownException()
        {
            Repository.Load(1297);
        }

        [TestMethod]
        public void GetSkillTestsByUserId_InvalidServer_ExpectsEmptyList()
        {
            var badRepo = new SkillTestRepository("Server=FakeServer;Database=FakeDB;Connect Timeout=1;");

            List<SkillTest> result = badRepo.GetSkillTestsByUserId(1);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Load_MalformedConnectionString_ExpectsSpecificExceptionThrown()
        {
            SkillTestRepository badRepository = new SkillTestRepository("This is not a connection string");
            try
            {
                badRepository.Load(1);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("not found"));
            }
        }

        [TestMethod]
        public void UpdateSkillTestScore_InvalidServer_ExpectsNoCrash()
        {
            var badRepo = new SkillTestRepository("Server=FakeServer;Database=FakeDB;Connect Timeout=1;");
            badRepo.UpdateSkillTestScore(1, 100);
        }

        [TestMethod]
        public void UpdateAchievedDate_InvalidServer_ExpectsNoCrash()
        {
            var badRepo = new SkillTestRepository("Server=FakeServer;Database=FakeDB;Connect Timeout=1;");
            badRepo.UpdateAchievedDate(1, default);
        }

        [TestMethod]
        public void Save_InvalidServer_ExpectsNoCrash()
        {
            var badRepo = new SkillTestRepository("Server=FakeServer;Database=FakeDB;Connect Timeout=1;");
            badRepo.Save(1, new SkillTest(1, 1, "Test", 0, default));
        }

        [TestMethod]
        public void Load_DatabaseNotFound_ExpectsSqlError()
        {
            string sqlExceptionConnString = "Server=ASUS\\SQLEXPRESS;Database=DB_THAT_DOES_NOT_EXIST;Trusted_Connection=True;TrustServerCertificate=True;Connect Timeout=2;";
            var repoWithSqlError = new SkillTestRepository(sqlExceptionConnString);

            try
            {
                repoWithSqlError.Load(1);
                Assert.Fail("The method should have thrown an Exception after catching the SqlException.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("SkillTest with ID 1 not found.", ex.Message);
            }
        }

        [TestMethod]
        public void Save_MalformedConnectionString_ExpectsErrorBegingCatched()
        {

            string malformedString = "ThisIsNotAConnectionString";
            var repoWithGeneralError = new SkillTestRepository(malformedString);

            var dummyData = new SkillTest(1, 1, "Test", 0, default);

            repoWithGeneralError.Save(1, dummyData);

        }

        [TestMethod]
        public void UpdateSkillTestScore_MalformedConnectionString_ExpectsErrorBegingCacthed()
        {
           
            string malformed = "Invalid Format Here";
            var repo = new SkillTestRepository(malformed);

            repo.UpdateSkillTestScore(1, 100);

        }

        [TestMethod]
        public void UpdateAchievedDate_MalformedConnectionString_ExpectsErrorBegingCatched()
        {
            string malformed = "Bad;Format;No;Equal;Sign";
            var repo = new SkillTestRepository(malformed);
            DateOnly dummyDate = new DateOnly(2026, 1, 1);


            repo.UpdateAchievedDate(1, dummyDate);
        }

        [TestMethod]
        public void GetSkillTestsByUserId_EmptyConnectionString_CatchesGeneralException()
        {

            string emptyConnString = "";
            var repo = new SkillTestRepository(emptyConnString);

            List<SkillTest> result = repo.GetSkillTestsByUserId(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }


    }
}

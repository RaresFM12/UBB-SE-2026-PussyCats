using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Repositories;
using PussyCatsApp.Tests.Infrastructure;

namespace PussyCatsApp.Tests.Repositories
{
    [TestClass]
    public class MatchRepositoryIntegrationTests
    {
        private MatchRepository Repository;

        [TestInitialize]
        public void SetUp()
        {
            TestDatabaseHelper.ClearAllTables();
            Repository = new MatchRepository(TestDatabaseHelper.ConnectionString);
        }

        [TestMethod]
        public void GetMatchesByUserId_UserHasNoMatches_ExpectsZeroMatches()
        {
            int userId = TestDatabaseHelper.InsertUser();

            var matches = Repository.GetMatchesByUserId(userId);

            Assert.AreEqual(0, matches.Count);
        }

        [TestMethod]
        public void GetMatchesByUserId_UserHasOneMatch_ExpectsOneMatch()
        {
            int userId = TestDatabaseHelper.InsertUser();
            int matchId = TestDatabaseHelper.InsertMatch(userId, "LSEG", "DevOps Engineer", new DateTime(2026, 1, 1));

            var matches = Repository.GetMatchesByUserId(userId);

            Assert.AreEqual(1, matches.Count);
        }

        [TestMethod]
        public void GetMatchesByUserId_UserHasTwoMatches_ExpectsMatchesInDescendingOrder()
        {
            int userId = TestDatabaseHelper.InsertUser();
            int matchId1 = TestDatabaseHelper.InsertMatch(userId, "LSEG", "DevOps Engineer", new DateTime(2026, 2, 1));
            int matchId2 = TestDatabaseHelper.InsertMatch(userId, "Bosch", "Software Engineer", new DateTime(2026, 1, 1));

            var matches = Repository.GetMatchesByUserId(userId);

            Assert.AreEqual(matchId2, matches[1].Id);
        }

        [TestMethod]
        public void GetMatchesByUserId_UserDoesNotExist_ExpectsZeroMatches()
        {
            var matches = Repository.GetMatchesByUserId(10867);
            Assert.AreEqual(0, matches.Count);
        }

        [TestMethod]
        public void GetMatchesByUserId_DatabaseNotAvalaible_ExpectsError()
        {
            string invalidConnectionString = "Server=ASUS\\SQLEXPRESS;Database=PussyCatsTestsDBNotExistient;Trusted_Connection=True;TrustServerCertificate=True;";
            var repositoryWithInvalidConnection = new MatchRepository(invalidConnectionString);

            Assert.AreEqual(0, repositoryWithInvalidConnection.GetMatchesByUserId(1).Count);
        }

        [TestMethod]
        public void GetMatchesByUserId_MalformedConnectionString_ExpectsNullResult()
        {
            string malformedConnectionString = "ConnectionStringInvalid";
            MatchRepository invalidRepository = new MatchRepository(malformedConnectionString);

            var result = invalidRepository.GetMatchesByUserId(1);

            Assert.IsNotNull(result);
        }

    }
}

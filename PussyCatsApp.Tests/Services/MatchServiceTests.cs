using Moq;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;
using Match = PussyCatsApp.Models.Match;


namespace PussyCatsApp.Tests.Services;

[TestClass]
public class MatchServiceTest
{
    private Mock<IMatchRepository> mockRepo;
    private MatchService service;
    private List<Match> matches;

    [TestInitialize]
    public void Initialize()
    {
        mockRepo = new Mock<IMatchRepository>();
        service = new MatchService(mockRepo.Object);
        matches = new List<Match>
    {
        new Match { JobRole = "Backend", MatchDate = DateTime.Now.AddDays(-10) },
        new Match { JobRole = "Frontend", MatchDate = DateTime.Now.AddMonths(-3) },
        new Match { JobRole = "Backend", MatchDate = DateTime.Now.AddMonths(-8) }
    };
    }

    [TestMethod]
    public void GetStatistics_UserWithMatches_ReturnsCorrectCounts()
    {
        
        mockRepo.Setup(r => r.GetMatchesByUserId(1)).Returns(matches);
       

        var result = service.GetMatchStatistics(1);

       // Assert.AreEqual(3, result.TotalMatches);
       // Assert.AreEqual(1, result.MatchesLastMonth);
        //Assert.AreEqual(2, result.MatchesLastSixMonths);
       // Assert.AreEqual(3, result.MatchesLastYear);
        Assert.AreEqual(2, result.MatchesPerPosition["Backend"]);
       // Assert.AreEqual(1, result.MatchesPerPosition["Frontend"]);
    }

   /* [TestMethod]
    public void GetStatistics_UserWithNoMatches_ReturnsZeroCounts()
    {
        
        mockRepo.Setup(r => r.GetMatchesByUserId(1)).Returns(new List<Match>());
       

        var result = service.GetMatchStatistics(1);

        Assert.AreEqual(0, result.TotalMatches);
        Assert.AreEqual(0, result.MatchesLastMonth);
        Assert.AreEqual(0, result.MatchesLastSixMonths);
        Assert.AreEqual(0, result.MatchesLastYear);
        Assert.AreEqual(0, result.MatchesPerPosition.Count);
    }*/

   
[TestMethod]
public void GetStatistics_UserWithMatches_ReturnsTotalCount()
{
   mockRepo.Setup(r => r.GetMatchesByUserId(1)).Returns(matches);
   var result = service.GetMatchStatistics(1);
   Assert.AreEqual(3, result.TotalMatches);
}
[TestMethod]
public void GetStatistics_UserWithMatches_ReturnsCorrectLastMonthCount()
{
   mockRepo.Setup(r => r.GetMatchesByUserId(1)).Returns(matches);
   var result = service.GetMatchStatistics(1);
   Assert.AreEqual(1, result.MatchesLastMonth);
}
[TestMethod]
public void GetStatistics_UserWithMatches_ReturnsCorrectSixMonthCount()
{
   mockRepo.Setup(r => r.GetMatchesByUserId(1)).Returns(matches);
   var result = service.GetMatchStatistics(1);
   Assert.AreEqual(2, result.MatchesLastSixMonths);
}
    [TestMethod]
    public void LastYearMatches_ShouldBeCorrect()
    {
        mockRepo.Setup(r => r.GetMatchesByUserId(1)).Returns(matches);

        var result = service.GetMatchStatistics(1);

        Assert.AreEqual(3, result.MatchesLastYear);
    }
    [TestMethod]
    public void GetStatistics_UserWithMatches_ReturnsFrontendCount()
    {
        mockRepo.Setup(r => r.GetMatchesByUserId(1)).Returns(matches);

        var result = service.GetMatchStatistics(1);

        Assert.AreEqual(1, result.MatchesPerPosition["Frontend"]);
    }

}


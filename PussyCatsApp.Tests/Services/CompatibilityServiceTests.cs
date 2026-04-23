using Moq;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;

[TestClass]
public class CompatibilityServiceTest
{
    private Mock<IUserSkillRepository> mockUserSkillRepo;
    private Mock<ISkillGroupRepository> mockSkillGroupRepo;
    private CompatibilityService service;

    [TestInitialize]
    public void Setup()
    {
        mockUserSkillRepo = new Mock<IUserSkillRepository>();
        mockSkillGroupRepo = new Mock<ISkillGroupRepository>();
        service = new CompatibilityService(mockUserSkillRepo.Object, mockSkillGroupRepo.Object);
    }

    [TestMethod]
    public void CalculateForRole_NoSkills_ReturnsZeroScore()
    {
        mockUserSkillRepo.Setup(r => r.GetVerifiedSkillsByUserId(1)).Returns(new List<UserSkill>());
        mockUserSkillRepo.Setup(r => r.GetParsedCvByUserId(1)).Returns(string.Empty);
        mockSkillGroupRepo.Setup(r => r.GetByRole(JobRole.FrontendDeveloper)).Returns(new List<SkillGroup>
        {
            new SkillGroup { GroupName = "G1", Skills = new List<string> { "React" }, Weight = 10 }
        });

        var result = service.CalculateForRole(1, JobRole.FrontendDeveloper);

        Assert.AreEqual(0, result.MatchScore);
    }

    [TestMethod]
    public void CalculateForRole_AllSkillsVerified_ReturnsHighScore()
    {
        mockUserSkillRepo.Setup(r => r.GetVerifiedSkillsByUserId(1)).Returns(new List<UserSkill>
        {
            new UserSkill { SkillName = "React", IsVerified = true, Score = 95 }
        });
        mockUserSkillRepo.Setup(r => r.GetParsedCvByUserId(1)).Returns(string.Empty);
        mockSkillGroupRepo.Setup(r => r.GetByRole(JobRole.FrontendDeveloper)).Returns(new List<SkillGroup>
        {
            new SkillGroup { GroupName = "G1", Skills = new List<string> { "React" }, Weight = 10 }
        });

        var result = service.CalculateForRole(1, JobRole.FrontendDeveloper);

        Assert.IsTrue(result.MatchScore > 50);
    }

    //CalculateForRole — no groups => totalWeight 0 => matchScore -1
    [TestMethod]
    public void CalculateForRole_NoGroups_ReturnsInvalidScore()
    {
        mockUserSkillRepo.Setup(r => r.GetVerifiedSkillsByUserId(1)).Returns(new List<UserSkill>());
        mockUserSkillRepo.Setup(r => r.GetParsedCvByUserId(1)).Returns(string.Empty);
        mockSkillGroupRepo.Setup(r => r.GetByRole(JobRole.FrontendDeveloper)).Returns(new List<SkillGroup>());

        var result = service.CalculateForRole(1, JobRole.FrontendDeveloper);

        Assert.AreEqual(-1, result.MatchScore);
    }

    //CalculateForRole — skills din CV (parsedCv cu 3 linii)
    [TestMethod]
    public void CalculateForRole_WithCvSkills_ReturnsNonZeroScore()
    {
        mockUserSkillRepo.Setup(r => r.GetVerifiedSkillsByUserId(1)).Returns(new List<UserSkill>());
        mockUserSkillRepo.Setup(r => r.GetParsedCvByUserId(1)).Returns("line1\nline2\nReact");
        mockSkillGroupRepo.Setup(r => r.GetByRole(JobRole.FrontendDeveloper)).Returns(new List<SkillGroup>
        {
            new SkillGroup { GroupName = "G1", Skills = new List<string> { "React" }, Weight = 10 }
        });

        var result = service.CalculateForRole(1, JobRole.FrontendDeveloper);

        Assert.IsTrue(result.MatchScore > 0);
    }

    //CalculateForRole- cv cu mai putin de 3 linii
    [TestMethod]
    public void CalculateForRole_CvLessThan3Lines_ReturnsZeroScore()
    {
        mockUserSkillRepo.Setup(r => r.GetVerifiedSkillsByUserId(1)).Returns(new List<UserSkill>());
        mockUserSkillRepo.Setup(r => r.GetParsedCvByUserId(1)).Returns("line1\nline2");
        mockSkillGroupRepo.Setup(r => r.GetByRole(JobRole.FrontendDeveloper)).Returns(new List<SkillGroup>
        {
            new SkillGroup { GroupName = "G1", Skills = new List<string> { "React" }, Weight = 10 }
        });

        var result = service.CalculateForRole(1, JobRole.FrontendDeveloper);

        Assert.AreEqual(0, result.MatchScore);
    }

    [TestMethod]
    public void CalculateForRole_CvThirdLineEmpty_ReturnsZeroScore()
    {
        mockUserSkillRepo.Setup(r => r.GetVerifiedSkillsByUserId(1)).Returns(new List<UserSkill>());
        mockUserSkillRepo.Setup(r => r.GetParsedCvByUserId(1)).Returns("line1\nline2\n   ");
        mockSkillGroupRepo.Setup(r => r.GetByRole(JobRole.FrontendDeveloper)).Returns(new List<SkillGroup>
        {
            new SkillGroup { GroupName = "G1", Skills = new List<string> { "React" }, Weight = 10 }
        });

        var result = service.CalculateForRole(1, JobRole.FrontendDeveloper);

        Assert.AreEqual(0, result.MatchScore);
    }

    // skill verified, groupScore > 0.5 so IdentifyGaps skip
    [TestMethod]
    public void CalculateForRole_HighGroupScore_ReturnsEmptySuggestions()
    {
        mockUserSkillRepo.Setup(r => r.GetVerifiedSkillsByUserId(1)).Returns(new List<UserSkill>
        {
            new UserSkill { SkillName = "React", IsVerified = true, Score = 90 }
        });
        mockUserSkillRepo.Setup(r => r.GetParsedCvByUserId(1)).Returns(string.Empty);
        mockSkillGroupRepo.Setup(r => r.GetByRole(JobRole.FrontendDeveloper)).Returns(new List<SkillGroup>
        {
            new SkillGroup { GroupName = "G1", Skills = new List<string> { "React" }, Weight = 10 }
        });

        var result = service.CalculateForRole(1, JobRole.FrontendDeveloper);

        Assert.AreEqual(0, result.Suggestions.Count);
    }

    //CalculateForRole-mai mult de 3 suggestions - GetRange(0, 3)
    [TestMethod]
    public void CalculateForRole_MoreThan3Gaps_Returns3Suggestions()
    {
        mockUserSkillRepo.Setup(r => r.GetVerifiedSkillsByUserId(1)).Returns(new List<UserSkill>());
        mockUserSkillRepo.Setup(r => r.GetParsedCvByUserId(1)).Returns(string.Empty);
        mockSkillGroupRepo.Setup(r => r.GetByRole(JobRole.FrontendDeveloper)).Returns(new List<SkillGroup>
        {
            new SkillGroup { GroupName = "G1", Skills = new List<string> { "Skill1" }, Weight = 10 },
            new SkillGroup { GroupName = "G2", Skills = new List<string> { "Skill2" }, Weight = 9 },
            new SkillGroup { GroupName = "G3", Skills = new List<string> { "Skill3" }, Weight = 8 },
            new SkillGroup { GroupName = "G4", Skills = new List<string> { "Skill4" }, Weight = 7 }
        });

        var result = service.CalculateForRole(1, JobRole.FrontendDeveloper);

        Assert.AreEqual(3, result.Suggestions.Count);
    }

    [TestMethod]
    public void CalculateAll_ReturnsResultForEachRole()
    {
        mockUserSkillRepo.Setup(r => r.GetVerifiedSkillsByUserId(1)).Returns(new List<UserSkill>());
        mockUserSkillRepo.Setup(r => r.GetParsedCvByUserId(1)).Returns(string.Empty);
        mockSkillGroupRepo.Setup(r => r.GetByRole(It.IsAny<JobRole>())).Returns(new List<SkillGroup>());

        var results = service.CalculateAll(1);

        Assert.AreEqual(Enum.GetValues(typeof(JobRole)).Length, results.Count);
    }
}
/*using Moq;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;

namespace PussyCatsApp.Tests.Services;

[TestClass]
public class CompatibilityServiceTest
{
    private Mock<IUserSkillRepository> mockUserRepo;
    private Mock<ISkillGroupRepository> mockGroupRepo;
    private CompatibilityService service;
    
    [TestInitialize]
    public void Initialize()
    {
        mockUserRepo = new Mock<IUserSkillRepository>();
        mockGroupRepo = new Mock<ISkillGroupRepository>();
        service = new CompatibilityService(mockUserRepo.Object, mockGroupRepo.Object);
    }

    [TestMethod]
    public void CalculateForRole_NoSkills_ReturnsZeroScore()
    {
       
        mockUserRepo.Setup(r => r.GetVerifiedSkillsByUserId(1))
                    .Returns(new List<UserSkill>());

        mockUserRepo.Setup(r => r.GetParsedCvByUserId(1))
                    .Returns("");

        var groups = new List<SkillGroup>{
            new SkillGroup{ GroupName = "Backend", Weight = 1, Skills = new List<string> { "C#" } }
        };

        mockGroupRepo.Setup(r => r.GetByRole(JobRole.BackendDeveloper)).Returns(groups);

        var result = service.CalculateForRole(1, JobRole.BackendDeveloper);

        Assert.AreEqual(0, result.MatchScore);
    }

    [TestMethod]
    public void CalculateForRole_AllSkillsVerified_ReturnsHighScore()
    {

        var userSkills = new List<UserSkill> { 
            new UserSkill { SkillName = "C#", IsVerified = true, Score = 100 } 
        };

        mockUserRepo.Setup(r => r.GetVerifiedSkillsByUserId(1))
                    .Returns(userSkills);

        mockUserRepo.Setup(r => r.GetParsedCvByUserId(1))
                    .Returns("");

        var groups = new List<SkillGroup>
    {
        new SkillGroup
        {
            GroupName = "Backend",
            Weight = 1,
            Skills = new List<string> { "C#" }
        }
    };

        mockGroupRepo.Setup(r => r.GetByRole(JobRole.BackendDeveloper)).Returns(groups);

        var result = service.CalculateForRole(1, JobRole.BackendDeveloper);

        Assert.AreEqual(100, result.MatchScore);
    }
    
    [TestMethod]
    public void CalculateForRole_NoGroups_ReturnsMinusOne()
    { 

        mockUserRepo.Setup(r => r.GetVerifiedSkillsByUserId(1)).Returns(new List<UserSkill>());
        mockUserRepo.Setup(r => r.GetParsedCvByUserId(1)).Returns(string.Empty);
        mockGroupRepo.Setup(r => r.GetByRole(JobRole.FrontendDeveloper)).Returns(new List<SkillGroup>());

        var result = service.CalculateForRole(1, JobRole.FrontendDeveloper);

        Assert.AreEqual(-1, result.MatchScore);
    }

    [TestMethod]
    public void CalculateForRole_WithCvSkills_ReturnsPartialScore()
    {

        mockUserRepo.Setup(r => r.GetVerifiedSkillsByUserId(1)).Returns(new List<UserSkill>());
        mockUserRepo.Setup(r => r.GetParsedCvByUserId(1)).Returns("a\nb\nC#");

        var groups = new List<SkillGroup>
    {
        new SkillGroup { GroupName = "Backend", Weight = 1, Skills = new List<string> { "C#" } }
    };

        mockGroupRepo.Setup(r => r.GetByRole(JobRole.BackendDeveloper)).Returns(groups);

        var result = service.CalculateForRole(1, JobRole.BackendDeveloper);

        Assert.AreEqual(50, result.MatchScore); // 0.5 * 100
    }
    

    [TestMethod]
    public void CalculateForRole_MissingSkills_ReturnsSuggestions()
    {
       
        mockUserRepo.Setup(r => r.GetVerifiedSkillsByUserId(1)).Returns(new List<UserSkill>());
        mockUserRepo.Setup(r => r.GetParsedCvByUserId(1)).Returns("");

        var groups = new List<SkillGroup>
    {
        new SkillGroup { GroupName = "Backend", Weight = 1, Skills = new List<string> { "C#" } }
    };

        mockGroupRepo.Setup(r => r.GetByRole(JobRole.BackendDeveloper)).Returns(groups);

        var result = service.CalculateForRole(1, JobRole.BackendDeveloper);

        Assert.AreEqual(1, result.Suggestions.Count);
        Assert.AreEqual("C#", result.Suggestions[0].SkillName);
    }

    [TestMethod]
    public void CalculateForRole_MoreThan3Suggestions_ReturnsOnly3()
    {
       
        mockUserRepo.Setup(r => r.GetVerifiedSkillsByUserId(1)).Returns(new List<UserSkill>());
        mockUserRepo.Setup(r => r.GetParsedCvByUserId(1)).Returns("");

        var groups = new List<SkillGroup>
    {
        new SkillGroup { GroupName = "G1", Skills = new List<string> { "A" }, Weight = 1 },
        new SkillGroup { GroupName = "G2", Skills = new List<string> { "B" }, Weight = 1 },
        new SkillGroup { GroupName = "G3", Skills = new List<string> { "C" }, Weight = 1 },
        new SkillGroup { GroupName = "G4", Skills = new List<string> { "D" }, Weight = 1 }
    };

        mockGroupRepo.Setup(r => r.GetByRole(JobRole.BackendDeveloper)).Returns(groups);

       

        var result = service.CalculateForRole(1, JobRole.BackendDeveloper);

        Assert.AreEqual(3, result.Suggestions.Count);
    }
    [TestMethod]
    public void CalculateAll_ReturnsResultForEachRole()
    {
       
        mockUserRepo.Setup(r => r.GetVerifiedSkillsByUserId(1)).Returns(new List<UserSkill>());
        mockUserRepo.Setup(r => r.GetParsedCvByUserId(1)).Returns("");
        mockGroupRepo.Setup(r => r.GetByRole(It.IsAny<JobRole>())).Returns(new List<SkillGroup>());

        var results = service.CalculateAll(1);

        Assert.AreEqual(Enum.GetValues(typeof(JobRole)).Length, results.Count);
    }
}
*/
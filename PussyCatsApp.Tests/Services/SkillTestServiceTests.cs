using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using PussyCatsApp.models;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.services;

namespace PussyCatsApp.Tests.Services
{
    [TestClass]
    public class SkillTestServiceTests
    {

        private Mock<ISkillTestRepository> mockRepo;
        private SkillTestService service;

        [TestInitialize]
        public void Initialize()
        {
            mockRepo = new Mock<ISkillTestRepository>();
            service = new SkillTestService(mockRepo.Object);
        }

        [TestMethod]
        public void CanRetakeTest_ValidSkillId_ReturnsTrue()
        {
            var skill = new SkillTest(1, 10, "React");
            skill.AchievedDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(-4));
           
            mockRepo.Setup(r => r.Load(1)).Returns(skill);
            
            var result = service.CanRetakeTest(1);

            Assert.IsTrue(result);
        }


        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CanRetakeTest_InvalidSkillId_ThrowsException()
        {
            mockRepo.Setup(r => r.Load(1)).Returns((SkillTest)null);
            service.CanRetakeTest(1);
        }


        [TestMethod]
        public void SubmitRetake_EligibleTest_ReturnsNewBadge()
        {
            var skill = new SkillTest(1, 10, "React");
            skill.AchievedDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(-4));

            mockRepo.Setup(r => r.Load(1)).Returns(skill);

            var result = service.SubmitRetake(1, 95);

            mockRepo.Verify(r => r.UpdateSkillTestScore(1, 95), Times.Once);
            mockRepo.Verify(r => r.UpdateAchievedDate(1, It.IsAny<DateOnly>()), Times.Once);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SubmitRetake_NotEligible_ThrowsException()
        {
            var skill = new SkillTest(1, 10, "React");
            skill.AchievedDate = DateOnly.FromDateTime(DateTime.Now);
           
            mockRepo.Setup(r => r.Load(1)).Returns(skill);

            service.SubmitRetake(1, 50);
        }

    }
}

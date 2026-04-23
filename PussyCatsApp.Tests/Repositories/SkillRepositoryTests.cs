using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;

namespace PussyCatsApp.Tests.Repositories
{
    [TestClass]
    public class SkillRepositoryTests
    {
        private SkillRepository Repository;

        [TestInitialize]
        public void SetUp()
        {
            Repository=new SkillRepository();
        }

        [TestMethod]
        public void Load_SkillExists_ExpectsSkillReturned()
        {
            Skill skill = new Skill();
            skill.SkillId = 10;
            Repository.AddSkill(skill);
                
            Skill result = Repository.Load(10);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Load_SkillDoesNotExist_ExpectsNull()
        {
            Skill result = Repository.Load(999);

            Assert.IsNull(result);
        }


        [TestMethod]
        public void Save_SkillExists_ExpectsUpdatedExistingData()
        {
            Skill initial = new Skill();
            initial.SkillId = 1;
            initial.Name = "Old Name";
            Repository.AddSkill(initial);

            Skill newData = new Skill();
            newData.Name = "New Name";
            newData.Score = 85.0;

            Repository.Save(1, newData);

            Skill updated = Repository.Load(1);
            Assert.AreEqual("New Name", updated.Name);
        }

        [TestMethod]
        public void Save_SkillDoesNotExist_ExcpectsNewSkillAdded()
        {
            Skill newData = new Skill();
            newData.Name = "Brand New";

            Repository.Save(50, newData);

            Skill result = Repository.Load(50);
            Assert.AreEqual("Brand New", result.Name);
        }

        [TestMethod]
        public void AddSkill_FirstSkill_ExpectsIdOne()
        {
            Skill skill = new Skill();
            skill.SkillId = 0; 

            Repository.AddSkill(skill);

            Assert.AreEqual(1, skill.SkillId);
        }

        [TestMethod]
        public void AddSkill_ExistingSkills_ExpectsMaximumIdPlusOne()
        {
            Skill first = new Skill();
            first.SkillId = 10;
            Repository.AddSkill(first);

            Skill second = new Skill();
            second.SkillId = 0; 

            Repository.AddSkill(second);

            Assert.AreEqual(11, second.SkillId);
        }


        [TestMethod]
        public void GetSkillsByUserId_UserHasSkills_ReturnsCorrectList()
        {
            Skill s1 = new Skill(); s1.UserId = 1; Repository.AddSkill(s1);
            Skill s2 = new Skill(); s2.UserId = 1; Repository.AddSkill(s2);
            Skill s3 = new Skill(); s3.UserId = 2; Repository.AddSkill(s3);

            List<Skill> results = Repository.GetSkillsByUserId(1);

            Assert.AreEqual(2, results.Count);
        }


        [TestMethod]
        public void RemoveSkill_SkillExists_RemovesFromList()
        {
            Skill skill = new Skill();
            skill.SkillId = 5;
            Repository.AddSkill(skill);

            Repository.RemoveSkill(5);

            Assert.IsNull(Repository.Load(5));
        }

        [TestMethod]
        public void RemoveSkill_DoesNotExist_ExpectsExceptionHandled()
        {
            Repository.RemoveSkill(999);
        }


        [TestMethod]
        public void UpdateSkillScore_SkillExists_ExpectsNewScore()
        {
            Skill skill = new Skill();
            skill.SkillId = 1;
            skill.Score = 10.0;
            Repository.AddSkill(skill);

            Repository.UpdateSkillScore(1, 95.5);

            Assert.AreEqual(95.5, Repository.Load(1).Score);
        }

        [TestMethod]
        public void UpdateSkillScore_DoesNotExist_ExcpectsExceptionHandled()
        {
            Repository.UpdateSkillScore(999, 100.0);
        }
    }
}


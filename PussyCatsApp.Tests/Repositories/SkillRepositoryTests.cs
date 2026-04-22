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
        public void Load_SkillExists_ReturnsSkill()
        {
            Skill skill = new Skill();
            skill.SkillId = 10;
            Repository.AddSkill(skill);
                
            Skill result = Repository.Load(10);

            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.SkillId);
        }

        [TestMethod]
        public void Load_SkillDoesNotExist_ReturnsNull()
        {
            Skill result = Repository.Load(999);

            Assert.IsNull(result);
        }


        [TestMethod]
        public void Save_SkillExists_UpdatesExistingData()
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
            Assert.AreEqual(85.0, updated.Score);
        }

        [TestMethod]
        public void Save_SkillDoesNotExist_AddsNewSkill()
        {
            Skill newData = new Skill();
            newData.Name = "Brand New";

            Repository.Save(50, newData);

            Skill result = Repository.Load(50);
            Assert.IsNotNull(result);
            Assert.AreEqual("Brand New", result.Name);
        }

        // --- AddSkill Tests (Auto-ID Logic Coverage) ---

        [TestMethod]
        public void AddSkill_FirstSkill_GetsIdOne()
        {
            Skill skill = new Skill();
            skill.SkillId = 0; // Trigger auto-id

            Repository.AddSkill(skill);

            Assert.AreEqual(1, skill.SkillId);
        }

        [TestMethod]
        public void AddSkill_ExistingSkills_GetsMaxPlusOne()
        {
            Skill first = new Skill();
            first.SkillId = 10;
            Repository.AddSkill(first);

            Skill second = new Skill();
            second.SkillId = 0; // Trigger auto-id

            Repository.AddSkill(second);

            Assert.AreEqual(11, second.SkillId);
        }

        // --- GetSkillsByUserId Tests ---

        [TestMethod]
        public void GetSkillsByUserId_UserHasSkills_ReturnsCorrectList()
        {
            Skill s1 = new Skill(); s1.UserId = 1; Repository.AddSkill(s1);
            Skill s2 = new Skill(); s2.UserId = 1; Repository.AddSkill(s2);
            Skill s3 = new Skill(); s3.UserId = 2; Repository.AddSkill(s3);

            List<Skill> results = Repository.GetSkillsByUserId(1);

            Assert.AreEqual(2, results.Count);
        }

        // --- RemoveSkill Tests ---

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
        public void RemoveSkill_DoesNotExist_DoesNothing()
        {
            // This covers the "if (skill != null)" branch when it's false
            Repository.RemoveSkill(999);
            // No exception thrown means success
        }

        // --- UpdateSkillScore Tests ---

        [TestMethod]
        public void UpdateSkillScore_SkillExists_UpdatesScore()
        {
            Skill skill = new Skill();
            skill.SkillId = 1;
            skill.Score = 10.0;
            Repository.AddSkill(skill);

            Repository.UpdateSkillScore(1, 95.5);

            Assert.AreEqual(95.5, Repository.Load(1).Score);
        }

        [TestMethod]
        public void UpdateSkillScore_DoesNotExist_DoesNothing()
        {
            // Covers the "if (skill != null)" branch being false
            Repository.UpdateSkillScore(999, 100.0);
        }
    }
}


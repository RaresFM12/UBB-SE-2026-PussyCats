using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Services;
using PussyCatsApp.ViewModels;

namespace PussyCatsApp.Tests.ViewModels
{
    [TestClass]
    public class ProfileFormViewModelTests
    {
        private Mock<IUserProfileService> mockProfileService;
        private Mock<ICVParsingService> mockCVParsingService;
        private ProfileFormViewModel viewModel;

        [TestInitialize]
        public void Setup()
        {
            mockProfileService = new Mock<IUserProfileService>();
            mockCVParsingService = new Mock<ICVParsingService>();
            viewModel = new ProfileFormViewModel(mockProfileService.Object, mockCVParsingService.Object);
        }

        [TestMethod]
        public void TestAddSkillAddsSkillToList()
        {
            string skill = "C#";
            viewModel.AddSkill(skill);
            Assert.AreEqual(1, viewModel.Skills.Count);
            Assert.IsTrue(viewModel.Skills.Contains(skill));
        }

        [TestMethod]
        public void TestAddSkillDoesNotAddEmptySkill()
        {
            string skill = string.Empty;
            viewModel.AddSkill(skill);
            Assert.AreEqual(0, viewModel.Skills.Count);
        }

        [TestMethod]
        public void TestAddDuplicateSkillDoesNotAddToList()
        {
            string skill = "C#";
            viewModel.AddSkill(skill);
            viewModel.AddSkill(skill);
            Assert.AreEqual(1, viewModel.Skills.Count);
        }

        [TestMethod]
        public void TestAddDuplicateSkillShowsInfoBar()
        {
            string skill = "C#";
            string duplicateStringInfoBarMessage = "This skill has already been added.";

            viewModel.AddSkill(skill);
            viewModel.AddSkill(skill);

            Assert.IsTrue(viewModel.IsInfoBarOpen);
            Assert.AreEqual(duplicateStringInfoBarMessage, viewModel.InfoBarMessage);
        }

        [TestMethod]
        public void TestAddSkillCannotAddMoreThanMaximumNumberAllowed()
        {
            int maximumNumberOfSkillsAllowed = 30;
            string skill = "Skill";
            for (int skillIndex = 0; skillIndex < maximumNumberOfSkillsAllowed; skillIndex++)
            {
                viewModel.AddSkill(skill + skillIndex);
            }

            viewModel.AddSkill("ExtraSkill");
            Assert.AreEqual(maximumNumberOfSkillsAllowed, viewModel.Skills.Count);
        }

        [TestMethod]
        public void TestAddSkillShowsInfoBarWhenMaximumNumberOfSkillsAllowedIsReached()
        {
            int maximumNumberOfSkillsAllowed = 30;
            string skill = "Skill";
            string maximumNumberOfSkillsInfoBarMessage = $"Maximum of {maximumNumberOfSkillsAllowed} skills allowed.";
            for (int skillIndex = 0; skillIndex < maximumNumberOfSkillsAllowed; skillIndex++)
            {
                viewModel.AddSkill(skill + skillIndex);
            }
            viewModel.AddSkill("ExtraSkill");
            Assert.IsTrue(viewModel.IsInfoBarOpen);
            Assert.AreEqual(maximumNumberOfSkillsInfoBarMessage, viewModel.InfoBarMessage);
        }

        [TestMethod]
        public void TestAddSkillDoesNotAddSkillWithTooLongName()
        {
            int maximumSkillNameLength = 60;
            string longSkillName = new string('a', maximumSkillNameLength + 1);

            viewModel.AddSkill(longSkillName);
            Assert.AreEqual(0, viewModel.Skills.Count);
        }

        [TestMethod]
        public void TestAddSkillShowsInfoBarWhenSkillNameExceedsMaximumLength()
        {
            int maximumSkillNameLength = 60;
            string longSkillName = new string('a', maximumSkillNameLength + 1);

            string skillNameTooLongInfoBarMessage = $"Skill name must be less than {maximumSkillNameLength} characters.";
            viewModel.AddSkill(longSkillName);

            Assert.IsTrue(viewModel.IsInfoBarOpen);
            Assert.AreEqual(skillNameTooLongInfoBarMessage, viewModel.InfoBarMessage);
        }

        [TestMethod]
        public void TestRemoveSkillRemovesSkillFromList()
        {
            string skill = "C#";
            viewModel.AddSkill(skill);
            viewModel.RemoveSkill(skill);
            Assert.AreEqual(0, viewModel.Skills.Count);
        }

        [TestMethod]
        public void TestAddWorkExperienceAddsExperienceToList()
        {
            viewModel.AddWorkExperience();
            Assert.AreEqual(1, viewModel.WorkExperiences.Count);
        }

        [TestMethod]
        public void TestAddWorkExperienceFailsWhenMaximumNumberOfWorkExperiencesIsReached()
        {
            int maximumNumberOfWorkExperiencesAllowed = 10;
            for (int experienceIndex = 0; experienceIndex < maximumNumberOfWorkExperiencesAllowed; experienceIndex++)
            {
                viewModel.AddWorkExperience();
            }

            viewModel.AddWorkExperience();

            Assert.AreEqual(maximumNumberOfWorkExperiencesAllowed, viewModel.WorkExperiences.Count);
        }

        [TestMethod]
        public void TestAddWorkExperienceShowInfoBarWhenMaximumNumberOfWorkExperiencesIsReached()
        {
            int maximumNumberOfWorkExperiencesAllowed = 10;
            for (int experienceIndex = 0; experienceIndex < maximumNumberOfWorkExperiencesAllowed; experienceIndex++)
            {
                viewModel.AddWorkExperience();
            }

            viewModel.AddWorkExperience();

            Assert.AreEqual(maximumNumberOfWorkExperiencesAllowed, viewModel.WorkExperiences.Count);
        }

    }
}

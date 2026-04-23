using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Services;
using PussyCatsApp.ViewModels;
using PussyCatsApp.Models;

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

            Assert.IsTrue(viewModel.IsInfoBarOpen);
            Assert.AreEqual($"Maximum of {maximumNumberOfWorkExperiencesAllowed} work experiences allowed.", viewModel.InfoBarMessage);
        }

        [TestMethod]
        public void TestRemoveWorkExperienceRemovesExperienceFromList()
        {
            viewModel.AddWorkExperience();
            var experience = viewModel.WorkExperiences.First();

            viewModel.RemoveWorkExperience(experience);
            Assert.AreEqual(0, viewModel.WorkExperiences.Count);
        }

        [TestMethod]
        public void TestAddProjectAddsProjectToList()
        {
            viewModel.AddProject();
            Assert.AreEqual(1, viewModel.Projects.Count);
        }

        [TestMethod]
        public void TestAddProjectFailsWhenMaximumNumberOfProjectsIsReached()
        {
            int maximumNumberOfProjectsAllowed = 10;
            for (int projectIndex = 0; projectIndex < maximumNumberOfProjectsAllowed; projectIndex++)
            {
                viewModel.AddProject();
            }
            viewModel.AddProject();
            Assert.AreEqual(maximumNumberOfProjectsAllowed, viewModel.Projects.Count);
        }

        [TestMethod]
        public void TestAddProjectShowInfoBarWhenMaximumNumberOfProjectsIsReached()
        {
            int maximumNumberOfProjectsAllowed = 10;
            for (int projectIndex = 0; projectIndex < maximumNumberOfProjectsAllowed; projectIndex++)
            {
                viewModel.AddProject();
            }
            viewModel.AddProject();
            Assert.IsTrue(viewModel.IsInfoBarOpen);
            Assert.AreEqual($"Maximum of {maximumNumberOfProjectsAllowed} projects allowed.", viewModel.InfoBarMessage);
        }

        [TestMethod]
        public void TestRemoveProjectRemovesProjectFromList()
        {
            viewModel.AddProject();
            var project = viewModel.Projects.First();
            viewModel.RemoveProject(project);
            Assert.AreEqual(0, viewModel.Projects.Count);
        }

        [TestMethod]
        public void AddExtraCurricularActivityAddsActivityToList()
        {
            viewModel.AddExtraCurricularActivity();
            Assert.AreEqual(1, viewModel.ExtraCurricularActivities.Count);
        }

        [TestMethod]
        public void AddExtraCurricularActivityFailsWhenMaximumNumberOfActivitiesIsReached()
        {
            int maximumNumberOfExtraCurricularActivitiesAllowed = 10;
            for (int activityIndex = 0; activityIndex < maximumNumberOfExtraCurricularActivitiesAllowed; activityIndex++)
            {
                viewModel.AddExtraCurricularActivity();
            }
            viewModel.AddExtraCurricularActivity();
            Assert.AreEqual(maximumNumberOfExtraCurricularActivitiesAllowed, viewModel.ExtraCurricularActivities.Count);
        }

        [TestMethod]
        public void AddExtraCurricularActivityShowInfoBarWhenMaximumNumberOfActivitiesIsReached()
        {
            int maximumNumberOfExtraCurricularActivitiesAllowed = 10;
            for (int activityIndex = 0; activityIndex < maximumNumberOfExtraCurricularActivitiesAllowed; activityIndex++)
            {
                viewModel.AddExtraCurricularActivity();
            }
            viewModel.AddExtraCurricularActivity();
            Assert.IsTrue(viewModel.IsInfoBarOpen);
            Assert.AreEqual($"Maximum of {maximumNumberOfExtraCurricularActivitiesAllowed} extra-curricular activities allowed.", viewModel.InfoBarMessage);
        }

        [TestMethod]
        public void RemoveExtraCurricularActivityRemovesActivityFromList()
        {
            viewModel.AddExtraCurricularActivity();
            var activity = viewModel.ExtraCurricularActivities.First();
            viewModel.RemoveExtraCurricularActivity(activity);
            Assert.AreEqual(0, viewModel.ExtraCurricularActivities.Count);
        }

        [TestMethod]
        public void TestLoadProfile()
        {
            UserProfile userProfile = new UserProfile
            {
                FirstName = "John",
                LastName = "Doe",
                Age = 25,
                University = "University of Test",
                Degree = "Bachelor's in Testing",
                ExpectedGraduationYear = 2022,
                PhoneNumber = "+40123456789",
                Skills = new List<string> { "Testing", "C#" },
                WorkExperiences = new List<WorkExperience>
                {
                    new WorkExperience
                    {
                        Company = "Test Company",
                        JobTitle = "Tester",
                        StartDate = new DateTime(2020, 1, 1),
                        EndDate = new DateTime(2021, 1, 1),
                        Description = "Testing software",
                        CurrentlyWorking = false
                    }
                },
                Projects = new List<Project>
                {
                    new Project
                    {
                        Name = "Test Project",
                        Description = "A project for testing",
                        Technologies = new List<string> { "C#, NUnit" },
                        Url = "http://testproject.com"
                    }
                },
                ExtraCurricularActivities = new List<ExtraCurricularActivity>
                {
                    new ExtraCurricularActivity
                    {
                        ActivityName = "Testing Club",
                        Description = "A club for testing enthusiasts"
                    }
                }
            };
            viewModel.LoadProfile(userProfile);
            Assert.AreEqual(userProfile.FirstName, viewModel.FirstName);
            Assert.AreEqual(userProfile.LastName, viewModel.LastName);
            Assert.AreEqual(userProfile.Skills.Count, viewModel.Skills.Count);
            Assert.AreEqual(userProfile.WorkExperiences.Count, viewModel.WorkExperiences.Count);
            Assert.AreEqual(userProfile.Projects.Count, viewModel.Projects.Count);
            Assert.AreEqual(userProfile.ExtraCurricularActivities.Count, viewModel.ExtraCurricularActivities.Count);
        }

        [TestMethod]
        public void TestIsDuplicateSkillReturnsTrueForExistingSkill()
        {
            string skill = "C#";
            viewModel.AddSkill(skill);
            Assert.IsTrue(viewModel.IsDuplicateSkill(skill));
        }
    }
}

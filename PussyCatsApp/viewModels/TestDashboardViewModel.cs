using PussyCatsApp.models;
using PussyCatsApp.Models;
using PussyCatsApp.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.viewModels
{
    public class TestDashboardViewModel
    {
        private List<SkillTest> tests;
        private SkillTestService skillTestService;
        private UserProfileViewModel userProfileViewModel;

        public List<SkillTestCardViewModel> TestCards { get; private set; } = new();

        public TestDashboardViewModel(SkillTestService skillTestService, UserProfileViewModel userProfileViewModel)
        {
            this.skillTestService = skillTestService;
            this.userProfileViewModel = userProfileViewModel;
        }

        public void LoadTests(UserProfile userProfile)
        {
            tests = skillTestService.GetTestsForUser(userProfile.UserId);

            TestCards = new List<SkillTestCardViewModel>();
            foreach (SkillTest test in tests)
            {
                TestCards.Add(new SkillTestCardViewModel(test, skillTestService, userProfileViewModel));
            }
        }

        public void goToAllTestsCommand()
        {
        }
    }
}

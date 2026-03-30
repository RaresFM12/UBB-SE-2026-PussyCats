using PussyCatsApp.models;
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

        private int currentUserId;

        public TestDashboardViewModel(SkillTestService skillTestService, UserProfileViewModel userProfileViewModel, int currentUserId)
        {
            this.skillTestService = skillTestService;
            this.userProfileViewModel = userProfileViewModel;
            this.currentUserId = currentUserId;

            LoadTests();
        }

        public void LoadTests()
        {
            tests = skillTestService.getTestsForUser(currentUserId);

            TestCards = new List<SkillTestCardViewModel>();
            foreach (SkillTest test in tests)
            {
                TestCards.Add(new SkillTestCardViewModel(test, skillTestService, userProfileViewModel));
            }
        }

        public void backCommand()
        {

        }

        public void goToAllTestsCommand()
        {

        }
    }
}

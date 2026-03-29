using PussyCatsApp.models;
using PussyCatsApp.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.viewModels
{
    internal class SkillTestCardViewModel
    {
        private SkillTest skillTest;
        private Badge badge;
        private SkillTestService skillTestService;
        private UserProfileViewModel userProfileViewModel;  
        private bool isRetakeEnabled;

        public SkillTest SkillTest => skillTest;
        public Badge Badge => badge;
        public bool IsRetakeEnabled => isRetakeEnabled;

        public SkillTestCardViewModel(SkillTest skillTest, SkillTestService skillTestService, UserProfileViewModel userProfileViewModel)
        {
            this.skillTest = skillTest;
            this.skillTestService = skillTestService;
            this.userProfileViewModel = userProfileViewModel;  

            CheckRetakeEligible();
            badge = Badge.assignTier(skillTest.Score);
        }

        public void LoadCardCommand()
        {
            CheckRetakeEligible();
            UpdateBadge();
        }
        public void CheckRetakeEligible()
        {
            isRetakeEnabled = skillTestService.canRetakeTest(skillTest.SkillTestId);
        }

        public void RetakeCommand()
        {
            if (!isRetakeEnabled)
                return;

            int newScore = GenerateRandomScore();

            badge = skillTestService.submitRetake(skillTest.SkillTestId, newScore);

            skillTest.AchievedDate = DateOnly.FromDateTime(DateTime.Now);
            skillTest.Score = newScore;

            UpdateBadge();

            userProfileViewModel.recalculateLevelCommand();  
        }

        public void UpdateBadge()
        {
            badge = Badge.assignTier(skillTest.Score);
        }

       /* public void onLevelUpdated()
        {
            userProfileViewModel.recalculateLevelCommand();
        }*/
        private int GenerateRandomScore()
        {
            Random random = new Random();
            return random.Next(0, 101);
        }
    }
}

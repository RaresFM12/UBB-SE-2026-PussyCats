using PussyCatsApp.models;
using PussyCatsApp.repositories;
using PussyCatsApp.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.viewModels
{
    public class UserProfileViewModel
    {
        private List<SkillTest> tests;

        public UserLevel CurrentLevel { get; private set; }
        public int TotalXP { get; private set; }

        public event Action OnLevelUpdated;
        public void recalculateLevelCommand()
        {
            int totalXP = 0;
            foreach (SkillTest test in tests)
            {
                totalXP += test.getXP();
            }

            CurrentLevel = UserLevel.calculateLevel(totalXP);

            OnLevelUpdated?.Invoke();
        }

        public void LoadTests()
        {
            tests = UserProfileService.GetSkillTestsForUser(currentUserId);
        }

    }
}

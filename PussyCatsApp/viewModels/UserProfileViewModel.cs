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
    internal class UserProfileViewModel
    {
        private List<SkillTest> tests;
          
        private UserLevel currentLevel;
        public void recalculateLevelCommand()
        {
            int totalXP = 0;
            foreach (SkillTest test in tests)
            {
                totalXP += test.getXP();
            }

            currentLevel = UserLevel.calculateLevel(totalXP);
        }

        public void LoadTests()
        {
            tests = UserProfileService.GetSkillTestsForUser(currentUserId);
        }

    }
}

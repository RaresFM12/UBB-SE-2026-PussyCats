using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.models;
using PussyCatsApp.Models;
using PussyCatsApp.services;
using PussyCatsApp.utilities;

namespace PussyCatsApp.viewModels
{
    public class SkillTestCardViewModel : INotifyPropertyChanged
    {
        private SkillTest skillTest;
        private Badge badge;
        private SkillTestService skillTestService;
        private UserProfileViewModel userProfileViewModel;
        private bool isRetakeEnabled;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public SkillTest SkillTest => skillTest;
        public Badge Badge => badge;
        public bool IsRetakeEnabled => isRetakeEnabled;
        public SkillTestCardViewModel(SkillTest skillTest, SkillTestService skillTestService, UserProfileViewModel userProfileViewModel)
        {
            this.skillTest = skillTest;
            this.skillTestService = skillTestService;
            this.userProfileViewModel = userProfileViewModel;

            CheckRetakeEligible();
            badge = Badge.AssignTier(skillTest.Score);
        }

        public void LoadCardCommand()
        {
            CheckRetakeEligible();
            UpdateBadge();
        }
        public void CheckRetakeEligible()
        {
            isRetakeEnabled = skillTestService.CanRetakeTest(skillTest.SkillTestId);
        }

        public void RetakeCommand()
        {
            if (!isRetakeEnabled)
            {
                return;
            }

            int newTestScore = Helpers.GenerateRandomScore(0, 100);

            badge = skillTestService.SubmitRetake(skillTest.SkillTestId, newTestScore);

            skillTest.AchievedDate = DateOnly.FromDateTime(DateTime.Now);
            skillTest.Score = newTestScore;

            CheckRetakeEligible();
            UpdateBadge();

            OnPropertyChanged(nameof(Badge));
            OnPropertyChanged(nameof(SkillTest));
            OnPropertyChanged(nameof(IsRetakeEnabled));
            userProfileViewModel.RecalculateLevelCommand();
        }

        public void UpdateBadge()
        {
            badge = Badge.AssignTier(skillTest.Score);
        }
    }
}

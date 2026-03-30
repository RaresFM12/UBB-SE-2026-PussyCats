using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.viewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PussyCatsApp.views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserProfileView : Page
    {
        private UserProfileViewModel userProfileViewModel;
        public UserProfileView(UserProfileViewModel userProfileViewModel)
        {
            this.InitializeComponent();
            this.userProfileViewModel = userProfileViewModel;

            userProfileViewModel.OnLevelUpdated += renderLevelDisplay;

            userProfileViewModel.LoadUser(userProfileViewModel);
            renderLevelDisplay();
        }

        private void renderLevelDisplay()
        {
            if (userProfileViewModel.CurrentLevel == null) return;

            LevelTitleText.Text = $"Level {userProfileViewModel.CurrentLevel.LevelNumber} — {userProfileViewModel.CurrentLevel.Title}";

            XpProgressBar.Value = userProfileViewModel.CurrentLevel.getProgressPercent(userProfileViewModel.TotalXP);

            int xpToNext = userProfileViewModel.CurrentLevel.getXPToNextLevel();
            XpCountText.Text = xpToNext > 0
                ? $"{userProfileViewModel.TotalXP} XP — {xpToNext} XP to {userProfileViewModel.CurrentLevel.Title}"
                : $"{userProfileViewModel.TotalXP} XP — Max level reached!";
        }
    }
}

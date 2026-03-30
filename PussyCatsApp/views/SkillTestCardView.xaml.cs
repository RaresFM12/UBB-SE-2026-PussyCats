using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
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
    public sealed partial class SkillTestCardView : UserControl
    {
        private SkillTestCardViewModel skillTestCardViewModel;
        public SkillTestCardView(SkillTestCardViewModel viewModel)
        {
            this.InitializeComponent();
            this.skillTestCardViewModel = viewModel;
            this.DataContext = skillTestCardViewModel;
            BadgeIcon.ImageFailed += (s, e) => {
                System.Diagnostics.Debug.WriteLine($"IMAGE ERROR: {e.ErrorMessage}");
            };
            loadCard();
        }

        private void loadCard()
        { 
            TestNameText.Text = skillTestCardViewModel.SkillTest.Name?.ToUpper() + " TEST" ?? "UNKNOWN TEST";


            string scoreDisplay = $"SCORE: {skillTestCardViewModel.SkillTest.Score}%";
            ScoreText.Text = scoreDisplay;

            DateText.Text = skillTestCardViewModel.SkillTest.AchievedDateFormatted;

            
            if (skillTestCardViewModel.Badge != null && !string.IsNullOrEmpty(skillTestCardViewModel.Badge.IconPath))
            {
                string path = skillTestCardViewModel.Badge.IconPath;

                if (!path.StartsWith("ms-appx:///"))
                {
                    path = $"ms-appx:///{path.TrimStart('/')}";
                }

                var uri = new Uri(path);
                System.Diagnostics.Debug.WriteLine($"FIXED URI: {uri}"); 

                var svgSource = new SvgImageSource(uri);
                svgSource.RasterizePixelWidth = 100;
                svgSource.RasterizePixelHeight = 100;

                BadgeIcon.Source = svgSource;

            }

           
            updateRetakeButton();
        }

        private void updateRetakeButton()
        {
            RetakeButton.IsEnabled = skillTestCardViewModel.IsRetakeEnabled;
            RetakeButton.Opacity = skillTestCardViewModel.IsRetakeEnabled ? 1.0 : 0.4;
        }

        private void RetakeButton_Click(object sender, RoutedEventArgs e)
        {
            skillTestCardViewModel.RetakeCommand();

          
            loadCard();
        }

    }
}

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
    public sealed partial class TestDashboardView : Page
    {
        private TestDashboardViewModel testDashboarddViewModel;

        public TestDashboardView(TestDashboardViewModel testDashboarddViewModel)
        {
            this.InitializeComponent();
            this.testDashboarddViewModel = testDashboarddViewModel;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            testDashboarddViewModel.LoadTests();
            renderTestCards();
        }

        private void renderTestCards()
        {
            TestCardsContainer.Children.Clear();

            foreach (SkillTestCardViewModel cardViewModel in testDashboarddViewModel.TestCards)
            {
                SkillTestCardView cardView = new SkillTestCardView(cardViewModel);
                TestCardsContainer.Children.Add(cardView);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            testDashboarddViewModel.backCommand();
        }

        private void GoToAllTestsButton_Click(object sender, RoutedEventArgs e)
        {
            testDashboarddViewModel.goToAllTestsCommand();
        }
    }
}

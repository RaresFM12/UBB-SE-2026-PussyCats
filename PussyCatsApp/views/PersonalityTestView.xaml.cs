using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.ViewModels;
using PussyCatsApp.Services;
using PussyCatsApp.Repositories.PersonalityTestRepo;
using Windows.Foundation;
using Windows.Foundation.Collections;
using PussyCatsApp.Configuration;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace PussyCatsApp.Views
{
    /// <summary>
    /// Personality Test page where users answer 24 questions to receive role recommendations.
    /// </summary>
    public sealed partial class PersonalityTestView : Page
    {
        private PersonalityTestViewModel personalityTestViewModel;

        public PersonalityTestView()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PersonalityTestView initialization error: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private void OnBackClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // Prefer using this page's Frame if it can go back
                if (Frame != null && Frame.CanGoBack)
                {
                    Frame.GoBack();
                    return;
                }

                // Otherwise, try the application's main window frame
                if (App.MainAppWindow is MainWindow mainWindow && mainWindow.NavigationFrame != null)
                {
                    var navFrame = mainWindow.NavigationFrame;
                    if (navFrame.CanGoBack)
                    {
                        navFrame.GoBack();
                    }
                    else
                    {
                        navFrame.Navigate(typeof(UserProfileView));
                    }
                    return;
                }

                // Fallback: navigate in this page's frame to the profile view
                Frame?.Navigate(typeof(UserProfileView));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[PersonalityTestView] Back navigation error: " + ex);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Get userId from navigation parameter
            int userId = 1; // Default value

            if (e.Parameter is int passedUserId)
            {
                userId = passedUserId;
            }

            InitializeViewModel(userId);
        }
        private void InitializeViewModel(int userId)
        {
            IPersonalityTestService personalityTestService = new PersonalityTestService(new PersonalityTestRepository(DatabaseConfiguration.GetConnectionString()));
            this.personalityTestViewModel = new PersonalityTestViewModel(userId, personalityTestService);
            this.DataContext = this.personalityTestViewModel;
        }
    }
}

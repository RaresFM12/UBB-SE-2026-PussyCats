using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
<<<<<<< HEAD
using PussyCatsApp.repositories.personality_test_repo;
using PussyCatsApp.services;
=======
using PussyCatsApp.repositories;
using PussyCatsApp.services;
using PussyCatsApp.viewModels;
>>>>>>> main
using PussyCatsApp.views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PussyCatsApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Title = "PussyCats - User Profile Management";

            // Set a default window size
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32(1200, 800));

            // Navigate to ProfileForm page by default
            ContentFrame.Navigate(typeof(views.ProfileFormPage));
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null)
            {
                var navItemTag = args.SelectedItemContainer.Tag.ToString();

                switch (navItemTag)
                {
                    case "ProfileForm":
                        ContentFrame.Navigate(typeof(views.ProfileFormPage));
                        break;
                    case "ViewProfile":
                        ContentFrame.Navigate(typeof(views.ViewProfilePage));
                        break;
                    case "Documents":
                        ContentFrame.Navigate(typeof(views.DocumentsPage));
                        break;
                    case "ExportProfile":
                        ContentFrame.Navigate(typeof(views.ExportCVTestPage));
                        break;
                }
            }
        }

        private void btnTestCompatibility_Click(object sender, RoutedEventArgs e)
        {
            RunCompatibilityTest();
        }

        private void RunCompatibilityTest()
        {
            string connectionString = "Data Source=DESKTOP-SCP6QST;Initial Catalog=UserManagementDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
            UserSkillRepository userSkillRepo = new UserSkillRepository(connectionString);
            SkillGroupRepository skillGroupRepo = new SkillGroupRepository();
            CompatibilityService service = new CompatibilityService(userSkillRepo, skillGroupRepo);
            CompatibilityOverviewViewModel vm = new CompatibilityOverviewViewModel(service, 1); // currentUserId

            Window compatibilityWindow = new Window();
            Frame frame = new Frame();
            compatibilityWindow.Content = frame;
            frame.Navigate(typeof(CompatibilityOverviewView), vm);
            compatibilityWindow.Activate();
        }
    }
}

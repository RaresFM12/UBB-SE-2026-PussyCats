using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
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
        private Frame? RootFrame;
        public UserProfileView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RootFrame = e.Parameter as Frame;
        }

        private void OnTakeTestClick(object sender, RoutedEventArgs e)
        {
            //var repository = new PersonalityTestRepository(connectionString);
            //var service = new PersonalityTestService(repository);
            //var viewModel = new PersonalityTestViewModel(service, userId);
            RootFrame?.Navigate(typeof(PersonalityTestView));
        }
    }
}

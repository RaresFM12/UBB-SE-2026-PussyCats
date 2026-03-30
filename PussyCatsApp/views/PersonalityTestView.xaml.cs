/*using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.viewModels;
using PussyCatsApp.services;
using PussyCatsApp.repositories.personality_test_repo;
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
    /// Personality Test page where users answer 24 questions to receive role recommendations.
    /// </summary>
    public sealed partial class PersonalityTestView : Page
    {
        private PersonalityTestViewModel PersonalityTestViewModel;

        public PersonalityTestView()
        {
            try
            {
                InitializeComponent();
                InitializeViewModel();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PersonalityTestView initialization error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private void InitializeViewModel()
        {
            // TODO: Inject userId from app state or navigation context
            // For now using a placeholder userId of 1
            int userId = 1;

            // TODO: Get connection string from app configuration
            string connectionString = "Data Source=.;Initial Catalog=UserManagementDB;Integrated Security=True;Trust Server Certificate=True";

            var repository = new PersonalityTestRepository(connectionString);
            var service = new PersonalityTestService(repository);
            PersonalityTestViewModel = new PersonalityTestViewModel(service, userId);

            this.DataContext = PersonalityTestViewModel;
        }
    }
}
*/
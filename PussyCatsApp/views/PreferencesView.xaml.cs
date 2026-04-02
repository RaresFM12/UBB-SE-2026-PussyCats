using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Linq;
using System;
namespace PussyCatsApp.views
{
    public sealed partial class PreferencesView : Page
    {
        // R13: Cele 8 roluri oficiale din documentație
        private readonly List<string> _allRoles = new()
        {
            "Frontend Developer", "Backend Developer", "UI/UX Designer",
            "DevOps Engineer", "Project Manager", "Data Analyst",
            "Cybersecurity Specialist", "AI/ML Engineer"
        };

        // R15: Câteva locații de test pentru a simula autocompletarea
        private readonly List<string> _mockLocations = new()
        {
            "Bucharest, Romania", "Cluj-Napoca, Romania", "Timisoara, Romania",
            "Sibiu, Romania", "London, UK", "Berlin, Germany", "Paris, France"
        };

        public PreferencesView()
        {
            this.InitializeComponent();

            // Umplem lista de pe ecran cu rolurile
            RolesListView.ItemsSource = _allRoles;
        }

        // Această funcție se declanșează de fiecare dată când bifezi/debifezi un rol
        private void RolesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RolesListView.SelectedItems.Count > 3)
            {
                // Afișează mesajul de eroare
                RoleWarningText.Visibility = Visibility.Visible;

                // Anulează ultima selecție pentru a păstra limita de 3
                foreach (var item in e.AddedItems)
                {
                    RolesListView.SelectedItems.Remove(item);
                }
            }
            else
            {
                // Ascunde mesajul de eroare dacă suntem sub limita de 3
                RoleWarningText.Visibility = Visibility.Collapsed;
            }
        }

        // Logica pentru autocompletarea locației
        private void LocationAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var suitableItems = _mockLocations.Where(x => x.ToLower().Contains(sender.Text.ToLower())).ToList();
                sender.ItemsSource = suitableItems;
            }
        }

        private void LocationAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem.ToString();
        }

        // Ce se întâmplă când dăm Save
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Gather data from the UI
            var selectedRoles = RolesListView.SelectedItems.Cast<string>().ToList();
            var workMode = (WorkModeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
            var location = LocationAutoSuggestBox.Text;

            // Optional Validation
            if (selectedRoles.Count == 0 || string.IsNullOrEmpty(workMode) || string.IsNullOrEmpty(location))
            {
                SuccessMessage.Text = "Please fill in all the fields before saving.";
                SuccessMessage.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
                SuccessMessage.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                // IMPORTANT: You need to pass your actual SQL connection string here!
                // If your app has a global connection string, use that instead.
                string connectionString = @"Server=FAMILY\SQLEXPRESS;Database=PussyCatsDB;Trusted_Connection=True;TrustServerCertificate=True;"; var repo = new repositories.PreferenceRepository(connectionString);

                int currentUserId = 1; // Mocking User ID 1 for testing purposes

                // 2. Clear old preferences to prevent duplicates
                repo.DeleteAllByUserId(currentUserId);

                // 3. Save each selected Job Role as a separate row
                foreach (var role in selectedRoles)
                {
                    repo.AddPreference(new models.Preference
                    {
                        UserId = currentUserId,
                        PreferenceType = "Role",
                        Value = role
                    });
                }

                // 4. Save Work Mode
                repo.AddPreference(new models.Preference
                {
                    UserId = currentUserId,
                    PreferenceType = "WorkMode",
                    Value = workMode
                });

                // 5. Save Location
                repo.AddPreference(new models.Preference
                {
                    UserId = currentUserId,
                    PreferenceType = "Location",
                    Value = location
                });

                // Success Output
                SuccessMessage.Text = "Preferences successfully saved to the database! 🎉";
                SuccessMessage.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green);
                SuccessMessage.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                SuccessMessage.Text = "Database Error: " + ex.Message;
                SuccessMessage.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
                SuccessMessage.Visibility = Visibility.Visible;
            }
        }
    }
}
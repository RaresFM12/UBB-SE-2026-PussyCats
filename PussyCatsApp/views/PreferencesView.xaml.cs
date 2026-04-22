using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using PussyCatsApp.utilities;

namespace PussyCatsApp.views
{
    public sealed partial class PreferencesView : Page
    {
        public PreferencesView()
        {
            this.InitializeComponent();

            RolesListView.ItemsSource = PreferencesData.AllRoles;

            LoadExistingPreferences();
        }

        private void LoadExistingPreferences()
        {
            try
            {
                string connectionString = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build()
                    .GetConnectionString("raresConnectionString");

                var repo = new Repositories.PreferenceRepository(connectionString);

                int currentUserId = 1;

                // presupun că metoda asta returnează toate preferințele userului
                var preferences = repo.GetPreferencesByUserId(currentUserId);

                if (preferences == null || preferences.Count == 0)
                    return;

                // Roluri
                var savedRoles = preferences
                    .Where(p => p.PreferenceType == "JobRole")
                    .Select(p => p.Value)
                    .ToList();

                foreach (var role in savedRoles)
                {
                    if (PreferencesData.AllRoles.Contains(role))
                    {
                        RolesListView.SelectedItems.Add(role);
                    }
                }

                // Work mode
                var savedWorkMode = preferences
                    .FirstOrDefault(p => p.PreferenceType == "WorkMode")?.Value;

                if (!string.IsNullOrEmpty(savedWorkMode))
                {
                    foreach (var item in WorkModeComboBox.Items)
                    {
                        if (item is ComboBoxItem comboBoxItem &&
                            comboBoxItem.Content?.ToString() == savedWorkMode)
                        {
                            WorkModeComboBox.SelectedItem = comboBoxItem;
                            break;
                        }
                    }
                }

                // Location
                var savedLocation = preferences
                    .FirstOrDefault(p => p.PreferenceType == "Location")?.Value;

                if (!string.IsNullOrEmpty(savedLocation))
                {
                    LocationAutoSuggestBox.Text = savedLocation;
                }
            }
            catch (Exception ex)
            {
                SuccessMessage.Text = "Error loading preferences: " + ex.Message;
                SuccessMessage.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red);
                SuccessMessage.Visibility = Visibility.Visible;
            }
        }

        private void RolesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RolesListView.SelectedItems.Count > 3)
            {
                RoleWarningText.Visibility = Visibility.Visible;

                foreach (var item in e.AddedItems)
                {
                    RolesListView.SelectedItems.Remove(item);
                }
            }
            else
            {
                RoleWarningText.Visibility = Visibility.Collapsed;
            }
        }

        private void LocationAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var suitableItems = PreferencesData.MockLocations
                    .Where(x => x.ToLower().Contains(sender.Text.ToLower()))
                    .ToList();

                sender.ItemsSource = suitableItems;
            }
        }

        private void LocationAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedRoles = RolesListView.SelectedItems.Cast<string>().ToList();
            var workMode = (WorkModeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
            var location = LocationAutoSuggestBox.Text;

            if (selectedRoles.Count == 0 || string.IsNullOrEmpty(workMode) || string.IsNullOrEmpty(location))
            {
                SuccessMessage.Text = "Please fill in all the fields before saving.";
                SuccessMessage.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red);
                SuccessMessage.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                string connectionString = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build()
                    .GetConnectionString("raresConnectionString");

                var repo = new Repositories.PreferenceRepository(connectionString);

                int currentUserId = 1;

                repo.DeleteAllByUserId(currentUserId);

                foreach (var role in selectedRoles)
                {
                    repo.AddPreference(new Models.Preference
                    {
                        UserId = currentUserId,
                        PreferenceType = "JobRole",
                        Value = role
                    });
                }

                repo.AddPreference(new Models.Preference
                {
                    UserId = currentUserId,
                    PreferenceType = "WorkMode",
                    Value = workMode
                });

                repo.AddPreference(new Models.Preference
                {
                    UserId = currentUserId,
                    PreferenceType = "Location",
                    Value = location
                });

                SuccessMessage.Text = "Preferences successfully saved to the database!";
                SuccessMessage.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Green);
                SuccessMessage.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                SuccessMessage.Text = "Database Error: " + ex.Message;
                SuccessMessage.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red);
                SuccessMessage.Visibility = Visibility.Visible;
            }
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }
    }
}
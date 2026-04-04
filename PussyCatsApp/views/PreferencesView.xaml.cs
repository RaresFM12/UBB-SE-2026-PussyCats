using Microsoft.Extensions.Configuration;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PussyCatsApp.views
{
    public sealed partial class PreferencesView : Page
    {
        private readonly List<string> _allRoles = new()
        {
            "Frontend Developer", "Backend Developer", "UI/UX Designer",
            "DevOps Engineer", "Project Manager", "Data Analyst",
            "Cybersecurity Specialist", "AI/ML Engineer"
        };

        private readonly List<string> _mockLocations = new()
        {
            // Romania
            "Bucharest, Romania", "Cluj-Napoca, Romania", "Timisoara, Romania", "Iasi, Romania",

            // UK & Ireland
            "London, UK", "Manchester, UK", "Birmingham, UK",
            "Dublin, Ireland",

            // Germany
            "Berlin, Germany", "Munich, Germany", "Hamburg, Germany", "Frankfurt, Germany",

            // France
            "Paris, France", "Lyon, France", "Marseille, France",

            // Spain
            "Madrid, Spain", "Barcelona, Spain", "Valencia, Spain",

            // Italy
            "Rome, Italy", "Milan, Italy", "Naples, Italy", "Florence, Italy",

            // Netherlands
            "Amsterdam, Netherlands", "Rotterdam, Netherlands", "Utrecht, Netherlands",

            // Belgium
            "Brussels, Belgium", "Antwerp, Belgium",

            // Switzerland
            "Zurich, Switzerland", "Geneva, Switzerland",

            // Austria
            "Vienna, Austria", "Salzburg, Austria",

            // Nordics
            "Stockholm, Sweden", "Gothenburg, Sweden",
            "Oslo, Norway",
            "Copenhagen, Denmark",
            "Helsinki, Finland",

            // Poland
            "Warsaw, Poland", "Krakow, Poland", "Wroclaw, Poland",

            // Czech Republic
            "Prague, Czech Republic", "Brno, Czech Republic",

            // Hungary
            "Budapest, Hungary",

            // Greece
            "Athens, Greece", "Thessaloniki, Greece",

            // Portugal
            "Lisbon, Portugal", "Porto, Portugal",

            // Balkans
            "Belgrade, Serbia",
            "Zagreb, Croatia",
            "Ljubljana, Slovenia",
            "Sarajevo, Bosnia and Herzegovina",
            "Sofia, Bulgaria",

            // Turkey
            "Istanbul, Turkey", "Ankara, Turkey",

            // USA (mai multe pentru că e huge)
            "New York, USA", "Los Angeles, USA", "Chicago, USA",
            "San Francisco, USA", "Seattle, USA", "Austin, USA",
            "Boston, USA", "Miami, USA",

            // Canada
            "Toronto, Canada", "Vancouver, Canada", "Montreal, Canada",

            // Australia
            "Sydney, Australia", "Melbourne, Australia", "Brisbane, Australia",

            // Japan
            "Tokyo, Japan", "Osaka, Japan",

            // South Korea
            "Seoul, South Korea",

            // China
            "Beijing, China", "Shanghai, China", "Shenzhen, China",

            // Southeast Asia
            "Singapore, Singapore",
            "Bangkok, Thailand",
            "Kuala Lumpur, Malaysia",
            "Jakarta, Indonesia",
            "Manila, Philippines",

            // India
            "Delhi, India", "Mumbai, India", "Bangalore, India",

            // Middle East
            "Dubai, UAE", "Abu Dhabi, UAE",
            "Tel Aviv, Israel",
            "Doha, Qatar",

            // Africa (puțin mai multe pe regiune)
            "Cape Town, South Africa", "Johannesburg, South Africa", "Durban, South Africa",
            "Cairo, Egypt",
            "Nairobi, Kenya",
            "Lagos, Nigeria",

            // South America
            "Sao Paulo, Brazil", "Rio de Janeiro, Brazil",
            "Buenos Aires, Argentina",
            "Santiago, Chile",
            "Bogota, Colombia",
            "Lima, Peru"
        };

        public PreferencesView()
        {
            this.InitializeComponent();

            RolesListView.ItemsSource = _allRoles;

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

                var repo = new repositories.PreferenceRepository(connectionString);

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
                    if (_allRoles.Contains(role))
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
                var suitableItems = _mockLocations
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

                var repo = new repositories.PreferenceRepository(connectionString);

                int currentUserId = 1;

                repo.DeleteAllByUserId(currentUserId);

                foreach (var role in selectedRoles)
                {
                    repo.AddPreference(new models.Preference
                    {
                        UserId = currentUserId,
                        PreferenceType = "JobRole",
                        Value = role
                    });
                }

                repo.AddPreference(new models.Preference
                {
                    UserId = currentUserId,
                    PreferenceType = "WorkMode",
                    Value = workMode
                });

                repo.AddPreference(new models.Preference
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
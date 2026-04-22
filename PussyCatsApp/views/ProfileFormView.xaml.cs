using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.Models;
using PussyCatsApp.ViewModels;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace PussyCatsApp.Views
{
    /// <summary>
    /// View for displaying and editing the user profile form, including data binding, validation, and navigation.
    /// </summary>
    public sealed partial class ProfileFormView : Page
    {
        private ProfileFormViewModel viewModel;

        public ProfileFormView()
        {
            this.InitializeComponent();

            viewModel = ProfileFormViewModel.Create();

            PopulateGraduationYears();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is UserProfile profile)
            {
                viewModel.LoadProfile(profile);
                LoadViewFromViewModel();
            }
            else
            {
                viewModel.LoadProfile(new UserProfile());
            }
        }

        private void PopulateGraduationYears()
        {
            foreach (var year in viewModel.GraduationYears)
            {
                GraduationYearComboBox.Items.Add(year.ToString());
            }
        }

        private void LoadViewFromViewModel()
        {
            FirstNameTextBox.Text = viewModel.FirstName;
            LastNameTextBox.Text = viewModel.LastName;
            AgeNumberBox.Value = viewModel.Age;

            if (!string.IsNullOrEmpty(viewModel.Gender))
            {
                SelectGender(viewModel.Gender);
            }

            EmailTextBox.Text = viewModel.Email;
            GitHubTextBox.Text = viewModel.GitHub;
            LinkedInTextBox.Text = viewModel.LinkedIn;
            UniversityAutoSuggest.Text = viewModel.University;
            AddressTextBox.Text = viewModel.Address;
            MotivationTextBox.Text = viewModel.Motivation;

            if (!string.IsNullOrEmpty(viewModel.PhonePrefix))
            {
                SelectPhonePrefix(viewModel.PhonePrefix);
            }

            PhoneNumberTextBox.Text = viewModel.PhoneNumber;

            if (!string.IsNullOrEmpty(viewModel.Country))
            {
                SelectCountry(viewModel.Country);
            }

            // Set city AFTER country selection to avoid CountryComboBox_SelectionChanged clearing it
            CityTextBox.Text = viewModel.City;

            if (viewModel.ExpectedGraduationYear > 0)
            {
                SelectGraduationYear(viewModel.ExpectedGraduationYear);
            }

            SkillsItemsRepeater.ItemsSource = viewModel.Skills;
            WorkExperienceItemsRepeater.ItemsSource = viewModel.WorkExperiences;
            ProjectsItemsRepeater.ItemsSource = viewModel.Projects;
            ActivitiesItemsRepeater.ItemsSource = viewModel.ExtraCurricularActivities;
            DisabilitiesCheckBox.IsChecked = viewModel.HasDisabilities;
        }

        private void SyncViewToViewModel()
        {
            viewModel.FirstName = FirstNameTextBox.Text;
            viewModel.LastName = LastNameTextBox.Text;
            viewModel.Age = AgeNumberBox.Value;
            viewModel.Gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty;
            viewModel.Email = EmailTextBox.Text;
            viewModel.PhonePrefix = (PhonePrefixComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty;
            viewModel.PhoneNumber = PhoneNumberTextBox.Text;
            viewModel.GitHub = GitHubTextBox.Text;
            viewModel.LinkedIn = LinkedInTextBox.Text;
            viewModel.Country = (CountryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty;
            viewModel.City = CityTextBox.Text;
            viewModel.University = UniversityAutoSuggest.Text;
            viewModel.Address = AddressTextBox.Text;
            viewModel.Motivation = MotivationTextBox.Text;
            viewModel.ExpectedGraduationYear = int.TryParse(GraduationYearComboBox.SelectedItem?.ToString(), out var yr) ? yr : 0;
            // ViewModel.HasDisabilities = DisabilitiesCheckBox.IsChecked == true;
        }

        private async void UploadCVButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".json");
            picker.FileTypeFilter.Add(".xml");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainAppWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                string content = await FileIO.ReadTextAsync(file);
                viewModel.ProcessCVFile(content, file.FileType);
                LoadViewFromViewModel();
                ShowInfoBarFromViewModel();
            }
        }

        private void ShowInfoBarFromViewModel()
        {
            CVUploadInfoBar.Message = viewModel.InfoBarMessage;
            CVUploadInfoBar.Severity = (InfoBarSeverity)viewModel.InfoBarSeverity;
            CVUploadInfoBar.IsOpen = viewModel.IsInfoBarOpen;
            CVStatusText.Text = viewModel.CvStatusText;
        }

        private void ShowInfoBar(string message, InfoBarSeverity severity)
        {
            CVUploadInfoBar.Message = message;
            CVUploadInfoBar.Severity = severity;
            CVUploadInfoBar.IsOpen = true;
        }

        private void ShowFormValidation(string message, InfoBarSeverity severity)
        {
            FormValidationInfoBar.Message = message;
            FormValidationInfoBar.Severity = severity;
            FormValidationInfoBar.IsOpen = true;
        }

        private void SelectGender(string gender)
        {
            foreach (ComboBoxItem item in GenderComboBox.Items)
            {
                if (item.Content.ToString().Equals(gender, StringComparison.OrdinalIgnoreCase))
                {
                    GenderComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void SelectCountry(string country)
        {
            foreach (ComboBoxItem item in CountryComboBox.Items)
            {
                if (item.Content.ToString().Equals(country, StringComparison.OrdinalIgnoreCase))
                {
                    CountryComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void SelectPhonePrefix(string prefix)
        {
            foreach (ComboBoxItem item in PhonePrefixComboBox.Items)
            {
                if (item.Content.ToString() == prefix)
                {
                    PhonePrefixComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void SelectGraduationYear(int year)
        {
            string yearStr = year.ToString();
            foreach (var item in GraduationYearComboBox.Items)
            {
                if (item.ToString() == yearStr)
                {
                    GraduationYearComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void UniversityAutoSuggest_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = viewModel.FilterUniversities(sender.Text);
            }
        }

        private void UniversityAutoSuggest_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                sender.Text = args.ChosenSuggestion.ToString();
            }
        }

        private void SkillsAutoSuggest_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = viewModel.FilterSkillSuggestions(sender.Text);
            }
        }

        private void SkillsAutoSuggest_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            string skillToAdd = args.ChosenSuggestion?.ToString() ?? sender.Text;
            viewModel.AddSkill(skillToAdd);
            sender.Text = string.Empty;
            ShowInfoBarFromViewModel();
        }

        private void AddSkillButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AddSkill(SkillsAutoSuggest.Text);
            SkillsAutoSuggest.Text = string.Empty;
            ShowInfoBarFromViewModel();
        }

        private void RemoveSkillButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string skill)
            {
                viewModel.RemoveSkill(skill);
            }
        }

        private void AddWorkExperienceButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AddWorkExperience();
            ShowInfoBarFromViewModel();
        }

        private void RemoveWorkExperienceButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is WorkExperience we)
            {
                viewModel.RemoveWorkExperience(we);
            }
        }

        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AddProject();
            ShowInfoBarFromViewModel();
        }

        private void RemoveProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Project project)
            {
                viewModel.RemoveProject(project);
            }
        }

        private void AddActivityButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AddExtraCurricularActivity();
            ShowInfoBarFromViewModel();
        }

        private void RemoveActivityButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ExtraCurricularActivity activity)
            {
                viewModel.RemoveExtraCurricularActivity(activity);
            }
        }

        private void NameTextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => char.IsDigit(c));
        }

        private void PhoneNumberTextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

        private void EndDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs args)
        {
            try
            {
                if (sender is DatePicker endPicker && endPicker.Parent is Grid dateGrid)
                {
                    var startPicker = dateGrid.Children.OfType<DatePicker>().FirstOrDefault(dp => dp != endPicker);
                    if (startPicker != null && args.NewDate.Date < startPicker.Date.Date)
                    {
                        ShowFormValidation("End date cannot be before start date. Please correct it before saving.", InfoBarSeverity.Warning);
                    }
                    else
                    {
                        FormValidationInfoBar.IsOpen = false;
                    }
                }
            }
            catch
            {
                // Prevent any crash from date picker events
            }
        }

        private void CountryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CityTextBox.Text = string.Empty;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SyncViewToViewModel();

            bool success = viewModel.SaveProfile();
            ShowInfoBarFromViewModel();

            if (!success)
            {
                ShowFormValidation(viewModel.InfoBarMessage, (InfoBarSeverity)viewModel.InfoBarSeverity);
                return;
            }

            FormValidationInfoBar.IsOpen = false;

            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
            else
            {
                Frame.Navigate(typeof(UserProfileView));
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
            else
            {
                Frame.Navigate(typeof(UserProfileView));
            }
        }
        private void EditPreferencesButton_Click(object sender, RoutedEventArgs e)
        {
            SyncViewToViewModel();

            Frame.Navigate(typeof(Views.PreferencesView));
        }
    }
}
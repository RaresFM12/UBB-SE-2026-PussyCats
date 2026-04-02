using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.models;
using PussyCatsApp.viewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace PussyCatsApp.views
{
    public sealed partial class ProfileFormPage : Page
    {
        private ProfileFormViewModel _viewModel;

        public ProfileFormPage()
        {
            this.InitializeComponent();

            _viewModel = ProfileFormViewModel.Create();

            PopulateGraduationYears();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is UserProfile profile)
            {
                _viewModel.LoadProfile(profile);
                LoadViewFromViewModel();
            }
            else
            {
                _viewModel.LoadProfile(new UserProfile());
            }
        }

        private void PopulateGraduationYears()
        {
            foreach (var year in _viewModel.GraduationYears)
            {
                GraduationYearComboBox.Items.Add(year.ToString());
            }
        }

        private void LoadViewFromViewModel()
        {
            FirstNameTextBox.Text = _viewModel.FirstName;
            LastNameTextBox.Text = _viewModel.LastName;
            AgeNumberBox.Value = _viewModel.Age;

            if (!string.IsNullOrEmpty(_viewModel.Gender))
                SelectGender(_viewModel.Gender);

            EmailTextBox.Text = _viewModel.Email;
            GitHubTextBox.Text = _viewModel.GitHub;
            LinkedInTextBox.Text = _viewModel.LinkedIn;
            UniversityAutoSuggest.Text = _viewModel.University;
            AddressTextBox.Text = _viewModel.Address;
            MotivationTextBox.Text = _viewModel.Motivation;

            if (!string.IsNullOrEmpty(_viewModel.PhonePrefix))
                SelectPhonePrefix(_viewModel.PhonePrefix);
            PhoneNumberTextBox.Text = _viewModel.PhoneNumber;

            if (!string.IsNullOrEmpty(_viewModel.Country))
                SelectCountry(_viewModel.Country);

            // Set city AFTER country selection to avoid CountryComboBox_SelectionChanged clearing it
            CityTextBox.Text = _viewModel.City;

            if (_viewModel.ExpectedGraduationYear > 0)
                SelectGraduationYear(_viewModel.ExpectedGraduationYear);

            SkillsItemsRepeater.ItemsSource = _viewModel.Skills;
            WorkExperienceItemsRepeater.ItemsSource = _viewModel.WorkExperiences;
            ProjectsItemsRepeater.ItemsSource = _viewModel.Projects;
            ActivitiesItemsRepeater.ItemsSource = _viewModel.ExtraCurricularActivities;
            DisabilitiesCheckBox.IsChecked = _viewModel.HasDisabilities;
        }

        private void SyncViewToViewModel()
        {
            _viewModel.FirstName = FirstNameTextBox.Text;
            _viewModel.LastName = LastNameTextBox.Text;
            _viewModel.Age = AgeNumberBox.Value;
            _viewModel.Gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
            _viewModel.Email = EmailTextBox.Text;
            _viewModel.PhonePrefix = (PhonePrefixComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
            _viewModel.PhoneNumber = PhoneNumberTextBox.Text;
            _viewModel.GitHub = GitHubTextBox.Text;
            _viewModel.LinkedIn = LinkedInTextBox.Text;
            _viewModel.Country = (CountryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
            _viewModel.City = CityTextBox.Text;
            _viewModel.University = UniversityAutoSuggest.Text;
            _viewModel.Address = AddressTextBox.Text;
            _viewModel.Motivation = MotivationTextBox.Text;
            _viewModel.ExpectedGraduationYear = int.TryParse(GraduationYearComboBox.SelectedItem?.ToString(), out var yr) ? yr : 0;
            _viewModel.HasDisabilities = DisabilitiesCheckBox.IsChecked == true;
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
                _viewModel.ProcessCVFile(content, file.FileType);
                LoadViewFromViewModel();
                ShowInfoBarFromViewModel();
            }
        }

        private void ShowInfoBarFromViewModel()
        {
            CVUploadInfoBar.Message = _viewModel.InfoBarMessage;
            CVUploadInfoBar.Severity = (InfoBarSeverity)_viewModel.InfoBarSeverity;
            CVUploadInfoBar.IsOpen = _viewModel.IsInfoBarOpen;
            CVStatusText.Text = _viewModel.CvStatusText;
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
                sender.ItemsSource = _viewModel.FilterUniversities(sender.Text);
            }
        }

        private void UniversityAutoSuggest_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
                sender.Text = args.ChosenSuggestion.ToString();
        }

        private void SkillsAutoSuggest_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = _viewModel.FilterSkillSuggestions(sender.Text);
            }
        }

        private void SkillsAutoSuggest_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            string skillToAdd = args.ChosenSuggestion?.ToString() ?? sender.Text;
            _viewModel.AddSkill(skillToAdd);
            sender.Text = string.Empty;
            ShowInfoBarFromViewModel();
        }

        private void AddSkillButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddSkill(SkillsAutoSuggest.Text);
            SkillsAutoSuggest.Text = string.Empty;
            ShowInfoBarFromViewModel();
        }

        private void RemoveSkillButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string skill)
                _viewModel.RemoveSkill(skill);
        }

        private void AddWorkExperienceButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddWorkExperience();
            ShowInfoBarFromViewModel();
        }

        private void RemoveWorkExperienceButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is WorkExperience we)
                _viewModel.RemoveWorkExperience(we);
        }

        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddProject();
            ShowInfoBarFromViewModel();
        }

        private void RemoveProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Project project)
                _viewModel.RemoveProject(project);
        }

        private void AddActivityButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddExtraCurricularActivity();
            ShowInfoBarFromViewModel();
        }

        private void RemoveActivityButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ExtraCurricularActivity activity)
                _viewModel.RemoveExtraCurricularActivity(activity);
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

            bool success = _viewModel.SaveProfile();
            ShowInfoBarFromViewModel();

            if (!success)
            {
                ShowFormValidation(_viewModel.InfoBarMessage, (InfoBarSeverity)_viewModel.InfoBarSeverity);
                return;
            }

            FormValidationInfoBar.IsOpen = false;

            if (Frame.CanGoBack)
                Frame.GoBack();
            else
                Frame.Navigate(typeof(UserProfileView));
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
            else
                Frame.Navigate(typeof(UserProfileView));
        }
    }
}
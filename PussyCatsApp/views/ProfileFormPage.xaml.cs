using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.models;
using PussyCatsApp.services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace PussyCatsApp.views
{
    public sealed partial class ProfileFormPage : Page
    {
        private UserProfile _userProfile;
        private ObservableCollection<string> _skills;
        private ObservableCollection<WorkExperience> _workExperiences;
        private ObservableCollection<Project> _projects;
        private List<string> _universityList;
        private List<string> _skillSuggestions;

        public ProfileFormPage()
        {
            this.InitializeComponent();
            InitializeCollections();
            LoadUniversityList();
            LoadSkillSuggestions();
            PopulateGraduationYears();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is UserProfile profile)
            {
                _userProfile = profile;
                LoadProfileData();
            }
            else
            {
                _userProfile = new UserProfile();
            }
        }

        private void InitializeCollections()
        {
            _skills = new ObservableCollection<string>();
            _workExperiences = new ObservableCollection<WorkExperience>();
            _projects = new ObservableCollection<Project>();

            SkillsItemsRepeater.ItemsSource = _skills;
            WorkExperienceItemsRepeater.ItemsSource = _workExperiences;
            ProjectsItemsRepeater.ItemsSource = _projects;
        }

        private void LoadUniversityList()
        {
            // Load a predefined list of universities
            _universityList = new List<string>
            {
                "Babes-Bolyai University",
                "Technical University of Cluj-Napoca",
                "University of Bucharest",
                "Politehnica University of Bucharest",
                "Alexandru Ioan Cuza University",
                "West University of Timisoara",
                "University of Medicine and Pharmacy Cluj-Napoca",
                "Academy of Economic Studies Bucharest"
            };
        }

        private void LoadSkillSuggestions()
        {
            _skillSuggestions = new List<string>
            {
                // Languages
                "JavaScript", "TypeScript", "Python", "Java", "C#", "C++", "Go", "Rust",
                "PHP", "Ruby", "Swift", "Kotlin", "Scala",
                // Frameworks
                "React", "Angular", "Vue.js", "Next.js", "ASP.NET Core", "Spring Boot",
                "Django", "Flask", "FastAPI", "Node.js",
                // DevOps/Cloud
                "Docker", "Kubernetes", "Git", "CI/CD", "Azure", "AWS", "GCP", "Linux",
                "Bash", "Terraform",
                // Databases
                "SQL Server", "PostgreSQL", "MySQL", "MongoDB", "Redis", "Oracle",
                // Data & AI
                "Machine Learning", "Deep Learning", "TensorFlow", "PyTorch", "Pandas",
                "NumPy", "Tableau", "Power BI",
                // Design
                "Figma", "Adobe XD", "UI/UX", "Wireframing", "Prototyping",
                // Soft Skills
                "Teamwork", "Communication", "Problem Solving", "Leadership", "Time Management",
                // Other
                "REST API", "GraphQL", "Agile/Scrum", "Unit Testing", "Cybersecurity",
                "OOP", "Microservices"
            };
        }

        private void PopulateGraduationYears()
        {
            int currentYear = DateTime.Now.Year;
            for (int year = currentYear; year <= currentYear + 10; year++)
            {
                GraduationYearComboBox.Items.Add(year.ToString());
            }
        }

        private void LoadProfileData()
        {
            if (_userProfile == null) return;

            // Load required fields
            FirstNameTextBox.Text = _userProfile.FirstName;
            LastNameTextBox.Text = _userProfile.LastName;
            AgeNumberBox.Value = _userProfile.Age;

            if (!string.IsNullOrEmpty(_userProfile.Gender))
            {
                foreach (ComboBoxItem item in GenderComboBox.Items)
                {
                    if (item.Content.ToString() == _userProfile.Gender)
                    {
                        GenderComboBox.SelectedItem = item;
                        break;
                    }
                }
            }

            EmailTextBox.Text = _userProfile.Email;
            GitHubTextBox.Text = _userProfile.GitHub;
            LinkedInTextBox.Text = _userProfile.LinkedIn;
            CityTextBox.Text = _userProfile.City;
            UniversityAutoSuggest.Text = _userProfile.University;
            AddressTextBox.Text = _userProfile.Address;

            // Load phone number
            if (!string.IsNullOrEmpty(_userProfile.PhoneNumber))
            {
                var parts = ExtractPhonePrefixAndNumber(_userProfile.PhoneNumber);
                SelectPhonePrefix(parts.prefix);
                PhoneNumberTextBox.Text = parts.number;
            }

            // Load country
            SelectCountry(_userProfile.Country);

            // Load graduation year
            if (_userProfile.ExpectedGraduationYear > 0)
            {
                string yearStr = _userProfile.ExpectedGraduationYear.ToString();
                foreach (var item in GraduationYearComboBox.Items)
                {
                    if (item.ToString() == yearStr)
                    {
                        GraduationYearComboBox.SelectedItem = item;
                        break;
                    }
                }
            }

            // Load optional fields
            MotivationTextBox.Text = _userProfile.Motivation;

            // Load collections
            foreach (var skill in _userProfile.Skills)
            {
                _skills.Add(skill);
            }

            foreach (var we in _userProfile.WorkExperiences)
            {
                _workExperiences.Add(we);
            }

            foreach (var project in _userProfile.Projects)
            {
                _projects.Add(project);
            }
        }

        private async void UploadCVButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".json");
            picker.FileTypeFilter.Add(".xml");

            // Get the window handle
            var window = (Application.Current as App)?.MainWindow as MainWindow;
            if (window == null) return;

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                await ProcessCVFile(file);
            }
        }

        private async Task ProcessCVFile(StorageFile file)
        {
            try
            {
                string content = await FileIO.ReadTextAsync(file);
                CVStructure cvData = null;

                if (file.FileType.ToLower() == ".json")
                {
                    cvData = JsonSerializer.Deserialize<CVStructure>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else if (file.FileType.ToLower() == ".xml")
                {
                    var xDoc = XDocument.Parse(content);
                    var rootName = xDoc.Root?.Name.LocalName ?? "CVStructure";
                    using (var reader = new StringReader(content))
                    {
                        var serializer = new XmlSerializer(typeof(CVStructure), new XmlRootAttribute(rootName));
                        cvData = (CVStructure)serializer.Deserialize(reader);
                    }
                }

                if (cvData != null)
                {
                    PopulateFormFromCV(cvData);
                    CVStatusText.Text = "CV loaded successfully!";
                    ShowInfoBar("CV data has been loaded. Please review and complete any missing fields.", InfoBarSeverity.Success);
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ex.InnerException?.Message ?? ex.Message;
                ShowInfoBar($"Error processing CV file: {errorMsg}", InfoBarSeverity.Error);
            }
        }

        private void PopulateFormFromCV(CVStructure cvData)
        {
            // Populate required fields
            if (!string.IsNullOrEmpty(cvData.FirstName))
                FirstNameTextBox.Text = cvData.FirstName;
            if (!string.IsNullOrEmpty(cvData.LastName))
                LastNameTextBox.Text = cvData.LastName;
            if (cvData.Age > 0)
                AgeNumberBox.Value = cvData.Age;
            if (!string.IsNullOrEmpty(cvData.Gender))
                SelectGender(cvData.Gender);
            if (!string.IsNullOrEmpty(cvData.Email))
                EmailTextBox.Text = cvData.Email;
            if (!string.IsNullOrEmpty(cvData.PhoneNumber))
            {
                var parts = ExtractPhonePrefixAndNumber(cvData.PhoneNumber);
                SelectPhonePrefix(parts.prefix);
                PhoneNumberTextBox.Text = parts.number;
            }
            if (!string.IsNullOrEmpty(cvData.GitHub))
                GitHubTextBox.Text = cvData.GitHub;
            if (!string.IsNullOrEmpty(cvData.LinkedIn))
                LinkedInTextBox.Text = cvData.LinkedIn;
            if (!string.IsNullOrEmpty(cvData.Country))
                SelectCountry(cvData.Country);
            if (!string.IsNullOrEmpty(cvData.City))
                CityTextBox.Text = cvData.City;
            if (!string.IsNullOrEmpty(cvData.University))
                UniversityAutoSuggest.Text = cvData.University;
            if (cvData.ExpectedGraduationYear > 0)
                SelectGraduationYear(cvData.ExpectedGraduationYear);
            if (!string.IsNullOrEmpty(cvData.Address))
                AddressTextBox.Text = cvData.Address;

            // Populate optional fields
            if (!string.IsNullOrEmpty(cvData.Motivation))
                MotivationTextBox.Text = cvData.Motivation;

            // Add skills (checking for duplicates)
            if (cvData.Skills != null)
            {
                foreach (var skill in cvData.Skills)
                {
                    if (!_skills.Any(s => s.Equals(skill, StringComparison.OrdinalIgnoreCase)))
                    {
                        if (_skills.Count < 30) // Max 30 skills
                        {
                            _skills.Add(skill);
                        }
                    }
                }
            }

            // Add work experiences
            if (cvData.WorkExperiences != null)
            {
                foreach (var we in cvData.WorkExperiences)
                {
                    if (_workExperiences.Count < 10) // Max 10 work experiences
                    {
                        _workExperiences.Add(new WorkExperience
                        {
                            Company = we.Company ?? "",
                            JobTitle = we.JobTitle ?? "",
                            StartDate = we.StartDate,
                            EndDate = we.EndDate,
                            CurrentlyWorking = we.CurrentlyWorking,
                            Description = we.Description ?? ""
                        });
                    }
                }
            }

            // Add projects
            if (cvData.Projects != null)
            {
                foreach (var proj in cvData.Projects)
                {
                    if (_projects.Count < 10) // Max 10 projects
                    {
                        _projects.Add(new Project
                        {
                            Name = proj.Name ?? "",
                            Description = proj.Description ?? "",
                            Technologies = proj.Technologies ?? new List<string>(),
                            Url = proj.Url ?? ""
                        });
                    }
                }
            }

            // List missing fields
            var missingFields = new List<string>();
            if (string.IsNullOrEmpty(FirstNameTextBox.Text)) missingFields.Add("First Name");
            if (string.IsNullOrEmpty(LastNameTextBox.Text)) missingFields.Add("Last Name");
            if (AgeNumberBox.Value == 0) missingFields.Add("Age");
            if (GenderComboBox.SelectedItem == null) missingFields.Add("Gender");
            if (string.IsNullOrEmpty(EmailTextBox.Text)) missingFields.Add("Email");
            if (string.IsNullOrEmpty(PhoneNumberTextBox.Text)) missingFields.Add("Phone Number");
            if (CountryComboBox.SelectedItem == null) missingFields.Add("Country");
            if (string.IsNullOrEmpty(CityTextBox.Text)) missingFields.Add("City");
            if (string.IsNullOrEmpty(UniversityAutoSuggest.Text)) missingFields.Add("University");
            if (GraduationYearComboBox.SelectedItem == null) missingFields.Add("Expected Graduation Year");

            if (missingFields.Any())
            {
                ShowInfoBar($"Missing fields: {string.Join(", ", missingFields)}", InfoBarSeverity.Warning);
            }
        }

        private void ShowInfoBar(string message, InfoBarSeverity severity)
        {
            CVUploadInfoBar.Message = message;
            CVUploadInfoBar.Severity = severity;
            CVUploadInfoBar.IsOpen = true;
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

        private (string prefix, string number) ExtractPhonePrefixAndNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return ("", "");

            var prefixes = new[] { "+40", "+44", "+49", "+33", "+39", "+34", "+31", "+48", "+43", "+32" };
            foreach (var prefix in prefixes)
            {
                if (phoneNumber.StartsWith(prefix))
                {
                    return (prefix, phoneNumber.Substring(prefix.Length));
                }
            }
            return ("", phoneNumber);
        }

        private void UniversityAutoSuggest_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var suitableItems = new List<string>();
                var splitText = sender.Text.ToLower().Split(' ');

                foreach (var uni in _universityList)
                {
                    var found = splitText.All((key) => uni.ToLower().Contains(key));
                    if (found)
                    {
                        suitableItems.Add(uni);
                    }
                }

                sender.ItemsSource = suitableItems;
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
                var suitableItems = new List<string>();
                var searchText = sender.Text.ToLower();

                foreach (var skill in _skillSuggestions)
                {
                    if (skill.ToLower().Contains(searchText) &&
                        !_skills.Any(s => s.Equals(skill, StringComparison.OrdinalIgnoreCase)))
                    {
                        suitableItems.Add(skill);
                    }
                }

                sender.ItemsSource = suitableItems;
            }
        }

        private void SkillsAutoSuggest_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            string skillToAdd = args.ChosenSuggestion?.ToString() ?? sender.Text;
            AddSkill(skillToAdd);
            sender.Text = string.Empty;
        }

        private void AddSkillButton_Click(object sender, RoutedEventArgs e)
        {
            AddSkill(SkillsAutoSuggest.Text);
            SkillsAutoSuggest.Text = string.Empty;
        }

        private void AddSkill(string skill)
        {
            if (string.IsNullOrWhiteSpace(skill))
                return;

            skill = skill.Trim();

            // Check for duplicates (case insensitive)
            if (_skills.Any(s => s.Equals(skill, StringComparison.OrdinalIgnoreCase)))
            {
                ShowInfoBar("This skill has already been added.", InfoBarSeverity.Warning);
                return;
            }

            // Check max limit
            if (_skills.Count >= 30)
            {
                ShowInfoBar("Maximum of 30 skills allowed.", InfoBarSeverity.Warning);
                return;
            }

            // Check skill length
            if (skill.Length > 60)
            {
                ShowInfoBar("Skill name must be less than 60 characters.", InfoBarSeverity.Warning);
                return;
            }

            _skills.Add(skill);
        }

        private void RemoveSkillButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string skill)
            {
                _skills.Remove(skill);
            }
        }

        private void AddWorkExperienceButton_Click(object sender, RoutedEventArgs e)
        {
            if (_workExperiences.Count >= 10)
            {
                ShowInfoBar("Maximum of 10 work experiences allowed.", InfoBarSeverity.Warning);
                return;
            }

            _workExperiences.Add(new WorkExperience
            {
                StartDate = DateTime.Now,
                CurrentlyWorking = false
            });
        }

        private void RemoveWorkExperienceButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is WorkExperience we)
            {
                _workExperiences.Remove(we);
            }
        }

        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (_projects.Count >= 10)
            {
                ShowInfoBar("Maximum of 10 projects allowed.", InfoBarSeverity.Warning);
                return;
            }

            _projects.Add(new Project());
        }

        private void CountryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Reset city when country changes
            CityTextBox.Text = string.Empty;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            var validationErrors = ValidateForm();
            if (validationErrors.Any())
            {
                ShowInfoBar($"Please fill in required fields: {string.Join(", ", validationErrors)}", InfoBarSeverity.Error);
                return;
            }

            // Create or update profile
            UpdateProfileFromForm();

            // Save to repository (implement this based on your repository pattern)
            try
            {
                // TODO: Save to database using repository
                // var repository = new UserProfileRepository();
                // await repository.SaveProfile(_userProfile);

                ShowInfoBar("Profile saved successfully!", InfoBarSeverity.Success);

                // Navigate to profile view
                if (Frame.CanGoBack)
                    Frame.GoBack();
                else
                    Frame.Navigate(typeof(ViewProfilePage), _userProfile);
            }
            catch (Exception ex)
            {
                ShowInfoBar($"Error saving profile: {ex.Message}", InfoBarSeverity.Error);
            }
        }

        private List<string> ValidateForm()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
                errors.Add("First Name");
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
                errors.Add("Last Name");
            if (AgeNumberBox.Value < 16 || AgeNumberBox.Value > 60)
                errors.Add("Age (must be between 16-60)");
            if (GenderComboBox.SelectedItem == null)
                errors.Add("Gender");
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text) || !IsValidEmail(EmailTextBox.Text))
                errors.Add("Valid Email");
            if (PhonePrefixComboBox.SelectedItem == null || string.IsNullOrWhiteSpace(PhoneNumberTextBox.Text))
                errors.Add("Phone Number");
            if (CountryComboBox.SelectedItem == null)
                errors.Add("Country");
            if (string.IsNullOrWhiteSpace(CityTextBox.Text))
                errors.Add("City");
            if (string.IsNullOrWhiteSpace(UniversityAutoSuggest.Text))
                errors.Add("University");
            if (GraduationYearComboBox.SelectedItem == null)
                errors.Add("Expected Graduation Year");

            return errors;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void UpdateProfileFromForm()
        {
            _userProfile.FirstName = FirstNameTextBox.Text.Trim();
            _userProfile.LastName = LastNameTextBox.Text.Trim();
            _userProfile.Age = (int)AgeNumberBox.Value;
            _userProfile.Gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
            _userProfile.Email = EmailTextBox.Text.Trim().ToLowerInvariant();

            var prefix = (PhonePrefixComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
            _userProfile.PhoneNumber = prefix + PhoneNumberTextBox.Text.Trim();

            _userProfile.GitHub = GitHubTextBox.Text.Trim();
            _userProfile.LinkedIn = LinkedInTextBox.Text.Trim();
            _userProfile.Country = (CountryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
            _userProfile.City = CityTextBox.Text.Trim();
            _userProfile.University = UniversityAutoSuggest.Text.Trim();
            _userProfile.ExpectedGraduationYear = int.Parse(GraduationYearComboBox.SelectedItem?.ToString() ?? "0");
            _userProfile.Address = AddressTextBox.Text.Trim();
            _userProfile.Motivation = MotivationTextBox.Text.Trim();

            // Update collections
            _userProfile.Skills = _skills.ToList();
            _userProfile.WorkExperiences = _workExperiences.ToList();
            _userProfile.Projects = _projects.ToList();

            // Update timestamp
            _userProfile.LastUpdated = DateTime.Now;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
            else
                Frame.Navigate(typeof(ViewProfilePage));
        }
    }

    // CV Structure classes for deserialization
    public class CVStructure
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string University { get; set; }
        public int ExpectedGraduationYear { get; set; }
        public string GitHub { get; set; }
        public string LinkedIn { get; set; }
        public string Address { get; set; }
        public string Motivation { get; set; }
        public List<string> Skills { get; set; }
        public List<WorkExperience> WorkExperiences { get; set; }
        public List<Project> Projects { get; set; }
        public List<ExtraCurricularActivity> ExtraCurricularActivities { get; set; }
    }
}
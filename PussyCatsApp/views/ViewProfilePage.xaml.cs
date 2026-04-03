using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.models;

namespace PussyCatsApp.views
{
    public sealed partial class ViewProfilePage : Page
    {
        private UserProfile _userProfile;

        public ViewProfilePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is UserProfile profile)
            {
                _userProfile = profile;
            }
            else
            {
                _userProfile = new UserProfile();
            }
            LoadProfileData();
        }

        private void LoadProfileData()
        {
            FirstNameText.Text = string.IsNullOrEmpty(_userProfile.FirstName) ? "\u2014" : _userProfile.FirstName;
            LastNameText.Text = string.IsNullOrEmpty(_userProfile.LastName) ? "\u2014" : _userProfile.LastName;
            AgeText.Text = _userProfile.Age > 0 ? _userProfile.Age.ToString() : "\u2014";
            GenderText.Text = string.IsNullOrEmpty(_userProfile.Gender) ? "\u2014" : _userProfile.Gender;
            EmailText.Text = string.IsNullOrEmpty(_userProfile.Email) ? "\u2014" : _userProfile.Email;
            PhoneText.Text = string.IsNullOrEmpty(_userProfile.PhoneNumber) ? "\u2014" : _userProfile.PhoneNumber;
            CountryText.Text = string.IsNullOrEmpty(_userProfile.Country) ? "\u2014" : _userProfile.Country;
            UniversityText.Text = string.IsNullOrEmpty(_userProfile.University) ? "\u2014" : _userProfile.University;
            GraduationYearText.Text = _userProfile.ExpectedGraduationYear > 0 ? _userProfile.ExpectedGraduationYear.ToString() : "\u2014";
            AddressText.Text = string.IsNullOrEmpty(_userProfile.Address) ? "\u2014" : _userProfile.Address;
            GitHubText.Text = string.IsNullOrEmpty(_userProfile.GitHub) ? "\u2014" : _userProfile.GitHub;
            LinkedInText.Text = string.IsNullOrEmpty(_userProfile.LinkedIn) ? "\u2014" : _userProfile.LinkedIn;
            MotivationText.Text = string.IsNullOrEmpty(_userProfile.Motivation) ? "No motivation provided." : _userProfile.Motivation;

            if (_userProfile.Skills != null && _userProfile.Skills.Count > 0)
            {
                SkillsRepeater.ItemsSource = _userProfile.Skills;
                NoSkillsText.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoSkillsText.Visibility = Visibility.Visible;
            }
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProfileFormView), _userProfile);
        }
    }
}
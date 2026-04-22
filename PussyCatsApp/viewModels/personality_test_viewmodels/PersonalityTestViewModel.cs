using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PussyCatsApp.Converters;
using PussyCatsApp.Models;
using PussyCatsApp.Services;

namespace PussyCatsApp.ViewModels
{
    /// <summary>
    /// ViewModel for managing the personality test workflow, including loading questions,
    /// handling user answers, submitting the test, and saving the selected role result.
    /// </summary>
    public partial class PersonalityTestViewModel : ObservableObject
    {
        private readonly IPersonalityTestService personalityTestService;
        private readonly int userId;

        public List<QuestionViewModel> Questions { get; }

        [ObservableProperty]
        public partial List<RoleResultViewModel> TopRoles { get; set; } = new ();

        [ObservableProperty]
        public partial RoleResultViewModel? SelectedRole { get; set; }

        [ObservableProperty]
        public partial string? SaveMessage { get; set; }

        [ObservableProperty]
        public partial bool IsTestSubmitted { get; set; }

        public bool CanSubmit => Questions.All(q => q.IsAnswered == true);

        public bool CanSave => SelectedRole != null;

        public PersonalityTestViewModel(int userId, IPersonalityTestService personalityTestService)
        {
            this.personalityTestService = personalityTestService;
            this.userId = userId;

            Questions = PersonalityTestService.LoadQuestions()
                .Select(question => new QuestionViewModel(question))
                .ToList();

            // Subscribe to answer changes to trigger CanSubmit validation
            // WHEN to re-evaluate the CanSubmit condition
            foreach (var question in Questions)
            {
                question.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(QuestionViewModel.SelectedAnswer))
                    {
                        SubmitCommand.NotifyCanExecuteChanged();
                    }
                };
            }
        }

        [RelayCommand(CanExecute = nameof(CanSubmit))]
        private void Submit()
        {
            // Collect answers from questions
            var answers = new Dictionary<Question, AnswerValue>();
            foreach (var questionVm in Questions)
            {
                answers[questionVm.Question] = (AnswerValue)questionVm.SelectedAnswer!;
            }

            // Calculate trait scores
            var traitScores = personalityTestService.CalculateTraitScores(answers);

            // Calculate role scores
            var roleScores = personalityTestService.CalculateRoleScores(traitScores);

            TopRoles = personalityTestService.GetTopRoles(roleScores, 3)
                .Select(role => new RoleResultViewModel(role.Key, role.Value))
                .ToList();

            // Mark test as submitted
            IsTestSubmitted = true;
        }

        [RelayCommand]
        private void SelectRole(RoleResultViewModel role)
        {
            // Deselect all roles
            foreach (var topRole in TopRoles)
            {
                topRole.IsSelected = false;
            }

            // Select the clicked role
            role.IsSelected = true;
            SelectedRole = role;

            // Notify SaveResultCommand that it can execute
            SaveResultCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanSave))]
        private void SaveResult()
        {
            if (SelectedRole != null)
            {
                personalityTestService.SaveResult(this.userId, SelectedRole.Role.ToString());

                // Convert role to a friendly display name using the existing converter
                var converter = new JobRoleToDisplayNameConverter();
                var displayNameObj = converter.Convert(SelectedRole.Role, typeof(string), null, "en-US");
                var displayName = displayNameObj as string ?? SelectedRole.Role.ToString();

                // Notify the view that the result was saved so it can display feedback
                SaveMessage = $"Your personality test result has been updated to {displayName}.";
            }
        }
    }
}

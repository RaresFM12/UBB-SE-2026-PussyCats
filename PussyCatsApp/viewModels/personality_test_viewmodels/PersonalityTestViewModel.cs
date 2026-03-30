/*using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PussyCatsApp.models;
using PussyCatsApp.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.viewModels
{
    public partial class PersonalityTestViewModel : ObservableObject
    {
        private readonly PersonalityTestService PersonalityTestService;
        private readonly int UserId;

        public List<QuestionViewModel> Questions { get; }

        [ObservableProperty]
        public partial List<RoleResultViewModel> TopRoles { get; set; } = new();

        [ObservableProperty]
        public partial RoleResultViewModel? SelectedRole { get; set; }

        [ObservableProperty]
        public partial bool IsTestSubmitted { get; set; }

        public bool CanSubmit => Questions.All(q => q.IsAnswered == true);

        public bool CanSave => SelectedRole != null;

        public PersonalityTestViewModel(PersonalityTestService service, int userId)
        {
            PersonalityTestService = service;
            UserId = userId;

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
                answers[questionVm.Question] = (AnswerValue)questionVm.SelectedAnswer!;

            // Calculate trait scores
            var traitScores = PersonalityTestService.CalculateTraitScores(answers);

            // Calculate role scores
            var roleScores = PersonalityTestService.CalculateRoleScores(traitScores);

            TopRoles = PersonalityTestService.GetTopRoles(roleScores, 3)
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
                topRole.IsSelected = false;

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
                PersonalityTestService.SaveResult(UserId, SelectedRole.Role.ToString());
        }
    }

}
*/
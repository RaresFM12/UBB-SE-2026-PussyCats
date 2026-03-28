using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PussyCatsApp.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

        public PersonalityTestViewModel(PersonalityTestService service, int userId)
        {
            PersonalityTestService = service;
            UserId = userId;

            Questions = PersonalityTestService.LoadQuestions()
                .Select(question => new QuestionViewModel(question))
                .ToList();
        }

        public bool CanSubmit => Questions.All(q => q.IsAnswered == true);

        public bool CanSave => SelectedRole != null;

        [RelayCommand]
        private void Submit()
        {
            // TODO
        }

        [RelayCommand]
        private void SelectRole(RoleResultViewModel role)
        {
            // TODO
        }

        [RelayCommand]
        private void SaveResult()
        {
            // TODO
        }
    }

}

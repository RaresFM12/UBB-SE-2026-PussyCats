using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PussyCatsApp.models;

namespace PussyCatsApp.viewModels
{
    public partial class QuestionViewModel : ObservableObject
    {
        public Question Question { get; }

        [ObservableProperty]
        public partial int? SelectedAnswer { get; set; }

        public bool IsAnswered => SelectedAnswer != null;

        public QuestionViewModel(Question question)
        {
            Question = question;
        }
    }
}

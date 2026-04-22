using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PussyCatsApp.Models;

namespace PussyCatsApp.ViewModels
{
    /// <summary>
    /// ViewModel for a single personality test question, tracking the question data and the user's selected answer.
    /// </summary>
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

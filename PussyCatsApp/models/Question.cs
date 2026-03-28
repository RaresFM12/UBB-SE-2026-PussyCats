using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.models
{
    public enum TraitType {ABSTRACTION, VISIBILITY, INTERACTION, DEPTH, CREATIVITY, PACE }
    public enum  JobRole { FrontendDeveloper, BackendDeveloper, UIUXDesigner, DevOpsEngineer, ProjectManager, DataAnalyst, CybersecuritySpecialist, AIMLEngineer }
    public enum AnswerValue { STRONGLY_DISAGREE = 1, DISAGREE = 2, NEUTRAL = 3, AGREE = 4, STRONGLY_AGREE = 5 }

    public class Question
    {

        private int Id { get; }
        private String QuestionText { get; }
        private TraitType Trait { get; }
        private int SortOrder { get; set; }

        public Question(int id, string questionText, TraitType trait, int sortOrder)
        {
            this.Id = id;
            this.QuestionText = questionText;
            this.Trait = trait;
            this.SortOrder = sortOrder;
        }
    }
}

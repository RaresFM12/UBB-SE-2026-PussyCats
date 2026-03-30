using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.models
{
    public class Question
    {
        public int Id { get; }
        public String QuestionText { get; }
        public TraitType Trait { get; }
        public int SortOrder { get; set; }

        public Question(int id, string questionText, TraitType trait, int sortOrder)
        {
            this.Id = id;
            this.QuestionText = questionText;
            this.Trait = trait;
            this.SortOrder = sortOrder;
        }
    }
}

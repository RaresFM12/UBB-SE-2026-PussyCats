using PussyCatsApp.models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.viewModels
{
    public class CompatibilityDetailViewModel : INotifyPropertyChanged
    {
        private RoleResult currentResult;
        private string errorMessage;

        public event PropertyChangedEventHandler PropertyChanged;

        public CompatibilityDetailViewModel() { }

        public void LoadResult(RoleResult result)
        {
            currentResult = result;
        }

        public double GetMatchScore()
        {
            return currentResult.MatchScore;
        }

        public string GetRoleName()
        {
            string formattedName = "";
            if (currentResult.JobRole == JobRole.UIUXDesigner)
                formattedName = "UI/UX Designer";
            else if (currentResult.JobRole == JobRole.AIMLEngineer)
                formattedName = "AI/ML Engineer";
            else
                 formattedName = currentResult.JobRole.ToString();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (char c in formattedName)
            {
                if (char.IsUpper(c) && sb.Length > 0)
                    sb.Append(' ');
                sb.Append(c);
            }
            return sb.ToString();
        }

        public List<Suggestion> GetSuggestions()
        {
            return currentResult.Suggestions;
        }

        public string GetErrorMessage()
        {
            return errorMessage;
        }
    }
}

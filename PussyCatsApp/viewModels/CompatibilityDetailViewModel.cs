using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models;
using PussyCatsApp.Utilities;

namespace PussyCatsApp.ViewModels
{
    public class CompatibilityDetailViewModel : INotifyPropertyChanged
    {
        private RoleResult currentRoleResult;
        private string errorMessage;

        public event PropertyChangedEventHandler PropertyChanged;

        public CompatibilityDetailViewModel()
        {
        }

        public void LoadResult(RoleResult result)
        {
            currentRoleResult = result;
        }

        public double GetMatchScore()
        {
            return currentRoleResult.MatchScore;
        }

        public string GetRoleName()
        {
            return Helpers.GetFormattedNameFromJobRole(currentRoleResult.JobRole);
        }

        public List<Suggestion> GetSuggestions()
        {
            return currentRoleResult.Suggestions;
        }

        public string GetErrorMessage()
        {
            return errorMessage;
        }
    }
}

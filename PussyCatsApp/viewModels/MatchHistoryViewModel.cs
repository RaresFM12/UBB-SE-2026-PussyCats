using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PussyCatsApp.Models;
using PussyCatsApp.services;

namespace PussyCatsApp.viewModels
{
    public class MatchHistoryViewModel
    {
        private List<Match> matches;
        private MatchStatistics statistics;
        private string errorMessage;
        private int currentUserId;
        private MatchService matchService; 

        public MatchHistoryViewModel(int userId)
        {
            matchService = new MatchService();
            currentUserId = userId;
            matches = new List<Match>();
            errorMessage = string.Empty;
        }

        public void LoadMatches()
        {
            errorMessage = string.Empty; 
            try
            {
                matches = matchService.GetMatchesForUser(currentUserId);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public void LoadStatistics()
        {
            errorMessage = string.Empty; 
            try
            {
                statistics = matchService.GetStatistics(currentUserId);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public List<Match> GetMatches()
        {
            return matches;
        }

        public MatchStatistics GetStatistics()
        {
            return statistics;
        }

        public string GetErrorMessage()
        {
            return errorMessage;
        }
    }
}

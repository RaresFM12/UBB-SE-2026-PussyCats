using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PussyCatsApp.models;
using PussyCatsApp.services;

namespace PussyCatsApp.viewModels
{
    public class MatchHistoryViewModel
    {
        // Private fields as requested
        private List<Match> matches;
        private MatchStatistics statistics;
        private string errorMessage;
        private int currentUserId;
        private MatchService matchService; // Change to IMatchService if you have an interface!

        // Constructor
        public MatchHistoryViewModel(MatchService service, int userId)
        {
            matchService = service;
            currentUserId = userId;
            matches = new List<Match>();
            errorMessage = string.Empty;
        }

        // Fetches the user's matchmaking history
        public void LoadMatches()
        {
            errorMessage = string.Empty; // Clear previous errors
            try
            {
                matches = matchService.GetMatchesForUser(currentUserId);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        // Fetches the calculated statistics for the user
        public void LoadStatistics()
        {
            errorMessage = string.Empty; // Clear previous errors
            try
            {
                statistics = matchService.GetStatistics(currentUserId);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        // Getters
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

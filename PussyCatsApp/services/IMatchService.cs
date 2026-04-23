using System.Collections.Generic;

using PussyCatsApp.Models;

namespace PussyCatsApp.Services
{
    public interface IMatchService
    {
        List<Match> GetMatchesForUser(int userId);

        MatchStatistics GetMatchStatistics(int userId);
    }
}
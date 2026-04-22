using System.Collections.Generic;

using PussyCatsApp.Models;

namespace PussyCatsApp.services
{
    public interface IMatchService
    {
        List<Match> GetMatchesForUser(int userId);

        MatchStatistics GetMatchStatistics(int userId);
    }
}
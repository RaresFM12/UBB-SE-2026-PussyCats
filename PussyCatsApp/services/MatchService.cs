using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PussyCatsApp.models;
using PussyCatsApp.repositories;

namespace PussyCatsApp.services
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository _matchRepository;

        public MatchService(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        private int CountMatchesInLastMonths(List<Match> matches, int months)
        {
            DateTime cutoffDate = DateTime.Now.AddMonths(-months);
            int count = 0;

            foreach (var match in matches)
            {
                if (match.MatchDate > cutoffDate)
                {
                    count++;
                }
            }

            return count;
        }

        private Dictionary<string, int> GroupMatchesByPosition(List<Match> matches)
        {
            var positionCounts = new Dictionary<string, int>();

            foreach (var match in matches)
            {
                if (positionCounts.ContainsKey(match.JobRole))
                {
                    positionCounts[match.JobRole]++;
                }
                else
                {
                    positionCounts.Add(match.JobRole, 1);
                }
            }

            return positionCounts;
        }

        public List<Match> GetMatchesForUser(int userId)
        {
            return _matchRepository.GetByUserId(userId);
        }

        public MatchStatistics GetMatchStatistics(int userId)
        {
            var matches = _matchRepository.GetByUserId(userId);
            var stats = new MatchStatistics();

            stats.TotalMatches = matches.Count;
            stats.MatchesLastMonth = CountMatchesInLastMonths(matches, 1);
            stats.MatchesLastSixMonths = CountMatchesInLastMonths(matches, 6);
            stats.MatchesLastYear = CountMatchesInLastMonths(matches, 12);
            stats.MatchesPerPosition = GroupMatchesByPosition(matches);

            return stats;
        }
    }
}
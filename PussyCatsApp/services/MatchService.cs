using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PussyCatsApp.models;
using PussyCatsApp.repositories;

namespace PussyCatsApp.services
{
    public class MatchService
    {
        private readonly MatchRepository _matchRepository;

        // Constructor to receive and store the repository
        public MatchService(MatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        // Calculates how many matches occurred within the last X months
        private int CountByPeriod(List<Match> matches, int months)
        {
            DateTime cutoffDate = DateTime.Now.AddMonths(-months);
            int count = 0;

            foreach (var match in matches)
            {
                // If the match date is strictly greater (more recent) than the cutoff
                if (match.MatchDate > cutoffDate)
                {
                    count++;
                }
            }

            return count;
        }

        // Groups matches by job role and counts them
        private Dictionary<string, int> GroupByPosition(List<Match> matches)
        {
            var positionCounts = new Dictionary<string, int>();

            foreach (var match in matches)
            {
                if (positionCounts.ContainsKey(match.JobRole))
                {
                    // Key exists, increment the count
                    positionCounts[match.JobRole]++;
                }
                else
                {
                    // New key, add it with a count of 1
                    positionCounts.Add(match.JobRole, 1);
                }
            }

            return positionCounts;
        }

        // Retrieves the raw list of matches for the user directly from the repo
        public List<Match> GetMatchesForUser(int userId)
        {
            return _matchRepository.GetByUserId(userId);
        }

        // Generates the complete statistics object for the user's dashboard
        public MatchStatistics GetStatistics(int userId)
        {
            var matches = _matchRepository.GetByUserId(userId);
            var stats = new MatchStatistics();

            stats.TotalMatches = matches.Count;
            stats.MatchesLastMonth = CountByPeriod(matches, 1);
            stats.MatchesLastSixMonths = CountByPeriod(matches, 6);
            stats.MatchesLastYear = CountByPeriod(matches, 12);
            stats.MatchesPerPosition = GroupByPosition(matches);

            return stats;
        }
    }
}
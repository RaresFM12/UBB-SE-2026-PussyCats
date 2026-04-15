using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;

namespace PussyCatsApp.Services
{
    public class MatchService
    {
        private readonly MatchRepository matchRepository;

        public MatchService()
        {
            matchRepository = new MatchRepository();
        }

        private int CountByPeriod(List<Match> matches, int months)
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

        private Dictionary<string, int> GroupByPosition(List<Match> matches)
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
            return matchRepository.GetByUserId(userId);
        }

        public MatchStatistics GetStatistics(int userId)
        {
            var matches = matchRepository.GetByUserId(userId);
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
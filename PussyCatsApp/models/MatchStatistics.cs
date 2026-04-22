using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.Models
{
    public class MatchStatistics
    {
        private int totalMatches;
        private Dictionary<string, int> matchesPerPosition = new Dictionary<string, int>();
        private int matchesLastMonth;
        private int matchesLastSixMonths;
        private int matchesLastYear;

        public int TotalMatches
        {
            get { return totalMatches; }
            set { totalMatches = value; }
        }

        public Dictionary<string, int> MatchesPerPosition
        {
            get { return matchesPerPosition; }
            set { matchesPerPosition = value; }
        }

        public int MatchesLastMonth
        {
            get { return matchesLastMonth; }
            set { matchesLastMonth = value; }
        }

        public int MatchesLastSixMonths
        {
            get { return matchesLastSixMonths; }
            set { matchesLastSixMonths = value; }
        }

        public int MatchesLastYear
        {
            get { return matchesLastYear; }
            set { matchesLastYear = value; }
        }
    }
}

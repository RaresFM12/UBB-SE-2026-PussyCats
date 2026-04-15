using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.Models
{
    public class RoleResult
    {
        private JobRole jobRole;
        private double matchScore;
        private List<Suggestion> suggestions;

        public JobRole JobRole
        {
            get { return jobRole; } set { jobRole = value; }
        }
        public double MatchScore
        {
            get { return matchScore; } set { matchScore = value; }
        }
        public List<Suggestion> Suggestions
        {
            get { return suggestions; } set { suggestions = value; }
        }
    }
}

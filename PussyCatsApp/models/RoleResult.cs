using System.Collections.Generic;

namespace PussyCatsApp.Models
{
    public class RoleResult
    {
        public JobRole JobRole { get; set; }
        public double MatchScore { get; set; }
        public List<Suggestion> Suggestions { get; set; }
    }
}

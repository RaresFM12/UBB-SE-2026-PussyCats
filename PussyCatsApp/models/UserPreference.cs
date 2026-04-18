using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.Models
{
    public class UserPreference
    {
        public int PId { get; set; }
        public int UserId { get; set; }
        public string PreferenceType { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.models;

namespace PussyCatsApp.services
{
    public interface IPreferenceService
    {
        List<Preference> GetByUserId(int userId);
        void SavePreferences(int userId, List<JobRole> roles, WorkMode workMode, string location);
        List<string> SearchLocations(string query);
    }
}

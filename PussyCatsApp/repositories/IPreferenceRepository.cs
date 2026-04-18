using System.Collections.Generic;
using PussyCatsApp.Models;

namespace PussyCatsApp.repositories
{
    internal interface IPreferenceRepository : IRepository<Preference>
    {
        List<Preference> GetPreferencesByUserId(int userId);
        void AddPreference(Preference preference);
        void RemovePreference(int preferenceId);
        void UpdatePreference(int preferenceId, string value);
    }
}

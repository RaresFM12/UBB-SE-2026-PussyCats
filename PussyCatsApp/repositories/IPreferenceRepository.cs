using System.Collections.Generic;
using PussyCatsApp.models;

namespace PussyCatsApp.Repositories
{
    internal interface IPreferenceRepository : IRepository<Preference>
    {
        List<Preference> GetPreferencesByUserId(int userId);
        void AddPreference(Preference preference);
        void RemovePreference(int preferenceId);
        void UpdatePreference(int preferenceId, string value);
    }
}

using System.Collections.Generic;
using PussyCatsApp.Models;

namespace PussyCatsApp.Repositories
{
    public interface IPreferenceRepository : IRepository<Preference>
    {
        List<Preference> GetPreferencesByUserId(int userId);
        void AddPreference(Preference preference);
        void RemovePreference(int preferenceId);
        void UpdatePreference(int preferenceId, string value);

        void DeleteAllByUserId(int userId);
    }
}

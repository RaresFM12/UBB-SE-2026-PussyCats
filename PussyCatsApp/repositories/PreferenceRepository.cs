using System.Collections.Generic;
using System.Linq;
using PussyCatsApp.models;

namespace PussyCatsApp.repositories
{
    internal class PreferenceRepository : IPreferenceRepository
    {
        private readonly List<Preference> _preferences = new();

        public Preference load(int id)
        {
            return _preferences.FirstOrDefault(p => p.PreferenceId == id);
        }

        public void save(int id, Preference data)
        {
            var existing = _preferences.FirstOrDefault(p => p.PreferenceId == id);
            if (existing != null)
            {
                existing.PreferenceType = data.PreferenceType;
                existing.Value = data.Value;
                existing.UserId = data.UserId;
            }
            else
            {
                data.PreferenceId = id;
                _preferences.Add(data);
            }
        }

        public List<Preference> GetPreferencesByUserId(int userId)
        {
            return _preferences.Where(p => p.UserId == userId).ToList();
        }

        public void AddPreference(Preference preference)
        {
            if (preference.PreferenceId == 0)
            {
                preference.PreferenceId = _preferences.Count > 0 ? _preferences.Max(p => p.PreferenceId) + 1 : 1;
            }
            _preferences.Add(preference);
        }

        public void RemovePreference(int preferenceId)
        {
            var pref = _preferences.FirstOrDefault(p => p.PreferenceId == preferenceId);
            if (pref != null)
            {
                _preferences.Remove(pref);
            }
        }

        public void UpdatePreference(int preferenceId, string value)
        {
            var pref = _preferences.FirstOrDefault(p => p.PreferenceId == preferenceId);
            if (pref != null)
            {
                pref.Value = value;
            }
        }
    }
}

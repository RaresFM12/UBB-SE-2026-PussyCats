using PussyCatsApp.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Repositories;
namespace PussyCatsApp.services
{
    public class PreferenceService: IPreferenceService
    {
        private readonly IPreferenceRepository _preferencesRepository;
        private List<string> _predefinedLocations = new List<string>();

        public PreferenceService(IPreferenceRepository preferenceRepository)
        {
            _preferencesRepository = preferenceRepository;
            LoadPredefinedLocations();
        }

        private void LoadPredefinedLocations()
        {
            _predefinedLocations = new List<string>
            {
                "Bucharest, Romania", "Cluj-Napoca, Romania", "Timisoara, Romania", "Iasi, Romania", "Brasov, Romania",
                "London, UK", "Manchester, UK", "Birmingham, UK", "Edinburgh, UK", "Glasgow, UK",
                "Berlin, Germany", "Munich, Germany", "Frankfurt, Germany", "Hamburg, Germany", "Stuttgart, Germany",
                "Paris, France", "Lyon, France", "Marseille, France", "Toulouse, France", "Nice, France"
            };
        }

        private bool ValidateRoleLimit(List<JobRole> roles)
        {
            if (roles == null) return false;
            return roles.Count >= 1 && roles.Count <= 3;
        }

        private List<Preference> BuildPreferenceRows(int userId, List<JobRole> roles, WorkMode workMode, string location)
        {
            var rows = new List<Preference>();

            foreach (var role in roles)
            {
                rows.Add(new Preference
                {
                    UserId = userId,
                    PreferenceType = "JobRole",
                    Value = role.ToString()
                });
            }

            rows.Add(new Preference
            {
                UserId = userId,
                PreferenceType = "WorkMode",
                Value = workMode.ToString()
            });

            rows.Add(new Preference
            {
                UserId = userId,
                PreferenceType = "Location",
                Value = location
            });

            return rows;
        }

        public List<Preference> GetByUserId(int userId)
        {
            return _preferencesRepository.GetPreferencesByUserId(userId);
        }

        public void SavePreferences(int userId, List<JobRole> roles, WorkMode workMode, string location)
        {
            if (!ValidateRoleLimit(roles))
            {
                throw new ArgumentException("You must select between 1 and 3 job roles.");
            }

            var rowsToInsert = BuildPreferenceRows(userId, roles, workMode, location);

            _preferencesRepository.DeleteAllByUserId(userId);

            foreach (var row in rowsToInsert)
            {
                _preferencesRepository.AddPreference(row);
            }
        }
        public List<string> SearchLocations(string query)
        {
            var result = new List<string>();

            if (string.IsNullOrWhiteSpace(query))
                return result;

            foreach (var loc in _predefinedLocations)
            {
                if (loc.Contains(query, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(loc);
                }
            }

            return result;
        }
        /*
        public List<string> SearchLocations(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<string>();
            }

            return _predefinedLocations
                .Where(loc => loc.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }*/
    }
}

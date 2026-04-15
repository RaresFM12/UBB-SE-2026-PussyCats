using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;

namespace PussyCatsApp.ViewModels
{
    public class PreferencesViewModel
    {
        private List<JobRole> selectedJobRoles;
        private WorkMode selectedWorkMode;
        private string preferredLocation;
        private List<string> locationSuggestions;
        private string errorMessage;
        private int currentUserId;
        private PreferenceService preferencesService;

        public PreferencesViewModel(int userId)
        {
            string connectionString = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build()
                .GetConnectionString("raresConnectionString");

            var repository = new PreferenceRepository(connectionString);
            preferencesService = new PreferenceService(repository);

            currentUserId = userId;
            selectedJobRoles = new List<JobRole>();
            locationSuggestions = new List<string>();
            preferredLocation = string.Empty;
            errorMessage = string.Empty;
        }

        public string GetPreferredLocation()
        {
            return preferredLocation;
        }

        public void LoadPreferences()
        {
            selectedJobRoles.Clear();
            errorMessage = string.Empty;

            var savedPreferences = preferencesService.GetByUserId(currentUserId);

            foreach (var pref in savedPreferences)
            {
                if (pref.PreferenceType == "JobRole")
                {
                    if (Enum.TryParse<JobRole>(pref.Value, out var role))
                    {
                        selectedJobRoles.Add(role);
                    }
                }
                else if (pref.PreferenceType == "WorkMode")
                {
                    if (Enum.TryParse<WorkMode>(pref.Value, out var mode))
                    {
                        selectedWorkMode = mode;
                    }
                }
                else if (pref.PreferenceType == "Location")
                {
                    preferredLocation = pref.Value;
                }
            }
        }

        public void ToggleJobRole(JobRole role)
        {
            errorMessage = string.Empty; // Clear any previous errors

            if (selectedJobRoles.Contains(role))
            {
                selectedJobRoles.Remove(role);
            }
            else
            {
                if (selectedJobRoles.Count < 3)
                {
                    selectedJobRoles.Add(role);
                }
                else
                {
                    errorMessage = "You can select a maximum of 3 job roles.";
                }
            }
        }

        public void SetWorkMode(WorkMode mode)
        {
            selectedWorkMode = mode;
        }

        public void SetLocation(string location)
        {
            preferredLocation = location;
        }

        public void SearchLocation(string query)
        {
            locationSuggestions = preferencesService.SearchLocations(query);
        }

        public void SavePreferences()
        {
            errorMessage = string.Empty;
            try
            {
                preferencesService.SavePreferences(currentUserId, selectedJobRoles, selectedWorkMode, preferredLocation);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public List<JobRole> GetSelectedJobRoles()
        {
            return selectedJobRoles;
        }

        public WorkMode GetSelectedWorkMode()
        {
            return selectedWorkMode;
        }

        public List<string> GetLocationSuggestions()
        {
            return locationSuggestions;
        }

        public string GetErrorMessage()
        {
            return errorMessage;
        }
    }
}

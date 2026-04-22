using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using PussyCatsApp.Models;

namespace PussyCatsApp.Repositories
{
    public class PreferenceRepository : IPreferenceRepository
    {
        private readonly string connectionString;

        public PreferenceRepository(string connectionStringArgument)
        {
            connectionString = connectionStringArgument;
        }

        public List<Preference> GetPreferencesByUserId(int userId)
        {
            var preferences = new List<Preference>();
            string query = "SELECT pID, userID, preferanceType, value FROM PREFERENCES WHERE userID = @UserId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var pref = new Preference();
                            pref.PreferenceId = Convert.ToInt32(reader["pID"]);
                            pref.UserId = Convert.ToInt32(reader["userID"]);
                            pref.PreferenceType = reader["preferanceType"].ToString();
                            pref.Value = reader["value"].ToString();

                            preferences.Add(pref);
                        }
                    }
                }
            }
            return preferences;
        }

        public void AddPreference(Preference preference)
        {
            string query = "INSERT INTO PREFERENCES (userID, preferanceType, value) VALUES (@UserId, @PreferenceType, @Value)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", preference.UserId);
                    command.Parameters.AddWithValue("@PreferenceType", preference.PreferenceType);
                    command.Parameters.AddWithValue("@Value", preference.Value);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void RemovePreference(int preferenceId)
        {
            string query = "DELETE FROM PREFERENCES WHERE pID = @PId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PId", preferenceId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteAllByUserId(int userId)
        {
            string query = "DELETE FROM PREFERENCES WHERE userID = @UserId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdatePreference(int preferenceId, string value)
        {
            throw new NotImplementedException("Use DeleteAllByUserId and AddPreference instead.");
        }

        public Preference Load(int id)
        {
            throw new NotImplementedException();
        }

        public void Save(int id, Preference data)
        {
            throw new NotImplementedException();
        }
    }
}
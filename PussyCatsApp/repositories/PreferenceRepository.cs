using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using PussyCatsApp.Models;

namespace PussyCatsApp.Repositories
{
    public class PreferenceRepository : IPreferenceRepository
    {
        private readonly string connectionString;

        public PreferenceRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Preference> GetPreferencesByUserId(int userId)
        {
            var preferences = new List<Preference>();
            string query = "SELECT pID, userID, preferanceType, value FROM PREFERENCES WHERE userID = @userId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
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
            string query = "INSERT INTO PREFERENCES (userID, preferanceType, value) VALUES (@userId, @PreferenceType, @Value)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", preference.UserId);
                    cmd.Parameters.AddWithValue("@PreferenceType", preference.PreferenceType);
                    cmd.Parameters.AddWithValue("@Value", preference.Value);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void RemovePreference(int preferenceId)
        {
            string query = "DELETE FROM PREFERENCES WHERE pID = @PId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PId", preferenceId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteAllByUserId(int userId)
        {
            string query = "DELETE FROM PREFERENCES WHERE userID = @userId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
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
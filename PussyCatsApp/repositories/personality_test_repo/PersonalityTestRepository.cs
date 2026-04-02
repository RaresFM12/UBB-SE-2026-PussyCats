using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.repositories.personality_test_repo
{
    public class PersonalityTestRepository : IPersonalityTestRepository
    {
        private string ConnectionString = "Data Source=.;Initial Catalog=PussyCatsDB;Integrated Security=True;Trust Server Certificate=True";

        public String? load(int id)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT personalityTestResult FROM Users WHERE userID = @userID", connection))
                    {
                        command.Parameters.AddWithValue("@userID", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                return reader["personalityTestResult"].ToString();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Debug.WriteLine($"Database error: {ex.Message}");
                }
            }

            return null;
        }

        public void save(int id, string personalityTestResult)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("UPDATE Users SET personalityTestResult = @personalityTestResult WHERE userID = @userID", connection))
                    {
                        command.Parameters.AddWithValue("@personalityTestResult", personalityTestResult);
                        command.Parameters.AddWithValue("@userID", id);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    Debug.WriteLine($"Database error: {ex.Message}");
                }
            }
        }
    }
}

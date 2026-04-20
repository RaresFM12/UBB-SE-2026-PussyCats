using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PussyCatsApp.Configuration;

namespace PussyCatsApp.Repositories.PersonalityTestRepo;

public class PersonalityTestRepository : IPersonalityTestRepository
{
    private readonly string connectionString = DatabaseConfiguration.GetConnectionString();

    public string? Load(int id)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
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
                        {
                            return reader["personalityTestResult"].ToString();
                        }
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

    public void Save(int id, string personalityTestResult)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using PussyCatsApp.models;

namespace PussyCatsApp.repositories
{
    public class MatchRepository
    {
        private readonly string _connectionString;

        // Constructor to receive and store the SQL connection string
        public MatchRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Retrieves all matchmaking history for a specific user
        public List<Match> GetByUserId(int userId)
        {
            var matches = new List<Match>();

            // The SQL query strictly matches the MATCHES table schema we created earlier
            string query = "SELECT id, userID, companyName, jobRole, matchDate FROM MATCHES WHERE userID = @UserId ORDER BY matchDate DESC";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Using parameters prevents SQL injection attacks
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var match = new Match();

                            // Map the database columns to the Match model properties
                            match.Id = Convert.ToInt32(reader["id"]);
                            match.UserId = Convert.ToInt32(reader["userID"]);
                            match.CompanyName = reader["companyName"].ToString();
                            match.JobRole = reader["jobRole"].ToString();
                            match.MatchDate = Convert.ToDateTime(reader["matchDate"]);

                            matches.Add(match);
                        }
                    }
                }
            }

            return matches;
        }
    }
}

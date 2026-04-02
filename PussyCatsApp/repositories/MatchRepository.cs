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
        private const string _connectionString = "Data Source=DESKTOP-LBK0E96\\SQLEXPRESS;Initial Catalog=PussyCatsDB;Integrated Security=True;Trust Server Certificate=True;";

        public MatchRepository()
        {}

        public List<Match> GetByUserId(int userId)
        {
            var matches = new List<Match>();

            string query = "SELECT id, userID, companyName, jobRole, matchDate FROM MATCHES WHERE userID = @UserId ORDER BY matchDate DESC";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var match = new Match();

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

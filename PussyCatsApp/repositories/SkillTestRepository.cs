using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PussyCatsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PussyCatsApp.repositories
{
    public class SkillTestRepository : ISkillTestRepository
    {
        private SqlConnection sqlConnection;
        private readonly string connectionString = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build().GetConnectionString("raresConnectionString");

        public SkillTestRepository()
        {
            sqlConnection = new SqlConnection(connectionString);
        }

        public SkillTest load(int skillId)
        {
            string query = "SELECT * FROM SKILLS WHERE skillID = @id";
            using SqlCommand cmd = new SqlCommand(query, sqlConnection);
            cmd.Parameters.AddWithValue("@id", skillId);
            sqlConnection.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var test = new SkillTest(
                    skillTestId: (int)reader["skillID"],
                    userId: (int)reader["userID"],
                    testName: reader["name"].ToString(),
                    testScore: (int)reader["score"],
                    achievedDate: reader["achievedDate"] != DBNull.Value ? DateOnly.FromDateTime((DateTime)reader["achievedDate"]) : default
                );
                sqlConnection.Close();
                return test;
            }
            sqlConnection.Close();
            throw new Exception($"SkillTest with ID {skillId} not found.");
        }


        public void save(int skillId, SkillTest data)
        {
            string query = "UPDATE SKILLS SET score = @score, achievedDate = @date WHERE skillID = @id";
            using SqlCommand cmd = new SqlCommand(query, sqlConnection);
            cmd.Parameters.AddWithValue("@score", data.Score);
            cmd.Parameters.AddWithValue("@date", data.AchievedDate); 
            cmd.Parameters.AddWithValue("@id", skillId);
            sqlConnection.Open();
            cmd.ExecuteNonQuery();
            sqlConnection.Close();
        }
        public List<SkillTest> GetSkillTestsByUserId(int userId)
        {
            var tests = new List<SkillTest>();
            string query = "SELECT * FROM SKILLS WHERE userID = @userId";
            using SqlCommand cmd = new SqlCommand(query, sqlConnection);
            cmd.Parameters.AddWithValue("@userId", userId);

            sqlConnection.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tests.Add(new SkillTest(
                    skillTestId: (int)reader["skillID"],
                    userId: (int)reader["userID"],
                    testName: reader["name"].ToString(),
                    testScore: (int)reader["score"],
                    achievedDate: reader["achievedDate"] != DBNull.Value ? DateOnly.FromDateTime((DateTime)reader["achievedDate"]) : default
                ));
            }
            sqlConnection.Close();
            return tests;
        }

        public void UpdateSkillTestScore(int skillId, int score)
        {
            string query = "UPDATE SKILLS SET score = @score WHERE skillID = @id";
            using SqlCommand cmd = new SqlCommand(query, sqlConnection);
            cmd.Parameters.AddWithValue("@score", score);
            cmd.Parameters.AddWithValue("@id", skillId);
            sqlConnection.Open();
            cmd.ExecuteNonQuery();
            sqlConnection.Close();
        }

        public void UpdateAchievedDate(int skillId, DateOnly date)
        {
            string query = "UPDATE SKILLS SET achievedDate = @date WHERE skillID = @id";
            using SqlCommand cmd = new SqlCommand(query, sqlConnection);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.Parameters.AddWithValue("@id", skillId);
            sqlConnection.Open();
            cmd.ExecuteNonQuery();
            sqlConnection.Close();
        }
    }


}


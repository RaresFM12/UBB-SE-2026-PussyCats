using Microsoft.Data.SqlClient;
using PussyCatsApp.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PussyCatsApp.repositories
{
    internal class SkillRepository : ISkillRepository
    {
        private SqlConnection sqlConnection;

        public SkillRepository(SqlConnection connection)
        {
            sqlConnection = connection;
        }

        public Skill load(int id)
        {
            string query = "SELECT * FROM SKILLS WHERE skillID = @id";
            using SqlCommand cmd = new SqlCommand(query, sqlConnection);
            cmd.Parameters.AddWithValue("@id", id);
            sqlConnection.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var test = new Skill(
                    skillId: (int)reader["skillID"],
                    userId: (int)reader["userID"],
                    name: reader["name"].ToString(),
                    score: (int)reader["score"],
                    achievedDate: DateOnly.FromDateTime((DateTime)reader["lastAttemptDate"])
                );
                sqlConnection.Close();
                return test;
            }
            sqlConnection.Close();
            throw new Exception($"Skill with ID {id} not found.");
        }


        public void save(int id, Skill data)
        {
            string query = "UPDATE SKILLS SET score = @score, achievedDate = @date WHERE skillID = @id";
            using SqlCommand cmd = new SqlCommand(query, sqlConnection);
            cmd.Parameters.AddWithValue("@score", data.Score);
            cmd.Parameters.AddWithValue("@date", data.AchievedDate); 
            cmd.Parameters.AddWithValue("@id", id);
            sqlConnection.Open();
            cmd.ExecuteNonQuery();
            sqlConnection.Close();
        }
        public List<Skill> GetSkillsByUserId(int userId)
        {
            var tests = new List<Skill>();
            string query = "SELECT * FROM SKILLS WHERE userID = @userId";
            using SqlCommand cmd = new SqlCommand(query, sqlConnection);
            cmd.Parameters.AddWithValue("@userId", userId);

            sqlConnection.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tests.Add(new Skill(
                    skillId: (int)reader["skillID"],
                    userId: (int)reader["userID"],
                    name: reader["name"].ToString(),
                    score: (int)reader["score"],
                    achievedDate: DateOnly.FromDateTime((DateTime)reader["achievedDate"])
                ));
            }
            sqlConnection.Close();
            return tests;
        }

        public void UpdateSkillScore(int id, int score)
        {
            string query = "UPDATE SKILLS SET score = @score WHERE skillID = @id";
            using SqlCommand cmd = new SqlCommand(query, sqlConnection);
            cmd.Parameters.AddWithValue("@score", score);
            cmd.Parameters.AddWithValue("@id", id);
            sqlConnection.Open();
            cmd.ExecuteNonQuery();
            sqlConnection.Close();
        }

        public void UpdateAchievedDate(int id, DateOnly date)
        {
            string query = "UPDATE SKILLS SET achievedDate = @date WHERE skillID = @id";
            using SqlCommand cmd = new SqlCommand(query, sqlConnection);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.Parameters.AddWithValue("@id", id);
            sqlConnection.Open();
            cmd.ExecuteNonQuery();
            sqlConnection.Close();
        }
    }


}


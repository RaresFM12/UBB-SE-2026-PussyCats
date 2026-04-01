using Microsoft.Data.SqlClient;
using PussyCatsApp.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PussyCatsApp.repositories
{
    public class SkillTestRepository : ISkillTestRepository
    {
        private SqlConnection sqlConnection;
        //private const string connectionString = "Data Source=DESKTOP-SCP6QST;Initial Catalog=UserManagementDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30";
        private const string connectionString = "Data Source=DESKTOP-C5LH746\\SQLEXPRESS;Initial Catalog=PussyCatsDB;Integrated Security=True;Trust Server Certificate=True";


        public SkillTestRepository()
        {
            sqlConnection = new SqlConnection(connectionString);
        }

        public SkillTest load(int id)
        {
            string query = "SELECT * FROM SKILLS WHERE skillID = @id";
            using SqlCommand cmd = new SqlCommand(query, sqlConnection);
            cmd.Parameters.AddWithValue("@id", id);
            sqlConnection.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var test = new SkillTest(
                    skillId: (int)reader["skillID"],
                    userId: (int)reader["userID"],
                    name: reader["name"].ToString(),
                    score: (int)reader["score"],
                    achievedDate: reader["achievedDate"] != DBNull.Value ? DateOnly.FromDateTime((DateTime)reader["achievedDate"]) : default
                );
                sqlConnection.Close();
                return test;
            }
            sqlConnection.Close();
            throw new Exception($"SkillTest with ID {id} not found.");
        }


        public void save(int id, SkillTest data)
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
                    skillId: (int)reader["skillID"],
                    userId: (int)reader["userID"],
                    name: reader["name"].ToString(),
                    score: (int)reader["score"],
                    achievedDate: reader["achievedDate"] != DBNull.Value ? DateOnly.FromDateTime((DateTime)reader["achievedDate"]) : default
                ));
            }
            sqlConnection.Close();
            return tests;
        }

        public void UpdateSkillTestScore(int id, int score)
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


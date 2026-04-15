using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PussyCatsApp.Models;

namespace PussyCatsApp.Repositories
{
    public class UserSkillRepository
    {
        private readonly string connectionString =
            new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build()
                .GetConnectionString("raresConnectionString");

        public UserSkillRepository()
        {
        }

        public List<UserSkill> GetVerifiedSkillsByUserId(int userId)
        {
            List<UserSkill> skills = new List<UserSkill>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT name, score FROM SKILLS WHERE userID = @userId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    UserSkill skill = new UserSkill
                    {
                        SkillName = reader["name"].ToString(),
                        IsVerified = true,
                        Score = (int)reader["score"]
                    };

                    skills.Add(skill);
                }
            }

            return skills;
        }

        public string GetParsedCvByUserId(int userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT ParsedCV FROM Users WHERE userID = @userId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);

                object result = command.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                {
                    return null;
                }
                return result.ToString();
            }
        }
    }
}
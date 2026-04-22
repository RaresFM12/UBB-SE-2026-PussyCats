using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using PussyCatsApp.Configuration;
using PussyCatsApp.Models;

namespace PussyCatsApp.Repositories
{
    public class SkillTestRepository : ISkillTestRepository
    {
        private readonly string connectionString = DatabaseConfiguration.GetConnectionString();

        public SkillTest Load(int skillId)
        {
            const string query = "SELECT * FROM SKILLS WHERE skillID = @id";

            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", skillId);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new SkillTest(
                        skillTestId: (int)reader["skillID"],
                        userId: (int)reader["userID"],
                        testName: reader["name"].ToString(),
                        testScore: (int)reader["score"],
                        achievedDate: reader["achievedDate"] != DBNull.Value
                            ? DateOnly.FromDateTime((DateTime)reader["achievedDate"])
                            : default);
                }
            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine($"Database error loading skill {skillId}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred loading skill {skillId}: {ex.Message}");
            }

            throw new Exception($"SkillTest with ID {skillId} not found.");
        }

        public void Save(int skillId, SkillTest data)
        {
            const string query = "UPDATE SKILLS SET score = @score, achievedDate = @date WHERE skillID = @id";

            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@score", data.Score);
                command.Parameters.AddWithValue("@date", data.AchievedDate);
                command.Parameters.AddWithValue("@id", skillId);

                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine($"Database error saving skill {skillId}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred saving skill {skillId}: {ex.Message}");
            }
        }

        public List<SkillTest> GetSkillTestsByUserId(int userId)
        {
            var tests = new List<SkillTest>();
            const string query = "SELECT * FROM SKILLS WHERE userID = @userId";

            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tests.Add(new SkillTest(
                        skillTestId: (int)reader["skillID"],
                        userId: (int)reader["userID"],
                        testName: reader["name"].ToString(),
                        testScore: (int)reader["score"],
                        achievedDate: reader["achievedDate"] != DBNull.Value
                            ? DateOnly.FromDateTime((DateTime)reader["achievedDate"])
                            : default));
                }
            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine($"Database error retrieving skills for user {userId}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred retrieving skills for user {userId}: {ex.Message}");
            }

            return tests;
        }

        public void UpdateSkillTestScore(int skillId, int score)
        {
            const string query = "UPDATE SKILLS SET score = @score WHERE skillID = @id";

            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@score", score);
                command.Parameters.AddWithValue("@id", skillId);

                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine($"Database error updating score for skill {skillId}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred updating score for skill {skillId}: {ex.Message}");
            }
        }

        public void UpdateAchievedDate(int skillId, DateOnly date)
        {
            const string query = "UPDATE SKILLS SET achievedDate = @date WHERE skillID = @id";

            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@date", date);
                command.Parameters.AddWithValue("@id", skillId);

                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine($"Database error updating achieved date for skill {skillId}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred updating achieved date for skill {skillId}: {ex.Message}");
            }
        }
    }
}
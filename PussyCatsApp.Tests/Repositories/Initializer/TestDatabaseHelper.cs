// Infrastructure/TestDatabaseHelper.cs
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;

namespace PussyCatsApp.Tests.Infrastructure
{
    public static class TestDatabaseHelper
    {
        // Reads from appsettings.test.json — always PussyCatsTestsDB, DO NOT USE THE APPLICATION DATABASE
        public static string ConnectionString => new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.test.json", optional: false)
            .Build()
            .GetConnectionString("testConnectionString");


        // Clear all tables before each test runs.
        public static void ClearAllTables()
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();

            const string script = @"
                DELETE FROM MATCHES;
                DELETE FROM SKILLS;
                DELETE FROM DOCUMENTS;
                DELETE FROM PREFERENCES;
                DELETE FROM USERS;

                DBCC CHECKIDENT ('MATCHES',      RESEED, 0);
                DBCC CHECKIDENT ('SKILLS',       RESEED, 0);
                DBCC CHECKIDENT ('DOCUMENTS',    RESEED, 0);
                DBCC CHECKIDENT ('PREFERENCES',  RESEED, 0);
                DBCC CHECKIDENT ('USERS',        RESEED, 0);";

            using var cmd = new SqlCommand(script, connection);
            cmd.ExecuteNonQuery();
        }


        public static int InsertUser(
            string firstName = "Test",
            string lastName = "User",
            string email = null,
            int age = 20,
            string gender = "Female",
            string parsedCv = null,
            string personalityTestResult = null,
            bool activeAccount = true,
            string profilePicture = null,
            string city = null,
            string university = null,
            string degree = null,
            int? universityStartYear = null,
            int? graduationYear = null,
            string country = null,
            string address = null,
            string motivation = null)
        {

            email ??= $"test.{Guid.NewGuid()}@test.com";

            using var connection = new SqlConnection(ConnectionString);
            connection.Open();

            const string query = @"
                INSERT INTO USERS (
                    firstName, lastName, gender, age, email,
                    activeAccount, parsedCV, personalityTestResult,
                    profilePicture, city, university, degree,
                    universityStartYear, graduationYear, country, address, motivation
                )
                VALUES (
                    @firstName, @lastName, @gender, @age, @email,
                    @activeAccount, @parsedCv, @personalityTestResult,
                    @profilePicture, @city, @university, @degree,
                    @universityStartYear, @graduationYear, @country, @address, @motivation
                );
                SELECT SCOPE_IDENTITY();";

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@firstName", firstName);
            cmd.Parameters.AddWithValue("@lastName", lastName);
            cmd.Parameters.AddWithValue("@gender", gender);
            cmd.Parameters.AddWithValue("@age", age);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@activeAccount", activeAccount ? 1 : 0);
            cmd.Parameters.AddWithValue("@parsedCv", (object)parsedCv ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@personalityTestResult", (object)personalityTestResult ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@profilePicture", (object)profilePicture ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@city", (object)city ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@university", (object)university ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@degree", (object)degree ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@universityStartYear", (object)universityStartYear ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@graduationYear", (object)graduationYear ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@country", (object)country ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@address", (object)address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@motivation", (object)motivation ?? DBNull.Value);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public static int InsertSkill(int userId, string name, int score = 0, DateTime? achievedDate = null)
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();

            const string query = @"
                INSERT INTO SKILLS (userID, name, score, achievedDate)
                VALUES (@userId, @name, @score, @achievedDate);
                SELECT SCOPE_IDENTITY();";

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@score", score);
            cmd.Parameters.AddWithValue("@achievedDate", (object)achievedDate ?? DBNull.Value);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public static int InsertDocument(int userId, string name, string filePath = null, DateTime? uploadDate = null)
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();

            const string query = @"
                INSERT INTO DOCUMENTS (userID, nameDocument, FilePath, UploadDate)
                VALUES (@userId, @name, @filePath, @uploadDate);
                SELECT SCOPE_IDENTITY();";

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@filePath", (object)filePath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@uploadDate", (object)uploadDate ?? DBNull.Value);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public static int InsertMatch(int userId, string companyName, string jobRole, DateTime? matchDate = null)
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();

            const string query = @"
                INSERT INTO MATCHES (userID, companyName, jobRole, matchDate)
                VALUES (@userId, @companyName, @jobRole, @matchDate);
                SELECT SCOPE_IDENTITY();";

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@companyName", companyName);
            cmd.Parameters.AddWithValue("@jobRole", jobRole);
            cmd.Parameters.AddWithValue("@matchDate", (object)matchDate ?? DateTime.Now);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public static int InsertPreference(int userId, string preferenceType, string value)
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();

            const string query = @"
                INSERT INTO PREFERENCES (userID, preferanceType, value)
                VALUES (@userId, @type, @value);
                SELECT SCOPE_IDENTITY();";

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@type", preferenceType);
            cmd.Parameters.AddWithValue("@value", value);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public static string GetUserPersonalityTestResult(int userId)
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();

            string query = "SELECT personalityTestResult FROM USERS WHERE userID = @userId";
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);

            object result = command.ExecuteScalar();
            return result == DBNull.Value ? null : result.ToString();

        }

        public static void SetUserPersonality(int userId, string result)
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();

            string query = "UPDATE USERS SET personalityTestResult=@result WHERE userID = @userId";
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@result", result);
            command.Parameters.AddWithValue("@userId", userId);

            command.ExecuteNonQuery();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PussyCatsApp.models;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Windows.System;
using System.Diagnostics;

namespace PussyCatsApp.repositories
{
    /// <summary>
    /// Reads and writes UserProfile data across 4 tables:
    ///
    ///   Users       — all scalar fields + parsedCV (JSON string storing
    ///                 WorkExperiences, Projects, ExtraCurricularActivities)
    ///   Skills      — one row per skill tag
    ///   Documents   — one row per certificate/diploma
    ///   Preferences — one row per preference (JobRole, WorkMode, Location)
    /// </summary>
    public class UserProfileRepository : IUserProileRepository
    {
        // JsonSerializerOptions reused across calls
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Shape stored in the parsedCV column
        private record ParsedCVData(
            List<WorkExperience> WorkExperiences,
            List<Project> Projects,
            List<ExtraCurricularActivity> ExtraCurricularActivities
        );

        private const string connectionString = "Data Source=DESKTOP-LBK0E96\\SQLEXPRESS;Initial Catalog=UserManagementDB;Integrated Security=True;Trust Server Certificate=True";
        private SqlConnection sqlConnection;

        public UserProfile getProfileById(int userId)
        {
            using var connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
            } catch (Exception e) {
                Debug.WriteLine($"Failed to connect to database.{e.Message}");
                return null;
            }


            try
            {
                var profile = LoadUserRow(connection, userId);
                Debug.WriteLine(profile);
                Debug.WriteLine($"Loaded user row for userId={userId}: {(profile == null ? "NOT FOUND" : "FOUND")}");
                if (profile == null) return null;

                profile.Skills = LoadSkills(connection, userId);
                profile.RelevantCertificates = LoadCertificates(connection, userId);
                LoadPreferences(connection, userId, profile);

                // WorkExperiences, Projects, ExtraCurricular live in parsedCV
                LoadParsedCV(connection, userId, profile);

                return profile;
            }
            catch (SqlException e)
            {
                Debug.WriteLine($"SQL Exception: {e.Message}");
                return null;
            }
            finally
            {
                connection.Close();
            }
        }


        public UserProfile load(int id)
        {
            return null;
        }

        public void save(int id, UserProfile data)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            try
            {
                using var tx = connection.BeginTransaction();
                UpsertUserRow(connection, tx, id, data);
                SaveSkills(connection, tx, id, data.Skills);
                SavePreferences(connection, tx, id, data);
                // Documents (certificates) are managed separately via upload flow,
                // not overwritten on every profile save.

                tx.Commit();
            }
            catch (SqlException e)
            {
                Debug.WriteLine($"SQL Exception: {e.Message}");
                return;
            }
            finally
            {
                connection.Close();
            }
        }

        public void updateAccountStatus(int userId, string status)
        {
            this.sqlConnection = new SqlConnection(connectionString);
            SqlCommand updateAccounStatusCommand = new SqlCommand("UPDATE Users SET activeAccount = @status WHERE userID = @userId", sqlConnection);

            bool isActive = (status == "ACTIVE");
            updateAccounStatusCommand.Parameters.AddWithValue("@status", isActive);
            updateAccounStatusCommand.Parameters.AddWithValue("@userId", userId);
            try
            {
                sqlConnection.Open();
                int rowsAffected = updateAccounStatusCommand.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    Console.WriteLine($"No user found with ID {userId} to update account status");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

        }

        public void updateProfileLastModified(int userId, DateTime timestamp)
        {
            using var connection = new SqlConnection(connectionString);
            using var cmd = connection.CreateCommand();

            cmd.CommandText = "UPDATE Users SET LastUpdated = @time WHERE userID = @userId";
            cmd.Parameters.AddWithValue("@time", timestamp);
            cmd.Parameters.AddWithValue("@userId", userId);

            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating LastModified: {ex.Message}");
            }
        }

        public void updateProfilePicture(int userId, string picturePath)
        {
            this.sqlConnection = new SqlConnection(connectionString);

            SqlCommand updatePictureCommand = new SqlCommand("UPDATE Users SET profilePicture = @path WHERE userID = @userId", sqlConnection);

            updatePictureCommand.Parameters.AddWithValue("@path", (object)picturePath ?? DBNull.Value);
            updatePictureCommand.Parameters.AddWithValue("@userId", userId);

            try
            {
                sqlConnection.Open();
                updatePictureCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        /// <summary>
        /// Loads all scalar fields from the Users table for the given userId.
        /// Does NOT load Skills, Preferences, or parse the parsedCV JSON column.
        /// The caller is responsible for loading those separately.
        /// Returns null if no user with the given ID exists.
        /// </summary>
       
        private static UserProfile LoadUserRow(SqlConnection conn, int userId)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT userID, firstName, lastName, gender, age,
                       email, phone, github, linkedin, universityStartYear,
                       graduationYear, country, address,
                       personalityTestResult, activeAccount,
                       profilePicture, university, degree, LastUpdated, parsedCV
                FROM Users
                WHERE userID = @id";
            cmd.Parameters.AddWithValue("@id", userId);

            Debug.WriteLine(cmd);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            var genderChar = GetString(reader, "gender").Trim();
            var genderDisplay = genderChar switch
            {
                "M" => "Male",
                "F" => "Female",
                _ => genderChar
            };

            return new UserProfile
            {
                UserId = reader.GetInt32(reader.GetOrdinal("userID")),
                FirstName = GetString(reader, "firstName"),
                LastName = GetString(reader, "lastName"),
                Gender = genderDisplay,
                Age = GetInt(reader, "age"),
                Email = GetString(reader, "email"),
                PhoneNumber = GetString(reader, "phone"),
                GitHub = GetString(reader, "github"),
                LinkedIn = GetString(reader, "linkedin"),
                UniversityStartYear = GetInt(reader, "universityStartYear"),
                ExpectedGraduationYear = GetInt(reader, "graduationYear"),
                Country = GetString(reader, "country"),
                Address = GetString(reader, "address"),
                University = GetString(reader, "university"),
                Degree = GetString(reader, "degree"),
                PersonalityTestResult = GetString(reader, "personalityTestResult"),
                ActiveAccount = reader.GetBoolean(reader.GetOrdinal("activeAccount")),
                LastUpdated = reader.IsDBNull(reader.GetOrdinal("LastUpdated")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("LastUpdated")),
                ProfilePicture = GetString(reader, "profilePicture"),
            };
        }

        /// <summary>
        /// Deserializes WorkExperiences, Projects, and ExtraCurricularActivities
        /// from the parsedCV JSON column and populates them on the profile.
        /// </summary>
        private static void LoadParsedCV(SqlConnection conn, int userId, UserProfile profile)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT parsedCV FROM Users WHERE userID = @id";
            cmd.Parameters.AddWithValue("@id", userId);

            var raw = cmd.ExecuteScalar() as string;
            if (string.IsNullOrWhiteSpace(raw)) return;

            try
            {
                var data = JsonSerializer.Deserialize<ParsedCVData>(raw, _jsonOptions);
                if (data == null) return;

                profile.WorkExperiences = data.WorkExperiences ?? new();
                profile.Projects = data.Projects ?? new();
                profile.ExtraCurricularActivities = data.ExtraCurricularActivities ?? new();
            }
            catch (JsonException)
            {
                // parsedCV is malformed — leave the lists empty rather than crashing
                profile.WorkExperiences = new();
                profile.Projects = new();
                profile.ExtraCurricularActivities = new();
            }
        }

        private static List<string> LoadSkills(SqlConnection conn, int userId)
        {
            var skills = new List<string>();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT name FROM Skills WHERE userID = @id ORDER BY skillID";
            cmd.Parameters.AddWithValue("@id", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                skills.Add(reader.GetString(0));

            return skills;
        }

        private static List<string> LoadCertificates(SqlConnection conn, int userId)
        {
            var list = new List<string>();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT nameDocument
                FROM Documents
                WHERE userID = @id
                ORDER BY dID";
            cmd.Parameters.AddWithValue("@id", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var name = GetString(reader, "nameDocument");
                if (!string.IsNullOrWhiteSpace(name))
                    list.Add(name);
            }
            return list;
        }

        private static void LoadPreferences(SqlConnection conn, int userId, UserProfile profile)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT preferanceType, value
                FROM Preferences
                WHERE userID = @id";
            cmd.Parameters.AddWithValue("@id", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var type = GetString(reader, "preferanceType");
                var value = GetString(reader, "value");

                switch (type)
                {
                    case "JobRole":
                        profile.PreferredJobRoles.Add(value);
                        break;
                    case "WorkMode":
                        profile.WorkModePreference = value;
                        break;
                    case "Location":
                        profile.LocationPreference = value;
                        break;
                }
            }
        }

        // ════════════════════════════════════════════════════════════
        // PRIVATE SAVERS
        // ════════════════════════════════════════════════════════════

        private static void UpsertUserRow(SqlConnection conn, SqlTransaction tx,
            int userId, UserProfile p)
        {
            // Serialize WorkExperiences + Projects + ExtraCurricular into parsedCV
            var parsedCVJson = JsonSerializer.Serialize(new ParsedCVData(
                p.WorkExperiences ?? new(),
                p.Projects ?? new(),
                p.ExtraCurricularActivities ?? new()
            ), _jsonOptions);

            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = @"
                IF EXISTS (SELECT 1 FROM Users WHERE userID = @id)
                    UPDATE Users SET
                        firstName             = @firstName,
                        lastName              = @lastName,
                        gender                = @gender,
                        age                   = @age,
                        email                 = @email,
                        phone                 = @phone,
                        github                = @github,
                        linkedin              = @linkedin,
                        universityStartYear   = @universityStartYear,
                        graduationYear        = @graduationYear,
                        country               = @country,
                        address               = @address,
                        university            = @university,
                        degree                = @degree,
                        personalityTestResult = @personalityTestResult,
                        activeAccount         = @activeAccount,
                        profilePicture        = @profilePicture,
                        parsedCV              = @parsedCV
                    WHERE userID = @id
                ELSE
                    INSERT INTO Users (
                        firstName, lastName, gender, age, email, phone,
                        github, linkedin, universityStartYear, graduationYear, country, address,
                        university, degree, personalityTestResult, activeAccount,
                        profilePicture, parsedCV
                    ) VALUES (
                        @firstName, @lastName, @gender, @age, @email, @phone,
                        @github, @linkedin, @universityStartYear, @graduationYear, @country, @address,
                        @university, @degree, @personalityTestResult, @activeAccount,
                        @profilePicture, @parsedCV
                    )";

            // Convert 'Male'/'Female' → 'M'/'F' for CHAR(1) DB column
            var genderDb = p.Gender switch
            {
                "Male" => "M",
                "Female" => "F",
                _ => p.Gender
            };

            cmd.Parameters.AddWithValue("@id", userId);
            cmd.Parameters.AddWithValue("@firstName", p.FirstName);
            cmd.Parameters.AddWithValue("@lastName", p.LastName);
            cmd.Parameters.AddWithValue("@gender", (object)genderDb ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@age", p.Age);
            cmd.Parameters.AddWithValue("@email", p.Email);
            cmd.Parameters.AddWithValue("@phone", (object)p.PhoneNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@github", (object)p.GitHub ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@linkedin", (object)p.LinkedIn ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@universityStartYear", p.UniversityStartYear);
            cmd.Parameters.AddWithValue("@graduationYear", p.ExpectedGraduationYear);
            cmd.Parameters.AddWithValue("@country", (object)p.Country ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@address", (object)p.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@university", (object)p.University ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@degree", (object)p.Degree ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@personalityTestResult", (object)p.PersonalityTestResult ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@activeAccount", p.ActiveAccount);
            cmd.Parameters.AddWithValue("@profilePicture", (object)p.ProfilePicture ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@parsedCV", parsedCVJson);
            cmd.ExecuteNonQuery();
        }

        private static void SaveSkills(SqlConnection conn, SqlTransaction tx,
            int userId, List<string> skills)
        {
            // Delete all then re-insert — simplest for a flat tag list
            using (var del = conn.CreateCommand())
            {
                del.Transaction = tx;
                del.CommandText = "DELETE FROM Skills WHERE userID = @id";
                del.Parameters.AddWithValue("@id", userId);
                del.ExecuteNonQuery();
            }

            foreach (var skill in skills ?? new List<string>())
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = "INSERT INTO Skills (userID, name) VALUES (@uid, @name)";
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@name", skill);
                cmd.ExecuteNonQuery();
            }
        }

        private static void SavePreferences(SqlConnection conn, SqlTransaction tx,
            int userId, UserProfile p)
        {
            using (var del = conn.CreateCommand())
            {
                del.Transaction = tx;
                del.CommandText = "DELETE FROM Preferences WHERE userID = @id";
                del.Parameters.AddWithValue("@id", userId);
                del.ExecuteNonQuery();
            }

            void Insert(string type, string value)
            {
                if (string.IsNullOrWhiteSpace(value)) return;
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = @"
                    INSERT INTO Preferences (userID, preferanceType, value)
                    VALUES (@uid, @type, @value)";
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@value", value);
                cmd.ExecuteNonQuery();
            }

            foreach (var role in p.PreferredJobRoles ?? new List<string>())
                Insert("JobRole", role);

            Insert("WorkMode", p.WorkModePreference);
            Insert("Location", p.LocationPreference);
        }

        // ── Null-safe reader helpers ───────────────────────────────────

        private static string GetString(SqlDataReader r, string col)
            => r.IsDBNull(r.GetOrdinal(col)) ? string.Empty : r.GetString(r.GetOrdinal(col));

        private static int GetInt(SqlDataReader r, string col)
        {
            int ordinal = r.GetOrdinal(col);
            if (r.IsDBNull(ordinal)) return 0;

            return Convert.ToInt32(r.GetValue(ordinal));
        }
    }
}


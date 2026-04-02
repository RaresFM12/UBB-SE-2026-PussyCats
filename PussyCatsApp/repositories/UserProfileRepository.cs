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
using Microsoft.Extensions.Configuration;

namespace PussyCatsApp.repositories
{
    public class UserProfileRepository : IUserProileRepository
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        private record ParsedCVData(
            List<WorkExperience> WorkExperiences,
            List<Project> Projects,
            List<ExtraCurricularActivity> ExtraCurricularActivities
        );
        private record FormDataSnapshot(
            string FirstName,
            string LastName,
            int Age,
            string Gender,
            string Email,
            string PhoneNumber,
            string GitHub,
            string LinkedIn,
            string Country,
            string City,
            string University,
            string Degree,
            int UniversityStartYear,
            int ExpectedGraduationYear,
            string Address,
            string Motivation,
            bool HasDisabilities,
            List<string> Skills,
            List<WorkExperience> WorkExperiences,
            List<Project> Projects,
            List<ExtraCurricularActivity> ExtraCurricularActivities
        );

        private readonly string connectionString = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build().GetConnectionString("raresConnectionString");
        private SqlConnection sqlConnection;

        public UserProfile getProfileById(int userId)
        {
            using var connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                Debug.WriteLine("Database connection opened successfully.");
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

                profile.RelevantCertificates = LoadCertificates(connection, userId);
                LoadPreferences(connection, userId, profile);

                //Skills, WorkExperiences, Projects, ExtraCurricular live in formDataJson
                LoadFormData(connection, userId, profile);

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

        private static UserProfile LoadUserRow(SqlConnection connection, int userId)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT userID, firstName, lastName, gender, age,
                       email, phone, github, linkedin, universityStartYear,
                       graduationYear, country, city, address, motivation,
                       disabilities,
                       personalityTestResult, activeAccount,
                       profilePicture, university, degree, LastUpdated, parsedCV,
                       formDataJson
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
                City = GetString(reader, "city"),
                Address = GetString(reader, "address"),
                Motivation = GetString(reader, "motivation"),
                HasDisabilities = !reader.IsDBNull(reader.GetOrdinal("disabilities")) && reader.GetBoolean(reader.GetOrdinal("disabilities")),
                University = GetString(reader, "university"),
                Degree = GetString(reader, "degree"),
                PersonalityTestResult = GetString(reader, "personalityTestResult"),
                ActiveAccount = reader.GetBoolean(reader.GetOrdinal("activeAccount")),
                LastUpdated = reader.IsDBNull(reader.GetOrdinal("LastUpdated")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("LastUpdated")),
                ProfilePicture = GetString(reader, "profilePicture"),
                FormDataJson = GetString(reader, "formDataJson"),
            };
        }

        /// <summary>
        /// Loads Skills, WorkExperiences, Projects, and ExtraCurricularActivities
        /// from the formDataJson column and populates them on the profile.
        /// </summary>
        private static void LoadFormData(SqlConnection connection, int userId, UserProfile profile)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT formDataJson FROM Users WHERE userID = @id";
            command.Parameters.AddWithValue("@id", userId);

            var raw = command.ExecuteScalar() as string;
            if (string.IsNullOrWhiteSpace(raw)) return;

            try
            {
                var formData = JsonSerializer.Deserialize<FormDataSnapshot>(raw, _jsonOptions);
                if (formData == null) return;

                profile.Skills = formData.Skills ?? new();
                profile.WorkExperiences = formData.WorkExperiences ?? new();
                profile.Projects = formData.Projects ?? new();
                profile.ExtraCurricularActivities = formData.ExtraCurricularActivities ?? new();
            }
            catch (JsonException)
            {
                profile.Skills = new();
                profile.WorkExperiences = new();
                profile.Projects = new();
                profile.ExtraCurricularActivities = new();
            }
        }

        /// <summary>
        /// Builds a plain-text summary for the parsedCV column.
        /// Line 1: Full name, Line 2: University, Line 3: Skills (comma-separated)
        /// </summary>
        private static string BuildParsedCVText(UserProfile profile)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{profile.FirstName} {profile.LastName}".Trim());
            sb.AppendLine(profile.University ?? string.Empty);
            sb.AppendLine(string.Join(", ", profile.Skills ?? new List<string>()));
            return sb.ToString().TrimEnd();
        }

        private static List<string> LoadSkills(SqlConnection connection, int userId)
        {
            var skills = new List<string>();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT name FROM Skills WHERE userID = @id ORDER BY skillID";
            command.Parameters.AddWithValue("@id", userId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
                skills.Add(reader.GetString(0));

            return skills;
        }

        private static List<string> LoadCertificates(SqlConnection connection, int userId)
        {
            var list = new List<string>();
            using var cmd = connection.CreateCommand();
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

        private static void LoadPreferences(SqlConnection connection, int userId, UserProfile profile)
        {
            using var cmd = connection.CreateCommand();
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

        
        private static void UpsertUserRow(SqlConnection connection, SqlTransaction transaction,
            int userId, UserProfile profile)
        {
            var parsedCVText = BuildParsedCVText(profile);

            var formDataJson = JsonSerializer.Serialize(new FormDataSnapshot(
                profile.FirstName, profile.LastName, profile.Age, profile.Gender,
                profile.Email, profile.PhoneNumber, profile.GitHub, profile.LinkedIn,
                profile.Country, profile.City, profile.University, profile.Degree,
                profile.UniversityStartYear, profile.ExpectedGraduationYear,
                profile.Address, profile.Motivation, profile.HasDisabilities,
                profile.Skills ?? new(),
                profile.WorkExperiences ?? new(),
                profile.Projects ?? new(),
                profile.ExtraCurricularActivities ?? new()
            ), _jsonOptions);

            using var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
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
                        city                  = @city,
                        address               = @address,
                        motivation            = @motivation,
                        disabilities          = @disabilities,
                        university            = @university,
                        degree                = @degree,
                        personalityTestResult = @personalityTestResult,
                        activeAccount         = @activeAccount,
                        profilePicture        = @profilePicture,
                        parsedCV              = @parsedCV,
                        formDataJson          = @formDataJson
                    WHERE userID = @id
                ELSE
                    INSERT INTO Users (
                        firstName, lastName, gender, age, email, phone,
                        github, linkedin, universityStartYear, graduationYear, country, city, address,
                        motivation, disabilities,
                        university, degree, personalityTestResult, activeAccount,
                        profilePicture, parsedCV, formDataJson
                    ) VALUES (
                        @firstName, @lastName, @gender, @age, @email, @phone,
                        @github, @linkedin, @universityStartYear, @graduationYear, @country, @city, @address,
                        @motivation, @disabilities,
                        @university, @degree, @personalityTestResult, @activeAccount,
                        @profilePicture, @parsedCV, @formDataJson
                    )";

            var genderDb = profile.Gender switch
            {
                "Male" => "M",
                "Female" => "F",
                _ => profile.Gender
            };

            cmd.Parameters.AddWithValue("@id", userId);
            cmd.Parameters.AddWithValue("@firstName", profile.FirstName);
            cmd.Parameters.AddWithValue("@lastName", profile.LastName);
            cmd.Parameters.AddWithValue("@gender", (object)genderDb ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@age", profile.Age);
            cmd.Parameters.AddWithValue("@email", profile.Email);
            cmd.Parameters.AddWithValue("@phone", (object)profile.PhoneNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@github", (object)profile.GitHub ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@linkedin", (object)profile.LinkedIn ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@universityStartYear", profile.UniversityStartYear);
            cmd.Parameters.AddWithValue("@graduationYear", profile.ExpectedGraduationYear);
            cmd.Parameters.AddWithValue("@country", (object)profile.Country ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@city", (object)profile.City ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@address", (object)profile.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@motivation", (object)profile.Motivation ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@disabilities", profile.HasDisabilities);
            cmd.Parameters.AddWithValue("@university", (object)profile.University ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@degree", (object)profile.Degree ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@personalityTestResult", (object)profile.PersonalityTestResult ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@activeAccount", profile.ActiveAccount);
            cmd.Parameters.AddWithValue("@profilePicture", (object)profile.ProfilePicture ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@parsedCV", parsedCVText);
            cmd.Parameters.AddWithValue("@formDataJson", formDataJson);
            cmd.ExecuteNonQuery();
        }

        private static void SaveSkills(SqlConnection connection, SqlTransaction transaction,
            int userId, List<string> skills)
        {
            using (var del = connection.CreateCommand())
            {
                del.Transaction = transaction;
                del.CommandText = "DELETE FROM Skills WHERE userID = @id";
                del.Parameters.AddWithValue("@id", userId);
                del.ExecuteNonQuery();
            }

            foreach (var skill in skills ?? new List<string>())
            {
                using var cmd = connection.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandText = "INSERT INTO Skills (userID, name) VALUES (@uid, @name)";
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@name", skill);
                cmd.ExecuteNonQuery();
            }
        }

        private static void SavePreferences(SqlConnection connection, SqlTransaction transaction,
            int userId, UserProfile profile)
        {
            using (var del = connection.CreateCommand())
            {
                del.Transaction = transaction;
                del.CommandText = "DELETE FROM Preferences WHERE userID = @id";
                del.Parameters.AddWithValue("@id", userId);
                del.ExecuteNonQuery();
            }

            void Insert(string type, string value)
            {
                if (string.IsNullOrWhiteSpace(value)) return;
                using var cmd = connection.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandText = @"
                    INSERT INTO Preferences (userID, preferanceType, value)
                    VALUES (@uid, @type, @value)";
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@value", value);
                cmd.ExecuteNonQuery();
            }

            foreach (var role in profile.PreferredJobRoles ?? new List<string>())
                Insert("JobRole", role);

            Insert("WorkMode", profile.WorkModePreference);
            Insert("Location", profile.LocationPreference);
        }

        private static string GetString(SqlDataReader reader, string col)
            => reader.IsDBNull(reader.GetOrdinal(col)) ? string.Empty : reader.GetString(reader.GetOrdinal(col));

        private static int GetInt(SqlDataReader reader, string col)
        {
            int ordinal = reader.GetOrdinal(col);
            if (reader.IsDBNull(ordinal)) return 0;

            return Convert.ToInt32(reader.GetValue(ordinal));
        }
    }
}


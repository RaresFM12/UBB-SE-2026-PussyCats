using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.models;
using Microsoft.Data.SqlClient;

namespace PussyCatsApp.repositories
{
    internal class UserProfileRepository : IUserProileRepository
    {
        private SqlConnection sqlConnection;
        public UserProfile getProfileById(int userId)
        {
            this.sqlConnection = new SqlConnection("Data Source=JEFF\\SQLEXPRESS;Initial Catalog=UserManagementDB;Integrated Security=True;TrustServerCertificate=True");
            SqlCommand getIdCommand = new SqlCommand($"SELECT * FROM Users WHERE @userId = userID",sqlConnection);
            getIdCommand.Parameters.AddWithValue("@userId", userId);
            UserProfile userProfile = null;
            try
            {
                sqlConnection.Open();
                SqlDataReader reader = getIdCommand.ExecuteReader();
                if (reader.Read())
                {
                    userProfile = new UserProfile();

                    userProfile.userID = reader.GetInt32(0);

                    userProfile.firstName = reader.GetString(1);

                    userProfile.lastName = reader.GetString(2);

                    userProfile.gender = !reader.IsDBNull(3) ? reader.GetString(3)[0] : 'n';

                    userProfile.age = !reader.IsDBNull(4) ? reader.GetInt32(4) : 0;

                    userProfile.emailAddress = reader.GetString(5);

                    userProfile.phoneNumber = !reader.IsDBNull(6) ? reader.GetString(6) : "";

                    userProfile.githubAccount = !reader.IsDBNull(7) ? reader.GetString(7) : "";

                    userProfile.linkedinAccount = !reader.IsDBNull(8) ? reader.GetString(8) : "";

                    userProfile.graduationYear = !reader.IsDBNull(9) ? (int)reader.GetInt16(9) : 0;

                    userProfile.country = !reader.IsDBNull(10) ? reader.GetString(10) : "";

                    userProfile.city = !reader.IsDBNull(11) ? reader.GetString(11) : "";

                    userProfile.sexualOrientation = !reader.IsDBNull(12) ? reader.GetString(12) : "";

                    userProfile.disability = reader.GetBoolean(13);

                    userProfile.personalityResult = !reader.IsDBNull(15) ? reader.GetString(15) : "";

                    bool isActive = reader.GetBoolean(16);

                    userProfile.accountStatus = isActive ? AccountStatus.ACTIVE : AccountStatus.INACTIVE;

                    userProfile.profilePicture = !reader.IsDBNull(17) ? reader.GetString(17) : "";
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
            return userProfile;
        }
        
        public UserProfile load(int id)
        {
            return null;
        }

        public void save(int id, UserProfile data)
        {
        }

        public void updateAccountStatus(int userId, string status)
        {
            this.sqlConnection = new SqlConnection("Data Source=JEFF\\SQLEXPRESS;Initial Catalog=UserManagementDB;Integrated Security=True;TrustServerCertificate=True");
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
        }

        public void updateProfilePicture(int userId, string picturePath)
        {
        }

    }
}

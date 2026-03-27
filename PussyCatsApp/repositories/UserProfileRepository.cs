using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.models;

namespace PussyCatsApp.repositories
{
    internal class UserProfileRepository : IUserProileRepository
    {
        public UserProfile getProfileById(int userId)
        {
            return new UserProfile();
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
        }

        public void updateProfileLastModified(int userId, DateTime timestamp)
        {
        }

        public void updateProfilePicture(int userId, string picturePath)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models;
using Windows.System.UserProfile;

namespace PussyCatsApp.repositories
{
    public interface IUserProileRepository : IRepository<UserProfile>
    {
        UserProfile getProfileById(int userId);
        void updateAccountStatus(int userId, string status);
        void updateProfilePicture(int userId, string picturePath);
        void updateProfileLastModified(int userId, DateTime timestamp);

    }
}

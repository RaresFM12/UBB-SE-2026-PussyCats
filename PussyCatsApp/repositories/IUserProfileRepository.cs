using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.UserProfile;
using PussyCatsApp.Models;

namespace PussyCatsApp.Repositories
{
    public interface IUserProfileRepository : IRepository<UserProfile>
    {
        UserProfile GetProfileById(int userId);
        void UpdateAccountStatus(int userId, string status);
        void UpdateProfilePicture(int userId, string picturePath);
        void UpdateProfileLastModified(int userId, DateTime timestamp);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models;
using Windows.System.UserProfile;

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

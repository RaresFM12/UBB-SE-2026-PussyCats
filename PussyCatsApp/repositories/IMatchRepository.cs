using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PussyCatsApp.models;

namespace PussyCatsApp.Repositories
{
    public interface IMatchRepository
    {
        List<models.Match> GetMatchesByUserId(int userId);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PussyCatsApp.models;
namespace PussyCatsApp.repositories;


public interface IMatchRepository
{
    List<Match> GetByUserId(int userId);
}

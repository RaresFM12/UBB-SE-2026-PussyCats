using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PussyCatsApp.Models;

namespace PussyCatsApp.Services
{
    public interface ICVParsingService
    {
        UserProfile ParseCVFile(string content, string fileType);
    }
}

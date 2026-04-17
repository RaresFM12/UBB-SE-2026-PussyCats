using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.models;

namespace PussyCatsApp.services
{
    public interface ICompletenessService
    {
        int CalculateCompleteness(UserProfile profile);
        string GetNextEmptyFieldPrompt(UserProfile profile);
    }
}

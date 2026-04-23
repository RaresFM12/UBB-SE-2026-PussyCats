using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace PussyCatsApp.Repositories.PersonalityTestRepo;

public interface IPersonalityTestRepository : IRepository<string>
{
    void Save(int id, string personalityTestResult);
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.repositories.personality_test_repo
{
    internal class PersonalityTestRepository : IPersonalityTestRepository
    {
        private string connectionString;
        
        PersonalityTestRepository(String connectionString)
        {
            this.connectionString = connectionString;
        }

        string IRepository<String>.load(int id)
        {
            throw new NotImplementedException();
        }

        void IRepository<String>.save(int id, String data)
        {
            throw new NotImplementedException();
        }

        void IPersonalityTestRepository.UpdateSelectedRole(int userId, String personalityTestResult)
        {
            throw new NotImplementedException();
        }
    }
}

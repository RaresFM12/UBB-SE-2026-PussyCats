using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.repositories.personality_test_repo
{
    internal class PersonalityTestRepository : IPersonalityTestRepository
    {
        private string ConnectionString;
        
        PersonalityTestRepository(String _ConnectionString)
        {
            this.ConnectionString = _ConnectionString;
        }

        public String load(int id)
        {
            throw new NotImplementedException();
        }

        public void save(int id, string data)
        {
            throw new NotImplementedException();
        }

        public void UpdateSelectedRole(int userId, string personalityTestResult)
        {
            throw new NotImplementedException();
        }
    }
}

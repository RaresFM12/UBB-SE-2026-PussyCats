using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.repositories
{
    public interface IRepository<T>
    {
        T? load(int id);
        void save(int id, T data);
    }
}

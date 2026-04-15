using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.Models
{
    public enum AccountStatus
    {
        /// <summary>
        /// Account is active and can be used normally.
        /// </summary>
        ACTIVE,

        /// <summary>
        /// Account is inactive and cannot be accessed.
        /// </summary>
        INACTIVE
    }
}

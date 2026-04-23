using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.Models
{
    public enum PreferenceType
    {
        /// <summary>
        /// The preferred job role of the user.
        /// </summary>
        JobRole,

        /// <summary>
        /// The preferred work mode (for example, remote, onsite, or hybrid).
        /// </summary>
        WorkMode,

        /// <summary>
        /// The preferred work location of the user.
        /// </summary>
        Location
    }
}

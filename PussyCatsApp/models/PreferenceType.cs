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
        /// Preference related to job role or position.
        /// </summary>
        JobRole,

        /// <summary>
        /// Preference related to work mode (remote, hybrid, on-site).
        /// </summary>
        WorkMode,

        /// <summary>
        /// Preference related to geographic location.
        /// </summary>
        Location
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.Models
{
    public enum WorkMode
    {
        /// <summary>
        /// Work is performed remotely.
        /// </summary>
        Remote,

        /// <summary>
        /// Work is performed in a hybrid mode (both remote and onsite).
        /// </summary>
        Hybrid,

        /// <summary>
        /// Work is performed onsite.
        /// </summary>
        OnSite
    }
}

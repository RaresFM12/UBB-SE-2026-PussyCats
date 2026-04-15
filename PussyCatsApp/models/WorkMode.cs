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
        /// Fully remote work arrangement with no in-office requirement.
        /// </summary>
        Remote,

        /// <summary>
        /// Hybrid work arrangement combining remote and in-office days.
        /// </summary>
        Hybrid,

        /// <summary>
        /// Fully on-site work arrangement requiring physical presence.
        /// </summary>
        OnSite
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.models
{
    public class UserPreference
    {
        private int pId;
        private int userId;
        private string preferenceType = string.Empty;
        private string value = string.Empty;

        public int PId
        {
            get { return pId; }
            set { pId = value; }
        }

        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        public string PreferenceType
        {
            get { return preferenceType; }
            set { preferenceType = value; }
        }

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
}

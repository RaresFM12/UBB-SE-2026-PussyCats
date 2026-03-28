using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Printing.PrintTicket;
using Windows.System.Profile;

namespace PussyCatsApp.models
{
    internal class UserProfile
    {
        public int userID { get; set; }
        public String firstName { get; set; }
        public String lastName { get; set; }
        public int age { get; set; }
        public int graduationYear { get; set; }
        public char gender { get; set; }
        public String city { get; set; }
        public String country { get; set; }
        public String phoneNumber { get; set; }
        public String emailAddress { get; set; }
        public String motivation { get; set; }
        public bool disability { get; set; }
        public String sexualOrientation { get; set; }
        public String githubAccount { get; set; }
        public String linkedinAccount { get; set; }
        public String personalityResult { get; set; }
        public String profilePicture { get; set; }
        public AccountStatus accountStatus { get; set; }
        public DateTime lastUpdated { get; set; }

        public List<Education> educationHistory { get; set; }
        public List<Work> workExperience { get; set; }
        public List<Skill> skills { get; set; }
        public List<Activity> extracurriculars { get; set; }
        public List<Documents> documents { get; set; }

        public UserProfile()
        {
            educationHistory = new List<Education>();
            workExperience = new List<Work>();
            skills = new List<Skill>();
            extracurriculars = new List<Activity>();
            documents = new List<Documents>();
        }


    }
}

using Microsoft.Data.SqlClient;
using PussyCatsApp.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.repositories
{
    public class UserSkillRepository 
    {
        //private const string connectionString = "Data Source=DESKTOP-LBK0E96\\SQLEXPRESS;Initial Catalog=PussyCatsDB;Integrated Security=True;Trust Server Certificate=True;";
        private const string connectionString = "Data Source=.;Initial Catalog=PussyCatsDB;Integrated Security=True;Trust Server Certificate=True";
        public UserSkillRepository()
        {}

        public List<UserSkill> GetByUserId(int userId)
        {
            List<UserSkill> skills = new List<UserSkill>();

            GetVerifiedSkills(userId, skills);
            GetUnverifiedSkillsFromCV(userId, skills);

            return skills;
        }

        private void GetVerifiedSkills(int userId, List<UserSkill> skills)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT name, score FROM Skills WHERE userId = @UserId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    UserSkill skill = new UserSkill();
                    skill.SkillName = reader["name"].ToString();
                    skill.IsVerified = true;
                    skill.Score = (int)reader["score"];
                    skills.Add(skill);
                }
            }
        }

        private void GetUnverifiedSkillsFromCV(int userId, List<UserSkill> skills)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT ParsedCV FROM Users WHERE userID = @UserId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);
                object result = command.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    return;

                string parsedCV = result.ToString();
                string[] lines = parsedCV.Split('\n');

                if (lines.Length < 3)
                    return;

                string skillsLine = lines[2].Trim();
                string[] cvSkills = skillsLine.Split(',');

                foreach (string cvSkill in cvSkills)
                {
                    string skillName = cvSkill.Trim();
                    if (string.IsNullOrWhiteSpace(skillName))
                        continue;

                    bool alreadyVerified = false;
                    foreach (UserSkill existing in skills)
                    {
                        if (existing.SkillName.ToLower() == skillName.ToLower())
                        {
                            alreadyVerified = true;
                            break;
                        }
                    }

                    if (!alreadyVerified)
                    {
                        UserSkill skill = new UserSkill();
                        skill.SkillName = skillName;
                        skill.IsVerified = false;
                        skill.Score = 0;
                        skills.Add(skill);
                    }
                }
            }
        }
    }
}

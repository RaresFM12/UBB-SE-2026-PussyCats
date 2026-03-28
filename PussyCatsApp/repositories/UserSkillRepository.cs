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
        private string connectionString;

        public UserSkillRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<UserSkill> GetByUserId(int userId)
        {
            List<UserSkill> skills = new List<UserSkill>();

            GetVerifiedSkills(userId, skills);
            GetUnverifiedSkillsFromCV(userId, skills);

            return skills;
        }

        private void GetVerifiedSkills(int userId, List<UserSkill> skills)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT SkillName, Score FROM Skills WHERE UserId = @UserId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    UserSkill skill = new UserSkill();
                    skill.SkillName = reader["SkillName"].ToString();
                    skill.IsVerified = true;
                    skill.Score = (int)reader["Score"];
                    skills.Add(skill);
                }
            }
        }

        private void GetUnverifiedSkillsFromCV(int userId, List<UserSkill> skills)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ParsedCV FROM Users WHERE Id = @UserId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                object result = cmd.ExecuteScalar();

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

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
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT SkillName, IsVerified, Score FROM UserSkills WHERE UserId = @UserId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    UserSkill skill = new UserSkill();
                    skill.SkillName = reader["SkillName"].ToString();
                    skill.IsVerified = (bool)reader["IsVerified"];
                    skill.Score = (int)reader["Score"];
                    skills.Add(skill);
                }
            }
            return skills;
        }
    }
}

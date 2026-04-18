using PussyCatsApp.Models;
using PussyCatsApp.repositories.personality_test_repo;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using static System.Formats.Asn1.AsnWriter;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PussyCatsApp.services
{
    public class PersonalityTestService
    {
        private PersonalityTestRepository Repository;

        public PersonalityTestService()
        {
            this.Repository = new PersonalityTestRepository();
        }

        public static List<Question> LoadQuestions()
        {
            return
            [
                // Visibility
                new(1,  "I notice design details in apps and websites that most people would overlook.", TraitType.VISIBILITY, 1),
                new(2,  "I believe a seamless, high-quality user interface is just as critical to a project's success as the underlying code.", TraitType.VISIBILITY, 2),
                new(3,  "I believe that a project isn't truly 'finished' until the visual polish matches the technical quality.", TraitType.VISIBILITY, 3),
                new(4,  "I find myself drawn to tools and interfaces that are clean and well-designed.", TraitType.VISIBILITY, 4),

                // Interaction
                new(5,  "I enjoy collaborating with others more than working through problems on my own.", TraitType.INTERACTION, 5),
                new(6,  "I feel energized after meetings or group discussions rather than drained.", TraitType.INTERACTION, 6),
                new(7,  "I would rather manage relationships and expectations than debug a technical issue.", TraitType.INTERACTION,  7),
                new(8,  "I prefer roles where communication is a big part of the daily work.", TraitType.INTERACTION,  8),

                // Depth
                new(9,  "When something breaks, I want to understand exactly why — not just fix the surface issue.", TraitType.DEPTH, 9),
                new(10, "I enjoy reading documentation or technical material to fully understand a system.", TraitType.DEPTH, 10),
                new(11, "I find it satisfying to deeply master one topic rather than know a little about many.", TraitType.DEPTH, 11),
                new(12, "I get curious about what's happening \"behind the scenes\" in the tools and systems I use.", TraitType.DEPTH, 12),

                // Creativity
                new(13, "I thrive when given a problem with no clear solution rather than a checklist to follow.", TraitType.CREATIVITY, 13),
                new(14, "I enjoy coming up with new ideas more than executing someone else's plan.", TraitType.CREATIVITY, 14),
                new(15, "I prefer work that leaves room for experimentation over work with strict rules and procedures.", TraitType.CREATIVITY, 15),
                new(16, "I am most productive when tackling new problems rather than refining existing processes.", TraitType.CREATIVITY, 16),

                // Pace
                new(17, "I work best when I have several different tasks to switch between throughout the day.", TraitType.PACE, 17),
                new(18, "I enjoy fast-paced environments where priorities shift and I have to adapt quickly.", TraitType.PACE, 18),
                new(19, "I prefer having many smaller responsibilities over owning one large long-term problem.", TraitType.PACE, 19),
                new(20, "I feel productive when I can check off multiple different things in a single day.", TraitType.PACE, 20),

                // Abstraction
                new(21, "I enjoy working with mathematical concepts, formulas, or statistical models.", TraitType.ABSTRACTION,  21),
                new(22, "I find theoretical or abstract problems more interesting than purely practical ones.", TraitType.ABSTRACTION,  22),
                new(23, "I am comfortable working with data, probabilities, and logical frameworks.", TraitType.ABSTRACTION,  23),
                new(24, "I prefer to understand the logic and first principles of a system rather than just knowing how to operate it.", TraitType.ABSTRACTION,  24),
            ];

            //return questions.OrderBy(_ => Random.Shared.Next()).ToList
            // TODO see what is the best way to shuffle a list in C#
        }

        public Dictionary<TraitType, double> CalculateTraitScores(Dictionary<Question, AnswerValue> answers)
        {
            var traitScores = new Dictionary<TraitType, double>();
            var traitQuestionCounts = new Dictionary<TraitType, int>();

            foreach (var answer in answers)
            {
                var trait = answer.Key.Trait;
                if (!traitScores.ContainsKey(trait))
                {
                    traitScores[trait] = 0;
                    traitQuestionCounts[trait] = 0;
                }

                traitScores[trait] += answer.Value switch
                {
                    AnswerValue.STRONGLY_DISAGREE => 1,
                    AnswerValue.DISAGREE => 2,
                    AnswerValue.NEUTRAL => 3,
                    AnswerValue.AGREE => 4,
                    AnswerValue.STRONGLY_AGREE => 5,
                    _ => 0
                };

                ++traitQuestionCounts[trait];
            }

            foreach(var trait in traitScores.Keys)
                traitScores[trait] /= traitQuestionCounts[trait];

            return traitScores;
        }

        public Dictionary<JobRole, double> CalculateRoleScores(Dictionary<TraitType, double> traitScores)
        {
            var roleScores = new Dictionary<JobRole, double>();

            // Frontend Developer  V×2 + C×2 + P
            roleScores.Add(JobRole.FrontendDeveloper,
                traitScores[TraitType.VISIBILITY] * 2 +
                traitScores[TraitType.CREATIVITY] * 2 +
                traitScores[TraitType.PACE]);

            // Backend Developer   D×2 + (5−V)×2 + P
            roleScores.Add(JobRole.BackendDeveloper,
                traitScores[TraitType.DEPTH] * 2 +
                (5 - traitScores[TraitType.VISIBILITY]) * 2 +
                traitScores[TraitType.PACE]);

            // UI / UX Designer V×3 + C×2 + I
            roleScores.Add(JobRole.UIUXDesigner,
                traitScores[TraitType.VISIBILITY] * 3 +
                traitScores[TraitType.CREATIVITY] * 2 +
                traitScores[TraitType.INTERACTION]);

            // DevOps Engineer D×2 + P×2 + (5−I)
            roleScores.Add(JobRole.DevOpsEngineer,
                traitScores[TraitType.DEPTH] * 2 +
                traitScores[TraitType.PACE] * 2 +
                (5 - traitScores[TraitType.INTERACTION]));

            // Project Manager I×3 + C + (5−D)
            roleScores.Add(JobRole.ProjectManager,
                traitScores[TraitType.INTERACTION] * 3 +
                traitScores[TraitType.CREATIVITY] +
                (5 - traitScores[TraitType.DEPTH]));


            //Data Analyst    D×2 + A×2 + (5−I)
            roleScores.Add(JobRole.DataAnalyst,
                traitScores[TraitType.DEPTH] * 2 +
                traitScores[TraitType.ABSTRACTION] * 2 +
                (5 - traitScores[TraitType.INTERACTION]));

            //Cybersecurity Specialist D×3 + (6−I) +(6−P)
            roleScores.Add(JobRole.CybersecuritySpecialist,
                traitScores[TraitType.DEPTH] * 3 +
                (6 - traitScores[TraitType.INTERACTION]) +
                (6 - traitScores[TraitType.PACE]));

            //AI / ML Engineer D×3 + C + A×2
            roleScores.Add(JobRole.AIMLEngineer,
                traitScores[TraitType.DEPTH] * 3 +
                traitScores[TraitType.CREATIVITY] +
                traitScores[TraitType.ABSTRACTION] * 2);

            return roleScores;
        }

        public Dictionary<JobRole, double> GetTopRoles(Dictionary<JobRole, double> roleScores, int length)
        {
            return roleScores
                .OrderByDescending(kvp => kvp.Value)
                .Take(length)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public void SaveResult(int userId, string personalityTestResult)
        {
            Repository.save(userId, personalityTestResult);
        }
    }
}

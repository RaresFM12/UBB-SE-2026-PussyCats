using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.Models
{
    public enum AnswerValue
    {
        /// <summary>
        /// Strongly disagree with the statement.
        /// </summary>
        STRONGLY_DISAGREE = 1,

        /// <summary>
        /// Disagree with the statement.
        /// </summary>
        DISAGREE = 2,

        /// <summary>
        /// Neither agree nor disagree with the statement.
        /// </summary>
        NEUTRAL = 3,

        /// <summary>
        /// Agree with the statement.
        /// </summary>
        AGREE = 4,

        /// <summary>
        /// Strongly agree with the statement.
        /// </summary>
        STRONGLY_AGREE = 5
    }
}

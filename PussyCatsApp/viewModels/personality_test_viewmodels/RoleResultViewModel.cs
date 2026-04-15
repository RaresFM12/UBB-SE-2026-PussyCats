using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PussyCatsApp.Models;

namespace PussyCatsApp.ViewModels
{
    /// <summary>
    /// ViewModel for a role result, displaying the matched job role and its calculated score.
    /// </summary>
    public partial class RoleResultViewModel : ObservableObject
    {
        public JobRole Role { get; }
        public double Score { get; }

        [ObservableProperty]
        public partial bool IsSelected { get; set; }

        public RoleResultViewModel(JobRole role, double score)
        {
            Role = role;
            Score = score;
        }
    }
}

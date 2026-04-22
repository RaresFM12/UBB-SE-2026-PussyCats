using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PussyCatsApp.Models;

namespace PussyCatsApp.viewModels
{
    public partial class RoleResultViewModel : ObservableObject
    {
        public JobRole Role { get; }
        public double Score { get; }

        [ObservableProperty]
        public partial bool IsSelected { get; set; }

        public RoleResultViewModel(JobRole jobRole, double score)
        {
            Role = jobRole;
            Score = score;
        }
    }
}

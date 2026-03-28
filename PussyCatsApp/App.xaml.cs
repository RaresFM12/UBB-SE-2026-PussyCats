using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using PussyCatsApp.models;
using PussyCatsApp.repositories.personality_test_repo;
using PussyCatsApp.services;
using PussyCatsApp.views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PussyCatsApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();

            //String conStr = "Data Source=.;Initial Catalog=UserManagementDB;Integrated Security=True;Trust Server Certificate=True";
            //var repo = new PersonalityTestRepository(conStr);

            //var service = new PersonalityTestService(repo);

            //var questions = PersonalityTestService.LoadQuestions();
            //var testAnswers = new Dictionary<Question, AnswerValue>
            //{
            //    // Visibility
            //    { questions[0],  AnswerValue.STRONGLY_AGREE },
            //    { questions[1],  AnswerValue.AGREE },
            //    { questions[2],  AnswerValue.STRONGLY_AGREE },
            //    { questions[3],  AnswerValue.AGREE },

            //    // Interaction
            //    { questions[4],  AnswerValue.NEUTRAL },
            //    { questions[5],  AnswerValue.DISAGREE },
            //    { questions[6],  AnswerValue.NEUTRAL },
            //    { questions[7],  AnswerValue.DISAGREE },

            //    // Depth
            //    { questions[8],  AnswerValue.AGREE },
            //    { questions[9],  AnswerValue.STRONGLY_AGREE },
            //    { questions[10], AnswerValue.AGREE },
            //    { questions[11], AnswerValue.STRONGLY_AGREE },

            //    // Creativity
            //    { questions[12], AnswerValue.AGREE },
            //    { questions[13], AnswerValue.STRONGLY_AGREE },
            //    { questions[14], AnswerValue.AGREE },
            //    { questions[15], AnswerValue.NEUTRAL },

            //    // Pace
            //    { questions[16], AnswerValue.NEUTRAL },
            //    { questions[17], AnswerValue.DISAGREE },
            //    { questions[18], AnswerValue.NEUTRAL },
            //    { questions[19], AnswerValue.DISAGREE },

            //    // Abstraction
            //    { questions[20], AnswerValue.AGREE },
            //    { questions[21], AnswerValue.STRONGLY_AGREE },
            //    { questions[22], AnswerValue.AGREE },
            //    { questions[23], AnswerValue.STRONGLY_AGREE },
            //};

            //var traitTypes = service.CalculateTraitScores(testAnswers);
            //var roleScores = service.CalculateRoleScores(traitTypes);
            //var topScores = service.GetTopRoles(roleScores, 3);
        }
    }
}

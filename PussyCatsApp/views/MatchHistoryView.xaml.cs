using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.ViewModels;
using PussyCatsApp.Services;
using PussyCatsApp.Repositories.PersonalityTestRepo;
using Windows.Foundation;
using Windows.Foundation.Collections;
using PussyCatsApp.Repositories;
using PussyCatsApp.Configuration;

namespace PussyCatsApp.Views
{
    /// <summary>
    /// View for displaying the user's match history and related statistics, handling navigation and data loading.
    /// </summary>
    public sealed partial class MatchHistoryView : Page
    {
        private MatchHistoryViewModel viewModel;

        public MatchHistoryView()
        {
            this.InitializeComponent();
            this.Loaded += MatchHistoryView_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is MatchHistoryViewModel vm)
            {
                viewModel = vm;
            }
        }

        private void MatchHistoryView_Loaded(object sender, RoutedEventArgs e)
        {
            if (viewModel == null)
            {
                IMatchService matchService = new MatchService(new MatchRepository(DatabaseConfiguration.GetConnectionString()));
                viewModel = new MatchHistoryViewModel(1, matchService);
            }

            LoadMatches();
            LoadStatistics();
        }

        private void LoadMatches()
        {
            viewModel.LoadMatches();

            string error = viewModel.GetErrorMessage();
            if (!string.IsNullOrEmpty(error))
            {
                return;
            }

            MatchesListView.ItemsSource = viewModel.GetMatches();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void LoadStatistics()
        {
            viewModel.LoadStatistics();

            string error = viewModel.GetErrorMessage();
            if (!string.IsNullOrEmpty(error))
            {
                return;
            }

            var stats = viewModel.GetStatistics();

            if (stats != null)
            {
                lblTotalMatches.Text = $"Total Matches: {stats.TotalMatches}";
                lblMatchesLastMonth.Text = $"Last Month: {stats.MatchesLastMonth}";
                lblMatchesLastSixMonths.Text = $"Last 6 Months: {stats.MatchesLastSixMonths}";
                lblMatchesLastYear.Text = $"Last Year: {stats.MatchesLastYear}";

                if (stats.MatchesPerPosition != null)
                {
                    var positionData = stats.MatchesPerPosition
                        .Select(kvp => new
                        {
                            JobRole = kvp.Key,
                            Count = kvp.Value
                        }).ToList();

                    PositionStatsListView.ItemsSource = positionData;
                }
            }
        }
    }
}

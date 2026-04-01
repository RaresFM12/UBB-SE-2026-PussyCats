using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.viewModels;
using PussyCatsApp.services;
using PussyCatsApp.repositories.personality_test_repo;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

using System;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.viewModels;
using PussyCatsApp.repositories;

namespace PussyCatsApp.views
{
    public sealed partial class MatchHistoryView : Page
    {
        private MatchHistoryViewModel _viewModel;

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
                _viewModel = vm;
            }
        }

        private void MatchHistoryView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
            {
                _viewModel = new MatchHistoryViewModel(1);
            }

            LoadMatches();
            LoadStatistics();
        }

        private void LoadMatches()
        {
            _viewModel.LoadMatches();

            string error = _viewModel.GetErrorMessage();
            if (!string.IsNullOrEmpty(error))
                return;

            MatchesListView.ItemsSource = _viewModel.GetMatches();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

        private void LoadStatistics()
        {
            _viewModel.LoadStatistics();

            string error = _viewModel.GetErrorMessage();
            if (!string.IsNullOrEmpty(error))
                return;
            

            var stats = _viewModel.GetStatistics();

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

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

        // Metoda standard WinUI 3 pentru a primi date când navighezi către această pagină
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Verificăm dacă pagina a primit ViewModel-ul ca parametru la navigare
            if (e.Parameter is MatchHistoryViewModel vm)
            {
                _viewModel = vm;
            }
        }

        private void MatchHistoryView_Loaded(object sender, RoutedEventArgs e)
        {
            // 1. Configurăm manual tot ce are nevoie pagina noastră (Mod Test Local)
            // Înlocuim logica de primire a parametrului cu o inițializare directă aici
            if (_viewModel == null)
            {
                // Datele de conexiune către baza ta locală FAMILY\SQLEXPRESS
                string connectionString = "Data Source=DESKTOP-SCP6QST;Initial Catalog=UserManagementDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30";

                // Creăm "motorul" paginii pe loc
                var repo = new MatchRepository(connectionString);
                var service = new MatchService(repo);

                // Folosim ID-ul 1 (Ioana Gavrila din baza de date)
                _viewModel = new MatchHistoryViewModel(service, 1);
            }

            // 2. Acum că avem ViewModel-ul gata, apelăm metodele de încărcare
            LoadMatches();
            LoadStatistics();
        }

        private void LoadMatches()
        {
            _viewModel.LoadMatches();

            string error = _viewModel.GetErrorMessage();
            if (!string.IsNullOrEmpty(error))
            {
                ShowError(error);
                return;
            }

            // Alocăm lista de istoric direct sursei de date a ListView-ului
            MatchesListView.ItemsSource = _viewModel.GetMatches();
        }

        private void LoadStatistics()
        {
            _viewModel.LoadStatistics();

            string error = _viewModel.GetErrorMessage();
            if (!string.IsNullOrEmpty(error))
            {
                ShowError(error);
                return;
            }

            var stats = _viewModel.GetStatistics();

            if (stats != null)
            {
                // Populăm etichetele text
                lblTotalMatches.Text = $"Total Matches: {stats.TotalMatches}";
                lblMatchesLastMonth.Text = $"Last Month: {stats.MatchesLastMonth}";
                lblMatchesLastSixMonths.Text = $"Last 6 Months: {stats.MatchesLastSixMonths}";
                lblMatchesLastYear.Text = $"Last Year: {stats.MatchesLastYear}";

                // Convertim dicționarul din MatchStatistics într-o listă anonimă pentru a-l putea afișa în tabel
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

        // Metoda nativă WinUI 3 pentru afișarea unui pop-up de eroare elegant (ContentDialog)
        private async void ShowError(string message)
        {
            /*if (this.XamlRoot == null)
                return;

            var dialog = new ContentDialog
            {
                Title = "Eroare la încărcare",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot // Obligatoriu în WinUI 3 pentru dialoguri
            };
            await dialog.ShowAsync();*/
        }
    }
}

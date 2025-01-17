using Projet_DesktopDev_Antoine_Richard.Class_DB;
using Projet_DesktopDev_Antoine_Richard.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;

namespace Projet_DesktopDev_Antoine_Richard
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Status_Table> StatusList { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();

            LoadStatusList();
        }

        private void btn_accueil_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }

        private void btn_gestion_Click(object sender, RoutedEventArgs e)
        {
            GestionJeux gestionJeuxWindow = new GestionJeux();
            this.Close();
            gestionJeuxWindow.Show();
        }
        private void LoadStatusList()
        {
            var statuses = SelectGame.GetAllStatus() ?? new List<Status_Table>();

            var totalStatus = new Status_Table
            {
                Status_name = "Total"
            };

            statuses.Insert(0, totalStatus);

            if (statuses != null && statuses.Any())
            {
                StatusBox.ItemsSource = statuses;
                StatusBox.DisplayMemberPath = "Status_name";
                StatusBox.SelectedValuePath = "Status_name";

                TotalBox.ItemsSource = statuses;
                TotalBox.DisplayMemberPath = "Status_name";
                TotalBox.SelectedValuePath = "Status_name";

                string savedStatus = Properties.Settings.Default.SelectedStatus;
                var savedStatusItem = statuses.FirstOrDefault(s => s.Status_name == savedStatus);

                if (savedStatusItem != null)
                {
                    StatusBox.SelectedItem = savedStatusItem;
                }
                else
                {
                    var defaultStatus = statuses.FirstOrDefault(status => status.Status_name == "terminer");
                    StatusBox.SelectedItem = defaultStatus;
                }

                string savedStatusTotal = Properties.Settings.Default.SelectedTotal;
                var savedStatusItemTotal = statuses.FirstOrDefault(s => s.Status_name == savedStatusTotal);
                if (savedStatusItemTotal != null)
                {
                    TotalBox.SelectedItem = savedStatusItemTotal;
                }
                else
                {
                    var defaultStatusTotal = statuses.FirstOrDefault(status => status.Status_name == "Total");
                    TotalBox.SelectedItem = defaultStatusTotal;
                }
            }
            else
            {
                MessageBox.Show("Aucun statut trouvé.");
            }
        }

        private void StatusBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedStatus = StatusBox.SelectedItem as Status_Table;

            if (selectedStatus != null)
            {
                Properties.Settings.Default.SelectedStatus = selectedStatus.Status_name;
                Properties.Settings.Default.Save();

                UpdateFinishedGames(selectedStatus.Status_name);
            }
        }
        private void TotalBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedTotal = TotalBox.SelectedItem as Status_Table;

            if (selectedTotal != null)
            {
                Properties.Settings.Default.SelectedTotal = selectedTotal.Status_name;
                Properties.Settings.Default.Save();

                UpdateTotalGames(selectedTotal.Status_name);
            }
        }

        private void UpdateTotalGames(string selectedStatusName)
        {
            var games = SelectGame.GetAllGames() ?? new List<Game_Table>();

            var viewModel = this.DataContext as MainViewModel;
            if (viewModel != null)
            {
                if (selectedStatusName.Equals("Total", StringComparison.OrdinalIgnoreCase))
                {
                    viewModel.TotalGames = games.Count;
                }
                else
                {
                    var filteredGames = games.Where(game => game.status != null
                                                             && string.Equals(game.status.Status_name, selectedStatusName, StringComparison.OrdinalIgnoreCase))
                                              .ToList();

                    viewModel.TotalGames = filteredGames.Count;
                }
            }
        }
        private void UpdateFinishedGames(string selectedStatusName)
        {
            var games = SelectGame.GetAllGames() ?? new List<Game_Table>();

            var viewModel = this.DataContext as MainViewModel;
            if (viewModel != null)
            {
                if (selectedStatusName.Equals("Total", StringComparison.OrdinalIgnoreCase))
                {
                    viewModel.FinishedGames = games.Count;
                }
                else
                {
                    var filteredGames = games.Where(game => game.status != null
                                                             && string.Equals(game.status.Status_name, selectedStatusName, StringComparison.OrdinalIgnoreCase))
                                              .ToList();

                    viewModel.FinishedGames = filteredGames.Count;
                }
            }
        }
    }
}
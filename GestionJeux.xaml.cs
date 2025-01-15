using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Projet_DesktopDev_Antoine_Richard.Class_DB;

namespace Projet_DesktopDev_Antoine_Richard
{
    public partial class GestionJeux : Window
    {
        public ObservableCollection<Game_Table> Games { get; set; }
        public ObservableCollection<Game_Table> FilteredGames { get; set; }
        public ObservableCollection<Game_Table> SearchedGames { get; set; }
        public ObservableCollection<Status_Table> Status { get; set; }
        public RelayCommand<int> FilterCommand { get; set; }

        public int AllFilterParameter => 0;
        public Visibility SearchResultsVisibility { get; set; } = Visibility.Collapsed;


        public GestionJeux()
        {
            InitializeComponent();

            try
            {
                Games = new ObservableCollection<Game_Table>(SelectGame.GetAllGames());

                if (Games == null || Games.Count == 0)
                {
                    MessageBox.Show("La collection de jeux est vide ou non initialisée.");
                    return;
                }

                FilteredGames = new ObservableCollection<Game_Table>(Games);
                SearchedGames = new ObservableCollection<Game_Table>(Games); // Pour la recherche

                Status = new ObservableCollection<Status_Table>(SelectGame.GetAllStatus());


                if (Status == null || Status.Count == 0)
                {
                    MessageBox.Show("La collection de statuts est vide ou non initialisée.");
                }

                FilterCommand = new RelayCommand<int>(FilterGamesByStatus);

                DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'initialisation des jeux : {ex.Message}");
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                // Vérifier que Games est bien initialisé et que SearchBox contient du texte
                if (Games == null || Games.Count == 0)
                {
                    return; // Rien à filtrer si Games est null ou vide
                }

                string query = SearchBox.Text?.ToLower() ?? string.Empty;

                // Si la barre de recherche est vide, on ne montre aucun résultat
                if (string.IsNullOrEmpty(query))
                {
                    SearchResults.Visibility = Visibility.Collapsed;
                    return;
                }

                // Filtrer les jeux dont le nom contient la lettre ou la chaîne recherchée
                var filterSearch = Games.Where(game => game != null && !string.IsNullOrEmpty(game?.name) && game.name?.ToLower().Contains(query) == true);

                // Vider la collection SearchedGames et ajouter les jeux filtrés
                SearchedGames.Clear();
                foreach (var game in filterSearch)
                {
                    SearchedGames.Add(game);
                }

                // Afficher ou masquer la zone de liste des résultats
                SearchResults.Visibility = SearchedGames.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du filtrage : {ex.Message}");
            }
        }



        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Effacer le texte de base (si présent)
            SearchBox.Text = "";
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Si le champ est vide, rétablir le texte de base à partir du Tag
            if (string.IsNullOrEmpty(SearchBox.Text))
            {
                SearchBox.Text = "Rechercher...";
            }
        }


        private void SearchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SearchResults.SelectedItem is Game_Table selectedGame)
            {
                OpenGameDetail(selectedGame.game_id);

            }
        }

        private void OpenGameDetail(int gameId)
        {
            DetailJeux detailJeuxWindow = new DetailJeux(gameId);
            this.Close();
            detailJeuxWindow.Show();
        }


        private void FilterGamesByStatus(int statusId)
        {
            if (Games == null || Status == null) return;

            var filtered = statusId == 0
                ? Games
                : Games.Where(game => game.status != null && game.status.status_id == statusId);

            FilteredGames.Clear();
            foreach (var game in filtered)
            {
                FilteredGames.Add(game);
            }
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

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            AddJeux addJeuxWindow = new AddJeux();
            this.Close();
            addJeuxWindow.Show();
        }

        private void btn_detail(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {

                if (button.CommandParameter is int gameId)
                {
                    DetailJeux detailJeuxWindow = new DetailJeux(gameId);
                    this.Close();
                    detailJeuxWindow.Show();
                }
                else
                {
                    MessageBox.Show("Impossible de récupérer l'ID du jeu. Paramètre incorrect.");
                }
            }
        }

        public void AddGameToCollection(Game_Table game)
        {
            Games.Add(game);
        }
    }
}

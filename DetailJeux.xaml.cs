using Projet_DesktopDev_Antoine_Richard.Class_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Projet_DesktopDev_Antoine_Richard
{
    /// <summary>
    /// Logique d'interaction pour DetailJeux.xaml
    /// </summary>
    public partial class DetailJeux : Window
    {

        private int GameId;
        public DetailJeux(int gameId)
        {
            InitializeComponent();
            GameId = gameId;

            LoadGameDetails();
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

        private void Modifier_Click(object sender, RoutedEventArgs e)
        {
            UpdateJeux updateJeuxWindow = new UpdateJeux(GameId);
            this.Close();
            updateJeuxWindow.Show();
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            GestionJeux gestionJeuxWindow = new GestionJeux();
            this.Close();
            gestionJeuxWindow.Show();
        }

        private void LoadGameDetails()
        {
            // Appelle la fonction backend pour récupérer les détails du jeu
            var gameDetails = SelectGame.GetGameById(GameId);

            if (gameDetails != null)
            {
                // Remplir les champs dans l'interface utilisateur
                Status_Form.Content = gameDetails.status.status_name;
                Name_Form.Content = gameDetails.name;
                Description_Form.Content = gameDetails.description;
                Annee_Form.Content = gameDetails.annee;
                Plateforme_Form.Content = gameDetails.plateforme;
                Genre_Form.Content = gameDetails.genre;

                // Charger l'image dans le contrôle Image
                if (!string.IsNullOrEmpty(gameDetails.image))
                {
                    try
                    {
                        var imagePath = System.IO.Path.GetFullPath(gameDetails.image);
                        Image_Form.Source = new BitmapImage(new Uri(imagePath));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erreur lors du chargement de l'image : " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Aucune image trouvée pour ce jeu.");
                }
            }
        }

        private void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer ce jeu ?", "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // Récupérer les détails du jeu (ici, GameId)
                var deletedGame = SelectGame.GetGameById(GameId);

                if (deletedGame != null)
                {
                    // Appeler la fonction de suppression de jeu en passant l'objet Game_Table
                    bool isDeleted = DeleteGame.DeleteJeux(deletedGame);

                    if (isDeleted)
                    {
                        // Si la suppression réussit, afficher un message de confirmation
                        MessageBox.Show("Le jeu a été supprimé avec succès.", "Suppression réussie", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Rediriger vers la page de gestion des jeux
                        GestionJeux gestionJeuxWindow = new GestionJeux();
                        this.Close();
                        gestionJeuxWindow.Show();
                    }
                    else
                    {
                        // Si la suppression échoue, afficher un message d'erreur
                        MessageBox.Show("Une erreur s'est produite lors de la suppression du jeu.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Le jeu n'a pas été trouvé.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

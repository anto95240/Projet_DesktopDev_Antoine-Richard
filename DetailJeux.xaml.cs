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
            var gameDetails = SelectGame.GetGameById(GameId);

            if (gameDetails != null)
            {
                Status_Form.Content = gameDetails.status.Status_name;
                Name_Form.Content = gameDetails.Name;
                Description_Form.Content = gameDetails.Description;
                Annee_Form.Content = gameDetails.Annee;
                Plateforme_Form.Content = gameDetails.Plateforme;
                Genre_Form.Content = gameDetails.Genre;

                if (!string.IsNullOrEmpty(gameDetails.Image))
                {
                    try
                    {
                        var imagePath = System.IO.Path.GetFullPath(gameDetails.Image);
                        Image_Form.Source = new BitmapImage(new Uri(imagePath));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erreur lors du chargement de l'image : " + ex.Message);
                    }
                }
            }
        }

        private void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer ce jeu ?", "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var deletedGame = SelectGame.GetGameById(GameId);

                if (deletedGame != null)
                {
                    bool isDeleted = DeleteGame.DeleteJeux(deletedGame);

                    if (isDeleted)
                    {
                        MessageBox.Show("Le jeu a été supprimé avec succès.", "Suppression réussie", MessageBoxButton.OK, MessageBoxImage.Information);

                        GestionJeux gestionJeuxWindow = new GestionJeux();
                        this.Close();
                        gestionJeuxWindow.Show();
                    }
                    else
                    {
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

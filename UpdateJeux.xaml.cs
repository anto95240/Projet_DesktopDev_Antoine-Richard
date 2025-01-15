using Projet_DesktopDev_Antoine_Richard.Class_DB;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
    /// Logique d'interaction pour UpdateJeux.xaml
    /// </summary>
    public partial class UpdateJeux : Window
    {

        private int GameId;
        private string Image = "";
        public UpdateJeux(int gameId)
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

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            DetailJeux DetailJeuxWindow = new DetailJeux(GameId);
            this.Close();
            DetailJeuxWindow.Show();
        }

        private void LoadGameDetails()
        {
            // Appelle la fonction backend pour récupérer les détails du jeu
            var gameDetails = SelectGame.GetGameById(GameId);

            if (gameDetails != null)
            {
                // Remplir les champs dans l'interface utilisateur
                nom_Form_Mod.Text = gameDetails.name;
                Description_Form_Mod.Text = gameDetails.description;
                Annee_Form_Mod.Text = gameDetails.annee;
                Plateforme_Form_Mod.Text = gameDetails.plateforme;
                Genre_Form_Mod.Text = gameDetails.genre;

                // Récupérer l'image
                Image = gameDetails.image;

                // Récupérer les statuts depuis la base de données
                List<Status_Table> statuses = GetStatutsFromDatabase();

                // Remplir la ComboBox avec les statuts
                StatusComboBox.Items.Clear();
                ComboBoxItem selectedStatusItem = null;

                foreach (var status in statuses)
                {
                    var statusItem = new ComboBoxItem
                    {
                        Content = status.status_name,
                        Tag = status.status_id
                    };

                    StatusComboBox.Items.Add(statusItem);

                    // Vérifier si ce statut correspond au statut actuel du jeu
                    if (status.status_id == gameDetails.status.status_id)
                    {
                        selectedStatusItem = statusItem;
                    }
                }

                // Si un statut est trouvé, le sélectionner par défaut
                if (selectedStatusItem != null)
                {
                    StatusComboBox.SelectedItem = selectedStatusItem;
                    Status_Form_Mod.Content = selectedStatusItem.Content.ToString(); // Mettre à jour le bouton
                }
            }
            else
            {
                MessageBox.Show("Le jeu n'a pas été trouvé dans la base de données.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void btn_Modifier_Click(object sender, RoutedEventArgs e)
        {
            var selectedStatusItem = StatusComboBox.SelectedItem as ComboBoxItem;

            if (selectedStatusItem == null)
            {
                MessageBox.Show("Veuillez sélectionner un statut pour le jeu.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Sortir de la méthode si aucun statut n'est sélectionné
            }

            var updatedGame = new Game_Table
            {
                game_id = GameId,
                name = nom_Form_Mod.Text.Trim(),
                description = Description_Form_Mod.Text.Trim(),
                annee = Annee_Form_Mod.Text.Trim(),
                plateforme = Plateforme_Form_Mod.Text.Trim(),
                genre = Genre_Form_Mod.Text.Trim(),
                status = new Status_Table
                {
                    status_id = (int)selectedStatusItem.Tag, // ID du statut sélectionné
                    status_name = selectedStatusItem.Content.ToString() // Nom du statut sélectionné
                },
                image = Image
            };

            // Valider si les champs obligatoires sont remplis
            if (string.IsNullOrWhiteSpace(updatedGame.name) || string.IsNullOrWhiteSpace(updatedGame.description) ||
                string.IsNullOrWhiteSpace(updatedGame.annee) || string.IsNullOrWhiteSpace(updatedGame.plateforme) ||
                string.IsNullOrWhiteSpace(updatedGame.genre))
            {
                MessageBox.Show("Tous les champs doivent être remplis, à l'exception du statut.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (updatedGame.status.status_id == 0)
            {
                MessageBox.Show("Veuillez sélectionner un statut pour le jeu.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Appelez la méthode de mise à jour dans la base de données
            var result = UpdateGame.UpdateJeux(updatedGame);

            // Vérifiez le résultat de la mise à jour
            if (result)
            {
                MessageBox.Show("Le jeu a été mis à jour avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                DetailJeux DetailJeuxWindow = new DetailJeux(GameId);
                this.Close();
                DetailJeuxWindow.Show();
            }
            else
            {
                MessageBox.Show("Une erreur s'est produite lors de la mise à jour du jeu.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Status_Form_Mod_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer les statuts depuis la base de données
            List<Status_Table> statuses = GetStatutsFromDatabase();

            // Effacer les éléments existants dans la ComboBox
            StatusComboBox.Items.Clear();

            // Ajouter les statuts à la ComboBox
            foreach (var status in statuses)
            {
                StatusComboBox.Items.Add(new ComboBoxItem
                {
                    Content = status.status_name,
                    Tag = status.status_id
                });
            }

            // Ouvrir la popup
            StatusPopup.IsOpen = true;
        }

        private void FermerPopup_Click(object sender, RoutedEventArgs e)
        {
            // Ferme la popup sans faire de modifications
            StatusPopup.IsOpen = false;
        }

        private void AjouterNouveauStatus_Click(object sender, RoutedEventArgs e)
        {
            // Afficher une boîte de saisie pour le nouveau statut
            string nouveauStatus = Microsoft.VisualBasic.Interaction.InputBox("Entrez le nouveau statut :", "Ajouter un statut", "");

            if (!string.IsNullOrEmpty(nouveauStatus))
            {
                using (var connection = Fonction.GetConnection()) // Utilisation de votre méthode de connexion
                {
                    if (connection == null)
                    {
                        MessageBox.Show("La connexion à la base de données a échoué.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string query = "INSERT INTO status_table (status_name) VALUES (@statusName)";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@statusName", nouveauStatus);
                        command.ExecuteNonQuery();
                    }
                }

                // Mettre à jour la ComboBox avec tous les statuts depuis la base de données
                StatusComboBox.Items.Clear();
                List<Status_Table> statuses = GetStatutsFromDatabase();
                ComboBoxItem newItem = null;

                foreach (var status in statuses)
                {
                    ComboBoxItem item = new ComboBoxItem
                    {
                        Content = status.status_name,
                        Tag = status.status_id
                    };
                    StatusComboBox.Items.Add(item);

                    // Si l'élément correspond au nouveau statut, on le sélectionne
                    if (status.status_name == nouveauStatus)
                    {
                        newItem = item;
                    }
                }

                // Sélectionner automatiquement le nouvel élément ajouté
                if (newItem != null)
                {
                    StatusComboBox.SelectedItem = newItem;
                    // Mettre à jour le bouton avec le nouveau statut
                    Status_Form_Mod.Content = nouveauStatus;
                }
            }
        }


        private void ValiderStatus_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = StatusComboBox.SelectedItem as ComboBoxItem;

            if (selectedItem != null)
            {
                int selectedStatusId = (int)selectedItem.Tag;

                // Utiliser l'ID du jeu actuel pour mettre à jour son statut
                int currentGameId = GetCurrentGameId(); // Remplacez par la méthode qui récupère l'ID du jeu courant

                using (var connection = Fonction.GetConnection()) // Utilisation de votre méthode de connexion
                {
                    if (connection == null)
                    {
                        MessageBox.Show("La connexion à la base de données a échoué.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string query = "UPDATE game_table SET status_id = @statusId WHERE game_id = @gameId";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@statusId", selectedStatusId);
                        command.Parameters.AddWithValue("@gameId", currentGameId); // Remplacez currentGameId par l'ID du jeu en cours
                        command.ExecuteNonQuery();
                    }
                }

                Status_Form_Mod.Content = selectedItem.Content.ToString();

                StatusPopup.IsOpen = false;
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner ou ajouter un statut.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Méthode pour récupérer les statuts depuis la base de données
        private List<Status_Table> GetStatutsFromDatabase()
        {
            List<Status_Table> statuses = new List<Status_Table>();

            using (var connection = Fonction.GetConnection()) // Utilisation de votre méthode de connexion
            {
                if (connection == null) return statuses;

                string query = "SELECT status_id, status_name FROM status_table";

                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            statuses.Add(new Status_Table
                            {
                                status_id = reader.GetInt32(0),
                                status_name = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            return statuses;
        }

        // Exemple de méthode pour récupérer l'ID du jeu actuel
        private int GetCurrentGameId()
        {
            // Cette méthode doit retourner l'ID du jeu que vous modifiez
            // Assurez-vous de bien définir cette méthode ou d'obtenir cet ID d'une autre manière
            return 1; // Remplacez par l'ID correct
        }

        private void Image_Form_Mod_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif";

            if (openFileDialog.ShowDialog() == true)
            {
                Image = openFileDialog.FileName;
                MessageBox.Show("Image sélectionnée : " + Image);
            }
        }

    }
}

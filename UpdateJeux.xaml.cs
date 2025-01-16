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
            var gameDetails = SelectGame.GetGameById(GameId);

            if (gameDetails != null)
            {
                nom_Form_Mod.Text = gameDetails.Name;
                Description_Form_Mod.Text = gameDetails.Description;
                Annee_Form_Mod.Text = gameDetails.Annee;
                Plateforme_Form_Mod.Text = gameDetails.Plateforme;
                Genre_Form_Mod.Text = gameDetails.Genre;

                Image = gameDetails.Image;

                List<Status_Table> statuses = SelectGame.GetAllStatus();

                StatusComboBox.Items.Clear();
                ComboBoxItem selectedStatusItem = null;

                foreach (var status in statuses)
                {
                    var statusItem = new ComboBoxItem
                    {
                        Content = status.Status_name,
                        Tag = status.status_id
                    };

                    StatusComboBox.Items.Add(statusItem);

                    if (status.status_id == gameDetails.status.status_id)
                    {
                        selectedStatusItem = statusItem;
                    }
                }

                if (selectedStatusItem != null)
                {
                    StatusComboBox.SelectedItem = selectedStatusItem;
                    Status_Form_Mod.Content = selectedStatusItem.Content.ToString();
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
                return;
            }

            var updatedGame = new Game_Table
            {
                game_id = GameId,
                Name = nom_Form_Mod.Text.Trim(),
                Description = Description_Form_Mod.Text.Trim(),
                Annee = Annee_Form_Mod.Text.Trim(),
                Plateforme = Plateforme_Form_Mod.Text.Trim(),
                Genre = Genre_Form_Mod.Text.Trim(),
                status = new Status_Table
                {
                    status_id = (int)selectedStatusItem.Tag,
                    Status_name = selectedStatusItem.Content.ToString() 
                },
                Image = Image
            };

            if (string.IsNullOrWhiteSpace(updatedGame.Name) || string.IsNullOrWhiteSpace(updatedGame.Description) ||
                string.IsNullOrWhiteSpace(updatedGame.Annee) || string.IsNullOrWhiteSpace(updatedGame.Plateforme) ||
                string.IsNullOrWhiteSpace(updatedGame.Genre))
            {
                MessageBox.Show("Tous les champs doivent être remplis, à l'exception du statut.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (updatedGame.status.status_id == 0)
            {
                MessageBox.Show("Veuillez sélectionner un statut pour le jeu.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = UpdateGame.UpdateJeux(updatedGame);

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
            List<Status_Table> statuses = SelectGame.GetAllStatus();

            StatusComboBox.Items.Clear();

            foreach (var status in statuses)
            {
                StatusComboBox.Items.Add(new ComboBoxItem
                {
                    Content = status.Status_name,
                    Tag = status.status_id
                });
            }

            StatusPopup.IsOpen = true;
        }

        private void FermerPopup_Click(object sender, RoutedEventArgs e)
        {
            StatusPopup.IsOpen = false;
        }

        private void AjouterNouveauStatus_Click(object sender, RoutedEventArgs e)
        {
            string nouveauStatus = Microsoft.VisualBasic.Interaction.InputBox("Entrez le nouveau statut :", "Ajouter un statut", "");

            if (!string.IsNullOrEmpty(nouveauStatus))
            {
                using (var connection = Fonction.GetConnection())
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

                StatusComboBox.Items.Clear();
                List<Status_Table> statuses = SelectGame.GetAllStatus();
                ComboBoxItem newItem = null;

                foreach (var status in statuses)
                {
                    ComboBoxItem item = new ComboBoxItem
                    {
                        Content = status.Status_name,
                        Tag = status.status_id
                    };
                    StatusComboBox.Items.Add(item);

                    if (status.Status_name == nouveauStatus)
                    {
                        newItem = item;
                    }
                }

                if (newItem != null)
                {
                    StatusComboBox.SelectedItem = newItem;
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

                int currentGameId = GetCurrentGameId(); 

                using (var connection = Fonction.GetConnection())
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
                        command.Parameters.AddWithValue("@gameId", currentGameId);
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

        private int GetCurrentGameId()
        {
            return 1;
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

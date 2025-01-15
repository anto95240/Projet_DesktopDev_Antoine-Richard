using Projet_DesktopDev_Antoine_Richard.Class_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Projet_DesktopDev_Antoine_Richard
{
    public partial class AddJeux : Window
    {
        private string Image = "";

        public AddJeux()
        {
            InitializeComponent();
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
            GestionJeux gestionJeuxWindow = new GestionJeux();
            this.Close();
            gestionJeuxWindow.Show();
        }

        private void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            
            // Récupérer le statut depuis la ComboBox
            var selectedItem = StatusComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner ou ajouter un statut pour le jeu.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var addededGame = new Game_Table
            {
                name = nom_Form_Add.Text.Trim(),
                description = Description_Form_Add.Text.Trim(),
                annee = Année_Form_Add.Text.Trim(),
                plateforme = Plateforme_Form_Add.Text.Trim(),
                genre = Genre_Form_Add.Text.Trim(),
                status = new Status_Table
                {
                    status_id = (int)selectedItem.Tag, // ID du statut sélectionné
                    status_name = selectedItem.Content.ToString() // Nom du statut sélectionné
                },
                image = Image
            };

            string selectedStatus = selectedItem.Content.ToString();

            // Ajouter le jeu à la base de données
            int gameId = AddGame.AddJeux(addededGame);
            if (gameId == -1)
            {
                MessageBox.Show("Erreur lors de l'ajout du jeu.");
                return;
            }

            MessageBox.Show("Jeu ajouté avec succès !");

            // Ajouter le jeu à l'interface de gestion
            addededGame.game_id = gameId;

            GestionJeux gestionJeux = Application.Current.Windows.OfType<GestionJeux>().FirstOrDefault();
            if (gestionJeux != null)
            {
                gestionJeux.AddGameToCollection(addededGame);
            }

            // Réinitialiser les champs
            nom_Form_Add.Clear();
            Description_Form_Add.Clear();
            Genre_Form_Add.Clear();
            Plateforme_Form_Add.Clear();
            Année_Form_Add.Clear();
            Image = "";
            StatusComboBox.SelectedItem = null;
        }

        private void Image_Form_Add_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif";

            if (openFileDialog.ShowDialog() == true)
            {
                Image = openFileDialog.FileName;
                MessageBox.Show("Image sélectionnée : " + Image);
            }
        }

        private void Status_Form_Add_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer les statuts depuis la base de données avec leur couleur
            List<Status_Table> statuses = GetStatutsFromDatabase();

            // Effacer les éléments existants dans la ComboBox
            StatusComboBox.Items.Clear();

            // Ajouter les statuts avec la couleur à la ComboBox
            foreach (var status in statuses)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = status.status_name,
                    Tag = status.status_id
                };
                StatusComboBox.Items.Add(item);
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
            string nouveauStatus = Microsoft.VisualBasic.Interaction.InputBox("Entrez le nouveau statut :", "Ajouter un statut", "");

            if (!string.IsNullOrEmpty(nouveauStatus))
            {
                // Ajouter le nouveau statut à la ComboBox sans l'insérer immédiatement dans la base de données
                ComboBoxItem newItem = new ComboBoxItem
                {
                    Content = nouveauStatus,
                    Tag = null // Le statut n'a pas encore d'ID
                };
                StatusComboBox.Items.Add(newItem);

                // Sélectionner automatiquement le nouvel élément
                StatusComboBox.SelectedItem = newItem;

                // Mettre à jour le bouton ou tout autre champ
                Status_Form_Add.Content = nouveauStatus;

                MessageBox.Show("Nouveau statut ajouté et en attente d'enregistrement !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ValiderStatus_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = StatusComboBox.SelectedItem as ComboBoxItem;

            if (selectedItem != null)
            {
                string selectedStatusName = selectedItem.Content.ToString();

                // Assigner le statut sélectionné pour l'ajout du jeu
                Status_Form_Add.Content = selectedStatusName; // Met à jour le bouton pour afficher le statut choisi
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

                string query = "SELECT * FROM status_table";

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
    }
}

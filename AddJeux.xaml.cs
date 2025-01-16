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
            
            var selectedItem = StatusComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner ou ajouter un statut pour le jeu.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var addededGame = new Game_Table
            {
                Name = nom_Form_Add.Text.Trim(),
                Description = Description_Form_Add.Text.Trim(),
                Annee = Année_Form_Add.Text.Trim(),
                Plateforme = Plateforme_Form_Add.Text.Trim(),
                Genre = Genre_Form_Add.Text.Trim(),
                status = new Status_Table
                {
                    status_id = (int)selectedItem.Tag,
                    Status_name = selectedItem.Content.ToString()
                },
                Image = Image
            };

            string selectedStatus = selectedItem.Content.ToString();

            int gameId = AddGame.AddJeux(addededGame);
            if (gameId == -1)
            {
                MessageBox.Show("Erreur lors de l'ajout du jeu.");
                return;
            }

            MessageBox.Show("Jeu ajouté avec succès !");

            addededGame.game_id = gameId;

            GestionJeux gestionJeux = Application.Current.Windows.OfType<GestionJeux>().FirstOrDefault();
            if (gestionJeux != null)
            {
                gestionJeux.AddGameToCollection(addededGame);
            }

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
            List<Status_Table> statuses = SelectGame.GetAllStatus();

            StatusComboBox.Items.Clear();

            foreach (var status in statuses)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = status.Status_name,
                    Tag = status.status_id
                };
                StatusComboBox.Items.Add(item);
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
                ComboBoxItem newItem = new ComboBoxItem
                {
                    Content = nouveauStatus,
                    Tag = null 
                };
                StatusComboBox.Items.Add(newItem);
                StatusComboBox.SelectedItem = newItem;

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

                Status_Form_Add.Content = selectedStatusName; 
                StatusPopup.IsOpen = false;
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner ou ajouter un statut.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}

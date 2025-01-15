using Projet_DesktopDev_Antoine_Richard.Class_DB;
using Projet_DesktopDev_Antoine_Richard.ViewModels;
using System.Windows;

namespace Projet_DesktopDev_Antoine_Richard
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
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
    }
}

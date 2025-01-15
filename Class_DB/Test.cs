using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Projet_DesktopDev_Antoine_Richard.Class_DB
{
    internal class Test
    {
        public static void TestConnection()
        {
            using (var connection = Fonction.GetConnection())
            {
                if (connection != null)
                {
                    MessageBox.Show("Connexion réussie !");
                }
                else
                {
                    MessageBox.Show("Erreur lors de la connexion.");
                }
            }
        }
    }
}

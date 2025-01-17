using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows;
using System.Xml.Linq;
using System.Data.SqlClient;

namespace Projet_DesktopDev_Antoine_Richard.Class_DB
{
    class Fonction
    {
        private const string ConnectionString = "Data Source=../Database/db_gameCollector.db;Version=3;";

        public static SQLiteConnection GetConnection()
        {
            try
            {
                var connection = new SQLiteConnection(ConnectionString);
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la connexion : " + ex.Message);
                return null;
            }
        }

    }
}

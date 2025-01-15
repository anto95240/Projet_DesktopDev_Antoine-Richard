using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Projet_DesktopDev_Antoine_Richard.Class_DB
{
    class DeleteGame
    {
        public static bool DeleteJeux(Game_Table deletedGame)
        {
            try
            {
                // Créer la connexion à la base de données
                using (var connection = Fonction.GetConnection())
                {
                    // Créer la commande SQL pour supprimer le jeu
                    string query = "DELETE FROM game_table WHERE game_id = @GameId";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        // Ajouter le paramètre GameId à la commande SQL
                        command.Parameters.AddWithValue("@GameId", deletedGame.game_id);

                        // Exécuter la commande de suppression
                        int rowsAffected = command.ExecuteNonQuery();

                        // Si une ligne a été supprimée, la suppression a réussi
                        if (rowsAffected > 0)
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Gérer l'exception et afficher un message d'erreur
                MessageBox.Show("Erreur lors de la suppression du jeu : " + ex.Message);
                return false;
            }
        }


    }
}

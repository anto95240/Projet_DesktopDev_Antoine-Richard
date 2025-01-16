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
                using (var connection = Fonction.GetConnection())
                {
                    string query = "DELETE FROM game_table WHERE game_id = @GameId";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@GameId", deletedGame.game_id);

                        int rowsAffected = command.ExecuteNonQuery();

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
                MessageBox.Show("Erreur lors de la suppression du jeu : " + ex.Message);
                return false;
            }
        }
    }
}

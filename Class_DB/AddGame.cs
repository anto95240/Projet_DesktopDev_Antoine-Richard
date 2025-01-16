using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_DesktopDev_Antoine_Richard.Class_DB
{
    class AddGame
    {
        public static int AddJeux(Game_Table addedGame)
        {
            try
            {
                using (var connection = Fonction.GetConnection())
                {
                    if (connection == null) return -1;

                    string normalizedStatusName = addedGame.status.Status_name.ToLower();

                    string checkStatusQuery = "SELECT COUNT(*) FROM status_table WHERE LOWER(status_name) = @status_name";
                    using (var command = new SQLiteCommand(checkStatusQuery, connection))
                    {
                        command.Parameters.AddWithValue("@status_name", normalizedStatusName);
                        int statusCount = Convert.ToInt32(command.ExecuteScalar());

                        if (statusCount == 0)
                        {
                            string insertStatusQuery = "INSERT INTO status_table (status_name) VALUES (@status_name)";
                            using (var insertCommand = new SQLiteCommand(insertStatusQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@status_name", normalizedStatusName);
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }

                    string idQuery = "SELECT status_id FROM status_table WHERE LOWER(status_name) = @status_name";
                    int statusId = -1;
                    using (var idCommand = new SQLiteCommand(idQuery, connection))
                    {
                        idCommand.Parameters.AddWithValue("@status_name", normalizedStatusName);
                        statusId = Convert.ToInt32(idCommand.ExecuteScalar());
                    }

                    string query = "INSERT INTO game_table (name, description, genre, plateforme, annee, image, status_id) " +
                                   "VALUES (@name, @description, @genre, @plateforme, @annee, @image, @status_id)";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", addedGame.Name);
                        command.Parameters.AddWithValue("@description", addedGame.Description);
                        command.Parameters.AddWithValue("@genre", addedGame.Genre.ToLower());
                        command.Parameters.AddWithValue("@plateforme", addedGame.Plateforme);
                        command.Parameters.AddWithValue("@annee", addedGame.Annee);
                        command.Parameters.AddWithValue("@image", addedGame.Image);
                        command.Parameters.AddWithValue("@status_id", addedGame.status.status_id);
                        command.ExecuteNonQuery();
                    }

                    string gameIdQuery = "SELECT last_insert_rowid()";
                    using (var command = new SQLiteCommand(gameIdQuery, connection))
                    {
                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'ajout du jeu : " + ex.Message);
                return -1;
            }
        }
    }
}

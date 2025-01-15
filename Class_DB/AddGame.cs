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

                    // Normaliser le statut en minuscules pour éviter les doublons
                    string normalizedStatusName = addedGame.status.status_name.ToLower();

                    // Vérifier si le statut existe déjà dans la base de données
                    string checkStatusQuery = "SELECT COUNT(*) FROM status_table WHERE LOWER(status_name) = @status_name";
                    using (var command = new SQLiteCommand(checkStatusQuery, connection))
                    {
                        command.Parameters.AddWithValue("@status_name", normalizedStatusName);
                        int statusCount = Convert.ToInt32(command.ExecuteScalar());

                        // Si le statut n'existe pas, on l'ajoute
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

                    // Récupérer l'ID du statut
                    string idQuery = "SELECT status_id FROM status_table WHERE LOWER(status_name) = @status_name";
                    int statusId = -1;
                    using (var idCommand = new SQLiteCommand(idQuery, connection))
                    {
                        idCommand.Parameters.AddWithValue("@status_name", normalizedStatusName);
                        statusId = Convert.ToInt32(idCommand.ExecuteScalar());
                    }

                    // Ajouter le jeu à la base de données
                    string query = "INSERT INTO game_table (name, description, genre, plateforme, annee, image, status_id) " +
                                   "VALUES (@name, @description, @genre, @plateforme, @annee, @image, @status_id)";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", addedGame.name);
                        command.Parameters.AddWithValue("@description", addedGame.description);
                        command.Parameters.AddWithValue("@genre", addedGame.genre.ToLower());
                        command.Parameters.AddWithValue("@plateforme", addedGame.plateforme);
                        command.Parameters.AddWithValue("@annee", addedGame.annee);
                        command.Parameters.AddWithValue("@image", addedGame.image);
                        command.Parameters.AddWithValue("@status_id", addedGame.status.status_id);  // Utiliser l'ID du statut
                        command.ExecuteNonQuery();
                    }

                    // Retourner l'ID du jeu inséré
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

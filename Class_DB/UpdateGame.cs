using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Projet_DesktopDev_Antoine_Richard.Class_DB
{
    class UpdateGame
    {

        public static bool UpdateJeux(Game_Table updatedGame)
        {
            try
            {
                using (var connection = Fonction.GetConnection())
                {
                    if (connection == null)
                    {
                        MessageBox.Show("Erreur : Impossible d'établir une connexion avec la base de données.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }

                    string query = @"UPDATE game_table
                             SET name = @Name,
                                 description = @Description,
                                 genre = @Genre,
                                 plateforme = @Plateforme,
                                 annee = @Annee,
                                 image = @Image,
                                 status_id = @StatusId
                             WHERE game_id = @Id";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        // Ajout des paramètres
                        command.Parameters.AddWithValue("@Name", updatedGame.name);
                        command.Parameters.AddWithValue("@Description", updatedGame.description);
                        command.Parameters.AddWithValue("@Annee", updatedGame.annee);
                        command.Parameters.AddWithValue("@Plateforme", updatedGame.plateforme);
                        command.Parameters.AddWithValue("@Genre", updatedGame.genre.ToLower());
                        command.Parameters.AddWithValue("@Image", updatedGame.image);
                        command.Parameters.AddWithValue("@StatusId", updatedGame.status.status_id); // Mise à jour du statut
                        command.Parameters.AddWithValue("@Id", updatedGame.game_id);

                        // Exécution de la requête
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return false;
                        }

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la mise à jour du jeu : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace Projet_DesktopDev_Antoine_Richard.Class_DB
{
    class SelectGame
    {
        public static List<Game_Table> GetAllGames()
        {
            var games = new List<Game_Table>();

            using (var connection = Fonction.GetConnection())
            {
                if (connection == null) return games;

                string query = @"
                    SELECT 
                        g.game_id, g.name, g.description, g.genre, g.plateforme, g.annee, g.image, 
                        s.status_id, s.status_name
                    FROM 
                        game_table g
                    LEFT JOIN 
                        status_table s
                    ON 
                        g.status_id = s.status_id";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var game = new Game_Table
                        {
                            game_id = reader.GetInt32(0),
                            name = reader.GetString(1),
                            description = reader.GetString(2),
                            genre = reader.IsDBNull(3) ? "Inconnu" : reader.GetString(3), // Gestion de null pour genre
                            plateforme = reader.IsDBNull(4) ? "Inconnu" : reader.GetString(4), // Gestion de null pour plateforme
                            annee = reader.GetString(5),
                            image = reader.GetString(6),
                            status = new Status_Table
                            {
                                status_id = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                                status_name = reader.IsDBNull(8) ? "Non défini" : reader.GetString(8) // Gestion de null pour status_name
                            }
                        };
                        games.Add(game);
                    }
                }
            }

            return games;
        }

        public static List<Status_Table> GetAllStatus()
        {
            var statuses = new List<Status_Table>();

            using (var connection = Fonction.GetConnection())
            {
                if (connection == null)
                {
                    Console.WriteLine("Erreur : Impossible de se connecter à la base de données.");
                    return statuses;
                }

                string query = "SELECT status_id, status_name FROM status_table";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var status = new Status_Table
                        {
                            status_id = reader.GetInt32(0),
                            status_name = reader.IsDBNull(1) ? null : reader.GetString(1)
                        };
                        statuses.Add(status);
                    }
                }
            }

            return statuses;
        }


        public static Game_Table GetGameById(int id)
        {
            try
            {
                using (var connection = Fonction.GetConnection())
                {
                    if (connection == null)
                    {
                        Console.WriteLine("Erreur : Impossible de se connecter à la base de données.");
                        return null;
                    }

                    // Jointure entre game_table et status_table
                    string query = @"
                        SELECT 
                            g.game_id, g.name, g.description, g.genre, g.plateforme, g.annee, g.image, 
                            s.status_id, s.status_name
                        FROM 
                            game_table g
                        LEFT JOIN 
                            status_table s
                        ON 
                            g.status_id = s.status_id
                        WHERE 
                            g.game_id = @id";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Game_Table
                                {
                                    game_id = reader.GetInt32(0),
                                    name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                    description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                    genre = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    plateforme = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    annee = reader.IsDBNull(5) ? null : reader.GetString(5),
                                    image = reader.IsDBNull(6) ? null : reader.GetString(6),
                                    status = new Status_Table
                                    {
                                        status_id = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                                        status_name = reader.IsDBNull(8) ? null : reader.GetString(8)
                                    }
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la récupération du jeu : " + ex.Message);
            }
            return null;
        }
    }
}

using OxyPlot;
using OxyPlot.Series;
using Projet_DesktopDev_Antoine_Richard.Class_DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Projet_DesktopDev_Antoine_Richard.ViewModels
{
    public class StatData
    {
        public string Title { get; set; }
        public string Fill { get; set; }
    }

    public class MainViewModel
    {
        public PlotModel GenrePlotModel { get; set; }
        public PlotModel PlatformPlotModel { get; set; }
        public int TotalGames { get; set; }
        public int FinishedGames { get; set; }
        public List<StatData> GenreStats { get; set; }
        public List<StatData> PlatformStats { get; set; }


        private static Dictionary<string, string> genreColors = new Dictionary<string, string>();
        private static Dictionary<string, string> platformColors = new Dictionary<string, string>();

        public MainViewModel()
        {
            GenrePlotModel = new PlotModel();
            PlatformPlotModel = new PlotModel();

            var games = SelectGame.GetAllGames() ?? new List<Game_Table>();

            if (!games.Any())
            {
                Console.WriteLine("Aucun jeu trouvé dans la base de données.");
                return;
            }

            string borderColor = "#3E527D";
            GenrePlotModel.Background = OxyColor.Parse(borderColor);
            PlatformPlotModel.Background = OxyColor.Parse(borderColor);

            GenrePlotModel.Padding = new OxyThickness(20, 20, 20, 20);
            PlatformPlotModel.Padding = new OxyThickness(20, 20, 20, 20);

            if (genreColors.Count == 0 || platformColors.Count == 0)
            {
                GenerateColorsForGenresAndPlatforms(games);
            }

            // Statistiques par genre
            var genreGroups = games.Where(game => !string.IsNullOrEmpty(game.Genre))
                                   .GroupBy(game => game.Genre)
                                   .Select(group => new { Genre = group.Key, Count = group.Count() })
                                   .ToList();

            GenreStats = genreGroups.Select(group => new StatData
            {
                Title = group.Genre ?? "Inconnu",
                Fill = genreColors.ContainsKey(group.Genre) ? genreColors[group.Genre] : "#808080"
            }).ToList();

            var genreSeries = new PieSeries
            {
                StrokeThickness = 2,
                InsideLabelPosition = 0.0,
                InsideLabelFormat = "",
                OutsideLabelFormat = "{0}",
                TextColor = OxyColor.FromRgb(255, 255, 255)
            };

            GenrePlotModel.Padding = new OxyThickness(20, 20, 20, 20);


            foreach (var group in genreGroups)
            {
                var genreColor = genreColors.ContainsKey(group.Genre) ? genreColors[group.Genre] : "#808080";
                genreSeries.Slices.Add(new PieSlice(group.Genre ?? "Inconnu", group.Count)
                {
                    Fill = OxyColor.Parse(genreColor)
                });
            }

            GenrePlotModel.Series.Add(genreSeries);

            // Statistiques par plateforme
            var platformGroups = games.Where(game => !string.IsNullOrEmpty(game.Plateforme))
                                      .GroupBy(game => game.Plateforme)
                                      .Select(group => new { Platform = group.Key, Count = group.Count() })
                                      .ToList();

            PlatformStats = platformGroups.Select(group => new StatData
            {
                Title = group.Platform ?? "Inconnu",
                Fill = platformColors.ContainsKey(group.Platform) ? platformColors[group.Platform] : "#808080"
            }).ToList();

            var platformSeries = new PieSeries
            {
                StrokeThickness = 2,
                InsideLabelPosition = 0.8,
                InsideLabelFormat = "",
                OutsideLabelFormat = "{0}",
                TextColor = OxyColor.FromRgb(255, 255, 255)
            };

            foreach (var group in platformGroups)
            {
                var platformColor = platformColors.ContainsKey(group.Platform) ? platformColors[group.Platform] : "#808080";
                platformSeries.Slices.Add(new PieSlice(group.Platform ?? "Inconnu", group.Count)
                {
                    Fill = OxyColor.Parse(platformColor)
                });
            }

            PlatformPlotModel.Series.Add(platformSeries);

            // Nombre de jeux terminés
            FinishedGames = games.Count(game => game.status != null && game.status.Status_name == "terminé" || game.status.Status_name == "terminer");

            // Calcul du nombre total de jeux
            TotalGames = games.Count;
        }

        // Fonction pour générer les couleurs aléatoires
        private void GenerateColorsForGenresAndPlatforms(IEnumerable<Game_Table> games)
        {
            var random = new Random();

            // Générer une couleur pour chaque genre unique
            var genres = games.Select(game => game.Genre).Distinct().Where(genre => !string.IsNullOrEmpty(genre));
            foreach (var genre in genres)
            {
                string color = $"#{random.Next(0x1000000):X6}";
                genreColors[genre] = color;
            }

            // Générer une couleur pour chaque plateforme unique
            var platforms = games.Select(game => game.Plateforme).Distinct().Where(platform => !string.IsNullOrEmpty(platform));
            foreach (var platform in platforms)
            {
                string color = $"#{random.Next(0x1000000):X6}";
                platformColors[platform] = color;
            }
        }
    }
}

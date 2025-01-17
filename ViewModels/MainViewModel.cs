using OxyPlot;
using OxyPlot.Series;
using Projet_DesktopDev_Antoine_Richard.Class_DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;

namespace Projet_DesktopDev_Antoine_Richard.ViewModels
{
    public class StatData
    {
        public string Title { get; set; }
        public string Fill { get; set; }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        private int _finishedGames;
        public int FinishedGames
        {
            get { return _finishedGames; }
            set
            {
                if (_finishedGames != value)
                {
                    _finishedGames = value;
                    OnPropertyChanged(nameof(FinishedGames));
                }
            }
        }

        private int _totalGames;
        public int TotalGames
        {
            get { return _totalGames; }
            set
            {
                if (_totalGames != value)
                {
                    _totalGames = value;
                    OnPropertyChanged(nameof(TotalGames));
                }
            }
        }

        public PlotModel GenrePlotModel { get; set; }
        public PlotModel PlatformPlotModel { get; set; }
        public List<StatData> GenreStats { get; set; }
        public List<StatData> PlatformStats { get; set; }

        private static Dictionary<string, string> genreColors = new Dictionary<string, string>();
        private static Dictionary<string, string> platformColors = new Dictionary<string, string>();

        public MainViewModel()
        {
            GenrePlotModel = new PlotModel();
            PlatformPlotModel = new PlotModel();

            var games = SelectGame.GetAllGames() ?? new List<Game_Table>();

            string borderColor = "#3E527D";
            GenrePlotModel.Background = OxyColor.Parse(borderColor);
            PlatformPlotModel.Background = OxyColor.Parse(borderColor);

            RestoreSavedColors();

            if (genreColors.Count == 0 || platformColors.Count == 0)
            {
                GenerateColorsForGenresAndPlatforms(games);
                SaveColors();
            }

            loadStatGenre();
            loadStatPlateforme();
        }

        private void loadStatGenre()
        {
            var games = SelectGame.GetAllGames() ?? new List<Game_Table>();
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
        }
        private void loadStatPlateforme()
        {
            var games = SelectGame.GetAllGames() ?? new List<Game_Table>();
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
                InsideLabelPosition = 0.0,
                InsideLabelFormat = "",
                OutsideLabelFormat = "{0}",
                TextColor = OxyColor.FromRgb(255, 255, 255)
            };

            PlatformPlotModel.Padding = new OxyThickness(20, 20, 20, 20);

            foreach (var group in platformGroups)
            {
                var platformColor = platformColors.ContainsKey(group.Platform) ? platformColors[group.Platform] : "#808080";
                platformSeries.Slices.Add(new PieSlice(group.Platform ?? "Inconnu", group.Count)
                {
                    Fill = OxyColor.Parse(platformColor)
                });
            }

            PlatformPlotModel.Series.Add(platformSeries);
        }

        private void GenerateColorsForGenresAndPlatforms(IEnumerable<Game_Table> games)
        {
            var random = new Random();
            foreach (var genre in games.Select(game => game.Genre).Distinct())
                genreColors[genre] = $"#{random.Next(0x1000000):X6}";
            foreach (var platform in games.Select(game => game.Plateforme).Distinct())
                platformColors[platform] = $"#{random.Next(0x1000000):X6}";
        }

        private void SaveColors()
        {
            Properties.Settings.Default.SavedGenreColors = JsonSerializer.Serialize(genreColors);
            Properties.Settings.Default.SavedPlatformColors = JsonSerializer.Serialize(platformColors);
            Properties.Settings.Default.Save();
        }

        private void RestoreSavedColors()
        {
            var savedGenreColors = Properties.Settings.Default.SavedGenreColors;
            var savedPlatformColors = Properties.Settings.Default.SavedPlatformColors;

            if (!string.IsNullOrEmpty(savedGenreColors))
                genreColors = JsonSerializer.Deserialize<Dictionary<string, string>>(savedGenreColors);
            if (!string.IsNullOrEmpty(savedPlatformColors))
                platformColors = JsonSerializer.Deserialize<Dictionary<string, string>>(savedPlatformColors);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

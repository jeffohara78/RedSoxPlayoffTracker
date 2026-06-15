using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace RedSoxPlayoffTracker
{
    public class PlayoffTrackerManager
    {
        private List<PlayoffSeries> seriesList = new List<PlayoffSeries>();
        private List<PlayoffGame> games = new List<PlayoffGame>();

        private int nextSeriesId = 1001;
        private int nextGameId = 5001;

        private string seriesFilePath = "playoffSeries.json";
        private string gamesFilePath = "playoffGames.json";

        public PlayoffTrackerManager()
        {
            LoadDataFromFiles();
        }

        // UPDATED 06/15/2026:
        // Wild Card is now treated as a single elimination game.
        // ALDS is best of 5.
        // ALCS and World Series are best of 7.
        public void AddSeries()
        {
            Console.WriteLine("\n======================================");
            Console.WriteLine("          ADD PLAYOFF SERIES");
            Console.WriteLine("======================================");
            Console.WriteLine("Use this screen to create a Boston Red Sox");
            Console.WriteLine("playoff matchup.");
            Console.WriteLine();
            Console.WriteLine("The app will automatically assign the format:");
            Console.WriteLine("- Wild Card: Single elimination game");
            Console.WriteLine("- ALDS: Best of 5");
            Console.WriteLine("- ALCS: Best of 7");
            Console.WriteLine("- World Series: Best of 7");
            Console.WriteLine();
            Console.WriteLine("Enter 0 at any prompt to cancel without saving.");
            Console.WriteLine();

            string roundName = GetRoundFromUser();

            if (roundName == "Cancel")
            {
                Console.WriteLine("Add series cancelled.");
                return;
            }

            string opponent = GetTextOrCancel("Opponent team name, such as Yankees, Rays, or Dodgers: ");

            if (opponent == "0")
            {
                Console.WriteLine("Add series cancelled.");
                return;
            }

            int bestOf = GetAutomaticSeriesLength(roundName);

            PlayoffSeries series = new PlayoffSeries(
                nextSeriesId,
                roundName,
                opponent,
                bestOf
            );

            seriesList.Add(series);

            SaveDataToFiles();

            Console.WriteLine("\nPlayoff matchup added successfully.");
            Console.WriteLine($"Series ID: {nextSeriesId}");
            Console.WriteLine($"Round: {roundName}");
            Console.WriteLine($"Opponent: {opponent}");
            Console.WriteLine($"Format: {GetSeriesFormatText(bestOf)}");
            Console.WriteLine($"Wins Needed: {series.WinsNeeded()}");

            nextSeriesId++;
        }

        // UPDATED 06/15/2026:
        // Displays single-game Wild Card matchups correctly.
        public void ViewSeries()
        {
            Console.WriteLine("\n======================================");
            Console.WriteLine("          PLAYOFF SERIES LIST");
            Console.WriteLine("======================================");

            if (seriesList.Count == 0)
            {
                Console.WriteLine("No playoff matchups have been added yet.");
                return;
            }

            foreach (PlayoffSeries series in seriesList)
            {
                Console.WriteLine("\n------------------------------");
                Console.WriteLine($"Series ID: {series.SeriesId}");
                Console.WriteLine($"Round: {series.RoundName}");
                Console.WriteLine($"Opponent: {series.Opponent}");
                Console.WriteLine($"Format: {GetSeriesFormatText(series.BestOf)}");
                Console.WriteLine($"Wins Needed: {series.WinsNeeded()}");
            }
        }

        public void AddGameResult()
        {
            Console.WriteLine("\n======================================");
            Console.WriteLine("          ADD GAME RESULT");
            Console.WriteLine("======================================");
            Console.WriteLine("Use this screen after a playoff game has");
            Console.WriteLine("finished to record the final score and");
            Console.WriteLine("update the Red Sox playoff performance.");
            Console.WriteLine();
            Console.WriteLine("Enter 0 at any prompt to cancel without saving.");
            Console.WriteLine();

            if (seriesList.Count == 0)
            {
                Console.WriteLine("Add a playoff series before adding game results.");
                return;
            }

            DisplaySeriesSummary();

            int seriesId = GetIntOrCancel("\nEnter Series ID for this game, or 0 to cancel: ");

            if (seriesId == 0)
            {
                Console.WriteLine("Add game result cancelled.");
                return;
            }

            PlayoffSeries series = seriesList.Find(item => item.SeriesId == seriesId);

            if (series == null)
            {
                Console.WriteLine("No series with that ID was found.");
                return;
            }

            int gameNumber = GetIntOrCancel("Game number in the series, such as 1, 2, or 3: ");

            if (gameNumber == 0)
            {
                Console.WriteLine("Add game result cancelled.");
                return;
            }

            int redSoxScore = GetIntOrCancel("Boston Red Sox final score: ");

            if (redSoxScore == 0)
            {
                Console.WriteLine("Add game result cancelled.");
                return;
            }

            int opponentScore = GetScoreAllowZero("Opponent final score: ");

            string location = GetLocationFromUser();

            if (location == "Cancel")
            {
                Console.WriteLine("Add game result cancelled.");
                return;
            }

            string notes = GetTextOrCancel("Game notes, or press ENTER to leave blank: ");

            if (notes == "0")
            {
                Console.WriteLine("Add game result cancelled.");
                return;
            }

            PlayoffGame game = new PlayoffGame(
                nextGameId,
                seriesId,
                gameNumber,
                series.Opponent,
                redSoxScore,
                opponentScore,
                location,
                notes
            );

            games.Add(game);
            SaveDataToFiles();

            Console.WriteLine("\nGame result saved successfully.");
            Console.WriteLine($"Game ID: {nextGameId}");
            Console.WriteLine($"Red Sox {redSoxScore} - {series.Opponent} {opponentScore}");
            Console.WriteLine($"Result: {(game.DidRedSoxWin() ? "Win" : "Loss")}");
            Console.WriteLine($"Run Differential: {game.GetRunDifferential()}");

            nextGameId++;
        }

        public void ViewAllGames()
        {
            Console.WriteLine("\n======================================");
            Console.WriteLine("          ALL PLAYOFF GAMES");
            Console.WriteLine("======================================");

            if (games.Count == 0)
            {
                Console.WriteLine("No playoff games have been added yet.");
                return;
            }

            foreach (PlayoffGame game in games)
            {
                DisplayGame(game);
            }
        }

        public void ViewSeriesReport()
        {
            Console.WriteLine("\n======================================");
            Console.WriteLine("           SERIES REPORT");
            Console.WriteLine("======================================");

            if (seriesList.Count == 0)
            {
                Console.WriteLine("No playoff series have been added yet.");
                return;
            }

            DisplaySeriesSummary();

            int seriesId = GetIntOrCancel("\nEnter Series ID to view report, or 0 to cancel: ");

            if (seriesId == 0)
            {
                Console.WriteLine("Series report cancelled.");
                return;
            }

            PlayoffSeries series = seriesList.Find(item => item.SeriesId == seriesId);

            if (series == null)
            {
                Console.WriteLine("No series with that ID was found.");
                return;
            }

            int wins = 0;
            int losses = 0;
            int runsScored = 0;
            int runsAllowed = 0;

            foreach (PlayoffGame game in games)
            {
                if (game.SeriesId == seriesId)
                {
                    if (game.DidRedSoxWin())
                    {
                        wins++;
                    }
                    else
                    {
                        losses++;
                    }

                    runsScored += game.RedSoxScore;
                    runsAllowed += game.OpponentScore;
                }
            }

            int gamesPlayed = wins + losses;
            int runDifferential = runsScored - runsAllowed;
            int winsNeeded = series.WinsNeeded();
            int winsRemaining = winsNeeded - wins;

            Console.WriteLine("\n======================================");
            Console.WriteLine("        RED SOX SERIES SUMMARY");
            Console.WriteLine("======================================");
            Console.WriteLine($"Round: {series.RoundName}");
            Console.WriteLine($"Opponent: {series.Opponent}");
            Console.WriteLine($"Format: Best of {series.BestOf}");
            Console.WriteLine($"Series Record: Red Sox {wins} - {series.Opponent} {losses}");
            Console.WriteLine($"Games Played: {gamesPlayed}");
            Console.WriteLine($"Runs Scored: {runsScored}");
            Console.WriteLine($"Runs Allowed: {runsAllowed}");
            Console.WriteLine($"Run Differential: {runDifferential}");
            Console.WriteLine();

            if (wins >= winsNeeded)
            {
                Console.WriteLine("Series Status: Red Sox have won the series.");
            }
            else if (losses >= winsNeeded)
            {
                Console.WriteLine("Series Status: Red Sox have lost the series.");
            }
            else
            {
                Console.WriteLine($"Series Status: Red Sox need {winsRemaining} more win(s) to advance.");
            }
        }

        public void ViewPlayoffDashboard()
        {
            Console.WriteLine("\n======================================");
            Console.WriteLine("          PLAYOFF DASHBOARD");
            Console.WriteLine("======================================");

            int totalWins = 0;
            int totalLosses = 0;
            int totalRunsScored = 0;
            int totalRunsAllowed = 0;
            int homeGames = 0;
            int awayGames = 0;

            foreach (PlayoffGame game in games)
            {
                if (game.DidRedSoxWin())
                {
                    totalWins++;
                }
                else
                {
                    totalLosses++;
                }

                totalRunsScored += game.RedSoxScore;
                totalRunsAllowed += game.OpponentScore;

                if (game.Location == "Home")
                {
                    homeGames++;
                }
                else if (game.Location == "Away")
                {
                    awayGames++;
                }
            }

            int totalGames = totalWins + totalLosses;
            int runDifferential = totalRunsScored - totalRunsAllowed;

            decimal winningPercentage = totalGames > 0
                ? ((decimal)totalWins / totalGames) * 100
                : 0;

            decimal averageRunsScored = totalGames > 0
                ? (decimal)totalRunsScored / totalGames
                : 0;

            decimal averageRunsAllowed = totalGames > 0
                ? (decimal)totalRunsAllowed / totalGames
                : 0;

            Console.WriteLine($"Total Series Tracked: {seriesList.Count}");
            Console.WriteLine($"Total Games Played: {totalGames}");
            Console.WriteLine($"Overall Record: {totalWins}-{totalLosses}");
            Console.WriteLine($"Winning Percentage: {winningPercentage:F1}%");
            Console.WriteLine();
            Console.WriteLine("--- Run Production ---");
            Console.WriteLine($"Runs Scored: {totalRunsScored}");
            Console.WriteLine($"Runs Allowed: {totalRunsAllowed}");
            Console.WriteLine($"Run Differential: {runDifferential}");
            Console.WriteLine($"Average Runs Scored: {averageRunsScored:F1}");
            Console.WriteLine($"Average Runs Allowed: {averageRunsAllowed:F1}");
            Console.WriteLine();
            Console.WriteLine("--- Location Summary ---");
            Console.WriteLine($"Home Games: {homeGames}");
            Console.WriteLine($"Away Games: {awayGames}");

            Console.WriteLine();
            Console.WriteLine("--- Performance Summary ---");

            if (totalGames == 0)
            {
                Console.WriteLine("No playoff games have been recorded yet.");
            }
            else if (winningPercentage >= 70 && runDifferential > 0)
            {
                Console.WriteLine("The Red Sox are performing strongly in the playoffs.");
            }
            else if (winningPercentage >= 50)
            {
                Console.WriteLine("The Red Sox are competitive, but the series remains tight.");
            }
            else
            {
                Console.WriteLine("The Red Sox need improved performance to stay alive.");
            }
        }

        // NEW 06/15/2026:
        // Converts the playoff series length into user-friendly wording.
        private string GetSeriesFormatText(int bestOf)
        {
            if (bestOf == 1)
            {
                return "Single Elimination Game";
            }

            return $"Best of {bestOf}";
        }

        // NEW 06/15/2026:
        // Automatically assigns the correct playoff format
        // based on the round selected by the user.
        private int GetAutomaticSeriesLength(string roundName)
        {
            if (roundName == "Wild Card")
            {
                return 1;
            }
            else if (roundName == "ALDS")
            {
                return 5;
            }
            else
            {
                return 7;
            }
        }

        // UPDATED 06/15/2026:
        // Uses "Wild Card" instead of "Wild Card Series"
        // because this version treats Wild Card as one game.
        private string GetRoundFromUser()
        {
            while (true)
            {
                Console.WriteLine("Choose playoff round:");
                Console.WriteLine("1. Wild Card");
                Console.WriteLine("2. ALDS");
                Console.WriteLine("3. ALCS");
                Console.WriteLine("4. World Series");
                Console.WriteLine("0. Cancel and return to main menu");
                Console.Write("Choose option 0 through 4: ");

                string choice = Console.ReadLine();

                if (choice == "1") return "Wild Card";
                if (choice == "2") return "ALDS";
                if (choice == "3") return "ALCS";
                if (choice == "4") return "World Series";
                if (choice == "0") return "Cancel";

                Console.WriteLine("Invalid option. Please choose 0 through 4.");
            }
        }

        
        private string GetLocationFromUser()
        {
            while (true)
            {
                Console.WriteLine("\nChoose game location:");
                Console.WriteLine("1. Home");
                Console.WriteLine("2. Away");
                Console.WriteLine("0. Cancel and return to main menu");
                Console.Write("Choose option 0 through 2: ");

                string choice = Console.ReadLine();

                if (choice == "1") return "Home";
                if (choice == "2") return "Away";
                if (choice == "0") return "Cancel";

                Console.WriteLine("Invalid option. Please choose 0 through 2.");
            }
        }

        private string GetTextOrCancel(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine().Trim();
        }

        private int GetIntOrCancel(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);

                string input = Console.ReadLine().Trim();

                if (input == "0")
                {
                    return 0;
                }

                bool isValidNumber = int.TryParse(input, out int number);

                if (isValidNumber && number > 0)
                {
                    return number;
                }

                Console.WriteLine("Invalid number. Enter a positive whole number, or 0 to cancel.");
            }
        }

        private int GetScoreAllowZero(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);

                string input = Console.ReadLine().Trim();

                bool isValidNumber = int.TryParse(input, out int score);

                if (isValidNumber && score >= 0)
                {
                    return score;
                }

                Console.WriteLine("Invalid score. Enter 0 or a positive whole number.");
            }
        }

        // UPDATED 06/15/2026:
        // Shows a cleaner matchup summary with the correct playoff format.
        private void DisplaySeriesSummary()
        {
            Console.WriteLine("\n--- Current Playoff Matchups ---");

            foreach (PlayoffSeries series in seriesList)
            {
                Console.WriteLine(
                    $"Series ID: {series.SeriesId} | {series.RoundName} vs {series.Opponent} | {GetSeriesFormatText(series.BestOf)}"
                );
            }
        }

        private void DisplayGame(PlayoffGame game)
        {
            Console.WriteLine("\n------------------------------");
            Console.WriteLine($"Game ID: {game.GameId}");
            Console.WriteLine($"Series ID: {game.SeriesId}");
            Console.WriteLine($"Game Number: {game.GameNumber}");
            Console.WriteLine($"Opponent: {game.Opponent}");
            Console.WriteLine($"Score: Red Sox {game.RedSoxScore} - {game.Opponent} {game.OpponentScore}");
            Console.WriteLine($"Result: {(game.DidRedSoxWin() ? "Win" : "Loss")}");
            Console.WriteLine($"Run Differential: {game.GetRunDifferential()}");
            Console.WriteLine($"Location: {game.Location}");
            Console.WriteLine($"Date Entered: {game.GameDate}");
            Console.WriteLine($"Notes: {game.Notes}");
        }

        private void SaveDataToFiles()
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string seriesJson = JsonSerializer.Serialize(seriesList, options);
            string gamesJson = JsonSerializer.Serialize(games, options);

            File.WriteAllText(seriesFilePath, seriesJson);
            File.WriteAllText(gamesFilePath, gamesJson);
        }

        private void LoadDataFromFiles()
        {
            if (File.Exists(seriesFilePath))
            {
                string seriesJson = File.ReadAllText(seriesFilePath);

                if (!string.IsNullOrWhiteSpace(seriesJson))
                {
                    seriesList = JsonSerializer.Deserialize<List<PlayoffSeries>>(seriesJson);

                    if (seriesList == null)
                    {
                        seriesList = new List<PlayoffSeries>();
                    }
                }
            }

            if (File.Exists(gamesFilePath))
            {
                string gamesJson = File.ReadAllText(gamesFilePath);

                if (!string.IsNullOrWhiteSpace(gamesJson))
                {
                    games = JsonSerializer.Deserialize<List<PlayoffGame>>(gamesJson);

                    if (games == null)
                    {
                        games = new List<PlayoffGame>();
                    }
                }
            }

            foreach (PlayoffSeries series in seriesList)
            {
                if (series.SeriesId >= nextSeriesId)
                {
                    nextSeriesId = series.SeriesId + 1;
                }
            }

            foreach (PlayoffGame game in games)
            {
                if (game.GameId >= nextGameId)
                {
                    nextGameId = game.GameId + 1;
                }
            }
        }
    }
}
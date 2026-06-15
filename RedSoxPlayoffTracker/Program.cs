/* Jeff O'Hara
 * 6/15/2026
 * 
 * Allows users to create playoff matchups, record game results, and track the team's performance throughout the postseason. 
 * The application calculates series standings, win-loss records, run differential, scoring trends, and overall playoff 
 * statistics through detailed reports and dashboard summaries.
 */


using System;

namespace RedSoxPlayoffTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            PlayoffTrackerManager manager = new PlayoffTrackerManager();

            bool running = true;

            while (running)
            {
                Console.WriteLine("\n==========================================");
                Console.WriteLine("        BOSTON RED SOX PLAYOFF TRACKER");
                Console.WriteLine("==========================================");
                Console.WriteLine("Track playoff series, game results, wins,");
                Console.WriteLine("losses, run differential, and performance trends.");
                Console.WriteLine();
                Console.WriteLine("1. Add playoff series");
                Console.WriteLine("2. View playoff series");
                Console.WriteLine("3. Add game result");
                Console.WriteLine("4. View all games");
                Console.WriteLine("5. View series report");
                Console.WriteLine("6. View playoff dashboard");
                Console.WriteLine("7. Exit");
                Console.Write("\nChoose an option 1 through 7: ");

                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    manager.AddSeries();
                }
                else if (choice == "2")
                {
                    manager.ViewSeries();
                }
                else if (choice == "3")
                {
                    manager.AddGameResult();
                }
                else if (choice == "4")
                {
                    manager.ViewAllGames();
                }
                else if (choice == "5")
                {
                    manager.ViewSeriesReport();
                }
                else if (choice == "6")
                {
                    manager.ViewPlayoffDashboard();
                }
                else if (choice == "7")
                {
                    running = false;
                    Console.WriteLine("Exiting Boston Red Sox Playoff Tracker.");
                }
                else
                {
                    Console.WriteLine("Invalid option. Please choose 1 through 7.");
                }
            }
        }
    }
}
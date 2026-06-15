using System;

namespace RedSoxPlayoffTracker
{
    public class PlayoffGame
    { 
        public int GameId { get; set; }

        public int SeriesId { get; set; }

        public int GameNumber  { get; set; }

        public string Opponent {  get; set; }

        public int RedSoxScore { get; set; }

        public int OpponentScore { get; set; }

        public string Location { get; set; }

        public DateTime GameDate { get; set; }

        public string Notes { get; set; }

        public PlayoffGame()
        {
        }

        public PlayoffGame(int gameId, int seriesId, int gameNumber, string opponent, int redSoxScore, int opponentScore, string location, string notes)
        { 
            GameId = gameId;
            SeriesId = seriesId;
            GameNumber = gameNumber;
            Opponent = opponent;
            RedSoxScore = redSoxScore;
            OpponentScore = opponentScore;
            Location = location;
            GameDate = DateTime.Now;
            Notes = notes;
        }

        public bool DidRedSoxWin()
        {
            return RedSoxScore > OpponentScore;
        }

        public int GetRunDifferential()
        {
            return RedSoxScore - OpponentScore;
        }

    }
}
using System;

namespace RedSoxPlayoffTracker
{
    public class PlayoffSeries
    { 
        public int SeriesId { get; set; }

        public string RoundName { get; set; }

        public string Opponent {  get; set; }

        public int BestOf {  get; set; }

        public DateTime DateCreated { get; set; }

        public PlayoffSeries()
        { 
        }

        public PlayoffSeries(int seriesId, string roundName, string opponent, int bestOf)
        { 
            SeriesId = seriesId;
            RoundName = roundName;
            Opponent = opponent;
            BestOf = bestOf;
            DateCreated = DateTime.Now;
        }

        public int WinsNeeded()
        {
            if (BestOf == 1)
            {
                return 1;
            }

            return (BestOf / 2) + 1;
        }
    }
}
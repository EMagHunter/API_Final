using System;
namespace API_Final.Models
{
    public class Match
    {
        public int MatchId { get; set; }
        public int Player1 { get; set; }
        public int Player2 { get; set; }
        public int Winner { get; set; }
        public DateTime MatchTimestamp { get; set; }
    }
}

using System;
namespace API_Final.Models
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public bool IsOnline { get; set; }
        public string PlayerRegion { get; set; }
    }
}

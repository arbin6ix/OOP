using System;
using System.Collections.Generic;
using System.Text;

namespace OOP4200_Tarneeb.Models
{
    public class Stats
    {
        public Guid Id { get; set; }
        public string PlayerName { get; set; }
        public int NumGames { get; set; }
        public int NumWins { get; set; }
        public int NumLosses { get; set; }

        public Stats(Guid id, string playerName, int numGames, int numWins, int numLosses)
        {
            Id = id;
            PlayerName = playerName;
            NumGames = numGames;
            NumWins = numWins;
            NumLosses = numLosses;
        }
    }
}

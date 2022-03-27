using System;
using System.Collections.Generic;
using System.Text;

namespace OOP4200_Tarneeb.Cards
{
    class Teams
    {
        // Fields
        public Player player1 { get; set; }
        public Player player2 { get; set; }
        public int score { get; set; }

        // Constructor
        public Teams(Player player1, Player player2)
        {
            this.player1 = player1;
            this.player2 = player2;
            score = 0;
        }

        // No param constructor
        public Teams()
        {
            player1 = null;
            player2 = null;
            score = 0;
        }
    }
}

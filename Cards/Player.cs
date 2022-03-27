using System;
using System.Collections.Generic;
using System.Text;

namespace OOP4200_Tarneeb.Cards
{
    class Player
    {
        public List<Card> playersCards = new List<Card>();


        public Player(List<Card> playersCards)
        {
            this.playersCards = playersCards;
        }
    }
}

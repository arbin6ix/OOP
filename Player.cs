using System;
using System.Collections.Generic;
using System.Text;

namespace OOP4200_Tarneeb
{
    class Player
    {

        public string name { get; set; }
        public List<Cards.Card> playersCards = new List<Cards.Card>();

        public Player(string name, List<Cards.Card> playersCards)
        {
            this.name = name;
            this.playersCards = playersCards;
        }
    }
}

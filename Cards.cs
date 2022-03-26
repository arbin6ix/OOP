using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OOP4200_Tarneeb
{
    class Cards
    {
        public static class Enums
        {

            public enum Suit
            {
                Spade = 1,
                Club = 2,
                Diamond = 3,
                Heart = 4,
            }

            public enum CardNumber
            {
                Ace = 1,
                Two = 2,
                Three = 3,
                Four = 4,
                Five = 5,
                Six = 6,
                Seven = 7,
                Eight = 8,
                Nine = 9,
                Ten = 10,
                Jack = 11,
                Queen = 12,
                King = 13,
            }
        }

        public class Card
        {
            public Enums.Suit Suit { get; set; }
            public Enums.CardNumber CardNumber { get; set; }
        }

        public class Deck
        {
            public Deck()
            {
                Reset();
            }

            public List<Card> Cards { get; set; }

            public void Reset()
            {
                Cards = Enumerable.Range(1, 4)
                    .SelectMany(s => Enumerable.Range(1, 13)
                                        .Select(c => new Card()
                                        {
                                            Suit = (Enums.Suit)s,
                                            CardNumber = (Enums.CardNumber)c
                                        }
                                                )
                                )
                       .ToList();
            }

            public void Shuffle()
            {
                Cards = Cards.OrderBy(c => Guid.NewGuid())
                             .ToList();
            }

            public Card TakeCard()
            {
                var card = Cards.FirstOrDefault();
                Cards.Remove(card);

                return card;
            }

            public List<Card> TakeCards(int numberOfCards)
            {
                var cards = Cards.Take(numberOfCards);

                var takeCards = cards as List<Card> ?? cards.ToList();
                Cards.RemoveAll(takeCards.Contains);

                return takeCards;
            }

            public List<Card> Sort(List<Card> listOfCards)
            {
                List<Card> sorted = listOfCards.GroupBy(s => s.Suit).
                    OrderByDescending(c => c.Count()).SelectMany(g => g.OrderByDescending(c => c.CardNumber)).ToList();

                return sorted;

            }

        }
    }
}

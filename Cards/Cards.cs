using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OOP4200_Tarneeb.Cards
{
    public static class Enums
    {

        public enum Suit
        {
            CLUB = 1,
            DIAMOND = 2,
            HEART = 3,
            SPADE = 4,
        }

        public enum CardNumber
        {
            TWO = 1,
            THREE = 2,
            FOUR = 3,
            FIVE = 4,
            SIX = 5,
            SEVEN = 6,
            EIGHT = 7,
            NINE = 8,
            TEN = 9,
            JACK = 10,
            QUEEN = 11,
            KING = 12,
            ACE = 13

        }
    }

    public class Card
    {

        public Enums.Suit Suit { get; set; }
        public Enums.CardNumber CardNumber { get; set; }

        public override string ToString()
        {

            return "The Suit is: " + this.Suit + " The card number is: " + this.CardNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="card">The card to convert to image</param>
        /// <returns>ImageSource of entered card</returns>
        public static ImageSource ToImage(Card card)
        {
            string formattedSuit = "";
            string formattedNumber;

            // Determine suit char representation by card.Suit int enum value
            if ((int)card.Suit == 1)
            {
                formattedSuit = "c";
            }
            else if ((int)card.Suit == 2)
            {
                formattedSuit = "d";
            }
            else if ((int)card.Suit == 3)
            {
                formattedSuit = "h";
            }
            else if ((int)card.Suit == 4)
            {
                formattedSuit = "s";
            }

            // If card is between 2 and 9, prepend a "0"
            if ((int)card.CardNumber <= 8)
            {
                formattedNumber = "0" + ((int)card.CardNumber + 1);
            }
            // If card is 10 to KING, return number as is
            else if ((int)card.CardNumber <= 12)
            {
                formattedNumber = "" + ((int)card.CardNumber + 1);
            }
            // If card is ACE, return "01" since Ace is valued as 13 in the enum but 01 in the image .bmp
            else
            {
                formattedNumber = "01";
            }

            // Return the image source for the card
            return (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/" + formattedSuit + formattedNumber + ".bmp");
        }

        /// <summary>
        /// Clears card slot of any card image stored in it
        /// </summary>
        /// <returns>ImageSource of transparent (empty) card</returns>
        public static ImageSource ClearImage()
        {
            return (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/CardSlot.png");
        }
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

            //var takeCards = cards as Card[] ?? cards.ToArray();
            var takeCards = cards as List<Card> ?? cards.ToList();
            Cards.RemoveAll(takeCards.Contains);

            return takeCards;
        }

        public List<Card> Sort(List<Card> listOfCards)
        {
            List<Card> sorted = listOfCards
                .GroupBy(s => s.Suit)
                .OrderByDescending(c => c.Count())
                .SelectMany(g => g
                .OrderBy(c => c.CardNumber))
                .ToList();

            return sorted;

        }

    }
}


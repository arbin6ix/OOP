using OOP4200_Tarneeb.Cards;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace OOP4200_Tarneeb
{
    /// <summary>
    /// Interaction logic for PageGame.xaml
    /// </summary>
    public partial class PageGame : Page
    {
        // List of String that holds Tarneeb suit betting (1. Tarneeb Suit, 2. Bet number, 3. Player who betted the most) global variable
        public static List<String> tarneebSuit = new List<String>() { };

        // Tarneeb played is a global variable
        public static bool tarneebPlayed = false;

        public static Card cardToBeat;

        // Counter for remaining cards in the hand
        public static int cardsRemaining = 13;

        // Create a list of the player's cards Image controls from the PageGame.xaml form
        List<Image> playerCardImages = new List<Image>();

        // List of all player's cards
        List<Card> playerHand = new List<Card>();
        List<Card> hand2 = new List<Card>();
        List<Card> hand3 = new List<Card>();
        List<Card> hand4 = new List<Card>();

        // Random class object instantiation
        Random rand = new Random();


        public PageGame()
        {
            InitializeComponent();
            CreateImageList();
            NewRound();
        }

        /// <summary>
        /// Initiates a new round of Tarneeb, including shuffling deck, dealing cards, making teams, etc..
        /// </summary>
        public void NewRound()
        {
            // Create a deck
            var deck = new Deck();

            // Shuffle the deck
            deck.Shuffle();

            // Pass out 13 cards to each
            playerHand = deck.Sort(deck.TakeCards(13));
            hand2 = deck.Sort(deck.TakeCards(13));
            hand3 = deck.Sort(deck.TakeCards(13));
            hand4 = deck.Sort(deck.TakeCards(13));

            // Create 4 Players each with their hand of 13 shuffled cards
            Player player1 = new Player(playerHand);
            Player player2 = new Player(hand2);
            Player player3 = new Player(hand3);
            Player player4 = new Player(hand4);

            // Display the player's cards
            DisplayCards(playerHand);

            //Assign Players to Teams
            Teams firstTeam = new Teams(player1, player2);
            Teams secondTeam = new Teams(player3, player4);

            // Create a List of Players
            List<Player> playerList = new List<Player> { player1, player2, player3, player4 };
        }

        /// <summary>
        /// Displays the list of cards given
        /// </summary>
        /// <param name="hand">List of Cards to display</param>
        public void DisplayCards(List<Card> hand)
        {
            // Display player 1's card images in the Image controls
            for (int i = 0; i < playerHand.Count; i++)
            {
                playerCardImages[i].Source = Card.ToImage(hand[i]);
            }

            // If player's hand is less than 13, set remaining card image controls to null
            for (int i = playerHand.Count; i < 13; i++)
            {
                playerCardImages[i].Source = null;
            }
        }

        /// <summary>
        /// Create a list of the player's cards Image controls from the PageGame.xaml form
        /// </summary>
        public void CreateImageList()
        {
            // Add all Image controls to the list
            playerCardImages.Add(p01);
            playerCardImages.Add(p02);
            playerCardImages.Add(p03);
            playerCardImages.Add(p04);
            playerCardImages.Add(p05);
            playerCardImages.Add(p06);
            playerCardImages.Add(p07);
            playerCardImages.Add(p08);
            playerCardImages.Add(p09);
            playerCardImages.Add(p10);
            playerCardImages.Add(p11);
            playerCardImages.Add(p12);
            playerCardImages.Add(p13);
        }

        /// <summary>
        /// Completes the turns of the computer players 2-4. This function is async
        /// so that I can wait a certain amount of time between/after turns
        /// </summary>
        /// <returns></returns>
        public async Task ComputerTurns()
        {
            Card chosenCard;

            // Wait X milliseconds
            await Task.Delay(500);

            // Play card for AI player 2 using AI logic
            chosenCard = AIChooseCard(hand2);
            playedCard2.Source = Card.ToImage(chosenCard);
            hand2.RemoveAll(card => card.CardNumber == chosenCard.CardNumber && card.Suit == chosenCard.Suit);

            // Wait X milliseconds
            await Task.Delay(500);

            // Play card for AI player 3 using AI logic
            chosenCard = AIChooseCard(hand3);
            playedCard3.Source = Card.ToImage(chosenCard);
            hand3.RemoveAll(card => card.CardNumber == chosenCard.CardNumber && card.Suit == chosenCard.Suit);

            // Wait X milliseconds
            await Task.Delay(500);

            // Play card for AI player 4 using AI logic
            chosenCard = AIChooseCard(hand4);
            playedCard4.Source = Card.ToImage(chosenCard);
            hand4.RemoveAll(card => card.CardNumber == chosenCard.CardNumber && card.Suit == chosenCard.Suit);

            // Wait X milliseconds before end of turn
            await Task.Delay(2500);

            // Clear all cards played
            playedCard1.Source = null;
            playedCard2.Source = null;
            playedCard3.Source = null;
            playedCard4.Source = null;

            // Refresh card display with remaining cards in hand
            DisplayCards(playerHand);
        }

        /// <summary>
        /// Returns the AI's choice of card to play with given hand
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public Card AIChooseCard(List<Card> hand)
        {
            // Properties of card that has been played by the player
            int suitPlayed = (int)cardToBeat.Suit;
            int numPlayed = (int)cardToBeat.CardNumber;

            // The card chosen by the AI to play
            Card chosenCard = new Card();

            // Create new list of cards to hold the cards that match the suit played
            List<Card> matchingSuits = new List<Card>();

            // Loop through the hand, adding any matching cards to the new list
            for (int i = 0; i < hand.Count; i++)
            {
                if ((int)hand[i].Suit == suitPlayed)
                {
                    matchingSuits.Add(hand[i]);
                }
            }

            // Loop through the new list of matching cards...
            for (int i = 0; i < matchingSuits.Count; i++)
            {
                // If a card hasn't been chosen, or if the current card beats the card played AND
                // the current card does NOT beat the card played (so as to not waste a better card)
                if (i == 0 
                    || numPlayed < (int)matchingSuits[i].CardNumber
                    && (int)matchingSuits[i].CardNumber < (int)chosenCard.CardNumber)
                {
                    // Choose the current card to play
                    chosenCard = matchingSuits[i];
                }
            }

            // If there are no cards with a matching suit...
            if (matchingSuits.Count == 0)
            {
                // ...loop through the remaining cards and pick out the lowest value card
                for (int i = 0; i < hand.Count; i++)
                {
                    if (i == 0 || (int)chosenCard.Suit > (int)hand[i].Suit)
                    {
                        chosenCard = hand[i];
                    }
                }
            }



            return chosenCard;
        }

        /// <summary>
        /// Sends user back to main menu (exits current game)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGameBackClick(object sender, RoutedEventArgs e)
        {
            PageMenu menuPage = new PageMenu();
            NavigationService.Navigate(menuPage);
        }


        #region Card Click Functionality


        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void card01MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p01.Source != null && playedCard1.Source == null)
            {
                // ...play the card.
                playedCard1.Source = p01.Source;
                p01.Source = null;

                // Remove card from hand
                cardToBeat = playerHand[0];
                playerHand.RemoveAt(0);

                // Complete computer turns (async)
                ComputerTurns();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void card02MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p02.Source != null && playedCard1.Source == null)
            {
                // ...play the card.
                playedCard1.Source = p02.Source;
                p02.Source = null;

                // Remove card from hand
                cardToBeat = playerHand[1];
                playerHand.RemoveAt(1);

                // Complete computer turns (async)
                ComputerTurns();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void card03MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p03.Source != null && playedCard1.Source == null)
            {
                // ...play the card.
                playedCard1.Source = p03.Source;
                p03.Source = null;

                // Remove card from hand
                cardToBeat = playerHand[2];
                playerHand.RemoveAt(2);

                // Complete computer turns (async)
                ComputerTurns();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void card04MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p04.Source != null && playedCard1.Source == null)
            {
                // ...play the card.
                playedCard1.Source = p04.Source;
                p04.Source = null;


                // Remove card from hand
                cardToBeat = playerHand[3];
                playerHand.RemoveAt(3);

                // Complete computer turns (async)
                ComputerTurns();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void card05MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p05.Source != null && playedCard1.Source == null)
            {
                // ...play the card.
                playedCard1.Source = p05.Source;
                p05.Source = null;

                // Remove card from hand
                cardToBeat = playerHand[4];
                playerHand.RemoveAt(4);

                // Complete computer turns (async)
                ComputerTurns();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void card06MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p06.Source != null && playedCard1.Source == null)
            {
                // ...play the card.
                playedCard1.Source = p06.Source;
                p06.Source = null;

                // Remove card from hand
                cardToBeat = playerHand[5];
                playerHand.RemoveAt(5);

                // Complete computer turns (async)
                ComputerTurns();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void card07MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p07.Source != null && playedCard1.Source == null)
            {
                // ...play the card.
                playedCard1.Source = p07.Source;
                p07.Source = null;

                // Remove card from hand
                cardToBeat = playerHand[6];
                playerHand.RemoveAt(6);

                // Complete computer turns (async)
                ComputerTurns();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void card08MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p08.Source != null && playedCard1.Source == null)
            {
                // ...play the card.
                playedCard1.Source = p08.Source;
                p08.Source = null;

                // Remove card from hand
                cardToBeat = playerHand[7];
                playerHand.RemoveAt(7);

                // Complete computer turns (async)
                ComputerTurns();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void card09MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p09.Source != null && playedCard1.Source == null)
            {
                // ...play the card.
                playedCard1.Source = p09.Source;
                p09.Source = null;

                // Remove card from hand
                cardToBeat = playerHand[8];
                playerHand.RemoveAt(8);

                // Complete computer turns (async)
                ComputerTurns();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void card10MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p10.Source != null && playedCard1.Source == null)
            {
                // ...play the card.
                playedCard1.Source = p10.Source;
                p10.Source = null;

                // Remove card from hand
                cardToBeat = playerHand[9];
                playerHand.RemoveAt(9);

                // Complete computer turns (async)
                ComputerTurns();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void card11MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p11.Source != null && playedCard1.Source == null)
            {
                // ...play the card.
                playedCard1.Source = p11.Source;
                p11.Source = null;

                // Remove card from hand
                cardToBeat = playerHand[10];
                playerHand.RemoveAt(10);

                // Complete computer turns (async)
                ComputerTurns();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void card12MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p12.Source != null && playedCard1.Source == null)
            {
                // ...play the card.
                playedCard1.Source = p12.Source;
                p12.Source = null;

                // Remove card from hand
                cardToBeat = playerHand[11];
                playerHand.RemoveAt(11);

                // Complete computer turns (async)
                ComputerTurns();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void card13MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p13.Source != null && playedCard1.Source == null)
            {
                // ...play the card.
                playedCard1.Source = p13.Source;
                p13.Source = null;

                // Remove card from hand
                cardToBeat = playerHand[12];
                playerHand.RemoveAt(12);

                // Complete computer turns (async)
                ComputerTurns();
            }
        }


        #endregion


    }
}

using OOP4200_Tarneeb.Cards;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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

        public string message = "";

        // Tarneeb played is a global variable
        public static bool tarneebPlayed = false;

        // The first card played in the round
        public static Card firstCard;

        // The best card played in the round
        public static Card cardToBeat;

        // The winner of the betting or the round
        // winner = 0 means new round (betting)
        public static int winner = 1;

        // Counter for remaining cards in the hand
        public static int cardsDone = 0;

        // Bool for player's turn completed and round completed
        public static bool playerDone = false;
        public static bool roundDone = false;

        // Team Colours
        public SolidColorBrush team1Color = new SolidColorBrush(Color.FromRgb(51, 188, 255));
        public SolidColorBrush team2Color = new SolidColorBrush(Color.FromRgb(255, 90, 90));

        // Create a list of the player's cards Image controls from the PageGame.xaml form
        public List<Image> playerCardImages = new List<Image>();

        // List of all player's cards
        public List<Card> playerHand = new List<Card>();
        public List<Card> hand2 = new List<Card>();
        public List<Card> hand3 = new List<Card>();
        public List<Card> hand4 = new List<Card>();

        // Random class object instantiation
        public Random rand = new Random();

        // Played Cards each turn
        public Card player1Card = new Card();
        public Card player2Card = new Card();
        public Card player3Card = new Card();
        public Card player4Card = new Card();

        // Tarneeb (Trump)
        public Cards.Enums.Suit tarneeb;

        // Create 4 Players each with their hand of 13 shuffled cards
        Player player1 = new Player();
        Player player2 = new Player();
        Player player3 = new Player();
        Player player4 = new Player();

        // Team round scores
        public int team1Score = 0;
        public int team2Score = 0;

        // Team total scores (game is to 31)
        public int team1Total = 0;
        public int team2Total = 0;



        public PageGame()
        {
            InitializeComponent();

            // Create both teams
            Team team1 = new Team();
            Team team2 = new Team();
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
            Team team1 = new Team(player1, player2);
            Team team2 = new Team(player3, player4);

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
        /// Updates the score labels for Team 1's score
        /// </summary>
        public void UpdateTeam1Score()
        {
            lblTeam1Score1.Content = team1Score;
            lblTeam1Score2.Content = team1Score;
            lblTeam1Score3.Content = team1Score;
            lblTeam1Score4.Content = team1Score;
            lblTeam1Score5.Content = team1Score;
        }

        /// <summary>
        /// Updates the score labels for Team 2's score
        /// </summary>
        public void UpdateTeam2Score()
        {
            lblTeam2Score1.Content = team2Score;
            lblTeam2Score2.Content = team2Score;
            lblTeam2Score3.Content = team2Score;
            lblTeam2Score4.Content = team2Score;
            lblTeam2Score5.Content = team2Score;
        }

        #region ComputerTurnLogic

        /// <summary>
        /// Completes the turns of the computer players 2-4. This function is async
        /// so that I can wait a certain amount of time between/after turns
        /// </summary>
        /// <returns></returns>
        public void ComputerTurns()
        {
            if (!roundDone)
            {
                // If the winner is 1, the player has chosen their card, and computers haven't
                if (winner == 1 && playerDone)
                {
                    // Set first card and card to beat to the player's card played
                    firstCard = player1Card;
                    cardToBeat = player1Card;

                    // Play the turns in order from player 2
                    Player2Turn();
                    Player3Turn();
                    Player4Turn();

                    roundDone = true;
                }
                // If the winner of the previous round was player 2:
                else if (winner == 2 && !playerDone)
                {
                    // Play the turns in order from player 2
                    Player2Turn();
                    Player3Turn();
                    Player4Turn();
                }
                // If the winner is 3 and the player HAS NOT completed their turn
                else if (winner == 3 && !playerDone)
                {
                    // Play the AI turns up the player's turn
                    Player3Turn();
                    Player4Turn();
                }
                // If the winner is 4 and the player HAS NOT completed their turn
                else if (winner == 4 && !playerDone)
                {
                    // Play the AI turns up the player's turn
                    Player4Turn();
                }
                else if (winner == 2 && playerDone)
                {
                    roundDone = true;
                }
                // If the winner is 3 and the player HAS completed their turn
                else if (winner == 3 && playerDone)
                {
                    // Play the remaining AI turns
                    Player2Turn();

                    roundDone = true;
                }
                // If the winner is 4 and the player HAS completed their turn
                else if (winner == 4 && playerDone)
                {
                    // Play the remaining AI turns
                    Player2Turn();
                    Player3Turn();

                    roundDone = true;
                }
            }

            // If all 4 players are done their turns and we reach the end of this function, start next round
            if (roundDone)
            {
                // Determine winner of round
                DetermineWinner(tarneeb, player1Card, player2Card, player3Card, player4Card);

                // Increment number of cards done
                cardsDone += 1;

                // If there are more cards to play, continue the game
                if (cardsDone < 13)
                {
                    // Show the Next Round button which starts the next round
                    btnNextRound.Visibility = Visibility.Visible;
                    btnNextRound.IsEnabled = true;
                }
                // If the cards are finished, end the game
                else
                {

                }
            }
        }

        

        

        /// <summary>
        /// Turn logic for Player 2 AI
        /// </summary>
        public void Player2Turn()
        {
            Card chosenCard;
            chosenCard = AIChooseCard(hand2);
            player2Card = chosenCard;
            playedCard2.Source = Card.ToImage(chosenCard);
            hand2.RemoveAll(card => card.CardNumber == chosenCard.CardNumber && card.Suit == chosenCard.Suit);
        }

        /// <summary>
        /// Turn logic for Player 3 AI
        /// </summary>
        public void Player3Turn()
        {
            Card chosenCard;
            chosenCard = AIChooseCard(hand3);
            player3Card = chosenCard;
            playedCard3.Source = Card.ToImage(chosenCard);
            hand3.RemoveAll(card => card.CardNumber == chosenCard.CardNumber && card.Suit == chosenCard.Suit);
        }

        /// <summary>
        /// Turn logic for Player 4 AI
        /// </summary>
        public void Player4Turn()
        {
            Card chosenCard;
            chosenCard = AIChooseCard(hand4);
            player4Card = chosenCard;
            playedCard4.Source = Card.ToImage(chosenCard);
            hand4.RemoveAll(card => card.CardNumber == chosenCard.CardNumber && card.Suit == chosenCard.Suit);
        }

        /// <summary>
        /// Returns the AI's choice of card to play with given hand
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public Card AIChooseCard(List<Card> hand)
        {
            // The card chosen by the AI to play
            Card chosenCard = new Card();

            // Create new list of cards that match the suit played, and a list of tarneebs
            List<Card> matchingList = new List<Card>();
            List<Card> tarneebList = new List<Card>();

            // Properties of card to beat
            int playedSuit;
            int playedNumber;

            // If a card to beat has been played
            if (cardToBeat != null)
            {
                // Set properties of card to beat
                playedSuit = (int)cardToBeat.Suit;
                playedNumber = (int)cardToBeat.CardNumber;
            }
            // If a card has not been played
            else
            {
                // Loop through the remaining cards and pick out the lowest value card
                for (int i = 0; i < hand.Count; i++)
                {
                    // If a card hasn't been chosen OR the current card's number is lower than
                    // the chosen card's number...
                    if (i == 0 || (int)hand[i].CardNumber < (int)chosenCard.CardNumber)
                    {
                        // ... choose the current card to play
                        chosenCard = hand[i];

                        // Set this card to the card to beat and the first card
                        cardToBeat = hand[i];
                        firstCard = hand[i];

                        // Set the tarneeb
                        tarneeb = hand[i].Suit;
                    }
                }

                // Set properties of card to beat
                playedSuit = (int)cardToBeat.Suit;
                playedNumber = (int)cardToBeat.CardNumber;
            }

            // Add any matching cards to the new list
            for (int i = 0; i < hand.Count; i++)
            {
                if ((int)hand[i].Suit == playedSuit)
                {
                    matchingList.Add(hand[i]);
                }
            }

            // Loop through the new list of matching cards...
            for (int i = 0; i < matchingList.Count; i++)
            {
                // If a card hasn't been chosen...
                if (i == 0)
                {
                    // ...choose the current card to play
                    chosenCard = matchingList[i];
                }

                // If the current card beats the card played AND the current card does NOT
                // beat the card played (so as to not waste a better card)...
                if (playedNumber < (int)matchingList[i].CardNumber
                    || playedNumber < (int)matchingList[i].CardNumber
                    && (int)matchingList[i].CardNumber < (int)chosenCard.CardNumber)
                {
                    // ...choose the current card to play
                    chosenCard = matchingList[i];

                    // Since this card is better, set it to the new cardToBeat
                    cardToBeat = chosenCard;
                }
            }

            // If there are no cards with a matching suit...
            if (matchingList.Count == 0)
            {
                // ...loop through the remaining cards and pick out the lowest value card
                for (int i = 0; i < hand.Count; i++)
                {
                    // If a card hasn't been chosen OR the current card's number is lower than
                    // the chosen card's number...
                    if (i == 0 || (int)hand[i].CardNumber < (int)chosenCard.CardNumber)
                    {
                        // ... choose the current card to play
                        chosenCard = hand[i];
                    }
                }
            }

            return chosenCard;
        }

        #endregion

        /// <summary>
        /// Sends user back to main menu (exits current game)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnGameBackClick(object sender, RoutedEventArgs e)
        {
            PageMenu menuPage = new PageMenu();
            NavigationService.Navigate(menuPage);
        }

        /// <summary>
        /// Starts the next round of Tarneeb
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnNextRoundClick(object sender, RoutedEventArgs e)
        {
            if (roundDone)
            {
                // Clear cards played
                player1Card = null;
                player2Card = null;
                player3Card = null;
                player4Card = null;
                firstCard = null;
                cardToBeat = null;

                // Clear respective card's images
                playedCard1.Source = null;
                playedCard2.Source = null;
                playedCard3.Source = null;
                playedCard4.Source = null;

                // Refresh card display with remaining cards in hand
                DisplayCards(playerHand);

                // Reset round completion bools to false
                playerDone = false;
                roundDone = false;

                // If a computer won, loop this function to complete the computer turns again
                if (winner > 1)
                {
                    ComputerTurns();
                }

                // Remove winner text
                lblWinner.Content = "";

                // Hide the button again
                btnNextRound.Visibility = Visibility.Hidden;
                btnNextRound.IsEnabled = false;
            }
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
                player1Card = playerHand[0];
                playerHand.RemoveAt(0);
                playerDone = true;

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
                player1Card = playerHand[1];
                playerHand.RemoveAt(1);
                playerDone = true;

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
                player1Card = playerHand[2];
                playerHand.RemoveAt(2);
                playerDone = true;

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
                player1Card = playerHand[3];
                playerHand.RemoveAt(3);
                playerDone = true;

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
                player1Card = playerHand[4];
                playerHand.RemoveAt(4);
                playerDone = true;

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
                player1Card = playerHand[5];
                playerHand.RemoveAt(5);
                playerDone = true;

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
                player1Card = playerHand[6];
                playerHand.RemoveAt(6);
                playerDone = true;

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
                player1Card = playerHand[7];
                playerHand.RemoveAt(7);
                playerDone = true;

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
                player1Card = playerHand[8];
                playerHand.RemoveAt(8);
                playerDone = true;

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
                player1Card = playerHand[9];
                playerHand.RemoveAt(9);
                playerDone = true;

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
                player1Card = playerHand[10];
                playerHand.RemoveAt(10);
                playerDone = true;

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
                player1Card = playerHand[11];
                playerHand.RemoveAt(11);
                playerDone = true;

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
                player1Card = playerHand[12];
                playerHand.RemoveAt(12);
                playerDone = true;

                // Complete computer turns (async)
                ComputerTurns();
            }
        }


        #endregion

        // returns Card that won the hand. (Returns Card to be used to match player that played it. Winner starts next round)
        private Card DetermineWinner(Cards.Enums.Suit tarneeb, Card card1, Card card2, Card card3, Card card4)
        {
            Card winningCard = card1;
            winner = 1;
            lblWinner.Content = "Player 1 Wins!";
            lblWinner.Foreground = team1Color;

            Cards.Enums.Suit suit;
            

            if(card2.Suit == tarneeb)
            {
                suit = tarneeb;
            }
            else if (card3.Suit == tarneeb)
            {
                suit = tarneeb;
            }
            else if (card4.Suit == tarneeb)
            {
                suit = tarneeb;
            }
            else
            {
                suit = card1.Suit;
            }


            if(card2.Suit == suit)
            {
                if(card2.CardNumber > winningCard.CardNumber)
                {
                    winningCard = card2;
                    lblWinner.Content = "Player 2 Wins!";
                    lblWinner.Foreground = team2Color;
                    winner = 2;
                }
            }
            if(card3.Suit == suit)
            {
                if (card3.CardNumber > winningCard.CardNumber)
                {
                    winningCard = card3;
                    lblWinner.Content = "Player 3 Wins!";
                    lblWinner.Foreground = team1Color;
                    winner = 3;
                }
            }
            if(card4.Suit == suit)
            {
                if (card4.CardNumber > winningCard.CardNumber)
                {
                    winningCard = card4;
                    lblWinner.Content = "Player 4 Wins!";
                    lblWinner.Foreground = team2Color;
                    winner = 4;
                }
            }

            switch (winner)
            {
                case 1:
                    team1Score ++;
                    
                    break;
                case 2:
                    team2Score++;
                    break;
                case 3:
                    team1Score++;
                    break;
                case 4:
                    team2Score++;
                    break;
                default:
                    break;
            }

            UpdateTeam1Score();
            UpdateTeam2Score();


            return winningCard;
        }

    }
}

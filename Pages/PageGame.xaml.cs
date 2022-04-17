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

        #region READ ME:

        /*
        
        TODO: List of things still needed:

         - Log & Stats (tough)

         - How To Play (easy)

         - Multiple Round Tarneeb to 31 Points(tough): right now it's single round, also idk if this is needed

        */
        // 

        #endregion

        #region Fields & Properties

        // Speed of the game in milliseconds
        public const int computerTurnRate = 100;
        public const int roundTurnRate = 100;

        public Enums.Suit tarneeb;          // Tarneeb (trump card)
        public bool tarneebPlayed = false;  // Tarneeb played bool
        public Card firstCard;              // The first card played in the round
        public Card cardToBeat;             // The best card played in the round
        public int cardsDone = 0;           // # of remaining cards in the hand
        public Random rand = new Random();  // Random class object instantiation

        public bool playerTurn = false;     // Needed for async, indicates the player's turn to click a card
        public bool playerDone = false;
        public bool roundDone = false;

        // The winner of the betting or the round. Winner places the first card of a new turn.
        // winner = 0 means new round (betting)
        public static int winner = 1;

        // Team round scores
        public int team1Score = 0;
        public int team2Score = 0;

        // Team total scores (game is to 31)
        public int team1Total = 0;
        public int team2Total = 0;

        // List of String that holds Tarneeb suit betting (1. Tarneeb Suit, 2. Bet number, 3. Player who betted the most) global variable
        public static List<string> tarneebSuit = new List<string>() { };

        // Team Colours + Misc Colours
        public SolidColorBrush team1Color = new SolidColorBrush(Color.FromRgb(51, 188, 255));
        public SolidColorBrush team2Color = new SolidColorBrush(Color.FromRgb(255, 90, 90));
        public SolidColorBrush scoreColor = new SolidColorBrush(Color.FromRgb(232, 193, 51));
        public SolidColorBrush greenColor = new SolidColorBrush(Color.FromRgb(61, 184, 93));
        public SolidColorBrush blackColor = new SolidColorBrush(Color.FromRgb(0, 0, 0));

        // Create a list of the player's cards Image controls from the PageGame.xaml form
        public List<Image> playerCardImages = new List<Image>();

        // List of all player's cards
        public List<Card> playerHand = new List<Card>();
        public List<Card> hand2 = new List<Card>();
        public List<Card> hand3 = new List<Card>();
        public List<Card> hand4 = new List<Card>();

        // Played Cards each turn
        public Card player1Card = new Card();
        public Card player2Card = new Card();
        public Card player3Card = new Card();
        public Card player4Card = new Card();

        // AI Difficulty setting (1 = easy, 2 = hard)
        // Not currently implemented
        public int computerDifficulty = 2;

        // Betting
        public int bettingPlayer = 0;
        public int player1Betting = 1;
        public int player2Betting = 2;
        public int player3Betting = 3;
        public int player4Betting = 4;
        public int bet = 7;
        public int topBet = 7;
        public int minimumBet = 7;
        public int maximumBet = 13;
        public int startingPlayerBetting = 1;
        public bool player1IsBetting = true;
        public bool player2IsBetting = true;
        public bool player3IsBetting = true;
        public bool player4IsBetting = true;

        #endregion

        #region Game Initialization

        public PageGame()
        {
            InitializeComponent();
            CreateImageList();
            NewRound();
        }

        /// <summary>
        /// Initiates a new round of Tarneeb, including shuffling deck, dealing cards, resetting variables, etc..
        /// </summary>
        public void NewRound()
        {
            // Create a deck
            Deck deck = new Deck();

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

            // Reset round progression variables
            winner = 1;
            playerDone = false;
            playerTurn = false;
            roundDone = false;
            cardsDone = 0;

            // Hide the Next Round button
            btnNextRound.Visibility = Visibility.Hidden;
            btnNextRound.IsEnabled = false;

            // Hide the played cards
            playedCard1.Source = null;
            playedCard2.Source = null;
            playedCard3.Source = null;
            playedCard4.Source = null;

            // Reset the Tarneeb
            tarneebPlayed = false;
            tarneebImage1.Source = null;
            tarneebImage2.Source = null;
            tarneebImage3.Source = null;
            tarneebImage4.Source = null;
            tarneebImage5.Source = null;

            // Reset the bet label
            lblBet1.Visibility = Visibility.Hidden;
            lblBet2.Visibility = Visibility.Hidden;
            lblBet3.Visibility = Visibility.Hidden;
            lblBet4.Visibility = Visibility.Hidden;
            lblBet5.Visibility = Visibility.Hidden;

            // Reset the team single round scores
            team1Score = 0;
            team2Score = 0;
            UpdateTeamScores();

            // Remove winner text
            lblWinner.Content = "";

            // Reset first card of round and the card to beat (AI logic)
            firstCard = null;
            cardToBeat = null;

            // Reset btnNextRound text
            btnNextRound.Content = "Next Round";

            // Starting Player bet
            if (startingPlayerBetting == 2)
            {
                Player2Bet();
                Player3Bet();
                Player4Bet();
            }
            else if (startingPlayerBetting == 3)
            {
                Player3Bet();
                Player4Bet();
            }
            else if (startingPlayerBetting == 4)
            {
                Player4Bet();
            }
            ChangeBettingButtons();
        }

        #endregion

        #region Card Display (Arbin)

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

        #endregion

        #region Betting

        private void BtnBetAddClick(object sender, RoutedEventArgs e)
        {
            if (bet < maximumBet)
            {
                bet += 1;
                lblBetting1.Content = bet.ToString();
                lblBetting2.Content = bet.ToString();
                lblBetting3.Content = bet.ToString();
                lblBetting4.Content = bet.ToString();
                lblBetting5.Content = bet.ToString();
            }
        }
        private void BtnBetSubClick(object sender, RoutedEventArgs e)
        {
            if (bet > minimumBet)
            {
                bet -= 1;
                lblBetting1.Content = bet.ToString();
                lblBetting2.Content = bet.ToString();
                lblBetting3.Content = bet.ToString();
                lblBetting4.Content = bet.ToString();
                lblBetting5.Content = bet.ToString();
            }
        }
        private void BtnPassClick(object sender, RoutedEventArgs e)
        {

            player1IsBetting = false;

            lblPrevBetting5.Content = "Pass";
            lblPrevBetting4.Content = "Pass";
            lblPrevBetting3.Content = "Pass";
            lblPrevBetting2.Content = "Pass";
            lblPrevBetting1.Content = "Pass";

            lblPrevBetting5.Visibility = Visibility.Visible;
            lblPrevBetting4.Visibility = Visibility.Visible;
            lblPrevBetting3.Visibility = Visibility.Visible;
            lblPrevBetting2.Visibility = Visibility.Visible;
            lblPrevBetting1.Visibility = Visibility.Visible;


            // AI betting functionality
            while ((player2IsBetting && player3IsBetting) || (player2IsBetting && player4IsBetting) || (player3IsBetting && player4IsBetting))
            {
                Player2Bet();
                Player3Bet();
                Player4Bet();
            }

            Player2Bet();
            Player3Bet();
            Player4Bet();

            // No bets were made, re-shuffle
            if (bettingPlayer == 0)
            {
            }
        }
        private void BtnBetClick(object sender, RoutedEventArgs e)
        {

            lblPrevBetting5.Content = bet.ToString();
            lblPrevBetting4.Content = bet.ToString();
            lblPrevBetting3.Content = bet.ToString();
            lblPrevBetting2.Content = bet.ToString();
            lblPrevBetting1.Content = bet.ToString();

            lblPrevBetting5.Visibility = Visibility.Visible;
            lblPrevBetting4.Visibility = Visibility.Visible;
            lblPrevBetting3.Visibility = Visibility.Visible;
            lblPrevBetting2.Visibility = Visibility.Visible;
            lblPrevBetting1.Visibility = Visibility.Visible;

            bettingPlayer = player1Betting;

            topBet = bet;

            minimumBet = bet + 1;

            // AI betting functionality
            Player2Bet();
            Player3Bet();
            Player4Bet();

            bet = minimumBet;
            lblBetting1.Content = bet.ToString();
            lblBetting2.Content = bet.ToString();
            lblBetting3.Content = bet.ToString();
            lblBetting4.Content = bet.ToString();
            lblBetting5.Content = bet.ToString();

            if (bettingPlayer == player1Betting)
            {
                HideBettingButtons();
                ShowTarneebSelection();
            }
        }

        // AI betting and tarneeb selection
        public void Player2Bet()
        {
            // no one made a bet. AI selects tarneeb
            if (bettingPlayer == 2)
            {
                HideBettingButtons();
                SetTarneeb(AITarneebSelection(hand2));
                winner = bettingPlayer;
                DoComputerTurns();
            }
            else
            {
                if (player2IsBetting == true)
                {
                    int roundNum = 1;
                    int playerBetAmount;
                    if (roundNum == 1)
                    {
                        playerBetAmount = AIBettingAmount(hand2);
                    }
                    else
                    {
                        playerBetAmount = AIBettingAmount(hand2) + 1;
                    }
                    if (playerBetAmount < minimumBet)
                    {
                        // pass
                        player2IsBetting = false;

                        lblP2Betting5.Content = "Pass";
                        lblP2Betting4.Content = "Pass";
                        lblP2Betting3.Content = "Pass";
                        lblP2Betting2.Content = "Pass";
                        lblP2Betting1.Content = "Pass";
                        lblP2Betting5.Visibility = Visibility.Visible;
                        lblP2Betting4.Visibility = Visibility.Visible;
                        lblP2Betting3.Visibility = Visibility.Visible;
                        lblP2Betting2.Visibility = Visibility.Visible;
                        lblP2Betting1.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        // bet
                        if (playerBetAmount >= maximumBet)
                        {
                            playerBetAmount = maximumBet;

                            player1IsBetting = false;
                            player3IsBetting = false;
                            player4IsBetting = false;
                        }

                        minimumBet = playerBetAmount + 1;

                        bettingPlayer = 2;

                        topBet = playerBetAmount;

                        lblP2Betting5.Content = playerBetAmount.ToString();
                        lblP2Betting4.Content = playerBetAmount.ToString();
                        lblP2Betting3.Content = playerBetAmount.ToString();
                        lblP2Betting2.Content = playerBetAmount.ToString();
                        lblP2Betting1.Content = playerBetAmount.ToString();
                        lblP2Betting5.Visibility = Visibility.Visible;
                        lblP2Betting4.Visibility = Visibility.Visible;
                        lblP2Betting3.Visibility = Visibility.Visible;
                        lblP2Betting2.Visibility = Visibility.Visible;
                        lblP2Betting1.Visibility = Visibility.Visible;
                    }
                    roundNum = 2;
                }
            }
        }

        public void Player3Bet()
        {
            if (bettingPlayer == 3)
            {
                HideBettingButtons();
                SetTarneeb(AITarneebSelection(hand4));
                winner = bettingPlayer;
                DoComputerTurns();
            }
            else
            {
                if (player3IsBetting == true)
                {
                    int roundNum = 1;
                    int playerBetAmount;
                    if (roundNum == 1)
                    {
                        playerBetAmount = AIBettingAmount(hand3);
                    }
                    else
                    {
                        playerBetAmount = AIBettingAmount(hand3) + 1;
                    }
                    if (playerBetAmount < minimumBet)
                    {
                        player3IsBetting = false;

                        lblP3Betting5.Content = "Pass";
                        lblP3Betting4.Content = "Pass";
                        lblP3Betting3.Content = "Pass";
                        lblP3Betting2.Content = "Pass";
                        lblP3Betting1.Content = "Pass";
                        lblP3Betting5.Visibility = Visibility.Visible;
                        lblP3Betting4.Visibility = Visibility.Visible;
                        lblP3Betting3.Visibility = Visibility.Visible;
                        lblP3Betting2.Visibility = Visibility.Visible;
                        lblP3Betting1.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        if (playerBetAmount >= maximumBet)
                        {
                            playerBetAmount = maximumBet;

                            player1IsBetting = false;
                            player2IsBetting = false;
                            player4IsBetting = false;
                        }

                        minimumBet = playerBetAmount + 1;

                        bettingPlayer = 3;

                        topBet = playerBetAmount;

                        lblP3Betting5.Content = playerBetAmount.ToString();
                        lblP3Betting4.Content = playerBetAmount.ToString();
                        lblP3Betting3.Content = playerBetAmount.ToString();
                        lblP3Betting2.Content = playerBetAmount.ToString();
                        lblP3Betting1.Content = playerBetAmount.ToString();
                        lblP3Betting5.Visibility = Visibility.Visible;
                        lblP3Betting4.Visibility = Visibility.Visible;
                        lblP3Betting3.Visibility = Visibility.Visible;
                        lblP3Betting2.Visibility = Visibility.Visible;
                        lblP3Betting1.Visibility = Visibility.Visible;
                    }
                    roundNum = 2;
                }
            }
        }
        public void Player4Bet()
        {
            if (bettingPlayer == 4)
            {
                HideBettingButtons();
                SetTarneeb(AITarneebSelection(hand4));
                winner = bettingPlayer;
                DoComputerTurns();
            }
            else
            {
                if (player4IsBetting == true)
                {
                    int roundNum = 1;
                    int playerBetAmount;
                    if (roundNum == 1)
                    {
                        playerBetAmount = AIBettingAmount(hand4);
                    }
                    else
                    {
                        playerBetAmount = AIBettingAmount(hand4) + 1;
                    }
                    if (playerBetAmount < minimumBet)
                    {
                        player4IsBetting = false;

                        lblP4Betting5.Content = "Pass";
                        lblP4Betting4.Content = "Pass";
                        lblP4Betting3.Content = "Pass";
                        lblP4Betting2.Content = "Pass";
                        lblP4Betting1.Content = "Pass";
                        lblP4Betting5.Visibility = Visibility.Visible;
                        lblP4Betting4.Visibility = Visibility.Visible;
                        lblP4Betting3.Visibility = Visibility.Visible;
                        lblP4Betting2.Visibility = Visibility.Visible;
                        lblP4Betting1.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        if (playerBetAmount >= maximumBet)
                        {
                            playerBetAmount = maximumBet;

                            player1IsBetting = false;
                            player2IsBetting = false;
                            player3IsBetting = false;
                        }

                        minimumBet = playerBetAmount + 1;

                        bettingPlayer = 4;

                        topBet = playerBetAmount;

                        lblP4Betting5.Content = playerBetAmount.ToString();
                        lblP4Betting4.Content = playerBetAmount.ToString();
                        lblP4Betting3.Content = playerBetAmount.ToString();
                        lblP4Betting2.Content = playerBetAmount.ToString();
                        lblP4Betting1.Content = playerBetAmount.ToString();
                        lblP4Betting5.Visibility = Visibility.Visible;
                        lblP4Betting4.Visibility = Visibility.Visible;
                        lblP4Betting3.Visibility = Visibility.Visible;
                        lblP4Betting2.Visibility = Visibility.Visible;
                        lblP4Betting1.Visibility = Visibility.Visible;
                    }
                    roundNum = 2;
                }
            }
        }

        // betting AI functionality
        public int AIBettingAmount(List<Card> cards)
        {
            int numOfClub = 0;
            int totalClubValue = 0;
            int numOfDiamond = 0;
            int totalDiamondValue = 0;
            int numOfHeart = 0;
            int totalHeartValue = 0;
            int numOfSpade = 0;
            int totalSpadeValue = 0;

            int numOfAces = 0;
            int bettingAmount = 0;
            int bettingSuit = 1;

            foreach (Card card in cards)
            {
                if ((int)card.Suit == 1)
                {
                    numOfClub += 1;
                    totalClubValue += (int)card.CardNumber;
                }
                if ((int)card.Suit == 2)
                {
                    numOfDiamond += 1;
                    totalDiamondValue += (int)card.CardNumber;
                }
                if ((int)card.Suit == 3)
                {
                    numOfHeart += 1;
                    totalHeartValue += (int)card.CardNumber;
                }
                if ((int)card.Suit == 4)
                {
                    numOfSpade += 1;
                    totalSpadeValue += (int)card.CardNumber;
                }
                if ((int)card.CardNumber == 13)
                {
                    numOfAces += 1;
                }
            }

            bettingAmount += numOfAces;

            // determine top suit for betting amount
            int topNumOfSuit = numOfClub;
            int topValue = totalClubValue;

            if ((numOfDiamond > topNumOfSuit) || ((numOfDiamond == topNumOfSuit) && (totalDiamondValue > topValue)))
            {
                topNumOfSuit = numOfDiamond;
                topValue = totalDiamondValue;
                bettingSuit = 2;
            }
            if ((numOfHeart > topNumOfSuit) || ((numOfHeart == topNumOfSuit) && (totalHeartValue > topValue)))
            {
                topNumOfSuit = numOfHeart;
                topValue = totalHeartValue;
                bettingSuit = 3;
            }
            if ((numOfSpade > topNumOfSuit) || ((numOfSpade == topNumOfSuit) && (totalSpadeValue > topValue)))
            {
                topNumOfSuit = numOfSpade;
                topValue = totalSpadeValue;
                bettingSuit = 4;
            }

            if (bettingSuit == 1)
            {
                bettingAmount += numOfClub;
            }
            if (bettingSuit == 2)
            {
                bettingAmount += numOfDiamond;
            }
            if (bettingSuit == 3)
            {
                bettingAmount += numOfHeart;
            }
            if (bettingSuit == 4)
            {
                bettingAmount += numOfSpade;
            }

            bettingAmount += 1;

            return bettingAmount;
        }
        public Enums.Suit AITarneebSelection(List<Card> cards)
        {
            int numOfClub = 0;
            int totalClubValue = 0;
            int numOfDiamond = 0;
            int totalDiamondValue = 0;
            int numOfHeart = 0;
            int totalHeartValue = 0;
            int numOfSpade = 0;
            int totalSpadeValue = 0;

            Enums.Suit selectedTarneeb = Enums.Suit.CLUB;

            foreach (Card card in cards)
            {
                if ((int)card.Suit == 1)
                {
                    numOfClub += 1;
                    totalClubValue += (int)card.CardNumber;
                }
                if ((int)card.Suit == 2)
                {
                    numOfDiamond += 1;
                    totalDiamondValue += (int)card.CardNumber;
                }
                if ((int)card.Suit == 3)
                {
                    numOfHeart += 1;
                    totalHeartValue += (int)card.CardNumber;
                }
                if ((int)card.Suit == 4)
                {
                    numOfSpade += 1;
                    totalSpadeValue += (int)card.CardNumber;
                }
            }

            // determine top suit for Tarneeb
            int topNumOfSuit = numOfClub;
            int topValue = totalClubValue;

            if ((numOfDiamond > topNumOfSuit) || ((numOfDiamond == topNumOfSuit) && (totalDiamondValue > topValue)))
            {
                topNumOfSuit = numOfDiamond;
                topValue = totalDiamondValue;
                selectedTarneeb = Enums.Suit.DIAMOND;
            }
            if ((numOfHeart > topNumOfSuit) || ((numOfHeart == topNumOfSuit) && (totalHeartValue > topValue)))
            {
                topNumOfSuit = numOfHeart;
                topValue = totalHeartValue;
                selectedTarneeb = Enums.Suit.HEART;
            }
            if ((numOfSpade > topNumOfSuit) || ((numOfSpade == topNumOfSuit) && (totalSpadeValue > topValue)))
            {
                topNumOfSuit = numOfSpade;
                topValue = totalSpadeValue;
                selectedTarneeb = Enums.Suit.SPADE;
            }

            return selectedTarneeb;
        }
        public void HideBettingButtons()
        {
            // Hide the buttons
            btnBetAdd.Visibility = Visibility.Hidden;
            btnBetAdd.IsEnabled = false;
            btnBetSub.Visibility = Visibility.Hidden;
            btnBetSub.IsEnabled = false;
            btnBet.Visibility = Visibility.Hidden;
            btnBet.IsEnabled = false;
            btnPass.Visibility = Visibility.Hidden;
            btnPass.IsEnabled = false;
            lblBetting1.Visibility = Visibility.Hidden;
            lblBetting2.Visibility = Visibility.Hidden;
            lblBetting3.Visibility = Visibility.Hidden;
            lblBetting4.Visibility = Visibility.Hidden;
            lblBetting5.Visibility = Visibility.Hidden;

            lblBet1.Content = topBet.ToString();
            lblBet2.Content = topBet.ToString();
            lblBet3.Content = topBet.ToString();
            lblBet4.Content = topBet.ToString();
            lblBet5.Content = topBet.ToString();
            lblBet2.Visibility = Visibility.Visible;
            lblBet3.Visibility = Visibility.Visible;
            lblBet4.Visibility = Visibility.Visible;
            lblBet5.Visibility = Visibility.Visible;
            // Change label colour based on team that won bet
            if (bettingPlayer == 1 || bettingPlayer == 3)
            {
                lblBet1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#33BCFF"));
            }
            else if (bettingPlayer == 2 || bettingPlayer == 4)
            {
                lblBet1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5A5A"));
            }
            lblBet1.Visibility = Visibility.Visible;

            lblPrevBetting5.Visibility = Visibility.Hidden;
            lblPrevBetting4.Visibility = Visibility.Hidden;
            lblPrevBetting3.Visibility = Visibility.Hidden;
            lblPrevBetting2.Visibility = Visibility.Hidden;
            lblPrevBetting1.Visibility = Visibility.Hidden;

            lblP2Betting5.Visibility = Visibility.Hidden;
            lblP2Betting4.Visibility = Visibility.Hidden;
            lblP2Betting3.Visibility = Visibility.Hidden;
            lblP2Betting2.Visibility = Visibility.Hidden;
            lblP2Betting1.Visibility = Visibility.Hidden;

            lblP3Betting5.Visibility = Visibility.Hidden;
            lblP3Betting4.Visibility = Visibility.Hidden;
            lblP3Betting3.Visibility = Visibility.Hidden;
            lblP3Betting2.Visibility = Visibility.Hidden;
            lblP3Betting1.Visibility = Visibility.Hidden;

            lblP4Betting5.Visibility = Visibility.Hidden;
            lblP4Betting4.Visibility = Visibility.Hidden;
            lblP4Betting3.Visibility = Visibility.Hidden;
            lblP4Betting2.Visibility = Visibility.Hidden;
            lblP4Betting1.Visibility = Visibility.Hidden;
        }

        public void ResetBettingButtons()
        {
            bettingPlayer = 0;
            bet = 7;
            topBet = 7;
            minimumBet = 7;
            player1IsBetting = true;
            player2IsBetting = true;
            player3IsBetting = true;
            player4IsBetting = true;

            // Show the buttons
            btnBetAdd.Visibility = Visibility.Visible;
            btnBetAdd.IsEnabled = true;
            btnBetSub.Visibility = Visibility.Visible;
            btnBetSub.IsEnabled = true;
            btnBet.Visibility = Visibility.Visible;
            btnBet.IsEnabled = true;
            btnPass.Visibility = Visibility.Visible;
            btnPass.IsEnabled = true;

            lblBet1.Visibility = Visibility.Hidden;
            lblBet2.Visibility = Visibility.Hidden;
            lblBet3.Visibility = Visibility.Hidden;
            lblBet4.Visibility = Visibility.Hidden;
            lblBet5.Visibility = Visibility.Hidden;

            winner = 0;
        }

        public void ChangeBettingButtons()
        {
            lblBetting1.Content = minimumBet.ToString();
            lblBetting2.Content = minimumBet.ToString();
            lblBetting3.Content = minimumBet.ToString();
            lblBetting4.Content = minimumBet.ToString();
            lblBetting5.Content = minimumBet.ToString();
            lblBetting1.Visibility = Visibility.Visible;
            lblBetting2.Visibility = Visibility.Visible;
            lblBetting3.Visibility = Visibility.Visible;
            lblBetting4.Visibility = Visibility.Visible;
            lblBetting5.Visibility = Visibility.Visible;

            bet = minimumBet;
        }
        #endregion

        #region Tarneeb Selection

        /// <summary>
        /// Hides all of the Tarneeb selection images
        /// </summary>
        private void HideTarneebSelection()
        {
            // Hide Tarneeb Labels
            lblSelectTarneeb1.Visibility = Visibility.Hidden;
            lblSelectTarneeb2.Visibility = Visibility.Hidden;
            lblSelectTarneeb3.Visibility = Visibility.Hidden;
            lblSelectTarneeb4.Visibility = Visibility.Hidden;
            lblSelectTarneeb5.Visibility = Visibility.Hidden;

            // Hide Clubs
            tc1.Visibility = Visibility.Hidden;
            tc2.Visibility = Visibility.Hidden;
            tc3.Visibility = Visibility.Hidden;
            tc4.Visibility = Visibility.Hidden;
            tc5.Visibility = Visibility.Hidden;

            // Hide Diamonds
            td1.Visibility = Visibility.Hidden;
            td2.Visibility = Visibility.Hidden;
            td3.Visibility = Visibility.Hidden;
            td4.Visibility = Visibility.Hidden;
            td5.Visibility = Visibility.Hidden;

            // Hide Hearts
            th1.Visibility = Visibility.Hidden;
            th2.Visibility = Visibility.Hidden;
            th3.Visibility = Visibility.Hidden;
            th4.Visibility = Visibility.Hidden;
            th5.Visibility = Visibility.Hidden;

            // Hide Spades
            ts1.Visibility = Visibility.Hidden;
            ts2.Visibility = Visibility.Hidden;
            ts3.Visibility = Visibility.Hidden;
            ts4.Visibility = Visibility.Hidden;
            ts5.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Shows all of the Tarneeb selection images
        /// </summary>
        private void ShowTarneebSelection()
        {
            // Show Tarneeb Labels
            lblSelectTarneeb1.Visibility = Visibility.Visible;
            lblSelectTarneeb2.Visibility = Visibility.Visible;
            lblSelectTarneeb3.Visibility = Visibility.Visible;
            lblSelectTarneeb4.Visibility = Visibility.Visible;
            lblSelectTarneeb5.Visibility = Visibility.Visible;

            // Show Clubs
            tc1.Visibility = Visibility.Visible;
            tc2.Visibility = Visibility.Visible;
            tc3.Visibility = Visibility.Visible;
            tc4.Visibility = Visibility.Visible;
            tc5.Visibility = Visibility.Visible;

            // Show Diamonds
            td1.Visibility = Visibility.Visible;
            td2.Visibility = Visibility.Visible;
            td3.Visibility = Visibility.Visible;
            td4.Visibility = Visibility.Visible;
            td5.Visibility = Visibility.Visible;

            // Show Hearts
            th1.Visibility = Visibility.Visible;
            th2.Visibility = Visibility.Visible;
            th3.Visibility = Visibility.Visible;
            th4.Visibility = Visibility.Visible;
            th5.Visibility = Visibility.Visible;

            // Show Spades
            ts1.Visibility = Visibility.Visible;
            ts2.Visibility = Visibility.Visible;
            ts3.Visibility = Visibility.Visible;
            ts4.Visibility = Visibility.Visible;
            ts5.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Select Clubs as the tarneeb suit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TarneebClubMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetTarneeb(Enums.Suit.CLUB);
            HideTarneebSelection();
            winner = bettingPlayer;
            playerTurn = true;
        }

        /// <summary>
        /// Select Diamonds as the tarneeb suit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TarneebDiamondMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetTarneeb(Enums.Suit.DIAMOND);
            HideTarneebSelection();
            winner = bettingPlayer;
            playerTurn = true;
        }

        /// <summary>
        /// Select Hearts as the tarneeb suit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TarneebHeartMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetTarneeb(Enums.Suit.HEART);
            HideTarneebSelection();
            winner = bettingPlayer;
            playerTurn = true;
        }

        /// <summary>
        /// Select Spades as the tarneeb suit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TarneebSpadeMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetTarneeb(Enums.Suit.SPADE);
            HideTarneebSelection();
            winner = bettingPlayer;
            playerTurn = true;
        }

        /// <summary>
        /// Sets the tarneeb suit
        /// </summary>
        /// <param name="suit">The tarneeb suit</param>
        public void SetTarneeb(Enums.Suit suit)
        {
            // Set tarneeb if it hasn't already been set
            if (!tarneebPlayed)
            {
                tarneeb = suit;
                tarneebImage1.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/SuitLogos/_" + suit + ".png");
                tarneebImage2.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/SuitLogos/_" + suit + "_B.png");
                tarneebImage3.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/SuitLogos/_" + suit + "_B.png");
                tarneebImage4.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/SuitLogos/_" + suit + "_B.png");
                tarneebImage5.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/SuitLogos/_" + suit + "_B.png");
                tarneebPlayed = true;
            }
        }

        #endregion

        #region AI Logic

        /// <summary>
        /// Completes the turns of the computer players 2-4 and starts the next round
        /// </summary>
        /// <returns></returns>
        public async void ComputerTurnLogic()
        {
            // If the round isn't over, execute the computer turns
            if (!roundDone)
            {
                await DoComputerTurns();
            }

            // If the round is completed, start the next round
            if (roundDone)
            {
                await PromptNextRound();
            }
        }

        /// <summary>
        /// Executes the computer player 2-4's turns
        /// </summary>
        public async Task DoComputerTurns()
        {
            // If player has made their turn already...
            if (playerDone)
            {
                // ...switch depending on winner (player who places first)
                switch (winner)
                {
                    case 1:
                        // Set first card and card to beat to the player's card played
                        firstCard = player1Card;
                        cardToBeat = player1Card;
                        // Play the AI turns and set the tarneeb
                        await Player2Turn();
                        await Player3Turn();
                        await Player4Turn();
                        roundDone = true;
                        break;
                    case 2:
                        roundDone = true;
                        break;
                    case 3:
                        // Play the remaining AI turn
                        await Player2Turn();
                        roundDone = true;
                        break;
                    case 4:
                        // Play the remaining AI turns
                        await Player2Turn();
                        await Player3Turn();
                        roundDone = true;
                        break;
                    default:
                        break;
                }
            }
            // If player has NOT made their turn already...
            else if (!playerDone)
            {
                // ...switch depending on winner (player who places first)
                switch (winner)
                {
                    case 2:
                        // Play the turns in order from player 2 and set Tarneeb / firstCard
                        await Player2Turn();
                        firstCard = player2Card;
                        await Player3Turn();
                        await Player4Turn();
                        playerTurn = true;
                        break;
                    case 3:
                        // Play the turns in order from player 3 and set Tarneeb / firstCard
                        await Player3Turn();
                        firstCard = player3Card;
                        await Player4Turn();
                        playerTurn = true;
                        break;
                    case 4:
                        // Play the first turn and set Tarneeb / firstCard
                        await Player4Turn();
                        firstCard = player4Card;
                        playerTurn = true;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Returns the AI's choice of card to play with given hand
        /// </summary>
        /// <param name="hand">The computer's current hand</param>
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

            // If a card has already been played this round
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
                    }
                }

                // Set properties of card to beat
                playedSuit = (int)cardToBeat.Suit;
                playedNumber = (int)cardToBeat.CardNumber;
            }

            // If the tarneeb has been selected...
            if (tarneebPlayed)
            {
                // ... make a list of tarneebs in the AI's hand
                for (int i = 0; i < hand.Count; i++)
                {
                    if (hand[i].Suit == tarneeb)
                    {
                        tarneebList.Add(hand[i]);
                    }
                }
            }

            // Make a list of cards that match the played suit in the AI's hand
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
                    && (int)matchingList[i].CardNumber < (int)chosenCard.CardNumber)
                {
                    // ...choose the current card to play
                    chosenCard = matchingList[i];

                    // Since this card is better, set it to the new cardToBeat
                    cardToBeat = chosenCard;
                }
                // If the lower card doesn't beat the cardToBeat but the higher one does,
                // play the higher one
                else if (playedNumber < (int)matchingList[i].CardNumber)
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

                // If the card to beat is a KING or ACE and AI has at least one tarneeb in hand,
                // play the lowest value tarneeb
                if ((int)cardToBeat.CardNumber > 11 && tarneebList.Count > 0)
                {
                    // ...loop through the remaining cards and pick out the lowest value tarneeb
                    for (int i = 0; i < tarneebList.Count; i++)
                    {
                        // If a card hasn't been chosen OR the current card's number is lower than
                        // the chosen card's number...
                        if (i == 0 || (int)tarneebList[i].CardNumber < (int)chosenCard.CardNumber)
                        {
                            // ... choose the current card to play
                            chosenCard = tarneebList[i];

                            // The tarneeb played beats the non-tarneeb cardToBeat
                            cardToBeat = chosenCard;
                        }
                    }
                }
            }

            // Return the AI's card choice
            return chosenCard;
        }

        /// <summary>
        /// Turn logic for Player 2 AI
        /// </summary>
        public async Task Player2Turn()
        {
            await Task.Delay(computerTurnRate / 2);
            Card chosenCard;
            chosenCard = AIChooseCard(hand2);
            player2Card = chosenCard;
            playedCard2.Source = Card.ToImage(chosenCard);
            hand2.RemoveAll(card => card.CardNumber == chosenCard.CardNumber && card.Suit == chosenCard.Suit);
            await Task.Delay(computerTurnRate / 2);
        }

        /// <summary>
        /// Turn logic for Player 3 AI
        /// </summary>
        public async Task Player3Turn()
        {
            await Task.Delay(computerTurnRate / 2);
            Card chosenCard;
            chosenCard = AIChooseCard(hand3);
            player3Card = chosenCard;
            playedCard3.Source = Card.ToImage(chosenCard);
            hand3.RemoveAll(card => card.CardNumber == chosenCard.CardNumber && card.Suit == chosenCard.Suit);
            await Task.Delay(computerTurnRate / 2);
        }

        /// <summary>
        /// Turn logic for Player 4 AI
        /// </summary>
        public async Task Player4Turn()
        {
            await Task.Delay(computerTurnRate / 2);
            Card chosenCard;
            chosenCard = AIChooseCard(hand4);
            player4Card = chosenCard;
            playedCard4.Source = Card.ToImage(chosenCard);
            hand4.RemoveAll(card => card.CardNumber == chosenCard.CardNumber && card.Suit == chosenCard.Suit);
            await Task.Delay(computerTurnRate / 2);
        }

        /// <summary>
        /// Starts the next round
        /// </summary>
        public async Task PromptNextRound()
        {
            // Determine winner of round
            EndOfRoundCleanup(tarneeb, player1Card, player2Card, player3Card, player4Card);

            // Increment number of cards done
            cardsDone += 1;

            // If there are more cards to play, continue the game
            if (cardsDone < 13)
            {
                // Show the Next Round button which starts the next round
                //btnNextRound.Background = greenColor;
                //btnNextRound.Foreground = blackColor;
                //btnNextRound.Visibility = Visibility.Visible;
                //btnNextRound.IsEnabled = true;
                await Task.Delay(roundTurnRate);
                InitiateNextRound();
            }
            // If the cards are finished, prompt for new game
            else
            {
                await Task.Delay(roundTurnRate);

                // Show the New Game button which creates a new fresh PageGame page
                btnNextRound.Background = scoreColor;
                btnNextRound.Foreground = blackColor;
                btnNextRound.Content = "New Game?";
                btnNextRound.Visibility = Visibility.Visible;
                btnNextRound.IsEnabled = true;
            }
        }


        #endregion

        #region Button Functionality

        /// <summary>
        /// Starts the next round of tarneeb
        /// </summary>
        private void InitiateNextRound()
        {
            if (cardsDone < 13 && roundDone)
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

                playerTurn = true;

                // If a computer won, loop this function to complete the computer turns again
                if (winner > 1)
                {
                    playerTurn = false;
                    ComputerTurnLogic();
                }

                // Remove winner text
                lblWinner.Content = "";

                // Hide the button again
                btnNextRound.Visibility = Visibility.Hidden;
                btnNextRound.IsEnabled = false;
            }
            // All 4 players are out of cards. Create new game
            else if (roundDone)
            {
                // Next player starts betting
                startingPlayerBetting += 1;
                if (startingPlayerBetting > 4)
                {
                    startingPlayerBetting = 1;
                }
                // reset betting
                ResetBettingButtons();

                // Call NewRound function
                NewRound();

            }
        }

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
            InitiateNextRound();
        }


        #endregion

        #region Card Click Functionality (Arbin)

        /// <summary>
        /// Determines if the card clicked is playable based on first card played on the current round
        /// </summary>
        /// <param name="card">The card that was selected by the player</param>
        /// <returns></returns>
        private bool IsPlayable(Card card)
        {
            // If the variable "winner" is set to 0, don't allow cards to be clicked
            // In the betting phase, setting winner to 0 will prevent the player from playing
            // Set winner equal to the player # of the winner of the betting
            if (winner == 0)
            {
                return false;
            }

            // If it's not the player's turn, return false
            if (!playerTurn)
            {
                return false;
            }

            // If the first card has already been placed...
            if (firstCard != null)
            {
                // ... check player's remaining hand for matching suits
                bool hasSuit = false;
                for (int i = 0; i < playerHand.Count; i++)
                {
                    // If there is a matching suit, set bool to true
                    if (playerHand[i].Suit == firstCard.Suit)
                    {
                        hasSuit = true;
                    }
                }

                // If the player doesn't have a matching suit, card is playable
                if (!hasSuit)
                {
                    return true;
                }
                // If the card the player is trying to play matches, card is playable
                else
                {
                    return card.Suit == firstCard.Suit;
                }
            }
            // If the card hasn't been played, card is playable
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Card01MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p01.Source != null && playedCard1.Source == null && IsPlayable(playerHand[0]))
            {
                // ...play the card.
                playedCard1.Source = p01.Source;
                p01.Source = null;

                // Remove card from hand
                player1Card = playerHand[0];
                playerHand.RemoveAt(0);
                playerDone = true;
                playerTurn = false;

                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Card02MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p02.Source != null && playedCard1.Source == null && IsPlayable(playerHand[1]))
            {
                // ...play the card.
                playedCard1.Source = p02.Source;
                p02.Source = null;

                // Remove card from hand
                player1Card = playerHand[1];
                playerHand.RemoveAt(1);
                playerDone = true;
                playerTurn = false;

                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Card03MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p03.Source != null && playedCard1.Source == null && IsPlayable(playerHand[2]))
            {
                // ...play the card.
                playedCard1.Source = p03.Source;
                p03.Source = null;

                // Remove card from hand
                player1Card = playerHand[2];
                playerHand.RemoveAt(2);
                playerDone = true;
                playerTurn = false;

                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Card04MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p04.Source != null && playedCard1.Source == null && IsPlayable(playerHand[3]))
            {
                // ...play the card.
                playedCard1.Source = p04.Source;
                p04.Source = null;


                // Remove card from hand
                player1Card = playerHand[3];
                playerHand.RemoveAt(3);
                playerDone = true;
                playerTurn = false;

                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Card05MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p05.Source != null && playedCard1.Source == null && IsPlayable(playerHand[4]))
            {
                // ...play the card.
                playedCard1.Source = p05.Source;
                p05.Source = null;

                // Remove card from hand
                player1Card = playerHand[4];
                playerHand.RemoveAt(4);
                playerDone = true;
                playerTurn = false;

                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Card06MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p06.Source != null && playedCard1.Source == null && IsPlayable(playerHand[5]))
            {
                // ...play the card.
                playedCard1.Source = p06.Source;
                p06.Source = null;

                // Remove card from hand
                player1Card = playerHand[5];
                playerHand.RemoveAt(5);
                playerDone = true;
                playerTurn = false;

                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Card07MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p07.Source != null && playedCard1.Source == null && IsPlayable(playerHand[6]))
            {
                // ...play the card.
                playedCard1.Source = p07.Source;
                p07.Source = null;

                // Remove card from hand
                player1Card = playerHand[6];
                playerHand.RemoveAt(6);
                playerDone = true;
                playerTurn = false;

                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Card08MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p08.Source != null && playedCard1.Source == null && IsPlayable(playerHand[7]))
            {
                // ...play the card.
                playedCard1.Source = p08.Source;
                p08.Source = null;

                // Remove card from hand
                player1Card = playerHand[7];
                playerHand.RemoveAt(7);
                playerDone = true;
                playerTurn = false;

                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Card09MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p09.Source != null && playedCard1.Source == null && IsPlayable(playerHand[8]))
            {
                // ...play the card.
                playedCard1.Source = p09.Source;
                p09.Source = null;

                // Remove card from hand
                player1Card = playerHand[8];
                playerHand.RemoveAt(8);
                playerDone = true;
                playerTurn = false;

                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Card10MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p10.Source != null && playedCard1.Source == null && IsPlayable(playerHand[9]))
            {
                // ...play the card.
                playedCard1.Source = p10.Source;
                p10.Source = null;

                // Remove card from hand
                player1Card = playerHand[9];
                playerHand.RemoveAt(9);
                playerDone = true;
                playerTurn = false;

                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Card11MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p11.Source != null && playedCard1.Source == null && IsPlayable(playerHand[10]))
            {
                // ...play the card.
                playedCard1.Source = p11.Source;
                p11.Source = null;

                // Remove card from hand
                player1Card = playerHand[10];
                playerHand.RemoveAt(10);
                playerDone = true;
                playerTurn = false;

                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Card12MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p12.Source != null && playedCard1.Source == null && IsPlayable(playerHand[11]))
            {
                // ...play the card.
                playedCard1.Source = p12.Source;
                p12.Source = null;

                // Remove card from hand
                player1Card = playerHand[11];
                playerHand.RemoveAt(11);
                playerDone = true;
                playerTurn = false;

                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Card13MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If this slot has a card in it and there's no currently played card...
            if (p13.Source != null && playedCard1.Source == null && IsPlayable(playerHand[12]))
            {
                // ...play the card.
                playedCard1.Source = p13.Source;
                p13.Source = null;

                // Remove card from hand
                player1Card = playerHand[12];
                playerHand.RemoveAt(12);
                playerDone = true;
                playerTurn = false;

                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        #endregion

        #region Scoring & Win Determination

        /// <summary>
        /// Main Scoring & Win Determination function that performs the end of round functions
        /// </summary>
        /// <param name="tarneeb">The current Tarneeb</param>
        /// <param name="card1">Player 1's card played</param>
        /// <param name="card2">Player 2's card played</param>
        /// <param name="card3">Player 3's card played</param>
        /// <param name="card4">Player 4's card played</param>
        private void EndOfRoundCleanup(Enums.Suit tarneeb, Card card1, Card card2, Card card3, Card card4)
        {
            // Determines the winner of the round when passed parent function's parameters
            DetermineWinner(tarneeb, card1, card2, card3, card4);

            // Displays the winner of the round
            DisplayWinner();

            // Adds points to the winning team and updates scores accordingly
            HandleScores();
        }

        /// <summary>
        /// Determines the winner of the round
        /// </summary>
        /// <param name="tarneeb">The current Tarneeb</param>
        /// <param name="card1">Player 1's card played</param>
        /// <param name="card2">Player 2's card played</param>
        /// <param name="card3">Player 3's card played</param>
        /// <param name="card4">Player 4's card played</param>
        public void DetermineWinner(Enums.Suit tarneeb, Card card1, Card card2, Card card3, Card card4)
        {

            Enums.Suit suit;
            Card winningCard = null;

            // Set winningCard equal to the card that was played first in the round
            switch (winner)
            {
                case 1:
                    winningCard = card1;
                    break;
                case 2:
                    winningCard = card2;
                    break;
                case 3:
                    winningCard = card3;
                    break;
                case 4:
                    winningCard = card4;
                    break;
                default:
                    break;
            }

            // If any tarneebs were played, set the winning suit to the tarneeb suit
            if (card1.Suit == tarneeb)
            {
                suit = tarneeb;
            }
            else if (card2.Suit == tarneeb)
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
            // Otherwise, the suit is the round suit (suit played by first player of round)
            else
            {
                suit = winningCard.Suit;
            }

            // Set winner to the player who played the highest card number of the winning suit
            if (card1.Suit == suit)
            {
                if (card1.CardNumber > winningCard.CardNumber || winningCard.Suit != suit)
                {
                    winningCard = card1;
                    winner = 1;
                }
            }

            if (card2.Suit == suit)
            {
                if (card2.CardNumber > winningCard.CardNumber || winningCard.Suit != suit)
                {
                    winningCard = card2;
                    winner = 2;
                }
            }

            if (card3.Suit == suit)
            {
                if (card3.CardNumber > winningCard.CardNumber || winningCard.Suit != suit)
                {
                    winningCard = card3;
                    winner = 3;
                }
            }

            if (card4.Suit == suit)
            {
                if (card4.CardNumber > winningCard.CardNumber || winningCard.Suit != suit)
                {
                    winner = 4;
                }
            }
        }

        /// <summary>
        /// Updates and displays winner labels based on the winner of the round
        /// </summary>
        public void DisplayWinner()
        {
            // Show the round winner label if the game isn't over
            if (cardsDone < 12)
            {
                switch (winner)
                {
                    case 1:
                        lblWinner.Content = "Player 1 Wins!";
                        lblWinner.Foreground = team1Color;
                        break;
                    case 2:
                        lblWinner.Content = "Player 2 Wins!";
                        lblWinner.Foreground = team2Color;
                        break;
                    case 3:
                        lblWinner.Content = "Player 3 Wins!";
                        lblWinner.Foreground = team1Color;
                        break;
                    case 4:
                        lblWinner.Content = "Player 4 Wins!";
                        lblWinner.Foreground = team2Color;
                        break;
                    default:
                        break;
                }
            }
            // If the game is over...
            else
            {
                // Switch the winning team to team 2 based on betting player and team scores
                if (bettingPlayer == 1 || bettingPlayer == 3)
                {
                    if (team1Score >= topBet)
                    {
                        WinningTeam(1);
                    }
                    else
                    {
                        WinningTeam(2);
                    }
                }

                if (bettingPlayer == 2 || bettingPlayer == 4)
                {
                    if (team2Score >= topBet)
                    {
                        WinningTeam(2);
                    }
                    else
                    {
                        WinningTeam(1);
                    }
                }
            }
        }

        /// <summary>
        /// Outputs the winning team (1 or 2)
        /// </summary>
        /// <param name="winningTeam">The winning team as int, accepts 1 or 2</param>
        public void WinningTeam(int winningTeam)
        {
            if (winningTeam == 1)
            {
                lblWinner.Content = "Team 1 Wins!";
                lblWinner.Foreground = team1Color;
            }
            else
            {
                lblWinner.Content = "Team 2 Wins!";
                lblWinner.Foreground = team2Color;
            }
        }

        /// <summary>
        /// Adds and updates scores based on the winner of the round
        /// </summary>
        public void HandleScores()
        {
            // Add score to whichever team won the round
            switch (winner)
            {
                case 1:
                    team1Score++;
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

            // Update scores
            UpdateTeamScores();
        }

        /// <summary>
        /// Updates the Score labels to current standing (current round score)
        /// </summary>
        public void UpdateTeamScores()
        {
            // Update Team 1 Score
            lblTeam1Score1.Content = team1Score;
            lblTeam1Score2.Content = team1Score;
            lblTeam1Score3.Content = team1Score;
            lblTeam1Score4.Content = team1Score;
            lblTeam1Score5.Content = team1Score;

            // Update Team 2 Score
            lblTeam2Score1.Content = team2Score;
            lblTeam2Score2.Content = team2Score;
            lblTeam2Score3.Content = team2Score;
            lblTeam2Score4.Content = team2Score;
            lblTeam2Score5.Content = team2Score;
        }

        /// <summary>
        /// Updates the Total labels to current standing (total summary score)
        /// </summary>
        public void UpdateTeamTotals()
        {
            // Update Team 1 Total
            lblTeam1Total1.Content = team1Total;
            lblTeam1Total2.Content = team1Total;
            lblTeam1Total3.Content = team1Total;
            lblTeam1Total4.Content = team1Total;
            lblTeam1Total5.Content = team1Total;

            // Update Team 2 Total
            lblTeam2Total1.Content = team2Total;
            lblTeam2Total2.Content = team2Total;
            lblTeam2Total3.Content = team2Total;
            lblTeam2Total4.Content = team2Total;
            lblTeam2Total5.Content = team2Total;
        }

        #endregion

    }
}
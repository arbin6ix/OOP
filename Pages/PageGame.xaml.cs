using OOP4200_Tarneeb.Cards;
using OOP4200_Tarneeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        #region Fields & Properties

        // Speed of the game in milliseconds
        public const int computerTurnRate = 800;
        public const int roundTurnRate = 2500;

        public Enums.Suit tarneeb;          // Tarneeb (trump card)
        public bool tarneebPlayed = false;  // Tarneeb played bool
        public Card firstCard;              // The first card played in the round
        public Card cardToBeat;             // The best card played in the round
        public int playerToBeat = 0;        // The player who played the best card, 0 = no player set
        public int cardsDone = 0;           // # of remaining cards in the hand
        public Random rand = new Random();  // Random class object instantiation

        public bool playerTurn = false;     // Needed for async, prevents player from clicking card when not player's turn
        public bool playerDone = false;
        public bool roundDone = false;
        public bool gameDone = false;

        // The winner of the betting or the round. Winner places the first card of a new turn.
        // winner = 0 means new round (betting)
        public static int winner = 1;

        // Team round scores
        public int team1Score = 0;
        public int team2Score = 0;

        // Team total scores (game is to 31)
        public int team1Total = 0;
        public int team2Total = 0;

        

        // Team Colours + Misc Colours
        public SolidColorBrush team1Color = new SolidColorBrush(Color.FromRgb(51, 188, 255));
        public SolidColorBrush team2Color = new SolidColorBrush(Color.FromRgb(255, 90, 90));
        public SolidColorBrush scoreColor = new SolidColorBrush(Color.FromRgb(232, 193, 51));
        public SolidColorBrush greenColor = new SolidColorBrush(Color.FromRgb(61, 184, 93));
        public SolidColorBrush blackColor = new SolidColorBrush(Color.FromRgb(0, 0, 0));

        // Create a list of the player's cards Image controls from the PageGame.xaml form
        public List<Image> playerCardImages = new List<Image>();

        // Image of empty card (used for AI turn check)
        ImageSource cardPlaceholder = (ImageSource) new ImageSourceConverter().ConvertFrom(@"../../../Images/EmptyCard.png");

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

        // AI Difficulty setting (1 = easy, 2 = medium, 3 = hard)
        public int computerDifficulty = ((MainWindow)Application.Current.MainWindow).difficulty;

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
            LoadPlayer1Name();
            NewRound();
            LogNewGame();
        }

        /// <summary>
        /// Display Player1 name dynamically. 
        /// </summary>
        private void LoadPlayer1Name() 
        {
            lblPlayer1_1.Content = Globals.gameStats.PlayerName;
            lblPlayer1_2.Content = Globals.gameStats.PlayerName;
            lblPlayer1_3.Content = Globals.gameStats.PlayerName;
            lblPlayer1_4.Content = Globals.gameStats.PlayerName;
            lblPlayer1_5.Content = Globals.gameStats.PlayerName;
        }

        /// <summary>
        /// Saves the new game in the log
        /// </summary>
        public async void LogNewGame()
        {
            await DBUtility.SaveLog(new Log("New Game", "", ""));
        }

        /// <summary>
        /// Saves the new round in the log
        /// </summary>
        public async Task LogNewRound()
        {
            await DBUtility.SaveLog(new Log("New Round", "", ""));
        }

        /// <summary>
        /// Initiates a new round of Tarneeb, including shuffling deck, dealing cards, resetting variables, etc..
        /// </summary>
        public async void NewRound()
        {
            await LogNewRound();
            
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
            await DBUtility.SaveLog(new Log("Player1-hand", "", String.Concat(playerHand.Select(o => o.Suit + "-" + o.CardNumber + ","))));
            Player player2 = new Player(hand2);
            await DBUtility.SaveLog(new Log("Player2-hand", "", String.Concat(hand2.Select(o => o.Suit + "-" + o.CardNumber + ","))));
            Player player3 = new Player(hand3);
            await DBUtility.SaveLog(new Log("Player3-hand", "", String.Concat(hand3.Select(o => o.Suit + "-" + o.CardNumber + ","))));
            Player player4 = new Player(hand4);
            await DBUtility.SaveLog(new Log("Player4-hand", "", String.Concat(hand4.Select(o => o.Suit + "-" + o.CardNumber + ","))));

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
            playerToBeat = 0;

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

            //await DBUtility.SaveLog(new Log("Betting Done", "", ""));

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

        /// <summary>
        /// Add 1 to selecting bet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBetAddClick(object sender, RoutedEventArgs e)
        {
            // can't go above maximum bet
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

        /// <summary>
        /// Subtract 1 from selecting bet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBetSubClick(object sender, RoutedEventArgs e)
        {
            // can't go under minimum bet
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

        /// <summary>
        /// Pass button. Pass then AIs bet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


            // AIs bet until 2 passed
            while ((player2IsBetting && player3IsBetting) || (player2IsBetting && player4IsBetting) || (player3IsBetting && player4IsBetting))
            {
                Player2Bet();
                Player3Bet();
                Player4Bet();
            }
            // comes back to AI that made bet so it knows it won
            Player2Bet();
            Player3Bet();
            Player4Bet();

            // No bets were made, restart the game (reshuffle)
            if (bettingPlayer == 0)
            {
                PageGame gamePage = new PageGame();
                NavigationService.Navigate(gamePage);
            }
        }

        /// <summary>
        /// Bet button. Makes bet then AIs bet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBetClick(object sender, RoutedEventArgs e)
        {
            // display labels
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

            // if AIs passed
            if (bettingPlayer == player1Betting)
            {
                HideBettingButtons();
                ShowTarneebSelection();
            }
        }

        /// <summary>
        /// Player 2 betting functionality
        /// </summary>
        public void Player2Bet()
        {
            if (bettingPlayer == 2)
            {
                // no one made a bet. AI selects tarneeb
                HideBettingButtons();
                SetTarneeb(AITarneebSelection(hand2));
                winner = bettingPlayer;
                DoComputerTurns();
            }
            else
            {
                // AI has not passed
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

                        // minimum bet is 1 above the bet made
                        minimumBet = playerBetAmount + 1;

                        // player 2 is the current player betting
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

        /// <summary>
        /// Player 3 betting functionality
        /// </summary>
        public void Player3Bet()
        {
            if (bettingPlayer == 3)
            {
                // Everyone passed. Select Tarneeb
                HideBettingButtons();
                SetTarneeb(AITarneebSelection(hand4));
                winner = bettingPlayer;
                DoComputerTurns();
            }
            else
            {
                // AI has not passed
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
                        // pass
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
                        // bet
                        if (playerBetAmount >= maximumBet)
                        {
                            playerBetAmount = maximumBet;

                            player1IsBetting = false;
                            player2IsBetting = false;
                            player4IsBetting = false;
                        }

                        // minimum bet is 1 above the bet made
                        minimumBet = playerBetAmount + 1;

                        // player 3 is the current player betting
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
        /// <summary>
        /// Player 4 betting functionality
        /// </summary>
        public void Player4Bet()
        {
            if (bettingPlayer == 4)
            {
                // Everyone passed. Select Tarneeb
                HideBettingButtons();
                SetTarneeb(AITarneebSelection(hand4));
                winner = bettingPlayer;
                DoComputerTurns();
            }
            else
            {
                // AI has not passed
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
                        // pass
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
                        // bet
                        if (playerBetAmount >= maximumBet)
                        {
                            playerBetAmount = maximumBet;

                            player1IsBetting = false;
                            player2IsBetting = false;
                            player3IsBetting = false;
                        }

                        // minimum bet is 1 above the bet made
                        minimumBet = playerBetAmount + 1;

                        // player 4 is the current player betting
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

        /// <summary>
        /// AI betting functionality
        /// </summary>
        /// <param name="cards"></param>
        /// <returns>bettingAmount</returns>
        public int AIBettingAmount(List<Card> cards)
        {
            Deck deck = new Deck();
            List<Card> diamonds = new List<Card>();
            List<Card> clubs = new List<Card>();
            List<Card> hearts = new List<Card>();
            List<Card> spades = new List<Card>();
            List<Card> sortedCards = new List<Card>();

            int diamondRunAmount = 0;
            int clubRunAmount = 0;
            int heartRunAmount = 0;
            int spadeRunAmount = 0;

            int diamondTotalPoints = 0;
            int clubTotalPoints = 0;
            int heartTotalPoints = 0;
            int spadeTotalPoints = 0;

            int bettingAmount = 0;

            sortedCards = deck.Sort(cards);

            // create 4 lists for each suit
            foreach (Card card in sortedCards)
            {
                if (card.Suit == Enums.Suit.DIAMOND)
                {
                    diamonds.Add(card);
                }
                if (card.Suit == Enums.Suit.CLUB)
                {
                    clubs.Add(card);
                }
                if (card.Suit == Enums.Suit.HEART)
                {
                    hearts.Add(card);
                }
                if (card.Suit == Enums.Suit.SPADE)
                {
                    spades.Add(card);
                }
            }
            // highest to lowest
            diamonds.Reverse();
            clubs.Reverse();
            hearts.Reverse();
            spades.Reverse();

            // count number of runs from Ace down. They are almost always guarenteed wins
            try
            {
                for (int i = 0; (int)diamonds[i].CardNumber + i == (int)Enums.CardNumber.ACE && i + 1 < diamonds.Count; i++)
                {
                    diamondRunAmount += 1;
                }
            }
            catch
            { }
            try
            {
                for (int i = 0; (int)clubs[i].CardNumber + i == (int)Enums.CardNumber.ACE && i + 1 < clubs.Count; i++)
                {
                    clubRunAmount += 1;
                }
            }
            catch
            { }
            try
            {
                for (int i = 0; (int)hearts[i].CardNumber + i == (int)Enums.CardNumber.ACE && i + 1 < hearts.Count; i++)
                {
                    heartRunAmount += 1;
                }
            }
            catch
            { }
            try
            {
                for (int i = 0; (int)spades[i].CardNumber + i == (int)Enums.CardNumber.ACE && i + 1 < spades.Count; i++)
                {
                    spadeRunAmount += 1;
                }
            }
            catch
            { }

            // point system to determine which suit is the top suit
            foreach (Card card in diamonds)
            {
                diamondTotalPoints += (int)card.CardNumber + 10;
            }
            foreach (Card card in clubs)
            {
                clubTotalPoints += (int)card.CardNumber + 10;
            }
            foreach (Card card in hearts)
            {
                heartTotalPoints += (int)card.CardNumber + 10;
            }
            foreach (Card card in spades)
            {
                spadeTotalPoints += (int)card.CardNumber + 10;
            }

            int topTotalPoints = diamondTotalPoints;

            if (clubTotalPoints > topTotalPoints)
            {
                topTotalPoints = clubTotalPoints;
            }
            if (heartTotalPoints > topTotalPoints)
            {
                topTotalPoints = heartTotalPoints;
            }
            if (spadeTotalPoints > topTotalPoints)
            {
                topTotalPoints = spadeTotalPoints;
            }

            // bettting amount is based on how many points the top suit has
            int betNum = 3;
            float points = 40;
            while(topTotalPoints > points)
            {
                bettingAmount = betNum;
                points += points - 20;
                betNum += 1;
            }

            // each run is almost a guaranteed hand win
            bettingAmount += diamondRunAmount + clubRunAmount + heartRunAmount + spadeRunAmount;

            return bettingAmount;
        }

        /// <summary>
        /// AI Tarneeb selection functionality
        /// </summary>
        /// <param name="cards"></param>
        /// <returns>topSuit</returns>
        public Enums.Suit AITarneebSelection(List<Card> cards)
        {
            Deck deck = new Deck();
            List<Card> diamonds = new List<Card>();
            List<Card> clubs = new List<Card>();
            List<Card> hearts = new List<Card>();
            List<Card> spades = new List<Card>();
            List<Card> sortedCards = new List<Card>();

            int diamondTotalPoints = 0;
            int clubTotalPoints = 0;
            int heartTotalPoints = 0;
            int spadeTotalPoints = 0;

            Enums.Suit topSuit = Enums.Suit.DIAMOND;

            sortedCards = deck.Sort(cards);

            // similar to AIBettingAmount. 
            foreach (Card card in sortedCards)
            {
                if (card.Suit == Enums.Suit.DIAMOND)
                {
                    diamonds.Add(card);
                }
                if (card.Suit == Enums.Suit.CLUB)
                {
                    clubs.Add(card);
                }
                if (card.Suit == Enums.Suit.HEART)
                {
                    hearts.Add(card);
                }
                if (card.Suit == Enums.Suit.SPADE)
                {
                    spades.Add(card);
                }
            }

            diamonds.Reverse();
            clubs.Reverse();
            hearts.Reverse();
            spades.Reverse();

            // point system to determine top suit
            foreach (Card card in diamonds)
            {
                diamondTotalPoints += (int)card.CardNumber + 10;
            }
            foreach (Card card in clubs)
            {
                clubTotalPoints += (int)card.CardNumber + 10;
            }
            foreach (Card card in hearts)
            {
                heartTotalPoints += (int)card.CardNumber + 10;
            }
            foreach (Card card in spades)
            {
                spadeTotalPoints += (int)card.CardNumber + 10;
            }

            int topTotalPoints = diamondTotalPoints;

            if (clubTotalPoints > topTotalPoints)
            {
                topTotalPoints = clubTotalPoints;
                topSuit = Enums.Suit.CLUB;
            }
            if (heartTotalPoints > topTotalPoints)
            {
                topTotalPoints = heartTotalPoints;
                topSuit = Enums.Suit.HEART;
            }
            if (spadeTotalPoints > topTotalPoints)
            {
                topTotalPoints = spadeTotalPoints;
                topSuit = Enums.Suit.SPADE;
            }

            DBUtility.SaveLog(new Log("AITarneeb-selected", "Computer", topSuit.ToString()));

            // return the top suit
            return topSuit;
        }

        /// <summary>
        /// Hide betting buttons and labels and display the bet won
        /// </summary>
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
            btnPass.IsDefault = false;
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
                //log betting
                DBUtility.SaveLog(new Log("Betting Done", "Team 1 bet", topBet.ToString()));
            }
            else if (bettingPlayer == 2 || bettingPlayer == 4)
            {
                lblBet1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5A5A"));
                //log betting
                DBUtility.SaveLog(new Log("Betting Done", "Team 2 bet" , topBet.ToString()));
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

        /// <summary>
        /// Reset betting fields to default values
        /// </summary>
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
            btnPass.IsDefault = true;

            lblBet1.Visibility = Visibility.Hidden;
            lblBet2.Visibility = Visibility.Hidden;
            lblBet3.Visibility = Visibility.Hidden;
            lblBet4.Visibility = Visibility.Hidden;
            lblBet5.Visibility = Visibility.Hidden;

            winner = 0;
        }

        /// <summary>
        /// Change betting number to the minimum betting amount
        /// </summary>
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
        public async void SetTarneeb(Enums.Suit suit)
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
            await DBUtility.SaveLog(new Log("Player1 tarneeb selected", "Player1", suit.ToString()));
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
        /// <param name="hand">The AI's hand</param>
        /// <param name="playerNumber">The AI's player number (2, 3, or 4)</param>
        /// <returns></returns>
        public Card AIChooseCard(List<Card> hand, int playerNumber)
        {
            // The card chosen by the AI to play
            Card chosenCard = new Card();

            // Create new list of cards matching suit, list of tarneebs, and list of non-tarneebs
            List<Card> matchingList = new List<Card>();
            List<Card> tarneebList = new List<Card>();
            List<Card> otherList = new List<Card>();

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

                if (firstCard == null)
                {
                    firstCard = cardToBeat;
                }
            }


            // Dividing hand into three convenient lists:
            for (int i = 0; i < hand.Count; i++)
            {
                // 1. List of cards matching first suit played
                if ((int)hand[i].Suit == (int)firstCard.Suit)
                {
                    matchingList.Add(hand[i]);
                }

                if (tarneebPlayed)
                {
                    // 2. List of cards matching tarneeb suit
                    if (hand[i].Suit == tarneeb)
                    {
                        tarneebList.Add(hand[i]);
                    }
                    // 3. List of cards not matching tarneeb suit
                    else
                    {
                        otherList.Add(hand[i]);
                    }
                }
            }

            // Teamplay section:
            // If teammate is either winning the hand, or likely to win with a high value card,
            // immediately return the worst card in hand.
            // DIFFICULTY SCALING: Opponent's ability to judge likelihood of winning depends on difficulty:
            // Hard mode   = Opponent sees 'Queen' or higher as likely to win
            // Medium mode = Opponent sees '10' or higher as likely to win
            // Easy mode   = Opponent sees '8' or higher as likely to win
            if ((playerNumber == 2 && playerToBeat == 4 && (playedCard3.Source != null || (int)cardToBeat.CardNumber > (4 + (2 * computerDifficulty)))) ||
                (playerNumber == 3 && playerToBeat == 1 && (playedCard4.Source != null || (int)cardToBeat.CardNumber > 10)) ||
                (playerNumber == 4 && playerToBeat == 2 && (playedCard1.Source != null || (int)cardToBeat.CardNumber > (4 + (2 * computerDifficulty)))))
            {
                // If there are no cards with a matching suit...
                if (matchingList.Count == 0)
                {
                    // If there's a non-tarneeb card left to play
                    if (otherList.Count > 0)
                    {
                        // ...loop through the remaining cards and pick out the lowest value non-tarneeb
                        for (int i = 0; i < otherList.Count; i++)
                        {
                            // If a card hasn't been chosen OR the current card's number is lower than
                            // the chosen card's number...
                            if (i == 0 || (int)otherList[i].CardNumber < (int)chosenCard.CardNumber)
                            {
                                // ... choose the current card to play
                                chosenCard = otherList[i];
                            }
                        }
                    }
                    else
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
                            }
                        }
                    }
                }
                // If there is a matching card, play the lowest value matching card
                else
                {
                    // Loop through the remaining cards and pick out the lowest value card
                    for (int i = 0; i < matchingList.Count; i++)
                    {
                        // If a card hasn't been chosen OR the current card's number is lower than
                        // the chosen card's number...
                        if (i == 0 || (int)matchingList[i].CardNumber < (int)chosenCard.CardNumber)
                        {
                            // ... choose the current card to play
                            chosenCard = matchingList[i];
                        }
                    }
                }

                return chosenCard;
            }


            // Comparing matching suits in hand:
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
                    playerToBeat = playerNumber;
                }
                // If the lower card doesn't beat the cardToBeat but the higher one does,
                // play the higher one
                else if (playedNumber < (int)matchingList[i].CardNumber)
                {
                    // ...choose the current card to play
                    chosenCard = matchingList[i];

                    // Since this card is better, set it to the new cardToBeat
                    cardToBeat = chosenCard;
                    playerToBeat = playerNumber;
                }
            }


            // Comparing non-matching suits in hand:
            // If there are no cards with a matching suit...
            if (matchingList.Count == 0)
            {
                // If there's a non-tarneeb card left to play
                if (otherList.Count > 0)
                {
                    // ...loop through the remaining cards and pick out the lowest value non-tarneeb
                    for (int i = 0; i < otherList.Count; i++)
                    {
                        // If a card hasn't been chosen OR the current card's number is lower than
                        // the chosen card's number...
                        if (i == 0 || (int)otherList[i].CardNumber < (int)chosenCard.CardNumber)
                        {
                            // ... choose the current card to play
                            chosenCard = otherList[i];
                        }
                    }
                }
                else
                {
                    // ...loop through the remaining cards and pick out the lowest value card
                    for (int i = 0; i < tarneebList.Count; i++)
                    {
                        // If a card hasn't been chosen OR the current card's number is lower than
                        // the chosen card's number...
                        if (i == 0 || (int)tarneebList[i].CardNumber < (int)chosenCard.CardNumber)
                        {
                            // ... choose the current card to play
                            chosenCard = tarneebList[i];
                        }
                    }
                }


                // Tarneeb cutting logic (steal high value hand with low value tarneeb)
                // If the card to beat is a non-tarneeb KING or ACE and AI has at least one tarneeb in hand,
                // play the lowest value tarneeb.
                // DIFFICULTY SCALING: Opponent does not do this on Easy mode
                if ((playerNumber == 3 || computerDifficulty > 1) &&
                    (int)cardToBeat.CardNumber > 11 &&
                    cardToBeat.Suit != tarneeb &&
                    tarneebList.Count > 0)
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
                            playerToBeat = playerNumber;
                        }
                    }
                }

                // Cutting logic as last player in trick
                if (tarneebList.Count > 0)
                {
                    if ((playerNumber == 2 && playedCard3.Source != null && playerToBeat != 4) ||
                        (playerNumber == 3 && playedCard4.Source != null && playerToBeat != 1) ||
                        (playerNumber == 4 && playedCard1.Source != null && playerToBeat != 2))
                    {
                        // Set placeholder to check lowest value tarneeb
                        Card lowestTarneeb = null;

                        // ...loop through the remaining cards and pick out the lowest value tarneeb
                        for (int i = 0; i < tarneebList.Count; i++)
                        {
                            // If a card hasn't been chosen OR the current card's number is lower than
                            // the chosen card's number...
                            if (i == 0 || (int)tarneebList[i].CardNumber < (int)lowestTarneeb.CardNumber)
                            {
                                // ... choose the current tarneeb
                                lowestTarneeb = tarneebList[i];

                                // Play the tarneeb if it's less than a Jack
                                if ((int)lowestTarneeb.CardNumber < 10)
                                {
                                    // The tarneeb played beats the non-tarneeb cardToBeat
                                    chosenCard = lowestTarneeb;
                                    cardToBeat = lowestTarneeb;
                                    playerToBeat = playerNumber;
                                }
                            }
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
            if(playedCard2.Source == null)
            {
                playedCard2.Source = cardPlaceholder;
                await Task.Delay(computerTurnRate);
                Card chosenCard;
                chosenCard = AIChooseCard(hand2, 2);
                player2Card = chosenCard;
                playedCard2.Source = Card.ToImage(chosenCard);
                hand2.RemoveAll(card => card.CardNumber == chosenCard.CardNumber && card.Suit == chosenCard.Suit);
                await DBUtility.SaveLog(new Log("Player2 turn", "Player2", chosenCard.ToString()));
            }
        }

        /// <summary>
        /// Turn logic for Player 3 AI
        /// </summary>
        public async Task Player3Turn()
        {
            if (playedCard3.Source == null)
            {
                playedCard3.Source = cardPlaceholder;
                await Task.Delay(computerTurnRate);
                Card chosenCard;
                chosenCard = AIChooseCard(hand3, 3);
                player3Card = chosenCard;
                playedCard3.Source = Card.ToImage(chosenCard);
                hand3.RemoveAll(card => card.CardNumber == chosenCard.CardNumber && card.Suit == chosenCard.Suit);
                await DBUtility.SaveLog(new Log("Player3 turn", "Player3", chosenCard.ToString()));
            }
        }

        /// <summary>
        /// Turn logic for Player 4 AI
        /// </summary>
        public async Task Player4Turn()
        {
            if (playedCard4.Source == null)
            {
                playedCard4.Source = cardPlaceholder;
                await Task.Delay(computerTurnRate);
                Card chosenCard;
                chosenCard = AIChooseCard(hand4, 4);
                player4Card = chosenCard;
                playedCard4.Source = Card.ToImage(chosenCard);
                hand4.RemoveAll(card => card.CardNumber == chosenCard.CardNumber && card.Suit == chosenCard.Suit);
                await DBUtility.SaveLog(new Log("Player4 turn", "Player4", chosenCard.ToString()));
            }
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
            if (cardsDone < 13 && !GameOver())
            {
                
                await Task.Delay(roundTurnRate);
                InitiateNextRound();
            }
            // If the cards are finished and the game isn't, prompt for next round
            else if (!GameOver())
            {
                await Task.Delay(roundTurnRate);
                btnNextRound.Background = greenColor;
                btnNextRound.Foreground = blackColor;
                btnNextRound.Content = "Next Round?";
                btnNextRound.Visibility = Visibility.Visible;
                btnNextRound.IsEnabled = true;
                btnNextRound.IsDefault = true;
            }
            // If the game is over, prompt for new game
            else
            {
                btnNextRound.Background = scoreColor;
                btnNextRound.Foreground = blackColor;
                btnNextRound.Content = "New Game?";
                btnNextRound.Visibility = Visibility.Visible;
                btnNextRound.IsEnabled = true;
                btnNextRound.IsDefault = true;
            }
        }


        #endregion

        #region Button Functionality

        /// <summary>
        /// Starts the next round of tarneeb
        /// </summary>
        private void InitiateNextRound()
        {
            if (cardsDone < 13 && roundDone && !GameOver())
            {
                // Clear cards played
                player1Card = null;
                player2Card = null;
                player3Card = null;
                player4Card = null;
                firstCard = null;
                cardToBeat = null;
                playerToBeat = 0;

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
            // All 4 players are out of cards. Start new round
            else if (roundDone && !GameOver())
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
            // If a team has won and "New Game" button is clicked, create a new fresh game
            else
            {
                PageGame gamePage = new PageGame();
                NavigationService.Navigate(gamePage);
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
            btnNextRound.IsDefault = false;
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

        private void SetCardToBeat(Card playersCard)
        {
            int playersNumber = (int)playersCard.CardNumber;
            Enums.Suit playersSuit = playersCard.Suit;

            // If card to beat is already set, compare players card to the card to beat
            if (cardToBeat != null)
            {
                int opponentNumber = (int)cardToBeat.CardNumber;
                Enums.Suit opponentSuit = cardToBeat.Suit;

                // Set cardToBeat to player's card if player cuts with tarneeb ...
                if (opponentSuit != tarneeb && playersSuit == tarneeb)
                {
                    cardToBeat = playersCard;
                    playerToBeat = 1;
                }
                // ... or if the player's card is a better card of the same suit
                else if (opponentSuit == playersSuit && playersNumber > opponentNumber)
                {
                    cardToBeat = playersCard;
                    playerToBeat = 1;
                }
            }
            // If card to beat is not already set, set it to player's card (first card of hand)
            else
            {
                cardToBeat = playersCard;
                playerToBeat = 1;
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Card01MouseDown(object sender, MouseButtonEventArgs e)
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

                // Set card to beat (if applicable)
                SetCardToBeat(player1Card);

                await DBUtility.SaveLog(new Log("Player1 turn", "Player1", player1Card.ToString()));
                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Card02MouseDown(object sender, MouseButtonEventArgs e)
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

                // Set card to beat (if applicable)
                SetCardToBeat(player1Card);

                await DBUtility.SaveLog(new Log("Player1 turn", "Player1", player1Card.ToString()));
                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Card03MouseDown(object sender, MouseButtonEventArgs e)
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

                // Set card to beat (if applicable)
                SetCardToBeat(player1Card);

                await DBUtility.SaveLog(new Log("Player1 turn", "Player1", player1Card.ToString()));
                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Card04MouseDown(object sender, MouseButtonEventArgs e)
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

                // Set card to beat (if applicable)
                SetCardToBeat(player1Card);

                await DBUtility.SaveLog(new Log("Player1 turn", "Player1", player1Card.ToString()));
                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Card05MouseDown(object sender, MouseButtonEventArgs e)
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

                // Set card to beat (if applicable)
                SetCardToBeat(player1Card);

                await DBUtility.SaveLog(new Log("Player1 turn", "Player1", player1Card.ToString()));
                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Card06MouseDown(object sender, MouseButtonEventArgs e)
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

                // Set card to beat (if applicable)
                SetCardToBeat(player1Card);

                await DBUtility.SaveLog(new Log("Player1 turn", "Player1", player1Card.ToString()));
                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Card07MouseDown(object sender, MouseButtonEventArgs e)
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

                // Set card to beat (if applicable)
                SetCardToBeat(player1Card);

                await DBUtility.SaveLog(new Log("Player1 turn", "Player1", player1Card.ToString()));
                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Card08MouseDown(object sender, MouseButtonEventArgs e)
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

                // Set card to beat (if applicable)
                SetCardToBeat(player1Card);

                await DBUtility.SaveLog(new Log("Player1 turn", "Player1", player1Card.ToString()));
                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Card09MouseDown(object sender, MouseButtonEventArgs e)
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

                // Set card to beat (if applicable)
                SetCardToBeat(player1Card);

                await DBUtility.SaveLog(new Log("Player1 turn", "Player1", player1Card.ToString()));
                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Card10MouseDown(object sender, MouseButtonEventArgs e)
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

                // Set card to beat (if applicable)
                SetCardToBeat(player1Card);

                await DBUtility.SaveLog(new Log("Player1 turn", "Player1", player1Card.ToString()));
                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Card11MouseDown(object sender, MouseButtonEventArgs e)
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

                // Set card to beat (if applicable)
                SetCardToBeat(player1Card);

                await DBUtility.SaveLog(new Log("Player1 turn", "Player1", player1Card.ToString()));
                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Card12MouseDown(object sender, MouseButtonEventArgs e)
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

                // Set card to beat (if applicable)
                SetCardToBeat(player1Card);

                await DBUtility.SaveLog(new Log("Player1 turn", "Player1", player1Card.ToString()));
                // Complete computer turns
                ComputerTurnLogic();
            }
        }

        /// <summary>
        /// Plays the card that is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Card13MouseDown(object sender, MouseButtonEventArgs e)
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

                // Set card to beat (if applicable)
                SetCardToBeat(player1Card);

                await DBUtility.SaveLog(new Log("Player1 turn", "Player1", player1Card.ToString()));
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

            // Adds points to the winning team and updates scores accordingly
            HandleScores();

            // Displays the winner of the round
            DisplayWinner();
        }

        /// <summary>
        /// Determines the card that wins the round
        /// </summary>
        /// <param name="tarneeb">The current Tarneeb</param>
        /// <param name="card1">Player 1's card played</param>
        /// <param name="card2">Player 2's card played</param>
        /// <param name="card3">Player 3's card played</param>
        /// <param name="card4">Player 4's card played</param>
        public async void DetermineWinner(Enums.Suit tarneeb, Card card1, Card card2, Card card3, Card card4)
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
            await DBUtility.SaveLog(new Log("Winner of this round", "", "Player" + winner));
        }

        /// <summary>
        /// Determine if either team has more than 30 total points
        /// </summary>
        /// <returns>True or False</returns>
        public bool GameOver()
        {
            if (team1Total > 30)
            {
                DBUtility.SaveLog(new Log("Game Over", "", "team 1 won!"));
            }
            if (team2Total > 30)
            {
                DBUtility.SaveLog(new Log("Game Over", "", "team 2 won!"));
            }
            return team1Total > 30 || team2Total > 30;
        }

        /// <summary>
        /// Updates and displays winner labels based on the winner of the round
        /// </summary>
        public void DisplayWinner()
        {
            // Show the round winner label if the game isn't over
            if (cardsDone < 12 && !GameOver())
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
            // If the round is over, but the game isn't...
            else if (!GameOver())
            {
                // Switch the winning team to team 2 based on betting player and team scores
                if (bettingPlayer == 1 || bettingPlayer == 3)
                {
                    if (team1Score >= topBet)
                    {
                        // Update team1Total score
                        team1Total += team1Score;

                        RoundWinner(1);
                    }
                    else
                    {
                        // Update both team1Total & team2Total scores
                        team1Total -= topBet;
                        team2Total += team2Score;

                        RoundWinner(2);
                    }
                }

                if (bettingPlayer == 2 || bettingPlayer == 4)
                {
                    if (team2Score >= topBet)
                    {
                        // Update team2Total score
                        team2Total += team2Score;

                        RoundWinner(2);
                    }
                    else
                    {
                        // Update both team1Total & team2Total scores
                        team1Total += team1Score;
                        team2Total -= topBet;

                        RoundWinner(1);
                    }
                }
                
                // Update team total scores
                UpdateTeamTotals();
            }

            // If a team has reached at least 31 total points, end the game.
            if (GameOver())
            {
                // Display the winner and update the Log & Stats
                if (team1Total > 30)
                {
                    GameWinner(1);
                }
                else
                {
                    GameWinner(2);
                }
            }
        }

        /// <summary>
        /// Outputs the winning team of the round (1 or 2)
        /// </summary>
        /// <param name="winningTeam">The winning team of the round as int, accepts 1 or 2</param>
        public void RoundWinner(int winningTeam)
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
        /// Outputs the winning team of the game (1 or 2)
        /// </summary>
        /// <param name="winningTeam">The winning team of the game as int, accepts 1 or 2</param>
        public async void GameWinner(int winningTeam)
        {
            if (winningTeam == 1)
            {
                lblWinner.Content = "Team 1 Wins!";
                lblWinner.Foreground = team1Color;

                //Increment numOfWins by 1
                Globals.gameStats.NumWins++;
                await DBUtility.SaveStats(Globals.gameStats);
            }
            else
            {
                lblWinner.Content = "Team 2 Wins!";
                lblWinner.Foreground = team2Color;

                //Increment numOfLosses by 1
                Globals.gameStats.NumLosses++;
                await DBUtility.SaveStats(Globals.gameStats);
            }
            //Increment numOfGames by 1
            Globals.gameStats.NumGames++;
            await DBUtility.SaveStats(Globals.gameStats);

            // Set gameDone bool to true
            gameDone = true;
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
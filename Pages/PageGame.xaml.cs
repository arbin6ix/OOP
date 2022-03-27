using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OOP4200_Tarneeb.Cards;

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

        // Create a list of the player's cards Image controls from the PageGame.xaml form
        List<Image> playerCardImages = new List<Image>();


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
            List<Card> playerHand = deck.Sort(deck.TakeCards(13));
            List<Card> hand2 = deck.Sort(deck.TakeCards(13));
            List<Card> hand3 = deck.Sort(deck.TakeCards(13));
            List<Card> hand4 = deck.Sort(deck.TakeCards(13));

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
            for (int i = 0; i < playerCardImages.Count; i++)
            {
                playerCardImages[i].Source = Card.ToImage(hand[i]);
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


        private void btnGameBackClick(object sender, RoutedEventArgs e)
        {
            PageMenu menuPage = new PageMenu();
            NavigationService.Navigate(menuPage);
        }
    }
}

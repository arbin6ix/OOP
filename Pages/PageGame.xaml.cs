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
        public PageGame()
        {
            InitializeComponent();
            DealCards();
        }

        private void DealCards()
        {
            // Create a deck
            var deck = new Deck();

            // Shuffle the deck
            deck.Shuffle();

            // Pass out 13 cards to each
            List<Card> hand1 = deck.Sort(deck.TakeCards(13));
            List<Card> hand2 = deck.Sort(deck.TakeCards(13));
            List<Card> hand3 = deck.Sort(deck.TakeCards(13));
            List<Card> hand4 = deck.Sort(deck.TakeCards(13));

            // Create 4 Players each with their hand of 13 shuffled cards
            Player player1 = new Player(hand1);
            Player player2 = new Player(hand2);
            Player player3 = new Player(hand3);
            Player player4 = new Player(hand4);

            // Display player 1's hand for the human player
            p01.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/" + Card.ToImage(hand1[0]) + ".bmp");
            p02.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/" + Card.ToImage(hand1[1]) + ".bmp");
            p03.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/" + Card.ToImage(hand1[2]) + ".bmp");
            p04.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/" + Card.ToImage(hand1[3]) + ".bmp");
            p05.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/" + Card.ToImage(hand1[4]) + ".bmp");
            p06.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/" + Card.ToImage(hand1[5]) + ".bmp");
            p07.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/" + Card.ToImage(hand1[6]) + ".bmp");
            p08.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/" + Card.ToImage(hand1[7]) + ".bmp");
            p09.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/" + Card.ToImage(hand1[8]) + ".bmp");
            p10.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/" + Card.ToImage(hand1[9]) + ".bmp");
            p11.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/" + Card.ToImage(hand1[10]) + ".bmp");
            p12.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/" + Card.ToImage(hand1[11]) + ".bmp");
            p13.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/" + Card.ToImage(hand1[12]) + ".bmp");
        }

        private void btnGameBackClick(object sender, RoutedEventArgs e)
        {
            PageMenu menuPage = new PageMenu();
            NavigationService.Navigate(menuPage);
        }
    }
}

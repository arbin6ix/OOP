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

namespace OOP4200_Tarneeb
{
    /// <summary>
    /// Interaction logic for PageMenu.xaml
    /// </summary>
    public partial class PageMenu : Page
    {
        public PageMenu()
        {
            InitializeComponent();
        }

        private void btnHowToClick(object sender, RoutedEventArgs e)
        {
            PageHowToPlay howToPage = new PageHowToPlay();
            NavigationService.Navigate(howToPage);
        }

        private void btnStatsClick(object sender, RoutedEventArgs e)
        {
            PageStats statsPage = new PageStats();
            NavigationService.Navigate(statsPage);
        }

        private void btnExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnPlayClick(object sender, RoutedEventArgs e)
        {
            PageGame gamePage = new PageGame();
            NavigationService.Navigate(gamePage);
        }
    }
}

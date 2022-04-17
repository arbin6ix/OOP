using OOP4200_Tarneeb.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for PageStats.xaml
    /// </summary>
    public partial class PageStats : Page
    {
        public PageStats()
        {
            InitializeComponent();
            //Display the current stats
            txtTimesPlayed.Text = Globals.gameStats.NumGames.ToString();
            txtNumberOfWins.Text = Globals.gameStats.NumWins.ToString();
            txtNumberOfLosses.Text = Globals.gameStats.NumLosses.ToString();
            txtPlayerName.Text = Globals.gameStats.PlayerName.ToString();
            this.UpdateWinLossRates();
            this.UpdateHoursPlayed();
            Globals.gameStopWatch.Start();
        }

        /// <summary>
        /// Updates the win loss percentages based on number of wins/losses
        /// </summary>
        private void UpdateWinLossRates()
        {
            int percentWins = 0;
            int percentLosses = 0;
            if (Globals.gameStats.NumGames > 0)
            {
                percentWins = (int)(Globals.gameStats.NumWins * 100.0 / Globals.gameStats.NumGames);
                percentLosses = 100 - percentWins;
            }
            txtWinRate.Text = percentWins.ToString();
            txtLoseRate.Text = percentLosses.ToString();
        }

        /// <summary>
        /// Updates the hours played using Stopwatch
        /// </summary>
        private void UpdateHoursPlayed()
        {
            Globals.gameStopWatch.Stop();

            TimeSpan ts = Globals.gameStopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            txtHoursPlayed.Text = elapsedTime;
        }

        private async void btnUpdateNameClick(object sender, RoutedEventArgs e)
        {
            Globals.gameStats.PlayerName = txtPlayerName.Text;
            await DBUtility.SaveStats(Globals.gameStats);
        }

        /// <summary>
        /// back click handler
        /// </summary>
        private void btnStatsBackClick(object sender, RoutedEventArgs e)
        {
            PageMenu menuPage = new PageMenu();
            NavigationService.Navigate(menuPage);
        }

        /// <summary>
        /// reset button click handler
        /// </summary>
        private async void btnResetClick(object sender, RoutedEventArgs e)
        {
            Globals.gameStats.NumGames = 0;
            Globals.gameStats.NumWins = 0;
            Globals.gameStats.NumLosses = 0;

            await DBUtility.SaveStats(Globals.gameStats);

            txtTimesPlayed.Text = Globals.gameStats.NumGames.ToString();
            txtNumberOfWins.Text = Globals.gameStats.NumWins.ToString();
            txtNumberOfLosses.Text = Globals.gameStats.NumLosses.ToString();

            this.UpdateWinLossRates();

            Globals.gameStopWatch.Restart();
            this.UpdateHoursPlayed();
        }
    }

    // static class to hold num of games, wins, losses and the stopwatch as globals.
    public static class Globals
    {
        // global int
        public static Stats gameStats;

        public static Stopwatch gameStopWatch;
    }
}

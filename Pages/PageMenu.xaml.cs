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
        // Create a golden brush and black brush for background image border colours
        public SolidColorBrush goldBorder = new SolidColorBrush(Color.FromRgb(232, 193, 51));
        public SolidColorBrush blackBorder = new SolidColorBrush(Color.FromRgb(0, 0, 0));

        // The currently selected background image (1 = green, 2 = red, 3 = blue)
        public int currentBackground = ((MainWindow)Application.Current.MainWindow).currentBackground;

        // ImageSources for the 3 background options
        public ImageSource greenBackground = (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/Backgrounds/_BGGreen.png");
        public ImageSource redBackground = (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/Backgrounds/_BGRed.png");
        public ImageSource blueBackground = (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/Backgrounds/_BGBlue.png");


        public PageMenu()
        {
            InitializeComponent();
            HighlightBorder(currentBackground);
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

        /// <summary>
        /// Changes the background to Green
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BGGreenMouseDown(object sender, MouseButtonEventArgs e)
        {
            // If the background isn't green, set it to green
            if (currentBackground != 1)
            {
                SetBackground(1);
            }
        }

        /// <summary>
        /// Changes the background to Red
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BGRedMouseDown(object sender, MouseButtonEventArgs e)
        {
            // If the background isn't red, set it to red
            if (currentBackground != 2)
            {
                SetBackground(2);
            }
        }

        /// <summary>
        /// Changes the background to Blue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BGBlueMouseDown(object sender, MouseButtonEventArgs e)
        {
            // If the background isn't blue, set it to blue
            if (currentBackground != 3)
            {
                SetBackground(3);
            }
        }

        /// <summary>
        /// Sets the background colour to the chosen colour
        /// </summary>
        /// <param name="background"></param>
        public void SetBackground(int background)
        {
            // Switch the background to the selected one
            switch (background)
            {
                case 1:
                    // Set background to green
                    ((MainWindow)Application.Current.MainWindow).BackgroundImage.ImageSource = greenBackground;

                    // Highlight the green background preview image's border
                    HighlightBorder(1);

                    // Set the currentBackground
                    currentBackground = 1;
                    ((MainWindow)Application.Current.MainWindow).currentBackground = 1;
                    break;
                case 2:
                    ((MainWindow)Application.Current.MainWindow).BackgroundImage.ImageSource = redBackground;
                    HighlightBorder(2);
                    currentBackground = 2;
                    ((MainWindow)Application.Current.MainWindow).currentBackground = 2;
                    break;
                case 3:
                    ((MainWindow)Application.Current.MainWindow).BackgroundImage.ImageSource = blueBackground;
                    HighlightBorder(3);
                    currentBackground = 3;
                    ((MainWindow)Application.Current.MainWindow).currentBackground = 3;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Highlights the selected border with a gold colour, while setting
        /// all other borders to black
        /// </summary>
        /// <param name="border">The border to highlight</param>
        public void HighlightBorder(int border)
        {
            // Reset all borders to black
            BorderGreen.BorderBrush = blackBorder;
            BorderRed.BorderBrush = blackBorder;
            BorderBlue.BorderBrush = blackBorder;

            // Highlight the selected border
            switch (border)
            {
                case 1:
                    BorderGreen.BorderBrush = goldBorder;
                    break;
                case 2:
                    BorderRed.BorderBrush = goldBorder;
                    break;
                case 3:
                    BorderBlue.BorderBrush = goldBorder;
                    break;
                default:
                    break;
            }
        }
    }
}

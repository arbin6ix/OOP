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
    /// Interaction logic for PageGame.xaml
    /// </summary>
    public partial class PageGame : Page
    {
        public PageGame()
        {
            InitializeComponent();
        }

        private void btnGameBackClick(object sender, RoutedEventArgs e)
        {
            var c01 = (ImageSource)new ImageSourceConverter().ConvertFrom(@"../../../Images/c01.bmp");
            //p01.Source = (ImageSource)new ImageSourceConverter().ConvertFrom("/Images/c01.bmp");
            p01.Source = c01;
            p01.Visibility = Visibility.Visible;
            //PageMenu menuPage = new PageMenu();
            //NavigationService.Navigate(menuPage);
        }
    }
}

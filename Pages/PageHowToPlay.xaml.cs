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
    /// Interaction logic for PageHowToPlay.xaml
    /// </summary>
    public partial class PageHowToPlay : Page
    {
        public PageHowToPlay()
        {
            InitializeComponent();
        }

        private void btnHowToBackClick(object sender, RoutedEventArgs e)
        {
            PageMenu menuPage = new PageMenu();
            NavigationService.Navigate(menuPage);
        }
    }
}

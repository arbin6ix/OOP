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
using System.Windows.Shapes;

namespace OOP4200_Tarneeb.Pages
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        public LogWindow()
        {
            InitializeComponent();

            dataGrid1.ItemsSource = DBUtility.FetchAndReturnLogs();
        }

        private void btnClearLogClick(object sender, RoutedEventArgs e)
        {

            DBUtility.FetchAndClearLogs();

            dataGrid1.ItemsSource = DBUtility.FetchAndReturnLogs();

        }
    }
}

using Microsoft.EntityFrameworkCore;
using OOP4200_Tarneeb.DbContexts;
using OOP4200_Tarneeb.DTOs;
using OOP4200_Tarneeb.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // The current background colour
        public int currentBackground = 1;

        // The AI difficulty selected by user
        // 1 = Easy, 2 = Medium, 3 = Hard
        public int difficulty = 1;

        public MainWindow()
        {
            InitializeComponent();
            Main.Content = new PageMenu();
        }

    }
}

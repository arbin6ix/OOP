using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OOP4200_Tarneeb.DbContexts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OOP4200_Tarneeb
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;
        public static TarneebDbContextFactory tarneebDbContextFactory;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    tarneebDbContextFactory = new TarneebDbContextFactory("Data Source=tarneeb.db");
                    services.AddSingleton(tarneebDbContextFactory);
                })
                .Build();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _host.Start();

            try
            {
                using (TarneebDbContext dbContext = tarneebDbContextFactory.CreateDbContext())
                {
                    dbContext.Database.Migrate();
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }

            // Initialize the global stats during startup
            DBUtility.LoadStats();
            Globals.gameStopWatch = Stopwatch.StartNew();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host.Dispose();

            base.OnExit(e);
        }
    }
}

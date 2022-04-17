using Microsoft.EntityFrameworkCore;
using OOP4200_Tarneeb.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OOP4200_Tarneeb.DbContexts
{
    public class TarneebDbContext : DbContext
    {
        public TarneebDbContext(DbContextOptions options) : base(options) { }

        public DbSet<LogDTO> Logs { get; set; }

        public DbSet<StatsDTO> Stats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StatsDTO>().HasData(
                new StatsDTO
                {
                    Id = Guid.NewGuid(),
                    PlayerName = "Player 1",
                    NumGames = 0,
                    NumWins = 0,
                    NumLosses = 0
                }
            );
        }
    }
}

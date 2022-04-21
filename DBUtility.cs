using Microsoft.EntityFrameworkCore;
using OOP4200_Tarneeb.DbContexts;
using OOP4200_Tarneeb.DTOs;
using OOP4200_Tarneeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP4200_Tarneeb
{
    public class DBUtility
    {
        public static async void LoadStats()
        {
            using (TarneebDbContext context = App.tarneebDbContextFactory.CreateDbContext())
            {
                //await Task.Delay(3000);

                IEnumerable<StatsDTO> statsDTOs = await context.Stats.ToListAsync();

                Globals.gameStats = statsDTOs.Select(s => ToStats(s)).ToList().First();
            }
        }

        private static Stats ToStats(StatsDTO dto)
        {
            return new Stats(dto.Id, dto.PlayerName, dto.NumGames, dto.NumWins, dto.NumLosses);
        }

        public static async Task SaveStats(Stats stats)
        {
            using (TarneebDbContext context = App.tarneebDbContextFactory.CreateDbContext())
            {

                StatsDTO statsDTO = ToStatsDTO(stats);

                context.Stats.Update(statsDTO);
                await context.SaveChangesAsync();
            }
        }

        private static StatsDTO ToStatsDTO(Stats stats)
        {
            return new StatsDTO()
            {
                Id = stats.Id,
                PlayerName = stats.PlayerName,
                NumGames = stats.NumGames,
                NumWins = stats.NumWins,
                NumLosses = stats.NumLosses,
            };
        }

        public static async Task SaveLog(Log log)
        {
            using (TarneebDbContext context = App.tarneebDbContextFactory.CreateDbContext())
            {

                LogDTO logDTO = ToLogDTO(log);

                context.Logs.Add(logDTO);

                await context.SaveChangesAsync();
            }
        }



        private static LogDTO ToLogDTO(Log log)
        {
            return new LogDTO()
            {
                Id = log.Id,
                EventType = log.EventType,
                EventActor = log.EventActor,
                EventDetails = log.EventDetails,
                EventTime = log.EventTime,
            };
        }

        public static async void FetchAndPrintLogs()
        {
            using (TarneebDbContext context = App.tarneebDbContextFactory.CreateDbContext())
            {
                IEnumerable<LogDTO> logDTOs = await context.Logs.ToListAsync();

                Console.WriteLine(string.Join(System.Environment.NewLine, logDTOs.Select(log => ToLog(log)).ToList())); //need fixing.
            }
        }

        private static Log ToLog(LogDTO dto)
        {
            return new Log(dto.EventType, dto.EventActor, dto.EventDetails, dto.EventTime);
        }

        public static IEnumerable<LogDTO> FetchAndReturnLogs()
        {
            using (TarneebDbContext context = App.tarneebDbContextFactory.CreateDbContext())
            {
                IEnumerable<LogDTO> logDTOs = context.Logs.ToList();

                return logDTOs;

            }

        }
        public static void FetchAndClearLogs()
        {
            using (TarneebDbContext context = App.tarneebDbContextFactory.CreateDbContext())
            {

                foreach (var id in context.Logs.Select(e => e.Id))
                {
                    var entity = new LogDTO { Id = id };
                    //dbContext.MyEntities.Attach(entity);
                    context.Logs.Remove(entity);
                }
                context.SaveChanges();



            }

        }
    }
}

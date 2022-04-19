using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OOP4200_Tarneeb.DTOs
{
    public class StatsDTO
    {
        [Key]
        public Guid Id { get; set; }
        public string PlayerName { get; set; }
        public int NumGames { get; set; }
        public int NumWins { get; set; }
        public int NumLosses { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OOP4200_Tarneeb.DTOs
{
    public class LogDTO
    {
        [Key]
        public Guid Id { get; set; }
        public string EventType { get; set; }
        public string EventActor { get; set; }
        public string EventDetails { get; set; }
        public DateTime EventTime { get; set; }
    }
}

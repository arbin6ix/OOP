using System;
using System.Collections.Generic;
using System.Text;

namespace OOP4200_Tarneeb.Models
{
    public class Log
    {
        public Guid Id { get; }
        public string EventType { get; }
        public string EventActor { get; }
        public string EventDetails { get; }
        public DateTime EventTime { get; }

        public Log(string eventType, string eventActor, string eventDetails)
        {
            Id = Guid.NewGuid();
            EventType = eventType;
            EventActor = eventActor;
            EventDetails = eventDetails;
            EventTime = DateTime.Now;
        }

        public Log(string eventType, string eventActor, string eventDetails, DateTime eventTime)
        {
            Id = Guid.NewGuid();
            EventType = eventType;
            EventActor = eventActor;
            EventDetails = eventDetails;
            EventTime = eventTime;
        }
    }
}

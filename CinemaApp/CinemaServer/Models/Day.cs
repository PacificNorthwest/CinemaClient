using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CinemaServer.Models
{
    public class Day
    {
        public DateTime Date { get; private set; }
        public List<Session> Sessions { get; set; } = new List<Session>();

        public Day(DateTime date) { Date = date; }
        public Day() : this(DateTime.Now) { }
    }
}
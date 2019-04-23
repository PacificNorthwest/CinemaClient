using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CinemaServer.Models
{
    public class Session
    {
        public int ID { get; private set; }
        public string Time { get; private set; }
        public int Hall { get; private set; }
        public List<Seat> BookedSeats { get; set; } = new List<Seat>();

        public Session(int id, string time, int hall)
        {
            ID = id;
            Time = time;
            Hall = hall;
        }
    }
}
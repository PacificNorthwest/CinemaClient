using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CinemaServer.Models
{
    public class Seat
    {
        public int ID { get; set; }
        public int Hall { get; set; }
        public int Row { get; set; }
        public int Number { get; set; }

        public Seat(int id, int hall, int row, int number)
        {
            ID = id;
            Hall = hall;
            Row = row;
            Number = number;
        }
    }
}
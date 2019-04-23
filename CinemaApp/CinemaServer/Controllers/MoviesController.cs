using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CinemaServer.Models;
using CinemaServer.SQL;
using Newtonsoft.Json;

namespace CinemaServer.Controllers
{
    public class MoviesController : ApiController
    {
        [RequireHttps]
        public string Get(string token)
        {
            IEnumerable<Movie> movies = DBManager.LoadMovieList();
            IEnumerable<int> tickets = DBManager.LoadUserTickets(token);
            return JsonConvert.SerializeObject(new { Movies = movies.ToList(), Tickets = tickets.ToList() });
        }
    }
}

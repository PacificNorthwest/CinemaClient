using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CinemaApp.Model
{
    static class Schedule
    {
        public static List<Movie> Movies { get; set; } = new List<Movie>();
        public static List<int> BookedTickets { get; set; } = new List<int>();
        public static Movie GetMovieByTitle(string title) => Movies.Find(movie => movie.Title == title);
        public static Movie GetMovieByID(int id) => Movies.Find(movie => movie.ID == id);
    }
}
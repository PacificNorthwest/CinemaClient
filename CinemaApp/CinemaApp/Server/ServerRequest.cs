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
using CinemaApp.Model;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace CinemaApp.Server
{
    class ServerRequest
    {
        public static void LoadMovieList()
        {
            Schedule.Movies = JsonConvert.DeserializeObject<List<Movie>>(SendMovieListRequest());
        }

        public static void SignUp(string email, string hash, CardInfo card)
        {

        }

        private static string SendMovieListRequest()
        {
            var request = WebRequest.Create(@"http://cinemaserver.azurewebsites.net/api/movies");
            request.ContentType = "application/json";
            request.Method = "GET";
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string buffer = reader.ReadToEnd();
                        return buffer;
                    }
                }
                else
                    throw new Exception($"Connection Failed. Reason: {response.StatusCode}");
            }
        }
    }
}
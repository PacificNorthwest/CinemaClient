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
using RestSharp;

namespace CinemaApp.Server
{
    class ServerRequest
    {

        public static void LoadMovieList()
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
                        Schedule.Movies = JsonConvert.DeserializeObject<List<Movie>>(buffer);
                    }
                }
                else
                    throw new Exception($"Connection Failed. Reason: {response.StatusCode}");
            }
        }

        public static bool SignUpNewUser(string email, string serverKey, CardInfo card)
        {
            var client = new RestClient(@"https://cinemaserver.azurewebsites.net");
            var request = new RestRequest("api/accounts", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(JsonConvert.SerializeObject(new { Email = email,
                                                                  Hash = serverKey,
                                                                  CardNumber = card.Number,
                                                                  ExpDate = card.ExpDate,
                                                                  CVV = card.CVV }));

            IRestResponse response = client.Execute(request);
            var result = bool.Parse(JsonConvert.DeserializeObject<string>(response.Content));
            return result;          
        }

        public static bool BookSeats(string userToken, string appKey, string sessionId, int hall, IEnumerable<int> seats)
        {
            var client = new RestClient(@"https://cinemaserver.azurewebsites.net");
            var request = new RestRequest("api/booking", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(JsonConvert.SerializeObject(new { UserToken = userToken,
                                                                  AppKey = appKey,
                                                                  SessionId = sessionId,
                                                                  Hall = hall,
                                                                  Seats = seats }));

            IRestResponse response = client.Execute(request);
            var result = bool.Parse(JsonConvert.DeserializeObject<string>(response.Content));
            return result;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json.Linq;

namespace CinemaServer
{
    public class OMDbRequest
    {
        public static string GetRating(string movieTitle)
        {
            var response = SendRequest(movieTitle);
            JObject data = JObject.Parse(response);
            return data.GetValue("imdbRating").ToString();
        }

        public static string SendRequest(string movieTitle)
        {
            var request = WebRequest.Create($@"http://www.omdbapi.com/?t={movieTitle}");
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
                return @"{'imdbRating':'N/A'}";
            }
        }
    }
}
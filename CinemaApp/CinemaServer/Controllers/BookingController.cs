using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;

namespace CinemaServer.Controllers
{
    public class BookingController : ApiController
    {
        [RequireHttps]
        public string Post([FromBody]string value)
        {
            JObject message = JObject.Parse(value);
            List<object> userData = SQL.DBManager.GetUserData((string)message["UserToken"]);
            bool result = Security.SecurityProvider.ProcessPayment((string)message["AppKey"],
                                                                   (string)userData[1],
                                                                   (string)userData[2],
                                                                   (string)userData[3],
                                                                   (string)userData[4],
                                                                   ((JArray)message["Seats"]).Select(s => (int)s).ToList());

            if (result)
                foreach (int localId in ((JArray)message["Seats"]).Select(s => (int)s).ToList())
                    SQL.DBManager.InsertBooking(((int)userData[0]).ToString(),
                                                (string)message["SessionId"],
                                                ((((int)message["Hall"] - 1) * 80) + localId).ToString());
            return result.ToString();
        }

        [RequireHttps]
        public string Get(int id)
        {
            var result = SQL.DBManager.RedeemTicket(id);
            return result;
        }
    }
}

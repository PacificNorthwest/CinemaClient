using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CinemaServer.SQL;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace CinemaServer.Controllers
{
    public class AccountsController : ApiController
    {
        [RequireHttps]
        public string Post([FromBody]string value)
        {
                JObject message = JObject.Parse(value);
                bool result = DBManager.SignUpNewUser((string)message["Email"],
                                                      (string)message["Hash"],
                                                      (string)message["CardNumber"],
                                                      (string)message["ExpDate"],
                                                      (string)message["CVV"]);
                return result.ToString();
        }

        [RequireHttps]
        public string Get(string value)
        {
            var parameters = JsonConvert.DeserializeAnonymousType(value, new { Email = string.Empty, UserToken = string.Empty });
            bool result = DBManager.VerifyUser(parameters.Email, parameters.UserToken);
            return result.ToString();
        }
    }
}

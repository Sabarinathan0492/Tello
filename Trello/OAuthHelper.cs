using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Net;
using OAuth;
using System.Net.Http;

namespace Trello
{
    public static class OAuthHelper
    {
        private static string _appKey = ConfigurationManager.AppSettings["AppKey"];
        private static string _secret = ConfigurationManager.AppSettings["Secret"];
        private static string requestURL = ConfigurationManager.AppSettings["requestURL"];

        public static string Login()
        {
            OAuthRequest request = new OAuthRequest();
            request.ConsumerKey = _appKey;
            request.ConsumerSecret = _secret;
            request.Method = "GET";
            request.RequestUrl = requestURL;
            request.Version = "1.0";
            request.Type = OAuthRequestType.RequestToken;

            using (HttpClient client = new HttpClient())
            {
                var response = client.GetAsync(requestURL + "?" + request.GetAuthorizationQuery()).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }

    }
}

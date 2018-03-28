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
        private static string accessURL = ConfigurationManager.AppSettings["accessURL"];

        public static Dictionary<string, string> Login()
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
                Dictionary<string, string> returnValue = new Dictionary<string, string>();
                if (response.IsSuccessStatusCode)
                {
                    var queryParams = response.Content.ReadAsStringAsync().Result.Split('&');
                    foreach (var param in queryParams)
                        returnValue.Add(param.Split('=')[0], param.Split('=')[1]);
                }
                return returnValue;
            }
        }

        public static string GetAccessToken(string token, string tokenSecret, string verificationCode)
        {
            OAuthRequest request = new OAuthRequest();
            request.ConsumerKey = _appKey;
            request.ConsumerSecret = _secret;
            request.Token = token;
            request.TokenSecret = tokenSecret;
            request.Verifier = verificationCode;
            request.Method = "GET";
            request.RequestUrl = accessURL;
            request.Version = "1.0";
            request.Type = OAuthRequestType.AccessToken;

            using (HttpClient client = new HttpClient())
            {
                var response = client.GetAsync(accessURL + "?" + request.GetAuthorizationQuery()).Result;
                string returnValue = null;
                if (response.IsSuccessStatusCode)
                {
                    returnValue = response.Content.ReadAsStringAsync().Result;
                }
                return returnValue;
            }
        }

    }
}

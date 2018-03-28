using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Trello
{
    public static class ApiHelper
    {
        private static readonly string _apiUrl = ConfigurationManager.AppSettings["ApiUrl"];
        public static T Get<T>(string url) where T: new()
        {
            using (HttpClient client = new HttpClient())
            {
                var response = client.GetAsync($"{_apiUrl}{url}").Result;
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
                }
                throw new HttpRequestException($"Response doesn't indicate success when calling {url}");
            }
        }
    }
}

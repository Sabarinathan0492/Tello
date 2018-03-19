using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trello
{
    class Program
    {
        static void Main(string[] args)
        {
            var token = OAuthHelper.Login();
            var verificationCode = SeleniumHelper.AuthorizeTrello(token.FirstOrDefault(x => x.Key == "oauth_token").Value);
            OAuthHelper.GetAccessToken(token.FirstOrDefault(x => x.Key == "oauth_token").Value, token.FirstOrDefault(x => x.Key == "oauth_token_secret").Value, verificationCode);
            Console.ReadLine();
        }
    }
}

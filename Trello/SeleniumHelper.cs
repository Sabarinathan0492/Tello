using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Trello
{
    public static class SeleniumHelper
    {
        private static string _authorizeURL = ConfigurationManager.AppSettings["authorizeURL"];
        private static string _userName = ConfigurationManager.AppSettings["userName"];
        private static string _password = ConfigurationManager.AppSettings["password"];

        public static string AuthorizeTrello(string token)
        {
            IWebDriver webdriver = new ChromeDriver();
            webdriver.Url = _authorizeURL + "?oauth_token=" + token + "&name=TrelloApp";
            webdriver.Navigate();
            webdriver.FindElement(By.LinkText("Log in")).Click();
            webdriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            webdriver.FindElement(By.Id("user")).SendKeys(_userName);
            webdriver.FindElement(By.Id("password")).SendKeys(_password);
            webdriver.FindElement(By.Id("login")).Click();
            webdriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            webdriver.FindElement(By.ClassName("primary")).Click();
            webdriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            var verificationCode =  webdriver.FindElement(By.TagName("pre")).Text;
            webdriver.Quit();
            return verificationCode;
        }
    }
}

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
        private static string _appKey = ConfigurationManager.AppSettings["AppKey"];
        private static string _userName = ConfigurationManager.AppSettings["userName"];
        private static string _password = ConfigurationManager.AppSettings["password"];

        public static string AuthorizeTrello()
        {
            IWebDriver webdriver = new ChromeDriver();
            webdriver.Url = $"{_authorizeURL}?expiration=1day&name=MyPersonalToken&scope=read&response_type=token&key={_appKey}";
            webdriver.Navigate();
            webdriver.FindElement(By.LinkText("Log in")).Click();
            webdriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            webdriver.FindElement(By.Id("user")).SendKeys(_userName);
            webdriver.FindElement(By.Id("password")).SendKeys(_password);
            webdriver.FindElement(By.Id("login")).Click();
            webdriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            webdriver.FindElement(By.ClassName("primary")).Click();
            webdriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            var token =  webdriver.FindElement(By.TagName("pre")).Text.Trim();
            webdriver.Quit();
            return token;
        }
    }
}

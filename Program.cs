using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;
using System.Diagnostics;

namespace testSahibineden
{
    class Program
    {
        public static CookieContainer cookieContainer = new CookieContainer();
        public static IWebDriver driver = new FirefoxDriver(Directory.GetCurrentDirectory(), new FirefoxOptions{AcceptInsecureCertificates = true});
        static async Task Main(string[] args)
        {
            try
            {
                await StartTest();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task StartTest()
        {
            var urls = GetUrls();

            //Console.WriteLine(urls);

            int counter = 0;

            foreach (var url in urls)
            {
                try
                {
                    var response = await TryWithWebRequest(url);
                    if (IsThereProblem(response))
                    {
                        try
                        {
                            response = await TryWithSelenium(url);
                            if(IsThereProblem(response)) 
                                throw new Exception();
                            Console.WriteLine($"##{counter} resolved: {url}  {DateTime.Now}");
                            counter++;    
                        }
                        catch (System.Exception ex)
                        {
                            Console.WriteLine($"selenium faced problem : {ex.Message}"); 
                            await Task.Delay(TimeSpan.FromMinutes(5));
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{counter} resolved: {url}  {DateTime.Now}");
                        counter++;
                    }
                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.Delay(TimeSpan.FromMinutes(2));
                }
            }
        }

        private static async Task<string> TryWithSelenium(string url)
        {
            driver.Url = url;
            
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMinutes(2));

            Func< IWebDriver, bool> waitToLoad = new Func<IWebDriver, bool>((IWebDriver webDriver) =>
            {
                try
                {
                    webDriver.FindElement(by: By.Id("classifiedId"));
                }
                catch
                {
                    return false;
                }
                return true;
            });

            wait.Until(waitToLoad);

            // now we can update cookieContainer:

            var cookies = driver.Manage().Cookies.AllCookies;

            foreach (var cookie in cookies)
            {
                cookieContainer.Add(
                    new System.Net.Cookie(
                        cookie.Name
                        ,cookie.Value
                        ,cookie.Path
                        ,cookie.Domain)
                        );
            }

            return driver.PageSource.ToString();
        }

        private static bool IsThereProblem(string response)
        {
            if(!(response.Contains("classifiedId")))
                return false;

            if (response.Contains("Loading site please wait..."))
            {
                Console.WriteLine("stuck!");
                return true;
            }
            
            return false;
        }

        private static async Task<string> TryWithWebRequest(string url)
        {
                var request = (HttpWebRequest)WebRequest.Create(url);
                string responseString;
                request.Proxy = System.Net.WebRequest.DefaultWebProxy;
                request.Referer = url;
                request.Method = "GET";
                request.Accept = "*/*";
                request.UserAgent =
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
                request.CookieContainer = cookieContainer;
                using (var response = await request.GetResponseAsync())
                {
                    using (var responseStream = new StreamReader(response.GetResponseStream()))
                    {
                        responseString = responseStream.ReadToEnd();
                        responseStream.Close();
                    }

                    response.Close();
                }
                return responseString;
        }
        public static List<string> GetUrls()
        {
            List<string> results;
            
            var executionPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            var filePath = Path.Combine(executionPath, "..","..", "..", "urls.txt");
            
            string text = System.IO.File.ReadAllText(filePath);
            
            string pattern = @"Url Resolved:(.*/detail)";
            
            Regex re = new Regex(pattern);
            
            results = re.Matches(text).Select(l => l.Groups[1]?.ToString()).ToList();
            
            return results;
        }
    }
}

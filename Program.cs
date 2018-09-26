using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;

namespace testSahibineden
{
    class Program
    {
        static async Task Main(string[] args)
        {
            
            int counter = 0;
            
            string responseString;
            
            IWebDriver driver = new FirefoxDriver(Directory.GetCurrentDirectory(), new FirefoxOptions{AcceptInsecureCertificates = true});
            
            while(true)
            {
                await Task.Delay(3000);

                string url = "https://www.sahibinden.com/listing/emlak-konut-satilik-muteahitten-satilik-balcova-hisar-koru-evleri-3-plus1-daireler-515361311/detail";

                driver.Url = "https://www.sahibinden.com/listing/emlak-konut-satilik-muteahitten-satilik-balcova-hisar-koru-evleri-3-plus1-daireler-515361311/detail";
            
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMinutes(5));

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
                
                counter ++;

                Console.WriteLine(counter);

                var source = driver.PageSource;
                
                if (source.Contains("Loading site please wait..."))
                {
                    Console.WriteLine("stuck!");
                }
            }
    
            

            // while(true)
            // {
            
            //     var request = (HttpWebRequest)WebRequest.Create(url);

            //     request.Proxy = System.Net.WebRequest.DefaultWebProxy;
            //     request.Referer = url;
            //     request.Method = "GET";
            //     request.Accept = "*/*";
            //     request.UserAgent =
            //         "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
                
            //     using (var response = await request.GetResponseAsync())
            //     {
            //         using (var responseStream = new StreamReader(response.GetResponseStream()))
            //         {
            //             responseString = responseStream.ReadToEnd();
            //             responseStream.Close();
            //         }

            //         response.Close();
            //     }

            //     driver.Url = "https://www.sahibinden.com/listing/emlak-konut-satilik-muteahitten-satilik-balcova-hisar-koru-evleri-3-plus1-daireler-515361311/detail";        
            // }

            
            
            //driver.Navigate().GoToUrl("http://www.google.com/");


            // Console.WriteLine(source);
        }
    }
}

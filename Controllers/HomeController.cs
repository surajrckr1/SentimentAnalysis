using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sentiment_WebApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Sentiment_WebApp.Controllers
{
    public class HomeController : Controller
    {
        List<string> FirstSetOfStocks = null; List<string> SecondSetOfStocks = null;
        List<string> ThirdSetOfStocks = null; List<string> FourthSetOfStocks = null;
        List<string> FifthSetOfStocks = null;

        public HomeController()
        {
            FirstSetOfStocks = System.IO.File.ReadLines("stocknames.txt").Take(5).ToList();
            SecondSetOfStocks = System.IO.File.ReadLines("stocknames.txt").Skip(5).Take(5).ToList();
            ThirdSetOfStocks = System.IO.File.ReadLines("stocknames.txt").Skip(10).Take(5).ToList();
            FourthSetOfStocks = System.IO.File.ReadLines("stocknames.txt").Skip(15).Take(5).ToList();
            FifthSetOfStocks = System.IO.File.ReadLines("stocknames.txt").Skip(20).Take(5).ToList();
        }

        public async Task<IActionResult> AnalyzeSetOfStocks(int stockSetId)
        {
            List<string> SetOfStocks = stockSetId == 1 ? FirstSetOfStocks : stockSetId == 2 ? SecondSetOfStocks : stockSetId == 3 ? ThirdSetOfStocks : FourthSetOfStocks;
            Task<ScoreModel[]> scoreTask = SplitTaskOfStocks(SetOfStocks);
            List<Task<ScoreModel[]>> completeScore = new List<Task<ScoreModel[]>>();
            completeScore.Add(scoreTask);
            try
            {
                var result = await Task.WhenAll(completeScore);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult IndexAsync()
        {
            List<StockModel> liststock = new List<StockModel>()
            {
                new StockModel{StockName=FirstSetOfStocks},
                new StockModel{StockName=SecondSetOfStocks},
                new StockModel{StockName=ThirdSetOfStocks}
            };
            return View(liststock);
        }

        static async Task<ScoreModel[]> SplitTaskOfStocks(List<string> url)
        {
            ScoreModel[] result = null;
           
            Task<ScoreModel> task1 = PasreHTML(url[0], 500);
            Task<ScoreModel> task2 = PasreHTML(url[1], 500);
            Task<ScoreModel> task3 = PasreHTML(url[2], 500);
            Task<ScoreModel> task4 = PasreHTML(url[3], 500);
            Task<ScoreModel> task5 = PasreHTML(url[4], 500);

            List<Task<ScoreModel>> parentTask = new List<Task<ScoreModel>>();

            parentTask.Add(task1);
            parentTask.Add(task2);
            parentTask.Add(task3);
            parentTask.Add(task4);
            parentTask.Add(task5);

            try
            {
               result = await Task.WhenAll(parentTask);
               return result;
            }
            catch(Exception)
            {
                return null;
            }
        }

        static async Task<ScoreModel> PasreHTML(string Url, int WordCount)
        {
            try
            {
                HtmlWeb web = new HtmlWeb();
                var html = @"https://stocks.tickertape.in/" + Url + "?broker=kite&theme=default";
                var htmlDoc = web.Load(html);

                var node = htmlDoc.DocumentNode.SelectNodes("//script[@id='__NEXT_DATA__']").First().InnerText;

                //var SteamDetails = JsonConvert.DeserializeObject<dynamic>(node);
                JObject htmlContent = JObject.Parse(node);
                var newsLink = from p in htmlContent["props"]["pageProps"]["news"]
                                 select (string)p["link"];
                List<string> link = new List<string>();
                List<string> stockLinks = new List<string>();
                foreach (var item in newsLink)
                {
                    link.Add(item);
                }

                stockLinks.AddRange(link.Take(10));
                ScoreModel dat = await InitiateThread(stockLinks, 500, Url);
                return dat;
            }
            catch (Exception)
            {
                return null;
            }
           
        }

        static async Task<ScoreModel> InitiateThread(List<string> url, int wordCount, string stockName)
        {
            ScoreModel scoreModel = new ScoreModel();
            Task<ScoreModel> task1=null; Task<ScoreModel> task2=null; Task<ScoreModel> task3=null; Task<ScoreModel> task4=null; Task<ScoreModel> task5=null;
            Task<ScoreModel> task6=null; Task<ScoreModel> task7=null; Task<ScoreModel> task8=null; Task<ScoreModel> task9=null; Task<ScoreModel> task10=null;

            if (url.Count>=1)
                task1 = SentimentScore( url[0], wordCount);

            if (url.Count >= 2)
                task2 = SentimentScore(url[1], wordCount);

            if (url.Count >= 3)
                task3 = SentimentScore(url[2], wordCount);

            if (url.Count >= 4)
                task4 = SentimentScore(url[3], wordCount);

            if (url.Count >= 5)
                task5 = SentimentScore(url[4], wordCount);

            if (url.Count >= 6)
                task6 = SentimentScore(url[5], wordCount);

            if (url.Count >= 7)
                task7 = SentimentScore(url[6], wordCount);

            if (url.Count >= 8)
                task8 = SentimentScore(url[7], wordCount);

            if (url.Count >= 9)
                task9 = SentimentScore(url[8], wordCount);

            if (url.Count >= 10)
                task10 = SentimentScore(url[9], wordCount);

            List<Task<ScoreModel>> parentTask = new List<Task<ScoreModel>>();

            if (url.Count >= 1)
                parentTask.Add(task1);

            if (url.Count >= 2)
                parentTask.Add(task2);

            if (url.Count >= 3)
                parentTask.Add(task3);

            if (url.Count >= 4)
                parentTask.Add(task4);

            if (url.Count >= 5)
                parentTask.Add(task5);

            if (url.Count >= 6)
                parentTask.Add(task6);

            if (url.Count >= 7)
                parentTask.Add(task7);

            if (url.Count >= 8)
                parentTask.Add(task8);

            if (url.Count >= 9)
                parentTask.Add(task9);

            if (url.Count >= 10)
                parentTask.Add(task10);

            try
            {
                ScoreModel[] result = await Task.WhenAll(parentTask);
                int positiveWordCount = 0; int negativeWordCount = 0;
                foreach (var i in result)
                {
                    positiveWordCount = positiveWordCount + Convert.ToInt32(i.Positive);
                    negativeWordCount = negativeWordCount + Convert.ToInt32(i.Negative);
                }
                scoreModel.Positive = positiveWordCount.ToString();
                scoreModel.Negative = negativeWordCount.ToString();
                scoreModel.StockName = stockName;
                scoreModel.TotalWords = wordCount.ToString();

                return scoreModel;
            }
            catch (Exception)
            {
                return null;
            }
            
        }

       
        public  async static Task<ScoreModel> SentimentScore(string url, int wordCount)
        {
            try
            {
                var score = (dynamic)null;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:8093/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    StockDataModel dataModel = new StockDataModel() { WordCount = wordCount, StockUrl = url };
                    var response = await client.PostAsJsonAsync("api/Sentiment", dataModel);
                    var responseString = response.Content.ReadAsStringAsync();
                    string finalResult = responseString.Result;
                    score = Newtonsoft.Json.JsonConvert.DeserializeObject<ScoreModel>(finalResult);
                }
                return score;
            }
            catch(Exception)
            {
                return null;
            }
            
        }

    }
}

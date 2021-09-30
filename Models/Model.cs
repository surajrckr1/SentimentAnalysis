using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sentiment_WebApp.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public string StockName { get; set; }
    }
    public class StockLink
    {
        public string StockUrl { get; set; }
    }
    public class StockDataModel
    {
       public int WordCount { get; set; }
       public string StockUrl { get; set; }
    }
    public class StockModel
    {
        public List<string> StockName { get; set; }
        // Fill the missing properties for your data
    }
    public class ScoreModel
    {
        public string Positive { get; set; }
        public string Negative { get; set; }
        public string TotalWords { get; set; }
        public string StockName { get; set; }
    }
}

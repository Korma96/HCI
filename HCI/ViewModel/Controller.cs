using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HCI.ViewModel
{

    public class Controller
    {
        
        public DataFromNetwork downloadSerializedJsonData(string url)
        {
            using (WebClient w = new WebClient())
            {
                string jsonData = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    jsonData = w.DownloadString(url);
                }
                catch (Exception) { }

                // if string with JSON data is not empty, deserialize it to class and return its instance 
                if (!string.IsNullOrEmpty(jsonData))
                {
                    return JsonConvert.DeserializeObject<DataFromNetwork>(jsonData);
                }

                return null;
            }
        }
    }
    
    public class TimeSerie
    {
        [JsonProperty("1. open")]
        public double Open { get; set; }
        [JsonProperty("2. high")]
        public double High { get; set; }
        [JsonProperty("3. low")]
        public double Low { get; set; }
        [JsonProperty("4. close")]
        public double Close { get; set; }
        [JsonProperty("5. volume")]
        public double Volume { get; set; }

        public override string ToString()
        {
            return "{1. open : " + Open + ", 2. high : " + High + ", 3. low : " + Low + ", 4. close : " + Close + ", 5. volume : " + Volume + "}";
        }
    }

    public class MetaData
    {
        [JsonProperty("1. Information")]
        public string Information { get; set; }
        [JsonProperty("2. Symbol")]
        public string Symbol { get; set; }
        [JsonProperty("3. Last Refreshed")]
        public DateTime LastRefreshed { get; set; }
        [JsonProperty("4. Output Size")]
        public string OutputSize { get; set; }
        [JsonProperty("5. Time Zone")]
        public string TimeZone { get; set; }
    }

    public class DataFromNetwork
    {
        [JsonProperty("Meta Data")]
        public MetaData MData { get; set; }
        [JsonProperty("Time Series (Daily)")]
        public Dictionary<string, TimeSerie> TimeSeries { get; set; }
    }
    
}

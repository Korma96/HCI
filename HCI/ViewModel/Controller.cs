using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HCI.ViewModel
{

    public class Controller
    {

        private Dictionary<string, string> symbols;

        public Controller()
        {
            readSymbolsOfCompanies();
        }

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
                catch(ArgumentNullException ane) { MessageBox.Show(ane.StackTrace, "Greska - ArgumentNullException"); }
                catch (WebException we) { MessageBox.Show(we.StackTrace, "Greska - WebException"); }
                catch (NotSupportedException nse) { MessageBox.Show(nse.StackTrace, "Greska - NotSupportedException"); }
                catch (Exception e) { MessageBox.Show(e.StackTrace, "Greska - Exception"); }

                // if string with JSON data is not empty, deserialize it to class and return its instance 
                if (!string.IsNullOrEmpty(jsonData))
                {
                    return JsonConvert.DeserializeObject<DataFromNetwork>(jsonData);
                }

                return null;
            }
        }

        private void readSymbolsOfCompanies()
        {
            symbols = new Dictionary<string, string>();

            string[] values;

            string path = @"..\..\ViewModel\symbols.csv";

            using (StreamReader reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    values = line.Split(',');

                    symbols.Add(values[1].Trim(), values[0].Trim());


                }
            }
          
        }

        public bool existsSymbol(string symbol)
        {
            return symbols.ContainsKey(symbol);
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
        [JsonProperty("Time Series (Daily)", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<DateTime, TimeSerie> TimeSeries { get; set; }
        [JsonProperty("Weekly Time Series", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<DateTime, TimeSerie> TimeSeriesWeekly { set { TimeSeries = value; } }
        [JsonProperty("Monthly Time Series", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<DateTime, TimeSerie> TimeSeriesMonthly { set { TimeSeries = value; } }

        //Ovih par redova iznad rade sledece: bice pronadjen neki od sledecih "Time Series (Daily)", "Weekly Time Series", "Monthly Time Series"
        //Koji god da bude pronadjen, njegova vrednost bice stavljena u TimeSeries

        // TODO treba dodati i za intrady

    }

}

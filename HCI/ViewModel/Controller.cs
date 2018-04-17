using HCI.View;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HCI.ViewModel
{

    public class Controller
    {

        public Dictionary<string, string> shares;
        public Dictionary<string, string> cryptos;
        public Dictionary<string, string> currs;

        string sharesPath = @"..\..\Files\shares.csv";
        string cryptoPath = @"..\..\Files\crypto.csv";
        string currPath = @"..\..\Files\curr.csv";

    public Controller()
        {
            shares = readFromFile(sharesPath);
            cryptos = readFromFile(cryptoPath);
            currs = readFromFile(currPath);
        }

        public string downloadJsonData(string url)
        {
            string jsonData = string.Empty;

            using (WebClient wc = new WebClient())
            {
                // attempt to download JSON data as a string
                try
                {
                    jsonData = wc.DownloadString(url);

                }
                catch (ArgumentNullException ane) { MessageBox.Show(ane.StackTrace, "Greska - ArgumentNullException"); }
                catch (WebException we) { MessageBox.Show(we.StackTrace, "Greska - WebException"); }
                catch (NotSupportedException nse) { MessageBox.Show(nse.StackTrace, "Greska - NotSupportedException"); }
                catch (Exception e) { MessageBox.Show(e.StackTrace, "Greska - Exception"); }

            }

            return jsonData;
        }

        public DataFromNetwork downloadSerializedJsonDataForShares(string url)
        {
            string jsonData = downloadJsonData(url);
            
            // if string with JSON data is not empty, deserialize it to class and return its instance 
            if (!string.IsNullOrEmpty(jsonData))
            {
                return JsonConvert.DeserializeObject<DataFromNetwork>(jsonData);
            }

            return null;
            
        }

        public DataFromNetworkCrypto downloadSerializedJsonDataForCrypto(string url)
        {
            string jsonData = downloadJsonData(url);

            // if string with JSON data is not empty, deserialize it to class and return its instance 
            if (!string.IsNullOrEmpty(jsonData))
            {
                 return JsonConvert.DeserializeObject<DataFromNetworkCrypto>(jsonData);    
            }

            return null;

        }

        /*private void showWaitDialog()
        {
            if(waitDialog == null) { 
                waitDialog = new WaitDialog();
                waitDialog.ShowDialog();
            }
        }
        
        public void wc_DownloadProgressChanged(Object sender, DownloadProgressChangedEventArgs e)
        {
            MessageBox.Show(e.ProgressPercentage + "%", "Download");

        }

        public void wc_DownloadDataCompleted(Object sender, DownloadDataCompletedEventArgs e)
        {
            //waitDialog.Close();
        }*/

        private Dictionary<string, string> readFromFile(string path)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string[] values;

            using (StreamReader reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    values = line.Split(',');

                    try { 
                    dict.Add(values[1].Trim(), values[0].Trim());
                    }
                    catch (Exception e) { MessageBox.Show(values[0].Trim() + " - " + values[1].Trim(), "Duplikati"); }

                }
            }

            return dict;
          
        }

        public bool existsShare(string share)
        {
            return shares.ContainsKey(share);
        }

        public bool existsCrypto(string crypto)
        {
            return cryptos.ContainsKey(crypto);
        }

        public bool existsCurr(string curr)
        {
            return currs.ContainsKey(curr);
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

    /*public class MetaData
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
    }*/

    public class DataFromNetwork
    {
        [JsonProperty("Meta Data")]
        public Dictionary<string, string> MetaData { get; set; }
        [JsonProperty("Time Series (Daily)", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<DateTime, TimeSerie> TimeSeries { get; set; }
        [JsonProperty("Weekly Time Series", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<DateTime, TimeSerie> TimeSeriesWeekly { set { TimeSeries = value; } }
        [JsonProperty("Monthly Time Series", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<DateTime, TimeSerie> TimeSeriesMonthly { set { TimeSeries = value; } }
        
        [JsonProperty("Time Series (1min)", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<DateTime, TimeSerie> TimeSeries1min { set { TimeSeries = value; } }
        [JsonProperty("Time Series (5min)", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<DateTime, TimeSerie> TimeSeries5min { set { TimeSeries = value; } }
        [JsonProperty("Time Series (15min)", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<DateTime, TimeSerie> TimeSeries15min { set { TimeSeries = value; } }
        [JsonProperty("Time Series (30min)", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<DateTime, TimeSerie> TimeSeries30min { set { TimeSeries = value; } }
        [JsonProperty("Time Series (60min)", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<DateTime, TimeSerie> TimeSeries60min { set { TimeSeries = value; } }


        //Ovih par redova iznad rade sledece: bice pronadjen neki od sledecih "Time Series (Daily)", "Weekly Time Series", "Monthly Time Series"
        //Koji god da bude pronadjen, njegova vrednost bice stavljena u TimeSeries

    }

    public class DataFromNetworkCrypto
    {
        [JsonProperty("Meta Data")]
        public Dictionary<string, string> MetaData { get; set; }

        [JsonProperty("Time Series (Digital Currency Intraday)", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<DateTime, Dictionary<string, string>> TimeSeries { get;  set; }
        [JsonProperty("Time Series (Digital Currency Daily)", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<DateTime, Dictionary<string, string>> TimeSeriesDaily { set { TimeSeries = value; } }
        [JsonProperty("Time Series (Digital Currency Weekly)", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<DateTime, Dictionary<string, string>> TimeSeriesWeekly { set { TimeSeries = value; } }
        [JsonProperty("Time Series (Digital Currency Monthly)", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<DateTime, Dictionary<string, string>> TimeSeriesMonthly { set { TimeSeries = value; } }

    }

}

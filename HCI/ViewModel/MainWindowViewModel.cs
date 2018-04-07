using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using HCI.Constants;

namespace HCI.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        string url = @"https://www.alphavantage.co/query?";
        public Controller Con { get; set; }
        Dictionary<string, TimeSerie> TimeSeriesData { get; set; }

        public MainWindowViewModel()
        {
            Con = new Controller();
            FormatData();
        }

        private void FormatData()
        {
            CreateUrl(TimeSeriesType.DAILY, "MSFT");
            TimeSeriesData = GetData(url);
            if (TimeSeriesData != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Time Series (Daily): { ");
                foreach (KeyValuePair<string, TimeSerie> kv in TimeSeriesData)
                {
                    sb.Append(kv.Key);
                    sb.Append(" : ");
                    sb.Append(kv.Value);
                }
                sb.Append("}");
                MessageBox.Show(sb.ToString(), "Uspesno odradjeno");
            }
            else
            {
                MessageBox.Show("Dictionary<string, TimeSerie> data = null;", "Greska");
            }
        }

        private void CreateUrl(TimeSeriesType timeSeriesType, string symbol)
        {
            string functionString = "function=TIME_SERIES_" + timeSeriesType.ToString();
            string symbolString = "symbol=" + symbol;
            string apiKeyString = "apikey=demo";
            StringBuilder sb = new StringBuilder();
            sb.Append(functionString);
            sb.Append("&");
            sb.Append(symbolString);
            sb.Append("&");
            sb.Append(apiKeyString);

            url += sb.ToString();
        }

        Dictionary<string, TimeSerie> GetData(string url)
        {
            DataFromNetwork dfn = Con.downloadSerializedJsonData(url);
            return dfn.TimeSeries;
        }
    }
}
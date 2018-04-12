using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using HCI.Constants;
using System;
using System.IO;
using OxyPlot;
using OxyPlot.Axes;

namespace HCI.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private const string firstPartOfUrl = @"https://www.alphavantage.co/query?";
        private string url;
        private string apiKey = "3GCFS7H570T93Y7M";
        public Controller Con { get; set; }
        //public Dictionary<DateTime, TimeSerie> TimeSeriesData { get; set; }
        public Dictionary<string, string> Symbols { get; set; }
        public Dictionary<string, Dictionary<DateTime,TimeSerie>> AllTimeSeries { get; set; }

        public MainWindowViewModel(Controller con)
        {
            this.Con = con;
            AllTimeSeries = new Dictionary<string, Dictionary<DateTime, TimeSerie>>();
            //FormatData();
        }

        /*private void FormatData()
        {
            CreateUrl(TimeSeriesType.DAILY, "MSFT", apiKey);
            TimeSeriesData = ReadData(url);
            if (TimeSeriesData != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Time Series (Daily): { ");
                foreach (KeyValuePair<DateTime, TimeSerie> kv in TimeSeriesData)
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
        }*/

        private void CreateUrl(string timeSeriesType, string symbol, string apiKey)
        {
            string functionString = "function=TIME_SERIES_" + timeSeriesType;
            string symbolString = "symbol=" + symbol;
            string apiKeyString = "apikey=" + apiKey;
            StringBuilder sb = new StringBuilder();
            sb.Append(firstPartOfUrl);
            sb.Append(functionString);
            sb.Append("&");
            sb.Append(symbolString);
            sb.Append("&");
            sb.Append(apiKeyString);

            url =  sb.ToString();
        }

        public List<DataPoint> getSpecificData(string symbol, string tsType, string priceType)
        {
            List<DataPoint> dataPoints = null;

            Dictionary<DateTime, TimeSerie> data = getData(symbol, tsType);

            if (data == null) return null;

            switch (priceType)
            {
                case "open":
                    dataPoints = getOpens(data);
                    break;
                case "high":
                    dataPoints = getHighs(data);
                    break;
                case "low":
                    dataPoints = getLows(data);
                    break;
                case "close":
                    dataPoints = getCloses(data);
                    break;
                case "volume":
                    dataPoints = getVolumes(data);
                    break;
                default: break;
            }

            return dataPoints;
        }

        public List<DataPoint>[] getDataForOpenHighLowClose(string symbol, string tsType)
        {
            List<DataPoint>[] dataPoints = new List<DataPoint>[4];
            for (int i = 0; i < dataPoints.Length; i++) dataPoints[i] = new List<DataPoint>();

            Dictionary<DateTime, TimeSerie> data = getData(symbol, tsType);

            if (data == null) return null;

            DataPoint dp;
            foreach (KeyValuePair<DateTime, TimeSerie> kv in data)
            {
                dp = DateTimeAxis.CreateDataPoint(kv.Key, kv.Value.Open);
                dataPoints[0].Add(dp);
                dp = DateTimeAxis.CreateDataPoint(kv.Key, kv.Value.High);
                dataPoints[1].Add(dp);
                dp = DateTimeAxis.CreateDataPoint(kv.Key, kv.Value.Low);
                dataPoints[2].Add(dp);
                dp = DateTimeAxis.CreateDataPoint(kv.Key, kv.Value.Close);
                dataPoints[3].Add(dp);
            }

            return dataPoints;
        }

        private List<DataPoint>  getOpens(Dictionary<DateTime, TimeSerie> data)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            DataPoint dp;
            foreach (KeyValuePair<DateTime, TimeSerie> kv in data)
            {
                dp = DateTimeAxis.CreateDataPoint(kv.Key, kv.Value.Open);
                dataPoints.Add(dp);
            }

            return dataPoints;
        }

        private List<DataPoint> getHighs(Dictionary<DateTime, TimeSerie> data)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            DataPoint dp;
            foreach (KeyValuePair<DateTime, TimeSerie> kv in data)
            {
                dp = DateTimeAxis.CreateDataPoint(kv.Key, kv.Value.High);
                dataPoints.Add(dp);
            }

            return dataPoints;
        }

        private List<DataPoint> getLows(Dictionary<DateTime, TimeSerie> data)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            DataPoint dp;
            foreach (KeyValuePair<DateTime, TimeSerie> kv in data)
            {
                dp = DateTimeAxis.CreateDataPoint(kv.Key, kv.Value.Low);
                dataPoints.Add(dp);
            }

            return dataPoints;
        }

        private List<DataPoint> getCloses(Dictionary<DateTime, TimeSerie> data)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            DataPoint dp;
            foreach (KeyValuePair<DateTime, TimeSerie> kv in data)
            {
                dp = DateTimeAxis.CreateDataPoint(kv.Key, kv.Value.Close);
                dataPoints.Add(dp);
            }

            return dataPoints;
        }

        private List<DataPoint> getVolumes(Dictionary<DateTime, TimeSerie> data)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            DataPoint dp;
            foreach (KeyValuePair<DateTime, TimeSerie> kv in data)
            {
                dp = DateTimeAxis.CreateDataPoint(kv.Key, kv.Value.Volume);
                dataPoints.Add(dp);
            }

            return dataPoints;
        }

        public Dictionary<DateTime, TimeSerie> getData(string symbol, string tsType)
        {
            CreateUrl(tsType, symbol, apiKey);

            if(AllTimeSeries.ContainsKey(url))
            {
                return AllTimeSeries[url];
            }

            Dictionary<DateTime, TimeSerie> timeSeries = readData(url);
            if (timeSeries != null) AllTimeSeries.Add(url, timeSeries);

            return timeSeries;
        }

        Dictionary<DateTime, TimeSerie> readData(string url)
        {
            DataFromNetwork dfn = Con.downloadSerializedJsonData(url);
            if(dfn != null)
            {
                return dfn.TimeSeries;
            }

            return null;
            
        }

    }
}
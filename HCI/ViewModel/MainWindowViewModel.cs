using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using HCI.Constants;
using System;
using System.IO;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using System.Threading;

namespace HCI.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private const string firstPartOfUrl = @"https://www.alphavantage.co/query?";
        private string[] timeSeriesTypes = {"INTRADAY", "DAILY", "WEEKLY", "MONTHLY"};
        private string[] intervalTypes = { "1min", "5min", "15min", "30min", "60min" };

        private string apiKey = "3GCFS7H570T93Y7M";
        public Controller Con { get; set; }
      
        public Dictionary<string, Dictionary<DateTime,TimeSerie>> AllTimeSeriesForShares { get; set; }
        public Dictionary<string, Dictionary<DateTime, Dictionary<string, string>>> AllTimeSeriesForCrypto { get; set; }

        public MainWindowViewModel(Controller con)
        {
            this.Con = con;
            AllTimeSeriesForShares = new Dictionary<string, Dictionary<DateTime, TimeSerie>>();
            AllTimeSeriesForCrypto = new Dictionary<string, Dictionary<DateTime, Dictionary<string, string>>>();
        }

        private string CreateUrl(string timeSeriesType, string symbol, string interval, string market, string apiKey)
        {
            string function;
            if (!market.Equals(""))
            {
                function = "DIGITAL_CURRENCY_";
            }
            else
            {
                function = "TIME_SERIES_";
            }
                string functionString = "function=" + function + timeSeriesType;
            string symbolString = "symbol=" + symbol;
            string marketString = "market=" + market;
            string intervalString = "interval=" + interval;
            string apiKeyString = "apikey=" + apiKey;
            StringBuilder sb = new StringBuilder();
            sb.Append(firstPartOfUrl);
            sb.Append(functionString);
            sb.Append("&");
            sb.Append(symbolString);
            if (!interval.Equals(""))
            {
                sb.Append("&");
                sb.Append(intervalString);
            }

            if (!market.Equals(""))
            {
                sb.Append("&");
                sb.Append(marketString);
            }
            sb.Append("&");
            sb.Append(apiKeyString);

            string url =  sb.ToString();

            return url;
        }

        public List<DataPoint> getSpecificData(string symbol, string tsType, string priceType, string interval, string market)
        {
            List<DataPoint> dataPoints = null;
            Dictionary<DateTime, TimeSerie> dataShares = null;
            Dictionary<DateTime, Dictionary<string, string>> dataCrypto = null;

            if (market.Equals(""))
            {
                dataShares = getDataForShares(symbol, tsType, interval);
                if (dataShares == null) return null;
            }
            else
            {
                dataCrypto = getDataForCrypto(symbol, tsType, market);
                if (dataCrypto == null) return null;
            }

            switch (priceType)
            {
                case "open":
                    dataPoints = getOpens(dataShares);
                    break;
                case "high":
                    dataPoints = getHighs(dataShares);
                    break;
                case "low":
                    dataPoints = getLows(dataShares);
                    break;
                case "close":
                    dataPoints = getCloses(dataShares);
                    break;
                case "volume":
                    dataPoints = getVolumes(dataShares);
                    break;
                case "price crypto":
                    dataPoints = getPrices(dataCrypto);
                    break;
                case "price usd crypto":
                    dataPoints = getUsd(dataCrypto);
                    break;
                case "volume crypto":
                    dataPoints = getVolumesForCrypto(dataCrypto);
                    break;
                case "market cap usd crypto":
                    dataPoints = getMarketCap(dataCrypto);
                    break;
                case "open crypto":
                    dataPoints = getOpenCrypto(dataCrypto);
                    break;
                case "open usd crypto":
                    dataPoints = getOpenUsdCrypto(dataCrypto);
                    break;
                case "high crypto":
                    dataPoints = getHighCrypto(dataCrypto);
                    break;
                case "high usd crypto":
                    dataPoints = getHighUsdCrypto(dataCrypto);
                    break;
                case "low crypto":
                    dataPoints = getLowCrypto(dataCrypto);
                    break;
                case "low usd crypto":
                    dataPoints = getLowUsdCrypto(dataCrypto);
                    break;
                case "close crypto":
                    dataPoints = getCloseCrypto(dataCrypto);
                    break;
                case "close usd crypto":
                    dataPoints = getCloseUsdCrypto(dataCrypto);
                    break;
                default: break;
            }

            return dataPoints;
        }

        private List<DataPoint> getCloseCrypto(Dictionary<DateTime, Dictionary<string, string>> dataCrypto)
        {
            string partOfKey = "4a. close";
            List<DataPoint> dataPoints = getDataPoints(dataCrypto, partOfKey);

            return dataPoints;
        }

        private List<DataPoint> getCloseUsdCrypto(Dictionary<DateTime, Dictionary<string, string>> dataCrypto)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            DataPoint dp;
            foreach (KeyValuePair<DateTime, Dictionary<string, string>> kv in dataCrypto)
            {

                try
                {
                    dp = DateTimeAxis.CreateDataPoint(kv.Key, Double.Parse(kv.Value["4b. close (USD)"]));
                    dataPoints.Add(dp);
                }
                catch { }

            }

            return dataPoints;
        }

        private List<DataPoint> getLowCrypto(Dictionary<DateTime, Dictionary<string, string>> dataCrypto)
        {
            string partOfKey = "3a. low";
            List<DataPoint> dataPoints = getDataPoints(dataCrypto, partOfKey);

            return dataPoints;
        }

        private List<DataPoint> getLowUsdCrypto(Dictionary<DateTime, Dictionary<string, string>> dataCrypto)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            DataPoint dp;
            foreach (KeyValuePair<DateTime, Dictionary<string, string>> kv in dataCrypto)
            {

                try
                {
                    dp = DateTimeAxis.CreateDataPoint(kv.Key, Double.Parse(kv.Value["3b. low (USD)"]));
                    dataPoints.Add(dp);
                }
                catch { }

            }

            return dataPoints;
        }

        private List<DataPoint> getHighCrypto(Dictionary<DateTime, Dictionary<string, string>> dataCrypto)
        {
            string partOfKey = "2a. high";
            List<DataPoint> dataPoints = getDataPoints(dataCrypto, partOfKey);

            return dataPoints;
        }

        private List<DataPoint> getHighUsdCrypto(Dictionary<DateTime, Dictionary<string, string>> dataCrypto)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            DataPoint dp;
            foreach (KeyValuePair<DateTime, Dictionary<string, string>> kv in dataCrypto)
            {

                try
                {
                    dp = DateTimeAxis.CreateDataPoint(kv.Key, Double.Parse(kv.Value["2b. high (USD)"]));
                    dataPoints.Add(dp);
                }
                catch { }

            }

            return dataPoints;
        }

        private List<DataPoint> getOpenUsdCrypto(Dictionary<DateTime, Dictionary<string, string>> dataCrypto)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            DataPoint dp;
            foreach (KeyValuePair<DateTime, Dictionary<string, string>> kv in dataCrypto)
            {

                try
                {
                    dp = DateTimeAxis.CreateDataPoint(kv.Key, Double.Parse(kv.Value["1b. open (USD)"]));
                    dataPoints.Add(dp);
                }
                catch { }

            }

            return dataPoints;
        }

        private List<DataPoint> getOpenCrypto(Dictionary<DateTime, Dictionary<string, string>> dataCrypto)
        {
            string partOfKey = "1a. open";
            List<DataPoint> dataPoints = getDataPoints(dataCrypto, partOfKey);

            return dataPoints;
        }

        private List<DataPoint> getDataPoints(Dictionary<DateTime, Dictionary<string, string>> dataCrypto, string partOfKey)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            DataPoint dp;
            foreach (KeyValuePair<DateTime, Dictionary<string, string>> kv1 in dataCrypto)
            {
                foreach (KeyValuePair<string, string> kv2 in kv1.Value)
                {
                    if (kv2.Key.Contains(partOfKey))
                    {
                        try
                        {
                            dp = DateTimeAxis.CreateDataPoint(kv1.Key, Double.Parse(kv2.Value));
                            dataPoints.Add(dp);
                        }
                        catch { }

                        break;
                    }
                }

            }

            return dataPoints;
        }

        private List<DataPoint> getMarketCap(Dictionary<DateTime, Dictionary<string, string>> dataCrypto)
        {
            string partOfKey = "market cap (USD)";
            List<DataPoint> dataPoints = getDataPoints(dataCrypto, partOfKey);
            
            return dataPoints;
        }

        private List<DataPoint> getVolumesForCrypto(Dictionary<DateTime, Dictionary<string, string>> dataCrypto)
        {
            string partOfKey = "volume";
            List<DataPoint> dataPoints = getDataPoints(dataCrypto, partOfKey);

            return dataPoints;
        }

        private List<DataPoint> getUsd(Dictionary<DateTime, Dictionary<string, string>> dataCrypto)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            DataPoint dp;
            foreach (KeyValuePair<DateTime, Dictionary<string, string>> kv in dataCrypto)
            {
               
                try
                {
                    dp = DateTimeAxis.CreateDataPoint(kv.Key, Double.Parse(kv.Value["1b. price (USD)"]));
                    dataPoints.Add(dp);
                }
                catch { }

            }

            return dataPoints;
        }

        private List<DataPoint> getPrices(Dictionary<DateTime, Dictionary<string, string>> dataCrypto)
        {
            string partOfKey = "1a. price";
            List<DataPoint> dataPoints = getDataPoints(dataCrypto, partOfKey);

            return dataPoints;
        }

        public List<DataPoint>[] getDataForOpenHighLowClose(string symbol, string tsType, string interval)
        {
            List<DataPoint>[] dataPoints = new List<DataPoint>[4];
            for (int i = 0; i < dataPoints.Length; i++) dataPoints[i] = new List<DataPoint>();

            Dictionary<DateTime, TimeSerie> data = getDataForShares(symbol, tsType, interval);

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

        private void getAnotherCryptoTypes(object obj)
        {
            string[] parameters = (string[])obj;
            string symbol = parameters[0];
            string tsType = parameters[1];
            string market = parameters[2];

            List<string> timeSeriesTypesList = timeSeriesTypes.ToList();
            timeSeriesTypesList.Remove(tsType);

            string url;

            foreach (string tst in timeSeriesTypesList)
            {
                
                    url = CreateUrl(tst, symbol, "", market, apiKey);

                    try
                    {
                        if (!AllTimeSeriesForCrypto.ContainsKey(url))
                        {
                            Dictionary<DateTime, Dictionary<string, string>> timeSeries = readDataForCrypto(url);
                            if (timeSeries != null) AllTimeSeriesForCrypto.Add(url, timeSeries);
                        }
                    }
                    catch { }

            }


        }

        private void getAnotherTimeSeriesTypes(object obj)
        {
            string[] parameters = (string[])obj;
            string symbol = parameters[0];
            string tsType = parameters[1];
            string interval = parameters[2];
            string market = "";

            List<string> timeSeriesTypesList = timeSeriesTypes.ToList();
            timeSeriesTypesList.Remove(tsType);

            List<string> intervalTypesList = intervalTypes.ToList();
            if (!interval.Equals(""))
            {
                intervalTypesList.Remove(interval);
            }

            string url;

            foreach (string tst in timeSeriesTypesList)
            {
                if(tst.Equals("INTRADAY"))
                {
                    foreach (string it in intervalTypesList)
                    {
                        url = CreateUrl(tst, symbol, it, market, apiKey);

                        try
                        {
                            if (!AllTimeSeriesForShares.ContainsKey(url))
                            {   
                                Dictionary<DateTime, TimeSerie> timeSeries = readDataForShares(url);
                                if (timeSeries != null) AllTimeSeriesForShares.Add(url, timeSeries);
                            
                            }
                        }
                        catch { }
                    }
                }
                else
                {
                    url = CreateUrl(tst, symbol, interval, market, apiKey);

                    try { 
                        if (!AllTimeSeriesForShares.ContainsKey(url))
                        {
                            Dictionary<DateTime, TimeSerie> timeSeries = readDataForShares(url);
                            if (timeSeries != null) AllTimeSeriesForShares.Add(url, timeSeries);
                        }
                    }
                    catch { }

                }

            }
            
            
        }

        public Dictionary<DateTime, TimeSerie> getDataForShares(string symbol, string tsType, string interval)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(getAnotherTimeSeriesTypes));
            thread.Start(new string[] {symbol, tsType, interval});

            string url = CreateUrl(tsType, symbol, interval, "", apiKey);

            if(AllTimeSeriesForShares.ContainsKey(url))
            {
                return AllTimeSeriesForShares[url];
            }

            Dictionary<DateTime, TimeSerie> timeSeries = readDataForShares(url);
            try { 
                if (timeSeries != null) AllTimeSeriesForShares.Add(url, timeSeries);
            }
            catch { }

            return timeSeries;
        }

        public Dictionary<DateTime, Dictionary<string, string>> getDataForCrypto(string symbol, string tsType, string market)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(getAnotherCryptoTypes));
            thread.Start(new string[] { symbol, tsType, market });

            string url = CreateUrl(tsType, symbol, "", market, apiKey);

            if (AllTimeSeriesForCrypto.ContainsKey(url))
            {
                return AllTimeSeriesForCrypto[url];
            }

            Dictionary<DateTime, Dictionary<string, string>> timeSeries = readDataForCrypto(url);
            try
            {
                if (timeSeries != null) AllTimeSeriesForCrypto.Add(url, timeSeries);
            }
            catch { }

            return timeSeries;
        }

        Dictionary<DateTime, TimeSerie> readDataForShares(string url)
        {
            DataFromNetwork dfn = Con.downloadSerializedJsonDataForShares(url);
            if(dfn != null)
            {
                return dfn.TimeSeries;
            }

            return null;
            
        }

        Dictionary<DateTime, Dictionary<string, string>> readDataForCrypto(string url)
        {
            DataFromNetworkCrypto dfn = Con.downloadSerializedJsonDataForCrypto(url);
            if (dfn != null)
            {
                return dfn.TimeSeries;
            }

            return null;

        }

    }
}
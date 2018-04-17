using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HCI.ViewModel;
using OxyPlot.Series;
using OxyPlot;
using OxyPlot.Axes;
using HCI.Constants;
using System.Collections.ObjectModel;
using HCI.View;
using System.Windows.Controls;
using HCI.Table;
using System.Windows.Data;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using HCI.Model;
using System.Windows.Threading;
using System.Threading;

namespace HCI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WaitDialogMarko wdm = null;

        public GraphicViewModel Gvm { get; set; }
        MainWindowViewModel Mwvm { get; set; }
        private Controller con;

        private string currentSelectedForShares;
        private string currentSelectedForCrypto;

        private string[] contentsForRadioButtonsIntraday = { "Price", "Price USD", "Volume", "Market cap USD" };
        private string[] namesForRadioButtonsIntraday = { "rbPrice", "rbPriceUSD", "rbVolumeCrypto", "rbMarketCap" };
        private string[] contentsForRadioButtonsAnother = { "Open", "Open USD", "High", "High USD", "Low", "Low USD", "Close", "Close USD", "Volume", "Market cap USD" };
        private string[] namesForRadioButtonsAnother = { "rbPrice", "rbPriceUSD" };

        public MainWindow()
        {
            InitializeComponent();
            this.Closing += new System.ComponentModel.CancelEventHandler(Window_Closing);
            this.Loaded += Window_Loaded;

            WindowState = WindowState.Maximized;

            con = new Controller();
            Gvm = new GraphicViewModel(con, "");
            Mwvm = new MainWindowViewModel(con);

            this.DataContext = Gvm;

            currentSelectedForShares = "open";
            currentSelectedForCrypto = "open";

            titleOfSeries.AddData(con, FileType.SHARE);
            nameOfCurrency.AddData(con, FileType.CURRENCY);
            nameOfCryptoCurrency.AddData(con, FileType.CRYPTO_CURRENCY);

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            dispatcherTimer.Start();

        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;

            DataFromNetwork dfn;
            DataFromNetworkCrypto dfnc;

            if (contentOfTimeSeriesComboBox.Equals("INTRADAY"))
            {
                string[] keysForShares = Mwvm.AllTimeSeriesForShares.Keys.ToArray();
                foreach (string url in keysForShares)
                {
                    if(url.Contains("INTRADAY"))
                    {
                        try
                        {
                            dfn = con.downloadSerializedJsonDataForShares(url);
                            if (dfn != null)
                            {
                                if (dfn.TimeSeries != null) Mwvm.AllTimeSeriesForShares[url] = dfn.TimeSeries;
                            }
                        }
                        catch { }
                        
                    }
                        
                    
                }

                string[] keysForCrypto = Mwvm.AllTimeSeriesForCrypto.Keys.ToArray();
                foreach (string url in keysForCrypto)
                {
                    if (url.Contains("INTRADAY"))
                    {
                        try
                        {
                            dfnc = con.downloadSerializedJsonDataForCrypto(url);
                            if (dfnc != null)
                            {
                                if (dfnc.TimeSeries != null) Mwvm.AllTimeSeriesForCrypto[url] = dfnc.TimeSeries;
                            }
                        }
                        catch { }
                           
                    }

                }

                iscrtajIspocetka(contentOfTimeSeriesComboBox);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> potencijalneGreske = new List<string>();

            Thread newWindowThread = new Thread(new ThreadStart(showWaitDialog));
            newWindowThread.SetApartmentState(ApartmentState.STA);
            newWindowThread.IsBackground = true;
            newWindowThread.Start();

            Workspace ws = loadWorkspace();
            if (ws == null) return;

            currentSelectedForShares = ws.CurrentSelectedForShares;
            currentSelectedForCrypto = ws.CurrentSelectedForCrypto;

            selektujOdgovarajuciRadioButtonForShares();

            if (ws.TimeSeriesType.Equals("INTRADAY"))
            {
                addRadioButtons(contentsForRadioButtonsIntraday, currentSelectedForCrypto);
            }
            else
            {
                addRadioButtons(contentsForRadioButtonsAnother, currentSelectedForCrypto);
            }

            TimeSeriesTypeComboBox.Text = ws.TimeSeriesType;
            if (!ws.Interval.Equals(""))
            {
                IntervalComboBox.Visibility = Visibility.Visible;
                IntervalComboBox.Text = ws.Interval;
            }

            string currentSelected;
            bool success;
            List<DataPoint> dataPoints;
            string interval;
            string typeForSeries;
            string type;
            string symbol;
            string market;
            string[] tokens;

            foreach (string title in ws.Titles)
            {
                if(title.Contains("__"))
                {
                    typeForSeries = "CRYPTO CURRENCIES";

                    currentSelected = currentSelectedForCrypto + " crypto";
                    tokens = title.Split(new string[] { "__" }, System.StringSplitOptions.None);
                    symbol = tokens[0];
                    market = tokens[1];
                    type = "crypto";
                    interval = "";
                }
                else
                {
                    currentSelected = currentSelectedForShares;
                    typeForSeries = "SHARES";
                    symbol = title;
                    market = "";
                    type = "shares";
                    interval = ws.Interval;
                }
       
                success = Gvm.addSeries(title, typeForSeries);

                if (success)
                {
                    int counterAttempts = 0;
                    do
                    {
                        dataPoints = Mwvm.getSpecificData(symbol, ws.TimeSeriesType, currentSelected, interval, market);
                        if (dataPoints != null)
                        {
                            Gvm.addPoints(title, dataPoints);
                            double[] values = Statistics.getValues(dataPoints);
                            StatisticsTable.Items.Add(new Statistics(values, type, title));
                        }

                        counterAttempts++;
                    }
                    while (dataPoints == null && counterAttempts < 3);

                    if (dataPoints == null)
                    {
                        Gvm.removeSeries(title);
                        potencijalneGreske.Add(title);
                    } 
                }
                
            }

            CloseDialogSafe();

            if(potencijalneGreske.Count > 0)
            {
                MessageBox.Show("Problem with data delivery for: " + getMessage(potencijalneGreske), "Error");
            }

        }

        private string getMessage(List<string> potencijalneGreske)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < potencijalneGreske.Count; i++)
            {
                sb.Append(potencijalneGreske[i]);
                if (i != potencijalneGreske.Count - 1) sb.Append(",");
            }
            return sb.ToString();
        }

        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //DialogForSaveWorkspace dfsw = new DialogForSaveWorkspace();
            //dfsw.ShowDialog();

            string messageBoxText = "Do you want to save workspace?";
            string caption = "Save workspace";
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxImage icon = MessageBoxImage.Question;

            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);
            
            // Process message box results
            switch (result)
            {
                case MessageBoxResult.Yes:
                    List<string> titles = Gvm.getAllSeriesTitles();
                    string interval;
                    string timeSeriesType = TimeSeriesTypeComboBox.Text;
                    if (timeSeriesType.Equals("INTRADAY"))
                    {
                        interval = IntervalComboBox.Text;
                    }
                    else
                    {
                        interval = "";
                    }

                    saveWorkspace(titles, currentSelectedForShares, currentSelectedForCrypto, timeSeriesType, interval);
                    e.Cancel = false;
                    break;
                case MessageBoxResult.No:
                    e.Cancel = false;
                    break;
                case MessageBoxResult.Cancel:
                    e.Cancel = true;
                    break;
            }
        }

        private void saveWorkspace(List<string> titles, string currentSelectedForShares, string currentSelectedForCrypto, string timeSeriesType, string interval)
        {
            string path = @"..\..\Files\workspace.xml";

            using (XmlWriter writer = XmlWriter.Create(path))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Worspace");
                writer.WriteStartElement("Series");

                foreach (string title in titles)
                {
                    writer.WriteStartElement("Serie");
                    writer.WriteElementString("title", title);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();

                writer.WriteElementString("currentSelectedForShares", currentSelectedForShares);
                writer.WriteElementString("currentSelectedForCrypto", currentSelectedForCrypto);
                writer.WriteElementString("timeSeriesType", timeSeriesType);
                writer.WriteElementString("interval", interval);

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public Workspace loadWorkspace()
        {
            string path = @"..\..\Files\workspace.xml";

            FileInfo fi = new FileInfo(path);
            if (fi.Length == 0 || (fi.Length < 100000
     && !File.ReadAllLines(path)
        .Where(l => !String.IsNullOrEmpty(l.Trim())).Any()))
            {
                return null;
            }

            try
            {
                XDocument doc = XDocument.Load(path);

                List<string> titles = new List<string>();
                string title;
                string currentSelectedForShares;
                string currentSelectedForCrypto;
                string timeSeriesType;
                string interval;

                XElement[] elements = doc.Root.Elements().ToArray();
                foreach (XElement el in elements[0].Elements())
                {
                    title = el.Elements().ToArray()[0].Value;
                    titles.Add(title);
                }

                currentSelectedForShares = elements[1].Value;
                currentSelectedForCrypto = elements[2].Value;
                timeSeriesType = elements[3].Value;
                interval = elements[4].Value;

                Workspace ws = new Workspace(titles, currentSelectedForShares, currentSelectedForCrypto, timeSeriesType, interval);
                return ws;
            }
            catch { return null; }
        }

        private string ParseCurrencyInput(AutoCompleteTextBox autoComplete, FileType fileType)
        {
            try
            {
                string[] enteredCurrency = autoComplete.Text.Split(',');
                string currencyFull = enteredCurrency[0];
                string currencyAbb = enteredCurrency[1].Trim();
                Dictionary<string, string> collection = new Dictionary<string, string>();
                switch (fileType)
                {
                    case FileType.SHARE:
                        collection = con.shares;
                        break;
                    case FileType.CRYPTO_CURRENCY:
                        collection = con.cryptos;
                        break;
                    case FileType.CURRENCY:
                        collection = con.currs;
                        break;
                }
                if (collection[currencyAbb].Equals(currencyFull))
                {
                    return currencyAbb;
                }
            }
            catch
            {
            }
            return string.Empty;
        }

        private void Button_Click_Add_Shares(object sender, RoutedEventArgs e)
        {

            bool success = false;
            string share = ParseCurrencyInput(titleOfSeries, FileType.SHARE);
            if (!share.Equals(""))
            {
                success = Gvm.addSeries(share, "SHARES");
                if (success)
                {
                    Thread newWindowThread = new Thread(new ThreadStart(showWaitDialog));
                    newWindowThread.SetApartmentState(ApartmentState.STA);
                    newWindowThread.IsBackground = true;
                    newWindowThread.Start();



                    string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;
                    string interval;

                    if (contentOfTimeSeriesComboBox.Equals("INTRADAY"))
                    {
                        interval = IntervalComboBox.Text;
                    }
                    else
                    {
                        interval = "";
                    }

                    List<DataPoint> dataPoints;

                    int counterAttempts = 0;
                    do
                    {
                        dataPoints = Mwvm.getSpecificData(share, contentOfTimeSeriesComboBox, currentSelectedForShares, interval, "");
                        if (dataPoints != null)
                        {
                            Gvm.addPoints(share, dataPoints);
                            double[] values = Statistics.getValues(dataPoints);
                            StatisticsTable.Items.Add(new Statistics(values, "shares", titleOfSeries.Text.ToUpper()));
                        }

                        counterAttempts++;
                    }
                    while (dataPoints == null && counterAttempts < 3);

                    CloseDialogSafe();

                    if (dataPoints == null)
                    {
                        Gvm.removeSeries(share);
                        MessageBox.Show("Problem with data delivery for " + share, "Error");
                    }
                    
                }
                else
                {
                    MessageBox.Show("This series already exists!");
                }
            }
            else
            {
                MessageBox.Show("Invalid input!");
            }

            PlotView.InvalidatePlot(true); // refresh
        }

        private void showWaitDialog()
        {
            wdm = new WaitDialogMarko();
            wdm.ShowDialog();

            System.Windows.Threading.Dispatcher.Run();
        }

        void CloseDialogSafe()
        {
            if (wdm.Dispatcher.CheckAccess())
                wdm.Close();
            else
                wdm.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(wdm.Close));
        }

        private void iscrtajIspocetka(string contentOfTimeSeriesComboBox)
        {
            Gvm.clearAllPoints();
            StatisticsTable.Items.Clear();

            List<DataPoint> dataPoints;
            List<string> seriesTitles = Gvm.getAllSeriesTitles();
            string market;
            string currentSelected;
            string symbol;
            string[] tokens;
            string type;
            string interval;

            foreach (string st in seriesTitles)
            {
                if (st.Contains("__"))
                {
                    //ovo imaju crypto valute u svom nazivu
                    currentSelected = currentSelectedForCrypto + " crypto";
                    tokens = st.Split(new string[] { "__" }, System.StringSplitOptions.None);
                    symbol = tokens[0];
                    market = tokens[1];
                    type = "crypto";
                    interval = "";
                }
                else
                {
                    currentSelected = currentSelectedForShares;
                    symbol = st;
                    market = "";
                    type = "shares";
                    if (contentOfTimeSeriesComboBox.Equals("INTRADAY"))
                    {
                        interval = IntervalComboBox.Text;
                    }
                    else
                    {
                        interval = "";
                    }
                }

                int counterAttempts = 0;

                do
                {
                    dataPoints = Mwvm.getSpecificData(symbol, contentOfTimeSeriesComboBox, currentSelected, interval, market);
                    if (dataPoints != null)
                    {
                        Gvm.addPoints(st, dataPoints);
                        double[] values = Statistics.getValues(dataPoints);
                        StatisticsTable.Items.Add(new Statistics(values, type, st));
                    }

                    counterAttempts++;

                } while (dataPoints == null && counterAttempts < 3);

                if (dataPoints == null)
                {
                    MessageBox.Show("Problem with data delivery for " + st, "Error");
                }
            }

            LastUpdatedTextBlock.Text = "Last Updated: " + DateTime.Now;

            PlotView.InvalidatePlot(true); // refresh
        }

        private void Button_Click_Add_Shares_In_New_Window(object sender, RoutedEventArgs e)
        {
            string timeSeriesType = TimeSeriesTypeComboBox.Text;
            string interval;

            if (timeSeriesType.Equals("INTRADAY"))
            {
                 interval = IntervalComboBox.Text;
            }
            else
            {
                interval = "";
            }

            string share = ParseCurrencyInput(titleOfSeries, FileType.SHARE);
            if (!share.Equals(""))
            {
                DialogForOneGraphic d = new DialogForOneGraphic(share, currentSelectedForShares, timeSeriesType, interval, con, Mwvm);
                d.Show();
            }
            else
            {
                MessageBox.Show("Invalid input!");
            }
        }

        private void addRadioButtons(string[] contents, string currentSelected)
        {
            StackPanelForRadioButtonCrypto.Children.Clear();

            RadioButton rb;
            for(int i = 0; i < contents.Length; i++)
            {
                rb = new RadioButton { GroupName = "Group2", Content = contents[i]};
                if (contents[i].ToLower().Equals(currentSelected)) rb.IsChecked = true;
                rb.Click += rb_Click_Crypto;
                StackPanelForRadioButtonCrypto.Children.Add(rb);
            }
            //((RadioButton)StackPanelForRadioButtonCrypto.Children[0]).IsChecked = true;
            
        }

        private void TimeSeriesType_ComboBox_DataContextChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            
            if (Gvm != null)
            {
                string contentOfTimeSeriesComboBox = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;
              
                if (contentOfTimeSeriesComboBox.Equals("INTRADAY"))
                {
                    IntervalComboBox.Visibility = Visibility.Visible;
                    OnlyForShares.Visibility = Visibility.Visible;
                    currentSelectedForCrypto = "price";
                    addRadioButtons(contentsForRadioButtonsIntraday, currentSelectedForCrypto);
                }
                else
                {
                    IntervalComboBox.Visibility = Visibility.Hidden;
                    OnlyForShares.Visibility = Visibility.Hidden;
                    currentSelectedForCrypto = "open";
                    addRadioButtons(contentsForRadioButtonsAnother, currentSelectedForCrypto);
                }

                iscrtajIspocetka(contentOfTimeSeriesComboBox);

              
            }

        }

        private void IntervalComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Gvm != null)
            {
                string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;
                IntervalComboBox.Text = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;

                iscrtajIspocetka(contentOfTimeSeriesComboBox);
            }
        }

        private void Button_Click_Add_Crypto(object sender, RoutedEventArgs e)
        {
            string cryptoCurrency = ParseCurrencyInput(nameOfCryptoCurrency, FileType.CRYPTO_CURRENCY);
            string currency = ParseCurrencyInput(nameOfCurrency, FileType.CURRENCY);
            if (!currency.Equals("") & !cryptoCurrency.Equals(""))
            {
                string nameOfSeries = cryptoCurrency + "__" + currency;
                bool success = Gvm.addSeries(nameOfSeries, "CRYPTO CURRENCIES");

                if (success)
                {
                    Thread newWindowThread = new Thread(new ThreadStart(showWaitDialog));
                    newWindowThread.SetApartmentState(ApartmentState.STA);
                    newWindowThread.IsBackground = true;
                    newWindowThread.Start();

                    string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;

                    int counterAttempts = 0;

                    List<DataPoint> dataPoints;
                    do
                    {
                        dataPoints = Mwvm.getSpecificData(cryptoCurrency, contentOfTimeSeriesComboBox, currentSelectedForCrypto + " crypto", "", currency);
                        if (dataPoints != null)
                        {
                            Gvm.addPoints(nameOfSeries, dataPoints);
                            double[] values = Statistics.getValues(dataPoints);
                            StatisticsTable.Items.Add(new Statistics(values, "crypto", nameOfSeries));
                        }

                        counterAttempts++;

                    } while (dataPoints == null && counterAttempts < 3);

                    CloseDialogSafe();

                    if (dataPoints == null)
                    {
                        Gvm.removeSeries(nameOfSeries);
                        MessageBox.Show("Problem with data delivery for " + nameOfSeries, "Error");
                    }
                    
                }
                else
                {
                    MessageBox.Show("This series already exists!");
                }
            }
            else
            {
                MessageBox.Show("Invalid input!");
            }

            PlotView.InvalidatePlot(true); // refresh
        }

        private void Button_Click_Add_Crypto_In_New_Window(object sender, RoutedEventArgs e)
        {
            string cryptoCurrency = ParseCurrencyInput(nameOfCryptoCurrency, FileType.CRYPTO_CURRENCY);
            string currency = ParseCurrencyInput(nameOfCurrency, FileType.CURRENCY);
            string timeSeriesType = TimeSeriesTypeComboBox.Text;
            string nameOfSeries = cryptoCurrency + "__" + currency;

            DialogForOneGraphic d = new DialogForOneGraphic(nameOfSeries, currentSelectedForCrypto, timeSeriesType, "", con, Mwvm);
            d.Show();

        }


        private void rb_Click(object sender, RoutedEventArgs e)
        {
            string oldCurrentSelectedForShares = currentSelectedForShares;
            currentSelectedForShares = ((sender as RadioButton).Content as string).ToLower(); ;
            if (currentSelectedForShares.Equals(oldCurrentSelectedForShares)) return;

            string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;

            iscrtajIspocetka(contentOfTimeSeriesComboBox);
        }

        private void rb_Click_Crypto(object sender, RoutedEventArgs e)
        {
            string oldCurrentSelectedForCrypto = currentSelectedForCrypto;
            currentSelectedForCrypto = ((sender as RadioButton).Content as string).ToLower();
            if (currentSelectedForCrypto.Equals(oldCurrentSelectedForCrypto)) return;

            string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;

            iscrtajIspocetka(contentOfTimeSeriesComboBox);
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            int index = StatisticsTable.SelectedIndex;

            if(index >= 0)
            {
                StatisticsTable.Items.RemoveAt(index);
                Gvm.removeSeries(index);

                PlotView.InvalidatePlot(true); // refresh
            }

        }

        private void selektujOdgovarajuciRadioButtonForShares()
        {
            switch (currentSelectedForShares)
            {
                case "open":
                    rbOpen.IsChecked = true;
                    break;
                case "high":
                    rbHigh.IsChecked = true;
                    break;
                case "low":
                    rbLow.IsChecked = true;
                    break;
                case "close":
                    rbClose.IsChecked = true;
                    break;
                case "volume":
                    rbVolume.IsChecked = true;
                    break;
                // all - ne moze biti inicijalno selektovano, jer nema all u glavnom prozoru
                default: break;
            }
        }

    }
}

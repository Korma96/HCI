using HCI.ViewModel;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HCI.View
{
    /// <summary>
    /// Interaction logic for DialogForOneGraphic.xaml
    /// </summary>
    public partial class DialogForOneGraphic : Window
    {
        public MainWindowViewModel Mwvm { get; set; }
        public GraphicViewModel Gvm { get; set; }

        public string Title { get; set; }
        private string[] contentsForRadioButtonsIntraday = { "Price", "Price USD", "Volume", "Market cap USD", "All" };
        private string[] contentsForRadioButtonsAnother = { "Open", "Open USD", "High", "High USD", "Low", "Low USD", "Close", "Close USD", "Volume", "Market cap USD", "All" };
        private string[] contentsForRadioButtonsForShares = { "Open", "High", "Low", "Close", "Volume", "All" };


        private string currentSelected;

        public DialogForOneGraphic(string title, string currentSelected, string timeSeriesType, string interval, string market, Controller con, MainWindowViewModel mwvm)
        {
            InitializeComponent();

            this.currentSelected = currentSelected;
            //selektujOdgovarajuciRadioButton();

            if(!market.Equals(""))
            {
                if (timeSeriesType.Equals("INTRADAY"))
                {
                    addRadioButtons(contentsForRadioButtonsIntraday, this.currentSelected);
                }
                else
                {
                    addRadioButtons(contentsForRadioButtonsAnother, this.currentSelected);
                }
            }
            else
            {
                addRadioButtons(contentsForRadioButtonsForShares, currentSelected);
            }

            Title = title;
            TimeSeriesTypeComboBox.Text = timeSeriesType;
            if(!interval.Equals(""))
            {
                IntervalComboBox.Visibility = Visibility.Visible;
                IntervalComboBox.Text = interval;
            }

            Mwvm = mwvm;
            Gvm = new GraphicViewModel(con, Title);

            this.DataContext = Gvm;

            Gvm.addSeriesWithOutCheck(currentSelected.ToUpper());
  
            string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;

            string symbol;
            if (Title.Contains("__"))
            {
                string[] tokens = Title.Split(new string[] { "__" }, System.StringSplitOptions.None);
                symbol = tokens[0];

            }
            else
            {
                symbol = Title;
            }

            List<DataPoint> dataPoints = Mwvm.getSpecificData(symbol, contentOfTimeSeriesComboBox, currentSelected, interval, market);
            if (dataPoints != null) Gvm.addPoints(currentSelected.ToUpper(), dataPoints);
            else
            {
                MessageBox.Show("Problem with data delivery for " + Title, "Error");
            }

            PlotView.InvalidatePlot(true); // refresh
            
            
        }

        private void addRadioButtons(string[] contents, string currentSelected)
        {
            StackPanelForRadioButtonCrypto.Children.Clear();

            RadioButton rb;
            for (int i = 0; i < contents.Length; i++)
            {
                rb = new RadioButton { GroupName = "Group2", Content = contents[i] };
                if (contents[i].ToLower().Equals(currentSelected)) rb.IsChecked = true;
                rb.Click += rb_Click;
                StackPanelForRadioButtonCrypto.Children.Add(rb);
            }
            //((RadioButton)StackPanelForRadioButtonCrypto.Children[0]).IsChecked = true;

        }

        private void rb_Click(object sender, RoutedEventArgs e)
        {
            string oldCurrentSelected = currentSelected;
            currentSelected = ((sender as RadioButton).Content as string).ToLower(); ;
            if (currentSelected.Equals(oldCurrentSelected)) return;

            string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;

            iscrtajIspocetka(contentOfTimeSeriesComboBox);
        }

        /*private void selektujOdgovarajuciRadioButton()
        {
            switch(currentSelected)
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
        }*/

        /*private void rbOpen_Click(object sender, RoutedEventArgs e)
        {
            if (currentSelected.Equals("open")) return;

            currentSelected = "open";
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
            iscrtajIspocetka(contentOfTimeSeriesComboBox);
        }

        private void rbHigh_Click(object sender, RoutedEventArgs e)
        {
            if (currentSelected.Equals("high")) return;

            currentSelected = "high";
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

            iscrtajIspocetka(contentOfTimeSeriesComboBox);
        }

        private void rbLow_Click(object sender, RoutedEventArgs e)
        {
            if (currentSelected.Equals("low")) return;

            currentSelected = "low";
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

            iscrtajIspocetka(contentOfTimeSeriesComboBox);
        }

        private void rbClose_Click(object sender, RoutedEventArgs e)
        {
            if (currentSelected.Equals("close")) return;

            currentSelected = "close";
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

            iscrtajIspocetka(contentOfTimeSeriesComboBox);
        }

        private void rbVolume_Click(object sender, RoutedEventArgs e)
        {
            if (currentSelected.Equals("volume")) return;

            currentSelected = "volume";
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

            iscrtajIspocetka(contentOfTimeSeriesComboBox);
        }

        private void rbAll_Click(object sender, RoutedEventArgs e)
        {
            if (currentSelected.Equals("all")) return;

            currentSelected = "all";
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

            iscrtajIspocetka(contentOfTimeSeriesComboBox);
        }*/

        private void iscrtajIspocetka(string contentOfTimeSeriesComboBox)
        {
            Gvm.clearAllSeries();

            string symbol;
            string market;
            string interval = "";
            string[] allSeries;

            if (Title.Contains("__"))
            {
                string[] tokens = Title.Split(new string[] { "__" }, System.StringSplitOptions.None);
                symbol = tokens[0];
                market = tokens[1];
                if (contentOfTimeSeriesComboBox.Equals("INTRADAY"))
                {
                    allSeries = contentsForRadioButtonsIntraday;
                }
                else
                {
                    allSeries = contentsForRadioButtonsAnother;
                }
            }
            else
            {
                symbol = Title;
                market = "";
                allSeries = contentsForRadioButtonsForShares;
                if (contentOfTimeSeriesComboBox.Equals("INTRADAY"))
                {
                    interval = IntervalComboBox.Text;
                }

            }

            if (!currentSelected.Equals("all"))
            {
                Gvm.addSeriesWithOutCheck(currentSelected.ToUpper());

                List<DataPoint> dataPoints = Mwvm.getSpecificData(symbol, contentOfTimeSeriesComboBox, currentSelected, interval, market);
                if (dataPoints != null) Gvm.addPoints(currentSelected.ToUpper(), dataPoints);
                else
                {
                    MessageBox.Show("Problem sa dobavljanjem podataka", "Greska");
                }
            }
            else
            {
                Gvm.addAllSeries(allSeries);

                List<DataPoint>[] dataPointsArray;
                if (market.Equals(""))
                {
                    dataPointsArray = Mwvm.getDataForOpenHighLowCloseForShares(Title, contentOfTimeSeriesComboBox, interval);
                }
                else
                {
                    dataPointsArray = Mwvm.getDataForOpenHighLowCloseForCrypto(Title, contentOfTimeSeriesComboBox);
                }
                
                List<string> seriesTitles = Gvm.getAllSeriesTitles();
                for (int i = 0; i < seriesTitles.Count; i++)
                {

                    if (dataPointsArray[i] != null) Gvm.addPoints(seriesTitles[i], dataPointsArray[i]);
                }
            }

            PlotView.InvalidatePlot(true); // refresh
        }

        private void TimeSeriesType_ComboBox_DataContextChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (Gvm != null)
            {
                string contentOfTimeSeriesComboBox = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;

                if (contentOfTimeSeriesComboBox.Equals("INTRADAY"))
                {
                    IntervalComboBox.Visibility = Visibility.Visible;
                }
                else
                {
                    IntervalComboBox.Visibility = Visibility.Hidden;
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
    }
}

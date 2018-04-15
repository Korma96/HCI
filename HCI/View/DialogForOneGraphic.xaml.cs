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


        private string currentSelected;

        public DialogForOneGraphic(string title, string currentSelected, string timeSeriesType, string interval, Controller con, MainWindowViewModel mwvm)
        {
            InitializeComponent();

            this.currentSelected = currentSelected;
            selektujOdgovarajuciRadioButton();

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

            string market;
            if (Title.Contains("__"))
            {
                //ovo imaju crypto valute u svom nazivu
                market = Title.Split(new string[] { "__" }, System.StringSplitOptions.None)[1];
            }
            else
            {
                market = "";
            }

            List<DataPoint> dataPoints = Mwvm.getSpecificData(Title, contentOfTimeSeriesComboBox, currentSelected, interval, market);
            if (dataPoints != null) Gvm.addPoints(currentSelected.ToUpper(), dataPoints);
            else
            {
                MessageBox.Show("Problem sa dobavljanjem podataka", "Greska");
            }

            PlotView.InvalidatePlot(true); // refresh
            
            
        }

        private void selektujOdgovarajuciRadioButton()
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
        }

        private void rbOpen_Click(object sender, RoutedEventArgs e)
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
            iscrtajIspocetka(contentOfTimeSeriesComboBox, interval);
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

            iscrtajIspocetka(contentOfTimeSeriesComboBox, interval);
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

            iscrtajIspocetka(contentOfTimeSeriesComboBox, interval);
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

            iscrtajIspocetka(contentOfTimeSeriesComboBox, interval);
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

            iscrtajIspocetka(contentOfTimeSeriesComboBox, interval);
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

            iscrtajIspocetka(contentOfTimeSeriesComboBox, interval);
        }

        private void iscrtajIspocetka(string contentOfTimeSeriesComboBox, string interval)
        {
            Gvm.clearAllSeries();

            string titleOfSeries = "";

            switch (currentSelected)
            {
                case "open":
                    titleOfSeries = "OPEN";
                    break;
                case "high":
                    titleOfSeries = "HIGH";
                    break;
                case "low":
                    titleOfSeries = "LOW";
                    break;
                case "close":
                    titleOfSeries = "CLOSE";
                    break;
                case "volume":
                    titleOfSeries = "VOLUME";
                    break;
                case "all":
                    Gvm.addAllSeries();
                    List<DataPoint>[] dataPointsArray = Mwvm.getDataForOpenHighLowClose(Title, contentOfTimeSeriesComboBox, interval);
                    List<string> seriesTitles = Gvm.getAllSeriesTitles();
                    for (int i = 0; i < seriesTitles.Count; i++)
                    {

                        if (dataPointsArray[i] != null) Gvm.addPoints(seriesTitles[i], dataPointsArray[i]);
                    }
                    break;
                default: break;
            }

            if (!currentSelected.Equals("all"))
            {
                Gvm.addSeriesWithOutCheck(titleOfSeries);

                string symbol;
                string market;
                if (titleOfSeries.Contains("__"))
                {
                    string[] tokens = Title.Split(new string[] { "__" }, System.StringSplitOptions.None);
                    symbol = tokens[0];
                    market = tokens[1];
                }
                else
                {
                    symbol = Title;
                    market = "";
                }

                List<DataPoint> dataPoints = Mwvm.getSpecificData(symbol, contentOfTimeSeriesComboBox, currentSelected, interval, market);
                if (dataPoints != null) Gvm.addPoints(titleOfSeries, dataPoints);
                else
                {
                    MessageBox.Show("Problem sa dobavljanjem podataka", "Greska");
                }
            }

            PlotView.InvalidatePlot(true); // refresh
        }

        private void TimeSeriesType_ComboBox_DataContextChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (Gvm != null)
            {
                string contentOfTimeSeriesComboBox = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;
                string interval;

                if (contentOfTimeSeriesComboBox.Equals("INTRADAY"))
                {
                    IntervalComboBox.Visibility = Visibility.Visible;
                    interval = IntervalComboBox.Text;
                }
                else
                {
                    IntervalComboBox.Visibility = Visibility.Hidden;
                    interval = "";
                }

                iscrtajIspocetka(contentOfTimeSeriesComboBox, interval);

                MessageBox.Show("Odradjeno- " + contentOfTimeSeriesComboBox, "Uradjeno");
            }

        }

        private void IntervalComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Gvm != null)
            {
                string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;
                string interval = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;

                iscrtajIspocetka(contentOfTimeSeriesComboBox, interval);

                MessageBox.Show("Odradjeno- " + interval, "Uradjeno");
            }
        }
    }
}

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

namespace HCI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public GraphicViewModel Gvm { get; set; }
        MainWindowViewModel Mwvm { get; set; }
        private Controller con;

        private string currentSelected;

        public MainWindow()
        {
            InitializeComponent();

            con = new Controller();
            Gvm = new GraphicViewModel(con, "");
            Mwvm = new MainWindowViewModel(con);

            this.DataContext = Gvm;

            currentSelected = "open";


            /*Gvm.addSeries("open");
            Gvm.addSeries("high");
            Gvm.addSeries("low");
            Gvm.addSeries("close");
            //Gvm.addSeries("volume");

            DataPoint dp;

            foreach (KeyValuePair<DateTime, TimeSerie> kv in Mwvm.TimeSeriesData)
            {
                dp = DateTimeAxis.CreateDataPoint(kv.Key, kv.Value.Open);
                Gvm.addPoint("open", dp);
                dp = DateTimeAxis.CreateDataPoint(kv.Key, kv.Value.High);
                Gvm.addPoint("high", dp);
                dp = DateTimeAxis.CreateDataPoint(kv.Key, kv.Value.Low);
                Gvm.addPoint("low", dp);
                dp = DateTimeAxis.CreateDataPoint(kv.Key, kv.Value.Close);
                Gvm.addPoint("close", dp);
                //dp = DateTimeAxis.CreateDataPoint(kv.Key, kv.Value.Volume);
                //Gvm.addPoint("volume", dp);
            }*/
        }

        private void Button_Dodaj_Click(object sender, RoutedEventArgs e)
        {
            
            bool success = Gvm.addSeries(titleOfSeries.Text.ToUpper());

            if (success)
            {
                string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;

                List<DataPoint> dataPoints = Mwvm.getSpecificData(titleOfSeries.Text.ToUpper(), contentOfTimeSeriesComboBox, currentSelected);
                if(dataPoints != null) Gvm.addPoints(titleOfSeries.Text.ToUpper(), dataPoints);
                else
                {
                    MessageBox.Show("Problem sa dobavljanjem podataka", "Greska");
                }

                PlotView.InvalidatePlot(true); // refresh
            }
            MessageBox.Show("Dodat Series " + PlotView.Model.Series.Count);
        }

        private void rbOpen_Click(object sender, RoutedEventArgs e)
        {
            if (currentSelected.Equals("open")) return;

            currentSelected = "open";
            string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;
            iscrtajIspocetka(contentOfTimeSeriesComboBox);
        }

        private void rbHigh_Click(object sender, RoutedEventArgs e)
        {
            if (currentSelected.Equals("high")) return;

            currentSelected = "high";
            string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;
            iscrtajIspocetka(contentOfTimeSeriesComboBox);
        }

        private void rbLow_Click(object sender, RoutedEventArgs e)
        {
            if (currentSelected.Equals("low")) return;

            currentSelected = "low";
            string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;
            iscrtajIspocetka(contentOfTimeSeriesComboBox);
        }

        private void rbClose_Click(object sender, RoutedEventArgs e)
        {
            if (currentSelected.Equals("close")) return;

            currentSelected = "close";
            string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;
            iscrtajIspocetka(contentOfTimeSeriesComboBox);
        }

        private void rbVolume_Click(object sender, RoutedEventArgs e)
        {
            currentSelected = "volume";
            string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;
            iscrtajIspocetka(contentOfTimeSeriesComboBox);
        }

        private void iscrtajIspocetka(string contentOfTimeSeriesComboBox)
        {
            Gvm.clearAllPoints(); 

            List<DataPoint> dataPoints;
            List<string> seriesTitles = Gvm.getAllSeriesTitles();
            foreach(string st in seriesTitles)
            {
                dataPoints = Mwvm.getSpecificData(st, contentOfTimeSeriesComboBox, currentSelected);
                if (dataPoints != null) Gvm.addPoints(st, dataPoints);
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

                iscrtajIspocetka(contentOfTimeSeriesComboBox);
                
                MessageBox.Show("Odradjeno- " + contentOfTimeSeriesComboBox, "Uradjeno");
            }
            
        }

        private void Button_DodajUNovomProzoru_Click(object sender, RoutedEventArgs e)
        {
            DialogForOneGraphic d = new DialogForOneGraphic(titleOfSeries.Text.ToUpper(), con, Mwvm);
            d.Show();
            
        }
    }
}

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

        public DialogForOneGraphic(string title, Controller con, MainWindowViewModel mwvm)
        {
            InitializeComponent();

            currentSelected = "open";
            Title = title;

            Mwvm = mwvm;
            Gvm = new GraphicViewModel(con, Title);

            this.DataContext = Gvm;

            Gvm.addSeriesWithOutCheck(currentSelected);
  
            string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;

            List<DataPoint> dataPoints = Mwvm.getSpecificData(Title, contentOfTimeSeriesComboBox, currentSelected);
            if (dataPoints != null) Gvm.addPoints(currentSelected, dataPoints);
            else
            {
                MessageBox.Show("Problem sa dobavljanjem podataka", "Greska");
            }

            PlotView.InvalidatePlot(true); // refresh
            
            
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
            if (currentSelected.Equals("volume")) return;

            currentSelected = "volume";
            string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;
            iscrtajIspocetka(contentOfTimeSeriesComboBox);
        }

        private void rbAll_Click(object sender, RoutedEventArgs e)
        {
            if (currentSelected.Equals("all")) return;

            currentSelected = "all";
            string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;
            iscrtajIspocetka(contentOfTimeSeriesComboBox);
        }

        private void iscrtajIspocetka(string contentOfTimeSeriesComboBox)
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
                case "all":
                    Gvm.addAllSeries();
                    List<DataPoint>[] dataPointsArray = Mwvm.getDataForOpenHighLowClose(Title, contentOfTimeSeriesComboBox);
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

                List<DataPoint> dataPoints = Mwvm.getSpecificData(Title, contentOfTimeSeriesComboBox, currentSelected);
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

                iscrtajIspocetka(contentOfTimeSeriesComboBox);

                MessageBox.Show("Odradjeno- " + contentOfTimeSeriesComboBox, "Uradjeno");
            }

        }

    }
}

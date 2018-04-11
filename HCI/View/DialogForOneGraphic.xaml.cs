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

       
        private string currentSelected;

        public DialogForOneGraphic(string title, Controller con, MainWindowViewModel mwvm)
        {
            InitializeComponent();

            currentSelected = "open";

            Mwvm = mwvm;
            Gvm = new GraphicViewModel(con, title);

            this.DataContext = Gvm;

            bool success = Gvm.addSeries(title);

            if (success)
            {
                string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;

                List<DataPoint> dataPoints = Mwvm.getSpecificData(title, contentOfTimeSeriesComboBox, currentSelected);
                if (dataPoints != null) Gvm.addPoints(title, dataPoints);
                PlotView.InvalidatePlot(true); // refresh
            }
            //MessageBox.Show("Dodat Series " + PlotView.Model.Series.Count);
        }

        private void rbOpen_Click(object sender, RoutedEventArgs e)
        {
            currentSelected = "open";
            string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;
            iscrtajIspocetka(contentOfTimeSeriesComboBox);
        }

        private void rbHigh_Click(object sender, RoutedEventArgs e)
        {
            currentSelected = "high";
            string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;
            iscrtajIspocetka(contentOfTimeSeriesComboBox);
        }

        private void rbLow_Click(object sender, RoutedEventArgs e)
        {
            currentSelected = "low";
            string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;
            iscrtajIspocetka(contentOfTimeSeriesComboBox);
        }

        private void rbClose_Click(object sender, RoutedEventArgs e)
        {
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
            foreach (string st in seriesTitles)
            {
                dataPoints = Mwvm.getSpecificData(st, contentOfTimeSeriesComboBox, currentSelected);
                if (dataPoints != null) Gvm.addPoints(st, dataPoints);
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

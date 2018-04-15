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

        private string currentSelectedForShares;
        private string currentSelectedForCrypto;

        private string[] namesForRadioButtonsIntraday = { "Price", "Price USD", "Volume", "Market cap USD" };
        private string[] namesForRadioButtonsAnother = { "Open", "Open USD", "High", "High USD", "Low", "Low USD", "Close", "Close USD", "Volume", "Market cap USD" };

        public MainWindow()
        {
            InitializeComponent();

            con = new Controller();
            Gvm = new GraphicViewModel(con, "");
            Mwvm = new MainWindowViewModel(con);

            this.DataContext = Gvm;

            currentSelectedForShares = "open";
            currentSelectedForCrypto = "open";

        }

        private void Button_Click_Add_Shares(object sender, RoutedEventArgs e)
        {
            
            bool success = Gvm.addSeries(titleOfSeries.Text.ToUpper(), "SHARES");

            if (success)
            {
                string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;
                string interval;

                if(contentOfTimeSeriesComboBox.Equals("INTRADAY"))
                {
                    interval = IntervalComboBox.Text;
                }
                else
                {
                    interval = "";
                }

                string nameOfSeries = titleOfSeries.Text.ToUpper();

                List<DataPoint> dataPoints;

                int counterAttempts = 0;
                do
                {
                    dataPoints = Mwvm.getSpecificData(nameOfSeries, contentOfTimeSeriesComboBox, currentSelectedForShares, interval, "");
                    if (dataPoints != null)
                    {
                        Gvm.addPoints(nameOfSeries, dataPoints);
                        double[] values = Statistics.getValues(dataPoints);
                        StatisticsTable.Items.Add(new Statistics(values, "shares", titleOfSeries.Text.ToUpper()));
                    }

                    counterAttempts++;
                }
                while (dataPoints == null && counterAttempts < 3);

                if (dataPoints == null)
                {
                    Gvm.removeSeries(nameOfSeries);
                    MessageBox.Show("Problem sa dobavljanjem podataka za " + nameOfSeries, "Greska");
                }


                PlotView.InvalidatePlot(true); // refresh
            }
            MessageBox.Show("Dodat Shares " + PlotView.Model.Series.Count);
        }

        private void iscrtajIspocetka(string contentOfTimeSeriesComboBox, string interval)
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
                }
                else
                {
                    currentSelected = currentSelectedForShares;
                    symbol = st;
                    market = "";
                    type = "shares";
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
                    MessageBox.Show("Problem sa dobavljanjem podataka za " + st, "Greska");
                }
            }
            
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

            DialogForOneGraphic d = new DialogForOneGraphic(titleOfSeries.Text.ToUpper(), currentSelectedForShares, timeSeriesType, interval, con, Mwvm);
            d.Show();
            
        }

        private void PlotView_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this);
            OxyRect legendArea = Gvm.MyModel.LegendArea;
            
            double x = p.X;
            double y = p.Y - 21; // korigovanje y koordinate u odnosu na koordinate legendarea-e (iz nekog razloga je ovo neophodno odraditi)

            MessageBox.Show("("+x+","+ y+")  (("+ legendArea.Left+", "+legendArea.Right+"), (" + legendArea.Top+ ", " + legendArea.Bottom+"))" , "LegendArea");
            if (legendArea.Contains(x, y))
            {
                MessageBox.Show("Kliknuto unutar", "LegendArea");
                
                
            }
            //MyPlotView.Model.Annotations.Add(new RectangleAnnotation() { MinimumX = e.Position.X, MinimumY = e.Position.Y });
        }

        private void addRadioButtons(string[] names)
        {
            StackPanelForRadioButtonCrypto.Children.Clear();

            RadioButton rb;
            foreach(string name in names)
            {
                rb = new RadioButton { GroupName = "Group2", Content = name};
                rb.Click += rb_Click_Crypto;
                StackPanelForRadioButtonCrypto.Children.Add(rb);
            }
            ((RadioButton)StackPanelForRadioButtonCrypto.Children[0]).IsChecked = true;
            
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
                    OnlyForShares.Visibility = Visibility.Visible;
                    interval = IntervalComboBox.Text;
                    addRadioButtons(namesForRadioButtonsIntraday);
                    currentSelectedForCrypto = "price";
                }
                else
                {
                    IntervalComboBox.Visibility = Visibility.Hidden;
                    OnlyForShares.Visibility = Visibility.Hidden;
                    interval = "";
                    addRadioButtons(namesForRadioButtonsAnother);
                    currentSelectedForCrypto = "open";
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

        private void Button_Click_Add_Crypto(object sender, RoutedEventArgs e)
        {
            string nameOfSeries = nameOfCryptoCurrency.Text.ToUpper() + "__" + nameOfCurrency.Text.ToUpper();
            bool success = Gvm.addSeries(nameOfSeries, "CRYPTO CURRENCIES");

            if (success)
            {
                string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;

                int counterAttempts = 0;

                List<DataPoint> dataPoints;
                do
                {
                    dataPoints = Mwvm.getSpecificData(nameOfCryptoCurrency.Text.ToUpper(), contentOfTimeSeriesComboBox, currentSelectedForCrypto + " crypto", "", nameOfCurrency.Text.ToUpper());
                    if (dataPoints != null)
                    {
                        Gvm.addPoints(nameOfSeries, dataPoints);
                        double[] values = Statistics.getValues(dataPoints);
                        StatisticsTable.Items.Add(new Statistics(values, "crypto", nameOfSeries));
                    }

                    counterAttempts++;

                } while (dataPoints == null && counterAttempts < 3);

                if (dataPoints == null)
                {
                    Gvm.removeSeries(nameOfSeries);
                    MessageBox.Show("Problem sa dobavljanjem podataka za " + nameOfSeries, "Greska");
                }
                

                PlotView.InvalidatePlot(true); // refresh
            }
            MessageBox.Show("Dodat Crypto Currency " + PlotView.Model.Series.Count);
        }

        private void Button_Click_Add_Crypto_In_New_Window(object sender, RoutedEventArgs e)
        {
            string timeSeriesType = TimeSeriesTypeComboBox.Text;
            string nameOfSeries = nameOfCryptoCurrency.Text.ToUpper() + "__" + nameOfCurrency.Text.ToUpper();

            DialogForOneGraphic d = new DialogForOneGraphic(nameOfSeries, currentSelectedForCrypto, timeSeriesType, "", con, Mwvm);
            d.Show();

        }


        private void rb_Click(object sender, RoutedEventArgs e)
        {
            string oldCurrentSelectedForShares = currentSelectedForShares;
            currentSelectedForShares = ((sender as RadioButton).Content as string).ToLower(); ;
            if (currentSelectedForShares.Equals(oldCurrentSelectedForShares)) return;

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

        private void rb_Click_Crypto(object sender, RoutedEventArgs e)
        {
            string oldCurrentSelectedForCrypto = currentSelectedForCrypto;
            currentSelectedForCrypto = ((sender as RadioButton).Content as string).ToLower();
            if (currentSelectedForCrypto.Equals(oldCurrentSelectedForCrypto)) return;

            string contentOfTimeSeriesComboBox = TimeSeriesTypeComboBox.Text;
            string interval = "";

            iscrtajIspocetka(contentOfTimeSeriesComboBox, interval);
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

    }
}

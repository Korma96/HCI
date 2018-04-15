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
    public partial class TableStatistics : Window
    {
        
        public TableStatistics()
        {
            InitializeComponent();
            this.DataContext = this;
            statistics = new ObservableCollection<Statistics>();
        }




        private void dgrMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

}
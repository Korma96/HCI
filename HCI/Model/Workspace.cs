using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI.Model
{
    public class Workspace
    {
        public List<string> Titles { get; set; }
        public string CurrentSelectedForShares { get; set; }
        public string CurrentSelectedForCrypto { get; set; }
        public string TimeSeriesType { get; set; }
        public string Interval { get; set; }

        public Workspace(List<string> titles, string currentSelectedForShares, string currentSelectedForCrypto, string timeSeriesType, string interval)
        {
            Titles = titles;
            CurrentSelectedForShares = currentSelectedForShares;
            CurrentSelectedForCrypto = currentSelectedForCrypto;
            TimeSeriesType = timeSeriesType;
            Interval = interval;
        }
    }
}

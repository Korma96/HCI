using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI.Table
{
    class Statistics
    {
        public string type { get; set; }
        public string name { get; set; }
        public double median { get; set; }
        public double lowest { get; set; }
        public double highest { get; set; }
        public double mode { get; set; }
        public double exp { get; set; }

        public Statistics(double[] data, string type, string name)
        {
            this.type = type;
            this.name = name;
            this.calculateMedian(data);
            this.calculateMax(data);
            this.calculateMin(data);
            this.calculateMode(data);
            this.calculateExpectation(data);

        }

        public void calculateMedian(double[] data)
        {
            double median = 0;
            data = (from element in data orderby element ascending select element).ToArray();
            int len = data.Count();
            if (len % 2 == 0)
            {
                median = (data[len / 2] + data[len / 2 - 1]) / 2;
            }
            else
            {
                if (len == 1)
                {
                    median = data[0];
                }
                else
                {
                    median = data[(len - 1) / 2];
                }
            }
            this.median = median;
        }



        public void calculateMode(double[] data)
        {
            Dictionary<int, int> counts = new Dictionary<int, int>();
            foreach (int a in data)
            {
                if (counts.ContainsKey(a))
                    counts[a] = counts[a] + 1;
                else
                    counts[a] = 1;
            }

            int result = int.MinValue;
            int max = int.MinValue;
            foreach (int key in counts.Keys)
            {
                if (counts[key] > max)
                {
                    max = counts[key];
                    result = key;
                }
            }
            this.mode = result;
        }

        public void calculateMax(double[] data)
        {
            this.highest = data.Max();
        }

        public void calculateMin(double[] data)
        {
            this.lowest = data.Min();
        }

        public void calculateExpectation(double[] data)
        {
            double n = data.Count();
            double prb = (1 / n);

            double sum = 0;

            for (int i = 0; i < n; i++)
                sum += data[i] * prb;

            this.exp = sum;
        }
    }

}

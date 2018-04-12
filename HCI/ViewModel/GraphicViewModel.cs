using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HCI.ViewModel
{
    public class GraphicViewModel
    {

        public PlotModel MyModel { get; set; }
        private OxyColor[] colors = { OxyColors.Red, OxyColors.Black, OxyColors.Blue, OxyColors.Green, OxyColors.Turquoise,
                                        OxyColors.Tomato, OxyColors.Thistle, OxyColors.Teal, OxyColors.Tan, OxyColors.SteelBlue,
                                        OxyColors.SpringGreen, OxyColors.SlateGray, OxyColors.SlateBlue, OxyColors.SkyBlue,
                                        OxyColors.Silver, OxyColors.Sienna, OxyColors.SeaShell, OxyColors.SeaGreen, OxyColors.SandyBrown,
                                        OxyColors.Salmon, OxyColors.SaddleBrown, OxyColors.RoyalBlue, OxyColors.RosyBrown, OxyColors.YellowGreen,
                                        OxyColors.Purple, OxyColors.PowderBlue, OxyColors.Plum, OxyColors.Pink, OxyColors.Peru,
                                        OxyColors.PeachPuff, OxyColors.PapayaWhip, OxyColors.PaleVioletRed, OxyColors.PaleTurquoise,
                                        OxyColors.PaleGreen, OxyColors.PaleGoldenrod, OxyColors.Orchid, OxyColors.OrangeRed, OxyColors.Orange,
                                        OxyColors.OliveDrab, OxyColors.Olive, OxyColors.OldLace, OxyColors.Navy, OxyColors.Moccasin,
                                        OxyColors.MistyRose, OxyColors.MintCream, OxyColors.MidnightBlue, OxyColors.MediumVioletRed,
                                        OxyColors.Beige, OxyColors.Yellow, OxyColors.Wheat, OxyColors.Violet};
        private int indexOfColor;
        public Controller Con { get; set; }
        public string TitleOfWindow { get; set; }
        public string Title { get; set; }

        public GraphicViewModel(Controller con, string title)
        {
            this.Con = con;
            this.Title = title;

            MyModel = new PlotModel();
            MyModel.PlotType = PlotType.XY;
            MyModel.LegendTitle = "Legend";
            MyModel.LegendOrientation = LegendOrientation.Horizontal;
            MyModel.LegendPlacement = LegendPlacement.Outside;
            MyModel.LegendPosition = LegendPosition.TopRight;
            MyModel.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            MyModel.LegendBorder = OxyColors.Black;

            LinearAxis valueAxis = new LinearAxis { Title = "Value Axis", Position = AxisPosition.Left };
            DateTimeAxis dateAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "dd.MM.yyyy.",
                Title = "Date Axis",
                //IntervalLength = 75,
                //MinorIntervalType = DateTimeIntervalType.Days,
                //IntervalType = DateTimeIntervalType.Days,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None 
            };

            MyModel.Axes.Add(valueAxis);
            MyModel.Axes.Add(dateAxis);


            /*for (int i = 0; i < colors.Length; i++)
            {
                addSeries(""+i);
            
            }

            LineSeries series1 = (LineSeries) MyModel.Series[0];
            series1.Points.Add(new DataPoint(0, 0));
            series1.Points.Add(new DataPoint(10, 18));
            series1.Points.Add(new DataPoint(20, 12));
            series1.Points.Add(new DataPoint(30, 8));
            series1.Points.Add(new DataPoint(40, 15));*/


            indexOfColor = 0;

            //MessageBox.Show("" + MyModel.Series.Count, "Uspesno odradjeno");
            /*this.MyModel = new PlotModel { Title = "Example 1" };
            this.MyModel.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));*/
        }

        public bool addSeries(string title)
        {

            if (title.Equals("")) return false;

            if (contains(title) || !Con.existsSymbol(title) )
            {
                return false;   
            }


            LineSeries ls = new LineSeries();
            ls.Title = title;
            ls.MarkerType = MarkerType.Circle;
            ls.Color = colors[indexOfColor];

            indexOfColor = (indexOfColor + 1) % (colors.Length - 1);

            MyModel.Series.Add(ls);

            return true; 
        }

        public void addSeriesWithOutCheck(string title)
        {
            LineSeries ls = new LineSeries();
            ls.Title = title;
            ls.MarkerType = MarkerType.Circle;
            ls.Color = colors[indexOfColor];

            //indexOfColor = (indexOfColor + 1) % (colors.Length - 1);

            MyModel.Series.Add(ls);
        }

        public void addAllSeries()
        {
            LineSeries lsOpen = new LineSeries();
            lsOpen.Title = "OPEN";
            lsOpen.MarkerType = MarkerType.Circle;
            lsOpen.Color = colors[indexOfColor];

            indexOfColor = (indexOfColor + 1) % (colors.Length - 1);

            LineSeries lsHigh = new LineSeries();
            lsHigh.Title = "HIGH";
            lsHigh.MarkerType = MarkerType.Circle;
            lsHigh.Color = colors[indexOfColor];

            indexOfColor = (indexOfColor + 1) % (colors.Length - 1);

            LineSeries lsLow = new LineSeries();
            lsLow.Title = "LOW";
            lsLow.MarkerType = MarkerType.Circle;
            lsLow.Color = colors[indexOfColor];

            indexOfColor = (indexOfColor + 1) % (colors.Length - 1);

            LineSeries lsClose = new LineSeries();
            lsClose.Title = "CLOSE";
            lsClose.MarkerType = MarkerType.Circle;
            lsClose.Color = colors[indexOfColor];

            indexOfColor = (indexOfColor + 1) % (colors.Length - 1);

            MyModel.Series.Add(lsOpen);
            MyModel.Series.Add(lsHigh);
            MyModel.Series.Add(lsLow);
            MyModel.Series.Add(lsClose);

        }

        public bool contains(string title)
        {
            foreach (Series s in MyModel.Series)
            {
                if (s.Title.Equals(title))
                {
                    return true;
                }
            }

            return false;
        }

        public LineSeries getSeries(string title)
        {
            foreach (Series s in MyModel.Series)
            {
                if (s.Title.Equals(title))
                {
                    return (LineSeries) s;
                }
            }

            return null;
        }

        public List<string> getAllSeriesTitles()
        {
            List<string> seriesTitles = new List<string>();

            foreach (Series s in MyModel.Series)
            {
                seriesTitles.Add(s.Title);
            }

            return seriesTitles;
        }

        public bool addPoints(string titleSeries, List<DataPoint> dataPoints)
        {
            LineSeries s = getSeries(titleSeries);
            if (s == null)
            {
                return false;
            }

            s.Points.AddRange(dataPoints);

            return true;
        }

        public void clearAllSeries()
        {
            MyModel.Series.Clear();
        }

        public void clearAllPoints()
        {
            foreach (Series s in MyModel.Series)
            {
                LineSeries ls = (LineSeries)s;
                ls.Points.Clear();
            }
        }
    }
}
﻿using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ObservableCollection<string> Series;

        public GraphicViewModel(Controller con, string title)
        {
            this.Con = con;
            this.Title = title;

            Series = new ObservableCollection<string>();

            MyModel = new PlotModel();
            MyModel.PlotType = PlotType.XY;
            MyModel.LegendTitle = "Legend";
            MyModel.LegendOrientation = LegendOrientation.Horizontal;
            MyModel.LegendPlacement = LegendPlacement.Outside;
            MyModel.LegendPosition = LegendPosition.TopLeft;
            MyModel.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            MyModel.LegendBorder = OxyColors.Black;
            MyModel.LegendPadding = 0;
            MyModel.LegendItemSpacing = 0;
            MyModel.LegendSymbolLength = 20;
            MyModel.LegendTitleFontSize = 0;
            MyModel.LegendSymbolMargin = 0;
  

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

        public bool addSeries(string title, string type)
        {

            if (title.Equals("")) return false;

            if (contains(title) || (type.Equals("SHARES") && !Con.existsShare(title)))
            {
                return false;   
            }
            
            if(title.Contains("__"))
            {
                string[] tokens = title.Split(new string[] { "__" }, System.StringSplitOptions.None);
                string symbol = tokens[0];
                string market = tokens[1];
                if (type.Equals("CRYPTO CURRENCIES") && (!Con.existsCrypto(symbol) || !Con.existsCurr(market)))
                {
                    return false;
                }
            }

            LineSeries ls = new LineSeries();
            ls.Title = title;
            ls.MarkerType = MarkerType.Circle;
            ls.Color = colors[indexOfColor];

            indexOfColor = (indexOfColor + 1) % (colors.Length - 1);

            MyModel.Series.Add(ls);
            Series.Add(title);

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

        public void addAllSeries(string[] titles)
        {
            for(int i = 0; i < titles.Length-1; i++)
            {
                LineSeries ls = new LineSeries();
                ls.Title = titles[i].ToUpper();
                ls.MarkerType = MarkerType.Circle;
                ls.Color = colors[indexOfColor];

                indexOfColor = (indexOfColor + 1) % (colors.Length - 1);

                MyModel.Series.Add(ls);
            }

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

            for (int i = 0; i < MyModel.Series.Count; i++)
            {
                seriesTitles.Add(MyModel.Series[i].Title);
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
            Series.Clear();
        }

        public void removeSeries(string title)
        {
            Series s = getSeries(title);
            MyModel.Series.Remove(s);
            Series.Remove(title);

            indexOfColor--;
        }

        public void removeSeries(int index)
        {
            MyModel.Series.RemoveAt(index);
            //Series.Remove(title);

            indexOfColor--;
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
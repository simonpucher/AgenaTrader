using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using AgenaTrader.API;
using AgenaTrader.Custom;
using AgenaTrader.Plugins;
using AgenaTrader.Helper;

/// <summary>
/// Version: 1.3.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// This indicator draws the day lines from previous days.
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
[Description("Draws days lines from previous day.")]
   [TimeFrameRequirements("1 Day")]
    public class DayLines_Previous_Indicator : UserIndicator
	{
		#region Variables

        private int _howmanydays = 3;
        private bool _extendlines = true;
        //private bool _showcurrentday = false;
        private bool _showlines = true;

        private bool _showopen = true;
        private bool _showhigh = true;
        private bool _showlow = true;
        private bool _showclose = true;

        private int _linewidth_O = 2;
        private DashStyle _linestyle_O = DashStyle.Solid;
        private Color _col_O = Color.Fuchsia;

        private int _linewidth_H = 2;
        private DashStyle _linestyle_H = DashStyle.Solid;
        private Color _col_H = Color.Green;

        private int _linewidth_L = 2;
        private DashStyle _linestyle_L = DashStyle.Solid;
        private Color _col_L = Color.OrangeRed;

        private int _linewidth_C = 2;
        private DashStyle _linestyle_C = DashStyle.Solid;
        private Color _col_C = Color.Goldenrod;

        //internal
        private IList<DateTime> _alldays = null;

        #endregion

        protected override void OnBarsRequirements()
        {
            Add(new TimeFrame(DatafeedHistoryPeriodicity.Day, 1));
        }


        protected override void OnInit()
		{
            IsOverlay = true;
			CalculateOnClosedBar = true;
            IsShowPriceMarkers = false;
            IsShowInDataBox = false;
        }


        

        protected override void OnCalculate()
        {
            TimeFrame tf = (TimeFrame)Bars.TimeFrame;
            if (Bars != null && Bars.Count() > 0 && Times != null && Times.Count > 0 && this.IsProcessingBarIndexLast &&
                (tf.Periodicity != DatafeedHistoryPeriodicity.Year
                && tf.Periodicity != DatafeedHistoryPeriodicity.Day && tf.Periodicity != DatafeedHistoryPeriodicity.Week))
            {
  

                for (int i = 0; i < this.HowManyDays; i++)
                {
                    if (_showopen)
                    {
                        AddChartLine("Open_" + i.ToString(), this.IsAutoAdjustableScale, Times[1][i], Opens[1][i], Times[0][0], Opens[1][i], this.Color_O, this.DashStyle_O, this.LineWidth_O);
                    }
                    if (_showhigh)
                    {
                        AddChartLine("High_" + i.ToString(), this.IsAutoAdjustableScale, Times[1][i], Highs[1][i], Times[0][0], Highs[1][i], this.Color_H, this.DashStyle_H, this.LineWidth_H);
                    }
                    if (_showlow)
                    {
                        AddChartLine("Low_" + i.ToString(), this.IsAutoAdjustableScale, Times[1][i], Lows[1][i], Times[0][0], Lows[1][i], this.Color_L, this.DashStyle_L, this.LineWidth_L);
                    }
                    if (_showclose)
                    {
                        AddChartLine("Close_" + i.ToString(), this.IsAutoAdjustableScale, Times[1][i], Closes[1][i], Times[0][0], Closes[1][i], this.Color_C, this.DashStyle_C, this.LineWidth_C);
                    }


                    if (_showlines)
                    {
                        DateTime enddrawing_string = Times[0][0].AddSeconds(this.TimeFrame.GetSeconds() + this.TimeFrame.GetSeconds() * 0.15);
                        if (_showopen)
                        {
                            AddChartText("String_Open_" + i.ToString(), this.IsAutoAdjustableScale, "O " + (i + 1).ToString(), enddrawing_string, Opens[1][i], 0, this.Color_O, new Font("Arial", 7.5f), StringAlignment.Far, Color.Transparent, Color.Transparent, 100);
                        }
                        if (_showhigh)
                        {
                            AddChartText("String_High_" + i.ToString(), this.IsAutoAdjustableScale, "H " + (i + 1).ToString(), enddrawing_string, Highs[1][i], 0, this.Color_H, new Font("Arial", 7.5f), StringAlignment.Far, Color.Transparent, Color.Transparent, 100);
                        }
                        if (_showlow)
                        {
                            AddChartText("String_Low_" + i.ToString(), this.IsAutoAdjustableScale, "L " + (i + 1).ToString(), enddrawing_string, Lows[1][i], 0, this.Color_L, new Font("Arial", 7.5f), StringAlignment.Far, Color.Transparent, Color.Transparent, 100);
                        }
                        if (_showclose)
                        {
                            AddChartText("String_Close_" + i.ToString(), this.IsAutoAdjustableScale, "C " + (i + 1).ToString(), enddrawing_string, Closes[1][i], 0, this.Color_C, new Font("Arial", 7.5f), StringAlignment.Far, Color.Transparent, Color.Transparent, 100);
                        }
                    }
                }
            }
        }


        public override string DisplayName
        {
            get
            {
                return "DayLines Previous (I)";
            }
        }


        public override string ToString()
        {
            return "DayLines Previous (I)";
        }

        #region Properties

        [Description("Days")]
        [InputParameter]
        [DisplayName("Days")]
        public int HowManyDays
        {
            get { return _howmanydays; }
            set { _howmanydays = value; }
        }

        [Description("Lines will be drawn till latest candle.")]
        [InputParameter]
        [DisplayName("Extend lines")]
        public bool ExtendLines
        {
            get { return _extendlines; }
            set { _extendlines = value; }
        }


        [Description("Names of the lines will be shown next to the line.")]
        [InputParameter]
        [DisplayName("Shows Names")]
        public bool ShowLines
        {
            get { return _showlines; }
            set { _showlines = value; }
        }

        [Description("If true the open lines are shown.")]
        [InputParameter]
        [DisplayName("Show Open lines")]
        public bool ShowOpen
        {
            get { return _showopen; }
            set { _showopen = value; }
        }

        [Description("If true the high lines are shown.")]
        [InputParameter]
        [DisplayName("Show High lines")]
        public bool ShowHigh
        {
            get { return _showhigh; }
            set { _showhigh = value; }
        }

        [Description("If true the low lines are shown.")]
        [InputParameter]
        [DisplayName("Show Low lines")]
        public bool ShowLow
        {
            get { return _showlow; }
            set { _showlow = value; }
        }

        [Description("If true the close lines are shown.")]
        [InputParameter]
        [DisplayName("Show Close lines")]
        public bool ShowClose
        {
            get { return _showclose; }
            set { _showclose = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Line Width of Open.")]
        [Category("Plots")]
        [DisplayName("LW Open")]
        public int LineWidth_O
        {
            get { return _linewidth_O; }
            set { _linewidth_O = Math.Max(1, value); }
        }

       
        /// <summary>
        /// </summary>
        [Description("DashStyle for Open.")]
        [Category("Plots")]
        [DisplayName("DS Open")]
        public DashStyle DashStyle_O
        {
            get { return _linestyle_O; }
            set { _linestyle_O = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Color for Open")]
        [Category("Plots")]
        [DisplayName("Color Open")]
        public Color Color_O
        {
            get { return _col_O; }
            set { _col_O = value; }
        }

        [Browsable(false)]
        public string Color_O_Serialize
        {
            get { return SerializableColor.ToString(_col_O); }
            set { _col_O = SerializableColor.FromString(value); }
        }


        /// <summary>
        /// </summary>
        [Description("Line Width of High.")]
        [Category("Plots")]
        [DisplayName("LW High")]
        public int LineWidth_H
        {
            get { return _linewidth_H; }
            set { _linewidth_H = Math.Max(1, value); }
        }


        /// <summary>
        /// </summary>
        [Description("DashStyle for High.")]
        [Category("Plots")]
        [DisplayName("DS High")]
        public DashStyle DashStyle_H
        {
            get { return _linestyle_H; }
            set { _linestyle_H = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Color for High")]
        [Category("Plots")]
        [DisplayName("Color High")]
        public Color Color_H
        {
            get { return _col_H; }
            set { _col_H = value; }
        }

        [Browsable(false)]
        public string Color_H_Serialize
        {
            get { return SerializableColor.ToString(_col_H); }
            set { _col_H = SerializableColor.FromString(value); }
        }


        //

        /// <summary>
        /// </summary>
        [Description("Line Width of Low.")]
        [Category("Plots")]
        [DisplayName("LW Low")]
        public int LineWidth_L
        {
            get { return _linewidth_L; }
            set { _linewidth_L = Math.Max(1, value); }
        }


        /// <summary>
        /// </summary>
        [Description("DashStyle for Low.")]
        [Category("Plots")]
        [DisplayName("DS Low")]
        public DashStyle DashStyle_L
        {
            get { return _linestyle_L; }
            set { _linestyle_L = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Color for Low")]
        [Category("Plots")]
        [DisplayName("Color Low")]
        public Color Color_L
        {
            get { return _col_L; }
            set { _col_L = value; }
        }

        [Browsable(false)]
        public string Color_L_Serialize
        {
            get { return SerializableColor.ToString(_col_L); }
            set { _col_L = SerializableColor.FromString(value); }
        }

        
        /// <summary>
        /// </summary>
        [Description("Line Width of Close.")]
        [Category("Plots")]
        [DisplayName("LW Close")]
        public int LineWidth_C
        {
            get { return _linewidth_C; }
            set { _linewidth_C = Math.Max(1, value); }
        }


        /// <summary>
        /// </summary>
        [Description("DashStyle for Close.")]
        [Category("Plots")]
        [DisplayName("DS Close")]
        public DashStyle DashStyle_C
        {
            get { return _linestyle_C; }
            set { _linestyle_C = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Color for Close")]
        [Category("Plots")]
        [DisplayName("Color Close")]
        public Color Color_C
        {
            get { return _col_C; }
            set { _col_C = value; }
        }

        [Browsable(false)]
        public string Color_C_Serialize
        {
            get { return SerializableColor.ToString(_col_C); }
            set { _col_C = SerializableColor.FromString(value); }
        }



        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Indicator_Curve_Open
        {
            get { return Outputs[0]; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Indicator_Curve_High
        {
            get { return Outputs[1]; }
        }


        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Indicator_Curve_Low
        {
            get { return Outputs[2]; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Indicator_Curve_Close
        {
            get { return Outputs[3]; }
        }


        #endregion
    }
}
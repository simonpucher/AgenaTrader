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
/// Version: 1.1
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// This indicator draws the day lines from the current trading day.
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("Draws days lines from the current day.")]
   [TimeFrameRequirements("1 Day")]
    public class DayLines_Current_Indicator : UserIndicator
	{
		#region Variables

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

        #endregion

        protected override void InitRequirements()
        {
            Add(new TimeFrame(DatafeedHistoryPeriodicity.Day, 1));
        }


        protected override void Initialize()
		{
            //Output Listen
            //Add(new Plot(new Pen(this.Color_O, this.LineWidth_O), PlotStyle.Line, "Plot_Open"));
            //Add(new Plot(new Pen(this.Color_H, this.LineWidth_H), PlotStyle.Line, "Plot_High"));
            //Add(new Plot(new Pen(this.Color_L, this.LineWidth_L), PlotStyle.Line, "Plot_Low"));
            //Add(new Plot(new Pen(this.Color_C, this.LineWidth_C), PlotStyle.Line, "Plot_Close"));

            Overlay = true;
			CalculateOnBarClose = false; //do not change this
            PaintPriceMarkers = false;
            DisplayInDataBox = false;

        }




        protected override void OnBarUpdate()
        {
            TimeFrame tf = (TimeFrame)Bars.TimeFrame;
            if (this.IsCurrentBarLast && (tf.Periodicity != DatafeedHistoryPeriodicity.Year && tf.Periodicity != DatafeedHistoryPeriodicity.Day && tf.Periodicity != DatafeedHistoryPeriodicity.Week))
            {
                
                DateTime datetillend = Bars.Where(x => x.Time.Date == Times[0][0].Date).Last().Time;
                DateTime date = Times[1][0];

                    //if (!_extendlines)
                    //{
                    //    IEnumerable<IBar> lisdateend = Bars.Where(x => x.Time.Date == date.Date);
                    //    datetillend = date.Date.AddDays(1).AddSeconds(-1);
                    //    if (lisdateend != null && lisdateend.Count() > 0)
                    //    {
                    //        datetillend = Bars.Where(x => x.Time.Date == date.Date).Last().Time;
                    //    }
                    //}

                    IEnumerable<IBar> list = Bars.Where(x => x.Time.Date == date.Date);
                    DateTime startdate = date.Date;
                    if (list != null && list.Count() > 0)
                    {
                        startdate = list.First().Time;
                    }

                    if (_showopen)
                    {
                        DrawLine("Open_" + date.ToString(), this.AutoScale, startdate, Opens[1][0], datetillend, Opens[1][0], this.Color_O, this.DashStyle_O, this.LineWidth_O);
                    }
                    if (_showhigh)
                    {
                        DrawLine("High_" + date.ToString(), this.AutoScale, startdate, Highs[1][0], datetillend, Highs[1][0], this.Color_H, this.DashStyle_H, this.LineWidth_H);
                    }
                    if (_showlow)
                    {
                        DrawLine("Low_" + date.ToString(), this.AutoScale, startdate, Lows[1][0], datetillend, Lows[1][0], this.Color_L, this.DashStyle_L, this.LineWidth_L);
                    }
                    if (_showclose)
                    {
                        DrawLine("Close_" + date.ToString(), this.AutoScale, startdate, Closes[1][0], datetillend, Closes[1][0], this.Color_C, this.DashStyle_C, this.LineWidth_C);
                    }


                    if (_showlines)
                    {
                        DateTime enddrawing_string = datetillend.AddSeconds(this.TimeFrame.GetSeconds() + this.TimeFrame.GetSeconds() * 0.15);
                        if (_showopen)
                        {
                            DrawText("String_Open_" + date.ToString(), this.AutoScale, "CDO", enddrawing_string, Opens[1][0], 0, this.Color_O, new Font("Arial", 7.5f), StringAlignment.Far, Color.Transparent, Color.Transparent, 100);
                        }
                        if (_showhigh)
                        {
                            DrawText("String_High_" + date.ToString(), this.AutoScale, "CDH", enddrawing_string, Highs[1][0], 0, this.Color_H, new Font("Arial", 7.5f), StringAlignment.Far, Color.Transparent, Color.Transparent, 100);
                        }
                        if (_showlow)
                        {
                            DrawText("String_Low_" + date.ToString(), this.AutoScale, "CDL", enddrawing_string, Lows[1][0], 0, this.Color_L, new Font("Arial", 7.5f), StringAlignment.Far, Color.Transparent, Color.Transparent, 100);
                        }
                        if (_showclose)
                        {
                            DrawText("String_Close_" + date.ToString(), this.AutoScale, "CDC", enddrawing_string, Closes[1][0], 0, this.Color_C, new Font("Arial", 7.5f), StringAlignment.Far, Color.Transparent, Color.Transparent, 100);
                        }
                    }

              

                  


            }

            ////Output Listen
            //if (_showindicatorlines)
            //{
            //    this.Indicator_Curve_Open.Set(Opens[1][0]);
            //    this.Indicator_Curve_High.Set(Highs[1][0]);
            //    this.Indicator_Curve_Low.Set(Lows[1][0]);
            //    this.Indicator_Curve_Close.Set(Closes[1][0]);
            //}
        }


        public override string DisplayName
        {
            get
            {
                return "DayLines Current (I)";
            }
        }


        public override string ToString()
        {
            return "DayLines Current (I)";
        }

        #region Properties

     

        [Description("Names of the lines will be shown next to the line.")]
        [Category("Parameters")]
        [DisplayName("Shows Names")]
        public bool ShowLines
        {
            get { return _showlines; }
            set { _showlines = value; }
        }

        [Description("If true the open lines are shown.")]
        [Category("Parameters")]
        [DisplayName("Show Open lines")]
        public bool ShowOpen
        {
            get { return _showopen; }
            set { _showopen = value; }
        }

        [Description("If true the high lines are shown.")]
        [Category("Parameters")]
        [DisplayName("Show High lines")]
        public bool ShowHigh
        {
            get { return _showhigh; }
            set { _showhigh = value; }
        }

        [Description("If true the low lines are shown.")]
        [Category("Parameters")]
        [DisplayName("Show Low lines")]
        public bool ShowLow
        {
            get { return _showlow; }
            set { _showlow = value; }
        }

        [Description("If true the close lines are shown.")]
        [Category("Parameters")]
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
            get { return Values[0]; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Indicator_Curve_High
        {
            get { return Values[1]; }
        }


        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Indicator_Curve_Low
        {
            get { return Values[2]; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Indicator_Curve_Close
        {
            get { return Values[3]; }
        }


        #endregion
    }
}
#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator
	{
		/// <summary>
		/// Draws days lines from the current day.
		/// </summary>
		public DayLines_Current_Indicator DayLines_Current_Indicator(System.Boolean showLines, System.Boolean showOpen, System.Boolean showHigh, System.Boolean showLow, System.Boolean showClose)
        {
			return DayLines_Current_Indicator(Input, showLines, showOpen, showHigh, showLow, showClose);
		}

		/// <summary>
		/// Draws days lines from the current day.
		/// </summary>
		public DayLines_Current_Indicator DayLines_Current_Indicator(IDataSeries input, System.Boolean showLines, System.Boolean showOpen, System.Boolean showHigh, System.Boolean showLow, System.Boolean showClose)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<DayLines_Current_Indicator>(input, i => i.ShowLines == showLines && i.ShowOpen == showOpen && i.ShowHigh == showHigh && i.ShowLow == showLow && i.ShowClose == showClose);

			if (indicator != null)
				return indicator;

			indicator = new DayLines_Current_Indicator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							ShowLines = showLines,
							ShowOpen = showOpen,
							ShowHigh = showHigh,
							ShowLow = showLow,
							ShowClose = showClose
						};
			indicator.SetUp();

			CachedCalculationUnits.AddIndicator2Cache(indicator);

			return indicator;
		}
	}

	#endregion

	#region Strategy

	public partial class UserStrategy
	{
		/// <summary>
		/// Draws days lines from the current day.
		/// </summary>
		public DayLines_Current_Indicator DayLines_Current_Indicator(System.Boolean showLines, System.Boolean showOpen, System.Boolean showHigh, System.Boolean showLow, System.Boolean showClose)
		{
			return LeadIndicator.DayLines_Current_Indicator(Input, showLines, showOpen, showHigh, showLow, showClose);
		}

		/// <summary>
		/// Draws days lines from the current day.
		/// </summary>
		public DayLines_Current_Indicator DayLines_Current_Indicator(IDataSeries input, System.Boolean showLines, System.Boolean showOpen, System.Boolean showHigh, System.Boolean showLow, System.Boolean showClose)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.DayLines_Current_Indicator(input, showLines, showOpen, showHigh, showLow, showClose);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Draws days lines from the current day.
		/// </summary>
		public DayLines_Current_Indicator DayLines_Current_Indicator(System.Boolean showLines, System.Boolean showOpen, System.Boolean showHigh, System.Boolean showLow, System.Boolean showClose)
		{
			return LeadIndicator.DayLines_Current_Indicator(Input, showLines, showOpen, showHigh, showLow, showClose);
		}

		/// <summary>
		/// Draws days lines from the current day.
		/// </summary>
		public DayLines_Current_Indicator DayLines_Current_Indicator(IDataSeries input, System.Boolean showLines, System.Boolean showOpen, System.Boolean showHigh, System.Boolean showLow, System.Boolean showClose)
		{
			return LeadIndicator.DayLines_Current_Indicator(input, showLines, showOpen, showHigh, showLow, showClose);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Draws days lines from the current day.
		/// </summary>
		public DayLines_Current_Indicator DayLines_Current_Indicator(System.Boolean showLines, System.Boolean showOpen, System.Boolean showHigh, System.Boolean showLow, System.Boolean showClose)
		{
			return LeadIndicator.DayLines_Current_Indicator(Input, showLines, showOpen, showHigh, showLow, showClose);
		}

		/// <summary>
		/// Draws days lines from the current day.
		/// </summary>
		public DayLines_Current_Indicator DayLines_Current_Indicator(IDataSeries input, System.Boolean showLines, System.Boolean showOpen, System.Boolean showHigh, System.Boolean showLow, System.Boolean showClose)
		{
			return LeadIndicator.DayLines_Current_Indicator(input, showLines, showOpen, showHigh, showLow, showClose);
		}
	}

	#endregion

}

#endregion
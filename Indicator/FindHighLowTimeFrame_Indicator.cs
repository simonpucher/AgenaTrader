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
/// Version: 1.2.3
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("This indicator finds the high, middle and low value in a dedicated timeframe or the current session.")]
	public class FindHighLowTimeFrame_Indicator : UserIndicator
	{
        //input
        private int _opacity = Const.DefaultOpacity;
        private Color _currentsessionlinecolor = Const.DefaultIndicatorColor;
        private Color _col_timeframe = Const.DefaultIndicatorColor;
        private int _currentsessionlinewidth = Const.DefaultLineWidth_small;
        private DashStyle _currentsessionlinestyle = Const.DefaultIndicatorDashStyle;
        private TimeSpan _tim_start = new TimeSpan(9, 0, 0);
        private TimeSpan _tim_end = new TimeSpan(16, 0, 0);
        private bool _UseDedicatedTimeFrame = false;

        private bool _IsDrawHighLineEnabled = true;
        private bool _IsDrawMiddleLineEnabled = true;
        private bool _IsDrawLowLineEnabled = true;
        private bool _IsDrawAreaplotEnabled = true;

        //output
        private double _lastlow = Double.NaN;
        private double _lasthigh = Double.NaN;
        private double _lastmiddle = Double.NaN;

        //internal
 

		protected override void Initialize()
		{
            CalculateOnBarClose = false;
            Overlay = true;
		}

		protected override void OnBarUpdate()
		{
            if (Bars != null && Bars.Count > 0 && this.IsCurrentBarLast)
            {
                this.calculateanddrawhighlowlines();
            }
		}


        /// <summary>
        /// Calculate and draw the high & low lines.
        /// </summary>
        private void calculateanddrawhighlowlines()
        {
            //Default timeframe is this Session
            DateTime start = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, 0, 0, 0);
            DateTime end = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, 23, 59, 59);

            //Override if we want this
            if (this.UseDedicatedTimeFrame)
            {
                 start = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, this.Time_Start.Hours, this.Time_Start.Minutes, this.Time_Start.Seconds);
                 end = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, this.Time_End.Hours, this.Time_End.Minutes, this.Time_End.Seconds);
            }
           

            //Select all data and find high & low.
            IEnumerable<IBar> list = Bars.Where(x => x.Time >= start).Where(x => x.Time <= end);

            //We save the high and low values in public variables to get access from other scripts
            this.LastLow = list.Where(x => x.Low == list.Min(y => y.Low)).LastOrDefault().Low;
            this.LastHigh = list.Where(x => x.High == list.Max(y => y.High)).LastOrDefault().High;
            this.LastMiddle = this.LastLow + ((this.LastHigh - this.LastLow) / 2);


            //Draw current lines for this day session
            if (Time[0].Date == DateTime.Now.Date)
            {
                if (this.IsDrawLowLineEnabled)
                {
                    DrawHorizontalLine("LowLine" + start.Ticks, true, this.LastLow, this.CurrentSessionLineColor, this.CurrentSessionLineStyle, this.CurrentSessionLineWidth);
                }
                if (this.IsDrawHighLineEnabled)
                {
                    DrawHorizontalLine("HighLine" + start.Ticks, true, this.LastHigh, this.CurrentSessionLineColor, this.CurrentSessionLineStyle, this.CurrentSessionLineWidth);
                }
                if (this.IsDrawMiddleLineEnabled)
                {
                    DrawHorizontalLine("MiddleLine" + start.Ticks, true, this.LastMiddle, this.CurrentSessionLineColor, this.CurrentSessionLineStyle, this.CurrentSessionLineWidth);          
                }
            }

            //Draw a rectangle at the dedicated time frame
            if (this.IsDrawAreaplotEnabled)
            {
                DrawRectangle("HighLowRect" + start.Ticks, true, start, this.LastLow, end, this.LastHigh, this.Color_TimeFrame, this.Color_TimeFrame, this.Opacity);
            }
            //Print(start.ToString() + " - Low: " + this.LastLow + " - High: " + this.LastHigh + " - Middle: " + this.LastMiddle);
        }


        public override string ToString()
        {
            return "FindHighMiddleLowTimeFrame";
        }

        public override string DisplayName
        {
            get
            {
                return "FindHighMiddleLowTimeFrame";
            }
        }



		#region Properties


        #region Input


       

        //[Browsable(false)]
        //[XmlIgnore()]
        //public DataSeries MyPlot1
        //{
        //    get { return Values[0]; }
        //}



        /// <summary>
        /// </summary>
        [Description("The start time of the time frame.")]
        [Category("Parameters")]
        [DisplayName("Start")]
        public TimeSpan Time_Start
        {
            get { return _tim_start; }
            set { _tim_start = value; }
        }
        [Browsable(false)]
        public long Time_StartSerialize
        {
            get { return _tim_start.Ticks; }
            set { _tim_start = new TimeSpan(value); }
        }

        /// <summary>
        /// </summary>
        [Description("The end time of the time frame.")]
        [Category("Parameters")]
        [DisplayName("End")]
        public TimeSpan Time_End
        {
            get { return _tim_end; }
            set { _tim_end = value; }
        }
        [Browsable(false)]
        public long Time_EndSerialize
        {
            get { return _tim_end.Ticks; }
            set { _tim_end = new TimeSpan(value); }
        }


        /// <summary>
        /// </summary>
        [Description("If true the you are able to specify a dedicated timeframe.")]
        [Category("Drawing")]
        [DisplayName("Dedicated TimeFrame")]
        public bool UseDedicatedTimeFrame
        {
            get { return _UseDedicatedTimeFrame; }
            set { _UseDedicatedTimeFrame = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Opacity for Drawing")]
        [Category("Drawing")]
        [DisplayName("Opacity")]
        public int Opacity
        {
            get { return _opacity; }
            set
            {
                if (value >= 1 && value <= 100)
                {
                    _opacity = value;
                }
                else
                {
                    _opacity = Const.DefaultOpacity;
                }
            }
        }


        [XmlIgnore()]
        [Description("Select color for the current session")]
        [Category("Drawing")]
        [DisplayName("Current Session")]
        public Color CurrentSessionLineColor
        {
            get { return _currentsessionlinecolor; }
            set { _currentsessionlinecolor = value; }
        }

        [Browsable(false)]
        public string CurrentSessionLineColorSerialize
        {
            get { return SerializableColor.ToString(_currentsessionlinecolor); }
            set { _currentsessionlinecolor = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Width for the line of the current session.")]
        [Category("Drawing")]
        [DisplayName("Line Width current session")]
        public int CurrentSessionLineWidth
        {
            get { return _currentsessionlinewidth; }
            set { _currentsessionlinewidth = Math.Max(1, value); }
        }


        /// <summary>
        /// </summary>
        [Description("DashStyle for line of the current session.")]
        [Category("Drawing")]
        [DisplayName("Dash Style current session")]
        public DashStyle CurrentSessionLineStyle
        {
            get { return _currentsessionlinestyle; }
            set { _currentsessionlinestyle = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Time Frame Color")]
        [Category("Drawing")]
        [DisplayName("Time Frame")]
        public Color Color_TimeFrame
        {
            get { return _col_timeframe; }
            set { _col_timeframe = value; }
        }

        [Browsable(false)]
        public string Color_TimeFrameSerialize
        {
            get { return SerializableColor.ToString(_col_timeframe); }
            set { _col_timeframe = SerializableColor.FromString(value); }
        }



        /// <summary>
        /// </summary>
        [Description("If true the you will see the high line.")]
        [Category("Drawing")]
        [DisplayName("High Line")]
        public bool IsDrawHighLineEnabled
        {
            get { return _IsDrawHighLineEnabled; }
            set { _IsDrawHighLineEnabled = value; }
        }

        /// <summary>
        /// </summary>
        [Description("If true the you will see the middle line.")]
        [Category("Drawing")]
        [DisplayName("Middle Line")]
        public bool IsDrawMiddleLineEnabled
        {
            get { return _IsDrawMiddleLineEnabled; }
            set { _IsDrawMiddleLineEnabled = value; }
        }

        /// <summary>
        /// </summary>
        [Description("If true the you will see the low line.")]
        [Category("Drawing")]
        [DisplayName("Low Line")]
        public bool IsDrawLowLineEnabled
        {
            get { return _IsDrawLowLineEnabled; }
            set { _IsDrawLowLineEnabled = value; }
        }

        /// <summary>
        /// </summary>
        [Description("If true the you will see an area plot on the chart.")]
        [Category("Drawing")]
        [DisplayName("Area Plot")]
        public bool IsDrawAreaplotEnabled
        {
            get { return _IsDrawAreaplotEnabled; }
            set { _IsDrawAreaplotEnabled = value; }
        }

        #endregion

       


        #region Output


   


        /// <summary>
        /// Last middle value in dedicated time frame.
        /// </summary>
        [Browsable(false)]
        [XmlIgnore()]
        public double LastMiddle
        {
            get { return _lastmiddle; }
            set { _lastmiddle = value; }
        }

            /// <summary>
        /// Last low value in dedicated time frame.
            /// </summary>
            [Browsable(false)]
            [XmlIgnore()]
            public double LastLow
            {
                get { return _lastlow; }
                set { _lastlow = value; }
            }

            /// <summary>
            /// Last high value in dedicated time frame.
            /// </summary>
            [Browsable(false)]
            [XmlIgnore()]
            public double LastHigh
            {
                get { return _lasthigh; }
                set { _lasthigh = value; }
            }

        #endregion

        #endregion
    }
}

#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator : Indicator
	{
		/// <summary>
		/// This indicator finds the high, middle and low value in a dedicated timeframe or the current session.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator(TimeSpan time_Start, TimeSpan time_End)
        {
			return FindHighLowTimeFrame_Indicator(Input, time_Start, time_End);
		}

		/// <summary>
		/// This indicator finds the high, middle and low value in a dedicated timeframe or the current session.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator(IDataSeries input, TimeSpan time_Start, TimeSpan time_End)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<FindHighLowTimeFrame_Indicator>(input, i => i.Time_Start == time_Start && i.Time_End == time_End);

			if (indicator != null)
				return indicator;

			indicator = new FindHighLowTimeFrame_Indicator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							Time_Start = time_Start,
							Time_End = time_End
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
		/// This indicator finds the high, middle and low value in a dedicated timeframe or the current session.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator(TimeSpan time_Start, TimeSpan time_End)
		{
			return LeadIndicator.FindHighLowTimeFrame_Indicator(Input, time_Start, time_End);
		}

		/// <summary>
		/// This indicator finds the high, middle and low value in a dedicated timeframe or the current session.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator(IDataSeries input, TimeSpan time_Start, TimeSpan time_End)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.FindHighLowTimeFrame_Indicator(input, time_Start, time_End);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// This indicator finds the high, middle and low value in a dedicated timeframe or the current session.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator(TimeSpan time_Start, TimeSpan time_End)
		{
			return LeadIndicator.FindHighLowTimeFrame_Indicator(Input, time_Start, time_End);
		}

		/// <summary>
		/// This indicator finds the high, middle and low value in a dedicated timeframe or the current session.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator(IDataSeries input, TimeSpan time_Start, TimeSpan time_End)
		{
			return LeadIndicator.FindHighLowTimeFrame_Indicator(input, time_Start, time_End);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// This indicator finds the high, middle and low value in a dedicated timeframe or the current session.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator(TimeSpan time_Start, TimeSpan time_End)
		{
			return LeadIndicator.FindHighLowTimeFrame_Indicator(Input, time_Start, time_End);
		}

		/// <summary>
		/// This indicator finds the high, middle and low value in a dedicated timeframe or the current session.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator(IDataSeries input, TimeSpan time_Start, TimeSpan time_End)
		{
			return LeadIndicator.FindHighLowTimeFrame_Indicator(input, time_Start, time_End);
		}
	}

	#endregion

}

#endregion

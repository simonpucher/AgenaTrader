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
/// Version: 1.2.2
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
	[Description("This indicator finds the high, middle and low value in a dedicated timeframe.")]
	public class FindHighLowTimeFrame_Indicator : UserIndicator
	{
        //input
        private int _opacity = Const.DefaultOpacity;
        private Color _currentsessionlinecolor = Const.DefaultIndicatorColor;
        private Color _col_timeframe = Const.DefaultIndicatorColor;
        private int _currentsessionlinewidth = Const.DefaultLineWidth_small;
        private DashStyle _currentsessionlinestyle = Const.DefaultIndicatorDashStyle;
        private TimeSpan _tim_start = new TimeSpan(12, 0, 0);
        private TimeSpan _tim_end = new TimeSpan(13, 0, 0);
        private bool _UseDedicatedTimeFrame = false;

        //output
        private double _lastlow = Double.NaN;
        private double _lasthigh = Double.NaN;
        private double _lastmiddle = Double.NaN;

        //internal
        //private DateTime _currentdayofupdate = DateTime.MinValue;


		protected override void Initialize()
		{
			//Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));

            CalculateOnBarClose = true;
            Overlay = false;
		}

		protected override void OnBarUpdate()
		{
			//MyPlot1.Set(Input[0]);

            if (this.IsCurrentBarLast)
            {
                this.calculateanddrawhighlowlines();
            }

            ////new day session is beginning so we need to calculate and redraw the lines
            //if (_currentdayofupdate.Date < Time[0].Date)
            //{
            //    this.calculateanddrawhighlowlines();
            //}


            ////When finished set the last day variable
            ////If we are online during the day session we do not set this variable so we are redrawing and recalculating the current session
            //if (Time[0].Date != DateTime.Now.Date)
            //{
            //    _currentdayofupdate = Time[0].Date;
            //}

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

            //Check if data for timeframe is valid.
            //we need to get sure that data for the whole time frame is available.
            bool isvalidtimeframe = false;
            if (list != null && !list.IsEmpty() && list.First().Time == start)
            {
                isvalidtimeframe = true;
            }

            if (isvalidtimeframe)
            {
                //We save the high and low values in public variables to get access from other scripts
                this.LastLow = list.Where(x => x.Low == list.Min(y => y.Low)).LastOrDefault().Low;
                this.LastHigh = list.Where(x => x.High == list.Max(y => y.High)).LastOrDefault().High;
                this.LastMiddle = this.LastLow + ((this.LastHigh - this.LastLow) / 2);


                //Draw current lines for this day session
                if (Time[0].Date == DateTime.Now.Date)
                {
                    DrawHorizontalLine("LowLine" + start.Ticks, true, this.LastLow, this.CurrentSessionLineColor, this.CurrentSessionLineStyle, this.CurrentSessionLineWidth);
                    DrawHorizontalLine("HighLine" + start.Ticks, true, this.LastHigh, this.CurrentSessionLineColor, this.CurrentSessionLineStyle, this.CurrentSessionLineWidth);
                    DrawHorizontalLine("MiddleLine" + start.Ticks, true, this.LastMiddle, this.CurrentSessionLineColor, this.CurrentSessionLineStyle, this.CurrentSessionLineWidth);
                
                }

                //Draw a rectangle at the dedicated time frame
                DrawRectangle("HighLowRect" + start.Ticks, true, start, this.LastLow, end, this.LastHigh, this.Color_TimeFrame, this.Color_TimeFrame, this.Opacity);
            }

            //Print(start.ToString() + " - Low: " + this.LastLow + " - High: " + this.LastHigh);
        }


        public override string ToString()
        {
            return "FindHighLowTimeFrame";
        }

        public override string DisplayName
        {
            get
            {
                return "FindHighLowTimeFrame";
            }
        }



		#region Properties


        #region Input


        /// <summary>
        /// </summary>
        [Description("Opacity for Drawing")]
        [Category("Parameters")]
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
        [Category("Parameters")]
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
        [Category("Parameters")]
        [DisplayName("Line Width current session")]
        public int CurrentSessionLineWidth
        {
            get { return _currentsessionlinewidth; }
            set { _currentsessionlinewidth = Math.Max(1, value); }
        }


        /// <summary>
        /// </summary>
        [Description("DashStyle for line of the current session.")]
        [Category("Parameters")]
        [DisplayName("Dash Style current session")]
        public DashStyle CurrentSessionLineStyle
        {
            get { return _currentsessionlinestyle; }
            set { _currentsessionlinestyle = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Time Frame Color")]
        [Category("Parameters")]
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
        [Category("Parameters")]
        [DisplayName("Dedicated TimeFrame")]
        public bool UseDedicatedTimeFrame
        {
            get { return _UseDedicatedTimeFrame; }
            set { _UseDedicatedTimeFrame = value; }
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
		/// This indicator finds the high, middle and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator(System.Int32 opacity, Color currentSessionLineColor, System.Int32 currentSessionLineWidth, DashStyle currentSessionLineStyle, Color color_TimeFrame, TimeSpan time_Start, TimeSpan time_End, System.Boolean useDedicatedTimeFrame)
        {
			return FindHighLowTimeFrame_Indicator(Input, opacity, currentSessionLineColor, currentSessionLineWidth, currentSessionLineStyle, color_TimeFrame, time_Start, time_End, useDedicatedTimeFrame);
		}

		/// <summary>
		/// This indicator finds the high, middle and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator(IDataSeries input, System.Int32 opacity, Color currentSessionLineColor, System.Int32 currentSessionLineWidth, DashStyle currentSessionLineStyle, Color color_TimeFrame, TimeSpan time_Start, TimeSpan time_End, System.Boolean useDedicatedTimeFrame)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<FindHighLowTimeFrame_Indicator>(input, i => i.Opacity == opacity && i.CurrentSessionLineColor == currentSessionLineColor && i.CurrentSessionLineWidth == currentSessionLineWidth && i.CurrentSessionLineStyle == currentSessionLineStyle && i.Color_TimeFrame == color_TimeFrame && i.Time_Start == time_Start && i.Time_End == time_End && i.UseDedicatedTimeFrame == useDedicatedTimeFrame);

			if (indicator != null)
				return indicator;

			indicator = new FindHighLowTimeFrame_Indicator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							Opacity = opacity,
							CurrentSessionLineColor = currentSessionLineColor,
							CurrentSessionLineWidth = currentSessionLineWidth,
							CurrentSessionLineStyle = currentSessionLineStyle,
							Color_TimeFrame = color_TimeFrame,
							Time_Start = time_Start,
							Time_End = time_End,
							UseDedicatedTimeFrame = useDedicatedTimeFrame
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
		/// This indicator finds the high, middle and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator(System.Int32 opacity, Color currentSessionLineColor, System.Int32 currentSessionLineWidth, DashStyle currentSessionLineStyle, Color color_TimeFrame, TimeSpan time_Start, TimeSpan time_End, System.Boolean useDedicatedTimeFrame)
		{
			return LeadIndicator.FindHighLowTimeFrame_Indicator(Input, opacity, currentSessionLineColor, currentSessionLineWidth, currentSessionLineStyle, color_TimeFrame, time_Start, time_End, useDedicatedTimeFrame);
		}

		/// <summary>
		/// This indicator finds the high, middle and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator(IDataSeries input, System.Int32 opacity, Color currentSessionLineColor, System.Int32 currentSessionLineWidth, DashStyle currentSessionLineStyle, Color color_TimeFrame, TimeSpan time_Start, TimeSpan time_End, System.Boolean useDedicatedTimeFrame)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.FindHighLowTimeFrame_Indicator(input, opacity, currentSessionLineColor, currentSessionLineWidth, currentSessionLineStyle, color_TimeFrame, time_Start, time_End, useDedicatedTimeFrame);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// This indicator finds the high, middle and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator(System.Int32 opacity, Color currentSessionLineColor, System.Int32 currentSessionLineWidth, DashStyle currentSessionLineStyle, Color color_TimeFrame, TimeSpan time_Start, TimeSpan time_End, System.Boolean useDedicatedTimeFrame)
		{
			return LeadIndicator.FindHighLowTimeFrame_Indicator(Input, opacity, currentSessionLineColor, currentSessionLineWidth, currentSessionLineStyle, color_TimeFrame, time_Start, time_End, useDedicatedTimeFrame);
		}

		/// <summary>
		/// This indicator finds the high, middle and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator(IDataSeries input, System.Int32 opacity, Color currentSessionLineColor, System.Int32 currentSessionLineWidth, DashStyle currentSessionLineStyle, Color color_TimeFrame, TimeSpan time_Start, TimeSpan time_End, System.Boolean useDedicatedTimeFrame)
		{
			return LeadIndicator.FindHighLowTimeFrame_Indicator(input, opacity, currentSessionLineColor, currentSessionLineWidth, currentSessionLineStyle, color_TimeFrame, time_Start, time_End, useDedicatedTimeFrame);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// This indicator finds the high, middle and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator(System.Int32 opacity, Color currentSessionLineColor, System.Int32 currentSessionLineWidth, DashStyle currentSessionLineStyle, Color color_TimeFrame, TimeSpan time_Start, TimeSpan time_End, System.Boolean useDedicatedTimeFrame)
		{
			return LeadIndicator.FindHighLowTimeFrame_Indicator(Input, opacity, currentSessionLineColor, currentSessionLineWidth, currentSessionLineStyle, color_TimeFrame, time_Start, time_End, useDedicatedTimeFrame);
		}

		/// <summary>
		/// This indicator finds the high, middle and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator(IDataSeries input, System.Int32 opacity, Color currentSessionLineColor, System.Int32 currentSessionLineWidth, DashStyle currentSessionLineStyle, Color color_TimeFrame, TimeSpan time_Start, TimeSpan time_End, System.Boolean useDedicatedTimeFrame)
		{
			return LeadIndicator.FindHighLowTimeFrame_Indicator(input, opacity, currentSessionLineColor, currentSessionLineWidth, currentSessionLineStyle, color_TimeFrame, time_Start, time_End, useDedicatedTimeFrame);
		}
	}

	#endregion

}

#endregion

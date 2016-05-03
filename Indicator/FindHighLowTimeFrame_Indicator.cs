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
/// Version: 1.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("This indicator finds the high and low value in a dedicated timeframe.")]
	public class FindHighLowTimeFrame_Indicator : UserIndicator
	{
        //input
        private int _opacity = 70;
        private Color _currentsessionlinecolor = Color.Brown;
        private Color _col_timeframe = Color.Brown;
        private int _currentsessionlinewidth = 1;
        private DashStyle _currentsessionlinestyle = DashStyle.Solid;
        private TimeSpan _tim_start = new TimeSpan(12, 0, 0);
        private TimeSpan _tim_end = new TimeSpan(13, 0, 0);

        //output

        //internal
        private DateTime _currentdayofupdate = DateTime.MinValue;


		protected override void Initialize()
		{
			//Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
			Overlay = true;
			CalculateOnBarClose = true;
		}

		protected override void OnBarUpdate()
		{
			//MyPlot1.Set(Input[0]);

            //new day session is beginning so we need to calculate and redraw the lines
            if (_currentdayofupdate.Date < Time[0].Date)
            {
                this.calculateanddrawhighlowlines();
            }


            //When finished set the last day variable
            //If we are online during the day session we do not set this variable so we are redrawing and recalculating the current session
            if (Time[0].Date != DateTime.Now.Date)
            {
                _currentdayofupdate = Time[0].Date;
            }

		}


        /// <summary>
        /// Calculate and draw the high & low lines.
        /// </summary>
        private void calculateanddrawhighlowlines()
        {

            DateTime start = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, this.Time_Start.Hours, this.Time_Start.Minutes, this.Time_Start.Seconds);
            DateTime end = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, this.Time_End.Hours, this.Time_End.Minutes, this.Time_End.Seconds);

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
                double low = list.Where(x => x.Low == list.Min(y => y.Low)).LastOrDefault().Low;
                double high = list.Where(x => x.High == list.Max(y => y.High)).LastOrDefault().High;

                //Draw current lines for this day session
                if (Time[0].Date == DateTime.Now.Date)
                {
                    DrawHorizontalLine("LowLine" + start.Ticks, true, low, this.CurrentSessionLineColor, DashStyle.Solid, 2);
                    DrawHorizontalLine("HighLine" + start.Ticks, true, high, this.CurrentSessionLineColor, DashStyle.Solid, 2);
                }

                //Draw a rectangle at the dedicated time frame
                DrawRectangle("HighLowRect" + start.Ticks, true, start, low, end, high, this.Color_TimeFrame, this.Color_TimeFrame, this.Opacity);
            }
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
        [Category("Colors")]
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
                    _opacity = 70;
                }
            }
        }


        [XmlIgnore()]
        [Description("Select color for the current session")]
        [Category("Colors")]
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
        [Category("Plots")]
        [DisplayName("Line Width current session")]
        public int CurrentSessionLineWidth
        {
            get { return _currentsessionlinewidth; }
            set { _currentsessionlinewidth = Math.Max(1, value); }
        }


        /// <summary>
        /// </summary>
        [Description("DashStyle for line of the current session.")]
        [Category("Plots")]
        [DisplayName("Dash Style current session")]
        public DashStyle CurrentSessionLineStyle
        {
            get { return _currentsessionlinestyle; }
            set { _currentsessionlinestyle = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Time Frame Color")]
        [Category("Colors")]
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

        #endregion

        //[Browsable(false)]
        //[XmlIgnore()]
        //public DataSeries MyPlot1
        //{
        //    get { return Values[0]; }
        //}



        /// <summary>
        /// </summary>
        [Description("The start time of the time frame.")]
        [Category("TimeSpan")]
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
        [Category("TimeSpan")]
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
		/// This indicator finds the high and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator()
        {
			return FindHighLowTimeFrame_Indicator(Input);
		}

		/// <summary>
		/// This indicator finds the high and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<FindHighLowTimeFrame_Indicator>(input);

			if (indicator != null)
				return indicator;

			indicator = new FindHighLowTimeFrame_Indicator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input
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
		/// This indicator finds the high and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator()
		{
			return LeadIndicator.FindHighLowTimeFrame_Indicator(Input);
		}

		/// <summary>
		/// This indicator finds the high and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator(IDataSeries input)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.FindHighLowTimeFrame_Indicator(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// This indicator finds the high and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator()
		{
			return LeadIndicator.FindHighLowTimeFrame_Indicator(Input);
		}

		/// <summary>
		/// This indicator finds the high and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator(IDataSeries input)
		{
			return LeadIndicator.FindHighLowTimeFrame_Indicator(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// This indicator finds the high and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator()
		{
			return LeadIndicator.FindHighLowTimeFrame_Indicator(Input);
		}

		/// <summary>
		/// This indicator finds the high and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowTimeFrame_Indicator FindHighLowTimeFrame_Indicator(IDataSeries input)
		{
			return LeadIndicator.FindHighLowTimeFrame_Indicator(input);
		}
	}

	#endregion

}

#endregion

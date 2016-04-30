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
using System.Globalization;

/// <summary>
/// Version: in progress
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    /// <summary>
    /// This interface must be used in each ORB indicator, ORB condition and ORB strategy.
    /// </summary>
    interface IORB
    {
        //input
        int ORBMinutes { get; set; }
        Color Color_ORB { get; set; }
        string Color_ORBSerialize { get; set; }
        Color Color_TargetAreaShort { get; set; }
        string Color_TargetAreaShortSerialize { get; set; }
        Color Color_TargetAreaLong { get; set; }
        string Color_TargetAreaLongSerialize { get; set; }
        TimeSpan Time_OpenRangeStartDE { get; set; }
        //TimeSpan Time_OpenRangeEndDE { get; set; }
        TimeSpan Time_OpenRangeStartUS { get; set; }
        //TimeSpan Time_OpenRangeEndUS { get; set; }
        TimeSpan Time_EndOfDay_DE { get; set; }
        TimeSpan Time_EndOfDay_US { get; set; }
        //string EmailAdress { get; set; }
        bool Send_email { get; set; }

        //output


        //internal
        bool IsEmailFunctionActive { get; }
    }


    [Description("ORB Indicator")]
    public class ORB_Indicator : UserIndicator, IORB
    {

        //input
        private Color _currentsessionlinecolor = Color.LightBlue;
        private int _currentsessionlinewidth = Const.DefaultLineWidth;
        private DashStyle _currentsessionlinestyle = DashStyle.Solid;

        private int _opacity = Const.DefaultOpacity;
        private Color _plot1color = Color.Orange;
        private int _plot1width = Const.DefaultLineWidth;
        private DashStyle _plot1dashstyle = DashStyle.Solid;
        private int _orbminutes = Const.DefaultOpenRangeSizeinMinutes;
        private Color _col_orb = Color.LightBlue;
        private Color _col_target_short = Color.PaleVioletRed;
        private Color _col_target_long = Color.PaleGreen;

        private TimeSpan _tim_OpenRangeStartDE = new TimeSpan(9, 0, 0);  
        //private TimeSpan _tim_OpenRangeEndDE = new TimeSpan(10, 15, 0);  

        private TimeSpan _tim_OpenRangeStartUS = new TimeSpan(15, 30, 0);   
        //private TimeSpan _tim_OpenRangeEndUS = new TimeSpan(16, 45, 0);    

        private TimeSpan _tim_EndOfDay_DE = new TimeSpan(17, 30, 0);  
        private TimeSpan _tim_EndOfDay_US = new TimeSpan(22, 00, 0);  

        private bool _send_email = false;

        //output
        private double _rangelow = Double.NaN;
        private double _rangehigh = Double.NaN;

        //internal 
        private IBar long_breakout = null;
        private IBar short_breakout = null;
        private IBar long_target_reached = null;
        private IBar short_target_reached = null;
        private DateTime currentdayofupdate = DateTime.MinValue;
        private ITimePeriod timeperiod = null;


		protected override void Initialize()
		{
            Add(new Plot(new Pen(this.Plot1Color, this.Plot0Width), PlotStyle.Line, "IndicatorPlot1"));
            Overlay = false;
            CalculateOnBarClose = true;
		}

        protected override void InitRequirements()
        {
            //Print("InitRequirements");

            //Add(DatafeedHistoryPeriodicity.Minute, 1);
        }

        protected override void OnStartUp()
        {
            //Print("OnStartUp");

            //IExchangeDescription exdescrip = this.Root.Core.MarketplaceManager.GetExchangeDescription(this.Instrument.Exchange);
            //Print(exdescrip.ExtentedTradingHours);
            //Print(exdescrip.TradingHours);

            timeperiod = this.Root.Core.MarketplaceManager.GetExchangeDescription(this.Instrument.Exchange).TradingHours;

            ////Check if datafeed periodicity is the right one for this indicator
            //if (this.DatafeedPeriodicityIsValid)
            //{
            //    //ok
            //}
            //else {
            //    Log(Const.DefaultStringDatafeedPeriodicity, InfoLogLevel.Warning);
            //}
        }

		protected override void OnBarUpdate()
		{
            if (this.DatafeedPeriodicityIsValid)
            {
               //new day session is beginning so we need to calculate the open range breakout
                if (currentdayofupdate.Date < Time[0].Date) 
                {
                    //reset session day data
                    this.long_breakout = null;
                    this.short_breakout = null;

                    //draw the open range
                    calculateanddrawOpenRange();
                }


                //Set the indicator value on each bar update
                if (long_breakout != null && long_breakout.Time == Bars[0].Time)
                {
                    BarColor = Color.Turquoise;
                    Value.Set(1);
                }
                else if (short_breakout != null && short_breakout.Time == Bars[0].Time)
                {
                    BarColor = Color.Purple;
                    Value.Set(-1);
                }
                else
                {
                    Value.Set(0);
                }

                //Set the color
                PlotColors[0][0] = this.Plot1Color;
                Plots[0].PenStyle = this.Dash0Style;
                Plots[0].Pen.Width = this.Plot0Width;


                //When finished set the last day variable
                //If we are online during the day session we do not set this variable so we áre redrawing and recalculating the current session 
                if (Time[0].Date != DateTime.Now.Date)
                {
                    currentdayofupdate = Time[0].Date;   
                }
            }
            else
            {
                //Data feed perodicity is not valid, print info in chart panel 
                if (IsCurrentBarLast)
                {
                    DrawTextFixed("AlertText", Const.DefaultStringDatafeedPeriodicity, TextPosition.Center, Color.Red, new Font("Arial", 30), Color.Red, Color.Red, 20);
                }
            }
        }


        /// <summary>
        /// Draws the open range per day.
        /// </summary>
        private void calculateanddrawOpenRange() {

            DateTime start = this.getOpenRangeStart(Time[0]);
            DateTime start_date = start.Date;
            DateTime end = this.getOpenRangeEnd(start);

            //Select all data and find high & low.
            IEnumerable<IBar> list = Bars.Where(x => x.Time >= start).Where(x => x.Time <= end);

            //Check if data for open range is valid.
            //we need to ignore the first day which is normally invalid.
            bool isvalidORB = false;
            if (list != null && !list.IsEmpty() && list.First().Time == start)
            {
                isvalidORB = true;
            }

            if (isvalidORB)
            {
                this.RangeLow = list.Where(x => x.Low == list.Min(y => y.Low)).LastOrDefault().Low;
                this.RangeHigh = list.Where(x => x.High == list.Max(y => y.High)).LastOrDefault().High;

                DrawRectangle("ORBRect" + start_date.Ticks, true, start, this.RangeLow, end, this.RangeHigh, this.Color_ORB, this.Color_ORB, this.Opacity);
                DrawText("ORBRangeString" + start_date.Ticks, true, Math.Round((this.RangeHeight), 2).ToString(), start, this.RangeHigh, 9, Color.Black, new Font("Arial", 9), StringAlignment.Center, Color.Gray, this.Color_ORB, this.Opacity);

                //if we are live on the trading day
                if (DateTime.Now.Date == start_date)
                {
                    DrawHorizontalLine("LowLine" + start_date.Ticks, true, this.RangeLow, this.CurrentSessionLineColor, this.CurrentSessionLineStyle, this.CurrentSessionLineWidth);
                    DrawHorizontalLine("HighLine" + start_date.Ticks, true, this.RangeHigh, this.CurrentSessionLineColor, this.CurrentSessionLineStyle, this.CurrentSessionLineWidth);
                    DrawVerticalLine("BeginnSession" + start_date.Ticks, start, this.CurrentSessionLineColor, this.CurrentSessionLineStyle, this.CurrentSessionLineWidth);
                }

                //Targets
                double target_long = this.RangeHigh + this.RangeHeight;
                double target_short = this.RangeLow - this.RangeHeight;
                DrawRectangle("TargetAreaLong" + start_date.Ticks, true, this.getOpenRangeEnd(this.getOpenRangeStart(start_date)), this.RangeHigh, this.getEndOfTradingDay(start_date), target_long, this.Color_TargetAreaLong, this.Color_TargetAreaLong, this.Opacity);
                DrawRectangle("TargetAreaShort" + start_date.Ticks, true, this.getOpenRangeEnd(this.getOpenRangeStart(start_date)), this.RangeLow, this.getEndOfTradingDay(start_date), target_short, this.Color_TargetAreaShort, this.Color_TargetAreaShort, this.Opacity);

                //load the data after the open range
                list = Bars.Where(x => x.Time >= end).Where(x => x.Time <= this.getEndOfTradingDay(start));

                //find the first breakout to the long side
                long_breakout = list.Where(x => x.Close > this.RangeHigh).FirstOrDefault();
                if (long_breakout != null)
                {
                    DrawArrowUp("ArrowLong" + start_date.Ticks, true, long_breakout.Time, long_breakout.Low, Color.Green);
                }

                //find the first breakout to the short side
                short_breakout = list.Where(x => x.Close < this.RangeLow).FirstOrDefault();
                if (short_breakout != null)
                {
                    DrawArrowDown("ArrowShort" + start_date.Ticks, true, short_breakout.Time, short_breakout.High, Color.Red);
                }

                //find the first target to the long side
                long_target_reached = list.Where(x => x.Close > target_long).FirstOrDefault();
                if (long_target_reached != null)
                {
                    DrawArrowDown("ArrowTargetLong" + start_date.Ticks, true, long_target_reached.Time, long_target_reached.High, Color.Red);
                }

                //find the first target to the short side
                short_target_reached = list.Where(x => x.Close < target_short).FirstOrDefault();
                if (short_target_reached != null)
                {
                    DrawArrowUp("ArrowTargetShort" + start_date.Ticks, true, short_target_reached.Time, short_target_reached.Low, Color.Green);
                }
            }
        }



        /// <summary>
        /// Returns the start of the open range on the dedicated date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private DateTime getOpenRangeStart(DateTime date)
        {
            //Use Marketplace-Escort
            DateTime returnvalue = new DateTime(date.Year, date.Month, date.Day, this.timeperiod.StartTime.Hours, this.timeperiod.StartTime.Minutes, this.timeperiod.StartTime.Seconds);

            //Use CFD data
            if (Bars.Instrument.Symbol.Contains("DE.30") || Bars.Instrument.Symbol.Contains("DE-XTB"))
            {
                //return new TimeSpan(9,00,00);
                returnvalue = new DateTime(date.Year, date.Month, date.Day, this._tim_OpenRangeStartDE.Hours, this._tim_OpenRangeStartDE.Minutes, this._tim_OpenRangeStartDE.Seconds);
            }
            else if (Bars.Instrument.Symbol.Contains("US.30") || Bars.Instrument.Symbol.Contains("US-XTB"))
            {
                //return new TimeSpan(15,30,00);
                returnvalue = new DateTime(date.Year, date.Month, date.Day, this._tim_OpenRangeStartUS.Hours, this._tim_OpenRangeStartUS.Minutes, this._tim_OpenRangeStartUS.Seconds);
            }
            return returnvalue;
        }

        /// <summary>
        ///  Returns the end of the open range on the dedicated date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private DateTime getOpenRangeEnd(DateTime date)
        {
            return date.AddMinutes(_orbminutes);
        }

        /// <summary>
        ///  Returns the end of the trading day on the dedicated date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private DateTime getEndOfTradingDay(DateTime date)
        {
            //Use Marketplace-Escort
            DateTime returnvalue = new DateTime(date.Year, date.Month, date.Day, this.timeperiod.EndTime.Hours, this.timeperiod.EndTime.Minutes, this.timeperiod.EndTime.Seconds);

            //Use CFD data
            if (Bars.Instrument.Symbol.Contains("DE.30") || Bars.Instrument.Symbol.Contains("DE-XTB"))
            {
                //return new TimeSpan(9,00,00);
                returnvalue = new DateTime(date.Year, date.Month, date.Day, this.Time_EndOfDay_DE.Hours, this.Time_EndOfDay_DE.Minutes, this.Time_EndOfDay_DE.Seconds);
            }
            else if (Bars.Instrument.Symbol.Contains("US.30") || Bars.Instrument.Symbol.Contains("US-XTB"))
            {
                //return new TimeSpan(15,30,00);
                returnvalue = new DateTime(date.Year, date.Month, date.Day, this._tim_EndOfDay_US.Hours, this._tim_EndOfDay_US.Minutes, this._tim_EndOfDay_US.Seconds);
            }
            return returnvalue;
        }

        /// <summary>
        /// True if the Periodicity of the data feed is correct for this indicator.
        /// </summary>
        /// <returns></returns>
        private bool DatafeedPeriodicityIsValid {
            get {
                TimeFrame tf = (TimeFrame)Bars.TimeFrame;
                if (tf.Periodicity == DatafeedHistoryPeriodicity.Tick || tf.Periodicity == DatafeedHistoryPeriodicity.Second || tf.Periodicity == DatafeedHistoryPeriodicity.Minute)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }



        public override string ToString()
        {
            return "ORB";
        }

        public override string DisplayName
        {
            get
            {
                return "ORB";
            }
        }



		#region Properties

        #region Input


        /// <summary>
        /// </summary>
        [Description("Period in minutes for ORB")]
        [Category("Minutes")]
        [DisplayName("Minutes ORB")]
        public int ORBMinutes
        {
            get { return _orbminutes; }
            set {
                if (value >= 1 && value <= 300)
                {
                    _orbminutes = value;
                }
                else
                {
                    _orbminutes = Const.DefaultOpenRangeSizeinMinutes;
                }
            }
        }
        
        /// <summary>
        /// </summary>
        [Description("Opacity for Drawing")]
        [Category("Colors")]
        [DisplayName("Opacity")]
        public int Opacity
        {
            get { return _opacity; }
            set {
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
        [Description("Open Range Color")]
        [Category("Colors")]
        [DisplayName("Open Range")]
        public Color Color_ORB
        {
            get { return _col_orb; }
            set { _col_orb = value; }
        }

        [Browsable(false)]
        public string Color_ORBSerialize
        {
            get { return SerializableColor.ToString(_col_orb); }
            set { _col_orb = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Select Color TargetAreaShort")]
        [Category("Colors")]
        [DisplayName("TargetAreaShort")]
        public Color Color_TargetAreaShort
        {
            get { return _col_target_short; }
            set { _col_target_short = value; }
        }
        [Browsable(false)]
        public string Color_TargetAreaShortSerialize
        {
            get { return SerializableColor.ToString(_col_target_short); }
            set { _col_target_short = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Select Color TargetAreaLong")]
        [Category("Colors")]
        [DisplayName("TargetAreaLong")]
        public Color Color_TargetAreaLong
        {
            get { return _col_target_long; }
            set { _col_target_long = value; }
        }
        [Browsable(false)]
        public string Color_TargetAreaLongSerialize
        {
            get { return SerializableColor.ToString(_col_target_long); }
            set { _col_target_long = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("OpenRange DE Start: Uhrzeit ab wann Range gemessen wird")]
        [Category("TimeSpan")]
        [DisplayName("OpenRange Start DE")]
        public TimeSpan Time_OpenRangeStartDE
        {
            get { return _tim_OpenRangeStartDE; }
            set { _tim_OpenRangeStartDE = value; }
        }
        [Browsable(false)]
        public long Time_OpenRangeStartDESerialize
        {
            get { return _tim_OpenRangeStartDE.Ticks; }
            set { _tim_OpenRangeStartDE = new TimeSpan(value); }
        }

        ///// <summary>
        ///// </summary>
        //[Description("OpenRange DE End: Uhrzeit wann Range geschlossen wird")]
        //[Category("TimeSpan")]
        //[DisplayName("2. OpenRange End DE")]
        //public TimeSpan Time_OpenRangeEndDE
        //{
        //    get { return _tim_OpenRangeEndDE; }
        //    set { _tim_OpenRangeEndDE = value; }
        //}
        //[Browsable(false)]
        //public long Time_OpenRangeEndDESerialize
        //{
        //    get { return _tim_OpenRangeEndDE.Ticks; }
        //    set { _tim_OpenRangeEndDE = new TimeSpan(value); }
        //}

        /// <summary>
        /// </summary>
        [Description("OpenRange US Start: Uhrzeit ab wann Range gemessen wird")]
        [Category("TimeSpan")]
        [DisplayName("OpenRange Start US")]
        public TimeSpan Time_OpenRangeStartUS
        {
            get { return _tim_OpenRangeStartUS; }
            set { _tim_OpenRangeStartUS = value; }
        }
        [Browsable(false)]
        public long Time_OpenRangeStartUSSerialize
        {
            get { return _tim_OpenRangeStartUS.Ticks; }
            set { _tim_OpenRangeStartUS = new TimeSpan(value); }
        }

        ///// <summary>
        ///// </summary>
        //[Description("OpenRange US End: Uhrzeit wann Range geschlossen wird")]
        //[Category("TimeSpan")]
        //[DisplayName("4. OpenRange End US")]
        //public TimeSpan Time_OpenRangeEndUS
        //{
        //    get { return _tim_OpenRangeEndUS; }
        //    set { _tim_OpenRangeEndUS = value; }
        //}
        //[Browsable(false)]
        //public long Time_OpenRangeEndUSSerialize
        //{
        //    get { return _tim_OpenRangeEndUS.Ticks; }
        //    set { _tim_OpenRangeEndUS = new TimeSpan(value); }
        //}

        /// <summary>
        /// </summary>
        [Description("EndOfDay DE: Uhrzeit spätestens verkauft wird")]
        [Category("TimeSpan")]
        [DisplayName("EndOfDay DE")]
        public TimeSpan Time_EndOfDay_DE
        {
            get { return _tim_EndOfDay_DE; }
            set { _tim_EndOfDay_DE = value; }
        }
        [Browsable(false)]
        public long Time_EndOfDay_DESerialize
        {
            get { return _tim_EndOfDay_DE.Ticks; }
            set { _tim_EndOfDay_DE = new TimeSpan(value); }
        }

        /// <summary>
        /// </summary>
        [Description("EndOfDay US: Uhrzeit spätestens verkauft wird")]
        [Category("TimeSpan")]
        [DisplayName("EndOfDay US")]
        public TimeSpan Time_EndOfDay_US
        {
            get { return _tim_EndOfDay_US; }
            set { _tim_EndOfDay_US = value; }
        }
        [Browsable(false)]
        public long Time_EndOfDay_USSerialize
        {
            get { return _tim_EndOfDay_US.Ticks; }
            set { _tim_EndOfDay_US = new TimeSpan(value); }
        }


        [Description("If true an email will be send on open range breakout.")]
        [Category("Email")]
        [DisplayName("Send email on breakout")]
        public bool Send_email
        {
            get { return _send_email; }
            set { _send_email = value; }
        }


        #region Plotstyle

        [XmlIgnore()]
        [Description("Select Color")]
        [Category("Colors")]
        [DisplayName("ORB Indicator")]
        public Color Plot1Color
        {
            get { return _plot1color; }
            set { _plot1color = value; }
        }

        [Browsable(false)]
        public string Plot1ColorSerialize
        {
            get { return SerializableColor.ToString(_plot1color); }
            set { _plot1color = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Width for Indicator.")]
        [Category("Plots")]
        [DisplayName("Line Width Indicator")]
        public int Plot0Width
        {
            get { return _plot1width; }
            set { _plot1width = Math.Max(1, value); }
        }


        /// <summary>
        /// </summary>
        [Description("DashStyle for Indicator.")]
        [Category("Plots")]
        [DisplayName("Dash Style Indicator")]
        public DashStyle Dash0Style
        {
            get { return _plot1dashstyle; }
            set { _plot1dashstyle = value; }
        }

        #endregion

        #endregion

        #region Output

            [Browsable(false)]
            [XmlIgnore()]
            public DataSeries MyPlot1
            {
                get { return Values[0]; }
            }


            [Browsable(false)]
            [XmlIgnore()]
            public double RangeHeight
            {
                get { return this.RangeHigh - this.RangeLow; }
            }

            [Browsable(false)]
            [XmlIgnore()]
            public double RangeLow
            {
                get { return _rangelow; }
                set { _rangelow = value; }
            }

            [Browsable(false)]
            [XmlIgnore()]
            public double RangeHigh
            {
                get { return _rangehigh; }
                set { _rangehigh = value; }
            }

        #endregion

        #region Internals


            [Browsable(false)]
            public bool IsEmailFunctionActive
            {
                get
                {
                    if (this.Send_email)
                    {
                        return true;
                    }
                    return false;
                }
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
		/// ORB Indicator
		/// </summary>
		public ORB_Indicator ORB_Indicator()
        {
			return ORB_Indicator(Input);
		}

		/// <summary>
		/// ORB Indicator
		/// </summary>
		public ORB_Indicator ORB_Indicator(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<ORB_Indicator>(input);

			if (indicator != null)
				return indicator;

			indicator = new ORB_Indicator
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
		/// ORB Indicator
		/// </summary>
		public ORB_Indicator ORB_Indicator()
		{
			return LeadIndicator.ORB_Indicator(Input);
		}

		/// <summary>
		/// ORB Indicator
		/// </summary>
		public ORB_Indicator ORB_Indicator(IDataSeries input)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.ORB_Indicator(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// ORB Indicator
		/// </summary>
		public ORB_Indicator ORB_Indicator()
		{
			return LeadIndicator.ORB_Indicator(Input);
		}

		/// <summary>
		/// ORB Indicator
		/// </summary>
		public ORB_Indicator ORB_Indicator(IDataSeries input)
		{
			return LeadIndicator.ORB_Indicator(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// ORB Indicator
		/// </summary>
		public ORB_Indicator ORB_Indicator()
		{
			return LeadIndicator.ORB_Indicator(Input);
		}

		/// <summary>
		/// ORB Indicator
		/// </summary>
		public ORB_Indicator ORB_Indicator(IDataSeries input)
		{
			return LeadIndicator.ORB_Indicator(input);
		}
	}

	#endregion

}

#endregion


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
/// ToDo
/// 1)  Customzing für Börsenstart 09.00 oder 15.30
/// 2)  Drawings in Background bringen, aktuell verdecken sie andere Indikatoren wie zB SMA200 -> erledigt mit Opacity
/// 3)  automatische Ordererstellung (http://www.tradeescort.com/phpbb_de/viewtopic.php?f=19&t=2401)
/// 4)  Im 1-Stundenchart wird automatisch das High/Low von dem kompletten Bar genommen. Es wird also die OpenRange von 2 Stunden genommen (120 Mins statt 75) Noch testen!
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
        TimeSpan Time_OpenRangeEndDE { get; set; }
        TimeSpan Time_OpenRangeStartUS { get; set; }
        TimeSpan Time_OpenRangeEndUS { get; set; }
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
        private int _currentsessionlinewidth = 2;
        private DashStyle _currentsessionlinestyle = DashStyle.Solid;

        private Color _plot1color = Color.Orange;
        private int _plot1width = 2;
        private DashStyle _plot1dashstyle = DashStyle.Solid;
        private int _orbminutes = 75;
        private Color _col_orb = Color.LightBlue;
        private Color _col_target_short = Color.PaleVioletRed;
        private Color _col_target_long = Color.PaleGreen;

        private TimeSpan _tim_OpenRangeStartDE = new TimeSpan(9, 0, 0);    //09:00:00   
        private TimeSpan _tim_OpenRangeEndDE = new TimeSpan(10, 15, 0);    //09:00:00   

        private TimeSpan _tim_OpenRangeStartUS = new TimeSpan(15, 30, 0);  //15:30:00   
        private TimeSpan _tim_OpenRangeEndUS = new TimeSpan(16, 45, 0);  //15:30:00   

        private TimeSpan _tim_EndOfDay_DE = new TimeSpan(16, 30, 0);  //16:30:00   
        private TimeSpan _tim_EndOfDay_US = new TimeSpan(21, 30, 0);  //21:30:00

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


		protected override void Initialize()
		{
            Add(new Plot(new Pen(this.Plot1Color, this.Plot0Width), PlotStyle.Line, "IndicatorPlot1"));
            Overlay = false;
            CalculateOnBarClose = true;
		}

        protected override void InitRequirements()
        {
            //Print("InitRequirements");

        }

        protected override void OnStartUp()
        {
            //Print("OnStartUp");
        }

		protected override void OnBarUpdate()
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


        /// <summary>
        /// Draws the open range per day.
        /// </summary>
        private void calculateanddrawOpenRange() {

            //Print(Time[0]);

            DateTime start = Bars.Where(x => x.Time.Date == Bars[0].Time.Date).FirstOrDefault().Time;
            DateTime start_date = start.Date;
            DateTime end = this.getOpenRangeEnd(start);

            //Selektiere alle gültigen Kurse und finde low und high.
            IEnumerable<IBar> list = Bars.Where(x => x.Time >= start).Where(x => x.Time <= end);
            if (list != null && !list.IsEmpty())
            {
                this.RangeLow = list.Where(x => x.Low == list.Min(y => y.Low)).LastOrDefault().Low;
                this.RangeHigh = list.Where(x => x.High == list.Max(y => y.High)).LastOrDefault().High;

                DrawRectangle("ORBRect" + start_date.Ticks, true, start, this.RangeLow, end, this.RangeHigh, this.Color_ORB, this.Color_ORB, 70);
                DrawText("ORBRangeString" + start_date.Ticks, true, Math.Round((this.RangeHeight), 2).ToString(), start, this.RangeHigh, 9, Color.Black, new Font("Arial", 9), StringAlignment.Center, Color.Gray, this.Color_ORB, 70);

                //if we are live on the trading day
                if (DateTime.Now.Date == start_date)
                {
                    DrawHorizontalLine("LowLine" + start_date.Ticks, true, this.RangeLow, this.CurrentSessionLineColor, this.CurrentSessionLineStyle, this.CurrentSessionLineWidth);
                    DrawHorizontalLine("HighLine" + start_date.Ticks, true, this.RangeHigh, this.CurrentSessionLineColor, this.CurrentSessionLineStyle, this.CurrentSessionLineWidth);
                    DrawVerticalLine("BeginnSession" + start_date.Ticks, start_date, this.CurrentSessionLineColor, this.CurrentSessionLineStyle, this.CurrentSessionLineWidth);
                }

                //Targets
                double target_long = this.RangeHigh + this.RangeHeight;
                double target_short = this.RangeLow - this.RangeHeight;
                DrawRectangle("TargetAreaLong" + start_date.Ticks, true, this.getOpenRangeEnd(this.getOpenRangeStart(start_date)), this.RangeHigh, this.getEndOfTradingDay(start_date), target_long, this.Color_TargetAreaLong, this.Color_TargetAreaLong, 70);
                DrawRectangle("TargetAreaShort" + start_date.Ticks, true, this.getOpenRangeEnd(this.getOpenRangeStart(start_date)), this.RangeLow, this.getEndOfTradingDay(start_date), target_short, this.Color_TargetAreaShort, this.Color_TargetAreaShort, 70);

                //load the data after the open range
                list = Bars.Where(x => x.Time >= end).Where(x => x.Time <= this.getEndOfTradingDay(start));

                //find the first breakout to the long side
                long_breakout = list.Where(x => x.Close > this.RangeHigh).FirstOrDefault();
                if (long_breakout != null)
                {
                    DrawArrowUp("ArrowLong" + start_date.Ticks, true, long_breakout.Time, long_breakout.Low - 100 * TickSize, Color.Green);
                }

                //find the first breakout to the short side
                short_breakout = list.Where(x => x.Close < this.RangeLow).FirstOrDefault();
                if (short_breakout != null)
                {
                    DrawArrowDown("ArrowShort" + start_date.Ticks, true, short_breakout.Time, short_breakout.High + 100 * TickSize, Color.Red);
                }

                //find the first target to the long side
                long_target_reached = list.Where(x => x.Close > target_long).FirstOrDefault();
                if (long_target_reached != null)
                {
                    DrawArrowDown("ArrowTargetLong" + start_date.Ticks, true, long_target_reached.Time, long_target_reached.High + 100 * TickSize, Color.Red);
                }

                //find the first target to the short side
                short_target_reached = list.Where(x => x.Close < target_short).FirstOrDefault();
                if (short_target_reached != null)
                {
                    DrawArrowUp("ArrowTargetShort" + start_date.Ticks, true, short_target_reached.Time, short_target_reached.Low - 100 * TickSize, Color.Green);
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
            //Print(this.Instrument.Symbol);

            if (Bars.Instrument.Symbol.Contains("DE.30") || Bars.Instrument.Symbol.Contains("DE-XTB") || Bars.Instrument.Symbol.Contains("DAX.IND"))
            {
                //return new TimeSpan(9,00,00);
                return new DateTime(date.Year, date.Month, date.Day, this._tim_OpenRangeStartDE.Hours, this._tim_OpenRangeStartDE.Minutes, this._tim_OpenRangeStartDE.Seconds);
            }
            else if (Bars.Instrument.Symbol.Contains("US.30") || Bars.Instrument.Symbol.Contains("US-XTB") || Bars.Instrument.Symbol.Contains("DOW.IND") || Bars.Instrument.Symbol.Contains("NDX.IND"))
            {
                //return new TimeSpan(15,30,00);
                return new DateTime(date.Year, date.Month, date.Day, this._tim_OpenRangeStartUS.Hours, this._tim_OpenRangeStartUS.Minutes, this._tim_OpenRangeStartUS.Seconds);
            }
            else
            {
                return new DateTime(date.Year, date.Month, date.Day, this._tim_OpenRangeStartDE.Hours, this._tim_OpenRangeStartDE.Minutes, this._tim_OpenRangeStartDE.Seconds);
            }
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

            if (Bars.Instrument.Symbol.Contains("DE.30") || Bars.Instrument.Symbol.Contains("DE-XTB") || Bars.Instrument.Symbol.Contains("DAX.IND"))
            {
                //return new TimeSpan(9,00,00);
                return new DateTime(date.Year, date.Month, date.Day, this.Time_EndOfDay_DE.Hours, this.Time_EndOfDay_DE.Minutes, this.Time_EndOfDay_DE.Seconds);
            }
            else if (Bars.Instrument.Symbol.Contains("US.30") || Bars.Instrument.Symbol.Contains("US-XTB") || Bars.Instrument.Symbol.Contains("DOW.IND") || Bars.Instrument.Symbol.Contains("NDX.IND"))
            {
                //return new TimeSpan(15,30,00);
                return new DateTime(date.Year, date.Month, date.Day, this._tim_EndOfDay_US.Hours, this._tim_EndOfDay_US.Minutes, this._tim_EndOfDay_US.Seconds);
            }
            else
            {
                return new DateTime(date.Year, date.Month, date.Day, this.Time_EndOfDay_DE.Hours, this.Time_EndOfDay_DE.Minutes, this.Time_EndOfDay_DE.Seconds);
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
            set { _orbminutes = value; }
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
        [DisplayName("1. OpenRange Start DE")]
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

        /// <summary>
        /// </summary>
        [Description("OpenRange DE End: Uhrzeit wann Range geschlossen wird")]
        [Category("TimeSpan")]
        [DisplayName("2. OpenRange End DE")]
        public TimeSpan Time_OpenRangeEndDE
        {
            get { return _tim_OpenRangeEndDE; }
            set { _tim_OpenRangeEndDE = value; }
        }
        [Browsable(false)]
        public long Time_OpenRangeEndDESerialize
        {
            get { return _tim_OpenRangeEndDE.Ticks; }
            set { _tim_OpenRangeEndDE = new TimeSpan(value); }
        }

        /// <summary>
        /// </summary>
        [Description("OpenRange US Start: Uhrzeit ab wann Range gemessen wird")]
        [Category("TimeSpan")]
        [DisplayName("3. OpenRange Start US")]
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

        /// <summary>
        /// </summary>
        [Description("OpenRange US End: Uhrzeit wann Range geschlossen wird")]
        [Category("TimeSpan")]
        [DisplayName("4. OpenRange End US")]
        public TimeSpan Time_OpenRangeEndUS
        {
            get { return _tim_OpenRangeEndUS; }
            set { _tim_OpenRangeEndUS = value; }
        }
        [Browsable(false)]
        public long Time_OpenRangeEndUSSerialize
        {
            get { return _tim_OpenRangeEndUS.Ticks; }
            set { _tim_OpenRangeEndUS = new TimeSpan(value); }
        }

        /// <summary>
        /// </summary>
        [Description("EndOfDay DE: Uhrzeit spätestens verkauft wird")]
        [Category("TimeSpan")]
        [DisplayName("5. EndOfDay DE")]
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
        [DisplayName("5. EndOfDay US")]
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

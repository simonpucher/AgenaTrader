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
using AgenaTrader.Helper.TradingManager;

/// <summary>
/// Version: in progress
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// The initial version of this strategy was inspired by the work of Birger Sch�fermeier: https://www.whselfinvest.at/de/Store_Birger_Schaefermeier_Trading_Strategie_Open_Range_Break_Out.php
/// Further developments are inspired by the work of Mehmet Emre Cekirdekci and Veselin Iliev from the Worcester Polytechnic Institute (2010)
/// Trading System Development: Trading the Opening Range Breakouts https://www.wpi.edu/Pubs/E-project/Available/E-project-042910-142422/unrestricted/Veselin_Iliev_IQP.pdf
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this indicator without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
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
        TimeSpan Time_OpenRangeStartDE { get; set; }
        TimeSpan Time_OpenRangeStartUS { get; set; }
        TimeSpan Time_EndOfDay_DE { get; set; }
        TimeSpan Time_EndOfDay_US { get; set; }
    }

    /// <summary>
    /// Result Object for the Open Range.
    /// </summary>
    public class ORB_Result {

        public bool IsORBValid { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime Start  { get; set; }

        public DateTime StartDate {
            get {
                return Start.Date;
            }
        }

        public DateTime End  { get; set; }

    }


    [Description("Open Range Breakout Indicator")]
    public class ORB_Indicator : UserIndicator, IORB
    {

        //input
        private int _currentsessionlinewidth = Const.DefaultLineWidth;
        private DashStyle _currentsessionlinestyle = DashStyle.Solid;

        private int _opacity = Const.DefaultOpacity;
        private Color _plot1color = Const.DefaultIndicatorColor;
        private int _plot1width = Const.DefaultLineWidth;
        private DashStyle _plot1dashstyle = Const.DefaultIndicatorDashStyle;

        private Color _col_orb = Color.LightBlue;
        private Color _col_orb_invalid = Color.Fuchsia;
        private Color _col_target_short = Color.PaleVioletRed;
        private Color _col_target_long = Color.PaleGreen;

        private int _orbminutes = Const.DefaultOpenRangeSizeinMinutes;
        private TimeSpan _tim_OpenRangeStartDE = new TimeSpan(8, 0, 0);
        private TimeSpan _tim_OpenRangeStartUS = new TimeSpan(14, 30, 0);

        private TimeSpan _tim_EndOfDay_DE = new TimeSpan(17, 30, 0);
        private TimeSpan _tim_EndOfDay_US = new TimeSpan(22, 00, 0);


        //output
        private double _rangelow = Double.MinValue;
        private double _rangehigh = Double.MinValue;
        private double _targetlong = Double.MinValue;
        private double _targetshort = Double.MinValue;

      
        //internal 
        private IBar _long_breakout = null;
        private IBar _short_breakout = null;
        private IBar _long_target_reached = null;
        private IBar _short_target_reached = null;
        private DateTime _currentdayofupdate = DateTime.MinValue;
        private ITimePeriod _timeperiod = null;



        /// <summary>
        /// If we use this indicator from another script we need to initalize all important data first.
        /// </summary>
        public void SetData(IInstrument instrument)
        {
            this.TimePeriod = this.Root.Core.MarketplaceManager.GetExchangeDescription(instrument.Exchange).TradingHours;
        }

		protected override void OnInit()
		{
            Add(new OutputDescriptor(new Pen(this.Plot1Color, this.Plot0Width), OutputSerieDrawStyle.Line, "ORB_IndicatorPlot"));
            IsOverlay = false;
            CalculateOnClosedBar = true;

            //Because of Backtesting reasons if we use the afvanced mode we need at least two bars
            this.RequiredBarsCount = 2;
		}

        protected override void OnBarsRequirements()
        {
            //Print("InitRequirements");
            //Add(DatafeedHistoryPeriodicity.Minute, 1);
        }

        protected override void OnStart()
        {
            //Print("OnStartUp");

            this.TimePeriod = this.Root.Core.MarketplaceManager.GetExchangeDescription(this.Instrument.Exchange).TradingHours;
        }

		protected override void OnCalculate()
		{

            if (this.DatafeedPeriodicityIsValid(Bars))
            {

                //new day session is beginning so we need to calculate the open range breakout
                //if we are calculation "older" trading days the whole day will be calculated because to enhance performance.
                if (this.CurrentdayOfUpdate.Date < Bars[0].Time.Date)
                {

                    ORB_Result resultvalue = calculate(this.Bars, this.Bars[0]);

                    //Draw if the calculate method was processed till the end
                    if (resultvalue.IsCompleted)
                    {
                        //Set Color for open range
                        Color openrangecolorvalidornot = this.Color_ORB;
                        if (!resultvalue.IsORBValid)
                        {
                            openrangecolorvalidornot = this.Color_ORB_invalid;
                        }

                        //Draw the Open Range
                        AddChartRectangle("ORBRect" + resultvalue.StartDate.Ticks, true, resultvalue.Start, this.RangeLow, resultvalue.End, this.RangeHigh, openrangecolorvalidornot, openrangecolorvalidornot, this.Opacity);
                        AddChartText("ORBRangeString" + resultvalue.StartDate.Ticks, true, Math.Round((this.RangeHeight), 2).ToString(), resultvalue.Start, this.RangeHigh, 9, Color.Black, new Font("Arial", 9), StringAlignment.Center, Color.Gray, openrangecolorvalidornot, this.Opacity);

                        //if we are live on the trading day
                        if (this.Bars.Last().Time.Date == resultvalue.StartDate)
                        {
                            AddChartHorizontalLine("LowLine" + resultvalue.StartDate.Ticks, true, this.RangeLow, openrangecolorvalidornot, this.CurrentSessionLineStyle, this.CurrentSessionLineWidth);
                            AddChartHorizontalLine("HighLine" + resultvalue.StartDate.Ticks, true, this.RangeHigh, openrangecolorvalidornot, this.CurrentSessionLineStyle, this.CurrentSessionLineWidth);
                        }

                        //Draw the target areas
                        AddChartRectangle("TargetAreaLong" + resultvalue.StartDate.Ticks, true, this.getOpenRangeEnd(this.getOpenRangeStart(this.Bars, resultvalue.StartDate)), this.RangeHigh, this.getEndOfTradingDay(this.Bars, resultvalue.StartDate), this.TargetLong, this.Color_TargetAreaLong, this.Color_TargetAreaLong, this.Opacity);
                        AddChartRectangle("TargetAreaShort" + resultvalue.StartDate.Ticks, true, this.getOpenRangeEnd(this.getOpenRangeStart(this.Bars, resultvalue.StartDate)), this.RangeLow, this.getEndOfTradingDay(this.Bars, resultvalue.StartDate), this.TargetShort, this.Color_TargetAreaShort, this.Color_TargetAreaShort, this.Opacity);
                    }
                }


                //Set the indicator value on each bar update, if the breakout is on the current bar
                if (this.LongBreakout != null && this.LongBreakout.Time == Bars[0].Time)
                {
                    BarColor = Color.Turquoise;
                    OutSeries.Set(1);
                    AddChartArrowUp("ArrowLong" + Bars[0].Time.Date.Ticks, true, this.LongBreakout.Time, this.LongBreakout.Low, Color.Green);
                }
                else if (this.ShortBreakout != null && this.ShortBreakout.Time == Bars[0].Time)
                {
                    BarColor = Color.Purple;
                    OutSeries.Set(-1);
                    AddChartArrowDown("ArrowShort" + Bars[0].Time.Date.Ticks, true, this.ShortBreakout.Time, this.ShortBreakout.High, Color.Red);
                }
                else
                {
                    OutSeries.Set(0);
                }

                //Draw the Long Target if this is necessary 
                if (this.LongTargetReached != null)
                {
                    AddChartArrowDown("ArrowTargetLong" + Bars[0].Time.Date.Ticks, true, this.LongTargetReached.Time, this.LongTargetReached.High, Color.Red);
                }

                //Draw the Short Target if this is necessary
                if (this.ShortTargetReached != null)
                {
                    AddChartArrowUp("ArrowTargetShort" + Bars[0].Time.Date.Ticks, true, this.ShortTargetReached.Time, this.ShortTargetReached.Low, Color.Green);
                }

                //Set the color
                PlotColors[0][0] = this.Plot1Color;
                OutputDescriptors[0].PenStyle = this.Dash0Style;
                OutputDescriptors[0].Pen.Width = this.Plot0Width;


                //When finished set the last day variable
                //If we are online during the day session we do not set this variable so we are redrawing and recalculating the current session again and again
                if (GlobalUtilities.IsCurrentBarLastDayInBars(this.Bars, this.Bars[0]))
                {
                    //the last session has started (current trading session, last day in Bars object, and so on)
                }
                else
                {
                    this.CurrentdayOfUpdate = this.Bars[0].Time.Date;
                }

            }
            else
            {
                //Data feed perodicity is not valid, print info in chart panel 
                if (IsProcessingBarIndexLast)
                {
                    AddChartTextFixed("AlertText", Const.DefaultStringDatafeedPeriodicity, TextPosition.Center, Color.Red, new Font("Arial", 30), Color.Red, Color.Red, 20);
                }
            }

        }

        /// <summary>
        /// Calculate Open Range with all IBar and current IBar 
        /// </summary>
        /// <param name="currentbar"></param>
        /// <returns></returns>
        public ORB_Result calculate(IBars bars, IBar currentbar)
        {
            //todo check if all data should be return via Result Object
            //todo check if it allowed that we are going two times through the limit (two buys per day, could this happen?)
            //todo tolerance level for breakout!

                ORB_Result resultvalue = new ORB_Result();

                //reset session day data
                this.LongBreakout = null;
                this.ShortBreakout = null;
                this.LongTargetReached = null;
                this.ShortTargetReached = null;

                //draw the open range
                resultvalue.Start = this.getOpenRangeStart(bars, currentbar.Time);
                resultvalue.End = this.getOpenRangeEnd(resultvalue.Start);

                //Select all data and find high & low.
                IEnumerable<IBar> list = bars.Where(x => x.Time >= resultvalue.Start).Where(x => x.Time <= resultvalue.End);

                //Check if there is data available
                if (list == null || list.IsEmpty())
                {
                    return resultvalue;
                }

                
                //Check if data for open range is valid.
                //we need to ignore the first day which is normally invalid.
                if (list.First().Time != resultvalue.Start)
                {
                    resultvalue.IsORBValid = false; 
                }
                else
                {
                    resultvalue.IsORBValid = true;
                }

                    //Calculate range
                    this.RangeLow = list.Where(x => x.Low == list.Min(y => y.Low)).LastOrDefault().Low;
                    this.RangeHigh = list.Where(x => x.High == list.Max(y => y.High)).LastOrDefault().High;

                    //Calculate targets
                    this.TargetLong = this.RangeHigh + this.RangeHeight;
                    this.TargetShort = this.RangeLow - this.RangeHeight;

                    //todo toleranz
                    double toleranz = 0; // this.RangeHeight / 5;

                    //load the data after the open range
                    list = bars.Where(x => x.Time >= resultvalue.End).Where(x => x.Time <= this.getEndOfTradingDay(bars, resultvalue.Start));

                    //find the first breakout to the long side
                    this.LongBreakout = list.Where(x => x.Close > this.RangeHigh + toleranz).FirstOrDefault();

                    //find the first breakout to the short side
                    this.ShortBreakout = list.Where(x => x.Close < this.RangeLow - toleranz).FirstOrDefault();

                    //find the first target to the long side
                    this.LongTargetReached = list.Where(x => x.Close > this.TargetLong + toleranz).FirstOrDefault();

                    //find the first target to the short side
                    this.ShortTargetReached = list.Where(x => x.Close < this.TargetShort - toleranz).FirstOrDefault();

                    //Everything was fine
                    resultvalue.IsCompleted = true;

                    return resultvalue;
        }

     




        /// <summary>
        /// Returns the start of the open range on the dedicated date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private DateTime getOpenRangeStart(IBars bars, DateTime date)
        {
            //Use Marketplace-Escort
            DateTime returnvalue = new DateTime(date.Year, date.Month, date.Day, this.TimePeriod.StartTime.Hours, this.TimePeriod.StartTime.Minutes, this.TimePeriod.StartTime.Seconds);

            //Use CFD data
            if (bars.Instrument.Name.Contains("DE.30"))
            {
                //return new TimeSpan(9,00,00);
                returnvalue = new DateTime(date.Year, date.Month, date.Day, this._tim_OpenRangeStartDE.Hours, this._tim_OpenRangeStartDE.Minutes, this._tim_OpenRangeStartDE.Seconds);
            }
            else if (bars.Instrument.Name.Contains("US.30") || bars.Instrument.Name.Contains("US.2000")
                || bars.Instrument.Name.Contains("US.100") || bars.Instrument.Name.Contains("US.500"))
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
            return date.AddMinutes(this.ORBMinutes);
        }

        /// <summary>
        ///  Returns the end of the trading day on the dedicated date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public DateTime getEndOfTradingDay(IBars bars, DateTime date)
        {
            //Use Marketplace-Escort
            DateTime returnvalue = new DateTime(date.Year, date.Month, date.Day, this.TimePeriod.EndTime.Hours, this.TimePeriod.EndTime.Minutes, this.TimePeriod.EndTime.Seconds);

            //Use CFD data
            if (bars.Instrument.Name.Contains("DE.30"))
            {
                //return new TimeSpan(9,00,00);
                returnvalue = new DateTime(date.Year, date.Month, date.Day, this.Time_EndOfDay_DE.Hours, this.Time_EndOfDay_DE.Minutes, this.Time_EndOfDay_DE.Seconds);
            }
            else if (bars.Instrument.Name.Contains("US.30") || bars.Instrument.Name.Contains("US.2000")
                ||  bars.Instrument.Name.Contains("US.100") || bars.Instrument.Name.Contains("US.500"))
            {
                //return new TimeSpan(15,30,00);
                returnvalue = new DateTime(date.Year, date.Month, date.Day, this._tim_EndOfDay_US.Hours, this._tim_EndOfDay_US.Minutes, this._tim_EndOfDay_US.Seconds);
            }
            return returnvalue;
        }

        /// <summary>
        /// Return the DateTime for the last candle before the last candel in this timeframe.
        ///You can use this function to close the trade in the end of the trading day.
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="date"></param>
        /// <param name="timeframe"></param>
        /// <param name="closexcandlesbeforeendofday"></param>
        /// <returns></returns>
        public DateTime getDateTimeForClosingBeforeTradingDayEnds(IBars bars, DateTime date, ITimeFrame timeframe, int closexcandlesbeforeendofday)
        {
            return getEndOfTradingDay(bars, date).AddSeconds((timeframe.GetSeconds() * (-1) * closexcandlesbeforeendofday));
        }

        /// <summary>
        /// True if the Periodicity of the data feed is correct for this indicator.
        /// </summary>
        /// <returns></returns>
        private bool DatafeedPeriodicityIsValid(IBars bars) {
                TimeFrame tf = (TimeFrame)bars.TimeFrame;
                if (tf.Periodicity == DatafeedHistoryPeriodicity.Tick || tf.Periodicity == DatafeedHistoryPeriodicity.Second )
                {
                    return true;
                }
                else if(tf.Periodicity == DatafeedHistoryPeriodicity.Minute) {
                    //Periodicity in minutes is right but in this case we need to check the modulus!
                    if (this.ORBMinutes % tf.PeriodicityValue == 0)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
        }



    



        public override string ToString()
        {
            return "ORB (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "ORB (I)";
            }
        }



		#region Properties

        #region InSeries


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
        [DisplayName("Open Range Color")]
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


        [XmlIgnore()]
        [Description("Open Range invalid Color")]
        [Category("Colors")]
        [DisplayName("Open Range invalid")]
        public Color Color_ORB_invalid
        {
            get { return _col_orb_invalid; }
            set { _col_orb_invalid = value; }
        }

        [Browsable(false)]
        public string Color_ORB_invalidSerialize
        {
            get { return SerializableColor.ToString(_col_orb_invalid); }
            set { _col_orb_invalid = SerializableColor.FromString(value); }
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
        [Description("Start of the open range in Germany")]
        [Category("CFD")]
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



        /// <summary>
        /// </summary>
        [Description("Start of the open range in America")]
        [Category("CFD")]
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



        /// <summary>
        /// </summary>
        [Description("End of trading day in Germany")]
        [Category("CFD")]
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
        [Description("End of trading day in America")]
        [Category("CFD")]
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
                get { return Outputs[0]; }
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

            [Browsable(false)]
            [XmlIgnore()]
            public double TargetLong
            {
                get { return _targetlong; }
                set { _targetlong = value; }
            }

            [Browsable(false)]
            [XmlIgnore()]
            public double TargetShort
            {
                get { return _targetshort; }
                set { _targetshort = value; }
            }

            [Browsable(false)]
            [XmlIgnore()]
            public IBar LongBreakout
            {
                get { return _long_breakout; }
                set { _long_breakout = value; }
            }

            [Browsable(false)]
            [XmlIgnore()]
            public IBar ShortBreakout
            {
                get { return _short_breakout; }
                set { _short_breakout = value; }
            }


        #endregion

        #region Internals





       


            private IBar LongTargetReached
            {
                get { return _long_target_reached; }
                set { _long_target_reached = value; }
            }


            private IBar ShortTargetReached
            {
                get { return _short_target_reached; }
                set { _short_target_reached = value; }
            }


            private DateTime CurrentdayOfUpdate
            {
                get { return _currentdayofupdate; }
                set { _currentdayofupdate = value; }
            }

            private ITimePeriod TimePeriod
            {
                get { return _timeperiod; }
                set { _timeperiod = value; }
            }

  


        #endregion

        #endregion
    }
}
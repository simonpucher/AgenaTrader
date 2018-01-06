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
using System.Text;
using System.Windows.Forms;
using System.Threading;

/// <summary>
/// Version: in progress
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// The initial version of this strategy was inspired by the work of Birger Schäfermeier: https://www.whselfinvest.at/de/Store_Birger_Schaefermeier_Trading_Strategie_Open_Range_Break_Out.php
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
    [Description("Automatic trading for Open Range Breakout strategy")]
    public class ORB_Strategy : UserStrategy, IORB
	{
        //input
        private int _orbminutes = Const.DefaultOpenRangeSizeinMinutes;

        private TimeSpan _tim_OpenRangeStartDE = new TimeSpan(8, 0, 0);
        private TimeSpan _tim_OpenRangeStartUS = new TimeSpan(14, 30, 0);  

        private TimeSpan _tim_EndOfDay_DE = new TimeSpan(17, 30, 0);
        private TimeSpan _tim_EndOfDay_US = new TimeSpan(22, 00, 0);

        private int _closexcandlesbeforeendoftradingday = 2;

        private bool _send_email = false;
        private bool _autopilot = true;
        private bool _closeorderbeforendoftradingday = true;
        private bool _statisticbacktesting = false;
        private bool _useprofittarget = true;
        private bool _usestoploss = true;

        //output
        //no output variables yet

        //internal
        private IOrder _orderenterlong;
        private IOrder _orderentershort;
        private ORB_Indicator _orb_indicator = null;
        private DateTime _currentdayofupdate = DateTime.MinValue;
        private StatisticContainer _StatisticContainer = null;
        private Statistic _statisticlong;
        private Statistic _statisticshort;


		protected override void OnInit()
		{
            //Set the default time frame if you start the strategy via the strategy-escort
            //if you start the strategy on a chart the TimeFrame is automatically set, this will lead to a better usability
            if (this.TimeFrame == null || this.TimeFrame.PeriodicityValue == 0)
            {
                 this.TimeFrame = new TimeFrame(DatafeedHistoryPeriodicity.Minute, 1);
            }

            //Because of Backtesting reasons if we use the advanced mode we need at least two bars
            this.RequiredBarsCount = 2;
		}


        protected override void OnStart()
        {
            base.OnStart();

            //Print("OnStartUp" + Bars[0].Timestamp);

            //Init our indicator to get code access
            this._orb_indicator = new ORB_Indicator();
            this._orb_indicator.SetData(this.Instrument);

            //Initalize Indicator parameters
            _orb_indicator.ORBMinutes = this.ORBMinutes;
            _orb_indicator.Time_OpenRangeStartDE = this.Time_OpenRangeStartDE;
            _orb_indicator.Time_OpenRangeStartUS = this.Time_OpenRangeStartUS;
            _orb_indicator.Time_EndOfDay_DE = this.Time_EndOfDay_DE;
            _orb_indicator.Time_EndOfDay_US = this.Time_EndOfDay_US;

            //Initalize statistic data list if this feature is enabled
            if (this.StatisticBacktesting)
            {
                this._StatisticContainer = new StatisticContainer();
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            //Print("OnTermination" + Bars[0].Timestamp);

            //Close statistic data list if this feature is enabled
            if (this.StatisticBacktesting)
            {
                //get the statistic data
                string data = this._StatisticContainer.getCSVData();

                //get the statistic data
                this._StatisticContainer.copyToClipboard();

            }
        }

        /// <summary>
        /// todo this method seam to be broken - get never called.
        /// </summary>
        protected override void OnBrokerConnect()
        {
           //base.OnBrokerConnect();

           ////send email
           //if (this.Send_email)
           //{
           //    this.SendEmail(Core.Settings.MailDefaultFromAddress, Core.PreferenceManager.DefaultEmailAddress,
           //        "OnBrokerConnect on Strategy: " + this.GetType().Name, "Broker was connected" + " - Date: " + DateTime.Now.ToString());
           //}

        }

        protected override void OnBrokerDisconnect(TradingDatafeedChangedEventArgs e)
        {
            //base.OnBrokerDisconnect(e);
            
            ////send email
            //if (this.Send_email)
            //{
            //    this.SendEmail(Core.Settings.MailDefaultFromAddress, Core.PreferenceManager.DefaultEmailAddress,
            //        "OnBrokerDisconnect on Strategy: " + this.GetType().Name, "Broker was disconnected" + " - Date: " + DateTime.Now.ToString());
            //}
        }

		protected override void OnCalculate()
		{
            //Print("OnBarUpdate" + Bars[0].Timestamp.ToString());

            this.IsAutoConfirmOrder = this.Autopilot;

            //Reset Strategy for the next/first trading day
            //todo => Not perfect if we are using GMT+12 and other markets than local markets like DAX 
            if (this.CurrentdayOfUpdate.Date < Bars[0].Time.Date)
            {
                this._orderenterlong = null;
                this._orderentershort = null;
                this.CurrentdayOfUpdate = Bars[0].Time.Date;

                ////send email
                //if (this.Send_email)
                //{
                //    this.SendEmail(Core.Settings.MailDefaultFromAddress, Core.PreferenceManager.DefaultEmailAddress,
                //        "Reset on Strategy: " + this.GetType().Name, "Strategy was restarted because a new trading day has arrived." + Environment.NewLine + "Instrument: " + this.Instrument.Name + " - Date: " + Bars[0].Timestamp);
                //}
            }

            //close manually the trades in the end of the trading day
            DateTime eod = this._orb_indicator.getDateTimeForClosingBeforeTradingDayEnds(this.Bars, this.Bars[0].GetTimestamp(), this.TimeFrame, this.CloseXCandlesBeforeEndOfTradingDay);
            if (this.CloseOrderBeforeEndOfTradingDay
                && (this._orderenterlong != null || this._orderentershort != null)
                && Bars[0].Time >= eod)
            {
                this.DoExitLong();
                this.DoExitShort();
            }

            //if it to late or one order already set stop execution of calculate
            // || Bars[0].Timestamp.TimeOfDay >= this._orb_indicator.getDateTimeForClosingBeforeTradingDayEnds(this.Bars, this.Bars[0].Timestamp, this.TimeFrame, this.CloseXCandlesBeforeEndOfTradingDay).TimeOfDay
            if (_orderenterlong != null || _orderentershort != null != Bars[0].Time >= eod)
            {
                return;
            }

            //Calulate data
            _orb_indicator.calculate(this.Bars, this.Bars[0]);

            //If there was a breakout and the current bar is the same bar as the long/short breakout, then trigger signal.
            if (_orb_indicator.LongBreakout != null && _orb_indicator.LongBreakout.GetTimestamp() == Bars[0].Time)
            {
                //Long Signal
                //Print("Enter Long" + Bars[0].Timestamp.ToString());
                DoEnterLong();
            }
            else if (_orb_indicator.ShortBreakout != null && _orb_indicator.ShortBreakout.GetTimestamp() == Bars[0].Time)
            {
                //Short Signal
                //Print("Enter Short" + Bars[0].Timestamp.ToString());
                DoEnterShort();
            }
            else
            {
                //nothing to do
            }
        }





        /// <summary>
        /// OnExecution of orders
        /// </summary>
        /// <param name="execution"></param>
            protected override void OnOrderExecution(IExecution execution)
            {
                ////info: was uncommented because exired date is not working in simulation mode or in backtesting mode AND also not in AT 1.9
                ////set expiration date to close at the end of the trading day
                //if (this.CloseOrderBeforeEndOfTradingDay)
                //{
                //    foreach (AgenaTrader.Helper.TradingManager.Trade item in this.Root.Core.TradingManager.ActiveOpenedTrades)
                //    {
                //        if ((this._orderenterlong != null && item.EntryOrder.Name == this._orderenterlong.Name)
                //         || (this._orderentershort != null && item.EntryOrder.Name == this._orderentershort.Name))
                //        {
                //            item.Expiration = this._orb_indicator.getDateTimeForClosingBeforeTradingDayEnds(this.Bars, this.Bars[0].Timestamp, this.TimeFrame, this.CloseXCandlesBeforeEndOfTradingDay);
                //            //Print("Expiration: " + item.Expiration.ToString());
                //        }
                //    }
                //}

                ////Create statistic for execution
                ////todo use it or delete it - there is no try!
                //if (this.StatisticBacktesting)
                //{
                //    this._StatisticContainer.Add(this.Root.Core.TradingManager, this, execution);
                //}

                //send email
                if (this.Send_email)
                {
                    this.SendEmail(Core.Settings.MailDefaultFromAddress, Core.PreferenceManager.DefaultEmailAddress,
                            GlobalUtilities.GetEmailSubject(execution), GlobalUtilities.GetEmailText(execution, this.GetType().Name));
                }
            }

        /// <summary>
        /// Create Long Order and Stop.
        /// </summary>
        private void DoEnterLong() {
            if (this._orderenterlong == null)
            {
                string entryreason = "ORB_Long_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString();
                _orderenterlong = SubmitOrder(new StrategyOrderParameters {Direction = OrderDirection.Buy, Type = OrderType.Market, Quantity = GlobalUtilities.AdjustPositionToRiskManagement(this.Root.Core.AccountManager, this.Root.Core.PreferenceManager, this.Instrument, Bars[0].Close), SignalName =  entryreason, Instrument =  this.Instrument, TimeFrame =  this.TimeFrame});
                if (this.UseStopLoss)
                {
                    SetUpStopLoss(_orderenterlong.Name, CalculationMode.Price, this._orb_indicator.RangeLow, false);
                }
                if (this.UseProfitTarget)
                {
                      SetUpProfitTarget(_orderenterlong.Name, CalculationMode.Price, this._orb_indicator.TargetLong); 
                }

                if (this.StatisticBacktesting)
                {
                    _statisticlong = new Statistic(this, PositionType.Long);
                    this._statisticlong.SetEntry(entryreason, _orderenterlong.Quantity, Bars[0].Close, Bars[0].Time, OrderType.Market);
                }
            }
        }

        /// <summary>
        /// Create Short Order and Stop.
        /// </summary>
        private void DoEnterShort() {
            if (this._orderentershort == null)
            {
                string entryreason = "ORB_Short_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString();
                _orderentershort = SubmitOrder(new StrategyOrderParameters {Direction = OrderDirection.Sell, Type = OrderType.Market, Quantity = GlobalUtilities.AdjustPositionToRiskManagement(this.Root.Core.AccountManager, this.Root.Core.PreferenceManager, this.Instrument, Bars[0].Close), SignalName =  entryreason, Instrument =  this.Instrument, TimeFrame =  this.TimeFrame});
                if (this.UseStopLoss)
                {
                    SetUpStopLoss(_orderentershort.Name, CalculationMode.Price, this._orb_indicator.RangeHigh, false);
                }
                if (this.UseProfitTarget)
                {
                     SetUpProfitTarget(_orderentershort.Name, CalculationMode.Price, this._orb_indicator.TargetShort);
                }

                if (this.StatisticBacktesting)
                {
                    _statisticshort = new Statistic(this, PositionType.Short);
                    this._statisticshort.SetEntry(entryreason, _orderentershort.Quantity, Bars[0].Close, Bars[0].Time, OrderType.Market);
                }
            }
        }


        /// <summary>
        /// Exit the LONG position.
        /// </summary>
        private void DoExitLong()
        {
            if (_orderenterlong != null)
            {
                //todo exit reason on target?
                CloseLongTrade(new StrategyOrderParameters {Type = OrderType.Market, Quantity = this._orderenterlong.Quantity, SignalName =  Const.DefaultExitReasonEOD, FromEntrySignal =  this._orderenterlong.Name, Instrument =  this._orderenterlong.Instrument, TimeFrame =  this._orderenterlong.TimeFrame});

                if (this.StatisticBacktesting)
                {
                    this._statisticlong.SetExit(Const.DefaultExitReasonEOD, this._orderenterlong.Quantity, Bars[0].Close, Bars[0].Time, OrderType.Market);
                    this._StatisticContainer.Add(this._statisticlong);
                    _statisticlong = null;
                }

                this._orderenterlong = null;
            }
        }

        /// <summary>
        /// Fill the SHORT position.
        /// </summary>
        private void DoExitShort()
        {

            if (_orderentershort != null)
            {
                //todo exit reason on target?
                CloseShortTrade(new StrategyOrderParameters {Type = OrderType.Market, Quantity = this._orderentershort.Quantity, SignalName =  Const.DefaultExitReasonEOD, FromEntrySignal =  this._orderentershort.Name, Instrument =  this._orderentershort.Instrument, TimeFrame =  this._orderentershort.TimeFrame});

                if (this.StatisticBacktesting)
                {
                    this._statisticshort.SetExit(Const.DefaultExitReasonEOD, this._orderentershort.Quantity, Bars[0].Close, Bars[0].Time, OrderType.Market);
                    this._StatisticContainer.Add(this._statisticshort);
                    this._statisticshort = null;
                }

                this._orderentershort = null;
            }
        }



        public override string ToString()
        {
            return "ORB (S)";
        }

        public override string DisplayName
        {
            get
            {
                return "ORB (S)";
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
            set
            {
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
            [Description("Close the order x candles before the end of trading day")]
            [Category("Settings")]
            [DisplayName("Close Order X candles")]
            public int CloseXCandlesBeforeEndOfTradingDay
            {
                get { return _closexcandlesbeforeendoftradingday; }
                set { _closexcandlesbeforeendoftradingday = value; }
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


            [Description("If true an email will be send on order execution and on other important issues")]
            [Category("Safety first!")]
            [DisplayName("Send email")]
            public bool Send_email
            {
                get { return _send_email; }
                set { _send_email = value; }
            }


            [Description("If true the strategy will handle everything. It will create buy orders, sell orders, stop loss orders, targets fully automatically")]
            [Category("Safety first!")]
            [DisplayName("Autopilot")]
            public bool Autopilot
            {
                get { return _autopilot; }
                set { _autopilot = value; }
            }

            [Description("If true the strategy will close the orders before the end of trading day")]
            [Category("Safety first!")]
            [DisplayName("Close order EOD")]
            public bool CloseOrderBeforeEndOfTradingDay
            {
                get { return _closeorderbeforendoftradingday; }
                set { _closeorderbeforendoftradingday = value; }
            }


            [Description("If true the strategy will create statistic data during the backtesting process")]
            [Category("Safety first!")]
            [DisplayName("Statistic Backtesting")]
            public bool StatisticBacktesting
            {
                get { return _statisticbacktesting; }
                set { _statisticbacktesting = value; }
            }

            [Description("If true the strategy will will use profit targets.")]
            [Category("Safety first!")]
            [DisplayName("Use profit target")]
            public bool UseProfitTarget
            {
                get { return _useprofittarget; }
                set { _useprofittarget = value; }
            }

            [Description("If true the strategy will will use stop loss.")]
            [Category("Safety first!")]
            [DisplayName("Use stop loss")]
            public bool UseStopLoss
            {
                get { return _usestoploss; }
                set { _usestoploss = value; }
            }


            #endregion

        #region Output

            
        #endregion

        #region Internals


            [Browsable(false)]
        private DateTime CurrentdayOfUpdate
        {
            get { return _currentdayofupdate; }
            set { _currentdayofupdate = value; }
        }


        #endregion


        #endregion
    }
}

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
/// You will find this indicator on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Automatic trading for ORB strategy")]
    public class ORB_Strategy : UserStrategy, IORB
	{
        //input
        private int _orbminutes = 75;

        private TimeSpan _tim_OpenRangeStartDE = new TimeSpan(9, 0, 0);
        //private TimeSpan _tim_OpenRangeEndDE = new TimeSpan(10, 15, 0);  

        private TimeSpan _tim_OpenRangeStartUS = new TimeSpan(15, 30, 0);
        //private TimeSpan _tim_OpenRangeEndUS = new TimeSpan(16, 45, 0);    

        private TimeSpan _tim_EndOfDay_DE = new TimeSpan(17, 30, 0);
        private TimeSpan _tim_EndOfDay_US = new TimeSpan(22, 00, 0);

        private int _closexcandlesbeforeendoftradingday = 2;

        private bool _send_email = false;
        private bool _automation = false;
        private bool _closeorderbeforendoftradingday = false;

        //output


        //internal
        private IOrder _orderenterlong;
        private IOrder _orderentershort;
        //private IOrder _orderenterlong_stop;
        //private IOrder _orderentershort_stop;
        //private IOrder _orderlong_target;
        //private IOrder _ordershort_target;
        private ORB_Indicator _orb_indicator = null;
        //protected TimeFrame tf = new TimeFrame(DatafeedHistoryPeriodicity.Minute, 10);


		protected override void Initialize()
		{

            //ClearOutputWindow();
            //TraceOrders = true;

            //Set the default time frame if you start the strategy via the strategy-escort
            //if you start the strategy on a chart the TimeFrame is automatically set.
            if (this.TimeFrame == null || this.TimeFrame.PeriodicityValue == 0)
            {
                 this.TimeFrame = new TimeFrame(DatafeedHistoryPeriodicity.Minute, 1);
            }

            //We need at least one bar.
            this.BarsRequired = 1;
		}

        protected override void OnStartUp()
        {
            base.OnStartUp();

            //Init our indicator to get code access
            this._orb_indicator = new ORB_Indicator();
            this._orb_indicator.SetData(this.Instrument);

            //Initalize Indicator parameters
            _orb_indicator.ORBMinutes = this.ORBMinutes;
            _orb_indicator.Time_OpenRangeStartDE = this.Time_OpenRangeStartDE;
            //_orb_indicator.Time_OpenRangeEndDE = this.Time_OpenRangeEndDE;
            _orb_indicator.Time_OpenRangeStartUS = this.Time_OpenRangeStartUS;
            //_orb_indicator.Time_OpenRangeEndUS = this.Time_OpenRangeEndUS;
            _orb_indicator.Time_EndOfDay_DE = this.Time_EndOfDay_DE;
            _orb_indicator.Time_EndOfDay_US = this.Time_EndOfDay_US;
        }

		protected override void OnBarUpdate()
		{
            //Print("OnBarUpdate" + Bars[0].Time.ToString());

            this.IsAutomated = this.Automation;

            //IAccount account = this.Core.AccountManager.GetAccount(this.Instrument, true);
            //int quantity = this.Instrument.GetDefaultQuantity(account);

            //Print("Order Quantity: " + quantity);

            //todo reset strategy for the next/first trading day

            //if it to late or one order already set stop execution of calculate
            if ((_orderenterlong != null || _orderentershort != null)
                || Bars[0].Time.TimeOfDay >= this._orb_indicator.getDateTimeForClosingBeforeTradingDayEnds(this.Bars, this.Bars[0].Time, this.TimeFrame, this.CloseXCandlesBeforeEndOfTradingDay).TimeOfDay)
            {
                return;
            }

            this.calculate();
        }


        /// <summary>
        /// 
        /// </summary>
        private void calculate()
        {

            _orb_indicator.calculate(this.Bars, this.Bars[0]);

            //If there was a breakout and the current bar is the same bar as the long/short breakout, then trigger signal.
            if (_orb_indicator.LongBreakout != null && _orb_indicator.LongBreakout.Time == Bars[0].Time)
            {
                //Long Signal
                //Print("Enter Long" + Bars[0].Time.ToString());
                EnterLong();
            }
            else if (_orb_indicator.ShortBreakout != null && _orb_indicator.ShortBreakout.Time == Bars[0].Time)
            {
                //Short Signal
                //Print("Enter Short" + Bars[0].Time.ToString());
                EnterShort();
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
            protected override void OnExecution(IExecution execution)
            {
                //set expiration date to close at the end of the trading day
                if (this.CloseOrderBeforeEndOfTradingDay)
                {
                     foreach (AgenaTrader.Helper.TradingManager.Trade item in this.Root.Core.TradingManager.ActiveOpenedTrades)
                    {
                        if ((this._orderenterlong != null && item.EntryOrder.Name == this._orderenterlong.Name)
                         || (this._orderentershort != null && item.EntryOrder.Name == this._orderentershort.Name))
                        {
                            item.Expiration = this._orb_indicator.getDateTimeForClosingBeforeTradingDayEnds(this.Bars, this.Bars[0].Time, this.TimeFrame, this.CloseXCandlesBeforeEndOfTradingDay); 
                            //Print("Expiration: " + item.Expiration.ToString());
                        }
                    }  
                }

                //send email
                if (IsEmailFunctionActive) {
                    this.SendEmail(Core.Settings.MailDefaultFromAddress, Core.PreferenceManager.DefaultEmailAddress,
                            GlobalUtilities.GetEmailSubject(execution), GlobalUtilities.GetEmailText(execution, this.GetType().Name));
                }
            }

        /// <summary>
        /// Create Long Order and Stop.
        /// </summary>
        private void EnterLong() {

            //todo positionsgröße bestimmen
            _orderenterlong = EnterLong(1, "ORB_Long_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(), this.Instrument, this.TimeFrame);
            SetStopLoss(_orderenterlong.Name, CalculationMode.Price, this._orb_indicator.RangeLow, false);
            SetProfitTarget(_orderenterlong.Name, CalculationMode.Price, this._orb_indicator.TargetLong);

            //Connect the entry and the stop order and fire it!
            if (this.IsAutomated)
            {
                _orderenterlong.ConfirmOrder();  
            }
        }

        /// <summary>
        /// Create Short Order and Stop.
        /// </summary>
        private void EnterShort() {

            //todo positionsgröße bestimmen
            _orderentershort = EnterShort(1, "ORB_Short_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(), this.Instrument, this.TimeFrame);
            SetStopLoss(_orderentershort.Name, CalculationMode.Price, this._orb_indicator.RangeHigh, false);
            SetProfitTarget(_orderentershort.Name, CalculationMode.Price, this._orb_indicator.TargetShort);

            //Connect the entry and the stop order and fire it!
            if (this.IsAutomated)
            {
                _orderenterlong.ConfirmOrder();
            }
        }



        #region Properties



        #region Input

            /// <summary>
            /// </summary>
            [Description("Period in minutes for the ORB")]
            [Category("Settings")]
            [DisplayName("Minutes ORB")]
            public int ORBMinutes
            {
                get { return _orbminutes; }
                set { _orbminutes = value; }
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
            [Category("Settings")]
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
            [Description("Start of the open range in USA")]
            [Category("Settings")]
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
            [Category("Settings")]
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
            [Description("End of trading day in USA")]
            [Category("Settings")]
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


            [Description("If true an email will be send on order execution")]
            [Category("Safety features")]
            [DisplayName("Send email")]
            public bool Send_email
            {
                get { return _send_email; }
                set { _send_email = value; }
            }


            [Description("If true you can go to the beach, the strategy will handle everything")]
            [Category("Safety features")]
            [DisplayName("Fully automatic operation")]
            public bool Automation
            {
                get { return _automation; }
                set { _automation = value; }
            }

            [Description("If true the strategy will close the order before the end of trading day")]
            [Category("Safety features")]
            [DisplayName("Close order today")]
            public bool CloseOrderBeforeEndOfTradingDay
            {
                get { return _closeorderbeforendoftradingday; }
                set { _closeorderbeforendoftradingday = value; }
            }


            #endregion

        #region Output

            
        #endregion

        #region Internals


        [Browsable(false)]
        public bool IsEmailFunctionActive
        {
            get
            {
                if (this.Send_email) //&& GlobalUtilities.IsValidEmail(this.EmailAdress))
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

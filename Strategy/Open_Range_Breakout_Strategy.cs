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
	[Description("Handelsautomatik für ORB Strategy")]
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

        private bool _send_email = false;
     

        //output


        //internal
        private IOrder _orderenterlong;
        private IOrder _orderentershort;
        private IOrder _orderenterlong_stop;
        private IOrder _orderentershort_stop;
        private ORB_Indicator _orb_indicator = null;
        //protected TimeFrame tf = new TimeFrame(DatafeedHistoryPeriodicity.Minute, 10);


		protected override void Initialize()
		{
            this.IsAutomated = false;

            //Set the default time frame if you start the strategy via the strategy-escort
            //if you start the strategy on a chart the TimeFrame is automatically set.
            if (this.TimeFrame == null || this.TimeFrame.PeriodicityValue == 0)
            {
                 this.TimeFrame = new TimeFrame(DatafeedHistoryPeriodicity.Minute, 1);
            }
		}

        protected override void OnStartUp()
        {
            base.OnStartUp();

            //Init our indicator to get code access
            this._orb_indicator = new ORB_Indicator();
            this._orb_indicator.SetData(this.Instrument, this.Bars);

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
          

            //if (Bars[0].Time == new DateTime(2016, 4, 20, 15, 00, 0)) {
              
            //}

            //Only excecute on last bar
            if (!IsCurrentBarLast )
            {
                return;
            }

            //if one order already set stop execution
            if (_orderenterlong != null || _orderenterlong_stop != null || _orderentershort != null || _orderentershort_stop != null)
            {
                return;
            }

           

            //EnterLong(3);




            //CreateOCOGroup(new List<IOrder> { oEnterLong, oEnterShort });

            //oEnterLong.ConfirmOrder();
            //oEnterShort.ConfirmOrder();

            //if (ORB_Indicator()[0] == 1)
            //{


            //    oEnterLong = SubmitOrder(0, OrderAction.Buy, OrderType.Market, 3, 0, Close[0], "ocoId", "signalName");
            //}



            //Initalize Indicator parameters
            _orb_indicator.ORBMinutes = this.ORBMinutes;
            _orb_indicator.Time_OpenRangeStartDE = this.Time_OpenRangeStartDE;
            //_orb_indicator.Time_OpenRangeEndDE = this.Time_OpenRangeEndDE;
            _orb_indicator.Time_OpenRangeStartUS = this.Time_OpenRangeStartUS;
            //_orb_indicator.Time_OpenRangeEndUS = this.Time_OpenRangeEndUS;
            _orb_indicator.Time_EndOfDay_DE = this.Time_EndOfDay_DE;
            _orb_indicator.Time_EndOfDay_US = this.Time_EndOfDay_US;

            switch ((int)_orb_indicator[0])
            {
                case 1:
                    //Long Signal
                    //Print("Enter Long");
                    EnterLong();
                    break;
                case -1:
                    //Short Signal
                    //Print("Enter Short");
                    EnterShort();
                    break;
                default:
                    //nothing to do
                    break;
            }

     
        }

        
            protected override void OnExecution(IExecution execution)
            {
                //foreach (Trade item in this.Root.Core.TradingManager.ActiveOpenedTrades)
                //{
                //    if (item.EntryOrder.Name == "signalName")
                //    {
                //        //Order ord = (Order)this.TradingManager.GetOrder(item.Id);


                //        item.Expiration = DateTime.Now.AddMinutes(5);

                //        //Order ord = (Order)this.TradingManager.GetOrder(item.Id);
                //        //ord.Quantity = ord.Exp - 1;
                //        //this.TradingManager.EditOrder(ord);
                //    }

                //}

                if (execution.Order != null && execution.Order.OrderState == OrderState.Filled) { 
                    if (_orderenterlong != null && execution.Name == _orderenterlong.Name)
                    {
                    // Enter-Order gefüllt
                        if (IsEmailFunctionActive) this.SendEmail(Core.AccountManager.Core.Settings.MailDefaultFromAddress, Core.PreferenceManager.DefaultEmailAddress,
                        execution.Instrument.Symbol + " Order " + execution.Name + " ausgeführt.", "Die LONG Order für " + execution.Instrument.Name + " wurde ausgeführt. Invest: " + (Trade.Quantity * Trade.AvgPrice).ToString("F2"));
                    }
                    else if (_orderenterlong_stop != null && execution.Name == _orderenterlong_stop.Name)
                    {
                        if (IsEmailFunctionActive) this.SendEmail(Core.AccountManager.Core.Settings.MailDefaultFromAddress, Core.PreferenceManager.DefaultEmailAddress,
                       execution.Instrument.Symbol + " Order " + execution.Name + " ausgeführt.", "Die LONG STOP für " + execution.Instrument.Name + " wurde ausgeführt. Invest: " + (Trade.Quantity * Trade.AvgPrice).ToString("F2"));
                    }
                    else if (_orderentershort != null && execution.Name == _orderentershort.Name)
                    {
                        if (IsEmailFunctionActive) this.SendEmail(Core.AccountManager.Core.Settings.MailDefaultFromAddress, Core.PreferenceManager.DefaultEmailAddress,
                        execution.Instrument.Symbol + " Order " + execution.Name + " ausgeführt.", "Die SHORT Order für " + execution.Instrument.Name + " wurde ausgeführt. Invest: " + (Trade.Quantity * Trade.AvgPrice).ToString("F2"));
                    }
                    else if (_orderentershort_stop != null && execution.Name == _orderentershort_stop.Name)
                    {
                        if (IsEmailFunctionActive) this.SendEmail(Core.AccountManager.Core.Settings.MailDefaultFromAddress, Core.PreferenceManager.DefaultEmailAddress,
                        execution.Instrument.Symbol + " Order " + execution.Name + " ausgeführt.", "Die SHORT STOP Order für " + execution.Instrument.Name + " wurde ausgeführt. Invest: " + (Trade.Quantity * Trade.AvgPrice).ToString("F2"));
                    }
                }
            }

        /// <summary>
        /// Create Long Order and Stop.
        /// </summary>
        private void EnterLong() {
            _orderenterlong = SubmitOrder(0, OrderAction.Buy, OrderType.Market, 1, 0, Close[0], "long_ocoId" + this.Instrument.ISIN, "ORB");
            _orderenterlong_stop = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, 1, 0, this._orb_indicator.RangeLow, "long_ocoId" + this.Instrument.ISIN, "ORB");

            //Core.PreferenceManager.DefaultEmailAddress
            if (IsEmailFunctionActive) this.SendEmail(Core.AccountManager.Core.Settings.MailDefaultFromAddress, this.Core.PreferenceManager.DefaultEmailAddress,
                this.Instrument.Symbol + " ORB Long", "Open Range Breakout in Richtung Long bei " + Close[0]);
        }

        /// <summary>
        /// Create Short Order and Stop.
        /// </summary>
        private void EnterShort() {
            _orderentershort = SubmitOrder(0, OrderAction.SellShort, OrderType.Market, 1, 0, Close[0], "short_ocoId" + this.Instrument.ISIN, "ORB");
            _orderentershort_stop = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, 1, 0, this._orb_indicator.RangeHigh, "short_ocoId" + this.Instrument.ISIN, "ORB");

            //Core.PreferenceManager.DefaultEmailAddress
            if (IsEmailFunctionActive) this.SendEmail(Core.AccountManager.Core.Settings.MailDefaultFromAddress, this.Core.PreferenceManager.DefaultEmailAddress,
                this.Instrument.Symbol + " ORB Short", "Open Range Breakout in Richtung Short bei " + Close[0]);
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

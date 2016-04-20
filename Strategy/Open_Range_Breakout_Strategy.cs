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

namespace AgenaTrader.UserCode
{
	[Description("ORB Strategy")]
    public class ORB_Strategy : UserStrategy, IORB
	{
        //input
        private int _orbminutes = 75;
        private Color _col_orb = Color.Brown;
        private Color _col_target_short = Color.PaleVioletRed;
        private Color _col_target_long = Color.PaleGreen;

        private TimeSpan _tim_OpenRangeStartDE = new TimeSpan(9, 0, 0);    //09:00:00   
        private TimeSpan _tim_OpenRangeEndDE = new TimeSpan(10, 15, 0);    //09:00:00   

        private TimeSpan _tim_OpenRangeStartUS = new TimeSpan(15, 30, 0);  //15:30:00   
        private TimeSpan _tim_OpenRangeEndUS = new TimeSpan(16, 45, 0);  //15:30:00   

        private TimeSpan _tim_EndOfDay_DE = new TimeSpan(16, 30, 0);  //16:30:00   
        private TimeSpan _tim_EndOfDay_US = new TimeSpan(21, 30, 0);  //21:30:00

        private bool _send_email = false;
        private string _emailaddress = String.Empty;

        //output


        //internal
        private IOrder _orderenterlong;
        private IOrder _orderentershort;
        private IOrder _orderenterlong_stop;
        private IOrder _orderentershort_stop;
        private ORB_Indicator _orb_indicator = null;


		protected override void Initialize()
		{
            this.IsAutomated = false;
		}

        protected override void OnStartUp()
        {
            base.OnStartUp();

            _orb_indicator = new ORB_Indicator();
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
            _orb_indicator.Time_OpenRangeEndDE = this.Time_OpenRangeEndDE;
            _orb_indicator.Time_OpenRangeStartUS = this.Time_OpenRangeStartUS;
            _orb_indicator.Time_OpenRangeEndUS = this.Time_OpenRangeEndUS;
            _orb_indicator.Time_EndOfDay_DE = this.Time_EndOfDay_DE;
            _orb_indicator.Time_EndOfDay_US = this.Time_EndOfDay_US;

            switch ((int)_orb_indicator[0])
            {
                case 1:
                    //Long Signal
                    EnterLong();
                    break;
                case -1:
                    //Short Signal
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
            if (IsEmailFunctionActive) this.SendEmail(Core.AccountManager.Core.Settings.MailDefaultFromAddress, this.EmailAdress,
                this.Instrument.Symbol + " ORB Long", "Open Range Breakout in Richtung Long bei " + Close[0]);
        }

        /// <summary>
        /// Create Short Order and Stop.
        /// </summary>
        private void EnterShort() {
            _orderentershort = SubmitOrder(0, OrderAction.SellShort, OrderType.Market, 1, 0, Close[0], "short_ocoId" + this.Instrument.ISIN, "ORB");
            _orderentershort_stop = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, 1, 0, this._orb_indicator.RangeHigh, "short_ocoId" + this.Instrument.ISIN, "ORB");

            //Core.PreferenceManager.DefaultEmailAddress
            if (IsEmailFunctionActive) this.SendEmail(Core.AccountManager.Core.Settings.MailDefaultFromAddress, this.EmailAdress,
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
            [Description("Select Color")]
            [Category("Colors")]
            [DisplayName("ORB")]
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


            [Description("Recipient Email Address")]
            [Category("Email")]
            [DisplayName("Email Address")]
            public string EmailAdress
            {
                get { return _emailaddress; }
                set { _emailaddress = value; }
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
                if (this.Send_email && GlobalUtilities.IsValidEmail(this.EmailAdress))
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

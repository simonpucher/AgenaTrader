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
/// Version: 1.1.3
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// This indicator provides entry and exit signals on time.
/// Long signal in every even minute. Short signal every odd minute.
/// You can use this indicator also as a template for further script development.
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("This indicator provides a long signal in every even minute and a short signal every odd minute.")]
    public class DummyOneMinuteEvenOdd_Advanced_Strategy : UserStrategy, IDummyOneMinuteEvenOdd_Advanced
	{
        //interface
        private bool _IsShortEnabled = true;
        private bool _IsLongEnabled = true;
        private bool _WarningOccured = false;
        private bool _ErrorOccured = false;

        //input
        private bool _autopilot = true;

        //output

        //internal
        private DummyOneMinuteEvenOdd_Advanced_Indicator _DummyOneMinuteEvenOdd_Advanced_Indicator = null;
        private IOrder _orderenterlong;
        private IOrder _orderentershort;

		protected override void Initialize()
		{
            CalculateOnBarClose = true;

            //Set the default time frame if you start the strategy via the strategy-escort
            //if you start the strategy on a chart the TimeFrame is automatically set, this will lead to a better usability
            if (this.TimeFrame == null || this.TimeFrame.PeriodicityValue == 0)
            {
                this.TimeFrame = new TimeFrame(DatafeedHistoryPeriodicity.Minute, 1);
            }

            //Because of Backtesting reasons if we use the advanced mode we need at least two bars
            this.BarsRequired = 2;
		}

        protected override void InitRequirements()
        {
            //Print("InitRequirements");
            base.InitRequirements();
        }


        protected override void OnStartUp()
        {
            base.OnStartUp();

            //Init our indicator to get code access to the calculate method
            this._DummyOneMinuteEvenOdd_Advanced_Indicator = new DummyOneMinuteEvenOdd_Advanced_Indicator();

            this.ErrorOccured = false;
            this.WarningOccured = false;
        }

		protected override void OnBarUpdate()
		{
            //Set Autopilot
            this.IsAutomated = this.Autopilot;

            //Check if peridocity is valid for this script
            if (!this._DummyOneMinuteEvenOdd_Advanced_Indicator.DatafeedPeriodicityIsValid(Bars.TimeFrame))
            {
                //Display warning just one time
                if (!this.WarningOccured)
                {
                    Log(this.DisplayName + ": " + Const.DefaultStringDatafeedPeriodicity, InfoLogLevel.Warning);
                    this.WarningOccured = true;
                }
                return;
            }

            //Lets call the calculate method and save the result with the trade action
            ResultValue returnvalue = this._DummyOneMinuteEvenOdd_Advanced_Indicator.calculate(Bars[0], this.IsLongEnabled, this.IsShortEnabled);

            //If the calculate method was not finished we need to stop and show an alert message to the user.
            if (returnvalue.ErrorOccured)
            {
                //Display error just one time
                if (!this.ErrorOccured)
                {
                    Log(this.DisplayName + ": " + Const.DefaultStringErrorDuringCalculation, InfoLogLevel.AlertLog);
                    this.ErrorOccured = true;
                }
                return;
            }

            //Entry
            if (returnvalue.Entry.HasValue)
            {
                switch (returnvalue.Entry)
                {
                    case OrderAction.Buy:
                        this.DoEnterLong();
                        break;
                    case OrderAction.SellShort:
                        this.DoEnterShort();
                        break;
                }
            }

            //Exit
            if (returnvalue.Exit.HasValue)
            {
                switch (returnvalue.Exit)
                {
                    case OrderAction.BuyToCover:
                        this.DoExitShort();
                        break;
                    case OrderAction.Sell:
                        this.DoExitLong();
                        break;
                }
            }
		}


        /// <summary>
        /// Create LONG order.
        /// </summary>
        private void DoEnterLong()
        {
            if (_orderenterlong == null)
            {
                _orderenterlong = EnterLong(GlobalUtilities.AdjustPositionToRiskManagement(this.Root.Core.AccountManager, this.Root.Core.PreferenceManager, this.Instrument, Bars[0].Close), this.DisplayName + "_" + OrderAction.Buy + "_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(), this.Instrument, this.TimeFrame);
                //SetStopLoss(_orderenterlong.Name, CalculationMode.Price, this._orb_indicator.RangeLow, false);
                //SetProfitTarget(_orderenterlong.Name, CalculationMode.Price, this._orb_indicator.TargetLong); 
            }
        }

        /// <summary>
        /// Create SHORT order.
        /// </summary>
        private void DoEnterShort()
        {
            if (_orderentershort == null)
            {
                _orderentershort = EnterShort(GlobalUtilities.AdjustPositionToRiskManagement(this.Root.Core.AccountManager, this.Root.Core.PreferenceManager, this.Instrument, Bars[0].Close), this.DisplayName + "_" + OrderAction.SellShort + "_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(), this.Instrument, this.TimeFrame);
                //SetStopLoss(_orderentershort.Name, CalculationMode.Price, this._orb_indicator.RangeHigh, false);
                //SetProfitTarget(_orderentershort.Name, CalculationMode.Price, this._orb_indicator.TargetShort);
            }
        }

        /// <summary>
        /// Exit the LONG position.
        /// </summary>
        private void DoExitLong()
        {
            if (_orderenterlong != null)
            {
                 ExitLong(this._orderenterlong.Name);
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
                ExitShort(this._orderentershort.Name);
                this._orderentershort = null;
            }
        }

        public override string ToString()
        {
            return "Dummy one minute even/odd (S)";
        }

        public override string DisplayName
        {
            get
            {
                return "Dummy one minute even/odd (S)";
            }
        }


        #region Properties

        #region Interface

        /// <summary>
        /// </summary>
        [Description("If true it is allowed to create long positions.")]
        [Category("Parameters")]
        [DisplayName("Allow Long")]
        public bool IsLongEnabled
        {
            get { return _IsLongEnabled; }
            set { _IsLongEnabled = value; }
        }


        /// <summary>
        /// </summary>
        [Description("If true it is allowed to create short positions.")]
        [Category("Parameters")]
        [DisplayName("Allow Short")]
        public bool IsShortEnabled
        {
            get { return _IsShortEnabled; }
            set { _IsShortEnabled = value; }
        }


        [Browsable(false)]
        [XmlIgnore()]
        public bool ErrorOccured
        {
            get { return _ErrorOccured; }
            set { _ErrorOccured = value; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public bool WarningOccured
        {
            get { return _WarningOccured; }
            set { _WarningOccured = value; }
        }

        #endregion

        #region Input

        [Description("If true the strategy will handle everything. It will create buy orders, sell orders, stop loss orders, targets fully automatically")]
        [Category("Safety first!")]
        [DisplayName("Autopilot")]
        public bool Autopilot
        {
            get { return _autopilot; }
            set { _autopilot = value; }
        }


       

        #endregion

        #endregion

    }
}

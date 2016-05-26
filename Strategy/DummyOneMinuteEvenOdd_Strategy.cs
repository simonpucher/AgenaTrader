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
/// Version: 1.1.2
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
    public class DummyOneMinuteEvenOdd_Strategy : UserStrategy
	{
        //input
        private bool _autopilot = true;

        //output


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
        }

		protected override void OnBarUpdate()
		{
            //Set Autopilot
            this.IsAutomated = this.Autopilot;

            //exit on error
            if (IsError)
            {
                return;
            }

            //get the indicator
            DummyOneMinuteEvenOdd_Indicator _DummyOneMinuteEvenOdd_Indicator = LeadIndicator.DummyOneMinuteEvenOdd_Indicator();

            //get the value
            double returnvalue = _DummyOneMinuteEvenOdd_Indicator[0];

            //Entry
            if (returnvalue == 1)
            {
                this.DoEnterLong();
            }
            else if (returnvalue == -1)
            {
                this.DoEnterShort();
            }


		}


        /// <summary>
        /// Create LONG order.
        /// </summary>
        private void DoEnterLong()
        {
           IOrder _orderenterlong = EnterLong(1, this.DisplayName + "_" + OrderAction.Buy + "_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(), this.Instrument, this.TimeFrame);
            SetStopLoss(_orderenterlong.Name, CalculationMode.Price, Bars[0].Close * 0.99, false);
            SetProfitTarget(_orderenterlong.Name, CalculationMode.Price, Bars[0].Close * 1.01); 
        }

        /// <summary>
        /// Create SHORT order.
        /// </summary>
        private void DoEnterShort()
        {
            IOrder _orderentershort = EnterShort(1, this.DisplayName + "_" + OrderAction.SellShort + "_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(), this.Instrument, this.TimeFrame);
            SetStopLoss(_orderentershort.Name, CalculationMode.Price, Bars[0].Close * 1.01, false);
            SetProfitTarget(_orderentershort.Name, CalculationMode.Price, Bars[0].Close * 0.99);
        }

     

        public override string ToString()
        {
            return "Dummy even/odd (S)";
        }

        public override string DisplayName
        {
            get
            {
                return "Dummy even/odd (S)";
            }
        }


        #region Properties


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

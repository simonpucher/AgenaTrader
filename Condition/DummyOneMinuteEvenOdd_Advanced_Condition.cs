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
    [IsEntryAttribute(true)]
    [IsStopAttribute(false)]
    [IsTargetAttribute(false)]
    [OverrulePreviousStopPrice(false)]
    public class DummyOneMinuteEvenOddEntry_Advanced_Condition : UserScriptedCondition, IDummyOneMinuteEvenOdd_Advanced
    {
        //interface
        private bool _IsShortEnabled = true;
        private bool _IsLongEnabled = true;
        private bool _WarningOccured = false;
        private bool _ErrorOccured = false;

        //input

        //output

        //internal
        private DummyOneMinuteEvenOdd_Advanced_Indicator _DummyOneMinuteEvenOdd_Advanced_Indicator = null;
        private IOrder _orderenterlong;
        private IOrder _orderentershort;

        private int _myvalue = 1;

        protected override void Initialize()
        {
            //Print("Initialize");
            IsEntry = true;
            IsStop = false;
            IsTarget = false;
            Add(new Plot(Const.DefaultIndicatorColor, "Occurred"));
            Add(new Plot(Const.DefaultIndicatorColor_GreyedOut, "Entry"));

            Overlay = false;
            CalculateOnBarClose = true;

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
            //Print("OnStartUp");
            base.OnStartUp();

            //Init our indicator to get code access to the calculate method
            this._DummyOneMinuteEvenOdd_Advanced_Indicator = new DummyOneMinuteEvenOdd_Advanced_Indicator();

            this.ErrorOccured = false;
            this.WarningOccured = false;
        }


        protected override void OnBarUpdate()
        {
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
                        //Long Signal
                        Occurred.Set(1);
                        //Entry.Set(Close[0]);
                        break;
                    case OrderAction.SellShort:
                        //Short Signal
                        Occurred.Set(-1);
                        //Entry.Set(Close[0]);
                        break;
                }
            }
            else
            {
                //No Signal
                Occurred.Set(0);
                //Entry.Set(Close[0]);
            }

        }

        public override string ToString()
        {
            return "Dummy one minute even/odd (C)";
        }

        public override string DisplayName
        {
            get
            {
                return "Dummy one minute even/odd (C)";
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

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Occurred
        {
            get { return Values[0]; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Entry
        {
            get { return Values[1]; }
        }

        public override IList<DataSeries> GetEntries()
        {
            return new[] { Entry };
        }

        #endregion
    }
}

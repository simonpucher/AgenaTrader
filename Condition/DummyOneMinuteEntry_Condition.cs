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
/// Version: 1.1
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
    public class DummyOneMinuteEntry_Condition : UserScriptedCondition, IDummyOneMinuteEven
    {
        //interface
        private bool _IsShortEnabled = true;
        private bool _IsLongEnabled = true;

        //input

        //output

        //internal
        private DummyOneMinuteEven_Indicator _DummyOneMinuteEven_Indicator = null;
        private IOrder _orderenterlong;
        private IOrder _orderentershort;

        private int _myvalue = 1;

        protected override void Initialize()
        {
            //Print("Initialize");
            IsEntry = true;
            IsStop = false;
            IsTarget = false;
            Add(new Plot(Color.FromKnownColor(KnownColor.Black), "Occurred"));
            Add(new Plot(Color.FromArgb(255, 153, 170, 61), "Entry"));
            Overlay = true;
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
            this._DummyOneMinuteEven_Indicator = new DummyOneMinuteEven_Indicator();
        }


        protected override void OnBarUpdate()
        {
            //Check if peridocity is valid for this script 
            if (!this._DummyOneMinuteEven_Indicator.DatafeedPeriodicityIsValid(Bars.TimeFrame))
            {
                Log(this.DisplayName + ": " + Const.DefaultStringDatafeedPeriodicity, InfoLogLevel.AlertLog);
                return;
            }

            //Lets call the calculate method and save the result with the trade action
            ResultValueDummyOneMinuteEven returnvalue = this._DummyOneMinuteEven_Indicator.calculate(Bars[0], this.IsLongEnabled, this.IsShortEnabled);

            //if (DummyOneMinuteEven_Indicator()[0] == 100)
            //{
            //    Occurred.Set(-1);
            //}
            //else
            //{
            //    Occurred.Set(1);
            //}

            //Entry.Set(iv_Bars.GetOpen(iv_CurrentBar));
            //Entry.Set(10390);
            //Entry.Set(GetCurrentBid());

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

        #region Input
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

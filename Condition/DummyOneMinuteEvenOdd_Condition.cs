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
    public class DummyOneMinuteEvenOddEntry_Condition : UserScriptedCondition
    {
  
        //input

        //output

        //internal


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
        }


        protected override void OnBarUpdate()
        {
           //exit on error
            if (IsError)
            {
                return;
            }

            //get the indicator
            DummyOneMinuteEvenOdd_Indicator _DummyOneMinuteEvenOdd_Indicator = LeadIndicator.DummyOneMinuteEvenOdd_Indicator();

            //get the value
            double returnvalue = _DummyOneMinuteEvenOdd_Indicator[0];
            
            //set the value
            Occurred.Set(returnvalue);
   
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

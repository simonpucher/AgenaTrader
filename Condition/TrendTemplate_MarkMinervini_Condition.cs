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
/// Version: 1.1.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// This indicator provides trend template by Mark Minervini signals http://www.stockfetcher.com/forums2/Filter-Exchange/Trend-Template-by-Mark-Minervini/125969
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("Geben Sie bitte hier die Beschreibung für die neue Condition ein")]
	[IsEntryAttribute(true)]
	[IsStopAttribute(false)]
	[IsTargetAttribute(false)]
	[OverrulePreviousStopPrice(false)]
	public class TrendTemplate_MarkMinervini_Condition : UserScriptedCondition
	{
        //input 


        //output


        //internal
        private bool ErrorOccured = false;
        private TrendTemplate_MarkMinervini_Indicator _TrendTemplate_MarkMinervini_Indicator = null;


        protected override void OnInit()
        {
            IsEntry = true;
            IsStop = false;
            IsTarget = false;
            Add(new OutputDescriptor(Const.DefaultIndicatorColor, "Occurred"));
            Add(new OutputDescriptor(Const.DefaultIndicatorColor, "Entry"));

            IsOverlay = false;
            CalculateOnClosedBar = true;

            //We need at least xy bars
            this.RequiredBarsCount = 130;

        }


        protected override void OnStart()
        {
            base.OnStart();

            //Init our indicator to get code access to the calculate method
            this._TrendTemplate_MarkMinervini_Indicator = new TrendTemplate_MarkMinervini_Indicator();
            
            this.ErrorOccured = false;
        }



        protected override void OnCalculate()
        {

            //calculate data
            ResultValue returnvalue = this._TrendTemplate_MarkMinervini_Indicator.calculate(this.InSeries);

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
                    case OrderDirection.Buy:
                        Occurred.Set(1);
                        break;
                }
            }
            else
            {
                Occurred.Set(0);
            }

        }


        public override string ToString()
        {
            return "TT Mark Minervini (C)";
        }

        public override string DisplayName
        {
            get
            {
                return "TT Mark Minervini (C)";
            }
        }




     
        #region InSeries

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Occurred
        {
            get { return Outputs[0]; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Entry
        {
            get { return Outputs[1]; }
        }

        public override IList<DataSeries> GetEntries()
        {
            return new[] { Entry };
        }


        #endregion
    }
}
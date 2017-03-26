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
/// Version: 1.0.1
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
            Add(new Plot(Const.DefaultIndicatorColor, "Occurred"));
            Add(new Plot(Const.DefaultIndicatorColor, "Entry"));

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
                    case OrderAction.Buy:
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

#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator
	{
		/// <summary>
		/// Geben Sie bitte hier die Beschreibung für die neue Condition ein
		/// </summary>
		public TrendTemplate_MarkMinervini_Condition TrendTemplate_MarkMinervini_Condition()
        {
			return TrendTemplate_MarkMinervini_Condition(InSeries);
		}

		/// <summary>
		/// Geben Sie bitte hier die Beschreibung für die neue Condition ein
		/// </summary>
		public TrendTemplate_MarkMinervini_Condition TrendTemplate_MarkMinervini_Condition(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<TrendTemplate_MarkMinervini_Condition>(input);

			if (indicator != null)
				return indicator;

			indicator = new TrendTemplate_MarkMinervini_Condition
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input
						};
			indicator.SetUp();

			CachedCalculationUnits.AddIndicator2Cache(indicator);

			return indicator;
		}
	}

	#endregion

	#region Strategy

	public partial class UserStrategy
	{
		/// <summary>
		/// Geben Sie bitte hier die Beschreibung für die neue Condition ein
		/// </summary>
		public TrendTemplate_MarkMinervini_Condition TrendTemplate_MarkMinervini_Condition()
		{
			return LeadIndicator.TrendTemplate_MarkMinervini_Condition(InSeries);
		}

		/// <summary>
		/// Geben Sie bitte hier die Beschreibung für die neue Condition ein
		/// </summary>
		public TrendTemplate_MarkMinervini_Condition TrendTemplate_MarkMinervini_Condition(IDataSeries input)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.TrendTemplate_MarkMinervini_Condition(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Geben Sie bitte hier die Beschreibung für die neue Condition ein
		/// </summary>
		public TrendTemplate_MarkMinervini_Condition TrendTemplate_MarkMinervini_Condition()
		{
			return LeadIndicator.TrendTemplate_MarkMinervini_Condition(InSeries);
		}

		/// <summary>
		/// Geben Sie bitte hier die Beschreibung für die neue Condition ein
		/// </summary>
		public TrendTemplate_MarkMinervini_Condition TrendTemplate_MarkMinervini_Condition(IDataSeries input)
		{
			return LeadIndicator.TrendTemplate_MarkMinervini_Condition(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Geben Sie bitte hier die Beschreibung für die neue Condition ein
		/// </summary>
		public TrendTemplate_MarkMinervini_Condition TrendTemplate_MarkMinervini_Condition()
		{
			return LeadIndicator.TrendTemplate_MarkMinervini_Condition(InSeries);
		}

		/// <summary>
		/// Geben Sie bitte hier die Beschreibung für die neue Condition ein
		/// </summary>
		public TrendTemplate_MarkMinervini_Condition TrendTemplate_MarkMinervini_Condition(IDataSeries input)
		{
			return LeadIndicator.TrendTemplate_MarkMinervini_Condition(input);
		}
	}

	#endregion

}

#endregion

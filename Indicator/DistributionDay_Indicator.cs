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
	/// <summary>
/// Version: 1.1.1
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://github.com/ScriptTrading/Basic-Package/blob/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
    [Description("Take your money and run when the smart money start the distribution.")]
	public class DistributionDay_Indicator : UserIndicator
	{
        private Queue<DateTime> _distributionlist = null;
        private int _period = 25;
        private double _percent = 0.2;
        private bool _showdistributiondayarrows = true;
        private int _distributiondaycount = 4;

        protected override void OnInit()
		{
			//Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
			CalculateOnClosedBar = false;
            this.IsOverlay = true;
		}

        protected override void OnCalculate()
        {
            //http://www.optionstradingiq.com/distribution-days-can-foreshadow-a-correction/

            //Init list on startup
            if (ProcessingBarIndex == 0)
            {
                _distributionlist = new Queue<DateTime>();
            }

            //Delete all old 
            if (this._distributionlist.Count() > 0 && this._distributionlist.Peek() <= Time[0].AddDays(this.Period * (-1)))
            {
                this._distributionlist.Dequeue();
            }

            //Draw Disrtibution Arrow.
            if (Volume[0] > Volume[1] && ((Close[1] - Close[0]) / Close[1]) > (this.Percent / 100.0))
            {
               
                this._distributionlist.Enqueue(Time[0]);

                if (ShowDistributionDayArrows)
                {
                    AddChartArrowDown(ProcessingBarIndex.ToString(), true, 0, High[0], Color.LightPink);
                }

                //Draw the indicator
                if (this._distributionlist.Count > this.DistributionDayCount)
                {
                    //MyPlot1.Set(1);
                    AddChartArrowDown(ProcessingBarIndex.ToString(), true, 0, High[0], Color.DeepPink);
                }
               
            }
            else
            {
                //MyPlot1.Set(0);
            }
		}

		#region Properties

		//[Browsable(false)]
		//[XmlIgnore()]
		//public DataSeries MyPlot1
		//{
		//	get { return Outputs[0]; }
		//}

        /// <summary>
        /// </summary>
        [Description("Period which will be used to count distribution days.")]
        [Category("Parameters")]
        [DisplayName("Period")]
        public int Period
        {
            get { return _period; }
            set { _period = value; }
        }

        [Description("Period which will be used to count distribution days.")]
        [Category("Parameters")]
        [DisplayName("Distribution Day Count")]
        public int DistributionDayCount
        {
            get { return _distributiondaycount; }
            set { _distributiondaycount = value; }
        }

        [Description("Percent down to count as a distribution day.")]
        [Category("Parameters")]
        [DisplayName("Percent")]
        public double Percent
        {
            get { return _percent; }
            set { _percent = value; }
        }

        
        [Description("Show all distribution day arrows.")]
        [Category("Parameters")]
        [DisplayName("Show all arrows")]
        public bool ShowDistributionDayArrows
        {
            get { return _showdistributiondayarrows; }
            set { _showdistributiondayarrows = value; }
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
		/// Take your money and run when the smart money start the distribution.
		/// </summary>
		public DistributionDay_Indicator DistributionDay_Indicator(System.Int32 period, System.Int32 distributionDayCount, System.Double percent, System.Boolean showDistributionDayArrows)
        {
			return DistributionDay_Indicator(InSeries, period, distributionDayCount, percent, showDistributionDayArrows);
		}

		/// <summary>
		/// Take your money and run when the smart money start the distribution.
		/// </summary>
		public DistributionDay_Indicator DistributionDay_Indicator(IDataSeries input, System.Int32 period, System.Int32 distributionDayCount, System.Double percent, System.Boolean showDistributionDayArrows)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<DistributionDay_Indicator>(input, i => i.Period == period && i.DistributionDayCount == distributionDayCount && Math.Abs(i.Percent - percent) <= Double.Epsilon && i.ShowDistributionDayArrows == showDistributionDayArrows);

			if (indicator != null)
				return indicator;

			indicator = new DistributionDay_Indicator
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input,
							Period = period,
							DistributionDayCount = distributionDayCount,
							Percent = percent,
							ShowDistributionDayArrows = showDistributionDayArrows
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
		/// Take your money and run when the smart money start the distribution.
		/// </summary>
		public DistributionDay_Indicator DistributionDay_Indicator(System.Int32 period, System.Int32 distributionDayCount, System.Double percent, System.Boolean showDistributionDayArrows)
		{
			return LeadIndicator.DistributionDay_Indicator(InSeries, period, distributionDayCount, percent, showDistributionDayArrows);
		}

		/// <summary>
		/// Take your money and run when the smart money start the distribution.
		/// </summary>
		public DistributionDay_Indicator DistributionDay_Indicator(IDataSeries input, System.Int32 period, System.Int32 distributionDayCount, System.Double percent, System.Boolean showDistributionDayArrows)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.DistributionDay_Indicator(input, period, distributionDayCount, percent, showDistributionDayArrows);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Take your money and run when the smart money start the distribution.
		/// </summary>
		public DistributionDay_Indicator DistributionDay_Indicator(System.Int32 period, System.Int32 distributionDayCount, System.Double percent, System.Boolean showDistributionDayArrows)
		{
			return LeadIndicator.DistributionDay_Indicator(InSeries, period, distributionDayCount, percent, showDistributionDayArrows);
		}

		/// <summary>
		/// Take your money and run when the smart money start the distribution.
		/// </summary>
		public DistributionDay_Indicator DistributionDay_Indicator(IDataSeries input, System.Int32 period, System.Int32 distributionDayCount, System.Double percent, System.Boolean showDistributionDayArrows)
		{
			return LeadIndicator.DistributionDay_Indicator(input, period, distributionDayCount, percent, showDistributionDayArrows);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Take your money and run when the smart money start the distribution.
		/// </summary>
		public DistributionDay_Indicator DistributionDay_Indicator(System.Int32 period, System.Int32 distributionDayCount, System.Double percent, System.Boolean showDistributionDayArrows)
		{
			return LeadIndicator.DistributionDay_Indicator(InSeries, period, distributionDayCount, percent, showDistributionDayArrows);
		}

		/// <summary>
		/// Take your money and run when the smart money start the distribution.
		/// </summary>
		public DistributionDay_Indicator DistributionDay_Indicator(IDataSeries input, System.Int32 period, System.Int32 distributionDayCount, System.Double percent, System.Boolean showDistributionDayArrows)
		{
			return LeadIndicator.DistributionDay_Indicator(input, period, distributionDayCount, percent, showDistributionDayArrows);
		}
	}

	#endregion

}

#endregion

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
/// Version: 1.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// https://www.youtube.com/watch?v=RpHOt08GRlE
/// http://investdaily.custhelp.com/app/answers/detail/a_id/276/~/what-exactly-is-a-distribution-day-in-a-market-index%3F
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://github.com/ScriptTrading/Basic-Package/blob/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
    [Description("Enter the description for the new custom indicator here")]
	public class DistributionDay_Indicator : UserIndicator
	{
        private Queue<DateTime> _distributionlist = null;

        protected override void OnInit()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
			CalculateOnClosedBar = true;
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
            if (this._distributionlist.Count() > 0 && this._distributionlist.Peek() <= Time[0].AddDays(-25))
            {
                this._distributionlist.Dequeue();
            }

            //Draw Disrtibution Arrow.
            if (Volume[0] > Volume[1] && ((Close[1] - Close[0]) / Close[1]) > 0.002)
            {
                AddChartArrowDown(ProcessingBarIndex.ToString(), true, 0, High[0] + 3 * TickSize, Color.Blue);
                this._distributionlist.Enqueue(Time[0]);
                
                //Draw the indicator
                if (this._distributionlist.Count > 4)
                {
                    MyPlot1.Set(1);
                }
                else {
                    MyPlot1.Set(0);
                }
            }
            else
            {
                MyPlot1.Set(0);
            }
		}

		#region Properties

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries MyPlot1
		{
			get { return Outputs[0]; }
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
		/// Enter the description for the new custom indicator here
		/// </summary>
		public DistributionDay_Indicator DistributionDay_Indicator()
        {
			return DistributionDay_Indicator(InSeries);
		}

		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public DistributionDay_Indicator DistributionDay_Indicator(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<DistributionDay_Indicator>(input);

			if (indicator != null)
				return indicator;

			indicator = new DistributionDay_Indicator
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
		/// Enter the description for the new custom indicator here
		/// </summary>
		public DistributionDay_Indicator DistributionDay_Indicator()
		{
			return LeadIndicator.DistributionDay_Indicator(InSeries);
		}

		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public DistributionDay_Indicator DistributionDay_Indicator(IDataSeries input)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.DistributionDay_Indicator(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public DistributionDay_Indicator DistributionDay_Indicator()
		{
			return LeadIndicator.DistributionDay_Indicator(InSeries);
		}

		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public DistributionDay_Indicator DistributionDay_Indicator(IDataSeries input)
		{
			return LeadIndicator.DistributionDay_Indicator(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public DistributionDay_Indicator DistributionDay_Indicator()
		{
			return LeadIndicator.DistributionDay_Indicator(InSeries);
		}

		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public DistributionDay_Indicator DistributionDay_Indicator(IDataSeries input)
		{
			return LeadIndicator.DistributionDay_Indicator(input);
		}
	}

	#endregion

}

#endregion

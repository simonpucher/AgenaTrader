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
/// Version: 1.0.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2017
/// -------------------------------------------------------------------------
/// King Pinball by Traderfox: https://youtu.be/bwFGeUVmF5o
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("King Pinball")]
	public class King_Pinball_Indicator : UserIndicator
	{

        bool shortsignalbb = false;
        bool longsignalbb = false;

        protected override void OnInit()
		{
			AddPlot(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
			CalculateOnClosedBar = true;
		}

		protected override void OnCalculate()
		{

            if (Close[0] < Bollinger(2, 100).Lower[0])
            {
                //AddChartArrowUp(Time[0].ToString(), 0, Low[0], Color.Green);
                longsignalbb = true;
            }
            else if (Close[0] > Bollinger(2, 20).Upper[0])
            {
                //AddChartArrowDown(Time[0].ToString(), 0, High[0], Color.Red);
                shortsignalbb = true;
            }
            else
            {
                //nothing
            }

            MACD macd = MACD(5, 50, 25);

            if (longsignalbb && CrossAbove(macd.Default, macd.Avg, 0))
            {
                AddChartArrowUp(Time[0].ToString()+"long", 0, Low[0], Color.Green);
                MyPlot1.Set(1);
                longsignalbb = false;
            }
            else if (shortsignalbb && CrossBelow(macd.Default, macd.Avg, 0))
            {
                AddChartArrowDown(Time[0].ToString()+"short", 0, High[0], Color.Red);
                MyPlot1.Set(-1);
                shortsignalbb = false;
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
#region AgenaTrader Automaticaly Generated Code. Do not change it manually

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator
	{
		/// <summary>
		/// King Pinball
		/// </summary>
		public King_Pinball_Indicator King_Pinball_Indicator()
        {
			return King_Pinball_Indicator(InSeries);
		}

		/// <summary>
		/// King Pinball
		/// </summary>
		public King_Pinball_Indicator King_Pinball_Indicator(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<King_Pinball_Indicator>(input);

			if (indicator != null)
				return indicator;

			indicator = new King_Pinball_Indicator
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
		/// King Pinball
		/// </summary>
		public King_Pinball_Indicator King_Pinball_Indicator()
		{
			return LeadIndicator.King_Pinball_Indicator(InSeries);
		}

		/// <summary>
		/// King Pinball
		/// </summary>
		public King_Pinball_Indicator King_Pinball_Indicator(IDataSeries input)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.King_Pinball_Indicator(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// King Pinball
		/// </summary>
		public King_Pinball_Indicator King_Pinball_Indicator()
		{
			return LeadIndicator.King_Pinball_Indicator(InSeries);
		}

		/// <summary>
		/// King Pinball
		/// </summary>
		public King_Pinball_Indicator King_Pinball_Indicator(IDataSeries input)
		{
			return LeadIndicator.King_Pinball_Indicator(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// King Pinball
		/// </summary>
		public King_Pinball_Indicator King_Pinball_Indicator()
		{
			return LeadIndicator.King_Pinball_Indicator(InSeries);
		}

		/// <summary>
		/// King Pinball
		/// </summary>
		public King_Pinball_Indicator King_Pinball_Indicator(IDataSeries input)
		{
			return LeadIndicator.King_Pinball_Indicator(input);
		}
	}

	#endregion

}

#endregion

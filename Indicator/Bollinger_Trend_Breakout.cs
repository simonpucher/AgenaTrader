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
/// https://www.godmode-trader.de/know-how/bollinger-trend-breakout-trendfolge-leicht-gemacht,4632226
/// /// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	public class Bollinger_Trend_Breakout : UserIndicator
	{
		protected override void OnInit()
		{
			IsOverlay = true;


            this.RequiredBarsCount = 40;
        }

		protected override void OnCalculate()
		{
            Bollinger bb = Bollinger(1, 40);

            if (Close[0] > bb.Upper[0])
            {
                this.BackColor = Color.Green;
            }
            else if (Close[0] < bb.Lower[0])
            {
                this.BackColor = Color.Red;
            }
            else
            {
                this.BackColor = Color.Yellow;
            }

		}



        public override string ToString()
        {
            return "Bollinger Trend Breakout (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "Bollinger Trend Breakout (I)";
            }
        }
    }
}
#region AgenaTrader Automaticaly Generated Code. Do not change it manually

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator
	{
		public Bollinger_Trend_Breakout Bollinger_Trend_Breakout()
        {
			return Bollinger_Trend_Breakout(InSeries);
		}

		public Bollinger_Trend_Breakout Bollinger_Trend_Breakout(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Bollinger_Trend_Breakout>(input);

			if (indicator != null)
				return indicator;

			indicator = new Bollinger_Trend_Breakout
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
		public Bollinger_Trend_Breakout Bollinger_Trend_Breakout()
		{
			return LeadIndicator.Bollinger_Trend_Breakout(InSeries);
		}

		public Bollinger_Trend_Breakout Bollinger_Trend_Breakout(IDataSeries input)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.Bollinger_Trend_Breakout(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		public Bollinger_Trend_Breakout Bollinger_Trend_Breakout()
		{
			return LeadIndicator.Bollinger_Trend_Breakout(InSeries);
		}

		public Bollinger_Trend_Breakout Bollinger_Trend_Breakout(IDataSeries input)
		{
			return LeadIndicator.Bollinger_Trend_Breakout(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		public Bollinger_Trend_Breakout Bollinger_Trend_Breakout()
		{
			return LeadIndicator.Bollinger_Trend_Breakout(InSeries);
		}

		public Bollinger_Trend_Breakout Bollinger_Trend_Breakout(IDataSeries input)
		{
			return LeadIndicator.Bollinger_Trend_Breakout(input);
		}
	}

	#endregion

}

#endregion

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
/// Simon Pucher 2017
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("Draw the candle of the higher timeframe on the chart.")]
	public class HigherTimeFrameCandle_Indicator : UserIndicator
	{
        private static readonly TimeFrame TF_Day = new TimeFrame(DatafeedHistoryPeriodicity.Day, 1);
        private static readonly TimeFrame TF_Week = new TimeFrame(DatafeedHistoryPeriodicity.Week, 1);

        protected override void OnBarsRequirements()
        {
            Add(TF_Day);
        }

        protected override void OnInit()
		{
			//Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
			IsOverlay = true;
            CalculateOnClosedBar = true;
        }

		protected override void OnCalculate()
		{
  
            if (ProcessingBarSeriesIndex == 1)
            {
                Color _col = Color.Gray;
                if (Opens[1][0] > Closes[1][0])
                {
                    _col = Color.Red;
                }
                else if (Opens[1][0] < Closes[1][0])
                {
                    _col = Color.Green;
                }
                //MyPlot1.Set(InSeries[0]);
                
                AddChartRectangle("HTFCandle-" + ProcessingBarIndex, true, Times[1][0], Lows[1][0], Times[1][0].AddSeconds(TimeFrames[1].GetSeconds()), Highs[1][0], _col, _col, 50);
            }
            
        }

        public override string ToString()
        {
            return "Higher TimeFrame Candle (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "Higher TimeFrame Candle (I)";
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
		/// Draw the candle of the higher timeframe on the chart.
		/// </summary>
		public HigherTimeFrameCandle_Indicator HigherTimeFrameCandle_Indicator()
        {
			return HigherTimeFrameCandle_Indicator(InSeries);
		}

		/// <summary>
		/// Draw the candle of the higher timeframe on the chart.
		/// </summary>
		public HigherTimeFrameCandle_Indicator HigherTimeFrameCandle_Indicator(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<HigherTimeFrameCandle_Indicator>(input);

			if (indicator != null)
				return indicator;

			indicator = new HigherTimeFrameCandle_Indicator
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
		/// Draw the candle of the higher timeframe on the chart.
		/// </summary>
		public HigherTimeFrameCandle_Indicator HigherTimeFrameCandle_Indicator()
		{
			return LeadIndicator.HigherTimeFrameCandle_Indicator(InSeries);
		}

		/// <summary>
		/// Draw the candle of the higher timeframe on the chart.
		/// </summary>
		public HigherTimeFrameCandle_Indicator HigherTimeFrameCandle_Indicator(IDataSeries input)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.HigherTimeFrameCandle_Indicator(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Draw the candle of the higher timeframe on the chart.
		/// </summary>
		public HigherTimeFrameCandle_Indicator HigherTimeFrameCandle_Indicator()
		{
			return LeadIndicator.HigherTimeFrameCandle_Indicator(InSeries);
		}

		/// <summary>
		/// Draw the candle of the higher timeframe on the chart.
		/// </summary>
		public HigherTimeFrameCandle_Indicator HigherTimeFrameCandle_Indicator(IDataSeries input)
		{
			return LeadIndicator.HigherTimeFrameCandle_Indicator(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Draw the candle of the higher timeframe on the chart.
		/// </summary>
		public HigherTimeFrameCandle_Indicator HigherTimeFrameCandle_Indicator()
		{
			return LeadIndicator.HigherTimeFrameCandle_Indicator(InSeries);
		}

		/// <summary>
		/// Draw the candle of the higher timeframe on the chart.
		/// </summary>
		public HigherTimeFrameCandle_Indicator HigherTimeFrameCandle_Indicator(IDataSeries input)
		{
			return LeadIndicator.HigherTimeFrameCandle_Indicator(input);
		}
	}

	#endregion

}

#endregion

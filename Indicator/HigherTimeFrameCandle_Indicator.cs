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
/// Version: 1.0.5
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
        private static readonly TimeFrame TF_Month = new TimeFrame(DatafeedHistoryPeriodicity.Month, 1);

        private Color _color_long_signal_background = Const.DefaultArrowLongColor;
        private Color _color_short_signal_background = Const.DefaultArrowShortColor;
        private int _opacity_signal = 25;

        protected override void OnBarsRequirements()
        {
            Add(TF_Day);
            Add(TF_Week);
            Add(TF_Month);
        }

        protected override void OnInit()
		{
			//Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
			IsOverlay = true;
            CalculateOnClosedBar = false;
            
        }

		protected override void OnCalculate()
		{
            //Always show the day candle
            int _timeseriescount = 1;

            if (ProcessingBarSeriesIndex == 1)
            {
                //Change time frame to higher candles
                if (this.TimeFrame.Periodicity == DatafeedHistoryPeriodicity.Day)
                {
                    _timeseriescount = 2;
                }
                else if (this.TimeFrame.Periodicity == DatafeedHistoryPeriodicity.Week)
                {
                    _timeseriescount = 3;
                }

                //Drawing
                Color _col = Color.Gray;
                if (Opens[_timeseriescount][1] > Closes[_timeseriescount][1])
                {
                    _col = this.ColorShortSignalBackground;
                }
                else if (Opens[_timeseriescount][1] < Closes[_timeseriescount][1])
                {
                    _col = this.ColorLongSignalBackground;
                }


                DateTime mystart = Times[_timeseriescount][1];
                DateTime myend = Times[_timeseriescount][0].AddSeconds(-1);
                AddChartRectangle("HTFCandle-" + Times[_timeseriescount][1], true, mystart, Lows[_timeseriescount][1], myend, Highs[_timeseriescount][1], _col, _col, this.OpacitySignal);



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

        /// <summary>
        /// </summary>
        [Description("Select opacity for the background in percent.")]
        [Category("Background")]
        [DisplayName("Opacity Background %")]
        public int OpacitySignal
        {
            get { return _opacity_signal; }
            set
            {
                if (value < 0) value = 0;
                if (value > 100) value = 100;
                _opacity_signal = value;
            }
        }

   


        /// <summary>
        /// </summary>
        [Description("Select Color for the background in long setup.")]
        [Category("Background")]
        [DisplayName("Color Background Long")]
        public Color ColorLongSignalBackground
        {
            get { return _color_long_signal_background; }
            set { _color_long_signal_background = value; }
        }


        // Serialize Color object
        [Browsable(false)]
        public string ColorLongSignalBackgroundSerialize
        {
            get { return SerializableColor.ToString(_color_long_signal_background); }
            set { _color_long_signal_background = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Select Color for the background in short setup.")]
        [Category("Background")]
        [DisplayName("Color Background Short")]
        public Color ColorShortSignalBackground
        {
            get { return _color_short_signal_background; }
            set { _color_short_signal_background = value; }
        }
        // Serialize Color object
        [Browsable(false)]
        public string ColorShortSignalBackgroundSerialize
        {
            get { return SerializableColor.ToString(_color_short_signal_background); }
            set { _color_short_signal_background = SerializableColor.FromString(value); }
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

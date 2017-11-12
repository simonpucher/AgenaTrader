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

        private int _period = 40;
        private int _stddev = 1;

        private Color _color_long_signal_background = Const.DefaultArrowLongColor;
        private Color _color_short_signal_background = Const.DefaultArrowShortColor;
        private int _opacity_long_signal = 25;
        private int _opacity_short_signal = 25;

        private DataSeries _signals;

        protected override void OnInit()
		{
			IsOverlay = true;

            _signals = new DataSeries(this);

            this.RequiredBarsCount = 40;
        }

		protected override void OnCalculate()
		{
            Bollinger bb = Bollinger(this.StandardDeviation, this.Period);

            if (Close[0] > bb.Upper[0])
            {
                this.BackColor = GlobalUtilities.AdjustOpacity(this.ColorLongSignalBackground, this.OpacityLongSignal / 100.0);
                _signals.Set(1);
            }
            else if (Close[0] < bb.Lower[0])
            {
                this.BackColor = GlobalUtilities.AdjustOpacity(this.ColorShortSignalBackground, this.OpacityShortSignal / 100.0);
                _signals.Set(-1);
            }
            else
            {
                //this.BackColor = Color.Yellow;
                _signals.Set(_signals[1]);
                if (_signals[1] == 1)
                {
                    this.BackColor = GlobalUtilities.AdjustOpacity(this.ColorLongSignalBackground, this.OpacityLongSignal / 100.0);
                }
                else
                {
                    this.BackColor = GlobalUtilities.AdjustOpacity(this.ColorShortSignalBackground, this.OpacityShortSignal / 100.0);
                }
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


        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Signals { get { return _signals; } }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Plot_1 { get { return Outputs[0]; } }


        /// <summary>
        /// </summary>
        [Description("Period for the Bollinger Band")]
        [Category("Parameters")]
        [DisplayName("BB Period")]
        public int Period
        {
            get { return _period; }
            set
            {
                _period = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("Standard Deviation for the Bollinger Band")]
        [Category("Parameters")]
        [DisplayName("BB Std. Dev.")]
        public int StandardDeviation
        {
            get { return _stddev; }
            set
            {
                _stddev = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("Select opacity for the background in long setup in percent.")]
        [Category("Background")]
        [DisplayName("Opacity Background Long %")]
        public int OpacityLongSignal
        {
            get { return _opacity_long_signal; }
            set
            {
                if (value < 0) value = 0;
                if (value > 100) value = 100;
                _opacity_long_signal = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("Select opacity for the background in short setup in percent.")]
        [Category("Background")]
        [DisplayName("Opacity Background Short %")]
        public int OpacityShortSignal
        {
            get { return _opacity_short_signal; }
            set
            {
                if (value < 0) value = 0;
                if (value > 100) value = 100;
                _opacity_short_signal = value;
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

    }
}
#region AgenaTrader Automaticaly Generated Code. Do not change it manually

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator
	{
		public Bollinger_Trend_Breakout Bollinger_Trend_Breakout(System.Int32 period, System.Int32 standardDeviation)
        {
			return Bollinger_Trend_Breakout(InSeries, period, standardDeviation);
		}

		public Bollinger_Trend_Breakout Bollinger_Trend_Breakout(IDataSeries input, System.Int32 period, System.Int32 standardDeviation)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Bollinger_Trend_Breakout>(input, i => i.Period == period && i.StandardDeviation == standardDeviation);

			if (indicator != null)
				return indicator;

			indicator = new Bollinger_Trend_Breakout
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input,
							Period = period,
							StandardDeviation = standardDeviation
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
		public Bollinger_Trend_Breakout Bollinger_Trend_Breakout(System.Int32 period, System.Int32 standardDeviation)
		{
			return LeadIndicator.Bollinger_Trend_Breakout(InSeries, period, standardDeviation);
		}

		public Bollinger_Trend_Breakout Bollinger_Trend_Breakout(IDataSeries input, System.Int32 period, System.Int32 standardDeviation)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.Bollinger_Trend_Breakout(input, period, standardDeviation);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		public Bollinger_Trend_Breakout Bollinger_Trend_Breakout(System.Int32 period, System.Int32 standardDeviation)
		{
			return LeadIndicator.Bollinger_Trend_Breakout(InSeries, period, standardDeviation);
		}

		public Bollinger_Trend_Breakout Bollinger_Trend_Breakout(IDataSeries input, System.Int32 period, System.Int32 standardDeviation)
		{
			return LeadIndicator.Bollinger_Trend_Breakout(input, period, standardDeviation);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		public Bollinger_Trend_Breakout Bollinger_Trend_Breakout(System.Int32 period, System.Int32 standardDeviation)
		{
			return LeadIndicator.Bollinger_Trend_Breakout(InSeries, period, standardDeviation);
		}

		public Bollinger_Trend_Breakout Bollinger_Trend_Breakout(IDataSeries input, System.Int32 period, System.Int32 standardDeviation)
		{
			return LeadIndicator.Bollinger_Trend_Breakout(input, period, standardDeviation);
		}
	}

	#endregion

}

#endregion

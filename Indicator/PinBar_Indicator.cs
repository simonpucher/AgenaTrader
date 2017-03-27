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
/// Version: 1.1
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// http://priceaction.com/price-action-university/strategies/pin-bar/
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this indicator without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	

    [Description("Show the Pin Bar Signal.")]
	public class PinBar_indicator : UserIndicator
	{
        private bool _showarrows = true;
        private bool _showindicatorbox = true;

        private int _percentage = 50;

        private Color _color_arrow_long_signal = Const.DefaultArrowLongColor;
        private Color _color_arrow_short_signal = Const.DefaultArrowShortColor;
        private Color _plot0color = Const.DefaultIndicatorColor;
        private int _plot0width = Const.DefaultLineWidth;
        private DashStyle _plot0dashstyle = Const.DefaultIndicatorDashStyle;

        protected override void OnInit()
        {
            Add(new Plot(new Pen(this.Plot0Color, this.Plot0Width), PlotStyle.Line, "PinBar_Indicator"));
            IsOverlay = false;
            CalculateOnClosedBar = true;
            IsAutoAdjustableScale = true;

            //this.RequiredBarsCount = 200;

     
        }

        protected override void OnCalculate()
		{
            int signal = 0;
            //Bars[0].IsGrowing && 
            if ((Bars[0].TailBottom/Bars[0].Range) > (this.Percentage/100.0))
            {
                signal = 1;
                if (this.ShowArrows)
                {
                    AddChartArrowUp("ArrowLong_PinBar" + +Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].Low, this.ColorArrowLongSignal);
                }
            }
            else if ((Bars[0].TailTop/Bars[0].Range) > (this.Percentage/100.0))
            {
                signal = -1;
                if (this.ShowArrows)
                {
                    AddChartArrowDown("ArrowShort_PinBar" + +Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].High, this.ColorArrowShortSignal);
                }
            }

            if (ShowIndicatorBox)
            {
                PlotLine.Set(signal);
            }



            PlotColors[0][0] = this.Plot0Color;
            Plots[0].PenStyle = this.Dash0Style;
            Plots[0].Pen.Width = this.Plot0Width;
        }
        


        public override string ToString()
        {
            return "Pin Bar (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "Pin Bar (I)";
            }
        }

        #region Properties

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries PlotLine
        {
            get { return Outputs[0]; }
        }

        /// <summary>
        /// </summary>
        [Description("If true then arrows are drawn on the chart.")]
        [Category("Plots")]
        [DisplayName("Show arrows")]
        public bool ShowArrows
        {
            get { return _showarrows; }
            set { _showarrows = value; }
        }

        /// <summary>
        /// </summary>
        [Description("If true then indicator box drawn on the chart.")]
        [Category("Plots")]
        [DisplayName("Show indicatorbox")]
        public bool ShowIndicatorBox
        {
            get { return _showindicatorbox; }
            set { _showindicatorbox = value; }
        }

        /// <summary>
        /// </summary>
        [Description("Select Color for the long arrows signal.")]
        [Category("Plots")]
        [DisplayName("Signal Long")]
        public Color ColorArrowLongSignal
        {
            get { return _color_arrow_long_signal; }
            set { _color_arrow_long_signal = value; }
        }
        // Serialize Color object
        [Browsable(false)]
        public string ColorArrowLongSignalSerialize
        {
            get { return SerializableColor.ToString(_color_arrow_long_signal); }
            set { _color_arrow_long_signal = SerializableColor.FromString(value); }
        }

        
        /// <summary>
        /// </summary>
        [Description("Select Color for the short arrows signal.")]
        [Category("Plots")]
        [DisplayName("Signal Short")]
        public Color ColorArrowShortSignal
        {
            get { return _color_arrow_short_signal; }
            set { _color_arrow_short_signal = value; }
        }
        // Serialize Color object
        [Browsable(false)]
        public string ColorArrowShortSignalSerialize
        {
            get { return SerializableColor.ToString(_color_arrow_short_signal); }
            set { _color_arrow_short_signal = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Select Color for the indicator.")]
        [Category("Plots")]
        [DisplayName("Plot Color")]
        public Color Plot0Color
        {
            get { return _plot0color; }
            set { _plot0color = value; }
        }
        // Serialize Color object
        [Browsable(false)]
        public string Plot0ColorSerialize
        {
            get { return SerializableColor.ToString(_plot0color); }
            set { _plot0color = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Line width for indicator.")]
        [Category("Plots")]
        [DisplayName("Plot Line width")]
        public int Plot0Width
        {
            get { return _plot0width; }
            set { _plot0width = Math.Max(1, value); }
        }

        /// <summary>
        /// </summary>
        [Description("DashStyle for indicator.")]
        [Category("Plots")]
        [DisplayName("Plot DashStyle")]
        public DashStyle Dash0Style
        {
            get { return _plot0dashstyle; }
            set { _plot0dashstyle = value; }
        }

        /// <summary>
        /// </summary>
        [Description("The pertage of the lenght of the tail.")]
        [Category("Plots")]
        [DisplayName("Percentage Tail")]
        public int Percentage
        {
            get { return _percentage; }
            set { _percentage = Math.Max(1, Math.Min(value, 100)); }
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
		/// Show the Pin Bar Signal.
		/// </summary>
		public PinBar_indicator PinBar_indicator()
        {
			return PinBar_indicator(InSeries);
		}

		/// <summary>
		/// Show the Pin Bar Signal.
		/// </summary>
		public PinBar_indicator PinBar_indicator(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<PinBar_indicator>(input);

			if (indicator != null)
				return indicator;

			indicator = new PinBar_indicator
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
		/// Show the Pin Bar Signal.
		/// </summary>
		public PinBar_indicator PinBar_indicator()
		{
			return LeadIndicator.PinBar_indicator(InSeries);
		}

		/// <summary>
		/// Show the Pin Bar Signal.
		/// </summary>
		public PinBar_indicator PinBar_indicator(IDataSeries input)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.PinBar_indicator(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Show the Pin Bar Signal.
		/// </summary>
		public PinBar_indicator PinBar_indicator()
		{
			return LeadIndicator.PinBar_indicator(InSeries);
		}

		/// <summary>
		/// Show the Pin Bar Signal.
		/// </summary>
		public PinBar_indicator PinBar_indicator(IDataSeries input)
		{
			return LeadIndicator.PinBar_indicator(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Show the Pin Bar Signal.
		/// </summary>
		public PinBar_indicator PinBar_indicator()
		{
			return LeadIndicator.PinBar_indicator(InSeries);
		}

		/// <summary>
		/// Show the Pin Bar Signal.
		/// </summary>
		public PinBar_indicator PinBar_indicator(IDataSeries input)
		{
			return LeadIndicator.PinBar_indicator(input);
		}
	}

	#endregion

}

#endregion

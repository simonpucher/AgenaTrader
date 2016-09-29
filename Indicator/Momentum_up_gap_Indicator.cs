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
/// Version: 1.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// Inspired by https://youtu.be/Qj_6DFTNfjE?t=437
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Instruments with gaps up tend to go higher.")]
    public class Momentum_up_gap_Indicator : UserIndicator
    {

        //input
        private int _percentage = 3;

        private bool _showarrows = true;
        private Color _plot0color = Const.DefaultIndicatorColor;
        private int _plot0width = Const.DefaultLineWidth;
        private DashStyle _plot0dashstyle = Const.DefaultIndicatorDashStyle;


        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(new Pen(this.Plot0Color, this.Plot0Width), PlotStyle.Line, "Plot_Line"));

            CalculateOnBarClose = false;
            Overlay = false;
            AutoScale = true;

            //Because of Backtesting reasons if we use the advanced mode we need at least two bars
            this.BarsRequired = 20;
        }

      

        protected override void OnBarUpdate()
        {
            double gapopen = ((Open[0] - Close[1]) * 100) / Close[1];
            double gapclose = ((Close[0] - Close[1]) * 100) / Close[1];
            if (gapopen >= this.Percentage && gapclose >= this.Percentage)
            {
                PlotLine.Set(1);
                if (ShowArrows)
                {
                    DrawArrowUp("ArrowLong_Entry" + +Bars[0].Time.Ticks, this.AutoScale, 0, Bars[0].Low, Color.LightGreen);
                }
            }
            else
            {
                PlotLine.Set(0);
            }

            PlotColors[0][0] = this.Plot0Color;
            Plots[0].PenStyle = this.Dash0Style;
            Plots[0].Pen.Width = this.Plot0Width;

        }


        public override string ToString()
        {
            return "Momentum up gap (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "Momentum up gap (I)";
            }
        }

        #region Properties

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries PlotLine
        {
            get { return Values[0]; }
        }

        /// <summary>
        /// </summary>
        [Description("Percentage for the up gap.")]
        [Category("Parameters")]
        [DisplayName("Percentage")]
        public int Percentage
        {
            get { return _percentage; }
            set { _percentage = value; }
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
        [Description("Select Color for the indicator.")]
        [Category("Plots")]
        [DisplayName("Color")]
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
        [DisplayName("Line width")]
        public int Plot0Width
        {
            get { return _plot0width; }
            set { _plot0width = Math.Max(1, value); }
        }

        /// <summary>
        /// </summary>
        [Description("DashStyle for indicator.")]
        [Category("Plots")]
        [DisplayName("DashStyle")]
        public DashStyle Dash0Style
        {
            get { return _plot0dashstyle; }
            set { _plot0dashstyle = value; }
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
		/// Instruments with gaps up tend to go higher.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator(System.Int32 percentage)
        {
			return Momentum_up_gap_Indicator(Input, percentage);
		}

		/// <summary>
		/// Instruments with gaps up tend to go higher.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator(IDataSeries input, System.Int32 percentage)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Momentum_up_gap_Indicator>(input, i => i.Percentage == percentage);

			if (indicator != null)
				return indicator;

			indicator = new Momentum_up_gap_Indicator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							Percentage = percentage
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
		/// Instruments with gaps up tend to go higher.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator(System.Int32 percentage)
		{
			return LeadIndicator.Momentum_up_gap_Indicator(Input, percentage);
		}

		/// <summary>
		/// Instruments with gaps up tend to go higher.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator(IDataSeries input, System.Int32 percentage)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.Momentum_up_gap_Indicator(input, percentage);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Instruments with gaps up tend to go higher.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator(System.Int32 percentage)
		{
			return LeadIndicator.Momentum_up_gap_Indicator(Input, percentage);
		}

		/// <summary>
		/// Instruments with gaps up tend to go higher.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator(IDataSeries input, System.Int32 percentage)
		{
			return LeadIndicator.Momentum_up_gap_Indicator(input, percentage);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Instruments with gaps up tend to go higher.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator(System.Int32 percentage)
		{
			return LeadIndicator.Momentum_up_gap_Indicator(Input, percentage);
		}

		/// <summary>
		/// Instruments with gaps up tend to go higher.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator(IDataSeries input, System.Int32 percentage)
		{
			return LeadIndicator.Momentum_up_gap_Indicator(input, percentage);
		}
	}

	#endregion

}

#endregion

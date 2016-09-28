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
    [IsEntryAttribute(true)]
	[IsStopAttribute(false)]
	[IsTargetAttribute(false)]
	[OverrulePreviousStopPrice(false)]
	public class Momentum_up_gap_Condition : UserScriptedCondition
	{
        #region Variables

        private int _percentage = 3;

        private Color _plot0color = Const.DefaultIndicatorColor;
        private int _plot0width = Const.DefaultLineWidth;
        private DashStyle _plot0dashstyle = Const.DefaultIndicatorDashStyle;
        private Color _plot1color = Const.DefaultIndicatorColor_GreyedOut;
        private int _plot1width = Const.DefaultLineWidth;
        private DashStyle _plot1dashstyle = Const.DefaultIndicatorDashStyle;

        #endregion

        protected override void Initialize()
		{
			IsEntry = true;
			IsStop = false;
			IsTarget = false;

            Add(new Plot(new Pen(this.Plot0Color, this.Plot0Width), PlotStyle.Line, "Occurred"));
            Add(new Plot(new Pen(this.Plot0Color, this.Plot0Width), PlotStyle.Line, "Entry"));

            Overlay = false;
			CalculateOnBarClose = false;
            AutoScale = true;

            this.BarsRequired = 20;
        }

		protected override void OnBarUpdate()
		{
            Occurred.Set(LeadIndicator.Momentum_up_gap_Indicator(this.Percentage)[0]);

            PlotColors[0][0] = this.Plot0Color;
            Plots[0].PenStyle = this.Dash0Style;
            Plots[0].Pen.Width = this.Plot0Width;

            PlotColors[1][0] = this.Plot1Color;
            Plots[1].PenStyle = this.Dash1Style;
            Plots[1].Pen.Width = this.Plot1Width;
        }


        public override string ToString()
        {
            return "Momentum up gap (C)";
        }

        public override string DisplayName
        {
            get
            {
                return "Momentum up gap (C)";
            }
        }

        #region Properties

        [Browsable(false)]
		[XmlIgnore()]
		public DataSeries Occurred
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Entry
		{
			get { return Values[1]; }
		}

		public override IList<DataSeries> GetEntries()
		{
			return new[]{Entry};
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

        /// <summary>
        /// </summary>
        [Description("Select Color for the indicator.")]
        [Category("Plots")]
        [DisplayName("Color")]
        public Color Plot1Color
        {
            get { return _plot1color; }
            set { _plot1color = value; }
        }
        // Serialize Color object
        [Browsable(false)]
        public string Plot1ColorSerialize
        {
            get { return SerializableColor.ToString(_plot1color); }
            set { _plot1color = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Line width for indicator.")]
        [Category("Plots")]
        [DisplayName("Line width")]
        public int Plot1Width
        {
            get { return _plot1width; }
            set { _plot1width = Math.Max(1, value); }
        }

        /// <summary>
        /// </summary>
        [Description("DashStyle for indicator.")]
        [Category("Plots")]
        [DisplayName("DashStyle")]
        public DashStyle Dash1Style
        {
            get { return _plot1dashstyle; }
            set { _plot1dashstyle = value; }
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
		public Momentum_up_gap_Condition Momentum_up_gap_Condition(System.Int32 percentage)
        {
			return Momentum_up_gap_Condition(Input, percentage);
		}

		/// <summary>
		/// Instruments with gaps up tend to go higher.
		/// </summary>
		public Momentum_up_gap_Condition Momentum_up_gap_Condition(IDataSeries input, System.Int32 percentage)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Momentum_up_gap_Condition>(input, i => i.Percentage == percentage);

			if (indicator != null)
				return indicator;

			indicator = new Momentum_up_gap_Condition
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
		public Momentum_up_gap_Condition Momentum_up_gap_Condition(System.Int32 percentage)
		{
			return LeadIndicator.Momentum_up_gap_Condition(Input, percentage);
		}

		/// <summary>
		/// Instruments with gaps up tend to go higher.
		/// </summary>
		public Momentum_up_gap_Condition Momentum_up_gap_Condition(IDataSeries input, System.Int32 percentage)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.Momentum_up_gap_Condition(input, percentage);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Instruments with gaps up tend to go higher.
		/// </summary>
		public Momentum_up_gap_Condition Momentum_up_gap_Condition(System.Int32 percentage)
		{
			return LeadIndicator.Momentum_up_gap_Condition(Input, percentage);
		}

		/// <summary>
		/// Instruments with gaps up tend to go higher.
		/// </summary>
		public Momentum_up_gap_Condition Momentum_up_gap_Condition(IDataSeries input, System.Int32 percentage)
		{
			return LeadIndicator.Momentum_up_gap_Condition(input, percentage);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Instruments with gaps up tend to go higher.
		/// </summary>
		public Momentum_up_gap_Condition Momentum_up_gap_Condition(System.Int32 percentage)
		{
			return LeadIndicator.Momentum_up_gap_Condition(Input, percentage);
		}

		/// <summary>
		/// Instruments with gaps up tend to go higher.
		/// </summary>
		public Momentum_up_gap_Condition Momentum_up_gap_Condition(IDataSeries input, System.Int32 percentage)
		{
			return LeadIndicator.Momentum_up_gap_Condition(input, percentage);
		}
	}

	#endregion

}

#endregion

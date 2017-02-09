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
/// Version: not working
/// -------------------------------------------------------------------------
/// Simon Pucher 2017
/// -------------------------------------------------------------------------
/// http://vtadwiki.vtad.de/index.php/Elder_Ray_-_Bull_and_Bear_Power
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://github.com/ScriptTrading/Basic-Package/blob/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Elder Ray - Bull and Bear Power")]
    [IsEntryAttribute(true)]
	[IsStopAttribute(false)]
	[IsTargetAttribute(false)]
	[OverrulePreviousStopPrice(false)]
	public class Elder_Ray_Bull_and_Bear_Power_Condition : UserScriptedCondition
	{
        #region Variables

        private int _period = 13;
        private ElderRayTyp _ElderRayTyp = ElderRayTyp.BullPower;

        private Color _plot0color = Const.DefaultIndicatorColor;
        private int _plot0width = Const.DefaultLineWidth;
        private DashStyle _plot0dashstyle = Const.DefaultIndicatorDashStyle;
        private Color _plot1color = Const.DefaultIndicatorColor_GreyedOut;
        private int _plot1width = Const.DefaultLineWidth;
        private DashStyle _plot1dashstyle = Const.DefaultIndicatorDashStyle;

        #endregion

        protected override void OnInit()
		{
			IsEntry = true;
			IsStop = false;
			IsTarget = false;
            Add(new Plot(new Pen(this.Plot0Color, this.Plot0Width), PlotStyle.Line, "Occurred"));
            Add(new Plot(new Pen(this.Plot0Color, this.Plot1Width), PlotStyle.Line, "Entry"));

            IsOverlay = false;
			CalculateOnClosedBar = true;

            this.RequiredBarsCount = 20;
        }

		protected override void OnCalculate()
		{
            EMA ema = EMA(this.Period);
            double bull_power = High[0] - ema[0];
            double bear_power = Low[0] - ema[0];


            //if (_ElderRayTyp == ElderRayTyp.BullPower)
            //{
            //    MyPlot1.Set(bull_power);
            //}
            //else
            //{
            //    MyPlot2.Set(bear_power);
            //}



           

            //if (ema[0] > ema[1] && bear_power < 0 && bear_power > MyPlot2[1])
            //{
            //    AddChartArrowUp("ArrowLong" + Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].Low, Color.LightGreen);
            //}

            //if (ema[0] < ema[1] && bull_power > 0 && bull_power < MyPlot1[1])
            //{
            //    AddChartArrowDown("ArrowShort" + Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].High, Color.Red);
            //}

            PlotColors[0][0] = this.Plot0Color;
            Plots[0].PenStyle = this.Dash0Style;
            Plots[0].Pen.Width = this.Plot0Width;

            PlotColors[1][0] = this.Plot1Color;
            Plots[1].PenStyle = this.Dash1Style;
            Plots[1].Pen.Width = this.Plot1Width;
        }


        public override string DisplayName
        {
            get
            {
                return "Elder Ray (I)";
            }
        }


        public override string ToString()
        {
            return "Elder Ray (I)";

        }

        #region Properties

        [Browsable(false)]
		[XmlIgnore()]
		public DataSeries Occurred
		{
			get { return Outputs[0]; }
		}

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Entry
		{
			get { return Outputs[1]; }
		}

		public override IList<DataSeries> GetEntries()
		{
			return new[]{Entry};
		}

        [Description("Type of ElderRayTyp.")]
        [Category("Parameters")]
        [DisplayName("ElderRayTyp")]
        public ElderRayTyp ElderRayTyp
        {
            get { return _ElderRayTyp; }
            set { _ElderRayTyp = value; }

        }
        [Description("Period of the EMA.")]
        [Category("Parameters")]
        [DisplayName("Period")]
        public int Period
        {
            get { return _period; }
            set { _period = value; }

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
        [Description("Select color for the indicator.")]
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
		/// Elder Ray - Bull and Bear Power
		/// </summary>
		public Elder_Ray_Bull_and_Bear_Power_Condition Elder_Ray_Bull_and_Bear_Power_Condition(ElderRayTyp elderRayTyp, System.Int32 period)
        {
			return Elder_Ray_Bull_and_Bear_Power_Condition(InSeries, elderRayTyp, period);
		}

		/// <summary>
		/// Elder Ray - Bull and Bear Power
		/// </summary>
		public Elder_Ray_Bull_and_Bear_Power_Condition Elder_Ray_Bull_and_Bear_Power_Condition(IDataSeries input, ElderRayTyp elderRayTyp, System.Int32 period)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Elder_Ray_Bull_and_Bear_Power_Condition>(input, i => i.ElderRayTyp == elderRayTyp && i.Period == period);

			if (indicator != null)
				return indicator;

			indicator = new Elder_Ray_Bull_and_Bear_Power_Condition
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input,
							ElderRayTyp = elderRayTyp,
							Period = period
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
		/// Elder Ray - Bull and Bear Power
		/// </summary>
		public Elder_Ray_Bull_and_Bear_Power_Condition Elder_Ray_Bull_and_Bear_Power_Condition(ElderRayTyp elderRayTyp, System.Int32 period)
		{
			return LeadIndicator.Elder_Ray_Bull_and_Bear_Power_Condition(InSeries, elderRayTyp, period);
		}

		/// <summary>
		/// Elder Ray - Bull and Bear Power
		/// </summary>
		public Elder_Ray_Bull_and_Bear_Power_Condition Elder_Ray_Bull_and_Bear_Power_Condition(IDataSeries input, ElderRayTyp elderRayTyp, System.Int32 period)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.Elder_Ray_Bull_and_Bear_Power_Condition(input, elderRayTyp, period);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Elder Ray - Bull and Bear Power
		/// </summary>
		public Elder_Ray_Bull_and_Bear_Power_Condition Elder_Ray_Bull_and_Bear_Power_Condition(ElderRayTyp elderRayTyp, System.Int32 period)
		{
			return LeadIndicator.Elder_Ray_Bull_and_Bear_Power_Condition(InSeries, elderRayTyp, period);
		}

		/// <summary>
		/// Elder Ray - Bull and Bear Power
		/// </summary>
		public Elder_Ray_Bull_and_Bear_Power_Condition Elder_Ray_Bull_and_Bear_Power_Condition(IDataSeries input, ElderRayTyp elderRayTyp, System.Int32 period)
		{
			return LeadIndicator.Elder_Ray_Bull_and_Bear_Power_Condition(input, elderRayTyp, period);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Elder Ray - Bull and Bear Power
		/// </summary>
		public Elder_Ray_Bull_and_Bear_Power_Condition Elder_Ray_Bull_and_Bear_Power_Condition(ElderRayTyp elderRayTyp, System.Int32 period)
		{
			return LeadIndicator.Elder_Ray_Bull_and_Bear_Power_Condition(InSeries, elderRayTyp, period);
		}

		/// <summary>
		/// Elder Ray - Bull and Bear Power
		/// </summary>
		public Elder_Ray_Bull_and_Bear_Power_Condition Elder_Ray_Bull_and_Bear_Power_Condition(IDataSeries input, ElderRayTyp elderRayTyp, System.Int32 period)
		{
			return LeadIndicator.Elder_Ray_Bull_and_Bear_Power_Condition(input, elderRayTyp, period);
		}
	}

	#endregion

}

#endregion

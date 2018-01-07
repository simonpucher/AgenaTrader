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
/// Version: 1.2
/// -------------------------------------------------------------------------
/// Simon Pucher 2017
/// -------------------------------------------------------------------------
/// http://vtadwiki.vtad.de/index.php/Elder_Ray_-_Bull_and_Bear_Power
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
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

        private DoubleSeries ds_bull_power;
        private DoubleSeries ds_bear_power;

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
            Add(new OutputDescriptor(new Pen(this.Plot0Color, this.Plot0Width), OutputSerieDrawStyle.Line, "Occurred"));
            Add(new OutputDescriptor(new Pen(this.Plot0Color, this.Plot1Width), OutputSerieDrawStyle.Line, "Entry"));

            ds_bull_power = new DoubleSeries(this);
            ds_bear_power = new DoubleSeries(this);

            IsOverlay = false;
			CalculateOnClosedBar = true;

            this.RequiredBarsCount = 20;
        }

		protected override void OnCalculate()
		{
            EMA ema = EMA(this.Period);
            double bull_power = High[0] - ema[0];
            double bear_power = Low[0] - ema[0];
            ds_bull_power.Set(bull_power);
            ds_bear_power.Set(bear_power);

            int resultsignal = 0;
            if (ema[0] > ema[1] && bear_power < 0 && bear_power > ds_bear_power.Get(1))
            {
                resultsignal = 1;
            }

            if (ema[0] < ema[1] && bull_power > 0 && bull_power < ds_bull_power.Get(1))
            {
                resultsignal = -1;
            }

            Occurred.Set(resultsignal);

            PlotColors[0][0] = this.Plot0Color;
            OutputDescriptors[0].PenStyle = this.Dash0Style;
            OutputDescriptors[0].Pen.Width = this.Plot0Width;

            PlotColors[1][0] = this.Plot1Color;
            OutputDescriptors[1].PenStyle = this.Dash1Style;
            OutputDescriptors[1].Pen.Width = this.Plot1Width;
        }


        public override string DisplayName
        {
            get
            {
                return "Elder Ray (C)";
            }
        }


        public override string ToString()
        {
            return "Elder Ray (C)";

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
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
/// Version: 1.3.2
/// -------------------------------------------------------------------------
/// Simon Pucher 2017
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Moving Averages Condition.")]
    [IsEntryAttribute(true)]
	[IsStopAttribute(false)]
	[IsTargetAttribute(false)]
	[OverrulePreviousStopPrice(false)]
	public class Moving_Averages_Condition : UserScriptedCondition
	{
        #region Variables
        private int _candles = 14;
        private Stack<DateTime> lastsignals;

        private Color _plot0color = Const.DefaultIndicatorColor;
        private int _plot0width = Const.DefaultLineWidth;
        private DashStyle _plot0dashstyle = Const.DefaultIndicatorDashStyle;
        private Color _plot1color = Const.DefaultIndicatorColor_GreyedOut;
        private int _plot1width = Const.DefaultLineWidth;
        private DashStyle _plot1dashstyle = Const.DefaultIndicatorDashStyle;

        private int _ma_long = 200;
        private int _ma_medium = 100;
        private int _ma_short = 50;

        private Color _signalcolor = Color.Transparent;

        #endregion

        protected override void OnInit()
		{
			IsEntry = true;
			IsStop = false;
			IsTarget = false;
            Add(new OutputDescriptor(new Pen(this.Plot0Color, this.Plot0Width), OutputSerieDrawStyle.Line, "Occurred"));
            Add(new OutputDescriptor(new Pen(this.Plot0Color, this.Plot1Width), OutputSerieDrawStyle.Line, "Entry"));

            IsOverlay = false;
			CalculateOnClosedBar = true;

            this.RequiredBarsCount = 200;
        }

		protected override void OnCalculate()
		{
            //Reset color
            _signalcolor = Color.Transparent;

            if (ProcessingBarIndex == 0)
            {
                lastsignals = new Stack<DateTime>();
            }

            bool therewasasignal = false;
            //if (Low[0] < SMA(200)[0] && SMA(50)[0] >= SMA(100)[0] && SMA(100)[0] >= SMA(200)[0]  && Close[0] > SuperTrend(SuperTrendMAType.HMA, SuperTrendMode.ATR, 14, 2.618, 14).UpTrend[0])
            //if (Low[0] < SMA(200)[0] && SMA(50)[0] >= SMA(100)[0] && Close[0] > SuperTrend(SuperTrendMAType.HMA, SuperTrendMode.ATR, 14, 2.618, 14).UpTrend[0])
            //if (Low[0] < SMA(200)[0] && SMA(50)[0] >= SMA(100)[0] && Close[0] > SuperTrend(SuperTrendMAType.SMA, SuperTrendMode.ATR, 50, 2.618, 50).UpTrend[0])
            if (Low[0] < SMA(this.MA_Long)[0] && SMA(this.MA_Short)[0] >= SMA(this.MA_Medium)[0] 
                && Close[0] > SuperTrend(SuperTrendMAType.SMA, SuperTrendMode.ATR, 200, 2.618, 200).UpTrend[0])
            {
                therewasasignal = true;
            }
            double thevalue = 0;
            if (therewasasignal)
            {
                thevalue = 1;
                _signalcolor = Color.LightGreen;
                AddChartArrowUp("ArrowLong_Entry" + +Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].Low, Color.Green);
                lastsignals.Push(Time[0]);
            }
            else
            {
                if (lastsignals != null && lastsignals.Count > 0 && lastsignals.Peek() >= Time[this.Candles - 1])
                {
                    AddChartArrowUp("ArrowLong_Echo_Entry" + +Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].Low, Color.LightGreen);
                    thevalue = 0.5;
                    _signalcolor = Color.Green;
                }
            }

        

            Occurred.Set(thevalue);

            PlotColors[0][0] = this.Plot0Color;
            OutputDescriptors[0].PenStyle = this.Dash0Style;
            OutputDescriptors[0].Pen.Width = this.Plot0Width;

            PlotColors[1][0] = this.Plot1Color;
            OutputDescriptors[1].PenStyle = this.Dash1Style;
            OutputDescriptors[1].Pen.Width = this.Plot1Width;
        }


        public override string ToString()
        {
            return "Moving Averages (C)";
        }

        public override string DisplayName
        {
            get
            {
                return "Moving Averages (C)";
            }
        }

        public override Color? GetSignalColor()
        {
            return _signalcolor;
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

        

        /// <summary>
        /// </summary>
        [Description("The period of the long Moving Average.")]
        [InputParameter]
        [DisplayName("Period MA Long")]
        public int MA_Long
        {
            get { return _ma_long; }
            set { _ma_long = value; }
        }

        /// <summary>
        /// </summary>
        [Description("The period of the medium Moving Average.")]
        [InputParameter]
        [DisplayName("Period MA Medium")]
        public int MA_Medium
        {
            get { return _ma_medium; }
            set { _ma_medium = value; }
        }

        /// <summary>
        /// </summary>
        [Description("The period of the short Moving Average.")]
        [InputParameter]
        [DisplayName("Period MA Short")]
        public int MA_Short
        {
            get { return _ma_short; }
            set { _ma_short = value; }
        }

        /// <summary>
        /// </summary>
        [Description("The script show a signal if the gap was during the last x candles.")]
        [InputParameter]
        [DisplayName("Candles")]
        public int Candles
        {
            get { return _candles; }
            set { _candles = value; }
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
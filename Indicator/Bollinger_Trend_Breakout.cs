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
/// Version: 1.1.0
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
        private bool _ShowSignalOnChartBackground = true;
        private bool _ShowSignalOnChartArrow = true;

        private int _period = 200;
        private int _stddev = 1;

        private int _linewidth_1 = 1;
        private DashStyle _linestyle_1 = DashStyle.Solid;
        private Color _col_1 = Color.Violet;

        private int _linewidth_2 = 1;
        private DashStyle _linestyle_2 = DashStyle.Solid;
        private Color _col_2 = Color.Gray;

        private int _linewidth_3 = 1;
        private DashStyle _linestyle_3 = DashStyle.Solid;
        private Color _col_3 = Color.Gray;

        private Color _color_long_signal_background = Const.DefaultArrowLongColor;
        private Color _color_short_signal_background = Const.DefaultArrowShortColor;
        private Color _color_long_signal_arrow = Const.DefaultArrowLongColor;
        private Color _color_short_signal_arrow = Const.DefaultArrowShortColor;

        private int _opacity_long_signal = 25;
        private int _opacity_short_signal = 25;

        private DataSeries _signals;

        protected override void OnInit()
		{
            Add(new OutputDescriptor(new Pen(this.Color_1, this.LineWidth_1), OutputSerieDrawStyle.Line, "MA_1"));
            Add(new OutputDescriptor(new Pen(this.Color_2, this.LineWidth_2), OutputSerieDrawStyle.Line, "MA_2"));
            Add(new OutputDescriptor(new Pen(this.Color_3, this.LineWidth_3), OutputSerieDrawStyle.Line, "MA_3"));

            IsOverlay = true;

            _signals = new DataSeries(this);

            this.RequiredBarsCount = 200;
        }

		protected override void OnCalculate()
		{
            Bollinger bb = Bollinger(this.StandardDeviation, this.Period);

            if (Close[0] > bb.Upper[0])
            {
                if (ShowSignalOnChartBackground) this.BackColor = GlobalUtilities.AdjustOpacity(this.ColorLongSignalBackground, this.OpacityLongSignal / 100.0);
                _signals.Set(1);
            }
            else if (Close[0] < bb.Lower[0])
            {
                if (ShowSignalOnChartBackground) this.BackColor = GlobalUtilities.AdjustOpacity(this.ColorShortSignalBackground, this.OpacityShortSignal / 100.0);
                _signals.Set(-1);
            }
            else
            {
                _signals.Set(_signals[1]);
                if (_signals[1] == 1)
                {
                    if (ShowSignalOnChartBackground) this.BackColor = GlobalUtilities.AdjustOpacity(this.ColorLongSignalBackground, this.OpacityLongSignal / 100.0);
                }
                else
                {
                    if (ShowSignalOnChartBackground) this.BackColor = GlobalUtilities.AdjustOpacity(this.ColorShortSignalBackground, this.OpacityShortSignal / 100.0);
                }
            }

            if (this.ShowSignalOnChartArrow)
            {
                if (_signals[0] == 1 && _signals[1] != 1) AddChartArrowUp("ArrowLong_MA" + +Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].Low, this.ColorLongSignalArrow);
                else if (_signals[0] == -1 && _signals[1] != -1) AddChartArrowDown("ArrowShort_MA" + +Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].High, this.ColorShortSignalArrow);
            }

            //set bb
            this.Plot_Middle.Set(bb.Middle[0]);
            this.Plot_High.Set(bb.Upper[0]);
            this.Plot_Low.Set(bb.Lower[0]);


            //Set the color
            PlotColors[0][0] = this.Color_1;
            OutputDescriptors[0].PenStyle = this.DashStyle_1;
            OutputDescriptors[0].Pen.Width = this.LineWidth_1;
            PlotColors[1][0] = this.Color_2;
            OutputDescriptors[1].PenStyle = this.DashStyle_2;
            OutputDescriptors[1].Pen.Width = this.LineWidth_2;
            PlotColors[2][0] = this.Color_3;
            OutputDescriptors[2].PenStyle = this.DashStyle_3;
            OutputDescriptors[2].Pen.Width = this.LineWidth_3;

        }



        public override string ToString()
        {
            return "Bollinger Band Trend Breakout (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "Bollinger Band Trend Breakout (I)";
            }
        }


        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Signals { get { return _signals; } }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Plot_Middle { get { return Outputs[0]; } }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Plot_High { get { return Outputs[1]; } }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Plot_Low { get { return Outputs[2]; } }



        /// <summary>
        /// </summary>
        [Description("Select Color for the arrow in long setup.")]
        [Category("Background")]
        [DisplayName("Color Arrow Long")]
        public Color ColorLongSignalArrow
        {
            get { return _color_long_signal_arrow; }
            set { _color_long_signal_arrow = value; }
        }


        // Serialize Color object
        [Browsable(false)]
        public string ColorLongSignalArrowSerialize
        {
            get { return SerializableColor.ToString(_color_long_signal_background); }
            set { _color_long_signal_background = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Select Color for the arrow in short setup.")]
        [Category("Background")]
        [DisplayName("Color Arrow Short")]
        public Color ColorShortSignalArrow
        {
            get { return _color_short_signal_arrow; }
            set { _color_short_signal_arrow = value; }
        }
        // Serialize Color object
        [Browsable(false)]
        public string ColorShortSignalArrowSerialize
        {
            get { return SerializableColor.ToString(_color_short_signal_arrow); }
            set { _color_short_signal_arrow = SerializableColor.FromString(value); }
        }


        /// <summary>
        /// </summary>
        [Description("Show signal strength on the chart (arrow).")]
        [Category("Background")]
        [DisplayName("Show arrow")]
        public bool ShowSignalOnChartArrow
        {
            get { return _ShowSignalOnChartArrow; }
            set
            {
                _ShowSignalOnChartArrow = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("Show signal strength on the chart (background).")]
        [Category("Background")]
        [DisplayName("Show background")]
        public bool ShowSignalOnChartBackground
        {
            get { return _ShowSignalOnChartBackground; }
            set
            {
                _ShowSignalOnChartBackground = value;
            }
        }


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



        /// <summary>
        /// </summary>
        [Description("Line Width of MA 1.")]
        [Category("Plots")]
        [DisplayName("LW MA 1")]
        public int LineWidth_1
        {
            get { return _linewidth_1; }
            set { _linewidth_1 = Math.Max(1, value); }
        }


        /// <summary>
        /// </summary>
        [Description("DashStyle for MA 1.")]
        [Category("Plots")]
        [DisplayName("DS MA 1")]
        public DashStyle DashStyle_1
        {
            get { return _linestyle_1; }
            set { _linestyle_1 = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Color for MA 1")]
        [Category("Plots")]
        [DisplayName("Color MA 1")]
        public Color Color_1
        {
            get { return _col_1; }
            set { _col_1 = value; }
        }

        [Browsable(false)]
        public string Color_1_Serialize
        {
            get { return SerializableColor.ToString(_col_1); }
            set { _col_1 = SerializableColor.FromString(value); }
        }


      

        /// <summary>
        /// </summary>
        [Description("Line Width of MA 2.")]
        [Category("Plots")]
        [DisplayName("LW MA 2")]
        public int LineWidth_2
        {
            get { return _linewidth_2; }
            set { _linewidth_2 = Math.Max(1, value); }
        }


        /// <summary>
        /// </summary>
        [Description("DashStyle for MA 2.")]
        [Category("Plots")]
        [DisplayName("DS MA 2")]
        public DashStyle DashStyle_2
        {
            get { return _linestyle_2; }
            set { _linestyle_2 = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Color for MA 2")]
        [Category("Plots")]
        [DisplayName("Color MA 2")]
        public Color Color_2
        {
            get { return _col_2; }
            set { _col_2 = value; }
        }

        [Browsable(false)]
        public string Color_2_Serialize
        {
            get { return SerializableColor.ToString(_col_2); }
            set { _col_2 = SerializableColor.FromString(value); }
        }


  
       

        /// <summary>
        /// </summary>
        [Description("Line Width of MA 3.")]
        [Category("Plots")]
        [DisplayName("LW MA 3")]
        public int LineWidth_3
        {
            get { return _linewidth_3; }
            set { _linewidth_3 = Math.Max(1, value); }
        }


        /// <summary>
        /// </summary>
        [Description("DashStyle for MA 3.")]
        [Category("Plots")]
        [DisplayName("DS MA 3")]
        public DashStyle DashStyle_3
        {
            get { return _linestyle_3; }
            set { _linestyle_3 = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Color for MA 3")]
        [Category("Plots")]
        [DisplayName("Color MA 3")]
        public Color Color_3
        {
            get { return _col_3; }
            set { _col_3 = value; }
        }

        [Browsable(false)]
        public string Color_3_Serialize
        {
            get { return SerializableColor.ToString(_col_3); }
            set { _col_3 = SerializableColor.FromString(value); }
        }

    }
}
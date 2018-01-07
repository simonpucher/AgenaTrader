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
/// Version: 1.2.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// Description http://lindaraschke.net/wp-content/uploads/2013/11/august1997.pdf
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Watch out for the lonely warrior behind enemy lines.")]
    public class Holy_Grail_Indicator : UserIndicator
    {

        //input
        private bool _showarrows = true;
        private Color _plot0color = Const.DefaultIndicatorColor;
        private int _plot0width = Const.DefaultLineWidth;
        private DashStyle _plot0dashstyle = Const.DefaultIndicatorDashStyle;
        private Color _plot1color = Const.DefaultIndicatorColor_GreyedOut;
        private int _plot1width = Const.DefaultLineWidth;
        private DashStyle _plot1dashstyle = Const.DefaultIndicatorDashStyle;


        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void OnInit()
        {
            Add(new OutputDescriptor(new Pen(this.Plot0Color, this.Plot0Width), OutputSerieDrawStyle.Line, "Plot_Line1"));
            Add(new OutputDescriptor(new Pen(this.Plot1Color, this.Plot1Width), OutputSerieDrawStyle.Line, "Plot_Line2"));
            Add(new OutputDescriptor(new Pen(this.Plot0Color, this.Plot0Width), OutputSerieDrawStyle.Line, "Plot_Line3"));

            CalculateOnClosedBar = true;
            IsOverlay = false;
            IsAutoAdjustableScale = true;

            //Because of Backtesting reasons if we use the advanced mode we need at least two bars
            this.RequiredBarsCount = 20;
        }

   

        protected override void OnCalculate()
        {

            ADX adx = ADX(14);
            EMA ema = EMA(20);
            RSI rsi = RSI(14, 3);

            double singnaldata = 0;


            if (adx[0] > 30 && adx[0] > adx[1] && InSeries[0] <= ema[0])
            {
                Color color = Color.Green;
                if (rsi[0] <= 30)
                {
                    color = Color.LightGreen;
                    singnaldata = 1;
                }
                else
                {
                    singnaldata = 0.5;
                }
                AddChartArrowUp("ArrowLong_Entry" + +Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].Low, color);
            }

            //ADX adx = ADX(14);
            //EMA ema = EMA(20);

            //double singnaldata = 0;


            //if (adx[0] > 30 && adx[0] > adx[1] && InSeries[0] <= ema[0])
            //{
            //    singnaldata = 1;
            //    AddChartArrowUp("ArrowLong_Entry" + +Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].Low, Color.Green);
            //}


            SignalLine.Set(singnaldata);
            PlotLine_ADX.Set(adx[0]);
            PlotLine_XMA.Set(ema[0]);



            PlotColors[0][0] = this.Plot0Color;
            OutputDescriptors[0].PenStyle = this.Dash0Style;
            OutputDescriptors[0].Pen.Width = this.Plot0Width;

            PlotColors[1][0] = this.Plot1Color;
            OutputDescriptors[1].PenStyle = this.Dash1Style;
            OutputDescriptors[1].Pen.Width = this.Plot1Width;

            PlotColors[2][0] = this.Plot1Color;
            OutputDescriptors[2].PenStyle = this.Dash1Style;
            OutputDescriptors[2].Pen.Width = this.Plot1Width;

        }


        public override string ToString()
        {
            return "Holy Grail (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "Holy Grail (I)";
            }
        }

        #region Properties

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries SignalLine
        {
            get { return Outputs[0]; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries PlotLine_ADX
        {
            get { return Outputs[1]; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries PlotLine_XMA
        {
            get { return Outputs[2]; }
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
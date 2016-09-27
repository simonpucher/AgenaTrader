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
/// Inspired by https://www.youtube.com/watch?v=Qj_6DFTNfjE
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Watch out for the lonely warrior behind enemy lines.")]
    public class Lonely_Warrior_Indicator : UserIndicator
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
        protected override void Initialize()
        {
            Add(new Plot(new Pen(this.Plot0Color, this.Plot0Width), PlotStyle.Line, "Plot_Line"));

            CalculateOnBarClose = true;
            Overlay = false;
            AutoScale = true;

            //Because of Backtesting reasons if we use the advanced mode we need at least two bars
            this.BarsRequired = 20;
        }

   

        protected override void OnBarUpdate()
        {
            Bollinger bb = Bollinger(2, 20);

            DrawLine("Plot_Middle" + Time[0].ToString(), this.AutoScale, 1, bb.Middle[1], 0, bb.Middle[0], this.Plot1Color, this.Dash1Style, this.Plot1Width);
            DrawLine("Plot_Low" + Time[0].ToString(), this.AutoScale, 1, bb.Lower[1], 0, bb.Lower[0], this.Plot0Color, this.Dash0Style, this.Plot0Width);
            DrawLine("Plot_High" + Time[0].ToString(), this.AutoScale, 1, bb.Upper[1], 0, bb.Upper[0], this.Plot0Color, this.Dash0Style, this.Plot0Width);

            if (High[0] < bb.Lower[0] || Low[0] > bb.Upper[0])
            {
                //ok
            }
            else
            {
                this.BarColor = Color.White;
            }

            //Trigger
            double signal = 0;
            if (High[1] < bb.Lower[1])
            {
                if (Low[0] > High[1] || High[0] > High[1])
                {
                    if (ShowArrows)
                    {
                        DrawArrowUp("ArrowLong_Entry" + +Bars[0].Time.Ticks, this.AutoScale, 0, Bars[0].Low, Color.LightGreen);
                    }
                    signal = 1;
                }
            }
            else if (Low[1] > bb.Upper[1])
            {
                if (Low[0] < Low[1] || High[0] < Low[1])
                {
                    if (ShowArrows)
                    {
                        DrawArrowDown("ArrowShort_Entry" + +Bars[0].Time.Ticks, this.AutoScale, 0, Bars[0].High, Color.Red);
                    }
                    signal = -1;
                }
            }


            PlotLine.Set(signal);


            PlotColors[0][0] = this.Plot0Color;
            Plots[0].PenStyle = this.Dash0Style;
            Plots[0].Pen.Width = this.Plot0Width;

        }


        public override string ToString()
        {
            return "Lonely Warrior (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "Lonely Warrior (I)";
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
#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator
	{
		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator()
        {
			return Lonely_Warrior_Indicator(Input);
		}

		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Lonely_Warrior_Indicator>(input);

			if (indicator != null)
				return indicator;

			indicator = new Lonely_Warrior_Indicator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input
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
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator()
		{
			return LeadIndicator.Lonely_Warrior_Indicator(Input);
		}

		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator(IDataSeries input)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.Lonely_Warrior_Indicator(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator()
		{
			return LeadIndicator.Lonely_Warrior_Indicator(Input);
		}

		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator(IDataSeries input)
		{
			return LeadIndicator.Lonely_Warrior_Indicator(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator()
		{
			return LeadIndicator.Lonely_Warrior_Indicator(Input);
		}

		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator(IDataSeries input)
		{
			return LeadIndicator.Lonely_Warrior_Indicator(input);
		}
	}

	#endregion

}

#endregion

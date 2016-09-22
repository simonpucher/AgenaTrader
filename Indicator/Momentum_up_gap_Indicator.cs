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
/// Version: in progress
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
    [Description("The force is strong in this instrument.")]
    public class Momentum_up_gap_Indicator : UserIndicator
    {

        //input
        private int _percentage = 3;
        private Color _plot0color = Const.DefaultIndicatorColor;
        private int _plot0width = Const.DefaultLineWidth;
        private DashStyle _plot0dashstyle = Const.DefaultIndicatorDashStyle;


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

        protected override void InitRequirements()
        {
            //Print("InitRequirements");
            base.InitRequirements();
        }

        protected override void OnBarUpdate()
        {
            double gapopen = ((Open[0] - Close[1]) * 100) / Close[1];
            double gapclose = ((Close[0] - Close[1]) * 100) / Close[1];
            if (gapopen >= this.Percentage && gapclose >= this.Percentage)
            {
                PlotLine.Set(1);
            }
            else
            {
                PlotLine.Set(0);
            }

            //Bollinger bb = Bollinger(2, 20);

            //DrawLine("Plot_Middle" + Time[0].ToString(), this.AutoScale, 1, bb.Middle[1], 0, bb.Middle[0], this.Plot1Color, this.Dash1Style, this.Plot1Width);
            //DrawLine("Plot_Low" + Time[0].ToString(), this.AutoScale, 1, bb.Lower[1], 0, bb.Lower[0], this.Plot0Color, this.Dash0Style, this.Plot0Width);
            //DrawLine("Plot_High" + Time[0].ToString(), this.AutoScale, 1, bb.Upper[1], 0, bb.Upper[0], this.Plot0Color, this.Dash0Style, this.Plot0Width);

            //if (High[0] < bb.Lower[0] || Low[0] > bb.Upper[0])
            //{
            //    //ok
            //}
            //else
            //{
            //    this.BarColor = Color.White;
            //}

            ////Trigger
            //double signal = 0;
            //if (High[1] < bb.Lower[1])
            //{
            //    if (Low[0] > High[1] || High[0] > High[1])
            //    {
            //        //DrawDot("ArrowLong_Entry" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.LightGreen);
            //        DrawArrowUp("ArrowLong_Entry" + +Bars[0].Time.Ticks, this.AutoScale, 0, Bars[0].Low, Color.LightGreen);
            //        signal = 1;
            //    }
            //}
            //else if (Low[1] > bb.Upper[1])
            //{
            //    if (Low[0] < Low[1] || High[0] < Low[1])
            //    {
            //        //DrawDiamond("ArrowShort_Entry" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.LightGreen);
            //        DrawArrowDown("ArrowShort_Entry" + +Bars[0].Time.Ticks, this.AutoScale, 0, Bars[0].High, Color.Red);
            //        signal = -1;
            //    }
            //}


            //PlotLine.Set(signal);


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
        [Category("Plots")]
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
		/// The force is strong in this instrument.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator()
        {
			return Momentum_up_gap_Indicator(Input);
		}

		/// <summary>
		/// The force is strong in this instrument.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Momentum_up_gap_Indicator>(input);

			if (indicator != null)
				return indicator;

			indicator = new Momentum_up_gap_Indicator
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
		/// The force is strong in this instrument.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator()
		{
			return LeadIndicator.Momentum_up_gap_Indicator(Input);
		}

		/// <summary>
		/// The force is strong in this instrument.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator(IDataSeries input)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.Momentum_up_gap_Indicator(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// The force is strong in this instrument.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator()
		{
			return LeadIndicator.Momentum_up_gap_Indicator(Input);
		}

		/// <summary>
		/// The force is strong in this instrument.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator(IDataSeries input)
		{
			return LeadIndicator.Momentum_up_gap_Indicator(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// The force is strong in this instrument.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator()
		{
			return LeadIndicator.Momentum_up_gap_Indicator(Input);
		}

		/// <summary>
		/// The force is strong in this instrument.
		/// </summary>
		public Momentum_up_gap_Indicator Momentum_up_gap_Indicator(IDataSeries input)
		{
			return LeadIndicator.Momentum_up_gap_Indicator(input);
		}
	}

	#endregion

}

#endregion

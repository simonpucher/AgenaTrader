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
/// Version: 1.2.1
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://github.com/ScriptTrading/Basic-Package/blob/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    public enum IndicatorEnum_HighestHighValue
    {
        SMA = 1,
        EMA = 2
    }

	[Description("Compare the current value of an indicator to latest high value of the indicator in a defined period of time.")]
	public class HighestHighValue_Indicator : UserIndicator
	{
        //input
        private Color _plot1color = Const.DefaultIndicatorColor;
        private int _plot1width = Const.DefaultLineWidth;
        private DashStyle _plot1dashstyle = Const.DefaultIndicatorDashStyle;
        private int _indicatorEMAPeriod = 200;
        private int _indicatorSMAPeriod = 200;
        private int _comparisonPeriod = 30;
        private IndicatorEnum_HighestHighValue _indicatorenum = IndicatorEnum_HighestHighValue.SMA;

        //output


        //internal
        private DataSeries _DATA_List;

        /// <summary>
        /// Initalizie the Plot.
        /// </summary>
		protected override void Initialize()
		{
            Add(new Plot(new Pen(this.Plot1Color, this.Plot0Width), PlotStyle.Line, "HighestHighValue_Indicator"));

            CalculateOnBarClose = true;
            Overlay = false;
		}

        /// <summary>
        /// Init all variables on startup.
        /// </summary>
        protected override void OnStartUp()
        {
            this._DATA_List = new DataSeries(this);
        }

        /// <summary>
        /// Recalculate all data on each each bar update. 
        /// </summary>
		protected override void OnBarUpdate()
		{
            double currentvalue = 0.0;
            switch (IndicatorEnum)
            {
                case IndicatorEnum_HighestHighValue.SMA:
                    currentvalue = SMA(IndicatorSMAPeriod)[0];
                    break;
                case IndicatorEnum_HighestHighValue.EMA:
                    currentvalue = EMA(IndicatorEMAPeriod)[0];
                    break;
                default:
                    break;
            }
            
            double lasthighvalue = _DATA_List.Reverse().Take(this.ComparisonPeriod).Max();

            if (lasthighvalue < currentvalue)
            {
                MyPlot1.Set(1);
            }
            else
            {
                MyPlot1.Set(0);
            }

            this._DATA_List.Set(currentvalue);

            //set the color
            PlotColors[0][0] = this.Plot1Color;
            Plots[0].PenStyle = this.Dash0Style;
            Plots[0].Pen.Width = this.Plot0Width;

		}


        public override string ToString()
        {
            return "HHV";
        }

        public override string DisplayName
        {
            get
            {
                return "HHV";
            }
        }


		#region Properties

        #region Input

        [Description("Type of the indicator")]
        [Category("Parameters")]
        [DisplayName("Type of the indicator")]
        public IndicatorEnum_HighestHighValue IndicatorEnum
        {
            get { return _indicatorenum; }
            set { _indicatorenum = value; }
        }


            [Description("Period for the SMA")]
            [Category("Parameters")]
            [DisplayName("Period SMA")]
            public int IndicatorSMAPeriod
            {
                get { return _indicatorSMAPeriod; }
                set { _indicatorSMAPeriod = value; }
            }

            [Description("Period for the EMA")]
            [Category("Parameters")]
            [DisplayName("Period EMA")]
            public int IndicatorEMAPeriod
            {
                get { return _indicatorEMAPeriod; }
                set { _indicatorEMAPeriod = value; }
            }



            [Description("Period for comparison")]
            [Category("Parameters")]
            [DisplayName("Period for comparison")]
            public int ComparisonPeriod
            {
                get { return _comparisonPeriod; }
                set { _comparisonPeriod = value; }
            }

            #region Plotstyle

                [XmlIgnore()]
                [Description("Select Color")]
                [Category("Parameters")]
                [DisplayName("Pricline")]
                public Color Plot1Color
                {
                    get { return _plot1color; }
                    set { _plot1color = value; }
                }
                [Browsable(false)]
                public string Plot1ColorSerialize
                {
                    get { return SerializableColor.ToString(_plot1color); }
                    set { _plot1color = SerializableColor.FromString(value); }
                }

                /// <summary>
                /// </summary>
                [Description("Width for Indicator.")]
                [Category("Parameters")]
                [DisplayName("Line Width Indicator")]
                public int Plot0Width
                {
                    get { return _plot1width; }
                    set { _plot1width = Math.Max(1, value); }
                }


                /// <summary>
                /// </summary>
                [Description("DashStyle for Indicator.")]
                [Category("Parameters")]
                [DisplayName("Dash Style Indicator")]
                public DashStyle Dash0Style
                {
                    get { return _plot1dashstyle; }
                    set { _plot1dashstyle = value; }
                }

            #endregion

            #endregion


            #region Output

            [Browsable(false)]
            [XmlIgnore()]
            public DataSeries MyPlot1
            {
                get { return Values[0]; }
            }

        #endregion

        #endregion
    }
}


#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator : Indicator
	{
		/// <summary>
		/// Compare the current value of an indicator to latest high value of the indicator in a defined period of time.
		/// </summary>
		public HighestHighValue_Indicator HighestHighValue_Indicator(IndicatorEnum_HighestHighValue indicatorEnum, System.Int32 indicatorSMAPeriod, System.Int32 indicatorEMAPeriod, System.Int32 comparisonPeriod, Color plot1Color, System.Int32 plot0Width, DashStyle dash0Style)
        {
			return HighestHighValue_Indicator(Input, indicatorEnum, indicatorSMAPeriod, indicatorEMAPeriod, comparisonPeriod, plot1Color, plot0Width, dash0Style);
		}

		/// <summary>
		/// Compare the current value of an indicator to latest high value of the indicator in a defined period of time.
		/// </summary>
		public HighestHighValue_Indicator HighestHighValue_Indicator(IDataSeries input, IndicatorEnum_HighestHighValue indicatorEnum, System.Int32 indicatorSMAPeriod, System.Int32 indicatorEMAPeriod, System.Int32 comparisonPeriod, Color plot1Color, System.Int32 plot0Width, DashStyle dash0Style)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<HighestHighValue_Indicator>(input, i => i.IndicatorEnum == indicatorEnum && i.IndicatorSMAPeriod == indicatorSMAPeriod && i.IndicatorEMAPeriod == indicatorEMAPeriod && i.ComparisonPeriod == comparisonPeriod && i.Plot1Color == plot1Color && i.Plot0Width == plot0Width && i.Dash0Style == dash0Style);

			if (indicator != null)
				return indicator;

			indicator = new HighestHighValue_Indicator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							IndicatorEnum = indicatorEnum,
							IndicatorSMAPeriod = indicatorSMAPeriod,
							IndicatorEMAPeriod = indicatorEMAPeriod,
							ComparisonPeriod = comparisonPeriod,
							Plot1Color = plot1Color,
							Plot0Width = plot0Width,
							Dash0Style = dash0Style
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
		/// Compare the current value of an indicator to latest high value of the indicator in a defined period of time.
		/// </summary>
		public HighestHighValue_Indicator HighestHighValue_Indicator(IndicatorEnum_HighestHighValue indicatorEnum, System.Int32 indicatorSMAPeriod, System.Int32 indicatorEMAPeriod, System.Int32 comparisonPeriod, Color plot1Color, System.Int32 plot0Width, DashStyle dash0Style)
		{
			return LeadIndicator.HighestHighValue_Indicator(Input, indicatorEnum, indicatorSMAPeriod, indicatorEMAPeriod, comparisonPeriod, plot1Color, plot0Width, dash0Style);
		}

		/// <summary>
		/// Compare the current value of an indicator to latest high value of the indicator in a defined period of time.
		/// </summary>
		public HighestHighValue_Indicator HighestHighValue_Indicator(IDataSeries input, IndicatorEnum_HighestHighValue indicatorEnum, System.Int32 indicatorSMAPeriod, System.Int32 indicatorEMAPeriod, System.Int32 comparisonPeriod, Color plot1Color, System.Int32 plot0Width, DashStyle dash0Style)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.HighestHighValue_Indicator(input, indicatorEnum, indicatorSMAPeriod, indicatorEMAPeriod, comparisonPeriod, plot1Color, plot0Width, dash0Style);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Compare the current value of an indicator to latest high value of the indicator in a defined period of time.
		/// </summary>
		public HighestHighValue_Indicator HighestHighValue_Indicator(IndicatorEnum_HighestHighValue indicatorEnum, System.Int32 indicatorSMAPeriod, System.Int32 indicatorEMAPeriod, System.Int32 comparisonPeriod, Color plot1Color, System.Int32 plot0Width, DashStyle dash0Style)
		{
			return LeadIndicator.HighestHighValue_Indicator(Input, indicatorEnum, indicatorSMAPeriod, indicatorEMAPeriod, comparisonPeriod, plot1Color, plot0Width, dash0Style);
		}

		/// <summary>
		/// Compare the current value of an indicator to latest high value of the indicator in a defined period of time.
		/// </summary>
		public HighestHighValue_Indicator HighestHighValue_Indicator(IDataSeries input, IndicatorEnum_HighestHighValue indicatorEnum, System.Int32 indicatorSMAPeriod, System.Int32 indicatorEMAPeriod, System.Int32 comparisonPeriod, Color plot1Color, System.Int32 plot0Width, DashStyle dash0Style)
		{
			return LeadIndicator.HighestHighValue_Indicator(input, indicatorEnum, indicatorSMAPeriod, indicatorEMAPeriod, comparisonPeriod, plot1Color, plot0Width, dash0Style);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Compare the current value of an indicator to latest high value of the indicator in a defined period of time.
		/// </summary>
		public HighestHighValue_Indicator HighestHighValue_Indicator(IndicatorEnum_HighestHighValue indicatorEnum, System.Int32 indicatorSMAPeriod, System.Int32 indicatorEMAPeriod, System.Int32 comparisonPeriod, Color plot1Color, System.Int32 plot0Width, DashStyle dash0Style)
		{
			return LeadIndicator.HighestHighValue_Indicator(Input, indicatorEnum, indicatorSMAPeriod, indicatorEMAPeriod, comparisonPeriod, plot1Color, plot0Width, dash0Style);
		}

		/// <summary>
		/// Compare the current value of an indicator to latest high value of the indicator in a defined period of time.
		/// </summary>
		public HighestHighValue_Indicator HighestHighValue_Indicator(IDataSeries input, IndicatorEnum_HighestHighValue indicatorEnum, System.Int32 indicatorSMAPeriod, System.Int32 indicatorEMAPeriod, System.Int32 comparisonPeriod, Color plot1Color, System.Int32 plot0Width, DashStyle dash0Style)
		{
			return LeadIndicator.HighestHighValue_Indicator(input, indicatorEnum, indicatorSMAPeriod, indicatorEMAPeriod, comparisonPeriod, plot1Color, plot0Width, dash0Style);
		}
	}

	#endregion

}

#endregion

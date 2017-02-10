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
/// Version: 1.1
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

    public enum ElderRayTyp {
        BullPower = 0,
        BearPower = 1
    }

	[Description("Elder Ray - Bull and Bear Power")]
	public class Elder_Ray_Bull_and_Bear_Power_Indicator : UserIndicator
	{
        private int _period = 13;
        private ElderRayTyp _ElderRayTyp = ElderRayTyp.BullPower;

        private DoubleSeries ds_bull_power;
        private DoubleSeries ds_bear_power;


        protected override void OnInit()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Gray), "bull_power"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Gray), "bear_power"));

            ds_bull_power = new DoubleSeries(this);
            ds_bear_power = new DoubleSeries(this);
        }

		protected override void OnCalculate()
		{
            EMA ema = EMA(this.Period);
            double bull_power = High[0] - ema[0];
            double bear_power = Low[0] - ema[0];
            ds_bull_power.Set(bull_power);
            ds_bear_power.Set(bear_power);

            if (_ElderRayTyp == ElderRayTyp.BullPower)
            {
                MyPlot1.Set(bull_power);
            }
            else
            {
                MyPlot2.Set(bear_power);
            }
           

            //Set the color
            if (ema[0] > ema[1])
            {
                PlotColors[0][0] = Color.Green;
                Plots[0].Pen.Width = 2;
            }
            else
            {
                PlotColors[0][0] = Color.Red;
                Plots[0].Pen.Width = 1;
            }
            Plots[0].PenStyle = DashStyle.Solid;
            Plots[0].PlotStyle = PlotStyle.Bar;

            if (ema[0] < ema[1])
            {
                PlotColors[1][0] = Color.Red;
                Plots[1].Pen.Width = 2;
            }
            else
            {
                PlotColors[1][0] = Color.Green;
                Plots[1].Pen.Width = 1;
            }
            Plots[1].PenStyle = DashStyle.Solid;
            Plots[1].PlotStyle = PlotStyle.Bar;

            if (ema[0] > ema[1] && bear_power < 0 && bear_power > ds_bear_power.Get(1))
            {
                AddChartArrowUp("ArrowLong" +Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].Low, Color.LightGreen);
            }

            if (ema[0] < ema[1] && bull_power > 0 && bull_power < ds_bull_power.Get(1))
            {
                AddChartArrowDown("ArrowShort" + Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].High, Color.Red);
            }

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
		public DataSeries MyPlot1
		{
			get { return Outputs[0]; }
		}

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries MyPlot2
        {
            get { return Outputs[1]; }
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
		public Elder_Ray_Bull_and_Bear_Power_Indicator Elder_Ray_Bull_and_Bear_Power_Indicator(ElderRayTyp elderRayTyp, System.Int32 period)
        {
			return Elder_Ray_Bull_and_Bear_Power_Indicator(InSeries, elderRayTyp, period);
		}

		/// <summary>
		/// Elder Ray - Bull and Bear Power
		/// </summary>
		public Elder_Ray_Bull_and_Bear_Power_Indicator Elder_Ray_Bull_and_Bear_Power_Indicator(IDataSeries input, ElderRayTyp elderRayTyp, System.Int32 period)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Elder_Ray_Bull_and_Bear_Power_Indicator>(input, i => i.ElderRayTyp == elderRayTyp && i.Period == period);

			if (indicator != null)
				return indicator;

			indicator = new Elder_Ray_Bull_and_Bear_Power_Indicator
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
		public Elder_Ray_Bull_and_Bear_Power_Indicator Elder_Ray_Bull_and_Bear_Power_Indicator(ElderRayTyp elderRayTyp, System.Int32 period)
		{
			return LeadIndicator.Elder_Ray_Bull_and_Bear_Power_Indicator(InSeries, elderRayTyp, period);
		}

		/// <summary>
		/// Elder Ray - Bull and Bear Power
		/// </summary>
		public Elder_Ray_Bull_and_Bear_Power_Indicator Elder_Ray_Bull_and_Bear_Power_Indicator(IDataSeries input, ElderRayTyp elderRayTyp, System.Int32 period)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.Elder_Ray_Bull_and_Bear_Power_Indicator(input, elderRayTyp, period);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Elder Ray - Bull and Bear Power
		/// </summary>
		public Elder_Ray_Bull_and_Bear_Power_Indicator Elder_Ray_Bull_and_Bear_Power_Indicator(ElderRayTyp elderRayTyp, System.Int32 period)
		{
			return LeadIndicator.Elder_Ray_Bull_and_Bear_Power_Indicator(InSeries, elderRayTyp, period);
		}

		/// <summary>
		/// Elder Ray - Bull and Bear Power
		/// </summary>
		public Elder_Ray_Bull_and_Bear_Power_Indicator Elder_Ray_Bull_and_Bear_Power_Indicator(IDataSeries input, ElderRayTyp elderRayTyp, System.Int32 period)
		{
			return LeadIndicator.Elder_Ray_Bull_and_Bear_Power_Indicator(input, elderRayTyp, period);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Elder Ray - Bull and Bear Power
		/// </summary>
		public Elder_Ray_Bull_and_Bear_Power_Indicator Elder_Ray_Bull_and_Bear_Power_Indicator(ElderRayTyp elderRayTyp, System.Int32 period)
		{
			return LeadIndicator.Elder_Ray_Bull_and_Bear_Power_Indicator(InSeries, elderRayTyp, period);
		}

		/// <summary>
		/// Elder Ray - Bull and Bear Power
		/// </summary>
		public Elder_Ray_Bull_and_Bear_Power_Indicator Elder_Ray_Bull_and_Bear_Power_Indicator(IDataSeries input, ElderRayTyp elderRayTyp, System.Int32 period)
		{
			return LeadIndicator.Elder_Ray_Bull_and_Bear_Power_Indicator(input, elderRayTyp, period);
		}
	}

	#endregion

}

#endregion

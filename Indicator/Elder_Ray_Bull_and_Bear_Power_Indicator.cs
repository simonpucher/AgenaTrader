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
			Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Gray), "bull_power"));
            Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Gray), "bear_power"));

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
                OutputDescriptors[0].Pen.Width = 2;
            }
            else
            {
                PlotColors[0][0] = Color.Red;
                OutputDescriptors[0].Pen.Width = 1;
            }
            OutputDescriptors[0].PenStyle = DashStyle.Solid;
            OutputDescriptors[0].OutputSerieDrawStyle = OutputSerieDrawStyle.Bar;

            if (ema[0] < ema[1])
            {
                PlotColors[1][0] = Color.Red;
                OutputDescriptors[1].Pen.Width = 2;
            }
            else
            {
                PlotColors[1][0] = Color.Green;
                OutputDescriptors[1].Pen.Width = 1;
            }
            OutputDescriptors[1].PenStyle = DashStyle.Solid;
            OutputDescriptors[1].OutputSerieDrawStyle = OutputSerieDrawStyle.Bar;

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
        [InputParameter]
        [DisplayName("ElderRayTyp")]
        public ElderRayTyp ElderRayTyp
        {
            get { return _ElderRayTyp; }
            set { _ElderRayTyp = value; }

        }
        [Description("Period of the EMA.")]
        [InputParameter]
        [DisplayName("Period")]
        public int Period
        {
            get { return _period; }
            set { _period = value; }

        }
        #endregion
    }
}
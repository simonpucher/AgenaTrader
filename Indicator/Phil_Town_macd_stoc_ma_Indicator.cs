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
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("Phil Town MACD Stocastic EMA.")]
	public class Phil_Town_macd_stoc_ma_Indicator : UserIndicator
	{

        //private int _period = 10;

        protected override void OnInit()
		{
			Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Orange), "Phil_Town_macd_stoc_ema_Plot"));
			CalculateOnClosedBar = true;
        }

		protected override void OnCalculate()
		{
          MACD macd = MACD(8, 17, 9);
            StochasticsFast stoc = StochasticsFast(5, 14);
            EMA ema = EMA(10);

          
            //if (macd[0] > macd.Avg[0]) {
            //    //AddChartArrowUp("ArrowLong_MACD" + +Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].Low, Color.Green);
            //}

            //if (stoc.K[0] > stoc.D[0])
            //{
            //    //AddChartArrowUp("ArrowLong_STOC" + +Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].High, Color.DarkMagenta);
            //}

            //if (Bars[0].Close > ema[0])
            //{
            //    //AddChartArrowUp("ArrowLong_EMA" + +Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].High - (Bars[0].Range/2), Color.DarkGoldenrod);
            //}


            if (macd[0] > macd.Avg[0] && stoc.K[0] > stoc.D[0] && Bars[0].Close > ema[0])
            {
                AddChartArrowUp("ArrowLong_philtown" + +Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].Low, Color.Green);
                MyPlot1[0] = 1;
            } else if(macd[0] < macd.Avg[0] && stoc.K[0] < stoc.D[0] && Bars[0].Close < ema[0]){
                AddChartArrowDown("ArrowShort_philtown" + +Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].High, Color.Red);
                MyPlot1[0] = -1;
            }
            else
            {
                MyPlot1[0] = 0;
            }

        }

		#region Properties

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries MyPlot1
		{
			get { return Outputs[0]; }
		}
         
        ///// <summary>
        ///// </summary>
        //[Description("Select the period for the bullbreath count.")]
        //[Category("Parameters")]
        //[DisplayName("Period")]
        //public int Period
        //{
        //    get { return _period; }
        //    set
        //    {
        //        if (value < 1) value = 1;
        //        _period = value;
        //    }
        //}


        public override string ToString()
        {
            return GetNameOnchart();
        }

        public override string DisplayName
        {
            get
            {
                return GetNameOnchart();
            }
        }


        private string GetNameOnchart()
        {
            return "Phil Town MACD STOC EMA (I)";
        }

        #endregion
    }
}
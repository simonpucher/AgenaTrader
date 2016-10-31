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
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// todo
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://github.com/ScriptTrading/Basic-Package/blob/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Indicator which shows deep correction of ongoing trend")]
    public class DeepCorrectionTrend_Indikator : UserIndicator
    {
        //constants
        const double MarketPhaseDeepCorrectionWithTrendLong = 5.3d;
        const double MarketPhaseDeepCorrectionWithTrendShort = -5.3d;
        const double MarketPhaseDeepCorrectionLong = 5.1d;
        const double MarketPhaseDeepCorrectionShort = -5.1d;
        int _trendSize = 1;

        bool FirstOnBarUpdate = false;
        bool FirstCalculate = false;

        protected override void Initialize()
        {
            Add(new Plot(Color.Brown, PlotStyle.Block, "DeepCorrection"));
            Add(new Plot(Color.Green, "Entry"));
            Overlay = false;
            CalculateOnBarClose = true;
           // BarsRequired = 20;
        }

        protected override void OnBarUpdate()
        {

            //for debugging reason only, just to get a hook in
            if (CurrentBar + 1 == 1051)
            {
                int a = 1 + 1;
            }

            if (CurrentBar < BarsRequired)
            {
                return;
            }


            if (FirstOnBarUpdate == false)
            {
                Print("FirstOnBarUpdate " + Bars[0].Time + " Indi");
                FirstOnBarUpdate = true;
            }

            //Lets call the calculate method and save the result with the trade action
            ResultValue_DeepCorrection ResultValue = this.calculate(Close, TrendSize, Bars[0], "Indi");

            if (ResultValue.Entry.HasValue)
            {
                switch (ResultValue.Entry)
                {
                    case OrderAction.Buy:
                        Values[1].Set(1);
                        break;
                    case OrderAction.SellShort:
                        //DrawDiamond("ArrowShort_Entry" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.LightGreen);
                        Values[1].Set(-1);
                        break;
                }
            }
            else
            {
                //Value was null so nothing to do.
                Values[1].Set(0);
            }


        }


        public ResultValue_DeepCorrection calculate(IDataSeries Input, int TrendSize, IBar myBar, string caller)
        {


            if (FirstCalculate == false)
            {
                Print("FirstCalculate " + myBar.Time + caller);
                FirstCalculate = true;
            }

            //Create a return object
            ResultValue_DeepCorrection ResultValue = new ResultValue_DeepCorrection();

            if (MarketPhasesAdv(Input, TrendSize)[0] == MarketPhaseDeepCorrectionLong
             || MarketPhasesAdv(Input, TrendSize)[0] == MarketPhaseDeepCorrectionWithTrendLong)
            {
                //Print(Bars[0].Time + " " + MarketPhasesAdv(Input, TrendSize)[0] + " tmp Pkt3 " + P123(Close, _trendSize).TempP3Price[0] + " valid Pkt3 " + P123(Input, _trendSize).ValidP3Price[0]);

                //try to get the temporary P3, if it does not exist, take the already validated P3
                if (P123(Input, _trendSize).TempP3Price[0] != 0)
                {
                    ResultValue.DeepCorrection = true;
                    ResultValue.Entry = OrderAction.Buy;
                    ResultValue.StopLoss = P123(Input, _trendSize).TempP3Price[0];
                    
                }
                else if (P123(Input, _trendSize).ValidP3Price[0] != 0)
                {
                    ResultValue.DeepCorrection = true;
                    ResultValue.Entry = OrderAction.Buy;
                    ResultValue.StopLoss = P123(Input, _trendSize).ValidP3Price[0];
                }

                //Check, if current Price is lower than P2 (because we want to go long towards P2)
                if (Input[0] < P123(Input, _trendSize).P2Price[0])
                {
                    ResultValue.Target = P123(Input, _trendSize).P2Price[0];
                    Print("Indikator" + myBar.Time + " Long " + "Close: " + myBar.Close + " StopLoss: " + ResultValue.StopLoss + " Target: " + ResultValue.Target + " Marktphase: " + MarketPhasesAdv(Input, TrendSize)[0] + " BarsCount: " + Input.Count);
                }
                else
                {
                    ResultValue.Target = Input[0] * 1.005;
                    Print("Indikator" + myBar.Time + " Long " + "Close: " + myBar.Close + " StopLoss: " + ResultValue.StopLoss + " Target: " + ResultValue.Target + " Marktphase: " + MarketPhasesAdv(Input, TrendSize)[0] + " BarsCount: " + Input.Count);
                }
            }
            else if (MarketPhasesAdv(Input, TrendSize)[0] == MarketPhaseDeepCorrectionShort
                  || MarketPhasesAdv(Input, TrendSize)[0] == MarketPhaseDeepCorrectionWithTrendShort)
            {

                //try to get the temporary P3, if it does not exist, take the already validated P3
                if (P123(Input, _trendSize).TempP3Price[0] != 0)
                {
                    ResultValue.DeepCorrection = true;
                    ResultValue.Entry = OrderAction.SellShort;
                    ResultValue.StopLoss = P123(Input, _trendSize).TempP3Price[0];
                    ResultValue.Target = P123(Input, _trendSize).P2Price[0];
                }
                else if (P123(Input, _trendSize).ValidP3Price[0] != 0)
                {
                    ResultValue.DeepCorrection = true;
                    ResultValue.Entry = OrderAction.SellShort;
                    ResultValue.StopLoss = P123(Input, _trendSize).ValidP3Price[0];
                }
                //Check, if current Price is higher than P2 (because we want to go short towards P2)
                    if (Input[0] > P123(Input, _trendSize).P2Price[0])
                {
                    ResultValue.Target = P123(Input, _trendSize).P2Price[0];
                }
                else
                {
                    ResultValue.Target = Input[0] / 1.005;
                }
            }
            return ResultValue;
        }

        public override string ToString()
        {
            return "Deep Correction Trend (S)";
        }

        public override string DisplayName
        {
            get
            {
                return "Deep Correction Trend (S)";
            }
        }

        #region Properties

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries DeepCorrection
        {
            get { return Values[0]; }
        }


        /// <summary>
        /// </summary>
        [Description("Trendsize (0-3)")]
        [Category("Parameters")]
        [DisplayName("Trendsize (0-3)")]
        public int TrendSize
        {
            get { return _trendSize; }
            set { _trendSize = value; }
        }

        #endregion
    }

    public class ResultValue_DeepCorrection : ResultValue
    {
        private double _stopLoss;
        private double _target;
        private bool _deepCorrection;

        public bool DeepCorrection
        {
            get { return _deepCorrection; }
            set { _deepCorrection = value; }
        }

        public double StopLoss
        {
            get { return _stopLoss; }
            set { _stopLoss = value; }
        }

        public double Target
        {
            get { return _target; }
            set { _target = value; }
        }
    }


}
#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator : Indicator
	{
		/// <summary>
		/// Indicator which shows deep correction of ongoing trend
		/// </summary>
		public DeepCorrectionTrend_Indikator DeepCorrectionTrend_Indikator(System.Int32 trendSize)
        {
			return DeepCorrectionTrend_Indikator(Input, trendSize);
		}

		/// <summary>
		/// Indicator which shows deep correction of ongoing trend
		/// </summary>
		public DeepCorrectionTrend_Indikator DeepCorrectionTrend_Indikator(IDataSeries input, System.Int32 trendSize)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<DeepCorrectionTrend_Indikator>(input, i => i.TrendSize == trendSize);

			if (indicator != null)
				return indicator;

			indicator = new DeepCorrectionTrend_Indikator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							TrendSize = trendSize
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
		/// Indicator which shows deep correction of ongoing trend
		/// </summary>
		public DeepCorrectionTrend_Indikator DeepCorrectionTrend_Indikator(System.Int32 trendSize)
		{
			return LeadIndicator.DeepCorrectionTrend_Indikator(Input, trendSize);
		}

		/// <summary>
		/// Indicator which shows deep correction of ongoing trend
		/// </summary>
		public DeepCorrectionTrend_Indikator DeepCorrectionTrend_Indikator(IDataSeries input, System.Int32 trendSize)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.DeepCorrectionTrend_Indikator(input, trendSize);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Indicator which shows deep correction of ongoing trend
		/// </summary>
		public DeepCorrectionTrend_Indikator DeepCorrectionTrend_Indikator(System.Int32 trendSize)
		{
			return LeadIndicator.DeepCorrectionTrend_Indikator(Input, trendSize);
		}

		/// <summary>
		/// Indicator which shows deep correction of ongoing trend
		/// </summary>
		public DeepCorrectionTrend_Indikator DeepCorrectionTrend_Indikator(IDataSeries input, System.Int32 trendSize)
		{
			return LeadIndicator.DeepCorrectionTrend_Indikator(input, trendSize);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Indicator which shows deep correction of ongoing trend
		/// </summary>
		public DeepCorrectionTrend_Indikator DeepCorrectionTrend_Indikator(System.Int32 trendSize)
		{
			return LeadIndicator.DeepCorrectionTrend_Indikator(Input, trendSize);
		}

		/// <summary>
		/// Indicator which shows deep correction of ongoing trend
		/// </summary>
		public DeepCorrectionTrend_Indikator DeepCorrectionTrend_Indikator(IDataSeries input, System.Int32 trendSize)
		{
			return LeadIndicator.DeepCorrectionTrend_Indikator(input, trendSize);
		}
	}

	#endregion

}

#endregion

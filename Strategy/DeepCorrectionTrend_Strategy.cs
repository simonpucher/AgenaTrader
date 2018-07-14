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
/// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Strategy which trades deep correction of ongoing trend")]
    public class DeepCorrectionTrend_Strategy : UserStrategy
    {
        #region Variables
        //constants
        const double MarketPhaseDeepCorrectionLong = 5.3d;
        const double MarketPhaseDeepCorrectionShort = -5.3d;

        //input
        private int _trendSize = 1;

        //internal
        private IOrder _orderenterlong;
        private IOrder _orderentershort;
        private DeepCorrectionTrend_Indikator _DeepCorrectionTrend_Indikator;
        bool FirstOnBarUpdate = false;
        bool FirstCalculate = false;

        #endregion

        protected override void OnInit()
        {
            CalculateOnClosedBar = true;
            RequiredBarsCount = 5;
            _DeepCorrectionTrend_Indikator = new DeepCorrectionTrend_Indikator();
        }

        protected override void OnOrderExecution(IExecution execution)
        {
            if (execution.PositionType == PositionType.Flat)
            {
                _orderentershort = null;
                _orderenterlong = null;
            }
        }


        protected override void OnCalculate()
        {
            //for debugging reason only, just to get a hook in
            if (ProcessingBarIndex + 1 == 157)
            {
                int a = 1 + 1;
            }

            if (FirstOnBarUpdate == false)
            {
                Print("FirstOnBarUpdate " + Bars[0].Time + " Strat");
                FirstOnBarUpdate = true;
            }


            //Lets call the calculate method and save the result with the trade action
            ResultValue_DeepCorrection ResultValue = this._DeepCorrectionTrend_Indikator.calculate(Close, TrendSize, Bars[0], "Strat");

            //Entry
            if (ResultValue.Entry.HasValue)
            {
                switch (ResultValue.Entry)
                {
                    case OrderDirection.Buy:
                        this.DoEnterLong(ResultValue.StopLoss, ResultValue.Target);
                        break;
                    case OrderDirection.Sell:
                        //            this.DoEnterShort(ResultValue.StopLoss, ResultValue.Target);
                        break;
                }
            }
        }

        /// <summary>
        /// Create Long Order and Stop.
        /// </summary>
        private void DoEnterLong(double StopLoss, double Target)
        {
            if (_orderenterlong == null)
            {
                Print("Strategie" + Bars[0].Time + " Long " + "Close: " + Bars[0].Close + " StopLoss: " + StopLoss + " Target: " + Target);
                _orderenterlong = SubmitOrder(new StrategyOrderParameters {Direction = OrderDirection.Buy, Type = OrderType.Market, Quantity = 10,
                                            Mode = OrderMode.Direct,
                                            //Mode = this.GetType().Name + " " + PositionType.Long + "_" + this.Instrument.Symbol + "_" + Bars[0].Timestamp.Ticks.ToString(),
                                            Instrument =  this.Instrument, TimeFrame =  this.TimeFrame});
                SetUpStopLoss(_orderenterlong.Name, CalculationMode.Price, StopLoss, false);
                SetUpProfitTarget(_orderenterlong.Name, CalculationMode.Price, Target);
            }
        }

        /// <summary>
        /// Create Short Order and Stop.
        /// </summary>
        private void DoEnterShort(double StopLoss, double Target)
        {
            if (_orderentershort == null)
            {
                Print("Short" + "Close: " + Bars[0].Close + "StopLoss: " + StopLoss + " Target: " + Target);
                _orderentershort = SubmitOrder(new StrategyOrderParameters {Direction = OrderDirection.Sell, Type = OrderType.Market, Quantity = GlobalUtilities.AdjustPositionToRiskManagement(this.Root.Core.AccountManager, this.Root.Core.PreferenceManager, this.Instrument, Bars[0].Close), SignalName =  this.GetType().Name + " " + PositionType.Short + "_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(), Instrument =  this.Instrument, TimeFrame =  this.TimeFrame});
                ////SetUpStopLoss(_orderentershort.Name, CalculationMode.Price, StopLoss, false);
                ////SetUpProfitTarget(_orderentershort.Name, CalculationMode.Price, Target);
                SetUpStopLoss(_orderenterlong.Name, CalculationMode.Price, Bars[0].Close * 1.05, false);
                SetUpProfitTarget(_orderenterlong.Name, CalculationMode.Price, Bars[0].Close / 1.11);
            }
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

        [Description("Trendsize (0-3)")]
        [InputParameter]
        public int TrendSize
        {
            get { return _trendSize; }
            set { _trendSize = Math.Max(1, value); }
        }

        #endregion
    }
}

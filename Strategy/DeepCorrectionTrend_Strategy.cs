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
/// You will find this indicator on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
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

        protected override void Initialize()
        {
            CalculateOnBarClose = true;
            BarsRequired = 5;
            _DeepCorrectionTrend_Indikator = new DeepCorrectionTrend_Indikator();
        }

        protected override void OnExecution(IExecution execution)
        {
            if (execution.MarketPosition == PositionType.Flat)
            {
                _orderentershort = null;
                _orderenterlong = null;
            }
        }


        protected override void OnBarUpdate()
        {
            //for debugging reason only, just to get a hook in
            if (CurrentBar + 1 == 157)
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
                    case OrderAction.Buy:
                        this.DoEnterLong(ResultValue.StopLoss, ResultValue.Target);
                        break;
                    case OrderAction.SellShort:
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
                _orderenterlong = EnterLong(//GlobalUtilities.AdjustPositionToRiskManagement(this.Root.Core.AccountManager, this.Root.Core.PreferenceManager, this.Instrument, Bars[0].Close), 
                                           10,
                                           this.GetType().Name + " " + PositionType.Long + "_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(),
                                           this.Instrument,
                                           this.TimeFrame);
                SetStopLoss(_orderenterlong.Name, CalculationMode.Price, StopLoss, false);
                SetProfitTarget(_orderenterlong.Name, CalculationMode.Price, Target);
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
                _orderentershort = EnterShort(GlobalUtilities.AdjustPositionToRiskManagement(this.Root.Core.AccountManager, this.Root.Core.PreferenceManager, this.Instrument, Bars[0].Close), this.GetType().Name + " " + PositionType.Short + "_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(), this.Instrument, this.TimeFrame);
                ////SetStopLoss(_orderentershort.Name, CalculationMode.Price, StopLoss, false);
                ////SetProfitTarget(_orderentershort.Name, CalculationMode.Price, Target);
                SetStopLoss(_orderenterlong.Name, CalculationMode.Price, Bars[0].Close * 1.05, false);
                SetProfitTarget(_orderenterlong.Name, CalculationMode.Price, Bars[0].Close / 1.11);
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
        [Category("Parameters")]
        public int TrendSize
        {
            get { return _trendSize; }
            set { _trendSize = Math.Max(1, value); }
        }

        #endregion
    }
}

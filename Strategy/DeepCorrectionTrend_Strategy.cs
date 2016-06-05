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
        private DeepCorrectionTrend_Indikator _DeepCorrectionTrend_Indikator = new DeepCorrectionTrend_Indikator();

        #endregion

        protected override void Initialize()
        {
            CalculateOnBarClose = true;
            BarsRequired = 20;
        }

        protected override void OnBarUpdate()
        {
              //for debugging reason only, just to get a hook in
            if (CurrentBar + 1 == 211)
            {
                int a = 1 + 1;
            }
            
            //Lets call the calculate method and save the result with the trade action
            ResultValue_DeepCorrection ResultValue = this._DeepCorrectionTrend_Indikator.calculate(Close, TrendSize);

            //Entry
            if (ResultValue.Entry.HasValue)
            {
                switch (ResultValue.Entry)
                {
                    case OrderAction.Buy:
                        this.DoEnterLong(ResultValue.StopLoss, ResultValue.Target);
                        break;
                    case OrderAction.SellShort:
                        this.DoEnterShort(ResultValue.StopLoss, ResultValue.Target);
                        break;
                }
            }

        }

        /// <summary>
        /// Create Long Order and Stop.
        /// </summary>
        private void DoEnterLong(double StopLoss, double target)
        {
            if (_orderenterlong == null)
            {
                _orderenterlong = EnterLong(GlobalUtilities.AdjustPositionToRiskManagement(this.Root.Core.AccountManager, this.Root.Core.PreferenceManager, this.Instrument, Bars[0].Close), this.GetType().Name + " " + PositionType.Long + "_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(), this.Instrument, this.TimeFrame);
                SetStopLoss(_orderenterlong.Name, CalculationMode.Price, StopLoss, false);
                SetProfitTarget(_orderenterlong.Name, CalculationMode.Price, target);
            }
        }

        /// <summary>
        /// Create Short Order and Stop.
        /// </summary>
        private void DoEnterShort(double StopLoss, double target)
        {
            if (_orderentershort == null)
            {
                _orderentershort = EnterShort(GlobalUtilities.AdjustPositionToRiskManagement(this.Root.Core.AccountManager, this.Root.Core.PreferenceManager, this.Instrument, Bars[0].Close), this.GetType().Name + " " + PositionType.Short + "_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(), this.Instrument, this.TimeFrame);
                SetStopLoss(_orderentershort.Name, CalculationMode.Price, StopLoss, false);
                SetProfitTarget(_orderentershort.Name, CalculationMode.Price, target);
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

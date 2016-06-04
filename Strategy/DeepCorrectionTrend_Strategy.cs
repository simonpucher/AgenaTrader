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

        #endregion

        protected override void Initialize()
        {
            CalculateOnBarClose = true;
           // BarsRequired = 20;
        }

        protected override void OnBarUpdate()
        {
            if (MarketPhasesAdv(_trendSize)[0] == MarketPhaseDeepCorrectionLong)
            {
                DoEnterLong();
            }
            else if (MarketPhasesAdv(_trendSize)[0] == MarketPhaseDeepCorrectionShort)
            {
                DoEnterShort();
            }
        }

        /// <summary>
        /// Create Long Order and Stop.
        /// </summary>
        private void DoEnterLong()
        {
            _orderenterlong = EnterLong(GlobalUtilities.AdjustPositionToRiskManagement(this.Root.Core.AccountManager, this.Root.Core.PreferenceManager, this.Instrument, Bars[0].Close), this.GetType().Name + " " + PositionType.Long + "_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(), this.Instrument, this.TimeFrame);
            SetStopLoss(_orderenterlong.Name, CalculationMode.Price, P123(_trendSize).TempP3Price[0], false);
            SetProfitTarget(_orderenterlong.Name, CalculationMode.Price, P123(_trendSize).P2Price[0]);
        }

        /// <summary>
        /// Create Short Order and Stop.
        /// </summary>
        private void DoEnterShort()
        {
            _orderentershort = EnterShort(GlobalUtilities.AdjustPositionToRiskManagement(this.Root.Core.AccountManager, this.Root.Core.PreferenceManager, this.Instrument, Bars[0].Close), this.GetType().Name + " " + PositionType.Short + "_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(), this.Instrument, this.TimeFrame);
            SetStopLoss(_orderentershort.Name, CalculationMode.Price, P123(_trendSize).TempP3Price[0], false);
            SetProfitTarget(_orderentershort.Name, CalculationMode.Price, P123(_trendSize).P2Price[0]);
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

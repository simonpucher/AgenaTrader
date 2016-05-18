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
using AgenaTrader.Helper.TradingManager;

namespace AgenaTrader.UserCode
{
    [Description("Strategie zu Pop Gun Bar Pattern")]
    public class PopGun_Strategy : UserStrategy
    {

        #region Variables
        double PopGun_Indicator_Value;

        //input
        private int _PopGunExpires = 5;
        private bool _issnapshotactive = false;
        private bool _isevaluationactive = false;
        private bool _fullIndicatorScan = false;
        private bool _setStopLoss = false;
        private bool _setTrailingStop = false;
        private bool _filter_NoShortRSI = false;
        private bool _filter_NoLongRSI = false;

        private IOrder oEnter;
        private IOrder oStop;

        string SignalNameEnter;
        string SignalNameStop;

        //internal
        private PopGun_Indicator _popgun_indicator = null;
        private IOrder _orderenterlong;
        private IOrder _orderentershort;

        #endregion

        protected override void OnStartUp()
        {
            base.OnStartUp();

            //Init our indicator to get code access
            this._popgun_indicator = new PopGun_Indicator();
            this._popgun_indicator.SetData(this.PopGunExpires, this.IsSnapshotActive, this.IsEvaluationActive);
            this._popgun_indicator.SetTimeFrame(this.TimeFrame);
        }

        protected override void Initialize()
        {
            CalculateOnBarClose = true;
            BarsRequired = 3;
        }

        protected override void OnBarUpdate()
        {


            if ((this._orderenterlong != null || this._orderentershort != null)
               && Bars[0].Time >= this._popgun_indicator.PopGunTargetDateTime)
            {
                if (this._orderenterlong != null)
                {
                    ExitLong(this._orderenterlong.Quantity, "PopGunTarget", this._orderenterlong.Name, this._orderenterlong.Instrument, this._orderenterlong.TimeFrame);
                }
                if (this._orderentershort != null)
                {
                    ExitShort(this._orderentershort.Quantity, "PopGunTarget", this._orderentershort.Name, this._orderentershort.Instrument, this._orderentershort.TimeFrame);
                }
            }

            if (_orderenterlong != null || _orderentershort != null)
            {
                return;
            }

            calculate();
        }



        protected override void OnExecution(IExecution execution)
        {
            if (execution.MarketPosition == PositionType.Flat)
            {
                _orderentershort = null;
                _orderenterlong = null;
            }
        }

        private void calculate()
        {
            double PopGun_Indicator_Value = this._popgun_indicator.calculate(this.Bars, this.CurrentBar);

            if (!IsCurrentBarLast || oEnter != null) return;

            this._popgun_indicator.calculate(this.Bars, this.CurrentBar);

            if (PopGun_Indicator_Value == 100)
            {

                //no LongPosition is RSI threshold is above 70
                if (Filter_NoLongRSI == true
                 && RSI(14, 3)[0] > 70)
                {
                    return;
                }

                DoEnterLong();
            }
            else if (PopGun_Indicator_Value == -100)
            {
                //no ShortPosition is RSI threshold is below 30
                if (Filter_NoShortRSI == true
                 && RSI(14, 3)[0] < 30)
                {
                    return;
                }
                DoEnterShort();
            }
            else
            {
                //nothing to do
            }
        }

        private void DoEnterLong()
        {
            _orderenterlong = EnterLong(GlobalUtilities.AdjustPositionToRiskManagement(this.Root.Core.AccountManager,
                                                                                       this.Root.Core.PreferenceManager,
                                                                                       this.Instrument, Bars[0].Close),
                                                                                       "PopGun_Long_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(),
                                                                                       this.Instrument, this.TimeFrame);
            if (UseStopLoss)
            {
                SetStopLoss(_orderenterlong.Name, CalculationMode.Price, this._popgun_indicator.PopGunTriggerShort, false);
            }

            if (UseTrailingStop)
            {
                SetTrailStop(_orderenterlong.Name, CalculationMode.Ticks, 10, false);
            }

            //SetProfitTarget(_orderenterlong.Name, CalculationMode.Price, <<<<TARGET>>>>);
        }

        private void DoEnterShort()
        {
            _orderentershort = EnterShort(GlobalUtilities.AdjustPositionToRiskManagement(this.Root.Core.AccountManager,
                                                                                         this.Root.Core.PreferenceManager,
                                                                                         this.Instrument, Bars[0].Close),
                                                                                         "PopGun_Short_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(),
                                                                                         this.Instrument, this.TimeFrame);
            if (UseStopLoss)
            {
                SetStopLoss(_orderentershort.Name, CalculationMode.Price, this._popgun_indicator.PopGunTriggerLong, false);
            }

            if (UseTrailingStop)
            {
                SetTrailStop(_orderenterlong.Name, CalculationMode.Ticks, 10, false);
            }
            //SetProfitTarget(_orderentershort.Name, CalculationMode.Price, <<<<TARGET>>>>);
        }

        public override string ToString()
        {
            return "PopGun";
        }

        public override string DisplayName
        {
            get
            {
                return "PopGun";
            }
        }

        #region Properties

        [Description("Wieviel Bars ist PopGunTrigger gültig?")]
        [Category("Parameters")]
        [DisplayName("PopGunExpires")]
        public int PopGunExpires
        {
            get { return _PopGunExpires; }
            set { _PopGunExpires = value; }
        }
        [Description("Creates snapshots on signals")]
        [Category("Parameters")]
        [DisplayName("Snapshot is active")]
        public bool IsSnapshotActive
        {
            get { return _issnapshotactive; }
            set { _issnapshotactive = value; }
        }

        [Description("Creates evalation (P/L) on signals")]
        [Category("Parameters")]
        [DisplayName("Evalation is active")]
        public bool IsEvaluationActive
        {
            get { return _isevaluationactive; }
            set { _isevaluationactive = value; }
        }

        [Description("Set StopLoss on opposite Trigger Range")]
        [Category("Parameters")]
        [DisplayName("Set StopLoss")]
        public bool UseStopLoss
        {
            get { return _setStopLoss; }
            set { _setStopLoss = value; }
        }
        [Description("Set TrailingStop")]
        [Category("Parameters")]
        [DisplayName("Set TrailingStop")]
        public bool UseTrailingStop
        {
            get { return _setTrailingStop; }
            set { _setTrailingStop = value; }
        }


        [Description("No Long Trades when RSI > 70")]
        [Category("TradeFilter")]
        [DisplayName("No Long Trades when RSI > 70")]
        public bool Filter_NoLongRSI
        {
            get { return _filter_NoLongRSI; }
            set { _filter_NoLongRSI = value; }
        }

        [Description("No Short Trades when RSI < 30")]
        [Category("TradeFilter")]
        [DisplayName("No Short Trades when RSI < 30")]
        public bool Filter_NoShortRSI
        {
            get { return _filter_NoShortRSI; }
            set { _filter_NoShortRSI = value; }
        }


        #endregion

    }
}

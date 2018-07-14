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

        protected override void OnInit()
        {
            Add(new OutputDescriptor(Color.Brown, OutputSerieDrawStyle.Block, "DeepCorrection"));
            Add(new OutputDescriptor(Color.Green, "Entry"));
            IsOverlay = false;
            CalculateOnClosedBar = true;
           // RequiredBarsCount = 20;
        }

        protected override void OnCalculate()
        {

            //for debugging reason only, just to get a hook in
            if (ProcessingBarIndex + 1 == 1051)
            {
                int a = 1 + 1;
            }

            if (ProcessingBarIndex < RequiredBarsCount)
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
                    case OrderDirection.Buy:
                        Outputs[1].Set(1);
                        break;
                    case OrderDirection.Sell:
                        //AddChartDiamond("ArrowShort_Entry" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.LightGreen);
                        Outputs[1].Set(-1);
                        break;
                }
            }
            else
            {
                //Value was null so nothing to do.
                Outputs[1].Set(0);
            }


        }


        public ResultValue_DeepCorrection calculate(IDataSeries InSeries, int TrendSize, IBar myBar, string caller)
        {


            if (FirstCalculate == false)
            {
                Print("FirstCalculate " + myBar.Time + caller);
                FirstCalculate = true;
            }

            //Create a return object
            ResultValue_DeepCorrection ResultValue = new ResultValue_DeepCorrection();

            if (MarketPhasesAdv(InSeries, TrendSize)[0] == MarketPhaseDeepCorrectionLong
             || MarketPhasesAdv(InSeries, TrendSize)[0] == MarketPhaseDeepCorrectionWithTrendLong)
            {
                //Print(Bars[0].Time + " " + MarketPhasesAdv(InSeries, TrendSize)[0] + " tmp Pkt3 " + P123(Close, _trendSize).TempP3Price[0] + " valid Pkt3 " + P123(InSeries, _trendSize).ValidP3Price[0]);

                //try to get the temporary P3, if it does not exist, take the already validated P3
                if (P123(InSeries, _trendSize).TempP3Price[0] != 0)
                {
                    ResultValue.DeepCorrection = true;
                    ResultValue.Entry = OrderDirection.Buy;
                    ResultValue.StopLoss = P123(InSeries, _trendSize).TempP3Price[0];
                    
                }
                else if (P123(InSeries, _trendSize).ValidP3Price[0] != 0)
                {
                    ResultValue.DeepCorrection = true;
                    ResultValue.Entry = OrderDirection.Buy;
                    ResultValue.StopLoss = P123(InSeries, _trendSize).ValidP3Price[0];
                }

                //Check, if current Price is lower than P2 (because we want to go long towards P2)
                if (InSeries[0] < P123(InSeries, _trendSize).P2Price[0])
                {
                    ResultValue.Target = P123(InSeries, _trendSize).P2Price[0];
                    Print("Indikator" + myBar.Time + " Long " + "Close: " + myBar.Close + " StopLoss: " + ResultValue.StopLoss + " Target: " + ResultValue.Target + " Marktphase: " + MarketPhasesAdv(InSeries, TrendSize)[0] + " BarsCount: " + InSeries.Count);
                }
                else
                {
                    ResultValue.Target = InSeries[0] * 1.005;
                    Print("Indikator" + myBar.Time + " Long " + "Close: " + myBar.Close + " StopLoss: " + ResultValue.StopLoss + " Target: " + ResultValue.Target + " Marktphase: " + MarketPhasesAdv(InSeries, TrendSize)[0] + " BarsCount: " + InSeries.Count);
                }
            }
            else if (MarketPhasesAdv(InSeries, TrendSize)[0] == MarketPhaseDeepCorrectionShort
                  || MarketPhasesAdv(InSeries, TrendSize)[0] == MarketPhaseDeepCorrectionWithTrendShort)
            {

                //try to get the temporary P3, if it does not exist, take the already validated P3
                if (P123(InSeries, _trendSize).TempP3Price[0] != 0)
                {
                    ResultValue.DeepCorrection = true;
                    ResultValue.Entry = OrderDirection.Sell;
                    ResultValue.StopLoss = P123(InSeries, _trendSize).TempP3Price[0];
                    ResultValue.Target = P123(InSeries, _trendSize).P2Price[0];
                }
                else if (P123(InSeries, _trendSize).ValidP3Price[0] != 0)
                {
                    ResultValue.DeepCorrection = true;
                    ResultValue.Entry = OrderDirection.Sell;
                    ResultValue.StopLoss = P123(InSeries, _trendSize).ValidP3Price[0];
                }
                //Check, if current Price is higher than P2 (because we want to go short towards P2)
                    if (InSeries[0] > P123(InSeries, _trendSize).P2Price[0])
                {
                    ResultValue.Target = P123(InSeries, _trendSize).P2Price[0];
                }
                else
                {
                    ResultValue.Target = InSeries[0] / 1.005;
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
            get { return Outputs[0]; }
        }


        /// <summary>
        /// </summary>
        [Description("Trendsize (0-3)")]
        [InputParameter]
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
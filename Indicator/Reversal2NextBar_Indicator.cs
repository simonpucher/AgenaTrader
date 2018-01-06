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
/// todo description
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Umkehrstab für nächste Periode")]
    public class Reversal2NextBar_Indicator : UserIndicator
    {
        DateTime ReversalTradeStartTSShort;
        DateTime ReversalTradeStartTSLong;
        DateTime TargetBarTime;
        decimal ReversalTradeResult;
        decimal ReversalTradeResultTotalLong;
        decimal ReversalTradeResultTotalShort;
        double TradeCounter;
        double TradeCounterLongWin;
        double TradeCounterLongFail;
        double TradeCounterShortWin;
        double TradeCounterShortFail;
        private Color colWin = Color.Yellow;
        private Color colFail = Color.Brown;

        protected override void OnInit()
        {
            Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Orange), "Reversal2NextBar"));
            IsOverlay = true;
            CalculateOnClosedBar = true;
        }

        protected override void OnCalculate()
        {

            string strReversalTradeLong = "ReversalTradeLong" + ProcessingBarIndex;
            string strReversalTradeShort = "ReversalTradeShort" + ProcessingBarIndex;
            string strTradeResultLong;
            string strTradeResultShort;
            Color colorTextBox;

            // 1 umkehr fallend auf steigend
            //-1 umkehr steigend auf fallend

            if (IsReversalLongTrade() == true)
            {
                ReversalTradeStartTSLong = Bars[0].Time;
                //TargetBarTime = GetTargetBar(Bars[0].Time);
                TargetBarTime = GlobalUtilities.GetTargetBar(Bars, Bars[0].Time, TimeFrame, 1);
                OutSeries.Set(100);
                Reversal2NextBar.Set(100);
            }
            else if (IsReversalShortTrade() == true)
            {

                ReversalTradeStartTSShort = Bars[0].Time;
                //TargetBarTime = GetTargetBar(Bars[0].Time);
                TargetBarTime = GlobalUtilities.GetTargetBar(Bars, Bars[0].Time, TimeFrame, 1);
                OutSeries.Set(-100);

            }
            else
            {
                Reversal2NextBar.Set(0);
            }

            //TradingKerze ist fertig, Zeiteinheit ist abgelaufen
            if (Bars[0].Time == TargetBarTime)
            {


                ReversalTradeResult = (decimal)Bars.GetClose(ProcessingBarIndex) - (decimal)Bars.GetOpen(ProcessingBarIndex);
                TradeCounter += 1;

                if (ReversalTradeStartTSLong > DateTime.MinValue)
                {

                    ReversalTradeResultTotalLong = ReversalTradeResultTotalLong + ReversalTradeResult;
                    if (ReversalTradeResult < 0)
                    {
                        strTradeResultLong = "Fail " + ReversalTradeResult.ToString();
                        colorTextBox = colFail;
                        TradeCounterLongFail += 1;
                    }
                    else
                    {
                        strTradeResultLong = "Win " + ReversalTradeResult.ToString();
                        colorTextBox = colWin;
                        TradeCounterLongWin += 1;
                    }
                    AddChartText(strReversalTradeLong, true, strTradeResultLong, Time[1], Bars.GetHigh(ProcessingBarIndex) + (100 * TickSize), 9, Color.Black, new Font("Arial", 9), StringAlignment.Center, Color.Black, colorTextBox, 70);
                }
                else if (ReversalTradeStartTSShort > DateTime.MinValue)
                {
                    ReversalTradeResultTotalShort = ReversalTradeResultTotalShort + ReversalTradeResult;
                    if (ReversalTradeResult < 0)
                    {
                        strTradeResultShort = "Win " + ReversalTradeResult.ToString();
                        colorTextBox = colWin;
                        TradeCounterShortWin += 1;
                    }
                    else
                    {
                        strTradeResultShort = "Fail " + ReversalTradeResult.ToString();
                        colorTextBox = colFail;
                        TradeCounterShortFail += 1;
                    }
                    AddChartText(strReversalTradeShort, true, strTradeResultShort, Time[1], Bars.GetHigh(ProcessingBarIndex) - (100 * TickSize), 9, Color.Black, new Font("Arial", 9), StringAlignment.Center, Color.Black, colorTextBox, 70);
                }

                //Variablen Resetten
                ReversalTradeStartTSLong = DateTime.MinValue;
                ReversalTradeStartTSShort = DateTime.MinValue;
            }

            if (IsProcessingBarIndexLast)
            {
                //       Print("LongWin: " + TradeCounterLongWin + " LongFail: " + TradeCounterLongFail + " ShortWin: " + TradeCounterShortWin + " ShortFail: " + TradeCounterShortFail);
                //       Print(Instrument.Name + "Trades: " + TradeCounter + " LongPunkte: " + ReversalTradeResultTotalLong + " ShortPunkte: " + ReversalTradeResultTotalShort);
            }

        }



        public double getReversalStop()
        {
            if (IsReversalLongTrade() == true)
            {
                return Bars[0].Low;
            }

            if (IsReversalShortTrade() == true)
            {
                return Bars[0].High;
            }

            return Bars[0].Close;


        }

        //public DateTime GetTargetBar(DateTime CurrentBarDateTime)
        //{
        //    switch (TimeFrame.Periodicity)
        //    {
        //        case DatafeedHistoryPeriodicity.Minute:
        //            switch (TimeFrame.PeriodicityValue)
        //            {
        //                case 1:
        //                    return CurrentBarDateTime.AddMinutes(1);
        //                //break;
        //                case 5:
        //                    return CurrentBarDateTime.AddMinutes(5);
        //                //break;
        //                case 15:
        //                    return CurrentBarDateTime.AddMinutes(15);
        //                //break;
        //                case 30:
        //                    return CurrentBarDateTime.AddMinutes(30);
        //                //break;
        //            }
        //            break;
        //        case DatafeedHistoryPeriodicity.Hour:
        //            switch (TimeFrame.PeriodicityValue)
        //            {
        //                case 1:
        //                    return CurrentBarDateTime.AddHours(1);
        //                //break;
        //                case 5:
        //                    return CurrentBarDateTime.AddHours(4);
        //                //break;
        //            }
        //            break;

        //        case DatafeedHistoryPeriodicity.Day:
        //            return CurrentBarDateTime.AddDays(1);
        //        //                  break;

        //        case DatafeedHistoryPeriodicity.Week:
        //            return CurrentBarDateTime.AddDays(7);
        //        //                break;



        //        default:
        //            return DateTime.MinValue;
        //    }
        //    return DateTime.MinValue;
        //}

        private bool IsReversalLongTrade()
        {
            bool result = false;
            if (ReversalBars()[0] == 1) //Fallend auf steigend
            {

                //double BarSize = Bars[0].Low - Bars[0].High;
                //double BarBodySize = Bars[0].Open - Bars[0].Close;
                //if (BarBodySize == 0)
                //{
                //    BarBodySize = 0.0001;
                //}

                //double HigherBarRange = BarSize * 0.2;
                //double Threshold = Bars[0].High - HigherBarRange;

                //if (Threshold > Bars[0].Close)

                //if ((BarSize / BarBodySize) < 20 )

                //if (Bars[1] != null && Bars[2] != null)
                //{
                //    if (Bars[1].IsFalling == true && Bars[2].IsFalling == true)
                //    //                IF (LinReg(Closes[0], 2)[0] > Bars[0].Close ){
                //    {
                //        result = true;
                //    }
                //}

                //double BarSize = Bars[0].High - Bars[0].Low;
                //double LunteSize = Bars[0].Close - Bars[0].Low;
                ////decimal LunteMax = BarSize * (decimal)0.07;
                //decimal LunteMax = Decimal.Multiply((decimal)BarSize, (decimal)0.07);

                //if ((decimal)LunteSize < LunteMax){
                //    result = true;
                //}

            }
            return result;
        }


        private bool IsReversalShortTrade()
        {
            bool result = false;
            if (ReversalBars()[0] == -1)
            {
                //if (Bars[1] != null && Bars[2] != null)
                //{
                //    if (Bars[1].IsGrowing == true && Bars[2].IsGrowing == true)
                //    {
                //        //            && LinReg(Closes[0], 2)[0] < Bars[0].Close)
                //        //             result = true;

                //    }
                //}

                double BarSize = Bars[0].High - Bars[0].Low;
                double LunteSize = Bars[0].Close - Bars[0].Low;
                decimal LunteMax = Decimal.Multiply((decimal)BarSize, (decimal)0.07);

                if ((decimal)LunteSize < LunteMax){
                    result = true;
                }

            }
            return result;
        }


        #region Properties

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Reversal2NextBar
        {
            get { return Outputs[0]; }
        }

        #endregion

    }
}
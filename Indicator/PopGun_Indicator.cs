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
/// Simon Pucher 2016
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

    public enum PopGunType
    {
        Classic = 1,
        ThreeBarReversal = 2
    }

    [Description("PopGun Bar Pattern")]
    public class PopGun_Indicator : UserIndicator
    {
        //Definition Outside Bar (not to be confused with the Outside Bar Markttechnik Defition based on Michael Voigt)
        // ->An outside bar occurs when the range of a bar encompasses the previous bar
        //http://www.debeurs.nl/Forum/Upload/2015/8690735.pdf

        //Definition Inside Bar (not to be confused with the Inside Bar Markttechnik Defition based on Michael Voigt)
        // ->An inside bar is a price bar whose range is encompessed by the previous bar
        //http://www.debeurs.nl/Forum/Upload/2015/8690735.pdf

        // 1) Determine high and low of price which was 2 bars ago
        // 2) Determine, if 1 Bar ago, there was an inside bar
        // 3) Determine, if current Bar is an outside bar

        //Three Bars Reversal
        //http://tradingsim.com/blog/day-trading-the-three-bar-reversal-pattern/
        //http://www.mypivots.com/dictionary/definition/208/3-bar-reversal-pattern-3br

        //input
        private int _PopGunExpires = 5;
        private bool _issnapshotactive = false;
        private bool _isevaluationactive = false;
        private PopGunType _PopGunType = PopGunType.ThreeBarReversal;
        private bool _drawlinesonchart = true;

        //output
        private double _popGunTriggerLong;
        private double _popGunTriggerShort;
        private DateTime _popGunTargetDateTime;

        //internal
        bool IsPopGun;
        int PopGunTarget;
        int PopGunTriggerBar;

        //Evaluation
        decimal PopGunTradeCounterShort;
        decimal PopGunTradeCounterLong;
        decimal PopGunTradeResultTotalShort;
        decimal PopGunTradeResultTotalLong;
        decimal PopGunTradeFailCounterShort;
        decimal PopGunTradeFailCounterLong;
        decimal PopGunTradeWinCounterShort;
        decimal PopGunTradeWinCounterLong;

        /// <summary>
        /// If we use this indicator from another script we need to initalize all important data first.
        /// </summary>
        public void SetData(int popgunexpires, bool isSnapShotActive, bool isEvaluationActive, bool filterNoTriggerEOD)
        {
            this.PopGunExpires = popgunexpires;
            this.IsSnapshotActive = isSnapShotActive;
            this.IsEvaluationActive = isEvaluationActive;
            this.Filter_NoTriggerEOD = filterNoTriggerEOD;            
        }

        public void SetTimeFrame(ITimeFrame timeFrame){
            this.TimeFrame = timeFrame;
        }

        protected override void OnInit()
        {
            Add(new OutputDescriptor(Color.Orange, "PopGun"));
            Add(new OutputDescriptor(Color.Green, OutputSerieDrawStyle.Block, "PopGunTrigger"));
            IsOverlay = false; //underneath the price chart in his own subchart
            IsAddDrawingsToPricePanel = true;
            CalculateOnClosedBar = true;
            RequiredBarsCount = 3;
        }

        protected override void OnCalculate()
        {
            int returnvalue = calculate(Bars, ProcessingBarIndex, this.PopGunType);
            OutSeries.Set(returnvalue);
            if (ProcessingBarIndex <= this.PopGunTarget)
            {
                Outputs[1].Set(0); //Indicates that there is a PopGun Trigger!      
            }
        }

        //todo -100 wird noch nicht zur�ckgegeben oder?
        public int calculate(IBars bars, int curbar, PopGunType popguntype)
        {
            bool noPopGunTrigger = false;
            int returnvalue = 0;

            //We need at least three bars
            if (curbar < 2) return 0;

            double TwoBarsAgo_High = bars[2].High;
            double TwoBarsAgo_Low = bars[2].Low;

            double OneBarAgo_High = bars[1].High;
            double OneBarAgo_Low = bars[1].Low;

            double CurrentBar_High = bars[0].High;
            double CurrentBar_Low = bars[0].Low;

            if (this.PopGunType == UserCode.PopGunType.Classic)
            {
                // 2) Determine, if 1 Bar ago, there was an inside bar
                if (TwoBarsAgo_High > OneBarAgo_High
                && TwoBarsAgo_Low < OneBarAgo_Low)
                {
                    //One Bar ago was an inside bar, so lets check if current bar is outside bar
                    if (TwoBarsAgo_High < CurrentBar_High
                    && TwoBarsAgo_Low > CurrentBar_Low)
                    {

                        // current bar is outside bar -> lets pop the gun
                        this.PopGunTargetDateTime = GlobalUtilities.GetTargetBar(bars, bars[0].Time, TimeFrame, PopGunExpires);

                        //check, if target bar would be on the following day, and therefor a risks of gap is given
                        if (Filter_NoTriggerEOD
                         && PopGunTargetDateTime.Date > bars[0].Time.Date)
                        {
                            //reject current PopGun Trigger and reset TargetDateTime
                            PopGunTargetDateTime = DateTime.MinValue;
                            noPopGunTrigger = true;
                        }

                        if (noPopGunTrigger == false)
                        {
                            this.PopGunTarget = curbar + this.PopGunExpires;
                            PopGunTriggerBar = ProcessingBarIndex;
                            this.PopGunTriggerLong = CurrentBar_High;
                            this.PopGunTriggerShort = CurrentBar_Low;
                        }
                    }
                }

                if (curbar < this.PopGunTarget)
                {
                    if (bars[0].Close > this.PopGunTriggerLong)
                    {
                        returnvalue = 100;
                    }
                    else if (bars[0].Close < this.PopGunTriggerShort)
                    {
                        returnvalue = -100;
                    }
                }

                drawTarget(bars, curbar);
                evaluation(bars, curbar);
 
            }
            else if (this.PopGunType == UserCode.PopGunType.ThreeBarReversal) {
                if (bars[2].IsFalling && bars[1].IsFalling && TwoBarsAgo_Low > OneBarAgo_Low && bars[0].Close > TwoBarsAgo_High && bars[0].Close > OneBarAgo_High)
                {
                    returnvalue = 100;
                    if (this.DrawLinesOnChart)
                    {
                        AddChartLine("high_" + bars[0].Time.Ticks.ToString(), true, bars[0].Time, bars[0].Close, bars[0].Time.AddDays(5), bars[0].Close, Color.Green, Const.DefaultIndicatorDashStyle, Const.DefaultLineWidth_large);

                        AddChartLine("low_" + bars[0].Time.Ticks.ToString(), true, bars[0].Time, bars[1].Close, bars[0].Time.AddDays(5), bars[1].Close, Color.Red, Const.DefaultIndicatorDashStyle, Const.DefaultLineWidth_large);
                        
                    }

                }
                else if (bars[2].IsGrowing && bars[1].IsGrowing && TwoBarsAgo_High < OneBarAgo_High && bars[0].Close < TwoBarsAgo_Low && bars[0].Close < OneBarAgo_Low)
                {
                    returnvalue = -100;

                    if (this.DrawLinesOnChart)
                    {
                        AddChartLine("high_" + bars[0].Time.Ticks.ToString(), true, bars[0].Time, bars[0].Close, bars[0].Time.AddDays(5), bars[0].Close, Color.Red, Const.DefaultIndicatorDashStyle, Const.DefaultLineWidth_large);

                        AddChartLine("low_" + bars[0].Time.Ticks.ToString(), true, bars[0].Time, bars[1].Close, bars[0].Time.AddDays(5), bars[1].Close, Color.Green, Const.DefaultIndicatorDashStyle, Const.DefaultLineWidth_large);

                    }
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            return returnvalue;
        }

        public void drawTarget(IBars bars, int curbar)
        {

            if (this.DrawLinesOnChart && curbar == PopGunTriggerBar)
            {
                AddChartText(("PopGunSize" + curbar), (Math.Round((((bars[0].High - bars[0].Low) / bars[0].Close) * 100),2)).ToString(), 0, bars.GetByIndex(PopGunTriggerBar).Low - TickSize*bars[0].Close, Color.Black);
            }

            if (curbar <= PopGunTarget
             && curbar > PopGunTriggerBar
             && curbar > 0)
            {
               

                DateTime lineend = GlobalUtilities.GetTargetBar(bars, bars.GetByIndex(PopGunTriggerBar).Time, TimeFrame, PopGunExpires);

                if (this.DrawLinesOnChart)
                {
                    string strPopGunLong = "PopGunLong" + curbar;
                    string strPopGunShort = "PopGunShort" + curbar;

                    AddChartLine(strPopGunLong, true, bars.GetByIndex(PopGunTriggerBar).Time, PopGunTriggerLong, lineend, PopGunTriggerLong,
                                                            Color.Green, Const.DefaultIndicatorDashStyle, Const.DefaultLineWidth_large);

                    AddChartLine(strPopGunShort, true, bars.GetByIndex(PopGunTriggerBar).Time, PopGunTriggerShort, lineend, PopGunTriggerShort,
                                                            Color.Red, Const.DefaultIndicatorDashStyle, Const.DefaultLineWidth_large);
                }


                if (this.IsSnapshotActive && curbar == PopGunTarget)
                {
                    GlobalUtilities.SaveSnapShot("PopGun", bars.Instrument.Name, this.Root.Core.ChartManager.AllCharts, bars, TimeFrame);
                }
            }
        }

        private void evaluation(IBars bars, int curbar)
        {
            if (IsEvaluationActive == false) return;
            if (curbar != PopGunTarget
                || curbar == 0) return;

            
            bool LongTrade = false;
            double PunkteLongTrade;
            bool ShortTrade = false;
            double PunkteShortTrade;
            int i;

            Statistic statistic = new Statistic("PopGun");

            #region LongAuswertung
            i = PopGunExpires - 1; //weil 0-Index
            do
            {
                if (bars[i].Close > PopGunTriggerLong)
                {
                    LongTrade = true;
                    statistic.EntryDateTime = bars[i].Time;
                    statistic.TradeDirection = PositionType.Long;
                    statistic.EntryPrice = PopGunTriggerLong;
                    statistic.StopPrice = PopGunTriggerShort;
                    break;
                }

                i--;
            } while (i >= 1);
            #endregion

            #region ShortAuswertung
            i = PopGunExpires - 1; 
            do
            {
                if (bars[i].Close < PopGunTriggerShort
                   && ShortTrade == false)
                {
                    ShortTrade = true;
                    statistic.EntryDateTime = bars[i].Time;
                    statistic.EntryPrice = PopGunTriggerShort;
                    statistic.StopPrice = PopGunTriggerLong;
                    statistic.TradeDirection = PositionType.Short;
                    break;
                }

                i--;
            } while (i >= 1);
            #endregion


            if (LongTrade == true && ShortTrade == true)
            {
           //     DoubleBreakOut = true;
            }
            else if (LongTrade == true)
            {
                PunkteLongTrade = bars[0].Close - PopGunTriggerLong;
                if (PunkteLongTrade > 0)
                {
                    PopGunTradeWinCounterLong += 1;
                }
                else
                {
                    PopGunTradeFailCounterLong += 1;
                }
                PopGunTradeCounterLong += 1;
                PopGunTradeResultTotalLong = PopGunTradeResultTotalLong + (decimal)PunkteLongTrade;
            }
            else if (ShortTrade == true)
            {
                PunkteShortTrade = bars[0].Close - PopGunTriggerShort;
                if (PunkteShortTrade < 0)
                {
                    PopGunTradeWinCounterShort += 1;
                }
                else
                {
                    PopGunTradeFailCounterShort += 1;
                }
                PopGunTradeCounterShort += 1;
                PopGunTradeResultTotalShort = PopGunTradeResultTotalShort + (decimal)PunkteShortTrade;
            }


            if (statistic.EntryDateTime > DateTime.MinValue)
            {
                statistic.TimeFrame = TimeFrame.PeriodicityValue.ToString() + TimeFrame.Periodicity.ToString();
                statistic.ExitDateTime = GlobalUtilities.GetTargetBar(bars, bars[0].Time, TimeFrame, 1);
                statistic.Instrument = bars.Instrument.Symbol.ToString();
                statistic.ExitPrice = bars[0].Close;
                statistic.ExitReason = "Expired (" + _PopGunExpires + " Bars)";
                
            //    Print(statistic.getCSVData());
                statistic.AppendToFile();
            }
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


        [Description("Draw Lines on chart")]
        [Category("Drawing")]
        [DisplayName("Draw lines")]
        public bool DrawLinesOnChart
        {
            get { return _drawlinesonchart; }
            set { _drawlinesonchart = value; }
        }

        [Description("Type of PopGun Pattern you would like to use.")]
        [Category("Parameters")]
        [DisplayName("Pop Gun Type")]
        public PopGunType PopGunType
        {
            get { return _PopGunType; }
            set { _PopGunType = value; }
        }

        [Description("Wieviel Bars ist PopGunTrigger g�ltig?")]
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

        private bool _filter_NoTriggerEOD = false;

        [Description("No PopGun is triggered, if the expire date is targeted for the following day")]
        [Category("TradeFilter")]
        [DisplayName("No Trigger before EOD")]
        public bool Filter_NoTriggerEOD
        {
            get { return _filter_NoTriggerEOD; }
            set { _filter_NoTriggerEOD = value; }
        }


        #endregion

        #region Output
        [Browsable(false)]
        [XmlIgnore()]
        public double PopGunTriggerLong
        {
            get { return _popGunTriggerLong; }
            set { _popGunTriggerLong = value; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public double PopGunTriggerShort
        {
            get { return _popGunTriggerShort; }
            set { _popGunTriggerShort = value; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public DateTime PopGunTargetDateTime
        {
            get { return _popGunTargetDateTime; }
            set { _popGunTargetDateTime = value; }
        }


        #endregion
    }
}
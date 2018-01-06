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
    [Description("Handelsautomatik für ShowGap")]
    public class ShowGap_Strategie : UserStrategy
    {

        #region Variables
        decimal _PunkteGapMin = 50;
        decimal _PunkteGapMax = 100;
        bool _testlauf = false;
        double ShowGap_Indicator_Value;

        private IOrder oEnter;
        private IOrder oStop;

        string SignalNameEnter;
        string SignalNameStop;
        #endregion

        protected override void OnInit()
        {
            CalculateOnClosedBar = false;
            IsAutoConfirmOrder = false;
        }

        protected override void OnCalculate()
        {

            string ocoId;
            double StopForShowGapTrade;
            DateTime ts_Einstieg;
            

            if (_testlauf == false)
            {
                ts_Einstieg = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 09, 15, 0);
            }
            else
            {
                ts_Einstieg = new DateTime(Bars[0].Time.Year, Bars[0].Time.Month, Bars[0].Time.Day,Bars[0].Time.Hour,Bars[0].Time.Minute, 0);
            }

            //todo Close before end of trading day - please check it!
            //if (this.oEnter != null)
            //{
            //    DateTime ts_Ausstieg;
            //    if (_testlauf == false)
            //    {
            //        ts_Ausstieg = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 09, 30, 0);
            //    }
            //    else
            //    {
            //        ts_Ausstieg = DateTime.Now.AddMinutes(1);
            //    }

            //    if (Bars[0].Timestamp >= ts_Ausstieg)
            //    {
            //        if (this.oEnter.Direction == OrderDirection.Buy)
            //        {
            //            CloseLongTrade(new StrategyOrderParameters {Type = OrderType.Market, Quantity = this.oEnter.Quantity, SignalName =  "EOD", FromEntrySignal =  this.oEnter.Name, Instrument =  this.oEnter.Instrument, TimeFrame =  this.oEnter.TimeFrame});
            //        }
            //        else if (this.oEnter.Direction == OrderDirection.Sell)
            //        {
            //            CloseShortTrade(new StrategyOrderParameters {Type = OrderType.Market, Quantity = this.oEnter.Quantity, SignalName =  "EOD", FromEntrySignal =  this.oEnter.Name, Instrument =  this.oEnter.Instrument, TimeFrame =  this.oEnter.TimeFrame});
            //        }
            //    }
            //}


            if (!IsProcessingBarIndexLast || oEnter != null)
            {
              return;
            }
            //Heute 09.15
            else if (Bars[0].Time == ts_Einstieg)
            {

                if (_testlauf == false)
                {
                    //ShowGap Indikator aufrufen. Dieser liefert 100 für Long Einstieg und -100 für Short Einstieg. Liefert 0 für kein Einstiegssignal
                    ShowGap_Indicator_Value = ShowGap_Indicator(PunkteGapMin, PunkteGapMax)[0];
                    StopForShowGapTrade = ShowGap_Indicator(PunkteGapMin, PunkteGapMax).StopForShowGapTrade;
                }
                else {
                    ShowGap_Indicator_Value = 100;
                    StopForShowGapTrade = (Bars[0].Close - 50 * TickSize);
                }
                if (ShowGap_Indicator_Value == 100)
                {
                    //Long          
                    SignalNameEnter = "ShowGapLong" + Bars[0].Time;
                    SignalNameStop = "ShowGapStop" + Bars[0].Time;
                    ocoId = "ShowGapLong_ocoID" + Bars[0].Time;
                    oEnter = SubmitOrder(0, OrderDirection.Buy, OrderType.Market, 3, 0, 0, ocoId, SignalNameEnter);
                    oStop = SubmitOrder(0, OrderDirection.Sell, OrderType.Stop, 3, 0, StopForShowGapTrade, ocoId, SignalNameStop);
                }
                else if (ShowGap_Indicator_Value == -100)
                {
                    //Short                
                    SignalNameEnter = "ShowGapShort" + Bars[0].Time;
                    SignalNameStop = "ShowGapStop" + Bars[0].Time;
                    ocoId = "ShowGapShort_ocoID" + Bars[0].Time;
                    oEnter = SubmitOrder(0, OrderDirection.Sell, OrderType.Market, 3, 0, 0, ocoId, SignalNameEnter);
                    oStop = SubmitOrder(0, OrderDirection.Buy, OrderType.Stop, 3, 0, StopForShowGapTrade, ocoId, SignalNameStop);
                }
                else
                {
                    //keine Aktion     
                    return;
                }
                CreateIfDoneGroup(new List<IOrder> { oEnter, oStop });
                oEnter.ConfirmOrder();
            }
        }


        protected override void OnOrderExecution(IExecution execution)
        {

            ////todo this is not working in 1.9
            //DateTime ts_Ausstieg;
            //if (_testlauf == false)
            //{
            //    ts_Ausstieg = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 09, 30, 0);
            //}
            //else
            //{
            //    ts_Ausstieg = DateTime.Now.AddMinutes(1);
            //}
            //foreach (Trade item in this.Root.Core.TradingManager.ActiveOpenedTrades)
            //{
            //    if (item.EntryOrder.Name == SignalNameEnter
            //     || item.EntryOrder.Name == SignalNameStop)
            //    {
            //        item.Expiration = ts_Ausstieg;
            //    }
            //}



            if (execution.PositionType == PositionType.Flat) {
                oStop = null;    //den Stop zuerst
                oEnter = null;                    
            }
        }

        #region Properties
        [Description("Mind. Punkte für Gap")]
        [Category("Parameters")]
        [DisplayName("MinPunkte")]
        public decimal PunkteGapMin
        {
            get { return _PunkteGapMin; }
            set { _PunkteGapMin = value; }
        }

        [Description("Max. Punkte für Gap")]
        [Category("Parameters")]
        [DisplayName("MaxPunkte")]
        public decimal PunkteGapMax
        {
            get { return _PunkteGapMax; }
            set { _PunkteGapMax = value; }
        }

        [Description("Testlauf")]
        [Category("Parameters")]
        [DisplayName("Testlauf")]
        public bool Testlauf
        {
            get { return _testlauf; }
            set { _testlauf = value; }
        }
        #endregion
    }
}

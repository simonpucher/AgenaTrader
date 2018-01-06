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
	[Description("Kauft bei Umkehrstab und hält für eine Periode")]
	public class Reversal2NextBar_Strategy : UserStrategy
	{
        #region Variables
        bool _testlauf = false;
        double Reversal_Indicator_Value;

        private IOrder oEnter;
        private IOrder oStop;

        string SignalNameEnter;
        string SignalNameStop;
        #endregion



        protected override void OnInit()
        {
            CalculateOnClosedBar = true;
            IsAutoConfirmOrder = false;
        }

        protected override void OnCalculate()
        {
            string ocoId;
            double StopForReversalTrade;

            if (!IsProcessingBarIndexLast || oEnter != null)
            {
                return;
            }


            if (_testlauf == false)
            {
                //Reversal_Indicator_Value aufrufen. Dieser liefert 100 für Long Einstieg und -100 für Short Einstieg. Liefert 0 für kein Einstiegssignal
                Reversal_Indicator_Value = Reversal2NextBar_Indicator()[0];
                Print(Bars[0].Time + " RevIndVal: " + Reversal_Indicator_Value);
                StopForReversalTrade = Reversal2NextBar_Indicator().getReversalStop();
                
            }
            else
            {
                Reversal_Indicator_Value = 100;
                StopForReversalTrade = (Bars[ProcessingBarIndex].Close - 50 * TickSize);
            }


            if (Reversal_Indicator_Value == 100)
            {
                //Long          
                SignalNameEnter = "ReversalLong" + Bars[0].Time;
                SignalNameStop = "ReversalStop" + Bars[0].Time;
                ocoId = "ReversalLong_ocoID" + Bars[0].Time;
                oEnter = SubmitOrder(0, OrderDirection.Buy, OrderType.Market, 3, 0, 0, ocoId, SignalNameEnter);
                oStop = SubmitOrder(0, OrderDirection.Sell, OrderType.Stop, 3, 0, StopForReversalTrade, ocoId, SignalNameStop);
            }
            else if (Reversal_Indicator_Value == -100)
            {
                //Short                
                SignalNameEnter = "ReversalShort" + Bars[0].Time;
                SignalNameStop = "ReversalStop" + Bars[0].Time;
                ocoId = "ReversalShort_ocoID" + Bars[0].Time;
                oEnter = SubmitOrder(0, OrderDirection.Sell, OrderType.Market, 3, 0, 0, ocoId, SignalNameEnter);
                oStop = SubmitOrder(0, OrderDirection.Buy, OrderType.Stop, 3, 0, StopForReversalTrade, ocoId, SignalNameStop);
            }
            else
            {
                //keine Aktion     
                return;
            }

            CreateIfDoneGroup(new List<IOrder> { oEnter, oStop });
            oEnter.ConfirmOrder();
        }


        protected override void OnOrderExecution(IExecution execution)
        {
            DateTime ts_Ausstieg;

           // ts_Ausstieg = Reversal2NextBar_Indicator().GetTargetBar(Bars[-1].Timestamp);
            ts_Ausstieg = GlobalUtilities.GetTargetBar(Bars, Bars[0].Time, TimeFrame, 1);
            Print("Ausstieg: " + ts_Ausstieg + "Bars[-1].Time: " + Bars[-1].Time);

            ////todo this is not working in 1.9
            //foreach (Trade item in this.Root.Core.TradingManager.ActiveOpenedTrades)
            //{
            //    if (item.EntryOrder.Name == SignalNameEnter
            //     || item.EntryOrder.Name == SignalNameStop)
            //    {
            //        item.Expiration = ts_Ausstieg;
            //    }
            //}


            if (execution.PositionType == PositionType.Flat)
            {
                oStop = null;    //den Stop zuerst
                oEnter = null;
            }
        }

        #region Properties
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

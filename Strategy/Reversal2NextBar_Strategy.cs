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



        protected override void Initialize()
        {
            CalculateOnBarClose = true;
            IsAutomated = false;
        }

        protected override void OnBarUpdate()
        {
            string ocoId;
            double StopForReversalTrade;

            if (!IsCurrentBarLast || oEnter != null)
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
                StopForReversalTrade = (Bars[CurrentBar].Close - 50 * TickSize);
            }


            if (Reversal_Indicator_Value == 100)
            {
                //Long          
                SignalNameEnter = "ReversalLong" + Bars[0].Time;
                SignalNameStop = "ReversalStop" + Bars[0].Time;
                ocoId = "ReversalLong_ocoID" + Bars[0].Time;
                oEnter = SubmitOrder(0, OrderAction.Buy, OrderType.Market, 3, 0, 0, ocoId, SignalNameEnter);
                oStop = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, 3, 0, StopForReversalTrade, ocoId, SignalNameStop);
            }
            else if (Reversal_Indicator_Value == -100)
            {
                //Short                
                SignalNameEnter = "ReversalShort" + Bars[0].Time;
                SignalNameStop = "ReversalStop" + Bars[0].Time;
                ocoId = "ReversalShort_ocoID" + Bars[0].Time;
                oEnter = SubmitOrder(0, OrderAction.SellShort, OrderType.Market, 3, 0, 0, ocoId, SignalNameEnter);
                oStop = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, 3, 0, StopForReversalTrade, ocoId, SignalNameStop);
            }
            else
            {
                //keine Aktion     
                return;
            }

            CreateIfDoneGroup(new List<IOrder> { oEnter, oStop });
            oEnter.ConfirmOrder();
        }


        protected override void OnExecution(IExecution execution)
        {
            DateTime ts_Ausstieg;

           // ts_Ausstieg = Reversal2NextBar_Indicator().GetTargetBar(Bars[-1].Time);
            ts_Ausstieg = GlobalUtilities.GetTargetBar(Bars, Bars[0].Time, TimeFrame, 1);
            Print("Ausstieg: " + ts_Ausstieg + "Bars[-1].Time: " + Bars[-1].Time);


            foreach (Trade item in this.Root.Core.TradingManager.ActiveOpenedTrades)
            {
                if (item.EntryOrder.Name == SignalNameEnter
                 || item.EntryOrder.Name == SignalNameStop)
                {
                    item.Expiration = ts_Ausstieg;
                }
            }


            if (execution.MarketPosition == PositionType.Flat)
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

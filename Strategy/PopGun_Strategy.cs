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
/// You will find this script on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
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

        private IOrder oEnter;
        private IOrder oStop;

        string SignalNameEnter;
        string SignalNameStop;

        //internal
        private PopGun_Indicator _popgun_indicator = null;

        #endregion

        protected override void OnStartUp()
        {
            base.OnStartUp();

            //Init our indicator to get code access
            this._popgun_indicator = new PopGun_Indicator();
            this._popgun_indicator.SetData(this.PopGunExpires, this.IsSnapshotActive, this.IsEvaluationActive);
        }

		protected override void Initialize()
		{
			CalculateOnBarClose = true;
		}

		protected override void OnBarUpdate()
		{
            string ocoId;
            double StopForPopGunTrade;
            
            if (!IsCurrentBarLast || oEnter != null) return;
            double PopGun_Indicator_Value = this._popgun_indicator.calculate(this.Bars, this.CurrentBar);
            

            if (PopGun_Indicator_Value == 100)
                {
                    //Long        
                StopForPopGunTrade = _popgun_indicator.PopGunTriggerShort;
                    SignalNameEnter = "PopGunLong" + Bars[0].Time;
                    SignalNameStop = "PopGunStop" + Bars[0].Time;
                    ocoId = "PopGunLong_ocoID" + Bars[0].Time;
                    oEnter = SubmitOrder(0, OrderAction.Buy, OrderType.Market, 3, 0, 0, ocoId, SignalNameEnter);
                    oStop = SubmitOrder(0, OrderAction.Sell, OrderType.Stop, 3, 0, StopForPopGunTrade, ocoId, SignalNameStop);
                }
                else if (PopGun_Indicator_Value == -100)
                {
                    //Short    
                    StopForPopGunTrade = _popgun_indicator.PopGunTriggerLong;
                    SignalNameEnter = "PopGunShort" + Bars[0].Time;
                    SignalNameStop = "PopGunStop" + Bars[0].Time;
                    ocoId = "PopGunShort_ocoID" + Bars[0].Time;
                    oEnter = SubmitOrder(0, OrderAction.SellShort, OrderType.Market, 3, 0, 0, ocoId, SignalNameEnter);
                    oStop = SubmitOrder(0, OrderAction.BuyToCover, OrderType.Stop, 3, 0, StopForPopGunTrade, ocoId, SignalNameStop);
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
            DateTime ts_Ausstieg = this._popgun_indicator.PopGunTargetDateTime; 

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
        #endregion
    
    }
}

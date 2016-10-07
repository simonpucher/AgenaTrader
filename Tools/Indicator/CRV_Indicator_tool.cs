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
/// Version: 1.3.4
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// todo
/// + crv calculation not working on proposals
/// + if there are positions left in the portfolio write + symbol
/// -------------------------------------------------------------------------
/// Shows the CRV of your current trade in the right upper corner of the chart.
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("Shows the CRV of your current trade in the right upper corner of the chart.")]
    [Category("Tools")]
    public class CRV_Indicator_Tool : UserIndicator
	{

        private static DateTime _lastupdate = DateTime.Now;
        private TextPosition _TextPositionCRV = TextPosition.TopRight;
        private int _FontSizeCRV = 20;
        private int _seconds = 2;
        private ITradingTrade openedtrade = null;

        protected override void Initialize()
		{
			Overlay = true;
            CalculateOnBarClose = false;
        }


        protected override void OnStartUp()
        {
            // Add event listener
            if (ChartControl != null)
                ChartControl.ChartPanelMouseMove += ChartControl_ChartPanelMouseMove;

            calculateannddrawdata(true);
        }

        private void ChartControl_ChartPanelMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            calculateannddrawdata();
        }

        protected override void OnBarUpdate()
		{
            calculateannddrawdata();
        }
        


        private void calculateannddrawdata(bool force = false) {


            

            if (force || _lastupdate.AddSeconds(this._seconds) < DateTime.Now)
            {
                string text = "flat";


               

                //openedtrade = this.Root.Core.TradingManager.GetOpenedTrade(this.Instrument);
                //IEnumerable<ITradingOrder> SubmittedOrders = this.Root.Core.TradingManager.SubmittedOrders;
                //IEnumerable<ITradingOrder> RegisteredOrders = this.Root.Core.TradingManager.RegisteredOrders;

                //IEnumerable<ITradingOrder> ActiveOpenedOrders = this.Root.Core.TradingManager.ActiveOpenedOrders;
                //IEnumerable<ITradingOrder> ActiveRegisteredOrders = this.Root.Core.TradingManager.ActiveRegisteredOrders;
                //IEnumerable<ITradingOrder> ActiveSubmittedOrders = this.Root.Core.TradingManager.ActiveSubmittedOrders;


                double crv = 0.0;
                double up = 0.0;
                double down = 0.0;


                //IEnumerable<ITradingOrder> _regorders = this.Root.Core.TradingManager.ActiveRegisteredOrders.Where(x => x.Instrument.Symbol == this.Instrument.Symbol);
                //if (_regorders != null && _regorders.Count() > 0)
                //{
                //    PositionType direction = PositionType.Flat;

                //    foreach (ITradingOrder item in _regorders)
                //    {

                //    }
                    
                //}

               

                IEnumerable<ITradingOrder> _openorders = this.Root.Core.TradingManager.OpenedOrders.Where(x => x.Instrument.Symbol == this.Instrument.Symbol);

                //IEnumerable<ITradingOrder> combine = _openorders.Union(_regorders);

                if (TradeInfo != null && _openorders != null && _openorders.Count() > 0)
                {
                    
                    foreach (ITradingOrder item in _openorders)
                    {
                        if (TradeInfo.MarketPosition == PositionType.Long && item.IsLong)
                        {

                        }
                        else if (TradeInfo.MarketPosition == PositionType.Short && item.IsShort)
                        {

                        }
                        else
                        {
                            double price = item.StopPrice;
                            if (price == 0.0)
                            {
                                price = item.Price;
                            }
                            //stop or target
                            if (price < TradeInfo.AvgPrice && TradeInfo.MarketPosition == PositionType.Long
                                || price > TradeInfo.AvgPrice && TradeInfo.MarketPosition == PositionType.Short)
                            {
                                down = down + ((TradeInfo.AvgPrice * item.Quantity) - (item.StopPrice * item.Quantity));
                            }
                            else
                            {
                                up = up + ((item.Price * item.Quantity) - (TradeInfo.AvgPrice * item.Quantity));
                            }
                        }
                    }
                    
                }





                //double ordersize = 0.0;
                //double pricestop = 0.0;
                //double pricetarget = 0.0;

                //if (openedtrade != null)
                //{
                //    entryprice = openedtrade.EntryPrice;
                //    ordersize = openedtrade.Quantity;
                //    //PositionType postype = openedtrade.Type;

                //    IEnumerable<ITradingOrder> data = this.Root.Core.TradingManager.OpenedOrders.Where(x => x.Instrument.Symbol == this.Instrument.Symbol);
                //    //IIfDoneGroup data = openedtrade.EntryOrder.IfDoneGroup;
                //    if (data != null)
                //    {
                //        double up = 0.0;
                //        double down = 0.0;

                //        foreach (ITradingOrder tradord in data)
                //        {
                //            if (tradord.IsOrderOpened)
                //            {

                //                //if (tradord.IsStopLoss)
                //                if (tradord.Type == OrderType.Stop)
                //                {
                //                    down = down + (entryprice - tradord.StopPrice);
                //                    //down = down + ((entryprice * ordersize) - (tradord.StopPrice * tradord.Quantity));
                //                }
                //                else
                //                {
                //                    up = up + (tradord.Price - entryprice);
                //                    //up = up + ((tradord.Price * tradord.Quantity) - (entryprice * ordersize));
                //                }
                //            }
                //        }
                //        //crv = Math.Abs(((entryprice - pricetarget) * ordersize) / ((entryprice - pricestop) * ordersize));
                //        crv = up / down;
                //    }
                //}

                crv = up / down;

                if (down == 0.0 && up != 0.0)
                {
                    //text = "in risk";
                    text = "-";
                }
                else if (down != 0.0 && up == 0.0)
                {
                    //text = "in love";
                    text = "-";
                }
                else if (down == 0.0 && up == 0.0)
                {
                    if (TradeInfo != null)
                    {
                        switch (TradeInfo.MarketPosition)
                        {
                            case PositionType.Long:
                                text = "long";
                                break;
                            case PositionType.Short:
                                text = "short";
                                break;
                        }
                    }
                }
                else
                {
                    text = Math.Round(crv, 3).ToString();
                }
               
                DrawTextFixed("CRV_string", text, this.TextPositionCRV, Color.Black, new Font("Arial", this.FontSizeCRV, FontStyle.Regular), Color.Transparent, Color.Transparent);
                _lastupdate = DateTime.Now;
            }
           
        }


        protected override void OnTermination()
        {
            // Remove event listener
            if (ChartControl != null)
                ChartControl.ChartPanelMouseMove -= ChartControl_ChartPanelMouseMove;
        }
        


        public override string ToString()
        {
            return "CRV (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "CRV (I)";
            }
        }



        #region Properties
  
        /// <summary>
        /// </summary>
        [Description("Position of the text for your CRV.")]
        [Category("Parameters")]
        [DisplayName("TextPosition")]
        public TextPosition TextPositionCRV
        {
            get { return _TextPositionCRV; }
            set { _TextPositionCRV = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Font size of the text for your CRV.")]
        [Category("Parameters")]
        [DisplayName("Font size")]
        public int FontSizeCRV
        {
            get { return _FontSizeCRV; }
            set { _FontSizeCRV = value; }
        }


        [Browsable(false)]
		[XmlIgnore()]
		public DataSeries MyPlot1
		{
			get { return Values[0]; }
		}

		#endregion
	}
}
#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator
	{
		/// <summary>
		/// Shows the CRV of your current trade in the right upper corner of the chart.
		/// </summary>
		public CRV_Indicator_Tool CRV_Indicator_Tool()
        {
			return CRV_Indicator_Tool(Input);
		}

		/// <summary>
		/// Shows the CRV of your current trade in the right upper corner of the chart.
		/// </summary>
		public CRV_Indicator_Tool CRV_Indicator_Tool(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<CRV_Indicator_Tool>(input);

			if (indicator != null)
				return indicator;

			indicator = new CRV_Indicator_Tool
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input
						};
			indicator.SetUp();

			CachedCalculationUnits.AddIndicator2Cache(indicator);

			return indicator;
		}
	}

	#endregion

	#region Strategy

	public partial class UserStrategy
	{
		/// <summary>
		/// Shows the CRV of your current trade in the right upper corner of the chart.
		/// </summary>
		public CRV_Indicator_Tool CRV_Indicator_Tool()
		{
			return LeadIndicator.CRV_Indicator_Tool(Input);
		}

		/// <summary>
		/// Shows the CRV of your current trade in the right upper corner of the chart.
		/// </summary>
		public CRV_Indicator_Tool CRV_Indicator_Tool(IDataSeries input)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.CRV_Indicator_Tool(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Shows the CRV of your current trade in the right upper corner of the chart.
		/// </summary>
		public CRV_Indicator_Tool CRV_Indicator_Tool()
		{
			return LeadIndicator.CRV_Indicator_Tool(Input);
		}

		/// <summary>
		/// Shows the CRV of your current trade in the right upper corner of the chart.
		/// </summary>
		public CRV_Indicator_Tool CRV_Indicator_Tool(IDataSeries input)
		{
			return LeadIndicator.CRV_Indicator_Tool(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Shows the CRV of your current trade in the right upper corner of the chart.
		/// </summary>
		public CRV_Indicator_Tool CRV_Indicator_Tool()
		{
			return LeadIndicator.CRV_Indicator_Tool(Input);
		}

		/// <summary>
		/// Shows the CRV of your current trade in the right upper corner of the chart.
		/// </summary>
		public CRV_Indicator_Tool CRV_Indicator_Tool(IDataSeries input)
		{
			return LeadIndicator.CRV_Indicator_Tool(input);
		}
	}

	#endregion

}

#endregion

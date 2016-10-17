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
/// Version: 1.3.8
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// todo
/// + show crv in scanner column
/// + calculate crv with account data 
/// -------------------------------------------------------------------------
/// Shows the CRV of your current trade in the right upper corner of the chart.
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{


    public class crv_resultobject {

        public string text = "flat";
        public double crv = 0.0;
       public double crv_price = 0.0;
        public double up = 0.0;
        public double down = 0.0;
        public double up_price = 0.0;
        public double down_price = 0.0;

        public double target_quant = 0.0;
        public double stop_quant = 0.0;

        public crv_resultobject() {

        }
    }


	[Description("Shows the CRV of your current trade in the right upper corner of the chart.")]
    [Category("Tools")]
    public class CRV_Indicator_Tool : UserIndicator
	{

        private static DateTime _lastupdate = DateTime.Now;
        private TextPosition _TextPositionCRV = TextPosition.TopRight;
        private int _FontSizeCRV = 20;
        private int _seconds = 2;
        private ITradingTrade openedtrade = null;
        private int _rounddecimal = 3;

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
            //DrawTextFixed("debug_string", "x: " + e.X + " y: "+e.Y, this.TextPositionCRV, Color.Black, new Font("Arial", this.FontSizeCRV, FontStyle.Regular), Color.Transparent, Color.Transparent);
            calculateannddrawdata();
        }

        protected override void OnBarUpdate()
		{
            calculateannddrawdata();
        }
        


        private void calculateannddrawdata(bool force = false) {

            int quantity = 0;
            double price = 0.0;
            PositionType marketposition = PositionType.Flat;
            IList<ITradingOrder> stopstargets = new List<ITradingOrder>();

            if (force || _lastupdate.AddSeconds(this._seconds) < DateTime.Now)
            {
                IEnumerable<ITradingOrder> _regorders = this.Root.Core.TradingManager.ActiveRegisteredOrders.Where(x => x.Instrument.Symbol == this.Instrument.Symbol);
                IEnumerable<ITradingOrder> _openorders = this.Root.Core.TradingManager.OpenedOrders.Where(x => x.Instrument.Symbol == this.Instrument.Symbol);

                
                if (this.TradeInfo != null)
                {
                    quantity = this.TradeInfo.Quantity;
                    price = TradeInfo.AvgPrice;
                    marketposition = TradeInfo.MarketPosition;
                }

                foreach (ITradingOrder item in _regorders)
                {
                    if (item.IsManuallyConfirmable && this.TradeInfo == null)
                    {
                        quantity = item.Quantity;
                        price = item.Price;
                        if (item.IsLong)
                        {
                            marketposition = PositionType.Long;
                        }
                        else
                        {
                            marketposition = PositionType.Short;
                        }
                    }
                    else
                    {
                        stopstargets.Add(item);
                    }
                }

                _openorders = _openorders.Concat(stopstargets);

                crv_resultobject resultdata = this.calculate(_openorders, quantity, price, marketposition);

              


                //if (_regorders != null && _regorders.Count() > 0)
                //{
                //    resultdata = new crv_resultobject();
                //    int entry_quantity = 0;
                //    double entry_price = 0.0;
                //    PositionType MarketPosition = PositionType.Flat;
                //    IList<ITradingOrder> stopstargets = new List<ITradingOrder>();
                //    foreach (ITradingOrder item in _regorders)
                //    {
                //        if (item.IsManuallyConfirmable)
                //        {
                //            entry_quantity = item.Quantity;
                //            entry_price = item.Price;
                //            if (item.IsLong)
                //            {
                //                MarketPosition = PositionType.Long;
                //            }
                //            else
                //            {
                //                MarketPosition = PositionType.Short;
                //            }
                //        }
                //        else
                //        {
                //            stopstargets.Add(item);
                //        }
                //    }


                //    resultdata = this.calculate(stopstargets, entry_quantity, entry_price, MarketPosition);

                //}



                //if (this.TradeInfo != null)
                //{
                //    resultdata = new crv_resultobject();
                //    resultdata = this.calculate(_openorders, this.TradeInfo.Quantity, TradeInfo.AvgPrice, TradeInfo.MarketPosition);
                //}


                DrawTextFixed("CRV_string", resultdata.text, this.TextPositionCRV, Color.Black, new Font("Arial", this.FontSizeCRV, FontStyle.Regular), Color.Transparent, Color.Transparent);
                _lastupdate = DateTime.Now;
            }
           
        }


        protected override void OnTermination()
        {
            // Remove event listener
            if (ChartControl != null)
                ChartControl.ChartPanelMouseMove -= ChartControl_ChartPanelMouseMove;
        }


        public crv_resultobject calculate(IEnumerable<ITradingOrder> _openorders, int entry_quantity, double entry_price, PositionType positiontype) {
            crv_resultobject result = new crv_resultobject();

            if ( _openorders != null && _openorders.Count() > 0)
            {
                result.target_quant = entry_quantity;
                result.stop_quant = entry_quantity;

                foreach (ITradingOrder item in _openorders)
                {
                    if ((positiontype == PositionType.Long && item.IsLong) || (positiontype == PositionType.Short && item.IsShort))
                    {
                        //
                    }
                    else
                    {
                        double price = item.StopPrice;
                        if (price == 0.0)
                        {
                            price = item.Price;
                        }
                        //stop or target
                        if (price < entry_price && positiontype == PositionType.Long
                            || price > entry_price && positiontype == PositionType.Short)
                        {
                            result.stop_quant = result.stop_quant - item.Quantity;
                            result.down_price = result.down_price + (entry_price - item.StopPrice);
                            result.down = result.down + ((entry_price * item.Quantity) - (item.StopPrice * item.Quantity));
                        }
                        else
                        {
                            result.target_quant = result.target_quant - item.Quantity;
                            result.up_price = result.up_price + (item.Price - entry_price);
                            result.up = result.up + ((item.Price * item.Quantity) - (entry_price * item.Quantity));
                        }
                    }
                }

            }
            

            result.crv = result.up / result.down;
            result.crv_price = result.up_price / result.down_price;

            if (result.down == 0.0 && result.up != 0.0)
            {
                //text = "in risk";
                result.text = "-";
            }
            else if (result.down != 0.0 && result.up == 0.0)
            {
                //text = "in love";
                result.text = "-";
            }
            else if (result.down == 0.0 && result.up == 0.0)
            {
                if (entry_price >= Double.Epsilon)
                {
                    switch (positiontype)
                    {
                        case PositionType.Long:
                            result.text = "long";
                            break;
                        case PositionType.Short:
                            result.text = "short";
                            break;
                    }
                }
            }
            else
            {
                string textquant = "";
                if (result.target_quant > 0 && result.stop_quant > 0)
                {
                    textquant = "*";
                }
                else if (result.target_quant > 0)
                {
                    textquant = "+";
                }
                else if (result.stop_quant > 0)
                {
                    textquant = "-";
                }
                result.text = Math.Round(result.crv, this.RoundDecimal).ToString() + textquant + " / " + Math.Round(result.crv_price, this.RoundDecimal).ToString();
            }

            return result;
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
        [Description("Round to decimal numbers.")]
        [Category("Parameters")]
        [DisplayName("Decimal numbers")]
        public int RoundDecimal
        {
            get { return _rounddecimal; }
            set { _rounddecimal = value; }
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

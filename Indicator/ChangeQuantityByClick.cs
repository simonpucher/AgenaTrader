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
	[Description("Changes the quantity of an order by clicking on the chart.")]
	public class ChangeQuantity : UserIndicator
	{
		#region Variables

		//private string _positionsizes = "1;2;3;4;5";
        private OrdersLogFilter olf = new OrdersLogFilter();

		#endregion

		protected override void Initialize()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Transparent), "MyPlot1"));
			Overlay = true;
			CalculateOnBarClose = true;
		}


        protected override void OnStartUp()
        {

            // Add event listener
            if (ChartControl != null)
                ChartControl.ChartPanelMouseDown += OnChartPanelMouseDown;

            //if (this.Instrument != null && this.Instrument.Core.TradingManager != null)
            //{
            //    this.Instrument.Core.TradingManager.NewOrder += TradingManager_NewOrder;
            //    this.Instrument.Core.TradingManager.NewTrade += TradingManager_NewTrade;
            //    this.Instrument.Core.TradingManager.NewPosition += TradingManager_NewPosition;
            //    this.Instrument.Core.TradingManager.NewExecution += TradingManager_NewExecution;
            //}

            //Init Filter
            olf.Instruments = new List<IInstrument>();
            olf.Instruments.Add(this.Instrument);

        }

        //void TradingManager_NewPosition(object sender, PositionEventArgs e)
        //{
      
        //}

        //void TradingManager_NewExecution(object sender, ExecutionEventArgs e)
        //{
          
        //}

        //void TradingManager_NewTrade(object sender, TradeEventArgs e)
        //{
       
        //}

        //void TradingManager_NewOrder(object sender, OrderEventArgs e)
        //{
        
        //}


		protected override void OnBarUpdate()
		{
			MyPlot1.Set(Input[0]);
		}


        protected override void OnTermination()
        {
            // Remove event listener
            if (ChartControl != null)
                ChartControl.ChartPanelMouseDown -= OnChartPanelMouseDown;
        }


        private void OnChartPanelMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //this.Instrument.Core.TradingManager.GetOpenOrders();
            //IList<ITradingOrder> tol = this.Instrument.Core.TradingManager.OpenedOrders.ToList();
            //IList<ITradingPosition> tpl = this.Instrument.Core.TradingManager.OpenedPositions.ToList();
            //IList<ITradingTrade> ttl = this.Instrument.Core.TradingManager.OpenedTrades.ToList();
            
            //if (e.Button == System.Windows.Forms.MouseButtons.Right) { MessageBox.Show("Right click"); }
            //if (e.Button == System.Windows.Forms.MouseButtons.Left) { MessageBox.Show("Left click"); }



            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                //Get the last order where IsProposal is true.
                if (this.TradingManager != null && olf != null && this.Instrument != null)
                {
                    IEnumerable<OrdersLogRecord> olren = this.TradingManager.GetOrdersLog(olf).Where(x => x.Instrument.Id == this.Instrument.Id).Where(x => x.IsProposal == true).Where(x=>x.State == OrderState.PendingSubmit);
                    List<OrdersLogRecord> hhh = olren.ToList();
                    OrdersLogRecord olr = olren.LastOrDefault();
                    if (olr != null)
                    {
                        //Cast the order
                        Order ord = (Order)this.TradingManager.GetOrder(olr.OrderId);
                        if (ord != null && ord.State == OrderState.PendingSubmit)
                        {
                            //Change quantity
                            double clickprice = ChartControl.GetPriceByY(e.Y);
                            if (clickprice >= ord.Price + (ord.Price/100*1) )
                            {
                                ord.Quantity = ord.Quantity + 1;
                                this.TradingManager.EditOrder(ord);
                            }
                            else if (clickprice <= ord.Price - (ord.Price/100*1))
                            {
                                if (ord.Quantity > 1)
                                {
                                    ord.Quantity = ord.Quantity - 1;
                                    this.TradingManager.EditOrder(ord);
                                }
                            }

                         
                        }
                    }
                }  
            }
            
            //Print("X = {0}, Y = {1}", ChartControl.GetDateTimeByX(e.X), ChartControl.GetPriceByY(e.Y));

        }


		#region Properties

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries MyPlot1
		{
			get { return Values[0]; }
		}

        //[Description("")]
        //[Category("Parameters")]
        //public string Positionsizes
        //{
        //    get { return _positionsizes; }
        //    set { _positionsizes = value; }
        //}

		#endregion
	}
}
#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator : Indicator
	{
		/// <summary>
		/// Changes the position size by clicking on the chart.
		/// </summary>
		public ChangeQuantity ChangeQuantity()
        {
			return ChangeQuantity(Input);
		}

		/// <summary>
		/// Changes the position size by clicking on the chart.
		/// </summary>
		public ChangeQuantity ChangeQuantity(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<ChangeQuantity>(input);

			if (indicator != null)
				return indicator;

			indicator = new ChangeQuantity
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
		/// Changes the position size by clicking on the chart.
		/// </summary>
		public ChangeQuantity ChangeQuantity()
		{
			return LeadIndicator.ChangeQuantity(Input);
		}

		/// <summary>
		/// Changes the position size by clicking on the chart.
		/// </summary>
		public ChangeQuantity ChangeQuantity(IDataSeries input)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.ChangeQuantity(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Changes the position size by clicking on the chart.
		/// </summary>
		public ChangeQuantity ChangeQuantity()
		{
			return LeadIndicator.ChangeQuantity(Input);
		}

		/// <summary>
		/// Changes the position size by clicking on the chart.
		/// </summary>
		public ChangeQuantity ChangeQuantity(IDataSeries input)
		{
			return LeadIndicator.ChangeQuantity(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Changes the position size by clicking on the chart.
		/// </summary>
		public ChangeQuantity ChangeQuantity()
		{
			return LeadIndicator.ChangeQuantity(Input);
		}

		/// <summary>
		/// Changes the position size by clicking on the chart.
		/// </summary>
		public ChangeQuantity ChangeQuantity(IDataSeries input)
		{
			return LeadIndicator.ChangeQuantity(input);
		}
	}

	#endregion

}

#endregion


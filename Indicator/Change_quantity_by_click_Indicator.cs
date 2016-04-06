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

/// Version: 1.0
namespace AgenaTrader.UserCode
{

    [Description("Changes the quantity of an order by clicking on the chart.")]
	public class ChangeQuantity : UserIndicator
	{
		#region Variables

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

            //Init Filter
            olf.Instruments = new List<IInstrument>();
            olf.Instruments.Add(this.Instrument);

        }



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
        }


        public override string ToString()
        {
            return "Change Quantity";
        }


        #region Properties

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

	public partial class UserIndicator : Indicator
	{
		/// <summary>
		/// Changes the quantity of an order by clicking on the chart.
		/// </summary>
		public ChangeQuantity ChangeQuantity()
        {
			return ChangeQuantity(Input);
		}

		/// <summary>
		/// Changes the quantity of an order by clicking on the chart.
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
		/// Changes the quantity of an order by clicking on the chart.
		/// </summary>
		public ChangeQuantity ChangeQuantity()
		{
			return LeadIndicator.ChangeQuantity(Input);
		}

		/// <summary>
		/// Changes the quantity of an order by clicking on the chart.
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
		/// Changes the quantity of an order by clicking on the chart.
		/// </summary>
		public ChangeQuantity ChangeQuantity()
		{
			return LeadIndicator.ChangeQuantity(Input);
		}

		/// <summary>
		/// Changes the quantity of an order by clicking on the chart.
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
		/// Changes the quantity of an order by clicking on the chart.
		/// </summary>
		public ChangeQuantity ChangeQuantity()
		{
			return LeadIndicator.ChangeQuantity(Input);
		}

		/// <summary>
		/// Changes the quantity of an order by clicking on the chart.
		/// </summary>
		public ChangeQuantity ChangeQuantity(IDataSeries input)
		{
			return LeadIndicator.ChangeQuantity(input);
		}
	}

	#endregion

}

#endregion


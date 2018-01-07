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
/// Version: 1.2
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// Changes the quantity of an order by clicking on the chart.
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this indicator without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{

    [Description("Changes the quantity of an order by clicking on the chart.")]
    public class ChangeQuantity_Tool : UserIndicator
	{
		#region Variables

        private OrdersLogFilter olf = new OrdersLogFilter();

		#endregion

		protected override void OnInit()
		{
			Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Transparent), "MyPlot1"));
			IsOverlay = true;
			CalculateOnClosedBar = true;
		}


        protected override void OnStart()
        {

            // Add event listener
            if (Chart != null)
                Chart.ChartPanelMouseDown += OnChartPanelMouseDown;

            //Init Filter
            olf.Instruments = new List<IInstrument>();
            olf.Instruments.Add(this.Instrument);

        }



		protected override void OnCalculate()
		{
			MyPlot1.Set(InSeries[0]);
		}


        protected override void OnDispose()
        {
            // Remove event listener
            if (Chart != null)
                Chart.ChartPanelMouseDown -= OnChartPanelMouseDown;
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
                            double clickprice = Chart.GetPriceByY(e.Y);
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
            return "Change quantity (T)";
        }

        public override string DisplayName
        {
            get
            {
                return "Change quantity (T)";
            }
        }


        #region Properties

        [Browsable(false)]
		[XmlIgnore()]
		public DataSeries MyPlot1
		{
			get { return Outputs[0]; }
		}

		#endregion
	}
}
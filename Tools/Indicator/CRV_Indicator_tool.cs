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
/// Version: 1.1
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// Shows the CRV in the right upper corner of the chart.
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("Shows the CRV of your current trade in the right upper corner of the chart.")]
    [Category("Tools")]
    public class CRV_Indicator_Tool : UserIndicator
	{

        private TextPosition _TextPositionCRV = TextPosition.TopRight;
        private int _FontSizeCRV = 20;
        private IEnumerable<ITradingTrade> openedtrades = null;


        protected override void Initialize()
		{
			Overlay = true;
            CalculateOnBarClose = false;
        }



        protected override void OnBarUpdate()
		{
            calculateannddrawdata();
        }
        


        private void calculateannddrawdata() {

            string text = "flat";
            openedtrades = this.Root.Core.TradingManager.GetOpenedTrades();
            double crv = 0.0;
            double entryprice = 0.0;
            double ordersize = 0.0;
            double pricestop = 0.0;
            double pricetarget = 0.0;

            foreach (ITradingTrade item in openedtrades)
            {
                if (openedtrades.Count() > 0)
                {
                    entryprice = openedtrades.First().EntryPrice;
                    ordersize = openedtrades.First().Quantity;
                    IIfDoneGroup data = item.EntryOrder.IfDoneGroup;
                    if (data != null)
                    {
                        foreach (ITradingOrder tradord in data)
                        {
                            if (tradord.IsOrderOpened)
                            {
                                if (tradord.IsStopLoss)
                                {
                                    pricestop = tradord.StopPrice;
                                }
                                else
                                {
                                    pricetarget = tradord.Price;
                                }
                            }
                        }
                        crv = Math.Abs(((entryprice - pricetarget) * ordersize) / ((entryprice - pricestop) * ordersize));
                    }
                }
            }
            if (crv != 0.0)
            {
                if (pricetarget > entryprice && pricestop > entryprice)
                {
                    text = "no risk";
                }
                else
                {
                    text = Math.Round(crv, 3).ToString();
                }
            }
            DrawTextFixed("CRV_string", text, this.TextPositionCRV, Color.Black, new Font("Arial", this.FontSizeCRV, FontStyle.Regular), Color.Transparent, Color.Transparent);
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
		public CRV_Indicator_Tool CRV_Indicator_Tool(TextPosition textPositionCRV, System.Int32 fontSizeCRV)
        {
			return CRV_Indicator_Tool(Input, textPositionCRV, fontSizeCRV);
		}

		/// <summary>
		/// Shows the CRV of your current trade in the right upper corner of the chart.
		/// </summary>
		public CRV_Indicator_Tool CRV_Indicator_Tool(IDataSeries input, TextPosition textPositionCRV, System.Int32 fontSizeCRV)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<CRV_Indicator_Tool>(input, i => i.TextPositionCRV == textPositionCRV && i.FontSizeCRV == fontSizeCRV);

			if (indicator != null)
				return indicator;

			indicator = new CRV_Indicator_Tool
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							TextPositionCRV = textPositionCRV,
							FontSizeCRV = fontSizeCRV
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
		public CRV_Indicator_Tool CRV_Indicator_Tool(TextPosition textPositionCRV, System.Int32 fontSizeCRV)
		{
			return LeadIndicator.CRV_Indicator_Tool(Input, textPositionCRV, fontSizeCRV);
		}

		/// <summary>
		/// Shows the CRV of your current trade in the right upper corner of the chart.
		/// </summary>
		public CRV_Indicator_Tool CRV_Indicator_Tool(IDataSeries input, TextPosition textPositionCRV, System.Int32 fontSizeCRV)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.CRV_Indicator_Tool(input, textPositionCRV, fontSizeCRV);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Shows the CRV of your current trade in the right upper corner of the chart.
		/// </summary>
		public CRV_Indicator_Tool CRV_Indicator_Tool(TextPosition textPositionCRV, System.Int32 fontSizeCRV)
		{
			return LeadIndicator.CRV_Indicator_Tool(Input, textPositionCRV, fontSizeCRV);
		}

		/// <summary>
		/// Shows the CRV of your current trade in the right upper corner of the chart.
		/// </summary>
		public CRV_Indicator_Tool CRV_Indicator_Tool(IDataSeries input, TextPosition textPositionCRV, System.Int32 fontSizeCRV)
		{
			return LeadIndicator.CRV_Indicator_Tool(input, textPositionCRV, fontSizeCRV);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Shows the CRV of your current trade in the right upper corner of the chart.
		/// </summary>
		public CRV_Indicator_Tool CRV_Indicator_Tool(TextPosition textPositionCRV, System.Int32 fontSizeCRV)
		{
			return LeadIndicator.CRV_Indicator_Tool(Input, textPositionCRV, fontSizeCRV);
		}

		/// <summary>
		/// Shows the CRV of your current trade in the right upper corner of the chart.
		/// </summary>
		public CRV_Indicator_Tool CRV_Indicator_Tool(IDataSeries input, TextPosition textPositionCRV, System.Int32 fontSizeCRV)
		{
			return LeadIndicator.CRV_Indicator_Tool(input, textPositionCRV, fontSizeCRV);
		}
	}

	#endregion

}

#endregion

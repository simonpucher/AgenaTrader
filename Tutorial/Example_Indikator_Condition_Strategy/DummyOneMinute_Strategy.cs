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

namespace AgenaTrader.UserCode
{
	[Description("Enter the description for the new strategy here")]
	public class DummyOneMinute_Strategy : UserStrategy
	{
        private IOrder oEnterLong;

		protected override void Initialize()
		{
		}

		protected override void OnBarUpdate()
		{
            //EnterLong(3);


            //oEnterLong = SubmitOrder(0, OrderAction.Buy, OrderType.Stop, DefaultQuantity, 0, Close[0] * 1.1, "ocoId", "signalName");
            //oEnterShort = SubmitOrder(0, OrderAction.SellShort, OrderType.Stop, DefaultQuantity, 0, Close[0] * -1.1, "ocoId", "signalName");

            //CreateOCOGroup(new List<IOrder> { oEnterLong, oEnterShort });

            //oEnterLong.ConfirmOrder();
            //oEnterShort.ConfirmOrder();

            if (DummyOneMinuteEven_Indicator()[0] == 100)
            {
                if (!IsCurrentBarLast || oEnterLong != null)
                    return;

                oEnterLong = SubmitOrder(0, OrderAction.Buy, OrderType.Market, 3, 0, Close[0], "ocoId", "signalName");
            }
		}

        

      //  public IOrder EnterLong(double price)
       // {
            //Print(Close);
            //oEnterLong = SubmitOrder(0, OrderAction.Buy, OrderType.Stop, 3, 0, price, "ocoId", "signalName");
           //return oEnterLong;
        //}
	}
}

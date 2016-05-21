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
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// This indicator provides entry and exit signals on time.
/// Long signal in every even minute. Short signal every odd minute.
/// You can use this indicator also as a template for further development.
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
  [Description("This indicator provides a long signal in every even minute and a short signal every odd minute.")]
    public class DummyOneMinute_Strategy : UserStrategy, IDummyOneMinuteEven
	{
        //input
        private bool _IsShortEnabled = false;
        private bool _IsLongEnabled = true;

        //output

        //internal
        private DummyOneMinuteEven_Indicator _DummyOneMinuteEven_Indicator = null;
        private IOrder oEnterLong;
        private IOrder oExitLong;

		protected override void Initialize()
		{
		}

        protected override void InitRequirements()
        {
            base.InitRequirements();

        }


        protected override void OnStartUp()
        {
            base.OnStartUp();

            //Init our indicator to get code access
            this._DummyOneMinuteEven_Indicator = new DummyOneMinuteEven_Indicator();
        }

		protected override void OnBarUpdate()
		{
            //EnterLong(3);


            //oEnterLong = SubmitOrder(0, OrderAction.Buy, OrderType.Stop, DefaultQuantity, 0, Close[0] * 1.1, "ocoId", "signalName");
            //oEnterShort = SubmitOrder(0, OrderAction.SellShort, OrderType.Stop, DefaultQuantity, 0, Close[0] * -1.1, "ocoId", "signalName");

            //CreateOCOGroup(new List<IOrder> { oEnterLong, oEnterShort });

            //oEnterLong.ConfirmOrder();
            //oEnterShort.ConfirmOrder();

            //Lets call the calculate method and save the result with the trade action
            ResultValueDummyOneMinuteEven returnvalue = this._DummyOneMinuteEven_Indicator.calculate(Bars[0], this.IsLongEnabled, this.IsShortEnabled);

            //if (DummyOneMinuteEven_Indicator()[0] == 100)
            //{
            //    if (!IsCurrentBarLast || oEnterLong != null)
            //        return;

            //    oEnterLong = SubmitOrder(0, OrderAction.Buy, OrderType.Market, 3, 0, Close[0], "ocoId", "signalName");
            //}
		}

        

      //  public IOrder EnterLong(double price)
       // {
            //Print(Close);
            //oEnterLong = SubmitOrder(0, OrderAction.Buy, OrderType.Stop, 3, 0, price, "ocoId", "signalName");
           //return oEnterLong;
        //}


        #region Properties

        #region Input
        /// <summary>
        /// </summary>
        [Description("If true it is allowed to create long positions.")]
        [Category("Parameters")]
        [DisplayName("Allow Long")]
        public bool IsLongEnabled
        {
            get { return _IsLongEnabled; }
            set { _IsLongEnabled = value; }
        }


        /// <summary>
        /// </summary>
        [Description("If true it is allowed to create short positions.")]
        [Category("Parameters")]
        [DisplayName("Allow Short")]
        public bool IsShortEnabled
        {
            get { return _IsShortEnabled; }
            set { _IsShortEnabled = value; }
        }

        #endregion

        #endregion

    }
}

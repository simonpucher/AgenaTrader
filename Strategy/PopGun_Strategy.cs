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
	[Description("Strategie zu Pop Gun Bar Pattern")]
	public class PopGun_Strategy : UserStrategy
	{

        #region Variables
        double PopGun_Indicator_Value;
        int _PopGunExpires = 5;

        private IOrder oEnter;
        private IOrder oStop;

        string SignalNameEnter;
        string SignalNameStop;
        #endregion

		protected override void Initialize()
		{
			CalculateOnBarClose = true;
		}

		protected override void OnBarUpdate()
		{
            if (!IsCurrentBarLast || oEnter != null) return;

            ////PopGun Indikator aufrufen. Dieser liefert für Mustererkennung
            //PopGun_Indicator_Value = PopGun_Indicator(_PopGunExpires)[0];
            //if (PopGun_Indicator_Value == 100)
            //{



            //}
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
        #endregion
    
    }
}

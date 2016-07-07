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
	[Description("Gets latest VDAX_NEW value from OnVista via httpRequest")]
	public class VDAX_NEW_OnVista : UserIndicator
	{
		protected override void Initialize()
		{
            Overlay = true;
		}


        protected override void OnStartUp()
        {
            decimal vdax_new = GlobalUtilities.GetCurrentVdaxNew();

            DrawTextFixed("VDAX_NEW", "VDAX-NEW: " + vdax_new, TextPosition.BottomRight);
        }


		protected override void OnBarUpdate()
		{

		}

		#region Properties

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
		/// Gets latest VDAX_NEW value from OnVista via httpRequest
		/// </summary>
		public VDAX_NEW_OnVista VDAX_NEW_OnVista()
        {
			return VDAX_NEW_OnVista(Input);
		}

		/// <summary>
		/// Gets latest VDAX_NEW value from OnVista via httpRequest
		/// </summary>
		public VDAX_NEW_OnVista VDAX_NEW_OnVista(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<VDAX_NEW_OnVista>(input);

			if (indicator != null)
				return indicator;

			indicator = new VDAX_NEW_OnVista
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
		/// Gets latest VDAX_NEW value from OnVista via httpRequest
		/// </summary>
		public VDAX_NEW_OnVista VDAX_NEW_OnVista()
		{
			return LeadIndicator.VDAX_NEW_OnVista(Input);
		}

		/// <summary>
		/// Gets latest VDAX_NEW value from OnVista via httpRequest
		/// </summary>
		public VDAX_NEW_OnVista VDAX_NEW_OnVista(IDataSeries input)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.VDAX_NEW_OnVista(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Gets latest VDAX_NEW value from OnVista via httpRequest
		/// </summary>
		public VDAX_NEW_OnVista VDAX_NEW_OnVista()
		{
			return LeadIndicator.VDAX_NEW_OnVista(Input);
		}

		/// <summary>
		/// Gets latest VDAX_NEW value from OnVista via httpRequest
		/// </summary>
		public VDAX_NEW_OnVista VDAX_NEW_OnVista(IDataSeries input)
		{
			return LeadIndicator.VDAX_NEW_OnVista(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Gets latest VDAX_NEW value from OnVista via httpRequest
		/// </summary>
		public VDAX_NEW_OnVista VDAX_NEW_OnVista()
		{
			return LeadIndicator.VDAX_NEW_OnVista(Input);
		}

		/// <summary>
		/// Gets latest VDAX_NEW value from OnVista via httpRequest
		/// </summary>
		public VDAX_NEW_OnVista VDAX_NEW_OnVista(IDataSeries input)
		{
			return LeadIndicator.VDAX_NEW_OnVista(input);
		}
	}

	#endregion

}

#endregion

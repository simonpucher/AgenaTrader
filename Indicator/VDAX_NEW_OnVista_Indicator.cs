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
/// Version: in progress
/// -------------------------------------------------------------------------
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// Gets latest VDAX_NEW value from OnVista via httpRequest
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Gets latest VDAX_NEW value from OnVista via httpRequest")]
    public class VDAX_NEW_OnVista_Indicator : UserIndicator
    {
        //*** input ***
        private TextPosition _TextPosition = TextPosition.BottomRight;
        private int _TextSize = 10;

        //*** internal ***
        static decimal vdax_new = 0;
        static DateTime? lastcheck = null;
        static int lastcheck_eachxseconds = 60;
        //Opening hours of VDAX are between: 08:50 - 17:50
        TimeSpan openinghours_open = new TimeSpan(8, 50, 0);
        TimeSpan openinghours_close = new TimeSpan(17, 50, 0);
        TimeSpan openinghours_marketdelay = new TimeSpan(0, 15, 0);


        protected override void Initialize()
        {
            Overlay = true;
        }


        protected override void OnStartUp()
        {


        }


        protected override void OnBarUpdate()
        {
            bool checkonline = false;
            TimeSpan now = DateTime.Now.TimeOfDay;
            
            //Check if we have not done this yet.
            if (lastcheck == null)
            {
                checkonline = true;
            }
            //Check the online service each x seconds
            else if (lastcheck.Value.AddSeconds(lastcheck_eachxseconds) <= DateTime.Now)
            {
                //If the market is closed we do not need to ask the online service
                if ((now >= openinghours_open) && (now <= openinghours_close.Add(openinghours_marketdelay)))
                {
                    checkonline = true;
                }
            }
            
            //If true we check online for the data
            if (checkonline)
            {
                vdax_new = GlobalUtilities.GetCurrentVdaxNew();
                lastcheck = DateTime.Now;
            }
            DrawTextFixed("VDAX_NEW", "VDAX-NEW: " + vdax_new, this.TextPosition, Color.Black, new Font("Arial", this.TextSize), Color.Transparent, Color.Transparent);
        }

        public override string ToString()
        {
            return "VDAX-NEW (OnVista)";
        }

        public override string DisplayName
        {
            get
            {
                return "VDAX-NEW (OnVista)";
            }
        }

        #region Properties

        #region Input

        /// <summary>
        /// </summary>
        [Description("Text Position")]
        [Category("Drawings")]
        [DisplayName("Text Position")]
        public TextPosition TextPosition
        {
            get { return _TextPosition; }
            set { _TextPosition = value; }
        }

        /// <summary>
        /// </summary>
        [Description("Text Size")]
        [Category("Drawings")]
        [DisplayName("Text Size")]
        public int TextSize
        {
            get { return _TextSize; }
            set { _TextSize = value; }
        }

        #endregion

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
		/// Gets latest VDAX_NEW value from OnVista via httpRequest
		/// </summary>
		public VDAX_NEW_OnVista_Indicator VDAX_NEW_OnVista_Indicator()
        {
			return VDAX_NEW_OnVista_Indicator(Input);
		}

		/// <summary>
		/// Gets latest VDAX_NEW value from OnVista via httpRequest
		/// </summary>
		public VDAX_NEW_OnVista_Indicator VDAX_NEW_OnVista_Indicator(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<VDAX_NEW_OnVista_Indicator>(input);

			if (indicator != null)
				return indicator;

			indicator = new VDAX_NEW_OnVista_Indicator
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
		public VDAX_NEW_OnVista_Indicator VDAX_NEW_OnVista_Indicator()
		{
			return LeadIndicator.VDAX_NEW_OnVista_Indicator(Input);
		}

		/// <summary>
		/// Gets latest VDAX_NEW value from OnVista via httpRequest
		/// </summary>
		public VDAX_NEW_OnVista_Indicator VDAX_NEW_OnVista_Indicator(IDataSeries input)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.VDAX_NEW_OnVista_Indicator(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Gets latest VDAX_NEW value from OnVista via httpRequest
		/// </summary>
		public VDAX_NEW_OnVista_Indicator VDAX_NEW_OnVista_Indicator()
		{
			return LeadIndicator.VDAX_NEW_OnVista_Indicator(Input);
		}

		/// <summary>
		/// Gets latest VDAX_NEW value from OnVista via httpRequest
		/// </summary>
		public VDAX_NEW_OnVista_Indicator VDAX_NEW_OnVista_Indicator(IDataSeries input)
		{
			return LeadIndicator.VDAX_NEW_OnVista_Indicator(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Gets latest VDAX_NEW value from OnVista via httpRequest
		/// </summary>
		public VDAX_NEW_OnVista_Indicator VDAX_NEW_OnVista_Indicator()
		{
			return LeadIndicator.VDAX_NEW_OnVista_Indicator(Input);
		}

		/// <summary>
		/// Gets latest VDAX_NEW value from OnVista via httpRequest
		/// </summary>
		public VDAX_NEW_OnVista_Indicator VDAX_NEW_OnVista_Indicator(IDataSeries input)
		{
			return LeadIndicator.VDAX_NEW_OnVista_Indicator(input);
		}
	}

	#endregion

}

#endregion

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
        private int _CheckEveryXSeconds = 60;

        //*** internal ***



        protected override void Initialize()
        {
            Overlay = true;
        }


        protected override void OnStartUp()
        {
            decimal vdax_new = GlobalUtilities.GetCurrentVdaxNew(this.CheckEveryXSeconds);
            DrawTextFixed("VDAX_NEW", "VDAX-NEW: " + vdax_new, this.TextPosition, Color.Black, new Font("Arial", this.TextSize), Color.Transparent, Color.Transparent);
        }


        protected override void OnBarUpdate()
        {

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
        [Description("Check online service each x seconds.")]
        [Category("Drawings")]
        [DisplayName("Seconds Online Check")]
        public int CheckEveryXSeconds
        {
            get { return _CheckEveryXSeconds; }
            set { _CheckEveryXSeconds = value; }
        }


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

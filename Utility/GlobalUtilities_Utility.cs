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
/// Version: 1.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// Global utilities as a helper in Agena Trader Script.
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    public static class GlobalUtilities
    {

        #region Colors

            /// <summary>
            /// Adjust the brightness of a color. 
            /// e.g. use this function to create a similiar color in a button click event or on mouse hover.
            /// </summary>
            /// <param name="originalColour"></param>
            /// <param name="brightnessFactor"></param>
            /// <returns></returns>
            public static Color AdjustBrightness(Color originalColour, double brightnessFactor)
            {
                return Color.FromArgb(originalColour.A, (int)(originalColour.R * brightnessFactor), (int)(originalColour.G * brightnessFactor), (int)(originalColour.B * brightnessFactor));
            }

            /// <summary>
            /// Adjust the opacity of a color. 
            /// e.g. use this function to change the alpha channel of the Color.
            /// </summary>
            /// <param name="originalColour"></param>
            /// <param name="opacityFactor"></param>
            /// <returns></returns>
            public static Color AdjustOpacity(Color originalColour, double opacityFactor)
            {
                return Color.FromArgb((int)(originalColour.A * opacityFactor), originalColour.R, originalColour.G, originalColour.B);
            }

        #endregion
    }


	[Description("We use this indicator to share global code in agena trader (like a global helper class).")]
	public class GlobalUtility : UserIndicator
	{



        
    }
}
#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator : Indicator
	{
		/// <summary>
		/// We use this indicator to share global code in agena trader (like a global helper class).
		/// </summary>
		public GlobalUtility GlobalUtility()
        {
			return GlobalUtility(Input);
		}

		/// <summary>
		/// We use this indicator to share global code in agena trader (like a global helper class).
		/// </summary>
		public GlobalUtility GlobalUtility(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<GlobalUtility>(input);

			if (indicator != null)
				return indicator;

			indicator = new GlobalUtility
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
		/// We use this indicator to share global code in agena trader (like a global helper class).
		/// </summary>
		public GlobalUtility GlobalUtility()
		{
			return LeadIndicator.GlobalUtility(Input);
		}

		/// <summary>
		/// We use this indicator to share global code in agena trader (like a global helper class).
		/// </summary>
		public GlobalUtility GlobalUtility(IDataSeries input)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.GlobalUtility(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// We use this indicator to share global code in agena trader (like a global helper class).
		/// </summary>
		public GlobalUtility GlobalUtility()
		{
			return LeadIndicator.GlobalUtility(Input);
		}

		/// <summary>
		/// We use this indicator to share global code in agena trader (like a global helper class).
		/// </summary>
		public GlobalUtility GlobalUtility(IDataSeries input)
		{
			return LeadIndicator.GlobalUtility(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// We use this indicator to share global code in agena trader (like a global helper class).
		/// </summary>
		public GlobalUtility GlobalUtility()
		{
			return LeadIndicator.GlobalUtility(Input);
		}

		/// <summary>
		/// We use this indicator to share global code in agena trader (like a global helper class).
		/// </summary>
		public GlobalUtility GlobalUtility(IDataSeries input)
		{
			return LeadIndicator.GlobalUtility(input);
		}
	}

	#endregion

}

#endregion

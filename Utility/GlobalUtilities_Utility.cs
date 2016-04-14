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


            public static TimeSpan GetOfficialMarketOpeningTime(string Symbol)
            {
                //Gets official Stock Market Opening Time
                //Dirty hack to handle different pre-market times
                //technically we can not distinguish between pre-market and market data
                //e.g. use this function to determine opening time for Dax-Index (09.00) or Nasdaq-Index(15.30)
                if (Symbol.Contains("DE.30") || Symbol.Contains("DE-XTB"))
                {
                    return new TimeSpan(9, 00, 00);
                }
                else if (Symbol.Contains("US.30") || Symbol.Contains("US-XTB"))
                {
                    return new TimeSpan(15, 30, 00);
                }
                else
                {
                    return new TimeSpan(9, 00, 00);
                }
            }

            public static TimeSpan GetOfficialMarketClosingTime(string Symbol)
            {
                //Gets official Stock Market Closing Time
                //e.g. use this function to determine closing time for Dax-Index (17.30) or Nasdaq-Index(22.00)
                if (Symbol.Contains("DE.30") || Symbol.Contains("DE-XTB"))
                {
                    return new TimeSpan(17, 30, 00);

                }
                else if (Symbol.Contains("US.30") || Symbol.Contains("US-XTB"))
                {
                    return new TimeSpan(22, 00, 00);
                }
                else
                {
                    return new TimeSpan(17, 30, 00);
                }
            }



            public static IBar GetFirstBarOfCurrentSession(IBars Bars) {
                //returns the first Bar of the latest(=current) Session
                return Bars.Where(x => x.Time.Date == Bars[0].Time.Date).FirstOrDefault();
            }


    }
        #endregion
    }


	[Description("We use this indicator to share global code in agena trader (like a global helper class).")]
	public class GlobalUtility : AgenaTrader.UserCode.UserIndicator
	{

        

}
#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
}

#endregion

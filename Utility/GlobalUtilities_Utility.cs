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
    /// <summary>
    /// Global static Helper.
    /// </summary>
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



        public static IBar GetFirstBarOfCurrentSession(IBars Bars, DateTime Date)
        {
            //returns the first Bar of the latest(=current) Session
            return Bars.Where(x => x.Time.Date == Date).FirstOrDefault();
        }

        public static double GetHighestHigh(IBars Bars, int BarsAgo) {
//HighestHigh Method is not available in Conditions, therefore this alternative can be used
                double HighestHigh = 0;
                for (int i = 0; i < BarsAgo; i++)

                    if (HighestHigh < Bars[i].High)
                    { 
                    HighestHigh = Bars[i].High;
                    }
                    ;
                    return HighestHigh;   
            }

        public static double GetLowestLow(IBars Bars, int BarsAgo)
        {
            //LowestLow Method is not available in Conditions, therefore this alternative can be used
            double LowestLow = 9999999999;
            for (int i = 0; i < BarsAgo; i++)

<<<<<<< HEAD
                if (LowestLow > Bars[i].High)
                {
                    LowestLow = Bars[i].High;
                }
            ;
            return LowestLow;
        }
    }


=======
 
>>>>>>> refs/remotes/simonpucher/master
        #endregion

        #region DateTime
        /// <summary>
        /// Returns the Start of the current week.
        /// Code from: http://www.codeproject.com/Articles/9706/C-DateTime-Library
        /// </summary>
        /// <returns></returns>
          public static DateTime GetStartOfCurrentWeek()
            {
                int DaysToSubtract = (int)DateTime.Now.DayOfWeek;
                DateTime dt = DateTime.Now.Subtract(System.TimeSpan.FromDays( DaysToSubtract ) );
                return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, 0);
            }

        #endregion

    }
}

/// <summary>
/// This class contains all extension methods for strings.
/// </summary>
public static class StringExtensions {

    /// <summary>
    /// Checks if the string contains a numeric value.
    /// </summary>
    /// <param name="theValue">The string to be tested.</param>
    /// <returns>True if the string is numeric.</returns>
    public static bool IsNumeric(this string theValue)
    {
        long retNum;
        return long.TryParse(theValue, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
    }

    /// <summary>
    /// Counts the words in a string.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int WordCount(this String str)
    {
        return str.Split(new char[] { ' ', '.', '?' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }
}


	[Description("We use this indicator to share global code in agena trader.")]
	public class GlobalUtility : AgenaTrader.UserCode.UserIndicator
	{

        

}
#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
}

#endregion

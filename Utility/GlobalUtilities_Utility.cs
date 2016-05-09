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
using System.Collections;
using System.IO;

/// <summary>
/// Version: 1.5
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

    #region Constants

    /// <summary>
        /// Constants to use in Agena Trader Scripts
        /// </summary>
        public static class Const
        {

            //Default Strings
            public const string DefaultStringDatafeedPeriodicity = "Periodicity of your data feed is suboptimal for this indicator!";

            //Default values for indicators
            public const int DefaultOpenRangeSizeinMinutes = 75;

            //Default opacity for drawing
            public const int DefaultOpacity = 70;
            public const int DefaultLineWidth = 2;
            public const int DefaultLineWidth_small = 1;
            public const int DefaultLineWidth_large = 3;
            public static readonly Color DefaultIndicatorColor = Color.Orange;
            public static readonly DashStyle DefaultIndicatorDashStyle = DashStyle.Solid;


        }

    #endregion

    #region GlobalUtilities with global static Helper with functions and methods.


        /// <summary>
        /// Global static Helper with functions and methods.
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

            #endregion

            #region Email

            /// <summary>
            /// True if the email address is valid.
            /// </summary>
            /// <param name="email"></param>
            /// <returns></returns>
            public static bool IsValidEmail(string email)
            {
                if (String.IsNullOrEmpty(email))
                {
                    return false;
                }

                //todo one liner for .net 4.5
                //Note that EmailAddressAttribute is less permissive than System.Net.Mail.MailAddress
                //if (new EmailAddressAttribute().IsValid(email))
                //   return true; 

                //for .net 3.x
                try
                {
                    var addr = new System.Net.Mail.MailAddress(email);
                    return addr.Address == email;
                }
                catch
                {
                    return false;
                }
            }


            #endregion

            #region Markets

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


            #endregion

            #region Bars

            /// <summary>
            /// Returns true if the currentbar is last day in the Bars object.
            /// We use this function to determine if the currentbar is a current active session or if this is the last shown trading day.
            /// </summary>
            /// <param name="bars"></param>
            /// <param name="currentbar"></param>
            /// <returns></returns>
            public static bool IsCurrentBarLastDayInBars(IBars bars, IBar currentbar)
            {
                if (currentbar.Time.Date == bars.Last().Time.Date)
                {
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Returns the first Bar of the latest(=current) Session.
            /// </summary>
            /// <param name="Bars"></param>
            /// <param name="Date"></param>
            /// <returns></returns>
            public static IBar GetFirstBarOfCurrentSession(IBars Bars, DateTime Date)
            {
                return Bars.Where(x => x.Time.Date == Date).FirstOrDefault();
            }

            /// <summary>
            /// Returns the Last Bar of the last Session
            /// During Simulation the latest session could also be a date older than today.
            /// </summary>
            /// <param name="Bars"></param>
            /// <param name="Date"></param>
            /// <returns></returns>
            public static IBar GetLastBarOfLastSession(IBars Bars, DateTime date)
            {
                return Bars.Where(x => x.Time.Date < date).LastOrDefault();
            }

            /// <summary>
            /// HighestHigh Method is not available in Conditions, therefore this alternative can be used
            /// </summary>
            /// <param name="Bars"></param>
            /// <param name="BarsAgo"></param>
            /// <returns></returns>
            public static double GetHighestHigh(IBars Bars, int BarsAgo)
            {
                double HighestHigh = 0;
                for (int i = 1; i <= BarsAgo; i++)

                    if (HighestHigh < Bars[i].High)
                    {
                        HighestHigh = Bars[i].High;
                    }
                return HighestHigh;
            }

            /// <summary>
            /// LowestLow Method is not available in Conditions, therefore this alternative can be used
            /// </summary>
            /// <param name="Bars"></param>
            /// <param name="BarsAgo"></param>
            /// <returns></returns>
            public static double GetLowestLow(IBars Bars, int BarsAgo)
            {
                double LowestLow = 9999999999;
                for (int i = 1; i <= BarsAgo; i++)
                    if (LowestLow > Bars[i].Low)
                    {
                        LowestLow = Bars[i].Low;
                    }
                ;
                return LowestLow;
            }
            #endregion

            #region Files

            /// <summary>
            /// Removes illegal letters on filenames and folders..
            /// </summary>
            /// <param name="fileName"></param>
            /// <returns></returns>
            public static string CleanFileName(string filename)
            {
                filename = filename.Replace(" ", "_");
                filename = filename.Replace(":", "_");
                //filename = filename.Replace(".", "_");
                return Path.GetInvalidFileNameChars().Aggregate(filename, (current, c) => current.Replace(c.ToString(), string.Empty));
                //return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
            }

            #endregion

            #region Chart


            /// <summary>
            /// Saves a snapshot of the current chart.
            /// </summary>
            /// <param name="Indicator"></param>
            /// <param name="InstrumentName"></param>
            /// <param name="Chart"></param>
            /// <param name="Bars"></param>
            /// <param name="TimeFrame"></param>
            public static void SaveSnapShot(string Indicator, String InstrumentName, IChart Chart, IBars Bars, ITimeFrame TimeFrame )
            {
                string filepart = GlobalUtilities.CleanFileName(Indicator + "_" + TimeFrame.PeriodicityValue + TimeFrame.Periodicity + "_" + InstrumentName + "_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm"));
                string directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Auswertung\\" + filepart + "\\";
                System.IO.Directory.CreateDirectory(directory);
                string fileName = InstrumentName + "_" + TimeFrame.PeriodicityValue + TimeFrame.Periodicity + "_" + Bars[0].Time.ToString("yyyy_MM_dd_HH_mm") + ".jpg";
                fileName = GlobalUtilities.CleanFileName(fileName);


                int i = 1;
                int j = 1;
                DateTime ChartStart = DateTime.MinValue;
                DateTime ChartEnd = DateTime.MinValue;

                ChartStart = Bars[0].Time;
                while (i < 100000)
                {
                    if (Bars[i] == null) break; //prüfen, ob BarsAgo existiert
                    
                    if (Bars[i].Time.Date < Bars[0].Time.Date)
                    {
                        j = i;
                        while (j < 100000)
                        {
                            if (Bars[j] == null) break; //prüfen, ob BarsAgo existiert

                            if (Bars[j].Time.Date < Bars[i].Time.Date)
                            {
                                break;
                            }
                            else
                            {
                                ChartStart = Bars[j].Time;
                            }
                            j++;
                        }
                        break;
                    }
                    i++;
                }

                i = -1;
                while (i > -100000)
                {
                    if (Bars[i] == null) break; //prüfen, ob BarsAgo existiert
                    if (Bars[i].Time.Date > Bars[0].Time.Date)
                    {
                        j = i;
                        while (j > -100000)
                        {
                            if (Bars[j] == null) break; //prüfen, ob BarsAgo existiert

                            if (Bars[j].Time.Date > Bars[i].Time.Date)
                            {
                                break;
                            }
                            else
                            {
                                ChartEnd = Bars[j].Time;
                            }
                            j--;
                        }
                        break;
                    }
                    else
                    {
                        ChartEnd = Bars[i].Time;
                    }
                    i--;
                }


                Chart.SetDateRange(ChartStart, ChartEnd);
                Chart.SaveChart(directory + fileName);
            }

            #endregion

            #region DateTimeHelpers taken from  http://www.codeproject.com/Articles/9706/C-DateTime-Library

            public enum Quarter
            {
                First = 1,
                Second = 2,
                Third = 3,
                Fourth = 4
            }

            public enum Month
            {
                January = 1,
                February = 2,
                March = 3,
                April = 4,
                May = 5,
                June = 6,
                July = 7,
                August = 8,
                September = 9,
                October = 10,
                November = 11,
                December = 12
            }

            public static DateTime GetStartOfQuarter(int Year, Quarter Qtr)
            {
                if (Qtr == Quarter.First)    // 1st Quarter = January 1 to March 31
                    return new DateTime(Year, 1, 1, 0, 0, 0, 0);
                else if (Qtr == Quarter.Second) // 2nd Quarter = April 1 to June 30
                    return new DateTime(Year, 4, 1, 0, 0, 0, 0);
                else if (Qtr == Quarter.Third) // 3rd Quarter = July 1 to September 30
                    return new DateTime(Year, 7, 1, 0, 0, 0, 0);
                else // 4th Quarter = October 1 to December 31
                    return new DateTime(Year, 10, 1, 0, 0, 0, 0);
            }

            public static DateTime GetEndOfQuarter(int Year, Quarter Qtr)
            {
                if (Qtr == Quarter.First)    // 1st Quarter = January 1 to March 31
                    return new DateTime(Year, 3, DateTime.DaysInMonth(Year, 3), 23, 59, 59, 999);
                else if (Qtr == Quarter.Second) // 2nd Quarter = April 1 to June 30
                    return new DateTime(Year, 6, DateTime.DaysInMonth(Year, 6), 23, 59, 59, 999);
                else if (Qtr == Quarter.Third) // 3rd Quarter = July 1 to September 30
                    return new DateTime(Year, 9, DateTime.DaysInMonth(Year, 9), 23, 59, 59, 999);
                else // 4th Quarter = October 1 to December 31
                    return new DateTime(Year, 12, DateTime.DaysInMonth(Year, 12), 23, 59, 59, 999);
            }

            public static Quarter GetQuarter(Month Month)
            {
                if (Month <= Month.March)
                    // 1st Quarter = January 1 to March 31
                    return Quarter.First;
                else if ((Month >= Month.April) && (Month <= Month.June))
                    // 2nd Quarter = April 1 to June 30
                    return Quarter.Second;
                else if ((Month >= Month.July) && (Month <= Month.September))
                    // 3rd Quarter = July 1 to September 30
                    return Quarter.Third;
                else // 4th Quarter = October 1 to December 31
                    return Quarter.Fourth;
            }

            public static DateTime GetEndOfLastQuarter()
            {
                if ((Month)DateTime.Now.Month <= Month.March)
                    //go to last quarter of previous year
                    return GetEndOfQuarter(DateTime.Now.Year - 1, Quarter.Fourth);
                else //return last quarter of current year
                    return GetEndOfQuarter(DateTime.Now.Year, GetQuarter((Month)DateTime.Now.Month));
            }

            public static DateTime GetStartOfLastQuarter()
            {
                if ((Month)DateTime.Now.Month <= Month.March)
                    //go to last quarter of previous year
                    return GetStartOfQuarter(DateTime.Now.Year - 1, Quarter.Fourth);
                else //return last quarter of current year
                    return GetStartOfQuarter(DateTime.Now.Year, GetQuarter((Month)DateTime.Now.Month));
            }

            public static DateTime GetStartOfCurrentQuarter()
            {
                return GetStartOfQuarter(DateTime.Now.Year, GetQuarter((Month)DateTime.Now.Month));
            }

            public static DateTime GetEndOfCurrentQuarter()
            {
                return GetEndOfQuarter(DateTime.Now.Year, GetQuarter((Month)DateTime.Now.Month));
            }

            public static DateTime GetStartOfLastWeek()
            {
                int DaysToSubtract = (int)DateTime.Now.DayOfWeek + 7;
                DateTime dt = DateTime.Now.Subtract(System.TimeSpan.FromDays(DaysToSubtract));
                return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, 0);
            }

            public static DateTime GetEndOfLastWeek()
            {
                DateTime dt = GetStartOfLastWeek().AddDays(6);
                return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, 999);
            }

            public static DateTime GetStartOfCurrentWeek()
            {
                int DaysToSubtract = (int)DateTime.Now.DayOfWeek;
                DateTime dt = DateTime.Now.Subtract(System.TimeSpan.FromDays(DaysToSubtract));
                return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, 0);
            }

            public static DateTime GetEndOfCurrentWeek()
            {
                DateTime dt = GetStartOfCurrentWeek().AddDays(6);
                return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, 999);
            }


            public static DateTime GetStartOfMonth(Month Month, int Year)
            {
                return new DateTime(Year, (int)Month, 1, 0, 0, 0, 0);
            }

            public static DateTime GetEndOfMonth(Month Month, int Year)
            {
                return new DateTime(Year, (int)Month, DateTime.DaysInMonth(Year, (int)Month), 23, 59, 59, 999);
            }

            public static DateTime GetStartOfLastMonth()
            {
                if (DateTime.Now.Month == 1)
                    return GetStartOfMonth((Month)12, DateTime.Now.Year - 1);
                else
                    return GetStartOfMonth((Month)DateTime.Now.Month - 1, DateTime.Now.Year);
            }

            public static DateTime GetEndOfLastMonth()
            {
                if (DateTime.Now.Month == 1)
                    return GetEndOfMonth((Month)12, DateTime.Now.Year - 1);
                else
                    return GetEndOfMonth((Month)DateTime.Now.Month - 1, DateTime.Now.Year);
            }

            public static DateTime GetStartOfCurrentMonth()
            {
                return GetStartOfMonth((Month)DateTime.Now.Month, DateTime.Now.Year);
            }

            public static DateTime GetEndOfCurrentMonth()
            {
                return GetEndOfMonth((Month)DateTime.Now.Month, DateTime.Now.Year);
            }

            public static DateTime GetStartOfYear(int Year)
            {
                return new DateTime(Year, 1, 1, 0, 0, 0, 0);
            }

            public static DateTime GetEndOfYear(int Year)
            {
                return new DateTime(Year, 12, DateTime.DaysInMonth(Year, 12), 23, 59, 59, 999);
            }

            public static DateTime GetStartOfLastYear()
            {
                return GetStartOfYear(DateTime.Now.Year - 1);
            }

            public static DateTime GetEndOfLastYear()
            {
                return GetEndOfYear(DateTime.Now.Year - 1);
            }

            public static DateTime GetStartOfCurrentYear()
            {
                return GetStartOfYear(DateTime.Now.Year);
            }

            public static DateTime GetEndOfCurrentYear()
            {
                return GetEndOfYear(DateTime.Now.Year);
            }

            public static DateTime GetStartOfDay(DateTime date)
            {
                return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
            }

            public static DateTime GetEndOfDay(DateTime date)
            {
                return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
            }

            #endregion

        }

        #endregion


   #region Global defined classes

    /// <summary>
    /// Statistic object to compare strategies.
    /// </summary>
    public class Statistic {

        public Statistic(string nameofthestrategy) {
            this.NameOfTheStrategy = nameofthestrategy;
        }

        /// <summary>
        /// Returns a string with csv data.
        /// </summary>
        /// <returns></returns>
        public string getCSVData()
        {
            return string.Format("{0},{1},{2},{3},{4},{5}", this.NameOfTheStrategy, "Long or Short", this.EntryDateTime.ToString(), this.ExitDateTime.ToString(), this.MinutesInMarket, this.Instrument);
        }

        #region Properties

        private string _nameofthestrategy = null;
        public string NameOfTheStrategy
        {
            get { return _nameofthestrategy; }
            set { _nameofthestrategy = value; }
        }

        private DateTime _entrydatetime = DateTime.MinValue;

        public DateTime EntryDateTime
        {
            get { return _entrydatetime; }
            set { _entrydatetime = value; }
        }

        private DateTime _exitdatetime = DateTime.MinValue;

        public DateTime ExitDateTime
        {
            get { return _exitdatetime; }
            set { _exitdatetime = value; }
        }

        public double MinutesInMarket {
            get {
                return this.ExitDateTime.Subtract(this.EntryDateTime).TotalMinutes;
            }
        }


        private string _instrument = String.Empty;

        public string Instrument
        {
            get { return _instrument; }
            set { _instrument = value; }
        }


        #endregion


    }


    /// <summary>
    /// We use this class to compare two IBar if they are equal on time.
    /// true when the time of the IBar is the same on x as on y.
    /// </summary>
    public class IBarTimeComparer : IEqualityComparer<IBar>
    {
        bool IEqualityComparer<IBar>.Equals(IBar x, IBar y)
        {
            return (x.Time.Equals(y.Time) && x.Time.Equals(y.Time));
        }

        int IEqualityComparer<IBar>.GetHashCode(IBar obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return obj.Time.GetHashCode() + obj.Time.GetHashCode();
        }
    }

    #endregion

}

/// <summary>
/// This class contains all extension methods for IEnumerable.
/// </summary>
public static class IEnumerableExtensions {

    public static IEnumerable<DateTime> GetDateRange(this DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
            throw new ArgumentException("endDate must be greater than or equal to startDate");

        while (startDate <= endDate)
        {
            yield return startDate;
            startDate = startDate.AddDays(1);
        }
    }

    public static IEnumerable Append(this IEnumerable first, params object[] second)
    {
        return first.OfType<object>().Concat(second);
    }
    public static IEnumerable<T> Append<T>(this IEnumerable<T> first, params T[] second)
    {
        return first.Concat(second);
    }
    public static IEnumerable Prepend(this IEnumerable first, params object[] second)
    {
        return second.Concat(first.OfType<object>());
    }
    public static IEnumerable<T> Prepend<T>(this IEnumerable<T> first, params T[] second)
    {
        return second.Concat(first);
    }



}

/// <summary>
/// This class contains all extension methods for strings.
/// </summary>
public static class StringExtensions {

    /// <summary>
    /// Checks if the string contains a numeric value.
    /// </summary>
    /// <param name="str">The string to be tested.</param>
    /// <returns>True if the string is numeric.</returns>
    public static bool IsNumeric(this string str)
    {
        long number;
        return long.TryParse(str, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo, out number);
    }

    /// <summary>
    /// Counts the words in a string.
    /// </summary>
    /// <param name="str">The string to be tested.</param>
    /// <returns>The number of words.</returns>
    public static int WordCount(this String str)
    {
        return str.Split(new char[] { ' ', '.', '?' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }
}


	[Description("We use this indicator to share global code in agena trader.")]
	public class GlobalUtility : AgenaTrader.UserCode.UserIndicator
	{
        //https://www.youtube.com/watch?v=5NNOrp_83RU
    }

#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
}

#endregion



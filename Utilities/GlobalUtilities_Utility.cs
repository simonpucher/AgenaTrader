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
using System.Text;
using AgenaTrader.Plugins.MoneyHandler;
using System.Threading;
using System.Windows.Forms;
using System.Data.SqlTypes;
using System.Linq.Expressions;
using System.Net;
using System.Globalization;

/// <summary>
/// Version: 2.0.1
/// -------------------------------------------------------------------------
/// Simon Pucher 2018
/// Christian Kovar 2018
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
        //Default Files
        public static readonly string DefaultFileStatistic = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Evaluation\\" + "Evaluation.csv";

        //Default Strings
        public const string DefaultStringDatafeedPeriodicity = "Periodicity of your data feed is suboptimal for this indicator!";
        public const string DefaultStringDatafeedBarsRequired = "Please increase bars required!";
        public const string DefaultStringDatafeedBarsRequiredCount = "Please increase bars required property to a minimum of {0} bars!";
        public const string DefaultStringErrorDuringCalculation = "A problem has occured during the calculation method!";
        public const string DefaultExitReasonEOD = "End of day";

        //Default values for indicators
        public const int DefaultOpenRangeSizeinMinutes = 60;

        //Default opacity for drawing
        public const int DefaultOpacity = 70;
        public const int DefaultLineWidth = 2;
        public const int DefaultLineWidth_small = 1;
        public const int DefaultLineWidth_large = 3;
        public static readonly Color DefaultArrowLongColor = Color.Lime;
        public static readonly Color DefaultArrowLong2Color = Color.Green;
        public static readonly Color DefaultArrowShortColor = Color.Tomato;
        public static readonly Color DefaultArrowShort2Color = Color.Red;
        public static readonly Color DefaultIndicatorColor = Color.Orange;
        public static readonly Color DefaultIndicatorColor_GreyedOut = Color.Gray;
        public static readonly DashStyle DefaultIndicatorDashStyle = DashStyle.Solid;

        //Fibonacci Retracements
        public const decimal DefaultFibonacciRetracement23_6 = 23.6m;
        public const decimal DefaultFibonacciRetracement38_2 = 38.2m;
        public const decimal DefaultFibonacciRetracement50 = 50;
        public const decimal DefaultFibonacciRetracement61_8 = 61.8m;
        public const decimal DefaultFibonacciRetracement100 = 100;

        //Fibonacci Extensions
        public const decimal DefaultFibonacciExtension161_8 = 161.8m;
        public const decimal DefaultFibonacciExtension200 = 200;
        public const decimal DefaultFibonacciExtension261_8 = 261.8m;
        public const decimal DefaultFibonacciExtension423_6 = 423.6m;

    }

    #endregion


    #region Global static Helper with functions and methods.

    /// <summary>
    /// Default Soundfiles by AT 
    /// </summary>
    public enum Soundfile
    {
        Alert1,
        Alert2,
        Alert3,
        Alert4,
        Announcement,
        AutoBreakEven,
        AutoChase,
        AutoTrail,
        Blip,
        Buzz,
        Camera,
        cashreg,
        Connected,
        ConnectionLost,
        Limit_order_filled,
        New,
        notify,
        OffsettedChange,
        OrderCancelled,
        OrderFilled,
        OrderPending,
        PositionClosed,
        Reversing,
        ringin,
        ringout,
        SOUND136,
        StopFilled,
        Stop_limit_order_filled,
        Stop_order_filled,
        TargetFilled,
        WMonline
    }

    /// <summary>
    /// Global static Helper with functions and methods.
    /// </summary>
    public static class GlobalUtilities
    {

        #region Sounds

        /// <summary>
        /// Return the filename of the Soundfile.
        /// </summary>
        /// <param name="soundfile"></param>
        /// <returns></returns>
        public static string GetSoundfile(Soundfile soundfile) {
            string returnfilename = null;
            returnfilename = soundfile.ToString().Replace("_", "-");
            returnfilename = returnfilename + ".wav";
            return returnfilename;
        }

        #endregion

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

        ///// <summary>
        ///// Returns the standard subject for Entry Signal Emails
        ///// </summary>
        ///// <param name="execution"></param>
        ///// <returns></returns>
        //public static string GetEmailSubjectEntrySignal(IInstrument instrument, OrderDirection orderaction)
        //{
        //    return "Entry Signal " + instrument.Symbol + " " + orderaction.ToString();
        //}

        ///// <summary>
        ///// Returns the standard text for Entry Signal Emails
        ///// </summary>
        ///// <param name="execution"></param>
        ///// <param name="strategyname"></param>
        ///// <returns></returns>
        //public static string GetEmailBodyEntrySignal(IInstrument instrument, OrderDirection orderaction, string strategyname, TimeFrame timeframe)
        //{
        //    StringBuilder str = new StringBuilder();

        //    str.AppendLine("Strategy: " + strategyname);
        //    str.AppendLine("Instrument: " + instrument.Name);
        //    str.AppendLine("OrderAction: " + orderaction.ToString());
        //    str.AppendLine("TimeFrame: " + timeframe.ToString());

        //    return str.ToString();
        //}

        /// <summary>
        /// Returns the standard subject for emails on order execution.
        /// </summary>
        /// <param name="execution"></param>
        /// <returns></returns>
        public static string GetEmailSubject(IExecution execution)
        {
            return execution.Instrument.Symbol + " Order " + execution.PositionType.ToString() + " executed";
        }

        /// <summary>
        /// Returns the standard text for emails on order execution.
        /// </summary>
        /// <param name="execution"></param>
        /// <param name="strategyname"></param>
        /// <returns></returns>
        public static string GetEmailText(IExecution execution, string strategyname)
        {
            StringBuilder str = new StringBuilder();

            str.AppendLine("Strategy: " + strategyname);
            str.AppendLine("Order " + execution.Name + " on instrument " + execution.Instrument.Name + " was executed.");
            str.AppendLine("Position: " + execution.PositionType.ToString());
            str.AppendLine("Quantity: " + (execution.Order.Quantity).ToString("F2"));
            str.AppendLine("Price: " + (execution.Order.Price).ToString("F2"));
            str.AppendLine("Investment: " + (execution.Order.Quantity * execution.Order.Price).ToString("F2"));

            return str.ToString();
        }


        #endregion

        #region Markets

        /// <summary>
        /// Changes money from one currency into another.
        /// </summary>
        /// <param name="cashamount"></param>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static decimal MoneyExchange(double cashamount, Currencies current, Currencies target) {
            return new Money(cashamount, current).ConvertToCurrency(target).RoundedAmount;
        }

        /// <summary>
        /// Calculates the position size regarding to risk management.
        /// </summary>
        /// <param name="instrument"></param>
        /// <returns></returns>
        public static int AdjustPositionToRiskManagement(IAccountManager accountmanager, IPreferenceManager preferencemanager, IInstrument instrument, double lastprice)
        {
            //Get Risk Management from Account
            IAccount account = accountmanager.GetAccount(instrument, true);
            if (account == null)
            {
                return instrument.GetDefaultQuantity();
            }

            //get the RiskParams on this account connection
            ConnectionRiskParams crp = preferencemanager.GetConnectionRiskParams(account.AccountConnection.ConnectionName);
            InstrumentRiskParams irp = crp.InstrumentRiskParams[instrument.InstrumentType];

            double maxpositionsizeincash = 0.0;
            if (irp.BasePositionSizing == BasePositionSizing.OnAmountPerPosition)
            {
                maxpositionsizeincash = irp.InvestedAmountPerPosition;
            }
            else if (irp.BasePositionSizing == BasePositionSizing.OnRiskAmountPerTrade)
            {
                maxpositionsizeincash = account.CashValue / 100 * irp.MaxInvestedAmountPercentage;
            }
            else if (irp.BasePositionSizing == BasePositionSizing.OnInitialRisk)
            {
                //hack: because of backtesting
                maxpositionsizeincash = account.CashValue / 100 * irp.MaxInvestedAmountPercentage;
            }
            else
            {
                throw new NotImplementedException("AdjustPositionToRiskManagement: BasePositionSizing " + irp.BasePositionSizing.ToString() + " not implemented", null);
            }

            //Check the type of instrument & return the position size
            if (instrument.InstrumentType == InstrumentType.Index)
            {
                //return 1;
                return instrument.GetDefaultQuantity();
            }
            if (instrument.InstrumentType == InstrumentType.Stock
             || instrument.InstrumentType == InstrumentType.CFD)
            {
                return (int)Math.Floor(decimal.ToDouble(MoneyExchange(maxpositionsizeincash, account.Currency, instrument.Currency)) / lastprice);
            }
            else if (instrument.InstrumentType == InstrumentType.CFD)
            {
                return instrument.GetDefaultQuantity();
            }
            else
            {
                throw new NotImplementedException("AdjustPositionToRiskManagement: InstrumentType " + instrument.InstrumentType.ToString() + " not implemented", null);
            }

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



        #endregion

        #region web harvester

        private static decimal _vdax_new_value = 0;
        private static DateTime? _vdax_new_lastcheck = null;
        //Opening hours of VDAX are between: 08:50 - 17:50
        private static TimeSpan _vdax_new_openinghours_open = new TimeSpan(8, 50, 0);
        private static TimeSpan _vdax_new_openinghours_close = new TimeSpan(17, 50, 0);
        private static TimeSpan _vdax_new_openinghours_marketdelay = new TimeSpan(0, 15, 0);

        /// <summary>
        /// dirty http request for the current VDAX-New value from the onvista website
        /// parsing is pretty dirty but does the job. anyway, expect some improvements or
        /// adaption as soon as the onvista site is changing :D
        /// </summary>
        /// <returns>vdax-new value</returns>
        public static decimal GetCurrentVdaxNew(int _CheckEveryXSeconds)
        {
            bool checkonline = false;

            //Check if we have not done this yet.
            if (_vdax_new_lastcheck == null)
            {
                checkonline = true;
            }
            //Check the online service each x seconds
            else if (_vdax_new_lastcheck.Value.AddSeconds(_CheckEveryXSeconds) <= DateTime.Now)
            {
                TimeSpan now = DateTime.Now.TimeOfDay;
                //If the market is closed we do not need to ask the online service
                if ((now >= _vdax_new_openinghours_open) && (now <= _vdax_new_openinghours_close.Add(_vdax_new_openinghours_marketdelay)))
                {
                    checkonline = true;
                }
            }

            //If true we check online for the data
            if (checkonline)
            {
                string url = "http://www.onvista.de/index/VDAX-NEW-Index-12105789";

                //Anfrage an die �bergebene URL starten
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

                //Antwort-Objekt erstellen
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Antwort Stream an Streamreader �bergeben
                StreamReader sr = new StreamReader(response.GetResponseStream());

                //Antwort (HTML Code) auslesen
                string html = sr.ReadToEnd();

                //Streamreader und Webanfrage schlie�en
                sr.Close();
                response.Close();


                int first = html.IndexOf("<div id=\"Index-Header\">");

                int EndOfValue = html.IndexOf("</span>", first);

                string _vdax_new_string_value = (html.Substring(EndOfValue - 6, 5));

                _vdax_new_string_value = _vdax_new_string_value.Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
                _vdax_new_value =  decimal.Parse(_vdax_new_string_value, NumberStyles.Any, CultureInfo.InvariantCulture);

                _vdax_new_lastcheck = DateTime.Now;
            }
            
            return _vdax_new_value;

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

        /// <summary>
        /// Return the target Bar, meaning a certain Bar in the future based on given timeFrame parameter
        /// eg getting 5 Bars in Future, this could be 5 days, or 75 Minutes in a 15M Chart
        /// </summary>
        /// <param name="Bars"></param>
        /// <param name="CurrentBarDateTime"></param>
        /// <param name="timeFrame"></param>
        /// <param name="BarsTilTarget"></param>
        /// <returns></returns>
        public static DateTime GetTargetBar(IBars Bars, DateTime CurrentBarDateTime, ITimeFrame timeFrame, int BarsTilTarget)
        {
            DateTime Target = CurrentBarDateTime;
            int i = 0;
            do
            {
                switch (timeFrame.Periodicity)
                {
                    case DatafeedHistoryPeriodicity.Minute:
                        switch (timeFrame.PeriodicityValue)
                        {
                            case 1:
                                Target = Target.AddMinutes(1);
                                break;
                            case 5:
                                Target = Target.AddMinutes(5);
                                break;
                            case 15:
                                Target = Target.AddMinutes(15);
                                break;
                            case 30:
                                Target = Target.AddMinutes(30);
                                break;
                        }
                        break;
                    case DatafeedHistoryPeriodicity.Hour:
                        switch (timeFrame.PeriodicityValue)
                        {
                            case 1:
                                Target = Target.AddHours(1);
                                break;
                            case 5:
                                Target = Target.AddHours(4);
                                break;
                        }
                        break;

                    case DatafeedHistoryPeriodicity.Day:
                        Target = Target.AddDays(1);
                        break;

                    case DatafeedHistoryPeriodicity.Week:
                        Target = Target.AddDays(7);
                        break;
                    default:
                        return DateTime.MinValue;
                }

                if (Bars.IsDateTimeInMarket(Target) == true)
                {
                    i++;
                }


            } while (i < BarsTilTarget);
            return Target;
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
        /// Draws a standard alert text in the chart.
        /// </summary>
        /// <param name="indicator"></param>
        /// <param name="text"></param>
        public static void DrawAlertTextOnChart(UserIndicator indicator, string text)
        {
            indicator.AddChartTextFixed("AlertText", text, TextPosition.Center, Color.Red, new Font("Arial", 30), Color.Red, Color.Red, 20);
        }

        /// <summary>
        /// Draws a standard warning text in the chart.
        /// </summary>
        /// <param name="indicator"></param>
        /// <param name="text"></param>
        public static void DrawWarningTextOnChart(UserIndicator indicator, string text)
        {
            indicator.AddChartTextFixed("WarningText", text, TextPosition.Center, Color.Goldenrod, new Font("Arial", 30), Color.Goldenrod, Color.Goldenrod, 20);
        }

        /// <summary>
        /// Saves a snapshot of the current chart.
        /// </summary>
        /// <param name="Indicator"></param>
        /// <param name="InstrumentName"></param>
        /// <param name="Chart"></param>
        /// <param name="Bars"></param>
        /// <param name="TimeFrame"></param>
        public static void SaveSnapShot(string Indicator, String InstrumentName, IEnumerable<IChart> AllCharts, IBars Bars, ITimeFrame TimeFrame)
        {

            string filepart = GlobalUtilities.CleanFileName(Indicator + "_" + TimeFrame.PeriodicityValue + TimeFrame.Periodicity + "_" + InstrumentName + "_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm"));
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Auswertung\\SnapShot\\" + filepart + "\\";
            string fileName = InstrumentName + "_" + TimeFrame.PeriodicityValue + TimeFrame.Periodicity + "_" + Bars[0].Time.ToString("yyyy_MM_dd_HH_mm") + ".jpg";
            fileName = GlobalUtilities.CleanFileName(fileName);

            //take 20 Bars to future and 20 Bars from the past (but only if they exist, if not, just take first or last)
            DateTime ChartStart = DateTime.MinValue;
            DateTime ChartEnd = DateTime.MinValue;

            if (Bars.GetByIndex(Bars.GetBarIndex(Bars[0]) + 20) != null)
            {
                ChartEnd = Bars.GetByIndex(Bars.GetBarIndex(Bars[0]) + 20).Time;
            }
            else
            {
                ChartEnd = Bars.Last().Time;
            }

            if (Bars.GetByIndex(Bars.GetBarIndex(Bars[0]) - 20) != null)
            {
                ChartStart = Bars.GetByIndex(Bars.GetBarIndex(Bars[0]) - 20).Time;
            }
            else
            {
                ChartStart = Bars.First().Time;
            }

            //pick the right chart matching the current timeframe
            foreach (IChart chart in AllCharts)
            {
                if (chart.HistoryRequest.TimeFrame.Periodicity == TimeFrame.Periodicity
                 && chart.HistoryRequest.TimeFrame.PeriodicityValue == TimeFrame.PeriodicityValue
                 && chart.Instrument.Name == Bars.Instrument.Name)
                {
                    System.IO.Directory.CreateDirectory(directory);
                    chart.SetDateRange(ChartStart, ChartEnd);
                    chart.SaveChart(directory + fileName);
                }
            }
        }

        #endregion

        #region Basic Calculation

        static public decimal getPercentage(double whole, double part)
        {
            if (whole != 0) {
            return (decimal)((part / whole) * 100);
            } else
	{
                return 0;
	}
        }

        static public double getMinPosNumber(double n1, double n2, double n3)
        {
            List<double> numbers = new List<double>();

            numbers.Add(n1);
            numbers.Add(n2);
            numbers.Add(n3);

            numbers.Sort();
            numbers.RemoveAll(x => x <= 0);
            return numbers.Min();

            
        }

        static public double getMaxNumber(double n1, double n2, double n3)
        {
            List<double> numbers = new List<double>();

            numbers.Add(n1);
            numbers.Add(n2);
            numbers.Add(n3);
            return numbers.Max();
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


        #region code utility

        // <summary>
        // Get the name of a static or instance property from a property access lambda.
        // Taken from: http://stackoverflow.com/questions/2820660/get-name-of-property-as-a-string
        // </summary>
        // <typeparam name="T">Type of the property</typeparam>
        // <param name="propertyLambda">lambda expression of the form: '() => Class.Property' or '() => object.Property'</param>
        // <returns>The name of the property</returns>

        public static string GetPropertyName<T>(Expression<Func<T>> propertyLambda)
        {
            var me = propertyLambda.Body as MemberExpression;

            if (me == null)
            {
                throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
            }

            return me.Member.Name;
        }

        #endregion

    }

    #endregion

    #region Global defined classes

    /// <summary>
    /// Different types of order directions.s
    /// </summary>
    public enum OrderDirection_Enum : byte
    {
        OpenLong = 1,
        OpenShort = 2,
        CloseLong = 3,
        CloseShort = 4
    }

    /// <summary>
    /// Class which holds all important data like the OrderDirection. 
    /// We use this object as a global default return object for the calculate method in indicators.
    /// </summary>
    public class ResultValue
    {
        public bool ErrorOccured = false;
        public OrderDirection? Entry = null;
        public OrderDirection? Exit = null;
    }

    /// <summary>
    /// Statistic object  to compare the performance of strategies.
    /// </summary>
    public class StatisticContainer {

        private List<Statistic> List = null;

        public StatisticContainer() {
            this.List = new List<Statistic>();
        }

        /// <summary>
        /// Add a execution to our statistic (e.g. for backtesting).
        /// </summary>
        /// <param name="tradingmanager"></param>
        /// <param name="nameofthestrategy"></param>
        /// <param name="execution"></param>
        public void Add(ITradingManager tradingmanager, IStrategy strategy, IExecution execution)
        {
            Statistic statistic = new Statistic(tradingmanager, strategy, execution);
            //Set the counter, we are starting at 0
            statistic.Counter = this.List.Count;
            //Add the item to the list
            this.List.Add(statistic); 
        }


        /// <summary>
        /// Add a statistic obect (e.g. for backtesting).
        /// </summary>
        /// <param name="tradingmanager"></param>
        /// <param name="nameofthestrategy"></param>
        /// <param name="execution"></param>
        public void Add(Statistic statistic)
        {
            //Set the counter, we are starting at 0
            statistic.Counter = this.List.Count;
            //Add the item to the list
            this.List.Add(statistic);
        }


        /// <summary>
        /// Copy the statistic csv file into the clipboard
        /// </summary>
        public void copyToClipboard()
        {
            try
            {
                string csvdata = this.getCSVData();
                if (!String.IsNullOrEmpty(csvdata))
                {
                    //Copy the csv data into clipboard
                    Thread thread = new Thread(() => Clipboard.SetText(csvdata));
                    //Set the thread to STA
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                    thread.Join();
                }
            }
            catch (Exception)
            {
                //todo log it
            }
        }

        /// <summary>
        /// Returns csv Data
        /// </summary>
        /// <param name="copytoclipboard"></param>
        /// <returns></returns>
        public string getCSVData()
        {
            StringBuilder returnvalue = new StringBuilder();
            if (this.List != null && this.List.Count > 0)
            {
                returnvalue.AppendLine(Statistic.getCSVDataHeader());
                foreach (Statistic item in this.List)
                {
                    returnvalue.AppendLine(item.getCSVData());
                }
            }
            return returnvalue.ToString();
        }
    }

    /// <summary>
    /// Statistic object for each order.
    /// </summary>
    public class Statistic
    {

        //todo remove this method
        [Obsolete("Please use another constructor", false)]
        public Statistic(string nameofthestrategy)
        {
            this.NameOfTheStrategy = nameofthestrategy;
        }

        /// <summary>
        /// Standard constructor with finalised executions (all data from trade and order is available).
        /// You should use this when you create statistic data during a backtest when you are using executions.
        /// </summary>
        /// <param name="tradingmanager"></param>
        /// <param name="nameofthestrategy"></param>
        /// <param name="execution"></param>
        public Statistic(ITradingManager tradingmanager, IStrategy strategy, IExecution execution)
        {
            //Logging only on flat transactions then we have all data available (entry & exit)
            if (execution.PositionType == PositionType.Flat)
            {
                //get the trade with all data
                int tradeid = tradingmanager.GetTradeIdByExecutionId(execution.ExecutionId);
                ITradingTrade trade = tradingmanager.GetTrade(tradeid);

                if (trade != null)
                {
                    //Log all data
                    this.NameOfTheStrategy = strategy.DisplayName;
                    this.Instrument = execution.Instrument.ToString();
                    this.TradeDirection = trade.EntryOrder.IsLong ? PositionType.Long : PositionType.Short;
                    this.TimeFrame = execution.Order.TimeFrame.ToString();
                    this.ProfitLoss = trade.ProfitLoss;
                    this.ProfitLossPercent = trade.ProfitLossPercent; 
                    this.ExitReason = trade.ExitReason;
                    this.ExitPrice = trade.ExitPrice;
                    //this.ExitDateTime = execution.Time;
                    this.ExitDateTime = execution.Timestamp;
                    this.ExitQuantity = execution.Quantity;
                    this.ExitOrderType = execution.Order.OrderType;
                    this.EntryDateTime = trade.EntryOrder.CreationTime;
                    this.EntryPrice = trade.EntryOrder.Price;
                    this.EntryQuantity = trade.EntryOrder.Quantity;
                    this.EntryOrderType = trade.EntryOrder.Type;
                    this.StopPrice = execution.Order.StopPrice;

                    //everything is fine
                    this.IsValid = true;
                }
                
                //we do not have a target.
                //this.TargetPrice   
            }
        }



        /// <summary>
        /// Standard constructor with all statistical trades.
        /// You should use this when you create statistic data during a backtest with parameter optimization.
        /// In this case you also need to use the methods SetEntry() and SetExit();
        /// </summary>
        /// <param name="tradingmanager"></param>
        /// <param name="nameofthestrategy"></param>
        /// <param name="execution"></param>
        public Statistic(IStrategy strategy, PositionType positiontype)
        {
           //Log all data
            //todo talk to christian concerning the unused properties and StopPrice
            this.NameOfTheStrategy = strategy.DisplayName;
            this.Instrument = strategy.Instrument.ToString();
            this.TradeDirection = positiontype;
            this.TimeFrame = strategy.TimeFrame.ToString();
            //this.ProfitLoss = trade.ProfitLoss;
            //this.ProfitLossPercent = trade.ProfitLossPercent;
            //this.ExitReason = trade.ExitReason;
           
            //this.StopPrice = execution.Order.StopPrice;

            //everything is fine
            this.IsValid = true;
        }


        public void SetEntry(string entryreason, int entry_quantity, double entry_price, DateTime entry_datetime, OrderType entry_ordertype) {
            this.EntryReason = entryreason;
            this.EntryDateTime = entry_datetime;
            this.EntryPrice = entry_price;
            this.EntryQuantity = entry_quantity;
            this.EntryOrderType = entry_ordertype;
        }

        public void SetExit(string exitreason, int exit_quantity, double exit_price, DateTime exit_datetime, OrderType exit_ordertype) {
            this.ExitReason = exitreason;
            this.ExitPrice = exit_price;
            this.ExitDateTime = exit_datetime;
            this.ExitQuantity = exit_quantity;
            this.ExitOrderType = exit_ordertype;
        }

        /// <summary>
        /// Returns a string with csv header text.
        /// ATTENTION: If you change parameters in this method, please also change it in the getCSVData()
        /// </summary>
        /// <returns></returns>
        public static string getCSVDataHeader()
        {
            return "Id;Strategy;Instrument;TimeFrame;TradeDirection;EntryReason;EntryDateTime;EntryPrice;EntryQuantity;EntryOrderType;ExitDateTime;ExitPrice;MinutesInMarket;ExitReason;ExitQuantity;ExitOrderType;PointsDiff;PointsDiffPerc;ProfitLoss;ProfitLossPercent;StopPrice;TargetPrice";
        }


        /// <summary>
        /// Returns a string with csv data.
        /// ATTENTION: If you change parameters in this method, please also change it in the getCSVDataHeader()
        /// </summary>
        /// <returns></returns>
        public string getCSVData()
        {
            return string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};{21}",
                                            this.Counter.ToString(),
                                            this.NameOfTheStrategy,
                                            this.Instrument,
                                            this.TimeFrame,
                                            this.TradeDirection.ToString(),
                                            this.EntryReason,
                                            this.EntryDateTime.ToString(),
                                            this.EntryPrice,
                                            this.EntryQuantity,
                                            this.EntryOrderType,
                                            this.ExitDateTime.ToString(),
                                            this.ExitPrice,
                                            this.MinutesInMarket,
                                            this.ExitReason,
                                            this.ExitQuantity,
                                            this.ExitOrderType,
                                            this.PointsDiff,
                                            this.PointsDiffPercentage,
                                            this.ProfitLoss,
                                            this.ProfitLossPercent,
                                            this.StopPrice,
                                            this.TargetPrice                                            
                                            );
        }

        /// <summary>
        /// Append csv data to a file
        /// </summary>
        public void AppendToFile()
        {
            string File = Const.DefaultFileStatistic;
            FileInfo fi = new FileInfo(File);
            if (fi.Exists == false)
            {
                using (StreamWriter stream = new StreamWriter(File)) {
                    stream.WriteLine(getCSVDataHeader());
                }
            }
            using (StreamWriter stream = new FileInfo(File).AppendText())
            {
                stream.WriteLine(getCSVData());
            }
        }

        #region Properties


        private int _counter = 0;
        public int Counter
        {
            get { return _counter; }
            set { _counter = value; }
        }

    
            private bool _IsValid = false;
            public bool IsValid
            {
                get { return _IsValid; }
                set { _IsValid = value; }
            }

    

        private string _nameofthestrategy = null;
        public string NameOfTheStrategy
        {
            get { return _nameofthestrategy; }
            set { _nameofthestrategy = value; }
        }


        private string _TimeFrame = string.Empty;

        public string TimeFrame
        {
            get { return _TimeFrame; }
            set { _TimeFrame = value; }
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

        private OrderType _EntryOrderType = OrderType.Unknown;

        public OrderType EntryOrderType
        {
            get { return _EntryOrderType; }
            set { _EntryOrderType = value; }
        }

        private OrderType _ExitOrderType = OrderType.Unknown;

        public OrderType ExitOrderType
        {
            get { return _ExitOrderType; }
            set { _ExitOrderType = value; }
        }

        public double MinutesInMarket
        {
            get
            {
                return this.ExitDateTime.Subtract(this.EntryDateTime).TotalMinutes;
            }
        }

        public double PointsDiff
        {
            get {
                    //if (TradeDirection == Const.strLong)
                    if (TradeDirection == PositionType.Long)
                    {
                        return ExitPrice - EntryPrice;
                    }
                    else
                    {
                        return EntryPrice - ExitPrice;
                    }
              }
        }

        public double PointsDiffPercentage
        {
            get
            {
                if (TradeDirection == PositionType.Long)
                {
                    return 1-(EntryPrice / ExitPrice );
                }
                else
                {
                    return 1-(ExitPrice/EntryPrice);
                }
            }
        }

        private string _instrument = String.Empty;

        public string Instrument
        {
            get { return _instrument; }
            set { _instrument = value; }
        }

        private PositionType _TradeDirection = PositionType.Flat;

        public PositionType TradeDirection
        {
            get { return _TradeDirection; }
            set { _TradeDirection = value; }
        }

        private double _EntryPrice = Double.MinValue;

        public double EntryPrice
        {
            get { return _EntryPrice; }
            set { _EntryPrice = value; }
        }

        private int _EntryQuantity = 0;

        public int EntryQuantity
        {
            get { return _EntryQuantity; }
            set { _EntryQuantity = value; }
        }

        

        private double _ExitPrice = Double.MinValue;

        public double ExitPrice
        {
            get { return _ExitPrice; }
            set { _ExitPrice = value; }
        }

        private String _EntryReason = String.Empty;

        public String EntryReason
        {
            get { return _EntryReason; }
            set { _EntryReason = value; }
        }


        private String _ExitReason = String.Empty;

        public String ExitReason
        {
            get { return _ExitReason; }
            set { _ExitReason = value; }
        }

        private int _ExitQuantity = 0;

        public int ExitQuantity
        {
            get { return _ExitQuantity; }
            set { _ExitQuantity = value; }
        }

        private double _ProfitLoss = 0;

        public double ProfitLoss
        {
            get { return _ProfitLoss; }
            set { _ProfitLoss = value; }
        }

        private double _ProfitLossPercent = 0;
         public double ProfitLossPercent
        {
            get { return _ProfitLossPercent; }
            set { _ProfitLossPercent = value; }
        }


        private double _StopPrice = 0;

        public double StopPrice
        {
            get { return _StopPrice; }
            set { _StopPrice = value; }
        }

        private double _TargetPrice = 0;

        public double TargetPrice
        {
            get { return _TargetPrice; }
            set { _TargetPrice = value; }
        }
        #endregion


    }




    /// <summary>
    /// Generic CSVExport (e.g. flat file for MatLab import)
    /// GitHub Fork https://gist.github.com/simonpucher/80b5ed382432ec14bd13708a7b984564
    /// </summary>
    /// <example>
    ///   CsvExport myExport = new CsvExport();
    ///
    ///   myExport.AddRow();
    ///   myExport["Region"] = "New York, USA";
    ///   myExport["Sales"] = 100000;
    ///   myExport["Date Opened"] = new DateTime(2003, 12, 31);
    ///
    ///   myExport.AddRow();
    ///   myExport["Region"] = "Sydney \"in\" Australia";
    ///   myExport["Sales"] = 50000;
    ///   myExport["Date Opened"] = new DateTime(2005, 1, 1, 9, 30, 0);
    ///
    /// Then you can do any of the following three output options:
    ///   string myCsv = myExport.Export();
    ///   myExport.ExportToFile("Somefile.csv");
    ///   byte[] myCsvData = myExport.ExportToBytes();
    /// </example>
    public class CsvExport
    {
        /// <summary>
        /// To keep the ordered list of column names
        /// </summary>
        List<string> fields = new List<string>();

        /// <summary>
        /// The list of rows
        /// </summary>
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

        /// <summary>
        /// The current row
        /// </summary>
        Dictionary<string, object> currentRow { get { return rows[rows.Count - 1]; } }

        /// <summary>
        /// Set a value on this column
        /// </summary>
        public object this[string field]
        {
            set
            {
                // Keep track of the field names, because the dictionary loses the ordering
                if (!fields.Contains(field)) fields.Add(field);
                currentRow[field] = value;
            }
        }

        /// <summary>
        /// Call this before setting any fields on a row
        /// </summary>
        public void AddRow()
        {
            rows.Add(new Dictionary<string, object>());
        }

        /// <summary>
        /// This method add the basic data rows for matlab import.
        /// Please do not change the Keys because we need this structure for matlab.
        /// </summary>
        public void AddRowBasicData(IStrategy strategy, IInstrument instrument, ITimeFrame timeframe, IBar bar, int ProcessingBarIndex)
        {
            this["Strategy"] = strategy.DisplayName;
            this["TimeFrame"] = timeframe.ToString();
            this["Instrument"] = instrument.Name;
            this[GlobalUtilities.GetPropertyName(() => instrument.MainSector)] = instrument.MainSector;
            this[GlobalUtilities.GetPropertyName(() => instrument.DetailSector)] = instrument.DetailSector;
            this[GlobalUtilities.GetPropertyName(() => instrument.Currency)] = instrument.Currency;
            this["CurrentBar"] = ProcessingBarIndex;
            this[GlobalUtilities.GetPropertyName(() => bar.Time)] = bar.Time;
   
            this[GlobalUtilities.GetPropertyName(() => bar.Open)] = bar.Open;
            this[GlobalUtilities.GetPropertyName(() => bar.High)] = bar.High;
            this[GlobalUtilities.GetPropertyName(() => bar.Low)] = bar.Low;
            this[GlobalUtilities.GetPropertyName(() => bar.Close)] = bar.Close;

            this[GlobalUtilities.GetPropertyName(() => bar.Range)] = bar.Range;
      
            this[GlobalUtilities.GetPropertyName(() => bar.IsFalling)] = bar.IsFalling;
            this[GlobalUtilities.GetPropertyName(() => bar.IsGrowing)] = bar.IsGrowing;     
            this[GlobalUtilities.GetPropertyName(() => bar.Volume)] = bar.Volume;
        
        }

        /// <summary>
        /// Converts a value to how it should output in a csv file
        /// If it has a comma, it needs surrounding with double quotes
        /// Eg Sydney, Australia -> "Sydney, Australia"
        /// Also if it contains any double quotes ("), then they need to be replaced with quad quotes[sic] ("")
        /// Eg "Dangerous Dan" McGrew -> """Dangerous Dan"" McGrew"
        /// </summary>
        string MakeValueCsvFriendly(object value)
        {
            if (value == null) return "";
            if (value is INullable && ((INullable)value).IsNull) return "";
            if (value is DateTime)
            {
                if (((DateTime)value).TimeOfDay.TotalSeconds == 0)
                    return ((DateTime)value).ToString("yyyy-MM-dd");
                return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            string output = value.ToString();
            if (output.Contains(",") || output.Contains("\""))
                output = '"' + output.Replace("\"", "\"\"") + '"';
            return output;
        }

        /// <summary>
        /// Output all rows as a CSV returning a string
        /// </summary>
        public string Export()
        {
            StringBuilder sb = new StringBuilder();

            // The header
            foreach (string field in fields)
                sb.Append(field).Append(",");
            sb.AppendLine();

            // The rows
            foreach (Dictionary<string, object> row in rows)
            {
                foreach (string field in fields)
                    sb.Append(MakeValueCsvFriendly(row[field])).Append(",");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Copy the csv file into the clipboard
        /// </summary>
        public void CopyToClipboard()
        {
            try
            {
                string csvdata = this.Export();
                if (!String.IsNullOrEmpty(csvdata))
                {
                    //Copy the csv data into clipboard
                    Thread thread = new Thread(() => Clipboard.SetText(csvdata));
                    //Set the thread to STA
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                    thread.Join();
                }
            }
            catch (Exception)
            {
                //todo log it
            }
        }

        /// <summary>
        /// Exports to a file
        /// </summary>
        public void ExportToFile(string path)
        {
            File.WriteAllText(path, Export());
        }

        /// <summary>
        /// Exports as raw UTF8 bytes
        /// </summary>
        public byte[] ExportToBytes()
        {
            return Encoding.UTF8.GetBytes(Export());
        }
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



#region ITradingOrderExtensions

public static class IBarExtensions
{

    public static DateTime GetTimestamp(this IBar item)
    {
        return item.Time;
    }

}

#endregion

#region ITradingOrderExtensions

public static class ITradingOrderExtensions
{
    
    public static double GetThePrice(this ITradingOrder item)
    {
        double price = 0.0;
        switch (item.Type)
        {
            case OrderType.Stop:
                price = item.StopPrice;
                break;
            case OrderType.Limit:
                price = item.Price;
                break;
            case OrderType.StopLimit:
                price = item.StopPrice;
                break;
            case OrderType.Market:
                price = item.StopPrice;
                break;
            case OrderType.Unknown:
                break;
            default:
                break;
        }

        //if (price == 0.0)
        //{
        //    price = item.StopPrice;
        //}
        return price;
    }

}

#endregion

#region IEnumerableExtensions
/// <summary>
/// This class contains all extension methods for IEnumerable.
/// </summary>
public static class IEnumerableExtensions
{

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
#endregion

#region StringExtensions
/// <summary>
/// This class contains all extension methods for strings.
/// </summary>
public static class StringExtensions
{

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

    /// <summary>
    /// Removes illegal letters on strings for filenames and folders.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string CleanFileName(this String str)
    {
        return CleanFileName(str);
    }
}
#endregion

[Description("We use this indicator to share global code in AgenaTrader.")]
[Category("GlobalUtility")]
public class GlobalUtility : AgenaTrader.UserCode.UserIndicator
{
    //https://www.youtube.com/watch?v=5NNOrp_83RU
}
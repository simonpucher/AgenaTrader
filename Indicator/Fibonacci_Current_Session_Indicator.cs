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
/// Version: 1.2.1
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// Plots the Fibonacci Lines of the current session.
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
 
	[Description("Plots the Fibonacci Lines of the current session.")]
	public class Fibonacci_Current_Session : UserIndicator
	{
        protected override void InitRequirements()
        {
            //  Print("InitRequirements");
        }

        protected override void OnStartUp()
        {
            // Print("OnStartUp");
        }

		protected override void Initialize()
		{
            //Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "Fibonacci_Current_Session_Plot1"));

            CalculateOnBarClose = true;
            Overlay = true;
		}

		protected override void OnBarUpdate()
		{
			//MyPlot1.Set(Input[0]);

            if (Bars != null && Bars.Count > 0 && IsCurrentBarLast)
            {

                //Check if peridocity is valid for this script
                if (!DatafeedPeriodicityIsValid(Bars.TimeFrame))
                {
                    GlobalUtilities.DrawWarningTextOnChart(this, Const.DefaultStringDatafeedPeriodicity);
                    return;
                }
              
                DateTime start = Bars.Where(x => x.Time.Date == Bars[0].Time.Date).FirstOrDefault().Time;
                DateTime start_date = start.Date;
                DateTime end = start.AddHours(23).AddMinutes(59).AddSeconds(59);

                //Selektiere alle gültigen Kurse und finde low und high.
                IEnumerable<IBar> list = Bars.Where(x => x.Time >= start).Where(x => x.Time <= end);
                if (list != null && !list.IsEmpty())
                {
                    double minvalue = list.Where(x => x.Low == list.Min(y => y.Low)).LastOrDefault().Low;
                    double maxvalue = list.Where(x => x.High == list.Max(y => y.High)).LastOrDefault().High;
                    double range = maxvalue - minvalue;

                    //DrawFibonacciRetracements("Fibonacci_Session", true, start_date, minvalue, end, maxvalue);
                    //DrawFibonacciProjections("Fibonacci_Session_Plot", true, start_date, minvalue, Time[0], maxvalue , start_date, minvalue);
                    //DrawHorizontalLine("Fibonacci_Session_LowLine", true, minvalue, Color.Red, DashStyle.Solid, 3);
                    //DrawHorizontalLine("Fibonacci_Session_HighLine", true, maxvalue, Color.Green, DashStyle.Solid, 3);

                    DrawLine("Fibonacci_Session_LowLine", true, start, minvalue, end, minvalue, Color.Red, DashStyle.Solid, 3);
                    DrawLine("Fibonacci_Session_HighLine", true, start, maxvalue, end, maxvalue, Color.Green, DashStyle.Solid, 3);

                    double _fibo_lv_236 = maxvalue - ((range / 100) * 23.6);
                    DrawText("Fibonacci_Session_23.6_String", true, _fibo_lv_236.ToString("N2"), start, _fibo_lv_236, 8, Color.Black, new Font("Arial", 9), StringAlignment.Center, Color.Transparent, Color.Transparent, 100);
                    DrawLine("Fibonacci_Session_23.6_Line", true, start, _fibo_lv_236, end, _fibo_lv_236, Color.Blue, DashStyle.Solid, 2);

                    double _fibo_lv_382 = maxvalue - ((range / 100) * 38.2);
                    DrawLine("Fibonacci_Session_38.2", true, start, _fibo_lv_382, end, _fibo_lv_382, Color.Blue, DashStyle.Solid, 2);

                    double _fibo_lv_500 = maxvalue - ((range / 100) * 50.0);
                    DrawLine("Fibonacci_Session_50.0", true, start, _fibo_lv_500, end, _fibo_lv_500, Color.Blue, DashStyle.Solid, 2);

                    double _fibo_lv_618 = maxvalue - ((range / 100) * 61.8);
                    DrawLine("Fibonacci_Session_61.8", true, start, _fibo_lv_618, end, _fibo_lv_618, Color.Blue, DashStyle.Solid, 2);

                    double _fibo_lv_786 = maxvalue - ((range / 100) * 78.6);
                    DrawLine("Fibonacci_Session_78.6", true, start, _fibo_lv_786, end, _fibo_lv_786, Color.Blue, DashStyle.Solid, 2);
                }
            }
		}


        protected override void OnTermination()
        {
            //Print("OnTermination");

        }

        public override string ToString()
        {
            return "Fibonacci Current Session (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "Fibonacci Current Session (I)";
            }
        }



        /// <summary>
        /// True if the periodicity of the data feed is correct for this indicator.
        /// </summary>
        /// <returns></returns>
        public bool DatafeedPeriodicityIsValid(ITimeFrame timeframe)
        {
            TimeFrame tf = (TimeFrame)timeframe;
            if (this.Bars.TimeFrame.Periodicity == DatafeedHistoryPeriodicity.Hour || this.Bars.TimeFrame.Periodicity == DatafeedHistoryPeriodicity.Minute
                || this.Bars.TimeFrame.Periodicity == DatafeedHistoryPeriodicity.Second || this.Bars.TimeFrame.Periodicity == DatafeedHistoryPeriodicity.Tick)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        #region Properties

        //[Browsable(false)]
        //[XmlIgnore()]
        //public DataSeries MyPlot1
        //{
        //    get { return Values[0]; }
        //}


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
		/// Plots the Fibonacci Lines of the current session.
		/// </summary>
		public Fibonacci_Current_Session Fibonacci_Current_Session()
        {
			return Fibonacci_Current_Session(Input);
		}

		/// <summary>
		/// Plots the Fibonacci Lines of the current session.
		/// </summary>
		public Fibonacci_Current_Session Fibonacci_Current_Session(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Fibonacci_Current_Session>(input);

			if (indicator != null)
				return indicator;

			indicator = new Fibonacci_Current_Session
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
		/// Plots the Fibonacci Lines of the current session.
		/// </summary>
		public Fibonacci_Current_Session Fibonacci_Current_Session()
		{
			return LeadIndicator.Fibonacci_Current_Session(Input);
		}

		/// <summary>
		/// Plots the Fibonacci Lines of the current session.
		/// </summary>
		public Fibonacci_Current_Session Fibonacci_Current_Session(IDataSeries input)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.Fibonacci_Current_Session(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Plots the Fibonacci Lines of the current session.
		/// </summary>
		public Fibonacci_Current_Session Fibonacci_Current_Session()
		{
			return LeadIndicator.Fibonacci_Current_Session(Input);
		}

		/// <summary>
		/// Plots the Fibonacci Lines of the current session.
		/// </summary>
		public Fibonacci_Current_Session Fibonacci_Current_Session(IDataSeries input)
		{
			return LeadIndicator.Fibonacci_Current_Session(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Plots the Fibonacci Lines of the current session.
		/// </summary>
		public Fibonacci_Current_Session Fibonacci_Current_Session()
		{
			return LeadIndicator.Fibonacci_Current_Session(Input);
		}

		/// <summary>
		/// Plots the Fibonacci Lines of the current session.
		/// </summary>
		public Fibonacci_Current_Session Fibonacci_Current_Session(IDataSeries input)
		{
			return LeadIndicator.Fibonacci_Current_Session(input);
		}
	}

	#endregion

}

#endregion



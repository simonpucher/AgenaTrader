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

/// Version: 1.0
namespace AgenaTrader.UserCode
{
	[Description("Plots the Fibonacci Lines of the current session.,")]
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
			Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
			Overlay = true;
			CalculateOnBarClose = true;
		}

		protected override void OnBarUpdate()
		{
			//MyPlot1.Set(Input[0]);

            if (this.Bars.TimeFrame.Periodicity == DatafeedHistoryPeriodicity.Hour ||  
                this.Bars.TimeFrame.Periodicity == DatafeedHistoryPeriodicity.Minute  ||
                    this.Bars.TimeFrame.Periodicity == DatafeedHistoryPeriodicity.Second ||
                this.Bars.TimeFrame.Periodicity == DatafeedHistoryPeriodicity.Tick)
            {
                
                if (Bars != null && Bars.Count > 0 && IsCurrentBarLast)
                {
                    DateTime start = Bars.Where(x => x.Time.Date == Bars[0].Time.Date).FirstOrDefault().Time;
                    DateTime start_date = start.Date;
                    DateTime end = start.AddHours(23).AddMinutes(59).AddSeconds(59);

                    //Selektiere alle gültigen Kurse und finde low und high.
                    IEnumerable<IBar> list = Bars.Where(x => x.Time >= start).Where(x => x.Time <= end);
                    if (list != null && !list.IsEmpty())
                    {
                        double minvalue = list.Where(x => x.Low == list.Min(y => y.Low)).LastOrDefault().Low;
                        double maxvalue = list.Where(x => x.High == list.Max(y => y.High)).LastOrDefault().High;

                        //DrawFibonacciRetracements("Fibonacci_Session", true, start_date, minvalue, end, maxvalue);
                        DrawFibonacciProjections("Fibonacci_Session", true, start_date, minvalue, Time[0], maxvalue  , start_date, minvalue);

                        DrawHorizontalLine("LowLine", true, minvalue, Color.Red, DashStyle.Solid, 3);
                        DrawHorizontalLine("HighLine", true, maxvalue, Color.Green, DashStyle.Solid, 3);

                    }
                }
            }
		}


        protected override void OnTermination()
        {
            //Print("OnTermination");

        }

		#region Properties

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries MyPlot1
		{
			get { return Values[0]; }
		}

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
		/// Plots the Fibonacci Lines of the current session.,
		/// </summary>
		public Fibonacci_Current_Session Fibonacci_Current_Session()
        {
			return Fibonacci_Current_Session(Input);
		}

		/// <summary>
		/// Plots the Fibonacci Lines of the current session.,
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
		/// Plots the Fibonacci Lines of the current session.,
		/// </summary>
		public Fibonacci_Current_Session Fibonacci_Current_Session()
		{
			return LeadIndicator.Fibonacci_Current_Session(Input);
		}

		/// <summary>
		/// Plots the Fibonacci Lines of the current session.,
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
		/// Plots the Fibonacci Lines of the current session.,
		/// </summary>
		public Fibonacci_Current_Session Fibonacci_Current_Session()
		{
			return LeadIndicator.Fibonacci_Current_Session(Input);
		}

		/// <summary>
		/// Plots the Fibonacci Lines of the current session.,
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
		/// Plots the Fibonacci Lines of the current session.,
		/// </summary>
		public Fibonacci_Current_Session Fibonacci_Current_Session()
		{
			return LeadIndicator.Fibonacci_Current_Session(Input);
		}

		/// <summary>
		/// Plots the Fibonacci Lines of the current session.,
		/// </summary>
		public Fibonacci_Current_Session Fibonacci_Current_Session(IDataSeries input)
		{
			return LeadIndicator.Fibonacci_Current_Session(input);
		}
	}

	#endregion

}

#endregion



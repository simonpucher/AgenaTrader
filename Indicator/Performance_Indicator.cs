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
/// Version: 1.1
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// This indicator can be used in the scanner column to display the yearly or daily performance.
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{

    public enum PerformanceCalculationType
    {
        BarCount,
        ThisYear
    }

	[Description("Show the performance of an instrument in the scanner column. Indicator calculates bars backwards so you need to configure the timeframe correctly.")]
	public class Performance_Indicator : UserIndicator
	{
        //Input
        private int _barscount = 400;
        private PerformanceCalculationType _PerformanceCalculationType = PerformanceCalculationType.BarCount;


        protected override void Initialize()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "Plot_Performance_Indicator"));

            //to get the latest data into the bars object
			CalculateOnBarClose = false;
            Overlay = false;

            //Because of Backtesting reasons if we use the advanced mode we need at least two bars
            this.BarsRequired = 365;
        }

		protected override void OnBarUpdate()
		{
            if (IsCurrentBarLast)
            {
                switch (this.PerformanceCalculationType)
                {
                    case PerformanceCalculationType.BarCount:
                        Plot_Performance_Indicator.Set(((Close[0] - Close[this.BarsCount]) * 100) / Close[this.BarsCount]);
                        break;
                    case PerformanceCalculationType.ThisYear:
                        IBar lb = Bars.Where(x => x.Time.Year != DateTime.Now.Year).Last();
                        Plot_Performance_Indicator.Set(((Close[0] - lb.Close) * 100) / lb.Close);
                        break;
                }

            }
        }


        public override string ToString()
        {
            return "Performance (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "Performance (I)";
            }
        }


        #region Properties

        [Description("Performance will be calculated x bars backwards. Insert 365 for the performance of the last year. Insert 1 for the perfomance since yesterday.")]
        [Category("Parameters")]
        [DisplayName("Bars backwards")]
        public int BarsCount
        {
            get { return _barscount; }
            set { _barscount = value; }
        }

        [Description("Choose the type of calculation. BarCount = The calculation will use the bars for the calculation. ThisYear = Calculation will be done for today minus the close of the last bar of last year.")]
        [Category("Parameters")]
        [DisplayName("Type")]
        public PerformanceCalculationType PerformanceCalculationType
        {
            get { return _PerformanceCalculationType; }
            set { _PerformanceCalculationType = value; }
        }

        [Browsable(false)]
		[XmlIgnore()]
		public DataSeries Plot_Performance_Indicator
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

	public partial class UserIndicator
	{
		/// <summary>
		/// Show the performance of an instrument in the scanner column. Indicator calculates bars backwards so you need to configure the timeframe correctly.
		/// </summary>
		public Performance_Indicator Performance_Indicator(System.Int32 barsCount, PerformanceCalculationType performanceCalculationType)
        {
			return Performance_Indicator(Input, barsCount, performanceCalculationType);
		}

		/// <summary>
		/// Show the performance of an instrument in the scanner column. Indicator calculates bars backwards so you need to configure the timeframe correctly.
		/// </summary>
		public Performance_Indicator Performance_Indicator(IDataSeries input, System.Int32 barsCount, PerformanceCalculationType performanceCalculationType)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Performance_Indicator>(input, i => i.BarsCount == barsCount && i.PerformanceCalculationType == performanceCalculationType);

			if (indicator != null)
				return indicator;

			indicator = new Performance_Indicator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							BarsCount = barsCount,
							PerformanceCalculationType = performanceCalculationType
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
		/// Show the performance of an instrument in the scanner column. Indicator calculates bars backwards so you need to configure the timeframe correctly.
		/// </summary>
		public Performance_Indicator Performance_Indicator(System.Int32 barsCount, PerformanceCalculationType performanceCalculationType)
		{
			return LeadIndicator.Performance_Indicator(Input, barsCount, performanceCalculationType);
		}

		/// <summary>
		/// Show the performance of an instrument in the scanner column. Indicator calculates bars backwards so you need to configure the timeframe correctly.
		/// </summary>
		public Performance_Indicator Performance_Indicator(IDataSeries input, System.Int32 barsCount, PerformanceCalculationType performanceCalculationType)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.Performance_Indicator(input, barsCount, performanceCalculationType);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Show the performance of an instrument in the scanner column. Indicator calculates bars backwards so you need to configure the timeframe correctly.
		/// </summary>
		public Performance_Indicator Performance_Indicator(System.Int32 barsCount, PerformanceCalculationType performanceCalculationType)
		{
			return LeadIndicator.Performance_Indicator(Input, barsCount, performanceCalculationType);
		}

		/// <summary>
		/// Show the performance of an instrument in the scanner column. Indicator calculates bars backwards so you need to configure the timeframe correctly.
		/// </summary>
		public Performance_Indicator Performance_Indicator(IDataSeries input, System.Int32 barsCount, PerformanceCalculationType performanceCalculationType)
		{
			return LeadIndicator.Performance_Indicator(input, barsCount, performanceCalculationType);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Show the performance of an instrument in the scanner column. Indicator calculates bars backwards so you need to configure the timeframe correctly.
		/// </summary>
		public Performance_Indicator Performance_Indicator(System.Int32 barsCount, PerformanceCalculationType performanceCalculationType)
		{
			return LeadIndicator.Performance_Indicator(Input, barsCount, performanceCalculationType);
		}

		/// <summary>
		/// Show the performance of an instrument in the scanner column. Indicator calculates bars backwards so you need to configure the timeframe correctly.
		/// </summary>
		public Performance_Indicator Performance_Indicator(IDataSeries input, System.Int32 barsCount, PerformanceCalculationType performanceCalculationType)
		{
			return LeadIndicator.Performance_Indicator(input, barsCount, performanceCalculationType);
		}
	}

	#endregion

}

#endregion

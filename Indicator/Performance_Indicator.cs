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
	[Description("Show the performance of an instrument in the scanner column. Indicator calculates bars backwards so you need to configure the timeframe correctly.")]
	public class Performance_Indicator : UserIndicator
	{
        //Input
        private int _barscount = 365;


        protected override void Initialize()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
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
                MyPlot1.Set(((Close[0] - Close[this.BarsCount]) * 100) / Close[this.BarsCount]);
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

	public partial class UserIndicator
	{
		/// <summary>
		/// Show the performance of an instrument in the scanner column. Indicator calculates bars backwards so you need to configure the timeframe correctly.
		/// </summary>
		public Performance_Indicator Performance_Indicator(System.Int32 barsCount)
        {
			return Performance_Indicator(Input, barsCount);
		}

		/// <summary>
		/// Show the performance of an instrument in the scanner column. Indicator calculates bars backwards so you need to configure the timeframe correctly.
		/// </summary>
		public Performance_Indicator Performance_Indicator(IDataSeries input, System.Int32 barsCount)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Performance_Indicator>(input, i => i.BarsCount == barsCount);

			if (indicator != null)
				return indicator;

			indicator = new Performance_Indicator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							BarsCount = barsCount
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
		public Performance_Indicator Performance_Indicator(System.Int32 barsCount)
		{
			return LeadIndicator.Performance_Indicator(Input, barsCount);
		}

		/// <summary>
		/// Show the performance of an instrument in the scanner column. Indicator calculates bars backwards so you need to configure the timeframe correctly.
		/// </summary>
		public Performance_Indicator Performance_Indicator(IDataSeries input, System.Int32 barsCount)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.Performance_Indicator(input, barsCount);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Show the performance of an instrument in the scanner column. Indicator calculates bars backwards so you need to configure the timeframe correctly.
		/// </summary>
		public Performance_Indicator Performance_Indicator(System.Int32 barsCount)
		{
			return LeadIndicator.Performance_Indicator(Input, barsCount);
		}

		/// <summary>
		/// Show the performance of an instrument in the scanner column. Indicator calculates bars backwards so you need to configure the timeframe correctly.
		/// </summary>
		public Performance_Indicator Performance_Indicator(IDataSeries input, System.Int32 barsCount)
		{
			return LeadIndicator.Performance_Indicator(input, barsCount);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Show the performance of an instrument in the scanner column. Indicator calculates bars backwards so you need to configure the timeframe correctly.
		/// </summary>
		public Performance_Indicator Performance_Indicator(System.Int32 barsCount)
		{
			return LeadIndicator.Performance_Indicator(Input, barsCount);
		}

		/// <summary>
		/// Show the performance of an instrument in the scanner column. Indicator calculates bars backwards so you need to configure the timeframe correctly.
		/// </summary>
		public Performance_Indicator Performance_Indicator(IDataSeries input, System.Int32 barsCount)
		{
			return LeadIndicator.Performance_Indicator(input, barsCount);
		}
	}

	#endregion

}

#endregion

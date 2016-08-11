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

namespace AgenaTrader.UserCode
{
	[Description("Show the performance of an instrument in the scanner column")]
	public class Performance_Indicator : UserIndicator
	{
		protected override void Initialize()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
			CalculateOnBarClose = true;
		}

		protected override void OnBarUpdate()
		{
            //MyPlot1.Set(Input[0]);
            //if(Bars.Count() > 30) {

            //}

            double value = ((Close[0] - Close[30]) * 100) / Close[30];

            //double bubu = (Close[0] /  (Close[30] / 100)) - 100;
            MyPlot1.Set(value);

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

	public partial class UserIndicator
	{
		/// <summary>
		/// Show the performance of an instrument in the scanner column
		/// </summary>
		public Performance_Indicator Performance_Indicator()
        {
			return Performance_Indicator(Input);
		}

		/// <summary>
		/// Show the performance of an instrument in the scanner column
		/// </summary>
		public Performance_Indicator Performance_Indicator(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Performance_Indicator>(input);

			if (indicator != null)
				return indicator;

			indicator = new Performance_Indicator
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
		/// Show the performance of an instrument in the scanner column
		/// </summary>
		public Performance_Indicator Performance_Indicator()
		{
			return LeadIndicator.Performance_Indicator(Input);
		}

		/// <summary>
		/// Show the performance of an instrument in the scanner column
		/// </summary>
		public Performance_Indicator Performance_Indicator(IDataSeries input)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.Performance_Indicator(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Show the performance of an instrument in the scanner column
		/// </summary>
		public Performance_Indicator Performance_Indicator()
		{
			return LeadIndicator.Performance_Indicator(Input);
		}

		/// <summary>
		/// Show the performance of an instrument in the scanner column
		/// </summary>
		public Performance_Indicator Performance_Indicator(IDataSeries input)
		{
			return LeadIndicator.Performance_Indicator(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Show the performance of an instrument in the scanner column
		/// </summary>
		public Performance_Indicator Performance_Indicator()
		{
			return LeadIndicator.Performance_Indicator(Input);
		}

		/// <summary>
		/// Show the performance of an instrument in the scanner column
		/// </summary>
		public Performance_Indicator Performance_Indicator(IDataSeries input)
		{
			return LeadIndicator.Performance_Indicator(input);
		}
	}

	#endregion

}

#endregion

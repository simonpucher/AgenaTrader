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
	[Description("Delta Distance in percent to SMA200")]
	public class Distance_Indicator : UserIndicator
	{
		protected override void OnInit()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Gray), "MyPlot2"));
            IsOverlay = true;
			CalculateOnClosedBar = true;
		}

		protected override void OnCalculate()
		{
            SMA _sma = SMA(200);
            double resuklt = (InSeries[0] / (_sma[0] / 100))-100;
            MyPlot1.Set(resuklt);
            MyPlot2.Set(0);
        }

        #region Properties

        [Browsable(false)]
		[XmlIgnore()]
		public DataSeries MyPlot1
		{
			get { return Outputs[0]; }
		}

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries MyPlot2
        {
            get { return Outputs[1]; }
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
		/// Delta Distance in percent to SMA200
		/// </summary>
		public Distance_Indicator Distance_Indicator()
        {
			return Distance_Indicator(InSeries);
		}

		/// <summary>
		/// Delta Distance in percent to SMA200
		/// </summary>
		public Distance_Indicator Distance_Indicator(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Distance_Indicator>(input);

			if (indicator != null)
				return indicator;

			indicator = new Distance_Indicator
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input
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
		/// Delta Distance in percent to SMA200
		/// </summary>
		public Distance_Indicator Distance_Indicator()
		{
			return LeadIndicator.Distance_Indicator(InSeries);
		}

		/// <summary>
		/// Delta Distance in percent to SMA200
		/// </summary>
		public Distance_Indicator Distance_Indicator(IDataSeries input)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.Distance_Indicator(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Delta Distance in percent to SMA200
		/// </summary>
		public Distance_Indicator Distance_Indicator()
		{
			return LeadIndicator.Distance_Indicator(InSeries);
		}

		/// <summary>
		/// Delta Distance in percent to SMA200
		/// </summary>
		public Distance_Indicator Distance_Indicator(IDataSeries input)
		{
			return LeadIndicator.Distance_Indicator(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Delta Distance in percent to SMA200
		/// </summary>
		public Distance_Indicator Distance_Indicator()
		{
			return LeadIndicator.Distance_Indicator(InSeries);
		}

		/// <summary>
		/// Delta Distance in percent to SMA200
		/// </summary>
		public Distance_Indicator Distance_Indicator(IDataSeries input)
		{
			return LeadIndicator.Distance_Indicator(input);
		}
	}

	#endregion

}

#endregion

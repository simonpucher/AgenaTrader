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
/// Version: 1.3
/// -------------------------------------------------------------------------
/// Simon Pucher 2017
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("Volatility Indicator by Robert Levy.")]
	public class Volatility_Levy_Stand_Dev_Mean_Average_Indicator : UserIndicator
	{
        private int _period = 27;


        protected override void OnInit()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "Plot_Volatility_Levy_"+this.Period));
		}

		protected override void OnCalculate()
		{

			MyPlot1.Set(StdDev(this.Period)[0] / SMA(this.Period)[0]);
		}

        public override string ToString()
        {
            return "Volatility Levy (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "Volatility Levy (I)";
            }
        }


        #region Properties

        [Browsable(false)]
		[XmlIgnore()]
		public DataSeries MyPlot1
		{
			get { return Outputs[0]; }
		}

        /// <summary>
        /// </summary>
        [Description("Number of historical bars.")]
        [Category("Parameters")]
        [DisplayName("Period")]
        public int Period
        {
            get { return _period; }
            set { _period = value; }
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
		/// Volatility Indicator by Robert Levy.
		/// </summary>
		public Volatility_Levy_Stand_Dev_Mean_Average_Indicator Volatility_Levy_Stand_Dev_Mean_Average_Indicator(System.Int32 period)
        {
			return Volatility_Levy_Stand_Dev_Mean_Average_Indicator(InSeries, period);
		}

		/// <summary>
		/// Volatility Indicator by Robert Levy.
		/// </summary>
		public Volatility_Levy_Stand_Dev_Mean_Average_Indicator Volatility_Levy_Stand_Dev_Mean_Average_Indicator(IDataSeries input, System.Int32 period)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Volatility_Levy_Stand_Dev_Mean_Average_Indicator>(input, i => i.Period == period);

			if (indicator != null)
				return indicator;

			indicator = new Volatility_Levy_Stand_Dev_Mean_Average_Indicator
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input,
							Period = period
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
		/// Volatility Indicator by Robert Levy.
		/// </summary>
		public Volatility_Levy_Stand_Dev_Mean_Average_Indicator Volatility_Levy_Stand_Dev_Mean_Average_Indicator(System.Int32 period)
		{
			return LeadIndicator.Volatility_Levy_Stand_Dev_Mean_Average_Indicator(InSeries, period);
		}

		/// <summary>
		/// Volatility Indicator by Robert Levy.
		/// </summary>
		public Volatility_Levy_Stand_Dev_Mean_Average_Indicator Volatility_Levy_Stand_Dev_Mean_Average_Indicator(IDataSeries input, System.Int32 period)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.Volatility_Levy_Stand_Dev_Mean_Average_Indicator(input, period);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Volatility Indicator by Robert Levy.
		/// </summary>
		public Volatility_Levy_Stand_Dev_Mean_Average_Indicator Volatility_Levy_Stand_Dev_Mean_Average_Indicator(System.Int32 period)
		{
			return LeadIndicator.Volatility_Levy_Stand_Dev_Mean_Average_Indicator(InSeries, period);
		}

		/// <summary>
		/// Volatility Indicator by Robert Levy.
		/// </summary>
		public Volatility_Levy_Stand_Dev_Mean_Average_Indicator Volatility_Levy_Stand_Dev_Mean_Average_Indicator(IDataSeries input, System.Int32 period)
		{
			return LeadIndicator.Volatility_Levy_Stand_Dev_Mean_Average_Indicator(input, period);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Volatility Indicator by Robert Levy.
		/// </summary>
		public Volatility_Levy_Stand_Dev_Mean_Average_Indicator Volatility_Levy_Stand_Dev_Mean_Average_Indicator(System.Int32 period)
		{
			return LeadIndicator.Volatility_Levy_Stand_Dev_Mean_Average_Indicator(InSeries, period);
		}

		/// <summary>
		/// Volatility Indicator by Robert Levy.
		/// </summary>
		public Volatility_Levy_Stand_Dev_Mean_Average_Indicator Volatility_Levy_Stand_Dev_Mean_Average_Indicator(IDataSeries input, System.Int32 period)
		{
			return LeadIndicator.Volatility_Levy_Stand_Dev_Mean_Average_Indicator(input, period);
		}
	}

	#endregion

}

#endregion

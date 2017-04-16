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
/// Version: 1.1.2
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
	[Description("Compare two instruments.")]
	public class CompareInstrument_Indicator : UserIndicator
	{

        private const int endOfScale = 1;
        private const int topOfScale = 100;

        protected override void OnInit()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Gray), "MyComparePlot_1"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyComparePlot_2"));
        }

        protected override void OnBarsRequirements()
        {
            Add(Core.InstrumentManager.GetInstrument(this.Instrument.Symbol));
            Add(Core.InstrumentManager.GetInstrument(_instrument_2));
        }


        protected override void OnCalculate()
		{
           
            //MyPlot1.Set(Closes[1][0]);
            MyPlot1.Set(Normalize(Closes[1].ToList(), Closes[1][0]));
            MyPlot2.Set(Normalize(Closes[2].ToList(), Closes[2][0]));
        }

        private static double Normalize(List<double> list, double currentValue)
        {
            
            double min = list.Min();
            double max = list.Max();

            return endOfScale + (currentValue - min) * (topOfScale - endOfScale) / (max - min);

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


        //private string _instrument = "AAPL";

        //[Description("First Symbol to compare")]
        //[Category("Parameters")]
        //[DisplayNameAttribute("1st Symbol")]
        //public string Symbol
        //{
        //    get { return _instrument; }
        //    set { _instrument = value; }
        //}

        private string _instrument_2 = "AAPL";

        [Description("First Symbol to compare")]
        [Category("Parameters")]
        [DisplayNameAttribute("2nd Symbol")]
        public string Symbol_2
        {
            get { return _instrument_2; }
            set { _instrument_2 = value; }
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
		/// Compare two instruments.
		/// </summary>
		public CompareInstrument_Indicator CompareInstrument_Indicator(System.String symbol_2)
        {
			return CompareInstrument_Indicator(InSeries, symbol_2);
		}

		/// <summary>
		/// Compare two instruments.
		/// </summary>
		public CompareInstrument_Indicator CompareInstrument_Indicator(IDataSeries input, System.String symbol_2)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<CompareInstrument_Indicator>(input, i => i.Symbol_2 == symbol_2);

			if (indicator != null)
				return indicator;

			indicator = new CompareInstrument_Indicator
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input,
							Symbol_2 = symbol_2
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
		/// Compare two instruments.
		/// </summary>
		public CompareInstrument_Indicator CompareInstrument_Indicator(System.String symbol_2)
		{
			return LeadIndicator.CompareInstrument_Indicator(InSeries, symbol_2);
		}

		/// <summary>
		/// Compare two instruments.
		/// </summary>
		public CompareInstrument_Indicator CompareInstrument_Indicator(IDataSeries input, System.String symbol_2)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.CompareInstrument_Indicator(input, symbol_2);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Compare two instruments.
		/// </summary>
		public CompareInstrument_Indicator CompareInstrument_Indicator(System.String symbol_2)
		{
			return LeadIndicator.CompareInstrument_Indicator(InSeries, symbol_2);
		}

		/// <summary>
		/// Compare two instruments.
		/// </summary>
		public CompareInstrument_Indicator CompareInstrument_Indicator(IDataSeries input, System.String symbol_2)
		{
			return LeadIndicator.CompareInstrument_Indicator(input, symbol_2);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Compare two instruments.
		/// </summary>
		public CompareInstrument_Indicator CompareInstrument_Indicator(System.String symbol_2)
		{
			return LeadIndicator.CompareInstrument_Indicator(InSeries, symbol_2);
		}

		/// <summary>
		/// Compare two instruments.
		/// </summary>
		public CompareInstrument_Indicator CompareInstrument_Indicator(IDataSeries input, System.String symbol_2)
		{
			return LeadIndicator.CompareInstrument_Indicator(input, symbol_2);
		}
	}

	#endregion

}

#endregion

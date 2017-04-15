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
	[Description("Compare instruments.")]
	public class CompareInstrument_Indicator : UserIndicator
	{



        protected override void OnInit()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyComparePlot"));
        }

        protected override void OnBarsRequirements()
        {
            Add(Core.InstrumentManager.GetInstrument(_instrument));
        }


        protected override void OnCalculate()
		{
            MyPlot1.Set(Closes[1][0]);
        }

		#region Properties

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries MyPlot1
		{
			get { return Outputs[0]; }
		}


        private string _instrument = "AAPL";

        [Description("Symbol to compare")]
        [Category("Parameters")]
        [DisplayNameAttribute("Symbol")]
        public string Symbol
        {
            get { return _instrument; }
            set { _instrument = value; }
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
		/// Compare instruments.
		/// </summary>
		public CompareInstrument_Indicator CompareInstrument_Indicator(System.String symbol)
        {
			return CompareInstrument_Indicator(InSeries, symbol);
		}

		/// <summary>
		/// Compare instruments.
		/// </summary>
		public CompareInstrument_Indicator CompareInstrument_Indicator(IDataSeries input, System.String symbol)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<CompareInstrument_Indicator>(input, i => i.Symbol == symbol);

			if (indicator != null)
				return indicator;

			indicator = new CompareInstrument_Indicator
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input,
							Symbol = symbol
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
		/// Compare instruments.
		/// </summary>
		public CompareInstrument_Indicator CompareInstrument_Indicator(System.String symbol)
		{
			return LeadIndicator.CompareInstrument_Indicator(InSeries, symbol);
		}

		/// <summary>
		/// Compare instruments.
		/// </summary>
		public CompareInstrument_Indicator CompareInstrument_Indicator(IDataSeries input, System.String symbol)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.CompareInstrument_Indicator(input, symbol);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Compare instruments.
		/// </summary>
		public CompareInstrument_Indicator CompareInstrument_Indicator(System.String symbol)
		{
			return LeadIndicator.CompareInstrument_Indicator(InSeries, symbol);
		}

		/// <summary>
		/// Compare instruments.
		/// </summary>
		public CompareInstrument_Indicator CompareInstrument_Indicator(IDataSeries input, System.String symbol)
		{
			return LeadIndicator.CompareInstrument_Indicator(input, symbol);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Compare instruments.
		/// </summary>
		public CompareInstrument_Indicator CompareInstrument_Indicator(System.String symbol)
		{
			return LeadIndicator.CompareInstrument_Indicator(InSeries, symbol);
		}

		/// <summary>
		/// Compare instruments.
		/// </summary>
		public CompareInstrument_Indicator CompareInstrument_Indicator(IDataSeries input, System.String symbol)
		{
			return LeadIndicator.CompareInstrument_Indicator(input, symbol);
		}
	}

	#endregion

}

#endregion

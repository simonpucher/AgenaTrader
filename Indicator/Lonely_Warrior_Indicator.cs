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
/// Version: in progress
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// todo
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("Enter the description for the new custom indicator here")]
	public class Lonely_Warrior_Indicator : UserIndicator
	{
        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            //Print("Initialize");

            //Add(new Plot(new Pen(this.Plot0Color, this.Plot0Width), PlotStyle.Line, "TestPlot_Indicator"));
            //Add(new Plot(new Pen(this.Plot1Color, this.Plot1Width), PlotStyle.Line, "TestPlot_GreyedOut_Indicator"));

            CalculateOnBarClose = true;
            Overlay = false;

            //Because of Backtesting reasons if we use the advanced mode we need at least two bars
            this.BarsRequired = 20;
        }

        protected override void InitRequirements()
        {
            //Print("InitRequirements");
            base.InitRequirements();
        }

        protected override void OnBarUpdate()
		{
			MyPlot1.Set(Input[0]);
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
		/// Enter the description for the new custom indicator here
		/// </summary>
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator()
        {
			return Lonely_Warrior_Indicator(Input);
		}

		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Lonely_Warrior_Indicator>(input);

			if (indicator != null)
				return indicator;

			indicator = new Lonely_Warrior_Indicator
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
		/// Enter the description for the new custom indicator here
		/// </summary>
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator()
		{
			return LeadIndicator.Lonely_Warrior_Indicator(Input);
		}

		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator(IDataSeries input)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.Lonely_Warrior_Indicator(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator()
		{
			return LeadIndicator.Lonely_Warrior_Indicator(Input);
		}

		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator(IDataSeries input)
		{
			return LeadIndicator.Lonely_Warrior_Indicator(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator()
		{
			return LeadIndicator.Lonely_Warrior_Indicator(Input);
		}

		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator(IDataSeries input)
		{
			return LeadIndicator.Lonely_Warrior_Indicator(input);
		}
	}

	#endregion

}

#endregion

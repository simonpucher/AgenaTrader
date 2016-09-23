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
/// Inspired by https://www.youtube.com/watch?v=Qj_6DFTNfjE
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Watch out for the lonely warrior behind enemy lines.")]
    [IsEntryAttribute(true)]
	[IsStopAttribute(false)]
	[IsTargetAttribute(false)]
	[OverrulePreviousStopPrice(false)]
	public class Lonely_Warrior_Condition : UserScriptedCondition
	{
		#region Variables

		private int _myCondition1 = 1;

		#endregion

		protected override void Initialize()
		{
			IsEntry = true;
			IsStop = false;
			IsTarget = false;
			Add(new Plot(Color.FromKnownColor(KnownColor.Black), "Occurred"));
			Add(new Plot(Color.FromArgb(255, 157, 214, 93), "Entry"));
			Overlay = true;
			CalculateOnBarClose = true;

            this.BarsRequired = 20;
        }

		protected override void OnBarUpdate()
		{
            Occurred.Set(Lonely_Warrior_Indicator()[0]);
        }


        public override string ToString()
        {
            return "Lonely Warrior (C)";
        }

        public override string DisplayName
        {
            get
            {
                return "Lonely Warrior (C)";
            }
        }

        #region Properties

        [Browsable(false)]
		[XmlIgnore()]
		public DataSeries Occurred
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Entry
		{
			get { return Values[1]; }
		}

		public override IList<DataSeries> GetEntries()
		{
			return new[]{Entry};
		}

		[Description("")]
		[Category("Parameters")]
		public int MyCondition1
		{
			get { return _myCondition1; }
			set { _myCondition1 = Math.Max(1, value); }
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
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Lonely_Warrior_Condition Lonely_Warrior_Condition(System.Int32 myCondition1)
        {
			return Lonely_Warrior_Condition(Input, myCondition1);
		}

		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Lonely_Warrior_Condition Lonely_Warrior_Condition(IDataSeries input, System.Int32 myCondition1)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Lonely_Warrior_Condition>(input, i => i.MyCondition1 == myCondition1);

			if (indicator != null)
				return indicator;

			indicator = new Lonely_Warrior_Condition
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							MyCondition1 = myCondition1
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
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Lonely_Warrior_Condition Lonely_Warrior_Condition(System.Int32 myCondition1)
		{
			return LeadIndicator.Lonely_Warrior_Condition(Input, myCondition1);
		}

		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Lonely_Warrior_Condition Lonely_Warrior_Condition(IDataSeries input, System.Int32 myCondition1)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.Lonely_Warrior_Condition(input, myCondition1);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Lonely_Warrior_Condition Lonely_Warrior_Condition(System.Int32 myCondition1)
		{
			return LeadIndicator.Lonely_Warrior_Condition(Input, myCondition1);
		}

		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Lonely_Warrior_Condition Lonely_Warrior_Condition(IDataSeries input, System.Int32 myCondition1)
		{
			return LeadIndicator.Lonely_Warrior_Condition(input, myCondition1);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Lonely_Warrior_Condition Lonely_Warrior_Condition(System.Int32 myCondition1)
		{
			return LeadIndicator.Lonely_Warrior_Condition(Input, myCondition1);
		}

		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Lonely_Warrior_Condition Lonely_Warrior_Condition(IDataSeries input, System.Int32 myCondition1)
		{
			return LeadIndicator.Lonely_Warrior_Condition(input, myCondition1);
		}
	}

	#endregion

}

#endregion

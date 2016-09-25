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
/// Inspired by https://youtu.be/Qj_6DFTNfjE?t=437
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("The force is strong in this instrument.")]
    [IsEntryAttribute(true)]
	[IsStopAttribute(false)]
	[IsTargetAttribute(false)]
	[OverrulePreviousStopPrice(false)]
	public class Momentum_up_gap_Condition : UserScriptedCondition
	{
        #region Variables

        private int _percentage = 3;

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
            Occurred.Set(Momentum_up_gap_Indicator(this.Percentage)[0]);
        }


        public override string ToString()
        {
            return "Momentum up gap (C)";
        }

        public override string DisplayName
        {
            get
            {
                return "Momentum up gap (C)";
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

        /// <summary>
        /// </summary>
        [Description("Percentage for the up gap.")]
        [Category("Parameters")]
        [DisplayName("Percentage")]
        public int Percentage
        {
            get { return _percentage; }
            set { _percentage = value; }
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
		/// The force is strong in this instrument.
		/// </summary>
		public Momentum_up_gap_Condition Momentum_up_gap_Condition(System.Int32 percentage)
        {
			return Momentum_up_gap_Condition(Input, percentage);
		}

		/// <summary>
		/// The force is strong in this instrument.
		/// </summary>
		public Momentum_up_gap_Condition Momentum_up_gap_Condition(IDataSeries input, System.Int32 percentage)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Momentum_up_gap_Condition>(input, i => i.Percentage == percentage);

			if (indicator != null)
				return indicator;

			indicator = new Momentum_up_gap_Condition
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							Percentage = percentage
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
		/// The force is strong in this instrument.
		/// </summary>
		public Momentum_up_gap_Condition Momentum_up_gap_Condition(System.Int32 percentage)
		{
			return LeadIndicator.Momentum_up_gap_Condition(Input, percentage);
		}

		/// <summary>
		/// The force is strong in this instrument.
		/// </summary>
		public Momentum_up_gap_Condition Momentum_up_gap_Condition(IDataSeries input, System.Int32 percentage)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.Momentum_up_gap_Condition(input, percentage);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// The force is strong in this instrument.
		/// </summary>
		public Momentum_up_gap_Condition Momentum_up_gap_Condition(System.Int32 percentage)
		{
			return LeadIndicator.Momentum_up_gap_Condition(Input, percentage);
		}

		/// <summary>
		/// The force is strong in this instrument.
		/// </summary>
		public Momentum_up_gap_Condition Momentum_up_gap_Condition(IDataSeries input, System.Int32 percentage)
		{
			return LeadIndicator.Momentum_up_gap_Condition(input, percentage);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// The force is strong in this instrument.
		/// </summary>
		public Momentum_up_gap_Condition Momentum_up_gap_Condition(System.Int32 percentage)
		{
			return LeadIndicator.Momentum_up_gap_Condition(Input, percentage);
		}

		/// <summary>
		/// The force is strong in this instrument.
		/// </summary>
		public Momentum_up_gap_Condition Momentum_up_gap_Condition(IDataSeries input, System.Int32 percentage)
		{
			return LeadIndicator.Momentum_up_gap_Condition(input, percentage);
		}
	}

	#endregion

}

#endregion

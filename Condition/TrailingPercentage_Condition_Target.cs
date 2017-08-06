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
	[Description("Trailing Stop with percentage value.")]
	[IsEntryAttribute(false)]
	[IsStopAttribute(false)]
	[IsTargetAttribute(true)]
	[OverrulePreviousStopPrice(false)]
	public class TrailingPercentage_Condition_Target : UserScriptedCondition
	{
		#region Variables

		private double _percentage = 1.5;

		#endregion

		protected override void OnInit()
		{
			IsEntry = false;
			IsStop = false;
			IsTarget = true;
			Add(new Plot(Color.FromKnownColor(KnownColor.Black), "Occurred"));
			Add(new Plot(Color.Orange, "Target"));
			IsOverlay = true;
            CalculateOnClosedBar = false;
		}

		protected override void OnCalculate()
		{
            //Print("TrailingPercentage_Condition_Target OnCalculate: " + Time[0].ToString());
            Occurred.Set(1);
            Target.Set(Close[0] * (1 + this.Percentage/100.0));
        }

		#region Properties

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Occurred
		{
			get { return Outputs[0]; }
		}

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Target
		{
			get { return Outputs[1]; }
		}

		public override IList<DataSeries> GetTargets()
		{
			return new[]{Target};
		}

		[Description("")]
		[Category("Parameters")]
		public double Percentage
		{
			get { return _percentage; }
			set { _percentage = value; }
		}

		#endregion
	}
}
#region AgenaTrader Automaticaly Generated Code. Do not change it manually

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator
	{
		/// <summary>
		/// Trailing Stop with percentage value.
		/// </summary>
		public TrailingPercentage_Condition_Target TrailingPercentage_Condition_Target(System.Double percentage)
        {
			return TrailingPercentage_Condition_Target(InSeries, percentage);
		}

		/// <summary>
		/// Trailing Stop with percentage value.
		/// </summary>
		public TrailingPercentage_Condition_Target TrailingPercentage_Condition_Target(IDataSeries input, System.Double percentage)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<TrailingPercentage_Condition_Target>(input, i => Math.Abs(i.Percentage - percentage) <= Double.Epsilon);

			if (indicator != null)
				return indicator;

			indicator = new TrailingPercentage_Condition_Target
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input,
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
		/// Trailing Stop with percentage value.
		/// </summary>
		public TrailingPercentage_Condition_Target TrailingPercentage_Condition_Target(System.Double percentage)
		{
			return LeadIndicator.TrailingPercentage_Condition_Target(InSeries, percentage);
		}

		/// <summary>
		/// Trailing Stop with percentage value.
		/// </summary>
		public TrailingPercentage_Condition_Target TrailingPercentage_Condition_Target(IDataSeries input, System.Double percentage)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.TrailingPercentage_Condition_Target(input, percentage);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Trailing Stop with percentage value.
		/// </summary>
		public TrailingPercentage_Condition_Target TrailingPercentage_Condition_Target(System.Double percentage)
		{
			return LeadIndicator.TrailingPercentage_Condition_Target(InSeries, percentage);
		}

		/// <summary>
		/// Trailing Stop with percentage value.
		/// </summary>
		public TrailingPercentage_Condition_Target TrailingPercentage_Condition_Target(IDataSeries input, System.Double percentage)
		{
			return LeadIndicator.TrailingPercentage_Condition_Target(input, percentage);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Trailing Stop with percentage value.
		/// </summary>
		public TrailingPercentage_Condition_Target TrailingPercentage_Condition_Target(System.Double percentage)
		{
			return LeadIndicator.TrailingPercentage_Condition_Target(InSeries, percentage);
		}

		/// <summary>
		/// Trailing Stop with percentage value.
		/// </summary>
		public TrailingPercentage_Condition_Target TrailingPercentage_Condition_Target(IDataSeries input, System.Double percentage)
		{
			return LeadIndicator.TrailingPercentage_Condition_Target(input, percentage);
		}
	}

	#endregion

}

#endregion

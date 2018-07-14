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
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("Trailing Stop with percentage value.")]
	[IsEntryAttribute(false)]
	[IsStopAttribute(true)]
	[IsTargetAttribute(false)]
	[OverrulePreviousStopPrice(false)]
	public class TrailingPercentage_Condition_Target : UserScriptedCondition
	{
		#region Variables

		private double _percentage = 1.5;

		#endregion

		protected override void OnInit()
		{
			IsEntry = false;
			IsStop = true;
			IsTarget = false;
			Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Black), "Occurred"));
			Add(new OutputDescriptor(Color.Orange, "Stop"));
			IsOverlay = true;
            CalculateOnClosedBar = false;
		}

		protected override void OnCalculate()
		{
            Occurred.Set(1);
            Stop.Set(Close[0] * (1 - this.Percentage/100.0));
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
		public DataSeries Stop
		{
			get { return Outputs[1]; }
		}

		public override IList<DataSeries> GetTargets()
		{
			return new[]{Stop};
		}

		[Description("")]
		[InputParameter]
		public double Percentage
		{
			get { return _percentage; }
			set { _percentage = value; }
		}

		#endregion
	}
}
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
/// todo description
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://github.com/ScriptTrading/Basic-Package/blob/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("Condition returns false if trading break is active.")]
	[IsEntryAttribute(true)]
	[IsStopAttribute(false)]
	[IsTargetAttribute(false)]
	[OverrulePreviousStopPrice(false)]
	public class TradingTimeActive_Condition : UserScriptedCondition
	{
        #region Variables

        //private TimeSpan _start = new TimeSpan(12, 00, 00);
        //private TimeSpan _end = new TimeSpan(13, 00, 00);

        private string _start ="12:00:00";
        private string _end = "13:00:00";

        #endregion

        protected override void Initialize()
		{
			IsEntry = true;
			IsStop = false;
			IsTarget = false;
			Add(new Plot(Color.FromKnownColor(KnownColor.Black), "Occurred"));
			//Add(new Plot(Color.FromArgb(255, 183, 128, 170), "Entry"));
			Overlay = false;
		}

		protected override void OnBarUpdate()
		{
            //Print(Time[0]);

            TimeSpan now = Time[0].TimeOfDay;

            if ((now >= TimeSpan.Parse(this.Start)) && (now <= TimeSpan.Parse(this.End)))
            {
                Occurred.Set(0);
            }
            else
            {
                Occurred.Set(1);
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
            return new[] { Entry };
        }

        [Description("Start of the trading break. (e.g. 12:00:00)")]
        [Category("Parameters")]
        [DisplayName("Start")]
        public string Start
        {
            get { return _start; }
            set { _start = value; }
        }

        //[Description("Start of the trading break. (e.g. 12:00:00)")]
        //[Category("Parameters")]
        //[DisplayName("Start")]
        //public TimeSpan Start
        //{
        //    get { return _start; }
        //    set { _start = value; }
        //}

        //[Browsable(false)]
        //public string StartSerialize
        //{
        //    get { return _start.ToString(); }
        //    set { _start = TimeSpan.Parse(value); }
        //}

        [Description("End of the trading break. (e.g. 13:00:00)")]
        [Category("Parameters")]
        [DisplayName("End")]
        public string End
        {
            get { return _end; }
            set { _end = value; }
        }

        //[Description("End of the trading break. (e.g. 13:00:00)")]
        //[Category("Parameters")]
        //[DisplayName("End")]
        //public TimeSpan End
        //{
        //    get { return _end; }
        //    set { _end = value; }
        //}

        //[Browsable(false)]
        //public string EndSerialize
        //{
        //    get { return _end.ToString(); }
        //    set { _end = TimeSpan.Parse(value); }
        //}

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
		/// Condition returns false if trading break is active.
		/// </summary>
		public TradingTimeActive_Condition TradingTimeActive_Condition(System.String start, System.String end)
        {
			return TradingTimeActive_Condition(Input, start, end);
		}

		/// <summary>
		/// Condition returns false if trading break is active.
		/// </summary>
		public TradingTimeActive_Condition TradingTimeActive_Condition(IDataSeries input, System.String start, System.String end)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<TradingTimeActive_Condition>(input, i => i.Start == start && i.End == end);

			if (indicator != null)
				return indicator;

			indicator = new TradingTimeActive_Condition
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							Start = start,
							End = end
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
		/// Condition returns false if trading break is active.
		/// </summary>
		public TradingTimeActive_Condition TradingTimeActive_Condition(System.String start, System.String end)
		{
			return LeadIndicator.TradingTimeActive_Condition(Input, start, end);
		}

		/// <summary>
		/// Condition returns false if trading break is active.
		/// </summary>
		public TradingTimeActive_Condition TradingTimeActive_Condition(IDataSeries input, System.String start, System.String end)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.TradingTimeActive_Condition(input, start, end);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Condition returns false if trading break is active.
		/// </summary>
		public TradingTimeActive_Condition TradingTimeActive_Condition(System.String start, System.String end)
		{
			return LeadIndicator.TradingTimeActive_Condition(Input, start, end);
		}

		/// <summary>
		/// Condition returns false if trading break is active.
		/// </summary>
		public TradingTimeActive_Condition TradingTimeActive_Condition(IDataSeries input, System.String start, System.String end)
		{
			return LeadIndicator.TradingTimeActive_Condition(input, start, end);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Condition returns false if trading break is active.
		/// </summary>
		public TradingTimeActive_Condition TradingTimeActive_Condition(System.String start, System.String end)
		{
			return LeadIndicator.TradingTimeActive_Condition(Input, start, end);
		}

		/// <summary>
		/// Condition returns false if trading break is active.
		/// </summary>
		public TradingTimeActive_Condition TradingTimeActive_Condition(IDataSeries input, System.String start, System.String end)
		{
			return LeadIndicator.TradingTimeActive_Condition(input, start, end);
		}
	}

	#endregion

}

#endregion

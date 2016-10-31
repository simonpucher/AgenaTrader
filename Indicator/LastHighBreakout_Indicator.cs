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
/// Version: 1.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). 
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). ")]
	public class LastHighBreakout_Indicator : UserIndicator
	{
	
	 private bool _showarrows = true;
	 private bool _showindicatorbox = false;
	 private int _days = 7;
	 private Stack<DateTime> lasthighs;
	 
	        private Color _plot0color = Const.DefaultIndicatorColor;
        private int _plot0width = Const.DefaultLineWidth;
        private DashStyle _plot0dashstyle = Const.DefaultIndicatorDashStyle;
	 
		protected override void Initialize()
		{
			Add(new Plot(new Pen(this.Plot0Color, this.Plot0Width), PlotStyle.Line, "TestPlot_Indicator"));
			Overlay = true;
			CalculateOnBarClose = true;
	
		}
		


		protected override void OnBarUpdate()
		{
		if(CurrentBar == 0){
			lasthighs = new Stack<DateTime>();
		}
			//MyPlot1.Set(Input[0]);
			if(HighestBar(High, 365) == 0) {
				//MyPlot1.Set(1.0);
				lasthighs.Push(Time[0]);
				if (ShowArrows)
                {
                    DrawArrowUp("ArrowLong_LHB" + +Bars[0].Time.Ticks, this.AutoScale, 0, Bars[0].Low, Color.LightGreen);
                }
			}
			
			if(lasthighs != null && lasthighs.Count > 0 && lasthighs.Peek() >= Time[0].AddDays(this.Days*(-1))){
			if(this.ShowIndicatorBox){
				MyPlot1.Set(1);
			}

			//DrawDot("DotLong_LHB" + +Bars[0].Time.Ticks, this.AutoScale, 0, Bars[0].High, Color.LightBlue);
			}else {
			if(this.ShowIndicatorBox){
				MyPlot1.Set(0);
			}
			}
			
		}
		
		
        public override string ToString()
        {
            return "Last High Breakout (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "Last High Breakout (I)";
            }
        }

		#region Properties

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries MyPlot1
		{
			get { return Values[0]; }
		}
		
		  /// <summary>
        /// </summary>
        [Description("If true then arrows are drawn on the chart.")]
        [Category("Plots")]
        [DisplayName("Show arrows")]
        public bool ShowArrows
        {
            get { return _showarrows; }
            set { _showarrows = value; }
        }

	  /// <summary>
        /// </summary>
        [Description("If true then indicator box drawn on the chart.")]
        [Category("Plots")]
        [DisplayName("Show indicatorbox")]
        public bool ShowIndicatorBox
        {
            get { return _showindicatorbox; }
            set { _showindicatorbox = value; }
        }
        
         /// <summary>
        /// </summary>
        [Description("Days")]
        [Category("Parameters")]
        [DisplayName("Days")]
        public int Days
        {
            get { return _days; }
            set { _days = value; }
        }
        
         /// <summary>
            /// </summary>
            [Description("Select Color for the indicator.")]
            [Category("Parameters")]
            [DisplayName("Color")]
            public Color Plot0Color
            {
                get { return _plot0color; }
                set { _plot0color = value; }
            }
            // Serialize Color object
            [Browsable(false)]
            public string Plot0ColorSerialize
            {
                get { return SerializableColor.ToString(_plot0color); }
                set { _plot0color = SerializableColor.FromString(value); }
            }

            /// <summary>
            /// </summary>
            [Description("Line width for indicator.")]
            [Category("Plots")]
            [DisplayName("Line width")]
            public int Plot0Width
            {
                get { return _plot0width; }
                set { _plot0width = Math.Max(1, value); }
            }

            /// <summary>
            /// </summary>
            [Description("DashStyle for indicator.")]
            [Category("Plots")]
            [DisplayName("DashStyle")]
            public DashStyle Dash0Style
            {
                get { return _plot0dashstyle; }
                set { _plot0dashstyle = value; }
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
		/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). 
		/// </summary>
		public LastHighBreakout_Indicator LastHighBreakout_Indicator(System.Int32 days, Color plot0Color)
        {
			return LastHighBreakout_Indicator(Input, days, plot0Color);
		}

		/// <summary>
		/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). 
		/// </summary>
		public LastHighBreakout_Indicator LastHighBreakout_Indicator(IDataSeries input, System.Int32 days, Color plot0Color)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<LastHighBreakout_Indicator>(input, i => i.Days == days && i.Plot0Color == plot0Color);

			if (indicator != null)
				return indicator;

			indicator = new LastHighBreakout_Indicator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							Days = days,
							Plot0Color = plot0Color
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
		/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). 
		/// </summary>
		public LastHighBreakout_Indicator LastHighBreakout_Indicator(System.Int32 days, Color plot0Color)
		{
			return LeadIndicator.LastHighBreakout_Indicator(Input, days, plot0Color);
		}

		/// <summary>
		/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). 
		/// </summary>
		public LastHighBreakout_Indicator LastHighBreakout_Indicator(IDataSeries input, System.Int32 days, Color plot0Color)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.LastHighBreakout_Indicator(input, days, plot0Color);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). 
		/// </summary>
		public LastHighBreakout_Indicator LastHighBreakout_Indicator(System.Int32 days, Color plot0Color)
		{
			return LeadIndicator.LastHighBreakout_Indicator(Input, days, plot0Color);
		}

		/// <summary>
		/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). 
		/// </summary>
		public LastHighBreakout_Indicator LastHighBreakout_Indicator(IDataSeries input, System.Int32 days, Color plot0Color)
		{
			return LeadIndicator.LastHighBreakout_Indicator(input, days, plot0Color);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). 
		/// </summary>
		public LastHighBreakout_Indicator LastHighBreakout_Indicator(System.Int32 days, Color plot0Color)
		{
			return LeadIndicator.LastHighBreakout_Indicator(Input, days, plot0Color);
		}

		/// <summary>
		/// This indicator shows an arrow on a new x days. The indicator will plot 1 if there was a high in a specific range (default: 52 week high in a 7 days range). 
		/// </summary>
		public LastHighBreakout_Indicator LastHighBreakout_Indicator(IDataSeries input, System.Int32 days, Color plot0Color)
		{
			return LeadIndicator.LastHighBreakout_Indicator(input, days, plot0Color);
		}
	}

	#endregion

}

#endregion

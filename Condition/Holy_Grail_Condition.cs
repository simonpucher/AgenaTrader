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
/// Description http://lindaraschke.net/wp-content/uploads/2013/11/august1997.pdf
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
    public class Holy_Grail_Condition : UserScriptedCondition
	{
        #region Variables

        private Color _plot0color = Color.Orange;
        private int _plot0width = 1;
        private DashStyle _plot0dashstyle = DashStyle.Solid;

        #endregion

      


        protected override void Initialize()
		{
			IsEntry = true;
			IsStop = false;
			IsTarget = false;

            Add(new Plot(this.Plot0Color, "Occurred"));
            Add(new Plot(this.Plot0Color, "Entry"));

            Overlay = false;
            CalculateOnBarClose = false; //do not change this
        }

		protected override void OnBarUpdate()
		{
 


            Occurred.Set(LeadIndicator.Holy_Grail_Indicator(this.Input)[0]);

            PlotColors[0][0] = this.Plot0Color;
            Plots[0].PenStyle = this.Dash0Style;
            Plots[0].Pen.Width = this.Plot0Width;
            
        }


        public override string DisplayName
        {
            get
            {
                return "Holy Grail (C)";
            }
        }


        public override string ToString()
        {
            return "Holy Grail (C)";
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
        [Description("Select Color for the indicator.")]
        [Category("Plots")]
        [DisplayName("High line color")]
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
        [DisplayName("High line width")]
        public int Plot0Width
        {
            get { return _plot0width; }
            set { _plot0width = Math.Max(1, value); }
        }

        /// <summary>
        /// </summary>
        [Description("DashStyle for indicator.")]
        [Category("Plots")]
        [DisplayName("High line dash style")]
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
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Holy_Grail_Condition Holy_Grail_Condition()
        {
			return Holy_Grail_Condition(Input);
		}

		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Holy_Grail_Condition Holy_Grail_Condition(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Holy_Grail_Condition>(input);

			if (indicator != null)
				return indicator;

			indicator = new Holy_Grail_Condition
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
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Holy_Grail_Condition Holy_Grail_Condition()
		{
			return LeadIndicator.Holy_Grail_Condition(Input);
		}

		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Holy_Grail_Condition Holy_Grail_Condition(IDataSeries input)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.Holy_Grail_Condition(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Holy_Grail_Condition Holy_Grail_Condition()
		{
			return LeadIndicator.Holy_Grail_Condition(Input);
		}

		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Holy_Grail_Condition Holy_Grail_Condition(IDataSeries input)
		{
			return LeadIndicator.Holy_Grail_Condition(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Holy_Grail_Condition Holy_Grail_Condition()
		{
			return LeadIndicator.Holy_Grail_Condition(Input);
		}

		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Holy_Grail_Condition Holy_Grail_Condition(IDataSeries input)
		{
			return LeadIndicator.Holy_Grail_Condition(input);
		}
	}

	#endregion

}

#endregion

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
/// Inspired by https://www.traderfox.de/nachrichten/blog/19-trefferquote-von-97-handelssignal-lonely-warrior
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
	public class Lonely_Warrior_Indicator : UserIndicator
	{

        //input
        private Color _plot0color = Const.DefaultIndicatorColor;
        private int _plot0width = Const.DefaultLineWidth;
        private DashStyle _plot0dashstyle = Const.DefaultIndicatorDashStyle;
        private Color _plot1color = Const.DefaultIndicatorColor_GreyedOut;
        private int _plot1width = Const.DefaultLineWidth;
        private DashStyle _plot1dashstyle = Const.DefaultIndicatorDashStyle;


        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            //Print("Initialize");

            Add(new Plot(new Pen(this.Plot0Color, this.Plot0Width), PlotStyle.Line, "Plot_High"));
            Add(new Plot(new Pen(this.Plot1Color, this.Plot1Width), PlotStyle.Line, "Plot_Middle"));
            Add(new Plot(new Pen(this.Plot0Color, this.Plot0Width), PlotStyle.Line, "Plot_Low"));

            CalculateOnBarClose = true;
            Overlay = true;

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
            Bollinger bb = Bollinger(1, 20);

            Plot_High.Set(bb.Upper[0]);
            Plot_Middle.Set(bb.Middle[0]);
            Plot_Low.Set(bb.Lower[0]);


        }


        public override string ToString()
        {
            return "Lonely Warrior (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "Lonely Warrior (I)";
            }
        }

        #region Properties

        [Browsable(false)]
		[XmlIgnore()]
		public DataSeries Plot_High
		{
			get { return Values[0]; }
		}

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Plot_Middle
        {
            get { return Values[1]; }
        }


        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Plot_Low
        {
            get { return Values[2]; }
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

        /// <summary>
        /// </summary>
        [Description("Select color for the indicator.")]
        [Category("Plots")]
        [DisplayName("Color")]
        public Color Plot1Color
        {
            get { return _plot1color; }
            set { _plot1color = value; }
        }
        // Serialize Color object
        [Browsable(false)]
        public string Plot1ColorSerialize
        {
            get { return SerializableColor.ToString(_plot1color); }
            set { _plot1color = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Line width for indicator.")]
        [Category("Plots")]
        [DisplayName("Line width")]
        public int Plot1Width
        {
            get { return _plot1width; }
            set { _plot1width = Math.Max(1, value); }
        }

        /// <summary>
        /// </summary>
        [Description("DashStyle for indicator.")]
        [Category("Plots")]
        [DisplayName("DashStyle")]
        public DashStyle Dash1Style
        {
            get { return _plot1dashstyle; }
            set { _plot1dashstyle = value; }
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
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator(Color plot0Color)
        {
			return Lonely_Warrior_Indicator(Input, plot0Color);
		}

		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator(IDataSeries input, Color plot0Color)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Lonely_Warrior_Indicator>(input, i => i.Plot0Color == plot0Color);

			if (indicator != null)
				return indicator;

			indicator = new Lonely_Warrior_Indicator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
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
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator(Color plot0Color)
		{
			return LeadIndicator.Lonely_Warrior_Indicator(Input, plot0Color);
		}

		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator(IDataSeries input, Color plot0Color)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.Lonely_Warrior_Indicator(input, plot0Color);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator(Color plot0Color)
		{
			return LeadIndicator.Lonely_Warrior_Indicator(Input, plot0Color);
		}

		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator(IDataSeries input, Color plot0Color)
		{
			return LeadIndicator.Lonely_Warrior_Indicator(input, plot0Color);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator(Color plot0Color)
		{
			return LeadIndicator.Lonely_Warrior_Indicator(Input, plot0Color);
		}

		/// <summary>
		/// Watch out for the lonely warrior behind enemy lines.
		/// </summary>
		public Lonely_Warrior_Indicator Lonely_Warrior_Indicator(IDataSeries input, Color plot0Color)
		{
			return LeadIndicator.Lonely_Warrior_Indicator(input, plot0Color);
		}
	}

	#endregion

}

#endregion

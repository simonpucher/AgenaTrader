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
/// Version: 1.1.3
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// This indicator provides entry and exit signals on time.
/// Long signal in every even minute. Short signal every odd minute.
/// You can use this indicator also as a template for further script development.
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{

    [Description("This indicator provides a long signal in every even minute and a short signal every odd minute.")]
    public class DummyOneMinuteEvenOdd_Indicator : UserIndicator
	{
        //input
        private Color _plot0color = Color.Orange;
        private int _plot0width = 2;
        private DashStyle _plot0dashstyle = DashStyle.Solid;
        //output

        //internal

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
		protected override void Initialize()
		{
            //Print("Initialize");

            Add(new Plot(new Pen(this.Plot0Color, this.Plot0Width), PlotStyle.Line, "DummyOneMinuteEvenOdd_Plot_Indicator"));

			CalculateOnBarClose = true;
            Overlay = false;

            //Because of Backtesting reasons if we use the advanced mode we need at least two bars
            this.BarsRequired = 2;
		}

        protected override void InitRequirements()
        {
            //Print("InitRequirements");
            base.InitRequirements();
        }


        /// <summary>
        /// Is called on startup.
        /// </summary>
        protected override void OnStartUp()
        {
            //Print("OnStartUp");
            base.OnStartUp();

        }

        /// <summary>
        /// Called on each update of the bar.
        /// </summary>
        protected override void OnBarUpdate()
        {
            //Print("OnBarUpdate");

            /*
         * Using modulus to check if one condition meets our entry trading plan:
         * + We sell SHORT every odd minute
         * + We buy LONG every even minute
         * + In all other cases we return null 
         */
            if (Bars[0].Time.Minute % 2 == 0)
            {
                this.Indicator_Curve_Entry.Set(1);
            }
            else if (Bars[0].Time.Minute % 2 != 0)
            {
                this.Indicator_Curve_Entry.Set(-1);
            }
            else
            {
                this.Indicator_Curve_Entry.Set(0);
            }


           // int returnvalue = this.calculate(Input, this.IsLongEnabled, this.IsShortEnabled);
          

        }

        /// <summary>
        /// Is called if the indicator stops.
        /// </summary>
        protected override void OnTermination()
        {
            //Print("OnTermination");
        }


        public override string ToString()
        {
            return "Dummy even/odd (S)";
        }

        public override string DisplayName
        {
            get
            {
                return "Dummy even/odd (S)";
            }
        }


        /// <summary>
        /// True if the periodicity of the data feed is correct for this indicator.
        /// </summary>
        /// <returns></returns>
        public bool DatafeedPeriodicityIsValid(ITimeFrame timeframe)
        {
            TimeFrame tf = (TimeFrame)timeframe;
            if (tf.Periodicity == DatafeedHistoryPeriodicity.Minute && tf.PeriodicityValue == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        #region Properties

       

        #region Input
            /// <summary>
            /// </summary>
            [Description("Select Color for the indicator.")]
            [Category("Plots")]
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

            ///// <summary>
            ///// </summary>
            //[Description("Select color for the indicator.")]
            //[Category("Plots")]
            //[DisplayName("Color")]
            //public Color Plot1Color
            //{
            //    get { return _plot1color; }
            //    set { _plot1color = value; }
            //}
            //// Serialize Color object
            //[Browsable(false)]
            //public string Plot1ColorSerialize
            //{
            //    get { return SerializableColor.ToString(_plot1color); }
            //    set { _plot1color = SerializableColor.FromString(value); }
            //}

            ///// <summary>
            ///// </summary>
            //[Description("Line width for indicator.")]
            //[Category("Plots")]
            //[DisplayName("Line width")]
            //public int Plot1Width
            //{
            //    get { return _plot1width; }
            //    set { _plot1width = Math.Max(1, value); }
            //}

            ///// <summary>
            ///// </summary>
            //[Description("DashStyle for indicator.")]
            //[Category("Plots")]
            //[DisplayName("DashStyle")]
            //public DashStyle Dash1Style
            //{
            //    get { return _plot1dashstyle; }
            //    set { _plot1dashstyle = value; }
            //}


         

        #endregion
  

            #region Output

            [Browsable(false)]
            [XmlIgnore()]
            public DataSeries Indicator_Curve_Entry
            {
                get { return Values[0]; }
            }

       

            #endregion




        #endregion

    }
}
#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator : Indicator
	{
		/// <summary>
		/// This indicator provides a long signal in every even minute and a short signal every odd minute.
		/// </summary>
		public DummyOneMinuteEvenOdd_Indicator DummyOneMinuteEvenOdd_Indicator()
        {
			return DummyOneMinuteEvenOdd_Indicator(Input);
		}

		/// <summary>
		/// This indicator provides a long signal in every even minute and a short signal every odd minute.
		/// </summary>
		public DummyOneMinuteEvenOdd_Indicator DummyOneMinuteEvenOdd_Indicator(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<DummyOneMinuteEvenOdd_Indicator>(input);

			if (indicator != null)
				return indicator;

			indicator = new DummyOneMinuteEvenOdd_Indicator
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
		/// This indicator provides a long signal in every even minute and a short signal every odd minute.
		/// </summary>
		public DummyOneMinuteEvenOdd_Indicator DummyOneMinuteEvenOdd_Indicator()
		{
			return LeadIndicator.DummyOneMinuteEvenOdd_Indicator(Input);
		}

		/// <summary>
		/// This indicator provides a long signal in every even minute and a short signal every odd minute.
		/// </summary>
		public DummyOneMinuteEvenOdd_Indicator DummyOneMinuteEvenOdd_Indicator(IDataSeries input)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.DummyOneMinuteEvenOdd_Indicator(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// This indicator provides a long signal in every even minute and a short signal every odd minute.
		/// </summary>
		public DummyOneMinuteEvenOdd_Indicator DummyOneMinuteEvenOdd_Indicator()
		{
			return LeadIndicator.DummyOneMinuteEvenOdd_Indicator(Input);
		}

		/// <summary>
		/// This indicator provides a long signal in every even minute and a short signal every odd minute.
		/// </summary>
		public DummyOneMinuteEvenOdd_Indicator DummyOneMinuteEvenOdd_Indicator(IDataSeries input)
		{
			return LeadIndicator.DummyOneMinuteEvenOdd_Indicator(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// This indicator provides a long signal in every even minute and a short signal every odd minute.
		/// </summary>
		public DummyOneMinuteEvenOdd_Indicator DummyOneMinuteEvenOdd_Indicator()
		{
			return LeadIndicator.DummyOneMinuteEvenOdd_Indicator(Input);
		}

		/// <summary>
		/// This indicator provides a long signal in every even minute and a short signal every odd minute.
		/// </summary>
		public DummyOneMinuteEvenOdd_Indicator DummyOneMinuteEvenOdd_Indicator(IDataSeries input)
		{
			return LeadIndicator.DummyOneMinuteEvenOdd_Indicator(input);
		}
	}

	#endregion

}

#endregion


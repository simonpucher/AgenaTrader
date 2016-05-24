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
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// This indicator provides entry and exit signals on time.
/// Long signal in every even minute. Short signal every odd minute.
/// You can use this indicator also as a template for further script development.
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{

    /// <summary>
    /// We use this interface to ensure that indicator, condition, strategy and alert all use the same properties and methods. 
    /// </summary>
    public interface IDummyOneMinuteEvenOdd_Advanced
    {
        //input
        bool IsShortEnabled { get; set; }
        bool IsLongEnabled { get; set; }

        //internal
        bool ErrorOccured { get; set; }
        bool WarningOccured { get; set; }
    }



    [Description("This indicator provides a long signal in every even minute and a short signal every odd minute.")]
    public class DummyOneMinuteEvenOdd_Advanced_Indicator : UserIndicator, IDummyOneMinuteEvenOdd_Advanced
	{
        //interface 
        private bool _IsShortEnabled = true;
        private bool _IsLongEnabled = true;
        private bool _WarningOccured = false;
        private bool _ErrorOccured = false;

        //input
        private Color _plot0color = Const.DefaultIndicatorColor;
        private int _plot0width = Const.DefaultLineWidth;
        private DashStyle _plot0dashstyle = Const.DefaultIndicatorDashStyle;
        private Color _plot1color = Const.DefaultIndicatorColor_GreyedOut;
        private int _plot1width = Const.DefaultLineWidth;
        private DashStyle _plot1dashstyle = Const.DefaultIndicatorDashStyle;


        //output

        //internal
        //private DataSeries _DataSeries_List = null;

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
		protected override void Initialize()
		{
            //Print("Initialize");

            Add(new Plot(new Pen(this.Plot0Color, this.Plot0Width), PlotStyle.Line, "DummyOneMinuteEvenOdd_Advanced_Plot_Indicator"));
            Add(new Plot(new Pen(this.Plot1Color, this.Plot1Width), PlotStyle.Line, "DummyOneMinuteEvenOdd_Advanced_Plot_GreyedOut_Indicator"));

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

            this.ErrorOccured = false;
            this.WarningOccured = false;
        }

        /// <summary>
        /// Called on each update of the bar.
        /// </summary>
        protected override void OnBarUpdate()
        {
            //Print("OnBarUpdate");

            //Check if peridocity is valid for this script
            if (!DatafeedPeriodicityIsValid(Bars.TimeFrame))
            {
                //Display warning just one time
                if (!this.WarningOccured)
                {
                    GlobalUtilities.DrawWarningTextOnChart(this, Const.DefaultStringDatafeedPeriodicity);
                    this.WarningOccured = true;
                }
                return; 
            }
           
            //Lets call the calculate method and save the result with the trade action
            ResultValue returnvalue = this.calculate(Bars[0], this.IsLongEnabled, this.IsShortEnabled);

            //If the calculate method was not finished we need to stop and show an alert message to the user.
            if (returnvalue.ErrorOccured)
            {
                //Display error just one time
                if (!this.ErrorOccured)
                {
                    GlobalUtilities.DrawAlertTextOnChart(this, Const.DefaultStringErrorDuringCalculation);
                    this.ErrorOccured = true;
                }
                return;
            }

            //Entry
            if (returnvalue.Entry.HasValue)
            {
                switch (returnvalue.Entry)
                {
                    case OrderAction.Buy:
                        //DrawDot("ArrowLong_Entry" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.LightGreen);
                        this.Indicator_Curve_Entry.Set(1);
                        break;
                    case OrderAction.SellShort:
                        //DrawDiamond("ArrowShort_Entry" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.LightGreen);
                        this.Indicator_Curve_Entry.Set(-1);
                        break;
                }
            }
            else
            {
                //Value was null so nothing to do.
                this.Indicator_Curve_Entry.Set(0);
            }

            //Exit
            if (returnvalue.Exit.HasValue)
            {
                switch (returnvalue.Exit)
                {
                    case OrderAction.BuyToCover:
                        //DrawDiamond("ArrowShort_Exit" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.Red);
                        this.Indicator_Curve_Exit.Set(0.5);
                        break;
                    case OrderAction.Sell:
                        //DrawDot("ArrowLong_Exit" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.Red);
                        this.Indicator_Curve_Exit.Set(-0.5);
                        break;
                }
            }
            else
            {
                //Value was null so nothing to do.
                this.Indicator_Curve_Exit.Set(0);
            }
        }

        /// <summary>
        /// Is called if the indicator stops.
        /// </summary>
        protected override void OnTermination()
        {
            //Print("OnTermination");
        }


        /// <summary>
        /// In this method we do all the work and return the object with all data like the OrderActions.
        /// This method can be called from any other script like strategies, indicators or conditions.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultValue calculate(IBar data, bool islongenabled, bool isshortenabled)
        {
            //Create a return object
            ResultValue returnvalue = new ResultValue();

            //try catch block with all calculations
            try
            {
                 /*
                 * Using modulus to check if one condition meets our entry trading plan:
                 * + We sell SHORT every odd minute
                 * + We buy LONG every even minute
                 * + In all other cases we return null 
                 */
                if (islongenabled)
                {
                    if (data.Time.Minute % 2 == 0)
                    {
                        returnvalue.Entry = OrderAction.Buy;
                    } else if (data.Time.Minute % 2 != 0)
                    {
                        returnvalue.Exit = OrderAction.Sell;
                    }
                }

                /*
                 * Using modulus to check if one condition meets our exit trading plan:
                 * + We cover the SHORT position every even minute
                 * + We sell the LONG position every odd minute
                 * + In all other cases we return null 
                 */
                if (isshortenabled)
                {
                    if (data.Time.Minute % 2 == 0)
                    {
                        returnvalue.Exit = OrderAction.BuyToCover;
                    } 
                    else if (data.Time.Minute % 2 != 0)
                    {
                        returnvalue.Entry = OrderAction.SellShort;
                    }
                }

            }
            catch (Exception)
            {
                //If this method is called via a strategy or a condition we need to log the error.
                returnvalue.ErrorOccured = true;
            }

            //return the result object
            return returnvalue;
        }




        public override string ToString()
        {
            return "Dummy one minute even/odd advanced (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "Dummy one minute even/odd advanced (I)";
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


        #region Interface

        #endregion

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

            #region Interface

            /// <summary>
            /// </summary>
            [Description("If true it is allowed to create long positions.")]
            [Category("Parameters")]
            [DisplayName("Allow Long")]
            public bool IsLongEnabled
            {
                get { return _IsLongEnabled; }
                set { _IsLongEnabled = value; }
            }


            /// <summary>
            /// </summary>
            [Description("If true it is allowed to create short positions.")]
            [Category("Parameters")]
            [DisplayName("Allow Short")]
            public bool IsShortEnabled
            {
                get { return _IsShortEnabled; }
                set { _IsShortEnabled = value; }
            }


            [Browsable(false)]
            [XmlIgnore()]
            public bool ErrorOccured
            {
                get { return _ErrorOccured; }
                set { _ErrorOccured = value; }
            }

            [Browsable(false)]
            [XmlIgnore()]
            public bool WarningOccured
            {
                get { return _WarningOccured; }
                set { _WarningOccured = value; }
            }

            #endregion

            #region Output

            [Browsable(false)]
            [XmlIgnore()]
            public DataSeries Indicator_Curve_Entry
            {
                get { return Values[0]; }
            }

            [Browsable(false)]
            [XmlIgnore()]
            public DataSeries Indicator_Curve_Exit
            {
                get { return Values[1]; }
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
		public DummyOneMinuteEvenOdd_Advanced_Indicator DummyOneMinuteEvenOdd_Advanced_Indicator(System.Boolean isLongEnabled, System.Boolean isShortEnabled)
        {
			return DummyOneMinuteEvenOdd_Advanced_Indicator(Input, isLongEnabled, isShortEnabled);
		}

		/// <summary>
		/// This indicator provides a long signal in every even minute and a short signal every odd minute.
		/// </summary>
		public DummyOneMinuteEvenOdd_Advanced_Indicator DummyOneMinuteEvenOdd_Advanced_Indicator(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<DummyOneMinuteEvenOdd_Advanced_Indicator>(input, i => i.IsLongEnabled == isLongEnabled && i.IsShortEnabled == isShortEnabled);

			if (indicator != null)
				return indicator;

			indicator = new DummyOneMinuteEvenOdd_Advanced_Indicator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							IsLongEnabled = isLongEnabled,
							IsShortEnabled = isShortEnabled
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
		public DummyOneMinuteEvenOdd_Advanced_Indicator DummyOneMinuteEvenOdd_Advanced_Indicator(System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.DummyOneMinuteEvenOdd_Advanced_Indicator(Input, isLongEnabled, isShortEnabled);
		}

		/// <summary>
		/// This indicator provides a long signal in every even minute and a short signal every odd minute.
		/// </summary>
		public DummyOneMinuteEvenOdd_Advanced_Indicator DummyOneMinuteEvenOdd_Advanced_Indicator(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.DummyOneMinuteEvenOdd_Advanced_Indicator(input, isLongEnabled, isShortEnabled);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// This indicator provides a long signal in every even minute and a short signal every odd minute.
		/// </summary>
		public DummyOneMinuteEvenOdd_Advanced_Indicator DummyOneMinuteEvenOdd_Advanced_Indicator(System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.DummyOneMinuteEvenOdd_Advanced_Indicator(Input, isLongEnabled, isShortEnabled);
		}

		/// <summary>
		/// This indicator provides a long signal in every even minute and a short signal every odd minute.
		/// </summary>
		public DummyOneMinuteEvenOdd_Advanced_Indicator DummyOneMinuteEvenOdd_Advanced_Indicator(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.DummyOneMinuteEvenOdd_Advanced_Indicator(input, isLongEnabled, isShortEnabled);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// This indicator provides a long signal in every even minute and a short signal every odd minute.
		/// </summary>
		public DummyOneMinuteEvenOdd_Advanced_Indicator DummyOneMinuteEvenOdd_Advanced_Indicator(System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.DummyOneMinuteEvenOdd_Advanced_Indicator(Input, isLongEnabled, isShortEnabled);
		}

		/// <summary>
		/// This indicator provides a long signal in every even minute and a short signal every odd minute.
		/// </summary>
		public DummyOneMinuteEvenOdd_Advanced_Indicator DummyOneMinuteEvenOdd_Advanced_Indicator(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.DummyOneMinuteEvenOdd_Advanced_Indicator(input, isLongEnabled, isShortEnabled);
		}
	}

	#endregion

}

#endregion


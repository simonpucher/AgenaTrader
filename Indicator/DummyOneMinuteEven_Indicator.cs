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
/// Version: 1.1
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// This indicator provides entry and exit signals on time.
/// Long signal in every even minute. Short signal every odd minute.
/// You can use this indicator also as a template for further development.
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
    public interface IDummyOneMinuteEven
    {
        //input
        bool IsShortEnabled { get; set; }
        bool IsLongEnabled { get; set; }
    }


    /// <summary>
    /// Class which holds all important data like the OrderAction. 
    /// </summary>
    public class ResultValueDummyOneMinuteEven {

        //Output
        public bool IsCompleted = false;
        public OrderAction? Entry = null;
        public OrderAction? Exit = null;

        public ResultValueDummyOneMinuteEven()
        {

        }
    }



    [Description("This indicator provides a long signal in every even minute and a short signal every odd minute.")]
    public class DummyOneMinuteEven_Indicator : UserIndicator, IDummyOneMinuteEven
	{
        //interface 

        //input
        private Color _plot0color = Const.DefaultIndicatorColor;
        private int _plot0width = Const.DefaultLineWidth;
        private DashStyle _plot0dashstyle = Const.DefaultIndicatorDashStyle;
        private Color _plot1color = Const.DefaultIndicatorColor_GreyedOut;
        private int _plot1width = Const.DefaultLineWidth;
        private DashStyle _plot1dashstyle = Const.DefaultIndicatorDashStyle;
        private bool _IsShortEnabled = true;
        private bool _IsLongEnabled = true;

        //output

        //internal
        //private DataSeries _DataSeries_List = null;

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
		protected override void Initialize()
		{
            //Print("Initialize");

            Add(new Plot(new Pen(this.Plot0Color, this.Plot0Width), PlotStyle.Line, "EvenMinutePlot_Indicator"));
            Add(new Plot(new Pen(this.Plot1Color, this.Plot1Width), PlotStyle.Line, "EvenMinutePlot_GreyedOut_Indicator"));

            BarsRequired = 1;
			CalculateOnBarClose = true;
            Overlay = false;
		}


        /// <summary>
        /// Is called on startup.
        /// </summary>
        protected override void OnStartUp()
        {
            //Print("OnStartUp");

            //this._DataSeries_List = new DataSeries(this);
        }

        /// <summary>
        /// Called on each update of the bar.
        /// </summary>
        protected override void OnBarUpdate()
        {
            //Print("OnBarUpdate");

            //todo check peridocity

            //if (Bars != null && Bars.Count > 0
            //             && TimeFrame.Periodicity == DatafeedHistoryPeriodicity.Minute
            //             && TimeFrame.PeriodicityValue == 1)
            //{ 
            //    //
            //}
            //else
            //{
            //    return;
            //}

            //Lets call the calculate method and save the result with the trade action
            ResultValueDummyOneMinuteEven returnvalue = this.calculate(Bars[0], this.IsLongEnabled, this.IsShortEnabled);

            //If the calculate method was not finished we need to stop and show an alert message to the user.
            if (!returnvalue.IsCompleted)
            {
                GlobalUtilities.DrawAlertTextOnChart(this, Const.DefaultStringErrorDuringCalculation);
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
        /// In this method we do all the work and return the OrderAction.
        /// This method can be called from any other script.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultValueDummyOneMinuteEven calculate(IBar data, bool islongenabled, bool isshortenabled)
        {
            ResultValueDummyOneMinuteEven returnvalue = new ResultValueDummyOneMinuteEven();

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

                //Everything is fine
                returnvalue.IsCompleted = true;

            }
            catch (Exception ex)
            {
                //If this method is called via a strategy or a condition we need to log the error.
                Log(this.DisplayName + ": " + Const.DefaultStringErrorDuringCalculation + " - " + ex.ToString(), InfoLogLevel.AlertLog);
                returnvalue.IsCompleted = false;
            }

            //return the result object
            return returnvalue;
        }




        public override string ToString()
        {
            return "Dummy one minute even (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "Dummy one minute even (I)";
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
		public DummyOneMinuteEven_Indicator DummyOneMinuteEven_Indicator(System.Boolean isLongEnabled, System.Boolean isShortEnabled)
        {
			return DummyOneMinuteEven_Indicator(Input, isLongEnabled, isShortEnabled);
		}

		/// <summary>
		/// This indicator provides a long signal in every even minute and a short signal every odd minute.
		/// </summary>
		public DummyOneMinuteEven_Indicator DummyOneMinuteEven_Indicator(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<DummyOneMinuteEven_Indicator>(input, i => i.IsLongEnabled == isLongEnabled && i.IsShortEnabled == isShortEnabled);

			if (indicator != null)
				return indicator;

			indicator = new DummyOneMinuteEven_Indicator
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
		public DummyOneMinuteEven_Indicator DummyOneMinuteEven_Indicator(System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.DummyOneMinuteEven_Indicator(Input, isLongEnabled, isShortEnabled);
		}

		/// <summary>
		/// This indicator provides a long signal in every even minute and a short signal every odd minute.
		/// </summary>
		public DummyOneMinuteEven_Indicator DummyOneMinuteEven_Indicator(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.DummyOneMinuteEven_Indicator(input, isLongEnabled, isShortEnabled);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// This indicator provides a long signal in every even minute and a short signal every odd minute.
		/// </summary>
		public DummyOneMinuteEven_Indicator DummyOneMinuteEven_Indicator(System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.DummyOneMinuteEven_Indicator(Input, isLongEnabled, isShortEnabled);
		}

		/// <summary>
		/// This indicator provides a long signal in every even minute and a short signal every odd minute.
		/// </summary>
		public DummyOneMinuteEven_Indicator DummyOneMinuteEven_Indicator(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.DummyOneMinuteEven_Indicator(input, isLongEnabled, isShortEnabled);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// This indicator provides a long signal in every even minute and a short signal every odd minute.
		/// </summary>
		public DummyOneMinuteEven_Indicator DummyOneMinuteEven_Indicator(System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.DummyOneMinuteEven_Indicator(Input, isLongEnabled, isShortEnabled);
		}

		/// <summary>
		/// This indicator provides a long signal in every even minute and a short signal every odd minute.
		/// </summary>
		public DummyOneMinuteEven_Indicator DummyOneMinuteEven_Indicator(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.DummyOneMinuteEven_Indicator(input, isLongEnabled, isShortEnabled);
		}
	}

	#endregion

}

#endregion


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
/// Description: https://en.wikipedia.org/wiki/Algorithmic_trading#Mean_reversion
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this indicator without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    public interface IMean_Reversion
    {
        //input
        bool IsShortEnabled { get; set; }
        bool IsLongEnabled { get; set; }
         int Bollinger_Period  { get; set; }
         double Bollinger_Standard_Deviation  { get; set; }
        int Momentum_Period  { get; set; }
        int RSI_Period  { get; set; }
        int RSI_Smooth { get; set; }
        int RSI_Level_Low  { get; set; }
        int RSI_Level_High { get; set; }
        int Momentum_Level_Low  { get; set; }
        int Momentum_Level_High { get; set; }

        //internal
        bool ErrorOccured { get; set; }
        bool WarningOccured { get; set; }

    }

	[Description("The mean reversion is the theory suggesting that prices and returns eventually move back towards the mean or average.")]
    public class Mean_Reversion_Indicator : UserIndicator, IMean_Reversion
	{

        //interface 
        private bool _IsShortEnabled = false;
        private bool _IsLongEnabled = true;
        private bool _WarningOccured = false;
        private bool _ErrorOccured = false;
        private int _Bollinger_Period = 20;
        private double _Bollinger_Standard_Deviation = 2;
        private int _Momentum_Period = 100;
        private int _RSI_Period = 14;
        private int _RSI_Smooth = 3;
        private int _RSI_Level_Low = 30;
        private int _RSI_Level_High = 70;
        private int _Momentum_Level_Low = -1;
        private int _Momentum_Level_High = 1;

        //internal
        double bb_upper = Double.MinValue;
        double bb_middle = Double.MinValue;
        double bb_lower = Double.MinValue;

		protected override void Initialize()
		{
            //Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));

            Add(new Plot(new Pen(Color.FromKnownColor(KnownColor.LightGray), 2), PlotStyle.Line, "BBUpper")); 
            Add(new Plot(new Pen(Color.FromKnownColor(KnownColor.Orange), 2), PlotStyle.Line, "BBMiddle"));
            Add(new Plot(new Pen(Color.FromKnownColor(KnownColor.LightGray), 2), PlotStyle.Line, "BBLower"));

			CalculateOnBarClose = true;
            Overlay = true;
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


   

		protected override void OnBarUpdate()
		{

            ////we need more contrast
            //this.BarColor = Color.White;
    
            //calculate data
            ResultValue returnvalue = this.calculate(Input, Open, High, null, null, this.Bollinger_Period, this.Bollinger_Standard_Deviation, this.Momentum_Period, this.RSI_Period, this.RSI_Smooth, this.RSI_Level_Low, this.RSI_Level_High, this.Momentum_Level_Low, this.Momentum_Level_High);

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

            Plot_1.Set(bb_upper);
            Plot_2.Set(bb_middle);
            Plot_3.Set(bb_lower);

            //Entry
            if (returnvalue.Entry.HasValue)
            {
                switch (returnvalue.Entry)
                {
                    case OrderAction.Buy:
                        DrawDot("ArrowLong_Entry" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.LightGreen);
                        //this.Indicator_Curve_Entry.Set(1);
                        break;
                    case OrderAction.SellShort:
                        DrawDiamond("ArrowShort_Entry" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.LightGreen);
                        //this.Indicator_Curve_Entry.Set(-1);
                        break;
                }
            }
            //else
            //{
            //    //Value was null so nothing to do.
            //    this.Indicator_Curve_Entry.Set(0);
            //}

            //Exit
            if (returnvalue.Exit.HasValue)
            {
                switch (returnvalue.Exit)
                {
                    case OrderAction.BuyToCover:
                        DrawDiamond("ArrowShort_Exit" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.Red);
                        //this.Indicator_Curve_Exit.Set(0.5);
                        break;
                    case OrderAction.Sell:
                        DrawDot("ArrowLong_Exit" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.Red);
                        //this.Indicator_Curve_Exit.Set(-0.5);
                        break;
                }
            }
            //else
            //{
            //    //Value was null so nothing to do.
            //    this.Indicator_Curve_Exit.Set(0);
            //}

		}


        /// <summary>
        /// In this function we do all the work and send back the OrderAction.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultValue calculate(IDataSeries data, IDataSeries open, IDataSeries high, IOrder longorder, IOrder shortorder, int bollinger_period, double bollinger_standarddeviation, int momentum_period, int rsi_period, int rsi_smooth, int rsi_level_low, int rsi_level_high, int momentum_level_low, int momentum_level_high)
        {
            //Create a return object
            ResultValue returnvalue = new ResultValue();

            try
            {
                
                //Calculate BB
                Bollinger bb = Bollinger(data, bollinger_standarddeviation, bollinger_period);
                Momentum mom = Momentum(data, momentum_period);
                RSI rsi = RSI(data, rsi_period, rsi_smooth);

                bb_lower = bb.Lower[0];
                bb_middle = bb.Middle[0];
                bb_upper = bb.Upper[0];


                if (mom[0] >= momentum_level_high && rsi[0] <= rsi_level_low && data[0] <= bb.Lower[0] && data[1] <= bb.Lower[1] && data[2] <= bb.Lower[2])
                {
                    returnvalue.Entry = OrderAction.Buy;
                }
                else if (mom[0] <= momentum_level_low && rsi[0] >= rsi_level_high && data[0] >= bb.Upper[0] && data[1] >= bb.Upper[1] && data[2] >= bb.Upper[2])
                {
                    returnvalue.Entry = OrderAction.SellShort;
                }
                else if (data[0] >= bb.Upper[0] && longorder != null)
                {
                    //currently we left the building on the upper band, is it better if we switch to a stop?
                    returnvalue.Exit = OrderAction.Sell;
                }
                else if (data[0] <= bb.Lower[0] && shortorder != null)
                {
                    returnvalue.Exit = OrderAction.BuyToCover;
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
            return "Mean Reversion (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "Mean Reversion (I)";
            }
        }


        

		#region Properties

      
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



        [Description("Period of the Bollinger Band.")]
        [Category("Parameters")]
        [DisplayName("BB Period")]
        public int Bollinger_Period
        {
            get { return _Bollinger_Period; }
            set { _Bollinger_Period = value; }
        }

        [Description("Standard Deviation of the Bollinger Band.")]
        [Category("Parameters")]
        [DisplayName("BB StdDev")]
        public double Bollinger_Standard_Deviation
        {
            get { return _Bollinger_Standard_Deviation; }
            set { _Bollinger_Standard_Deviation = value; }
        }

        [Description("Period of the Momentum.")]
        [Category("Parameters")]
        [DisplayName("MOM Period")]
        public int Momentum_Period
        {
            get { return _Momentum_Period; }
            set { _Momentum_Period = value; }
        }

        [Description("Period of the RSI.")]
        [Category("Parameters")]
        [DisplayName("RSI Period")]
        public int RSI_Period
        {
            get { return _RSI_Period; }
            set { _RSI_Period = value; }
        }

        [Description("Smooth Period of the RSI.")]
        [Category("Parameters")]
        [DisplayName("RSI Smooth Period")]
        public int RSI_Smooth
        {
            get { return _RSI_Smooth; }
            set { _RSI_Smooth = value; }
        }

        [Description("We trade long below this RSI level.")]
        [Category("Parameters")]
        [DisplayName("RSI Level Low")]
        public int RSI_Level_Low
        {
            get { return _RSI_Level_Low; }
            set { _RSI_Level_Low = value; }
        }

        [Description("We trade short above this RSI level.")]
        [Category("Parameters")]
        [DisplayName("RSI Level High")]
        public int RSI_Level_High
        {
            get { return _RSI_Level_High; }
            set { _RSI_Level_High = value; }
        }

        [Description("We trade long if momentum is above this level.")]
        [Category("Parameters")]
        [DisplayName("MOM Level Low")]
        public int Momentum_Level_Low
        {
            get { return _Momentum_Level_Low; }
            set { _Momentum_Level_Low = value; }
        }

        [Description("We trade short if momentum is below this level.")]
        [Category("Parameters")]
        [DisplayName("MOM Level High")]
        public int Momentum_Level_High
        {
            get { return _Momentum_Level_High; }
            set { _Momentum_Level_High = value; }
        }
        #endregion

        #region Input

      


        #endregion

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Plot_1 { get { return Values[0]; } }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Plot_2 { get { return Values[1]; } }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Plot_3 { get { return Values[2]; } }

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
		/// The mean reversion is the theory suggesting that prices and returns eventually move back towards the mean or average.
		/// </summary>
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(System.Boolean isLongEnabled, System.Boolean isShortEnabled, System.Int32 bollinger_Period, System.Double bollinger_Standard_Deviation, System.Int32 momentum_Period, System.Int32 rSI_Period, System.Int32 rSI_Smooth, System.Int32 rSI_Level_Low, System.Int32 rSI_Level_High, System.Int32 momentum_Level_Low, System.Int32 momentum_Level_High)
        {
			return Mean_Reversion_Indicator(Input, isLongEnabled, isShortEnabled, bollinger_Period, bollinger_Standard_Deviation, momentum_Period, rSI_Period, rSI_Smooth, rSI_Level_Low, rSI_Level_High, momentum_Level_Low, momentum_Level_High);
		}

		/// <summary>
		/// The mean reversion is the theory suggesting that prices and returns eventually move back towards the mean or average.
		/// </summary>
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled, System.Int32 bollinger_Period, System.Double bollinger_Standard_Deviation, System.Int32 momentum_Period, System.Int32 rSI_Period, System.Int32 rSI_Smooth, System.Int32 rSI_Level_Low, System.Int32 rSI_Level_High, System.Int32 momentum_Level_Low, System.Int32 momentum_Level_High)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Mean_Reversion_Indicator>(input, i => i.IsLongEnabled == isLongEnabled && i.IsShortEnabled == isShortEnabled && i.Bollinger_Period == bollinger_Period && Math.Abs(i.Bollinger_Standard_Deviation - bollinger_Standard_Deviation) <= Double.Epsilon && i.Momentum_Period == momentum_Period && i.RSI_Period == rSI_Period && i.RSI_Smooth == rSI_Smooth && i.RSI_Level_Low == rSI_Level_Low && i.RSI_Level_High == rSI_Level_High && i.Momentum_Level_Low == momentum_Level_Low && i.Momentum_Level_High == momentum_Level_High);

			if (indicator != null)
				return indicator;

			indicator = new Mean_Reversion_Indicator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							IsLongEnabled = isLongEnabled,
							IsShortEnabled = isShortEnabled,
							Bollinger_Period = bollinger_Period,
							Bollinger_Standard_Deviation = bollinger_Standard_Deviation,
							Momentum_Period = momentum_Period,
							RSI_Period = rSI_Period,
							RSI_Smooth = rSI_Smooth,
							RSI_Level_Low = rSI_Level_Low,
							RSI_Level_High = rSI_Level_High,
							Momentum_Level_Low = momentum_Level_Low,
							Momentum_Level_High = momentum_Level_High
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
		/// The mean reversion is the theory suggesting that prices and returns eventually move back towards the mean or average.
		/// </summary>
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(System.Boolean isLongEnabled, System.Boolean isShortEnabled, System.Int32 bollinger_Period, System.Double bollinger_Standard_Deviation, System.Int32 momentum_Period, System.Int32 rSI_Period, System.Int32 rSI_Smooth, System.Int32 rSI_Level_Low, System.Int32 rSI_Level_High, System.Int32 momentum_Level_Low, System.Int32 momentum_Level_High)
		{
			return LeadIndicator.Mean_Reversion_Indicator(Input, isLongEnabled, isShortEnabled, bollinger_Period, bollinger_Standard_Deviation, momentum_Period, rSI_Period, rSI_Smooth, rSI_Level_Low, rSI_Level_High, momentum_Level_Low, momentum_Level_High);
		}

		/// <summary>
		/// The mean reversion is the theory suggesting that prices and returns eventually move back towards the mean or average.
		/// </summary>
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled, System.Int32 bollinger_Period, System.Double bollinger_Standard_Deviation, System.Int32 momentum_Period, System.Int32 rSI_Period, System.Int32 rSI_Smooth, System.Int32 rSI_Level_Low, System.Int32 rSI_Level_High, System.Int32 momentum_Level_Low, System.Int32 momentum_Level_High)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.Mean_Reversion_Indicator(input, isLongEnabled, isShortEnabled, bollinger_Period, bollinger_Standard_Deviation, momentum_Period, rSI_Period, rSI_Smooth, rSI_Level_Low, rSI_Level_High, momentum_Level_Low, momentum_Level_High);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// The mean reversion is the theory suggesting that prices and returns eventually move back towards the mean or average.
		/// </summary>
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(System.Boolean isLongEnabled, System.Boolean isShortEnabled, System.Int32 bollinger_Period, System.Double bollinger_Standard_Deviation, System.Int32 momentum_Period, System.Int32 rSI_Period, System.Int32 rSI_Smooth, System.Int32 rSI_Level_Low, System.Int32 rSI_Level_High, System.Int32 momentum_Level_Low, System.Int32 momentum_Level_High)
		{
			return LeadIndicator.Mean_Reversion_Indicator(Input, isLongEnabled, isShortEnabled, bollinger_Period, bollinger_Standard_Deviation, momentum_Period, rSI_Period, rSI_Smooth, rSI_Level_Low, rSI_Level_High, momentum_Level_Low, momentum_Level_High);
		}

		/// <summary>
		/// The mean reversion is the theory suggesting that prices and returns eventually move back towards the mean or average.
		/// </summary>
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled, System.Int32 bollinger_Period, System.Double bollinger_Standard_Deviation, System.Int32 momentum_Period, System.Int32 rSI_Period, System.Int32 rSI_Smooth, System.Int32 rSI_Level_Low, System.Int32 rSI_Level_High, System.Int32 momentum_Level_Low, System.Int32 momentum_Level_High)
		{
			return LeadIndicator.Mean_Reversion_Indicator(input, isLongEnabled, isShortEnabled, bollinger_Period, bollinger_Standard_Deviation, momentum_Period, rSI_Period, rSI_Smooth, rSI_Level_Low, rSI_Level_High, momentum_Level_Low, momentum_Level_High);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// The mean reversion is the theory suggesting that prices and returns eventually move back towards the mean or average.
		/// </summary>
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(System.Boolean isLongEnabled, System.Boolean isShortEnabled, System.Int32 bollinger_Period, System.Double bollinger_Standard_Deviation, System.Int32 momentum_Period, System.Int32 rSI_Period, System.Int32 rSI_Smooth, System.Int32 rSI_Level_Low, System.Int32 rSI_Level_High, System.Int32 momentum_Level_Low, System.Int32 momentum_Level_High)
		{
			return LeadIndicator.Mean_Reversion_Indicator(Input, isLongEnabled, isShortEnabled, bollinger_Period, bollinger_Standard_Deviation, momentum_Period, rSI_Period, rSI_Smooth, rSI_Level_Low, rSI_Level_High, momentum_Level_Low, momentum_Level_High);
		}

		/// <summary>
		/// The mean reversion is the theory suggesting that prices and returns eventually move back towards the mean or average.
		/// </summary>
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled, System.Int32 bollinger_Period, System.Double bollinger_Standard_Deviation, System.Int32 momentum_Period, System.Int32 rSI_Period, System.Int32 rSI_Smooth, System.Int32 rSI_Level_Low, System.Int32 rSI_Level_High, System.Int32 momentum_Level_Low, System.Int32 momentum_Level_High)
		{
			return LeadIndicator.Mean_Reversion_Indicator(input, isLongEnabled, isShortEnabled, bollinger_Period, bollinger_Standard_Deviation, momentum_Period, rSI_Period, rSI_Smooth, rSI_Level_Low, rSI_Level_High, momentum_Level_Low, momentum_Level_High);
		}
	}

	#endregion

}

#endregion

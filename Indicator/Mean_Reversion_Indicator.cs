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
         double Bollinger_Standard_Deviationn  { get; set; }
        int Momentum_Period  { get; set; }
        int RSI_Period  { get; set; }
        int RSI_Smooth { get; set; }
        int RSI_Level_Low  { get; set; }
        int RSI_Level_High { get; set; }
        int Momentum_Level_Low  { get; set; }
        int Momentum_Level_High { get; set; }

    }

	[Description("The mean reversion is the theory suggesting that prices and returns eventually move back towards the mean or average.")]
    public class Mean_Reversion_Indicator : UserIndicator, IMean_Reversion
	{

        //interface 
        private bool _IsShortEnabled = false;
        private bool _IsLongEnabled = true;
        private int _Bollinger_Period = 20;
        private double _Bollinger_Standard_Deviationn = 2;
        private int _Momentum_Period = 100;
        private int _RSI_Period = 13;
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


   

		protected override void OnBarUpdate()
		{

            ////we need more contrast
            //this.BarColor = Color.White;
    
            //calculate data
            OrderAction? resultdata = this.calculate(Input, null, null, this.Bollinger_Period, this.Bollinger_Standard_Deviationn, this.Momentum_Period, this.RSI_Period, this.RSI_Smooth, this.RSI_Level_Low, this.RSI_Level_High, this.Momentum_Level_Low, this.Momentum_Level_High);

            Plot_1.Set(bb_upper);
            Plot_2.Set(bb_middle);
            Plot_3.Set(bb_lower);

            //draw other things
            if (resultdata.HasValue)
            {
                switch (resultdata)
                {
                    case OrderAction.Buy:
                        DrawDot("ArrowLong_Entry" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.LightGreen);
                        break;
                    case OrderAction.SellShort:
                        DrawDiamond("ArrowShort_Entry" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.LightGreen);
                        break;
                    case OrderAction.BuyToCover:
                        //DrawDiamond("ArrowShort_Exit" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.Red);
                        break;
                    case OrderAction.Sell:
                        //DrawDot("ArrowLong_Exit" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.Red);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                //value was null
            }
		}

        /// <summary>
        /// In this function we do all the work and send back the OrderAction.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public OrderAction? calculate(IDataSeries data, IOrder longorder, IOrder shortorder, int bollinger_period, double bollinger_standarddeviation, int momentum_period, int rsi_period, int rsi_smooth, int rsi_level_low, int rsi_level_high, int momentum_level_low, int momentum_level_high)
        {
            
            //Calculate BB
            Bollinger bb = Bollinger(data, bollinger_standarddeviation, bollinger_period);
            Momentum mom = Momentum(data, momentum_period);
            RSI rsi = RSI(data, rsi_period, rsi_smooth);

            bb_lower = bb.Lower[0];
            bb_middle = bb.Middle[0];
            bb_upper = bb.Upper[0];



            // 
            if (mom[0] >= momentum_level_high && rsi[0] <= rsi_level_low
          && data[0] <= bb.Lower[0]
               && data[1] <= bb.Lower[1]
              && data[2] <= bb.Lower[2])
            {
                return OrderAction.Buy;
            }
            else if (mom[0] <= momentum_level_low && rsi[0] >= rsi_level_high
          && data[0] >= bb.Upper[0]
               && data[1] >= bb.Upper[1]
              && data[2] >= bb.Upper[2])
            {
                return OrderAction.SellShort;
            }
            else if (data[0] >= bb.Upper[0] && longorder != null)
            {
                //currently we left the building on the upper band, is it better if we switch to a stop?
                return OrderAction.Sell;
            }
            else if (data[0] <= bb.Lower[0] && shortorder != null)
            {
                return OrderAction.BuyToCover;
            }
      


      

       
            
            //Calculate the MA
             //_sma20 = SMA(data, 20);
             //_sma50 = SMA(data, 50);
             //_sma200 = SMA(data, 200);


            //double marketupordown =  MarketPhases(data, 0)[0];

            

            ////sma20 und sma200
            //if (ShouldIGoLong && CrossAbove(_sma20, _sma200, 0) && marketupordown >= 0)
            // {
            //     return OrderAction.Buy;
            // }
            // else if (ShouldIGoShort && CrossBelow(_sma20, _sma200, 0) )
            // {
            //     return OrderAction.SellShort;
            // }
            // else if (ShouldIGoShort && CrossAbove(_sma20, _sma200, 0))
            // {
            //     return OrderAction.BuyToCover;
            // }
            //else if (ShouldIGoLong && CrossBelow(_sma20, _sma200, 0))
            // {
            //     return OrderAction.Sell;
            // }



            return null;
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
        [Description("If true it is allowed to go long")]
        [Category("Parameters")]
        [DisplayName("Allow Long")]
        public bool IsLongEnabled
        {
            get { return _IsLongEnabled; }
            set { _IsLongEnabled = value; }
        }


        /// <summary>
        /// </summary>
        [Description("If true it is allowed to go short")]
        [Category("Parameters")]
        [DisplayName("Allow Short")]
        public bool IsShortEnabled
        {
            get { return _IsShortEnabled; }
            set { _IsShortEnabled = value; }
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
        public double Bollinger_Standard_Deviationn
        {
            get { return _Bollinger_Standard_Deviationn; }
            set { _Bollinger_Standard_Deviationn = value; }
        }

        [Description("Period of the Momentum.")]
        [Category("Parameters")]
        [DisplayName("Momentum Period")]
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
        [DisplayName("RSI Level Low")]
        public int Momentum_Level_Low
        {
            get { return _Momentum_Level_Low; }
            set { _Momentum_Level_Low = value; }
        }

        [Description("We trade short if momentum is below this level.")]
        [Category("Parameters")]
        [DisplayName("RSI Level High")]
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
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(System.Boolean isLongEnabled, System.Boolean isShortEnabled, System.Int32 bollinger_Period, System.Double bollinger_Standard_Deviationn, System.Int32 momentum_Period, System.Int32 rSI_Period, System.Int32 rSI_Smooth, System.Int32 rSI_Level_Low, System.Int32 rSI_Level_High, System.Int32 momentum_Level_Low, System.Int32 momentum_Level_High)
        {
			return Mean_Reversion_Indicator(Input, isLongEnabled, isShortEnabled, bollinger_Period, bollinger_Standard_Deviationn, momentum_Period, rSI_Period, rSI_Smooth, rSI_Level_Low, rSI_Level_High, momentum_Level_Low, momentum_Level_High);
		}

		/// <summary>
		/// The mean reversion is the theory suggesting that prices and returns eventually move back towards the mean or average.
		/// </summary>
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled, System.Int32 bollinger_Period, System.Double bollinger_Standard_Deviationn, System.Int32 momentum_Period, System.Int32 rSI_Period, System.Int32 rSI_Smooth, System.Int32 rSI_Level_Low, System.Int32 rSI_Level_High, System.Int32 momentum_Level_Low, System.Int32 momentum_Level_High)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Mean_Reversion_Indicator>(input, i => i.IsLongEnabled == isLongEnabled && i.IsShortEnabled == isShortEnabled && i.Bollinger_Period == bollinger_Period && Math.Abs(i.Bollinger_Standard_Deviationn - bollinger_Standard_Deviationn) <= Double.Epsilon && i.Momentum_Period == momentum_Period && i.RSI_Period == rSI_Period && i.RSI_Smooth == rSI_Smooth && i.RSI_Level_Low == rSI_Level_Low && i.RSI_Level_High == rSI_Level_High && i.Momentum_Level_Low == momentum_Level_Low && i.Momentum_Level_High == momentum_Level_High);

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
							Bollinger_Standard_Deviationn = bollinger_Standard_Deviationn,
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
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(System.Boolean isLongEnabled, System.Boolean isShortEnabled, System.Int32 bollinger_Period, System.Double bollinger_Standard_Deviationn, System.Int32 momentum_Period, System.Int32 rSI_Period, System.Int32 rSI_Smooth, System.Int32 rSI_Level_Low, System.Int32 rSI_Level_High, System.Int32 momentum_Level_Low, System.Int32 momentum_Level_High)
		{
			return LeadIndicator.Mean_Reversion_Indicator(Input, isLongEnabled, isShortEnabled, bollinger_Period, bollinger_Standard_Deviationn, momentum_Period, rSI_Period, rSI_Smooth, rSI_Level_Low, rSI_Level_High, momentum_Level_Low, momentum_Level_High);
		}

		/// <summary>
		/// The mean reversion is the theory suggesting that prices and returns eventually move back towards the mean or average.
		/// </summary>
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled, System.Int32 bollinger_Period, System.Double bollinger_Standard_Deviationn, System.Int32 momentum_Period, System.Int32 rSI_Period, System.Int32 rSI_Smooth, System.Int32 rSI_Level_Low, System.Int32 rSI_Level_High, System.Int32 momentum_Level_Low, System.Int32 momentum_Level_High)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.Mean_Reversion_Indicator(input, isLongEnabled, isShortEnabled, bollinger_Period, bollinger_Standard_Deviationn, momentum_Period, rSI_Period, rSI_Smooth, rSI_Level_Low, rSI_Level_High, momentum_Level_Low, momentum_Level_High);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// The mean reversion is the theory suggesting that prices and returns eventually move back towards the mean or average.
		/// </summary>
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(System.Boolean isLongEnabled, System.Boolean isShortEnabled, System.Int32 bollinger_Period, System.Double bollinger_Standard_Deviationn, System.Int32 momentum_Period, System.Int32 rSI_Period, System.Int32 rSI_Smooth, System.Int32 rSI_Level_Low, System.Int32 rSI_Level_High, System.Int32 momentum_Level_Low, System.Int32 momentum_Level_High)
		{
			return LeadIndicator.Mean_Reversion_Indicator(Input, isLongEnabled, isShortEnabled, bollinger_Period, bollinger_Standard_Deviationn, momentum_Period, rSI_Period, rSI_Smooth, rSI_Level_Low, rSI_Level_High, momentum_Level_Low, momentum_Level_High);
		}

		/// <summary>
		/// The mean reversion is the theory suggesting that prices and returns eventually move back towards the mean or average.
		/// </summary>
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled, System.Int32 bollinger_Period, System.Double bollinger_Standard_Deviationn, System.Int32 momentum_Period, System.Int32 rSI_Period, System.Int32 rSI_Smooth, System.Int32 rSI_Level_Low, System.Int32 rSI_Level_High, System.Int32 momentum_Level_Low, System.Int32 momentum_Level_High)
		{
			return LeadIndicator.Mean_Reversion_Indicator(input, isLongEnabled, isShortEnabled, bollinger_Period, bollinger_Standard_Deviationn, momentum_Period, rSI_Period, rSI_Smooth, rSI_Level_Low, rSI_Level_High, momentum_Level_Low, momentum_Level_High);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// The mean reversion is the theory suggesting that prices and returns eventually move back towards the mean or average.
		/// </summary>
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(System.Boolean isLongEnabled, System.Boolean isShortEnabled, System.Int32 bollinger_Period, System.Double bollinger_Standard_Deviationn, System.Int32 momentum_Period, System.Int32 rSI_Period, System.Int32 rSI_Smooth, System.Int32 rSI_Level_Low, System.Int32 rSI_Level_High, System.Int32 momentum_Level_Low, System.Int32 momentum_Level_High)
		{
			return LeadIndicator.Mean_Reversion_Indicator(Input, isLongEnabled, isShortEnabled, bollinger_Period, bollinger_Standard_Deviationn, momentum_Period, rSI_Period, rSI_Smooth, rSI_Level_Low, rSI_Level_High, momentum_Level_Low, momentum_Level_High);
		}

		/// <summary>
		/// The mean reversion is the theory suggesting that prices and returns eventually move back towards the mean or average.
		/// </summary>
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled, System.Int32 bollinger_Period, System.Double bollinger_Standard_Deviationn, System.Int32 momentum_Period, System.Int32 rSI_Period, System.Int32 rSI_Smooth, System.Int32 rSI_Level_Low, System.Int32 rSI_Level_High, System.Int32 momentum_Level_Low, System.Int32 momentum_Level_High)
		{
			return LeadIndicator.Mean_Reversion_Indicator(input, isLongEnabled, isShortEnabled, bollinger_Period, bollinger_Standard_Deviationn, momentum_Period, rSI_Period, rSI_Smooth, rSI_Level_Low, rSI_Level_High, momentum_Level_Low, momentum_Level_High);
		}
	}

	#endregion

}

#endregion

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
	[Description("The mean reversion is the theory suggesting that prices and returns eventually move back towards the mean or average.")]
	public class Mean_Reversion_Indicator : UserIndicator
	{

        //input 
        bool _ShouldIGoShort = false;
        bool _ShouldIGoLong = true;

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
            OrderAction? resultdata = this.calculate(Input, null, null, 20, 2, 130, 14, 3, 30, 70, -1, 1);

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

        #region Input

        

              /// <summary>
        /// </summary>
        [Description("If true it is allowed to go long")]
        [Category("Parameters")]
        [DisplayName("Allow Long")]
        public bool ShouldIGoLong
        {
            get { return _ShouldIGoLong; }
            set { _ShouldIGoLong = value; }
        }


          /// <summary>
        /// </summary>
        [Description("If true it is allowed to go short")]
        [Category("Parameters")]
        [DisplayName("Allow Short")]
        public bool ShouldIGoShort
        {
            get { return _ShouldIGoShort; }
            set { _ShouldIGoShort = value; }
        }

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
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(System.Boolean shouldIGoLong, System.Boolean shouldIGoShort)
        {
			return Mean_Reversion_Indicator(Input, shouldIGoLong, shouldIGoShort);
		}

		/// <summary>
		/// The mean reversion is the theory suggesting that prices and returns eventually move back towards the mean or average.
		/// </summary>
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(IDataSeries input, System.Boolean shouldIGoLong, System.Boolean shouldIGoShort)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Mean_Reversion_Indicator>(input, i => i.ShouldIGoLong == shouldIGoLong && i.ShouldIGoShort == shouldIGoShort);

			if (indicator != null)
				return indicator;

			indicator = new Mean_Reversion_Indicator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							ShouldIGoLong = shouldIGoLong,
							ShouldIGoShort = shouldIGoShort
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
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(System.Boolean shouldIGoLong, System.Boolean shouldIGoShort)
		{
			return LeadIndicator.Mean_Reversion_Indicator(Input, shouldIGoLong, shouldIGoShort);
		}

		/// <summary>
		/// The mean reversion is the theory suggesting that prices and returns eventually move back towards the mean or average.
		/// </summary>
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(IDataSeries input, System.Boolean shouldIGoLong, System.Boolean shouldIGoShort)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.Mean_Reversion_Indicator(input, shouldIGoLong, shouldIGoShort);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// The mean reversion is the theory suggesting that prices and returns eventually move back towards the mean or average.
		/// </summary>
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(System.Boolean shouldIGoLong, System.Boolean shouldIGoShort)
		{
			return LeadIndicator.Mean_Reversion_Indicator(Input, shouldIGoLong, shouldIGoShort);
		}

		/// <summary>
		/// The mean reversion is the theory suggesting that prices and returns eventually move back towards the mean or average.
		/// </summary>
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(IDataSeries input, System.Boolean shouldIGoLong, System.Boolean shouldIGoShort)
		{
			return LeadIndicator.Mean_Reversion_Indicator(input, shouldIGoLong, shouldIGoShort);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// The mean reversion is the theory suggesting that prices and returns eventually move back towards the mean or average.
		/// </summary>
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(System.Boolean shouldIGoLong, System.Boolean shouldIGoShort)
		{
			return LeadIndicator.Mean_Reversion_Indicator(Input, shouldIGoLong, shouldIGoShort);
		}

		/// <summary>
		/// The mean reversion is the theory suggesting that prices and returns eventually move back towards the mean or average.
		/// </summary>
		public Mean_Reversion_Indicator Mean_Reversion_Indicator(IDataSeries input, System.Boolean shouldIGoLong, System.Boolean shouldIGoShort)
		{
			return LeadIndicator.Mean_Reversion_Indicator(input, shouldIGoLong, shouldIGoShort);
		}
	}

	#endregion

}

#endregion

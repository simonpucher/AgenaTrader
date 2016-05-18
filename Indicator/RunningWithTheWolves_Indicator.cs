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
/// todo description: http://systemtradersuccess.com/golden-cross-which-is-the-best/
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this indicator without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("Enter the description for the new custom indicator here")]
	public class RunningWithTheWolves_Indicator : UserIndicator
	{

        //input 
        bool _ShouldIGoShort = false;
        bool _ShouldIGoLong = true;

        //internal
        SMA _sma20 = null;
        SMA _sma50 = null;
        SMA _sma200 = null;
        RSI _rsi143 = null;

        EMA _slow = null;
        EMA _fast = null;

		protected override void Initialize()
		{
            //Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));

            Add(new Plot(new Pen(Color.FromKnownColor(KnownColor.Red), 2), PlotStyle.Line, "SMA20")); 
            Add(new Plot(new Pen(Color.FromKnownColor(KnownColor.Orange), 2), PlotStyle.Line, "SMA50")); 
            Add(new Plot(new Pen(Color.FromKnownColor(KnownColor.Blue), 2), PlotStyle.Line, "SMA200"));

			CalculateOnBarClose = true;
            Overlay = true;
        }


   

		protected override void OnBarUpdate()
		{
            //we need more contrast
            this.BarColor = Color.White;
    
            //calculate data
            OrderAction? resultdata = this.calculate(Input);

            //draw indicator lines
            Plot_1.Set(this._sma20[0]);
            Plot_2.Set(this._sma50[0]);
            Plot_3.Set(this._sma200[0]);

            //Plot_1.Set(this._slow[0]);
            //Plot_2.Set(this._fast[0]);


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
                        DrawDiamond("ArrowShort_Exit" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.Red);
                        break;
                    case OrderAction.Sell:
                        DrawDot("ArrowLong_Exit" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.Red);
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
        public OrderAction? calculate(IDataSeries data)
        {
            //Calculate the MA
             _sma20 = SMA(data, 20);
             _sma50 = SMA(data, 50);
             _sma200 = SMA(data, 200);


            double marketupordown =  MarketPhases(data, 0)[0];

            // _rsi143 = RSI(data, 14, 3);

             //_slow = EMA(data, 48);
             //_fast = EMA(data, 13);

             //if (CrossAbove(_fast, _slow, 1))
             //{
             //    return OrderAction.Buy;
             //}
             //else if (CrossBelow(_fast, _slow, 1))
             //{
             //    return OrderAction.Sell;
             //}



            // 20 und 200 mit zwei tage sma filter
             //if (CrossAbove(_sma20, _sma200, 1) && _sma200[0] > _sma200[1] && _sma200[1] > _sma200[2])
             //{
             //    return OrderAction.Buy;
             //}
             //else if (CrossBelow(_sma20, _sma200, 1) && _sma200[0] < _sma200[1] && _sma200[1] < _sma200[2])
             //{
             //    return OrderAction.SellShort;
             //}
             //else if (CrossAbove(_sma20, _sma50, 1))
             //{
             //    return OrderAction.BuyToCover;
             //}
             //else if (CrossBelow(_sma20, _sma50, 1))
             //{
             //    return OrderAction.Sell;

             //}

            //Print(marketupordown);

            //sma20 und sma200
            if (ShouldIGoLong && CrossAbove(_sma20, _sma200, 0) && marketupordown >= 0)
             {
                 return OrderAction.Buy;
             }
             else if (ShouldIGoShort && CrossBelow(_sma20, _sma200, 0) )
             {
                 return OrderAction.SellShort;
             }
             else if (ShouldIGoShort && CrossAbove(_sma20, _sma200, 0))
             {
                 return OrderAction.BuyToCover;
             }
            else if (ShouldIGoLong && CrossBelow(_sma20, _sma200, 0))
             {
                 return OrderAction.Sell;
             }



            return null;
        }

        public override string ToString()
        {
            return "Running with the wolves (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "Running with the wolves (I)";
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
		/// Enter the description for the new custom indicator here
		/// </summary>
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator(System.Boolean shouldIGoLong, System.Boolean shouldIGoShort)
        {
			return RunningWithTheWolves_Indicator(Input, shouldIGoLong, shouldIGoShort);
		}

		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator(IDataSeries input, System.Boolean shouldIGoLong, System.Boolean shouldIGoShort)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<RunningWithTheWolves_Indicator>(input, i => i.ShouldIGoLong == shouldIGoLong && i.ShouldIGoShort == shouldIGoShort);

			if (indicator != null)
				return indicator;

			indicator = new RunningWithTheWolves_Indicator
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
		/// Enter the description for the new custom indicator here
		/// </summary>
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator(System.Boolean shouldIGoLong, System.Boolean shouldIGoShort)
		{
			return LeadIndicator.RunningWithTheWolves_Indicator(Input, shouldIGoLong, shouldIGoShort);
		}

		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator(IDataSeries input, System.Boolean shouldIGoLong, System.Boolean shouldIGoShort)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.RunningWithTheWolves_Indicator(input, shouldIGoLong, shouldIGoShort);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator(System.Boolean shouldIGoLong, System.Boolean shouldIGoShort)
		{
			return LeadIndicator.RunningWithTheWolves_Indicator(Input, shouldIGoLong, shouldIGoShort);
		}

		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator(IDataSeries input, System.Boolean shouldIGoLong, System.Boolean shouldIGoShort)
		{
			return LeadIndicator.RunningWithTheWolves_Indicator(input, shouldIGoLong, shouldIGoShort);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator(System.Boolean shouldIGoLong, System.Boolean shouldIGoShort)
		{
			return LeadIndicator.RunningWithTheWolves_Indicator(Input, shouldIGoLong, shouldIGoShort);
		}

		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator(IDataSeries input, System.Boolean shouldIGoLong, System.Boolean shouldIGoShort)
		{
			return LeadIndicator.RunningWithTheWolves_Indicator(input, shouldIGoLong, shouldIGoShort);
		}
	}

	#endregion

}

#endregion

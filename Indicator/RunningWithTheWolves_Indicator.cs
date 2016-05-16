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
/// 
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

        //internal
        SMA _sma20 = null;
        SMA _sma50 = null;
        SMA _sma200 = null;

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

            this.BarColor = Color.White;

            //calculate data
            OrderAction? resultdata = this.calculate(Input);

            //draw lines
            Plot_1.Set(this._sma20[0]);
            Plot_2.Set(this._sma50[0]);
            Plot_3.Set(this._sma200[0]);

            //draw other things
            if (resultdata.HasValue)
            {
                switch (resultdata)
                {
                    case OrderAction.Buy:
                        //Value.Set(1);
                        DrawArrowUp("ArrowLong_Entry" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Low, Color.Green);
                        break;
                    case OrderAction.SellShort:
                        //Value.Set(-1);
                        DrawArrowDown("ArrowShort_Entry" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].High, Color.Green);
                        break;
                    case OrderAction.BuyToCover:
                        //Value.Set(0.5);
                        DrawArrowUp("ArrowShort_Exit" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Low, Color.Red);
                        break;
                    case OrderAction.Sell:
                        //Value.Set(-0.5);
                        DrawArrowDown("ArrowLong_Exit" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].High, Color.Red);
                        break;
                    default:
                        //Value.Set(0);
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

             _sma20 = SMA(data, 20);
             _sma50 = SMA(data, 50);
             _sma200 = SMA(data, 200);

            if (CrossAbove(_sma20, _sma200, 1) && _sma200[0] > _sma200[1] && _sma200[1] > _sma200[2])
            {
                return OrderAction.Buy;
            }
            else if (CrossBelow(_sma20, _sma200, 1) && _sma200[0] < _sma200[1] && _sma200[1] < _sma200[2])
            {
                return OrderAction.SellShort;
            }
            else if (CrossAbove(_sma20, _sma50, 1))
            {
                return OrderAction.BuyToCover;
            }
            else if (CrossBelow(_sma20, _sma50, 1))
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
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator()
        {
			return RunningWithTheWolves_Indicator(Input);
		}

		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<RunningWithTheWolves_Indicator>(input);

			if (indicator != null)
				return indicator;

			indicator = new RunningWithTheWolves_Indicator
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
		/// Enter the description for the new custom indicator here
		/// </summary>
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator()
		{
			return LeadIndicator.RunningWithTheWolves_Indicator(Input);
		}

		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator(IDataSeries input)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.RunningWithTheWolves_Indicator(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator()
		{
			return LeadIndicator.RunningWithTheWolves_Indicator(Input);
		}

		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator(IDataSeries input)
		{
			return LeadIndicator.RunningWithTheWolves_Indicator(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator()
		{
			return LeadIndicator.RunningWithTheWolves_Indicator(Input);
		}

		/// <summary>
		/// Enter the description for the new custom indicator here
		/// </summary>
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator(IDataSeries input)
		{
			return LeadIndicator.RunningWithTheWolves_Indicator(input);
		}
	}

	#endregion

}

#endregion

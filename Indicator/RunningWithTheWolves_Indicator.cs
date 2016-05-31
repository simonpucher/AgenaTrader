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
/// To compile this script without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    /// <summary>
    /// Selectable MAs for this indicator.
    /// </summary>
    public enum Enum_RunningWithTheWolves_Indicator_MA
    {
        SMA = 0,
        EMA = 1
    }

    [Description("RunningWithTheWolves")]
	public class RunningWithTheWolves_Indicator : UserIndicator
	{

        //input 
        private Enum_RunningWithTheWolves_Indicator_MA _MA_Selected = Enum_RunningWithTheWolves_Indicator_MA.SMA;

        private int _ma_slow = 200;
        private int _ma_medium = 100;
        private int _ma_fast = 20;

        private bool _IsShortEnabled = true;
        private bool _IsLongEnabled = true;

        //internal
        Indicator _maslow = null;
        Indicator _mamedium = null;
        Indicator _mafast = null;


		protected override void Initialize()
		{
            //Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));

            Add(new Plot(new Pen(Color.FromKnownColor(KnownColor.Red), 2), PlotStyle.Line, "MA_Fast")); 
            Add(new Plot(new Pen(Color.FromKnownColor(KnownColor.Orange), 2), PlotStyle.Line, "MA_Medium")); 
            Add(new Plot(new Pen(Color.FromKnownColor(KnownColor.Blue), 2), PlotStyle.Line, "MA_Slow"));

			CalculateOnBarClose = true;
            Overlay = true;
        }


   

		protected override void OnBarUpdate()
		{
            //we need more contrast
            this.BarColor = Color.White;
    
            //calculate data
            OrderAction? resultdata = this.calculate(Input, this.MA_Selected, this.MA_Fast, this.MA_Medium, this.MA_Slow);

            //draw indicator lines
            Plot_1.Set(this._mafast[0]);
            Plot_2.Set(this._mamedium[0]);
            Plot_3.Set(this._maslow[0]);

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
        public OrderAction? calculate(IDataSeries data, Enum_RunningWithTheWolves_Indicator_MA ma, int fast, int medium, int slow)
        {


            //set the ma and period
            switch (ma)
            {
                case Enum_RunningWithTheWolves_Indicator_MA.SMA:
                    _mafast = SMA(data, fast);
                    _mamedium = SMA(data, medium);
                    _maslow = SMA(data, slow);
                    break;
                case Enum_RunningWithTheWolves_Indicator_MA.EMA:
                    _mafast = EMA(data, fast);
                    _mamedium = EMA(data, medium);
                    _maslow = EMA(data, slow);
                    break;
                default:
                    break;
            }

            //double marketupordown =  MarketPhases(data, 0)[0];

            //2  && Rising(_mafast) && Falling(_mafast)
            if (IsLongEnabled && CrossAbove(_mafast, _maslow, 0) )
            {
                return OrderAction.Buy;
            }
            else if (IsShortEnabled && CrossBelow(_mafast, _maslow, 0) )
            {
                return OrderAction.SellShort;
            }
            else if (IsShortEnabled && CrossAbove(_mafast, _mamedium, 0))
            {
                return OrderAction.BuyToCover;
            }
            else if (IsLongEnabled && CrossBelow(_mafast, _mamedium, 0))
            {
                return OrderAction.Sell;
            }

            ////1
            //if (IsLongEnabled && CrossAbove(_maslow, _mafast, 0) && marketupordown >= 0)
            // {
            //     return OrderAction.Buy;
            // }
            //else if (IsShortEnabled && CrossBelow(_maslow, _mafast, 0) && marketupordown <= 0)
            // {
            //     return OrderAction.SellShort;
            // }
            //else if (IsShortEnabled && CrossAbove(_maslow, _mamedium, 0))
            // {
            //     return OrderAction.BuyToCover;
            // }
            //else if (IsLongEnabled && CrossBelow(_maslow, _mamedium, 0))
            // {
            //     return OrderAction.Sell;
            // }



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
        [Description("Select the type of MA you would like to use")]
        [Category("Parameters")]
        [DisplayName("Type of MA")]
        public Enum_RunningWithTheWolves_Indicator_MA MA_Selected
        {
            get { return _MA_Selected; }
            set
            {
                _MA_Selected = value;
            }
        }


        /// <summary>
        /// </summary>
        [Description("Period for the slow mean average")]
        [Category("Parameters")]
        [DisplayName("MA Slow")]
        public int MA_Slow
        {
            get { return _ma_slow; }
            set
            {
                _ma_slow = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("Period for the medium mean average")]
        [Category("Parameters")]
        [DisplayName("MA Medium")]
        public int MA_Medium
        {
            get { return _ma_medium; }
            set
            {
                _ma_medium = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("Period for the fast mean average")]
        [Category("Parameters")]
        [DisplayName("MA Fast")]
        public int MA_Fast
        {
            get { return _ma_fast; }
            set
            {
                _ma_fast = value;
            }
        }
        

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
		/// RunningWithTheWolves
		/// </summary>
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator(Enum_RunningWithTheWolves_Indicator_MA mA_Selected, System.Int32 mA_Slow, System.Int32 mA_Medium, System.Int32 mA_Fast, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
        {
			return RunningWithTheWolves_Indicator(Input, mA_Selected, mA_Slow, mA_Medium, mA_Fast, isLongEnabled, isShortEnabled);
		}

		/// <summary>
		/// RunningWithTheWolves
		/// </summary>
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator(IDataSeries input, Enum_RunningWithTheWolves_Indicator_MA mA_Selected, System.Int32 mA_Slow, System.Int32 mA_Medium, System.Int32 mA_Fast, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<RunningWithTheWolves_Indicator>(input, i => i.MA_Selected == mA_Selected && i.MA_Slow == mA_Slow && i.MA_Medium == mA_Medium && i.MA_Fast == mA_Fast && i.IsLongEnabled == isLongEnabled && i.IsShortEnabled == isShortEnabled);

			if (indicator != null)
				return indicator;

			indicator = new RunningWithTheWolves_Indicator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							MA_Selected = mA_Selected,
							MA_Slow = mA_Slow,
							MA_Medium = mA_Medium,
							MA_Fast = mA_Fast,
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
		/// RunningWithTheWolves
		/// </summary>
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator(Enum_RunningWithTheWolves_Indicator_MA mA_Selected, System.Int32 mA_Slow, System.Int32 mA_Medium, System.Int32 mA_Fast, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.RunningWithTheWolves_Indicator(Input, mA_Selected, mA_Slow, mA_Medium, mA_Fast, isLongEnabled, isShortEnabled);
		}

		/// <summary>
		/// RunningWithTheWolves
		/// </summary>
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator(IDataSeries input, Enum_RunningWithTheWolves_Indicator_MA mA_Selected, System.Int32 mA_Slow, System.Int32 mA_Medium, System.Int32 mA_Fast, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.RunningWithTheWolves_Indicator(input, mA_Selected, mA_Slow, mA_Medium, mA_Fast, isLongEnabled, isShortEnabled);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// RunningWithTheWolves
		/// </summary>
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator(Enum_RunningWithTheWolves_Indicator_MA mA_Selected, System.Int32 mA_Slow, System.Int32 mA_Medium, System.Int32 mA_Fast, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.RunningWithTheWolves_Indicator(Input, mA_Selected, mA_Slow, mA_Medium, mA_Fast, isLongEnabled, isShortEnabled);
		}

		/// <summary>
		/// RunningWithTheWolves
		/// </summary>
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator(IDataSeries input, Enum_RunningWithTheWolves_Indicator_MA mA_Selected, System.Int32 mA_Slow, System.Int32 mA_Medium, System.Int32 mA_Fast, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.RunningWithTheWolves_Indicator(input, mA_Selected, mA_Slow, mA_Medium, mA_Fast, isLongEnabled, isShortEnabled);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// RunningWithTheWolves
		/// </summary>
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator(Enum_RunningWithTheWolves_Indicator_MA mA_Selected, System.Int32 mA_Slow, System.Int32 mA_Medium, System.Int32 mA_Fast, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.RunningWithTheWolves_Indicator(Input, mA_Selected, mA_Slow, mA_Medium, mA_Fast, isLongEnabled, isShortEnabled);
		}

		/// <summary>
		/// RunningWithTheWolves
		/// </summary>
		public RunningWithTheWolves_Indicator RunningWithTheWolves_Indicator(IDataSeries input, Enum_RunningWithTheWolves_Indicator_MA mA_Selected, System.Int32 mA_Slow, System.Int32 mA_Medium, System.Int32 mA_Fast, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.RunningWithTheWolves_Indicator(input, mA_Selected, mA_Slow, mA_Medium, mA_Fast, isLongEnabled, isShortEnabled);
		}
	}

	#endregion

}

#endregion

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
/// Golden & Death cross: http://www.investopedia.com/ask/answers/121114/what-difference-between-golden-cross-and-death-cross-pattern.asp
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
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

    [Description("Use SMA or EMA crosses to find trends.")]
	public class RunningWithTheWolves_Indicator : UserIndicator
	{

        //input 
        private Enum_RunningWithTheWolves_Indicator_MA _MA_Selected = Enum_RunningWithTheWolves_Indicator_MA.SMA;

        private int _ma_slow = 200;
        private int _ma_medium = 100;
        private int _ma_fast = 20;

        private bool _IsShortEnabled = true;
        private bool _IsLongEnabled = true;
        private bool _UseWhiteCandles = true;

        //output
        private DataSeries _Delta_Price_to_xMA_Slow = null;
        private DataSeries _Delta_xMA_Fast_to_xMA_Slow = null;

        //internal
        Indicator _maslow = null;
        Indicator _mamedium = null;
        Indicator _mafast = null;


		protected override void OnInit()
		{
            //Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));

            Add(new OutputDescriptor(new Pen(Color.FromKnownColor(KnownColor.Red), 2), OutputSerieDrawStyle.Line, "MA_Fast")); 
            Add(new OutputDescriptor(new Pen(Color.FromKnownColor(KnownColor.Orange), 2), OutputSerieDrawStyle.Line, "MA_Medium")); 
            Add(new OutputDescriptor(new Pen(Color.FromKnownColor(KnownColor.Blue), 2), OutputSerieDrawStyle.Line, "MA_Slow"));

			CalculateOnClosedBar = true;
            IsOverlay = true;

            //init
            this._Delta_Price_to_xMA_Slow = new DataSeries(this);
            this._Delta_xMA_Fast_to_xMA_Slow = new DataSeries(this);
        }


   

		protected override void OnCalculate()
		{
            //we need more contrast
            if (this.UseWhiteCandles)
            {
                 this.BarColor = Color.White;
            }

            //calculate data
            OrderDirection_Enum? resultdata = this.calculate(InSeries, this.MA_Selected, this.MA_Fast, this.MA_Medium, this.MA_Slow);

            //draw indicator lines
            Plot_1.Set(this._mafast[0]);
            Plot_2.Set(this._mamedium[0]);
            Plot_3.Set(this._maslow[0]);

            //todo set the additional indicator values
           

            //draw other things
            if (resultdata.HasValue)
            {
                switch (resultdata)
                {
                    case OrderDirection_Enum.OpenLong:
                        AddChartDot("ArrowLong_Entry" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.LightGreen);
                        break;
                    case OrderDirection_Enum.OpenShort:
                        AddChartDiamond("ArrowShort_Entry" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.LightGreen);
                        break;
                    case OrderDirection_Enum.CloseShort:
                        AddChartDiamond("ArrowShort_Exit" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.Red);
                        break;
                    case OrderDirection_Enum.CloseLong:
                        AddChartDot("ArrowLong_Exit" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.Red);
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
        /// In this function we do all the work and send back the OrderDirection.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public OrderDirection_Enum? calculate(IDataSeries data, Enum_RunningWithTheWolves_Indicator_MA ma, int fast, int medium, int slow)
        {

            //Calculate the SMA or EMA
            ////DOW
            //_maslow = EMA(data, 20);
            // _mamedium = EMA(data, 100);
            // _mafast = EMA(data, 200);

            ////Dax
            //_mafast = SMA(data, 20);
            //_mamedium = SMA(data, 80);
            //_maslow = SMA(data, 200);


            //Bollinger mybollinger = Bollinger(data, 2, 20);
            //double bb_medium = mybollinger[0];
            //double bb_low = mybollinger.Lower[0];
            //double bb_high = mybollinger.Upper[0];

            //double rsi_value = RSI(data, 14, 3)[0];

          

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

            //Calculate Delta from stock price to slow xMA
            if (this._Delta_Price_to_xMA_Slow != null)
            {
                double percent = (data[0] / (_maslow[0] / 100)) - 100;
                this._Delta_Price_to_xMA_Slow.Set(percent);
            }

            //Calculate Delta from xMA fast to xMA slow.
            if (_Delta_xMA_Fast_to_xMA_Slow != null)
            {
                double percent = (_mafast[0] / (_maslow[0] / 100)) - 100;
                this._Delta_xMA_Fast_to_xMA_Slow.Set(percent); 
            }


            //2  && IsSeriesRising(_mafast) && IsSeriesFalling(_mafast)
            if (IsLongEnabled && CrossAbove(_mafast, _maslow, 0) )
            {
                return OrderDirection_Enum.OpenLong;
            }
            else if (IsShortEnabled && CrossBelow(_mafast, _maslow, 0) )
            {
                return OrderDirection_Enum.OpenShort;
            }
            //else if (IsShortEnabled && data[0] < bb_low && rsi_value < 70)
            else if (IsShortEnabled && CrossAbove(_mafast, _mamedium, 0))
            {
                return OrderDirection_Enum.CloseShort;
            }
            //else if (IsLongEnabled && data[0] > bb_high && rsi_value > 70)
            else if (IsLongEnabled && CrossBelow(_mafast, _mamedium, 0))
            {
                return OrderDirection_Enum.CloseLong;
            }

            ////1
            //double marketupordown =  MarketPhases(data, 0)[0];
            //if (IsLongEnabled && CrossAbove(_maslow, _mafast, 0) && marketupordown >= 0)
            // {
            //     return OrderDirection.Buy;
            // }
            //else if (IsShortEnabled && CrossBelow(_maslow, _mafast, 0) && marketupordown <= 0)
            // {
            //     return OrderDirection.Sell;
            // }
            //else if (IsShortEnabled && CrossAbove(_maslow, _mamedium, 0))
            // {
            //     return OrderDirection.Buy;
            // }
            //else if (IsLongEnabled && CrossBelow(_maslow, _mamedium, 0))
            // {
            //     return OrderDirection.Sell;
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

        #region InSeries

        
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

        /// <summary>
        /// </summary>
        [Description("If true candles will be display in white/gray to provide a better contrast")]
        [Category("Parameters")]
        [DisplayName("White candle colors")]
        public bool UseWhiteCandles
        {
            get { return _UseWhiteCandles; }
            set { _UseWhiteCandles = value; }
        }

        #endregion

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Plot_1 { get { return Outputs[0]; } }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Plot_2 { get { return Outputs[1]; } }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Plot_3 { get { return Outputs[2]; } }

        /// <summary>
        /// This is the delta value from price to slow xMA.
        /// Will be calculated in percent.
        /// </summary>
        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Delta_Price_to_xMA_Slow {
            get { return _Delta_Price_to_xMA_Slow; }
        }

        /// <summary>
        /// This is the delta value from xMA fast to xMA slow.
        /// Will be calculated in percent.
        /// </summary>
        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Delta_xMA_Fast_to_xMA_Slow
        {
            get { return _Delta_xMA_Fast_to_xMA_Slow; }
        }

		#endregion
	}
}
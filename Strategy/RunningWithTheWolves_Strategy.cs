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
using System.Linq.Expressions;


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
    [Description("Use SMA or EMA crosses to find trends.")]
    //[TimeFrameRequirements("1 Hour", "1 Day")]
	public class RunningWithTheWolves_Strategy : UserStrategy
	{
        
        //input
        private Enum_RunningWithTheWolves_Indicator_MA _MA_Selected = Enum_RunningWithTheWolves_Indicator_MA.SMA;

        private int _ma_slow = 200;
        private int _ma_medium = 100;
        private int _ma_fast = 20;


        private bool _IsShortEnabled = true;
        private bool _IsLongEnabled = true;

        private bool _send_email = false;
        private bool _autopilot = true;
        private bool _statisticbacktesting = false;

        //output

        //internal
        private IOrder _orderenterlong;
        private IOrder _orderentershort;
        private RunningWithTheWolves_Indicator _RunningWithTheWolves_Indicator = null;
        //private StatisticContainer _StatisticContainer = null;
        private CsvExport _CsvExport = new CsvExport();

      
     

		protected override void OnInit()
		{
            CalculateOnClosedBar = true;

            //Set the default time frame if you start the strategy via the strategy-escort
            //if you start the strategy on a chart the TimeFrame is automatically set, this will lead to a better usability
            if (this.TimeFrame == null || this.TimeFrame.PeriodicityValue == 0)
            {
                this.TimeFrame = new TimeFrame(DatafeedHistoryPeriodicity.Hour, 1);
            }

            //For xMA200 we need at least 200 Bars.
            this.RequiredBarsCount = 200;
		}

        /// <summary>
        /// init data for multi frame
        /// </summary>
        protected override void OnBarsRequirements()
        {
            //Add(this.HigherTimeFrame.Periodicity, this.HigherTimeFrame.PeriodicityValue);
        }


        protected override void OnStart()
        {
            base.OnStart();

            //Print("OnStartUp" + Bars[0].Timestamp);

            //Init our indicator to get code access
            this._RunningWithTheWolves_Indicator = new RunningWithTheWolves_Indicator();
            
            //Initalize statistic data list if this feature is enabled
            if (this.StatisticBacktesting)
            {
                //this._StatisticContainer = new StatisticContainer();
                this._CsvExport = new CsvExport();
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            ////Print("OnTermination" + Bars[0].Timestamp);
            //IAccount account = this.Core.AccountManager.GetAccount(this.Instrument, true);

            //double counti = this.Core.TradingManager.GetExecutions(account, this.Backtesting.Settings.DateTimeRange.Lower, this.Backtesting.Settings.DateTimeRange.Upper).Count();
            //IEnumerable<ExecutionHistory> data  = this.Backtesting.TradingProcessor.GetExecutions(this.Backtesting.TradingProcessor.Accounts.First(), this.Backtesting.Settings.DateTimeRange.Lower, this.Backtesting.Settings.DateTimeRange.Upper);
            //foreach (ExecutionHistory item in data)
            //{
            //    item.Ex
            //}
            
            //Close statistic data list if this feature is enabled
            if (this.StatisticBacktesting)
            {
                //get the statistic data
                //this._StatisticContainer.copyToClipboard();
                //string myCsv = this._CsvExport.Export();
                this._CsvExport.CopyToClipboard();
            }
        }

		protected override void OnCalculate()
		{
            //Set automated during configuration in input dialog at strategy escort in chart
            this.IsAutoConfirmOrder = this.Autopilot;

            //calculate data
            OrderDirection_Enum? resultdata = this._RunningWithTheWolves_Indicator.calculate(Closes[0], this.MA_Selected, this.MA_Fast, this.MA_Medium, this.MA_Slow);
            if (resultdata.HasValue)
            {
                switch (resultdata)
                {
                    case OrderDirection_Enum.OpenLong:
                        this.DoEnterLong();
                        break;
                    case OrderDirection_Enum.OpenShort:
                        //this.DoEnterShort();
                        break;
                    case OrderDirection_Enum.CloseShort:
                        // && this._orderentershort.IsOpened && !this._orderentershort.IsManuallyConfirmable
                        if (this._orderentershort != null && this.IsAutoConfirmOrder)
                        {
                            CloseShortTrade(new StrategyOrderParameters {Type = OrderType.Market});
                            this._orderentershort = null;
                        }
                        break;
                    case OrderDirection_Enum.CloseLong:
                        // && this._orderenterlong.IsOpened && !this._orderenterlong.IsManuallyConfirmable
                        if (this._orderenterlong != null && this.IsAutoConfirmOrder)
                        {
                            CloseLongTrade(new StrategyOrderParameters {Type = OrderType.Market});
                            this._orderenterlong = null;
                        }
                        break;
                    default:
                        //nothing to do
                        break;
                }
            } 

            //Create statistic 
            if (this.StatisticBacktesting)
            {
                ////todo Create statistic only on bar close and not during the candle session
                //if (CalculateOnClosedBar == false)
                //{
                    _CsvExport.AddRow();
                    _CsvExport.AddRowBasicData(this, this.Instrument, this.TimeFrame, Bars[0], ProcessingBarIndex);

                    //Order &  Trade
                    _CsvExport["OrderAction"] = resultdata;

                    //Additional indicators
                    _CsvExport["SMA-20"] = SMA(Closes[0], 20)[0];
                    _CsvExport["SMA-50"] = SMA(Closes[0], 50)[0];
                    _CsvExport["SMA-200"] = SMA(Closes[0], 200)[0];

                    _CsvExport["RSI-14-3"] = RSI(Closes[0], 14, 3)[0];

                    ////If there is a higher Time Frame configured and we are not on the same time frame.
                    //if (Closes.Count == 2)
                    //{
                    //    _CsvExport["RSI-14-3_"+this.HigherTimeFrame.PeriodicityValue.ToString()+this.HigherTimeFrame.Periodicity.ToString()] = RSI(Closes[1], 14, 3)[0];
                    //}

                    // todo columns for trades
                    //TradeDirection;EntryReason;EntryDateTime;EntryPrice;EntryQuantity;EntryOrderType;ExitDateTime;ExitPrice;MinutesInMarket;ExitReason;ExitQuantity;ExitOrderType;PointsDiff;PointsDiffPerc;ProfitLoss;ProfitLossPercent;StopPrice;TargetPrice";
 
                //}
          
            }



		}



      

        /// <summary>
        /// OnExecution of orders
        /// </summary>
        /// <param name="execution"></param>
        protected override void OnOrderExecution(IExecution execution)
        {
            //Create statistic for execution
            if (this.StatisticBacktesting)
            {
                //this._StatisticContainer.Add(this.Root.Core.TradingManager, this, execution);
            }

            //send email
            if (this.Send_email)
            {
                this.SendEmail(Core.Settings.MailDefaultFromAddress, Core.PreferenceManager.DefaultEmailAddress,
                        GlobalUtilities.GetEmailSubject(execution), GlobalUtilities.GetEmailText(execution, this.GetType().Name));
            }
        }


        /// <summary>
        /// Create Long Order and Stop.
        /// </summary>
        private void DoEnterLong()
        {
            _orderenterlong = SubmitOrder(new StrategyOrderParameters {Direction = OrderDirection.Buy, Type = OrderType.Market, Quantity = GlobalUtilities.AdjustPositionToRiskManagement(this.Root.Core.AccountManager, this.Root.Core.PreferenceManager, this.Instrument, Bars[0].Close), SignalName =  this.GetType().Name + " " + PositionType.Long + "_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(), Instrument =  this.Instrument, TimeFrame =  this.TimeFrame});
            SetUpStopLoss(_orderenterlong.Name, CalculationMode.Price, Bars[0].Close / 1.05, false);
            SetUpProfitTarget(_orderenterlong.Name, CalculationMode.Price, Bars[0].Close * 1.11);
        }

        /// <summary>
        /// Create Short Order and Stop.
        /// </summary>
        private void DoEnterShort()
        {
            _orderentershort = SubmitOrder(new StrategyOrderParameters {Direction = OrderDirection.Sell, Type = OrderType.Market, Quantity = GlobalUtilities.AdjustPositionToRiskManagement(this.Root.Core.AccountManager, this.Root.Core.PreferenceManager, this.Instrument, Bars[0].Close), SignalName =  this.GetType().Name + " " + PositionType.Short + "_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(), Instrument =  this.Instrument, TimeFrame =  this.TimeFrame});
            SetUpStopLoss(_orderentershort.Name, CalculationMode.Price, Bars[0].Close * 1.05, false);
            SetUpProfitTarget(_orderentershort.Name, CalculationMode.Price, Bars[0].Close / 1.11);
        }


        public override string ToString()
        {
            return "Running with the wolves (S)";
        }

        public override string DisplayName
        {
            get
            {
                return "Running with the wolves (S)";
            }
        }

        #region Properties

        #region InSeries

        //private TimeFrame _HigherTimeFrame = new TimeFrame(DatafeedHistoryPeriodicity.Day, 1);
        ///// <summary>
        ///// </summary>
        //[Description("Select the higher time frame for this strategy.")]
        //[Category("Parameters")]
        //[DisplayName("Higher TimeFrame")]
        //public TimeFrame HigherTimeFrame
        //{
        //    get { return _HigherTimeFrame; }
        //    set
        //    {
        //        _HigherTimeFrame = value;
        //    }
        //}


        /// <summary>
        /// </summary>
        [Description("Select the type of MA you would like to use.")]
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
        [Description("Period for the slow mean average.")]
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
        [Description("Period for the medium mean average.")]
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
        [Description("Period for the fast mean average.")]
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
        [Description("If true it is allowed to go long.")]
        [Category("Parameters")]
        [DisplayName("Allow Long")]
        public bool IsLongEnabled
        {
            get { return _IsLongEnabled; }
            set { _IsLongEnabled = value; }
        }


        /// <summary>
        /// </summary>
        [Description("If true it is allowed to go short.")]
        [Category("Parameters")]
        [DisplayName("Allow Short")]
        public bool IsShortEnabled
        {
            get { return _IsShortEnabled; }
            set { _IsShortEnabled = value; }
        }

   

        [Description("If true an email will be send on order execution and on other important issues.")]
        [Category("Safety first!")]
        [DisplayName("Send email")]
        public bool Send_email
        {
            get { return _send_email; }
            set { _send_email = value; }
        }


        [Description("If true the strategy will handle everything. It will create buy orders, sell orders, stop loss orders, targets fully automatically.")]
        [Category("Safety first!")]
        [DisplayName("Autopilot")]
        public bool Autopilot
        {
            get { return _autopilot; }
            set { _autopilot = value; }
        }


        [Description("If true the strategy will create statistic data during the backtesting process.")]
        [Category("Safety first!")]
        [DisplayName("Create Statistic")]
        public bool StatisticBacktesting
        {
            get { return _statisticbacktesting; }
            set { _statisticbacktesting = value; }
        }


        #endregion

   

        #endregion

      

    }
}

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
/// You will find this indicator on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Use SMA or EMA crosses to find trends.")]
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
        private StatisticContainer _StatisticContainer = null;

		protected override void Initialize()
		{
            CalculateOnBarClose = true;

            //Set the default time frame if you start the strategy via the strategy-escort
            //if you start the strategy on a chart the TimeFrame is automatically set, this will lead to a better usability
            if (this.TimeFrame == null || this.TimeFrame.PeriodicityValue == 0)
            {
                this.TimeFrame = new TimeFrame(DatafeedHistoryPeriodicity.Hour, 1);
            }

            //For xMA200 we need at least 200 Bars.
            this.BarsRequired = 200;
		}

        protected override void OnStartUp()
        {
            base.OnStartUp();

            //Print("OnStartUp" + Bars[0].Time);

            //Init our indicator to get code access
            this._RunningWithTheWolves_Indicator = new RunningWithTheWolves_Indicator();

            //Initalize statistic data list if this feature is enabled
            if (this.StatisticBacktesting)
            {
                this._StatisticContainer = new StatisticContainer();
            }
        }

        protected override void OnTermination()
        {
            base.OnTermination();

            ////Print("OnTermination" + Bars[0].Time);
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
                this._StatisticContainer.copyToClipboard();
            }
        }

		protected override void OnBarUpdate()
		{
            //Set automated during configuration in input dialog at strategy escort in chart
            this.IsAutomated = this.Autopilot;

            //calculate data
            OrderAction? resultdata = this._RunningWithTheWolves_Indicator.calculate(Input, this.MA_Selected, this.MA_Fast, this.MA_Medium, this.MA_Slow);
            if (resultdata.HasValue)
            {
                switch (resultdata)
                {
                    case OrderAction.Buy:
                        this.DoEnterLong();
                        break;
                    case OrderAction.SellShort:
                        //this.DoEnterShort();
                        break;
                    case OrderAction.BuyToCover:
                        // && this._orderentershort.IsOpened && !this._orderentershort.IsManuallyConfirmable
                        if (this._orderentershort != null && this.IsAutomated)
                        {
                            ExitShort();
                            this._orderentershort = null;
                        }
                        break;
                    case OrderAction.Sell:
                        // && this._orderenterlong.IsOpened && !this._orderenterlong.IsManuallyConfirmable
                        if (this._orderenterlong != null && this.IsAutomated)
                        {
                            ExitLong();
                            this._orderenterlong = null;
                        }
                        break;
                    default:
                        //nothing to do
                        break;
                }
            }


            //double returnvalue = this._RunningWithTheWolves_Indicator.calculate(data);
            //Print(returnvalue);
            //if (returnvalue == 1)
            //{
            //    if (this._orderentershort != null)
            //    {
            //        ExitShort();
            //        this._orderentershort = null;
            //    }
            //    this.DoEnterLong();
            //}
            //else if (returnvalue == -1)
            //{
            //    if (this._orderenterlong != null)
            //    {
            //        ExitLong();
            //        this._orderenterlong = null;
            //    }
            //    this.DoEnterShort();
            //}
		}





        /// <summary>
        /// OnExecution of orders
        /// </summary>
        /// <param name="execution"></param>
        protected override void OnExecution(IExecution execution)
        {
            //Create statistic for execution
            if (this.StatisticBacktesting)
            {
                this._StatisticContainer.Add(this.Root.Core.TradingManager, this, execution);
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
            _orderenterlong = EnterLong(GlobalUtilities.AdjustPositionToRiskManagement(this.Root.Core.AccountManager, this.Root.Core.PreferenceManager, this.Instrument, Bars[0].Close), this.GetType().Name + " " + PositionType.Long + "_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(), this.Instrument, this.TimeFrame);
            //SetStopLoss(_orderenterlong.Name, CalculationMode.Price, this._orb_indicator.RangeLow, false);
            //SetProfitTarget(_orderenterlong.Name, CalculationMode.Percent, 5);
        }

        /// <summary>
        /// Create Short Order and Stop.
        /// </summary>
        private void DoEnterShort()
        {
            _orderentershort = EnterShort(GlobalUtilities.AdjustPositionToRiskManagement(this.Root.Core.AccountManager, this.Root.Core.PreferenceManager, this.Instrument, Bars[0].Close), this.GetType().Name + " " + PositionType.Short + "_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(), this.Instrument, this.TimeFrame);
            //SetStopLoss(_orderentershort.Name, CalculationMode.Price, this._orb_indicator.RangeHigh, false);
            //SetProfitTarget(_orderentershort.Name, CalculationMode.Percent, 5);
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

   

        [Description("If true an email will be send on order execution and on other important issues")]
        [Category("Safety first!")]
        [DisplayName("Send email")]
        public bool Send_email
        {
            get { return _send_email; }
            set { _send_email = value; }
        }


        [Description("If true the strategy will handle everything. It will create buy orders, sell orders, stop loss orders, targets fully automatically")]
        [Category("Safety first!")]
        [DisplayName("Autopilot")]
        public bool Autopilot
        {
            get { return _autopilot; }
            set { _autopilot = value; }
        }


        [Description("If true the strategy will create statistic data during the backtesting process")]
        [Category("Safety first!")]
        [DisplayName("Statistic Backtesting")]
        public bool StatisticBacktesting
        {
            get { return _statisticbacktesting; }
            set { _statisticbacktesting = value; }
        }


        #endregion

   

        #endregion

      

    }
}

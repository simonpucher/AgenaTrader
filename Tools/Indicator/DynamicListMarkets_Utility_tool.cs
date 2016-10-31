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
using System.Windows.Forms;

/// <summary>
/// Version: 1.1
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// todo
/// + if markets are closed no OnBarUpdate() is called so we need an timer event. 
/// -------------------------------------------------------------------------
/// Adds instruments dynamical to a static list (e.g. portfolio) if the market is currently open.
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Adds instruments dynamical to a static list (e.g. portfolio) if the market is currently open.")]
    public class DynamicListMarkets_Indicator_Tool : UserIndicator
    {
        #region Variables

        private bool _usemarkethours = true;
        private string _instrumentlists = "DAX30;ATX20;DOW30;NASDAQ";
        private static DateTime _lastupdate = DateTime.Now;
        private int _seconds = 10;

        private string _name_of_list = String.Empty;
        private IInstrumentsList _list = null;


        #endregion

        protected override void Initialize()
        {
            Overlay = true;
            CalculateOnBarClose = false;
        }


        protected override void OnStartUp()
        {

            if (this.Instrument != null)
            {
                if (!String.IsNullOrEmpty(Name_of_list))
                {

                    this.Root.Core.InstrumentManager.GetInstrumentLists();
                    _list = this.Root.Core.InstrumentManager.GetInstrumentsListStatic(this.Name_of_list);
                    //if (_list == null)
                    //{
                    //    _list = this.Root.Core.InstrumentManager.GetInstrumentsListDynamic(this.Name_of_list);
                    //}
                    if (_list == null || _list.Count == 0)
                    {
                        Log(this.DisplayName + ": The list " + this.Name_of_list + " does not exist.", InfoLogLevel.Warning);
                    }
                }
                else
                {
                    Log(this.DisplayName + ": You need to specify a name for the list.", InfoLogLevel.Warning);
                }
            }
            
            this.CheckForNewInstruments();

        }


        protected override void OnBarUpdate()
        {
            this.CheckForNewInstruments();
        }



        private void CheckForNewInstruments() {


            if (_lastupdate.AddSeconds(this._seconds) < DateTime.Now)
            {
                if (_list != null)
                {
                    this.Root.Core.InstrumentManager.ClearInstrumentList(this.Name_of_list);
                }

               
                if (UseMarketHours && !String.IsNullOrWhiteSpace(this.Instrumentlists))
                {
                    string[] arr_Instrumentlists = this.Instrumentlists.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr_Instrumentlists != null && arr_Instrumentlists.Count() > 0)
                    {
                        foreach (string item in arr_Instrumentlists)
                        {
                            IInstrumentsList instlist = this.Root.Core.InstrumentManager.GetInstrumentsListStatic(item);

                            if (instlist != null && instlist.Count() > 0)
                            {
                                ITimePeriod timper = this.Root.Core.MarketplaceManager.GetExchangeDescription(instlist.First().Exchange).TradingHours;


                                if ((DateTime.Now.TimeOfDay > timper.StartTime) && (DateTime.Now.TimeOfDay < timper.EndTime))
                                {
                                    foreach (IInstrument inst in instlist)
                                    {
                                        if (!_list.Contains(inst))
                                        {
                                            this.Root.Core.InstrumentManager.AddInstrument2List(inst, this.Name_of_list);
                                        }
                                    }
                                }
                            }

                        }
                        

                    }
                }
                
                _lastupdate = DateTime.Now;
            }
            
        }

        



        public override string DisplayName
        {
            get
            {
                return "Dynamic List Markets (T)";
            }
        }


        public override string ToString()
        {
            return "Dynamic List Markets (T)";
        }




        #region Properties

    

        #region Input


        [Description("The name of the static list to which you would like to add the instruments.")]
            [Category("Parameters")]
            [DisplayName("Static list")]
            public string Name_of_list
            {
                get { return _name_of_list; }
                set { _name_of_list = value; }
            }
        

        [Description("If true then all markets with active trading session will be added to the static list.")]
        [Category("Parameters")]
        [DisplayName("Use market hours")]
        public bool UseMarketHours
        {
            get { return _usemarkethours; }
            set { _usemarkethours = value; }
        }

        [Description("All of these markets will be added automatically to your list if if the market is currently open.")]
        [Category("Parameters")]
        [DisplayName("Markets")]
        public string Instrumentlists
        {
            get { return _instrumentlists; }
            set { _instrumentlists = value; }
        }


        [Description("Update interval in seconds.")]
        [Category("Parameters")]
        [DisplayName("Update interval")]
        public int Seconds
        {
            get { return _seconds; }
            set { _seconds = value; }
        }


        #endregion



        #endregion


    }
}



#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator
	{
		/// <summary>
		/// Adds instruments dynamical to a static list (e.g. portfolio) if the market is currently open.
		/// </summary>
		public DynamicListMarkets_Indicator_Tool DynamicListMarkets_Indicator_Tool(System.String name_of_list, System.Boolean useMarketHours, System.String instrumentlists, System.Int32 seconds)
        {
			return DynamicListMarkets_Indicator_Tool(Input, name_of_list, useMarketHours, instrumentlists, seconds);
		}

		/// <summary>
		/// Adds instruments dynamical to a static list (e.g. portfolio) if the market is currently open.
		/// </summary>
		public DynamicListMarkets_Indicator_Tool DynamicListMarkets_Indicator_Tool(IDataSeries input, System.String name_of_list, System.Boolean useMarketHours, System.String instrumentlists, System.Int32 seconds)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<DynamicListMarkets_Indicator_Tool>(input, i => i.Name_of_list == name_of_list && i.UseMarketHours == useMarketHours && i.Instrumentlists == instrumentlists && i.Seconds == seconds);

			if (indicator != null)
				return indicator;

			indicator = new DynamicListMarkets_Indicator_Tool
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							Name_of_list = name_of_list,
							UseMarketHours = useMarketHours,
							Instrumentlists = instrumentlists,
							Seconds = seconds
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
		/// Adds instruments dynamical to a static list (e.g. portfolio) if the market is currently open.
		/// </summary>
		public DynamicListMarkets_Indicator_Tool DynamicListMarkets_Indicator_Tool(System.String name_of_list, System.Boolean useMarketHours, System.String instrumentlists, System.Int32 seconds)
		{
			return LeadIndicator.DynamicListMarkets_Indicator_Tool(Input, name_of_list, useMarketHours, instrumentlists, seconds);
		}

		/// <summary>
		/// Adds instruments dynamical to a static list (e.g. portfolio) if the market is currently open.
		/// </summary>
		public DynamicListMarkets_Indicator_Tool DynamicListMarkets_Indicator_Tool(IDataSeries input, System.String name_of_list, System.Boolean useMarketHours, System.String instrumentlists, System.Int32 seconds)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.DynamicListMarkets_Indicator_Tool(input, name_of_list, useMarketHours, instrumentlists, seconds);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Adds instruments dynamical to a static list (e.g. portfolio) if the market is currently open.
		/// </summary>
		public DynamicListMarkets_Indicator_Tool DynamicListMarkets_Indicator_Tool(System.String name_of_list, System.Boolean useMarketHours, System.String instrumentlists, System.Int32 seconds)
		{
			return LeadIndicator.DynamicListMarkets_Indicator_Tool(Input, name_of_list, useMarketHours, instrumentlists, seconds);
		}

		/// <summary>
		/// Adds instruments dynamical to a static list (e.g. portfolio) if the market is currently open.
		/// </summary>
		public DynamicListMarkets_Indicator_Tool DynamicListMarkets_Indicator_Tool(IDataSeries input, System.String name_of_list, System.Boolean useMarketHours, System.String instrumentlists, System.Int32 seconds)
		{
			return LeadIndicator.DynamicListMarkets_Indicator_Tool(input, name_of_list, useMarketHours, instrumentlists, seconds);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Adds instruments dynamical to a static list (e.g. portfolio) if the market is currently open.
		/// </summary>
		public DynamicListMarkets_Indicator_Tool DynamicListMarkets_Indicator_Tool(System.String name_of_list, System.Boolean useMarketHours, System.String instrumentlists, System.Int32 seconds)
		{
			return LeadIndicator.DynamicListMarkets_Indicator_Tool(Input, name_of_list, useMarketHours, instrumentlists, seconds);
		}

		/// <summary>
		/// Adds instruments dynamical to a static list (e.g. portfolio) if the market is currently open.
		/// </summary>
		public DynamicListMarkets_Indicator_Tool DynamicListMarkets_Indicator_Tool(IDataSeries input, System.String name_of_list, System.Boolean useMarketHours, System.String instrumentlists, System.Int32 seconds)
		{
			return LeadIndicator.DynamicListMarkets_Indicator_Tool(input, name_of_list, useMarketHours, instrumentlists, seconds);
		}
	}

	#endregion

}

#endregion

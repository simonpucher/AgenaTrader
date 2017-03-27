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
/// + if markets are closed no OnCalculate() is called so we need an timer event.
/// -------------------------------------------------------------------------
/// Adds instruments dynamical to a static list (e.g. portfolio) if there is an order on it or there is an trade on it.
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this indicator without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Adds instruments dynamical to a static list (e.g. portfolio) if there is an order on it or there is an trade on it.")]
    public class DynamicListTrades_Indicator_Tool : UserIndicator
    {
        #region Variables

        private bool _showtrades = true;
        private bool _showproposals = true;
        private bool _showpricealert = true;

        private static DateTime _lastupdate = DateTime.Now;
        private int _seconds = 10; 

        private string _name_of_list = String.Empty;
        private IInstrumentsList _list = null;

        private static  IEnumerable<ITradingTrade> _openedtrades = null;
        private static IEnumerable<ITradingOrder> _regorders = null;
        private static IEnumerable<IPriceAlert> pricealerts = null;

        #endregion

        protected override void OnInit()
        {
            IsOverlay = true;
            CalculateOnClosedBar = false;
        }


   


        protected override void OnCalculate()
        {
            this.CheckForNewInstruments();
        }

      



        private void CheckForNewInstruments() {


            if (_lastupdate.AddSeconds(this._seconds) < DateTime.Now)
            {
                if (!String.IsNullOrEmpty(Name_of_list))
                {

                    this.Root.Core.InstrumentManager.GetInstrumentLists();
                    _list = this.Root.Core.InstrumentManager.GetInstrumentsListStatic(this.Name_of_list);
                    if (_list == null || _list.Count == 0)
                    {
                        Log(this.DisplayName + ": The list " + this.Name_of_list + " does not exist.", InfoLogLevel.Warning);
                    }
                }
                else
                {
                    Log(this.DisplayName + ": You need to specify a name for the list.", InfoLogLevel.Warning);
                }

                if (_list != null)
                {
                    this.Root.Core.InstrumentManager.ClearInstrumentList(this.Name_of_list);
                }

                if (this.ShowProposals)
                {
                    _regorders = this.Root.Core.TradingManager.ActiveRegisteredOrders;
                    if (_regorders != null)
                    {
                        foreach (IInstrument item in _regorders.Select(x => x.Instrument).Distinct())
                        {
                            if (!_list.Contains(item))
                            {
                                this.Root.Core.InstrumentManager.AddInstrument2List(item, this.Name_of_list);
                            }
                        }
                    }
                }

                if (this.ShowTrades)
                {
                    _openedtrades = this.Root.Core.TradingManager.GetOpenedTrades();
                    if (_openedtrades != null)
                    {
                        foreach (ITradingTrade item in _openedtrades)
                        {
                            if (!_list.Contains((IInstrument)item.Instrument))
                            {
                                this.Root.Core.InstrumentManager.AddInstrument2List((IInstrument)item.Instrument, this.Name_of_list);
                            }
                        }
                    }
                }

                if (this.ShowPriceAlert)
                {
                    pricealerts =  this.Root.Core.AlertManager.PriceAlerts;
                    if (pricealerts != null)
                    {
                        foreach (IPriceAlert item in pricealerts)
                        {
                            if (!_list.Contains((IInstrument)item.Instrument))
                            {
                                this.Root.Core.InstrumentManager.AddInstrument2List((IInstrument)item.Instrument, this.Name_of_list);
                            }
                        }
                    }
                }
                
                _lastupdate = DateTime.Now;
            
                //((AgenaTrader.Plugins.IWatchComponent[])((AgenaTrader.WatchManager.WatchManager)this.Root.Core.WatchManager).GetComponents)[0].ReloadInstrumentList();
            }
            
        }

        



        public override string DisplayName
        {
            get
            {
                return "DLT (T)";
            }
        }


        public override string ToString()
        {
            return "DLT (T)";
        }




        #region Properties

   

        #region InSeries


        [Description("The name of the static list to which you would like to add the instruments.")]
            [Category("Parameters")]
            [DisplayName("Static list")]
            public string Name_of_list
            {
                get { return _name_of_list; }
                set { _name_of_list = value; }
            }

        [Description("If true then all open trades will be added dynamical to the static list.")]
        [Category("Parameters")]
        [DisplayName("Show trades")]
        public bool ShowTrades
        {
            get { return _showtrades; }
            set { _showtrades = value; }
        }

        
        [Description("If true then all active proposals will be added dynamical to the static list.")]
        [Category("Parameters")]
        [DisplayName("Show proposals")]
        public bool ShowProposals
        {
            get { return _showproposals; }
            set { _showproposals = value; }
        }

        [Description("If true then all instruments with price alerts will be added dynamical to the static list.")]
        [Category("Parameters")]
        [DisplayName("Show price alerts")]
        public bool ShowPriceAlert
        {
            get { return _showpricealert; }
            set { _showpricealert = value; }
        }

        [Description("Update interval in seconds.")]
        [Category("Parameters")]
        [DisplayName("Update interval (sec.)")]
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
		/// Adds instruments dynamical to a static list (e.g. portfolio) if there is an order on it or there is an trade on it.
		/// </summary>
		public DynamicListTrades_Indicator_Tool DynamicListTrades_Indicator_Tool(System.String name_of_list, System.Boolean showTrades, System.Boolean showProposals, System.Boolean showPriceAlert, System.Int32 seconds)
        {
			return DynamicListTrades_Indicator_Tool(InSeries, name_of_list, showTrades, showProposals, showPriceAlert, seconds);
		}

		/// <summary>
		/// Adds instruments dynamical to a static list (e.g. portfolio) if there is an order on it or there is an trade on it.
		/// </summary>
		public DynamicListTrades_Indicator_Tool DynamicListTrades_Indicator_Tool(IDataSeries input, System.String name_of_list, System.Boolean showTrades, System.Boolean showProposals, System.Boolean showPriceAlert, System.Int32 seconds)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<DynamicListTrades_Indicator_Tool>(input, i => i.Name_of_list == name_of_list && i.ShowTrades == showTrades && i.ShowProposals == showProposals && i.ShowPriceAlert == showPriceAlert && i.Seconds == seconds);

			if (indicator != null)
				return indicator;

			indicator = new DynamicListTrades_Indicator_Tool
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input,
							Name_of_list = name_of_list,
							ShowTrades = showTrades,
							ShowProposals = showProposals,
							ShowPriceAlert = showPriceAlert,
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
		/// Adds instruments dynamical to a static list (e.g. portfolio) if there is an order on it or there is an trade on it.
		/// </summary>
		public DynamicListTrades_Indicator_Tool DynamicListTrades_Indicator_Tool(System.String name_of_list, System.Boolean showTrades, System.Boolean showProposals, System.Boolean showPriceAlert, System.Int32 seconds)
		{
			return LeadIndicator.DynamicListTrades_Indicator_Tool(InSeries, name_of_list, showTrades, showProposals, showPriceAlert, seconds);
		}

		/// <summary>
		/// Adds instruments dynamical to a static list (e.g. portfolio) if there is an order on it or there is an trade on it.
		/// </summary>
		public DynamicListTrades_Indicator_Tool DynamicListTrades_Indicator_Tool(IDataSeries input, System.String name_of_list, System.Boolean showTrades, System.Boolean showProposals, System.Boolean showPriceAlert, System.Int32 seconds)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.DynamicListTrades_Indicator_Tool(input, name_of_list, showTrades, showProposals, showPriceAlert, seconds);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Adds instruments dynamical to a static list (e.g. portfolio) if there is an order on it or there is an trade on it.
		/// </summary>
		public DynamicListTrades_Indicator_Tool DynamicListTrades_Indicator_Tool(System.String name_of_list, System.Boolean showTrades, System.Boolean showProposals, System.Boolean showPriceAlert, System.Int32 seconds)
		{
			return LeadIndicator.DynamicListTrades_Indicator_Tool(InSeries, name_of_list, showTrades, showProposals, showPriceAlert, seconds);
		}

		/// <summary>
		/// Adds instruments dynamical to a static list (e.g. portfolio) if there is an order on it or there is an trade on it.
		/// </summary>
		public DynamicListTrades_Indicator_Tool DynamicListTrades_Indicator_Tool(IDataSeries input, System.String name_of_list, System.Boolean showTrades, System.Boolean showProposals, System.Boolean showPriceAlert, System.Int32 seconds)
		{
			return LeadIndicator.DynamicListTrades_Indicator_Tool(input, name_of_list, showTrades, showProposals, showPriceAlert, seconds);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Adds instruments dynamical to a static list (e.g. portfolio) if there is an order on it or there is an trade on it.
		/// </summary>
		public DynamicListTrades_Indicator_Tool DynamicListTrades_Indicator_Tool(System.String name_of_list, System.Boolean showTrades, System.Boolean showProposals, System.Boolean showPriceAlert, System.Int32 seconds)
		{
			return LeadIndicator.DynamicListTrades_Indicator_Tool(InSeries, name_of_list, showTrades, showProposals, showPriceAlert, seconds);
		}

		/// <summary>
		/// Adds instruments dynamical to a static list (e.g. portfolio) if there is an order on it or there is an trade on it.
		/// </summary>
		public DynamicListTrades_Indicator_Tool DynamicListTrades_Indicator_Tool(IDataSeries input, System.String name_of_list, System.Boolean showTrades, System.Boolean showProposals, System.Boolean showPriceAlert, System.Int32 seconds)
		{
			return LeadIndicator.DynamicListTrades_Indicator_Tool(input, name_of_list, showTrades, showProposals, showPriceAlert, seconds);
		}
	}

	#endregion

}

#endregion

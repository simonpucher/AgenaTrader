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
/// Version: 1.3.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// todo
/// + if markets are closed no OnCalculate() is called so we need an timer event. 
/// -------------------------------------------------------------------------
/// Adds instruments dynamical to a static list (e.g. portfolio) if the market is currently open.
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this indicator without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
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
        private string _instrumentlists = "DAX30;ATX20;DOW30;NASDAQ;S&P500";
        private static DateTime _lastupdate = DateTime.Now;
        private int _seconds = 60;

        private string _name_of_list = String.Empty;
        private IInstrumentsList _list = null;


        #endregion

        protected override void OnInit()
        {
            IsOverlay = true;
            CalculateOnClosedBar = false;
        }


        protected override void OnStart()
        {
            this.CheckForNewInstruments();
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
                    //this.Root.Core.InstrumentManager.ClearInstrumentList(this.Name_of_list);
                    Core.GuiManager.BeginInvoke((Action)(() => this.Root.Core.InstrumentManager.ClearInstrumentList(this.Name_of_list)));

                }


                if (!String.IsNullOrWhiteSpace(this.Instrumentlists))
                {
                    string[] arr_Instrumentlists = this.Instrumentlists.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr_Instrumentlists != null && arr_Instrumentlists.Count() > 0)
                    {
                        foreach (string item in arr_Instrumentlists)
                        {
                            IInstrumentsList instlist = this.Root.Core.InstrumentManager.GetInstrumentsListStatic(item);

                            if (instlist != null && instlist.Count() > 0)
                            {
                                if (UseMarketHours)
                                {
                                    ITimePeriod timper = this.Root.Core.MarketplaceManager.GetExchangeDescription(instlist.First().Exchange).TradingHours;
                                    
                                    if ((DateTime.Now.TimeOfDay > timper.StartTime) && (DateTime.Now.TimeOfDay < timper.EndTime))
                                    {
                                        foreach (IInstrument inst in instlist)
                                        {
                                            if (!_list.Contains(inst))
                                            {
                                                //this.Root.Core.InstrumentManager.AddInstrument2List(inst, this.Name_of_list);
                                                Core.GuiManager.BeginInvoke((Action)(() => Core.InstrumentManager.AddInstrument2List(inst, this.Name_of_list)));
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (IInstrument inst in instlist)
                                    {
                                        if (!_list.Contains(inst))
                                        {
                                            //this.Root.Core.InstrumentManager.AddInstrument2List(inst, this.Name_of_list);
                                            Core.GuiManager.BeginInvoke((Action)(() => Core.InstrumentManager.AddInstrument2List(inst, this.Name_of_list)));
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
                return "DLM (T)";
            }
        }


        public override string ToString()
        {
            return "DLM (T)";
        }




        #region Properties

    

        #region InSeries


        [Description("The name of the static list to which you would like to add the instruments.")]
            [InputParameter]
            [DisplayName("Static list")]
            public string Name_of_list
            {
                get { return _name_of_list; }
                set { _name_of_list = value; }
            }
        

        [Description("If true then all markets with active trading session will be added to the static list.")]
        [InputParameter]
        [DisplayName("Use market hours")]
        public bool UseMarketHours
        {
            get { return _usemarkethours; }
            set { _usemarkethours = value; }
        }

        [Description("All of these markets will be added automatically to your list if if the market is currently open.")]
        [InputParameter]
        [DisplayName("Markets")]
        public string Instrumentlists
        {
            get { return _instrumentlists; }
            set { _instrumentlists = value; }
        }


        [Description("Update interval in seconds.")]
        [InputParameter]
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
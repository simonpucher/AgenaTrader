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
/// Version: 1.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// Check if the current instrument is in a list.
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    
	[Description("Check if the current instrument is in a list.")]
    public class InstrumentIsInList_Tool : UserIndicator
	{
		#region Variables

		private string _instrumentlist = "";
        private IInstrumentsList _list = null;

        #endregion

        protected override void OnInit()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
			IsOverlay = true;
			CalculateOnClosedBar = true;
		}


        protected override void OnCalculate()
		{
            if (this.IsProcessingBarIndexLast && this.Instrument != null)
            {
                if (!String.IsNullOrEmpty(Instrumentlist))
                {
                     
                    this.Root.Core.InstrumentManager.GetInstrumentLists();
                    _list = this.Root.Core.InstrumentManager.GetInstrumentsListStatic(this.Instrumentlist);
                    
                    //if (_list == null)
                    //{
                    //    _list = this.Root.Core.InstrumentManager.GetInstrumentsListDynamic(this.Name_of_list);
                    //}
                    if (_list == null)
                    {
                        Log(this.DisplayName + ": The list " + this.Instrumentlist + " does not exist.", InfoLogLevel.Warning);
                        MyPlot1.Set(-1);
                    }

                    //We have found a list
                    if (_list.Contains((Instrument)this.Instrument))
                    {
                        MyPlot1.Set(1);
                    }
                    else
                    {
                        MyPlot1.Set(0);
                    }

                }
                else
                {
                    Log(this.DisplayName + ": You need to specify a name for the list.", InfoLogLevel.Warning);
                    MyPlot1.Set(-1);
                }
            }


            //MyPlot1.Set(InSeries[0]);
		}


        public override string DisplayName
        {
            get
            {
                return "Instrument is in list (T)";
            }
        }


        public override string ToString()
        {
            return "Instrument is in list (T)";
        }

        #region Properties

        [Browsable(false)]
		[XmlIgnore()]
		public DataSeries MyPlot1
		{
			get { return Outputs[0]; }
		}

        [Description("The name of the static list to which you would like to use.")]
        [Category("Parameters")]
        [DisplayName("Static list")]
        public string Instrumentlist
		{
			get { return _instrumentlist; }
			set { _instrumentlist = value; }
		}

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
		/// Check if the current instrument is in a list.
		/// </summary>
		public InstrumentIsInList_Tool InstrumentIsInList_Tool(System.String instrumentlist)
        {
			return InstrumentIsInList_Tool(InSeries, instrumentlist);
		}

		/// <summary>
		/// Check if the current instrument is in a list.
		/// </summary>
		public InstrumentIsInList_Tool InstrumentIsInList_Tool(IDataSeries input, System.String instrumentlist)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<InstrumentIsInList_Tool>(input, i => i.Instrumentlist == instrumentlist);

			if (indicator != null)
				return indicator;

			indicator = new InstrumentIsInList_Tool
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input,
							Instrumentlist = instrumentlist
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
		/// Check if the current instrument is in a list.
		/// </summary>
		public InstrumentIsInList_Tool InstrumentIsInList_Tool(System.String instrumentlist)
		{
			return LeadIndicator.InstrumentIsInList_Tool(InSeries, instrumentlist);
		}

		/// <summary>
		/// Check if the current instrument is in a list.
		/// </summary>
		public InstrumentIsInList_Tool InstrumentIsInList_Tool(IDataSeries input, System.String instrumentlist)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.InstrumentIsInList_Tool(input, instrumentlist);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Check if the current instrument is in a list.
		/// </summary>
		public InstrumentIsInList_Tool InstrumentIsInList_Tool(System.String instrumentlist)
		{
			return LeadIndicator.InstrumentIsInList_Tool(InSeries, instrumentlist);
		}

		/// <summary>
		/// Check if the current instrument is in a list.
		/// </summary>
		public InstrumentIsInList_Tool InstrumentIsInList_Tool(IDataSeries input, System.String instrumentlist)
		{
			return LeadIndicator.InstrumentIsInList_Tool(input, instrumentlist);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Check if the current instrument is in a list.
		/// </summary>
		public InstrumentIsInList_Tool InstrumentIsInList_Tool(System.String instrumentlist)
		{
			return LeadIndicator.InstrumentIsInList_Tool(InSeries, instrumentlist);
		}

		/// <summary>
		/// Check if the current instrument is in a list.
		/// </summary>
		public InstrumentIsInList_Tool InstrumentIsInList_Tool(IDataSeries input, System.String instrumentlist)
		{
			return LeadIndicator.InstrumentIsInList_Tool(input, instrumentlist);
		}
	}

	#endregion

}

#endregion

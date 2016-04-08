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
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("Adds the instrument to a static list.")]
	public class QuickAdd : UserIndicator
	{
		#region Variables

		private string _name_of_list = "";

		#endregion

        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
            Overlay = true;

        }


        protected override void OnStartUp()
        {
            base.OnStartUp();

            if (!String.IsNullOrEmpty(Name_of_list))
            {

                this.Root.Core.InstrumentManager.GetInstrumentLists();
                IInstrumentsList liste = this.Root.Core.InstrumentManager.GetInstrumentsListStatic(this.Name_of_list);
                if (liste == null)
                {
                    liste = this.Root.Core.InstrumentManager.GetInstrumentsListDynamic(this.Name_of_list);
                }
                if (liste != null)
                {
                    //Get instrument
                    Instrument instrument = this.Root.Core.InstrumentManager.GetInstrument("SKB.DE");

                   // System.Configuration.SettingsProvider;

                    // liste.Add(instrument);

                    //If you want to clear instruments from the list
                    //liste.Clear();

                    if (!this.Root.Core.InstrumentManager.IsInstrumentExists(instrument.Symbol))
                    {
                        this.Root.Core.InstrumentManager.AddInstrument2List(instrument, this.Name_of_list); 
                    }

                }
                else
                {
                    ////todo: log it!
                    //Log("Das ist eine Nachricht.", InfoLogLevel.Message);
                }

            }

          


        }



		protected override void OnBarUpdate()
		{
			MyPlot1.Set(Input[0]);


		}

		#region Properties

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries MyPlot1
		{
			get { return Values[0]; }
		}

		[Description("The name of the list you would like to add instruments.")]
		[Category("Parameters")]
		public string Name_of_list
		{
			get { return _name_of_list; }
			set { _name_of_list = value; }
		}

        public override string DisplayName
        {
            get
            {
                return "QA";//base.DisplayName;
            }
        }

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
		/// Adds the instrument to a static list.
		/// </summary>
		public QuickAdd QuickAdd(System.String name_of_list)
        {
			return QuickAdd(Input, name_of_list);
		}

		/// <summary>
		/// Adds the instrument to a static list.
		/// </summary>
		public QuickAdd QuickAdd(IDataSeries input, System.String name_of_list)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<QuickAdd>(input, i => i.Name_of_list == name_of_list);

			if (indicator != null)
				return indicator;

			indicator = new QuickAdd
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							Name_of_list = name_of_list
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
		/// Adds the instrument to a static list.
		/// </summary>
		public QuickAdd QuickAdd(System.String name_of_list)
		{
			return LeadIndicator.QuickAdd(Input, name_of_list);
		}

		/// <summary>
		/// Adds the instrument to a static list.
		/// </summary>
		public QuickAdd QuickAdd(IDataSeries input, System.String name_of_list)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.QuickAdd(input, name_of_list);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Adds the instrument to a static list.
		/// </summary>
		public QuickAdd QuickAdd(System.String name_of_list)
		{
			return LeadIndicator.QuickAdd(Input, name_of_list);
		}

		/// <summary>
		/// Adds the instrument to a static list.
		/// </summary>
		public QuickAdd QuickAdd(IDataSeries input, System.String name_of_list)
		{
			return LeadIndicator.QuickAdd(input, name_of_list);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Adds the instrument to a static list.
		/// </summary>
		public QuickAdd QuickAdd(System.String name_of_list)
		{
			return LeadIndicator.QuickAdd(Input, name_of_list);
		}

		/// <summary>
		/// Adds the instrument to a static list.
		/// </summary>
		public QuickAdd QuickAdd(IDataSeries input, System.String name_of_list)
		{
			return LeadIndicator.QuickAdd(input, name_of_list);
		}
	}

	#endregion

}

#endregion

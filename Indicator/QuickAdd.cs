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

		    private string _name_of_list = String.Empty;


		#endregion

        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
            Overlay = true;
        }


        protected override void OnStartUp()
        {
            base.OnStartUp();

            if (this.Instrument != null)
            {
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
                        // Instrument instrument = this.Root.Core.InstrumentManager.GetInstrument("SKB.DE");

                        // liste.Add(instrument);

                        //If you want to clear instruments from the list
                        //liste.Clear();

                        if (!this.Root.Core.InstrumentManager.IsInstrumentExists(this.Instrument.Symbol))
                        {
                            this.Root.Core.InstrumentManager.AddInstrument2List(this.Instrument, this.Name_of_list);
                        }

                    }
                    else
                    {
                        Log("The list " + this.Name_of_list + " does not exist.", InfoLogLevel.Warning);
                    }

                }
                else
                {
                    Log("You need to specify a name for the list.", InfoLogLevel.Warning);
                }
            }


        }







		protected override void OnBarUpdate()
		{
			MyPlot1.Set(Input[0]);


		}

        protected override void OnTermination()
        {
        
        }


        public override string DisplayName
        {
            get
            {
                return "QA";
            }
        }


        public override string ToString()
        {
            return "QA";
        }



		#region Properties

            #region Output

            [Browsable(false)]
            [XmlIgnore()]
            public DataSeries MyPlot1
            {
                get { return Values[0]; }
            }

            #endregion

            #region Input


            [Description("The name of the list to which you would like to add instruments.")]
            //[Category("Values")]
            [DisplayName("Name of the list")]
            public string Name_of_list
            {
                get { return _name_of_list; }
                set { _name_of_list = value; }
            }
            #endregion

  

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
		public QuickAdd QuickAdd()
        {
			return QuickAdd(Input);
		}

		/// <summary>
		/// Adds the instrument to a static list.
		/// </summary>
		public QuickAdd QuickAdd(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<QuickAdd>(input);

			if (indicator != null)
				return indicator;

			indicator = new QuickAdd
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
		/// Adds the instrument to a static list.
		/// </summary>
		public QuickAdd QuickAdd()
		{
			return LeadIndicator.QuickAdd(Input);
		}

		/// <summary>
		/// Adds the instrument to a static list.
		/// </summary>
		public QuickAdd QuickAdd(IDataSeries input)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.QuickAdd(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Adds the instrument to a static list.
		/// </summary>
		public QuickAdd QuickAdd()
		{
			return LeadIndicator.QuickAdd(Input);
		}

		/// <summary>
		/// Adds the instrument to a static list.
		/// </summary>
		public QuickAdd QuickAdd(IDataSeries input)
		{
			return LeadIndicator.QuickAdd(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Adds the instrument to a static list.
		/// </summary>
		public QuickAdd QuickAdd()
		{
			return LeadIndicator.QuickAdd(Input);
		}

		/// <summary>
		/// Adds the instrument to a static list.
		/// </summary>
		public QuickAdd QuickAdd(IDataSeries input)
		{
			return LeadIndicator.QuickAdd(input);
		}
	}

	#endregion

}

#endregion

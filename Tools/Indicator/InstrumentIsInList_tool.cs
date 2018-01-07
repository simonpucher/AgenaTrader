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
/// Version: 1.1
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// Check if the current instrument is in a list.
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this indicator without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
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
			Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
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
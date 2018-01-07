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
/// todo description
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("Condition returns false if trading break is active.")]
	[IsEntryAttribute(true)]
	[IsStopAttribute(false)]
	[IsTargetAttribute(false)]
	[OverrulePreviousStopPrice(false)]
	public class TradingTimeActive_Condition : UserScriptedCondition
	{
        #region Variables

        //private TimeSpan _start = new TimeSpan(12, 00, 00);
        //private TimeSpan _end = new TimeSpan(13, 00, 00);

        private string _start ="12:00:00";
        private string _end = "13:00:00";

        #endregion

        protected override void OnInit()
		{
			IsEntry = true;
			IsStop = false;
			IsTarget = false;
			Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Black), "Occurred"));
			//Add(new OutputDescriptor(Color.FromArgb(255, 183, 128, 170), "Entry"));
			IsOverlay = false;
		}

		protected override void OnCalculate()
		{
            //Print(Time[0]);

            TimeSpan now = Time[0].TimeOfDay;

            if ((now >= TimeSpan.Parse(this.Start)) && (now <= TimeSpan.Parse(this.End)))
            {
                Occurred.Set(0);
            }
            else
            {
                Occurred.Set(1);
            }


        }

		#region Properties

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Occurred
		{
			get { return Outputs[0]; }
		}

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Entry
        {
            get { return Outputs[1]; }
        }

        public override IList<DataSeries> GetEntries()
        {
            return new[] { Entry };
        }

        [Description("Start of the trading break. (e.g. 12:00:00)")]
        [Category("Parameters")]
        [DisplayName("Start")]
        public string Start
        {
            get { return _start; }
            set { _start = value; }
        }

        //[Description("Start of the trading break. (e.g. 12:00:00)")]
        //[Category("Parameters")]
        //[DisplayName("Start")]
        //public TimeSpan Start
        //{
        //    get { return _start; }
        //    set { _start = value; }
        //}

        //[Browsable(false)]
        //public string StartSerialize
        //{
        //    get { return _start.ToString(); }
        //    set { _start = TimeSpan.Parse(value); }
        //}

        [Description("End of the trading break. (e.g. 13:00:00)")]
        [Category("Parameters")]
        [DisplayName("End")]
        public string End
        {
            get { return _end; }
            set { _end = value; }
        }

        //[Description("End of the trading break. (e.g. 13:00:00)")]
        //[Category("Parameters")]
        //[DisplayName("End")]
        //public TimeSpan End
        //{
        //    get { return _end; }
        //    set { _end = value; }
        //}

        //[Browsable(false)]
        //public string EndSerialize
        //{
        //    get { return _end.ToString(); }
        //    set { _end = TimeSpan.Parse(value); }
        //}

        #endregion
    }
}
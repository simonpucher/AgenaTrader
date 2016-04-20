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
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Open Range Breakout Stop")]
	[IsEntryAttribute(false)]
	[IsStopAttribute(true)]
	[IsTargetAttribute(false)]
	[OverrulePreviousStopPrice(false)]
    public class ORB_Condition_Stop : UserScriptedCondition
	{
        //todo implement interface IORB

		#region Variables

        //private int _myCondition1 = 1;

  

		#endregion

		protected override void Initialize()
		{
			IsEntry = false;
			IsStop = true;
			IsTarget = false;
			Add(new Plot(Color.FromKnownColor(KnownColor.Black), "Occurred"));
			Add(new Plot(Color.FromArgb(255, 187, 128, 238), "Entry"));
			Overlay = true;
			CalculateOnBarClose = true;
		}

        protected override void OnBarUpdate()
        {
            //MyGap.Set(Input[0]);

           


        }




        public override string ToString()
        {
            return "ORB Stop";
        }

        public override string DisplayName
        {
            get
            {
                return "ORB Stop";
            }
        }

		#region Properties

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Occurred
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Entry
		{
			get { return Values[1]; }
		}

		public override IList<DataSeries> GetEntries()
		{
			return new[]{Entry};
		}

        //[Description("")]
        //[Category("Parameters")]
        //public int MyCondition1
        //{
        //    get { return _myCondition1; }
        //    set { _myCondition1 = Math.Max(1, value); }
        //}

  

		#endregion
	}
}

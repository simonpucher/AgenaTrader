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
/// Version: 1.4
/// -------------------------------------------------------------------------
/// Simon Pucher 2017
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("Volatility Indicator by Robert Levy.")]
	public class Volatility_Levy_Stand_Dev_Mean_Average_Indicator : UserIndicator
	{
        private int _period = 27;


        protected override void OnInit()
		{
			Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Orange), "Plot_Volatility_Levy_"+this.Period));
		}

		protected override void OnCalculate()
		{

			MyPlot1.Set(StdDev(this.Period)[0] / SMA(this.Period)[0]);
		}

        public override string ToString()
        {
            return "Volatility Levy (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "Volatility Levy (I)";
            }
        }


        #region Properties

        [Browsable(false)]
		[XmlIgnore()]
		public DataSeries MyPlot1
		{
			get { return Outputs[0]; }
		}

        /// <summary>
        /// </summary>
        [Description("Number of historical bars.")]
        [InputParameter]
        [DisplayName("Period")]
        public int Period
        {
            get { return _period; }
            set { _period = value; }
        }

        #endregion
    }
}
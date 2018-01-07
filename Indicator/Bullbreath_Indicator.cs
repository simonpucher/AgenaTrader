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
/// Version: 1.1.0
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
	[Description("Count all green candles in the last x candles.")]
	public class Bullbreath_Indicator : UserIndicator
	{

        private int _period = 10;

        protected override void OnInit()
		{
			Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Orange), "Bullbreath_Plot"));
			CalculateOnClosedBar = true;
        }

		protected override void OnCalculate()
		{
            if (this.Period > this.RequiredBarsCount)
            {
                AddChartTextFixed("AlertText", "Required bars must be at least as high as the period.", TextPosition.Center, Color.Red, new Font("Arial", 30), Color.Red, Color.Red, 20);
            }

            int myres = 0;
            for (int i = 0; i < Period; i++)
            {
                if (Close[i] > Open[i])
                {
                    myres = myres + 1;
                }
            }
            //MyPlot1.Set(Bars.Reverse().Take(10).Where(x => x.IsGrowing).Count());
            MyPlot1.Set(myres * 10);

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
        [Description("Select the period for the bullbreath count.")]
        [Category("Parameters")]
        [DisplayName("Period")]
        public int Period
        {
            get { return _period; }
            set
            {
                if (value < 1) value = 1;
                _period = value;
            }
        }


        public override string ToString()
        {
            return GetNameOnchart();
        }

        public override string DisplayName
        {
            get
            {
                return GetNameOnchart();
            }
        }


        private string GetNameOnchart()
        {
            return "Bullbreath (I)";
        }

        #endregion
    }
}
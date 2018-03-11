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
/// Simon Pucher 2018
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("Market Meanness Index Indicator")]
	public class Market_Meanness_Index_Indicator : UserIndicator
	{

        bool shortsignalbb = false;
        bool longsignalbb = false;

        private int _period = 20;

        private Color _color_long_signal = Const.DefaultArrowLongColor;
        private Color _color_short_signal = Const.DefaultArrowShortColor;

        protected override void OnInit()
		{
			AddOutput(new OutputDescriptor(Color.FromKnownColor(KnownColor.Orange), "Plot_Market_Meanness_Index_Indicator"));
			CalculateOnClosedBar = true;
		}

		protected override void OnCalculate()
		{


            //double m = Median(Data, TimePeriod);
            //int i, nh = 0, nl = 0;
            //for (i = 1; i < TimePeriod; i++)
            //{
            //    if (Data[i] > m && Data[i] > Data[i - 1])
            //        nl++;
            //    else if (Data[i] < m && Data[i] < Data[i - 1])
            //        nh++;
            //}
            //return 100.* (nl + nh) / (TimePeriod - 1);

            double m = Median[this.MMI_Period];
            int i, nh = 0, nl = 0;
            for (i = 1; i < this.MMI_Period; i++)
            {
                if (this.InSeries[i] > m && this.InSeries[i] > this.InSeries[i - 1])
                    nl++;
                else if (this.InSeries[i] < m && this.InSeries[i] < this.InSeries[i - 1])
                    nh++;
            }
            double resulti = 100.0 * (nl + nh) / (this.MMI_Period - 1);

            MyPlot1.Set(resulti);
            

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
        [Description("Period.")]
        [Category("Parameters")]
        [DisplayName("Period")]
        public int MMI_Period
        {
            get { return _period; }
            set
            {
                _period = value;
            }
        }

       
       

        /// <summary>
        /// </summary>
        [Description("Select Color for the long signal.")]
        [Category("Color")]
        [DisplayName("Signal Long")]
        public Color ColorLongSignal
        {
            get { return _color_long_signal; }
            set { _color_long_signal = value; }
        }


        // Serialize Color object
        [Browsable(false)]
        public string ColorLongSignalSerialize
        {
            get { return SerializableColor.ToString(_color_long_signal); }
            set { _color_long_signal = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Select Color for the long signal.")]
        [Category("Color")]
        [DisplayName("Signal Long")]
        public Color ColorShortSignal
        {
            get { return _color_short_signal; }
            set { _color_short_signal = value; }
        }


        // Serialize Color object
        [Browsable(false)]
        public string ColorShortSignalSerialize
        {
            get { return SerializableColor.ToString(_color_short_signal); }
            set { _color_short_signal = SerializableColor.FromString(value); }
        }

        #endregion
    }
}
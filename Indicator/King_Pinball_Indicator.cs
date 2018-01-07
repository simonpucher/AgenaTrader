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
/// King Pinball by Traderfox: https://youtu.be/bwFGeUVmF5o
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("King Pinball")]
	public class King_Pinball_Indicator : UserIndicator
	{

        bool shortsignalbb = false;
        bool longsignalbb = false;

        private int _bollinger_period = 20;
        private double _bollinger_stddev = 2;

        private int _macd_fast = 12;
        private int _macd_slow = 26;
        private int _macd_smooth = 9;

        private Color _color_long_signal = Const.DefaultArrowLongColor;
        private Color _color_short_signal = Const.DefaultArrowShortColor;

        protected override void OnInit()
		{
			AddOutput(new OutputDescriptor(Color.FromKnownColor(KnownColor.Orange), "Plot_Signal_King_Pinball"));
			CalculateOnClosedBar = true;
		}

		protected override void OnCalculate()
		{
            Bollinger bol = Bollinger(this.Bollinger_stddev, this.Bollinger_Period);
            if (Close[0] < bol.Lower[0])
            {
                longsignalbb = true;
            }
            else if (Close[0] > bol.Upper[0])
            {
                shortsignalbb = true;
            }
            else
            {
                //nothing
            }

            MACD macd = MACD(this.MACD_Fast, this.MACD_Slow, this.MACD_Smooth);
            if (longsignalbb && CrossAbove(macd.Default, macd.Avg, 0))
            {
                AddChartArrowUp(Time[0].ToString()+"long", 0, Low[0], this.ColorLongSignal);
                MyPlot1.Set(1);
                longsignalbb = false;
            }
            else if (shortsignalbb && CrossBelow(macd.Default, macd.Avg, 0))
            {
                AddChartArrowDown(Time[0].ToString()+"short", 0, High[0], this.ColorShortSignal);
                MyPlot1.Set(-1);
                shortsignalbb = false;
            }
            else
            {
                MyPlot1.Set(0);
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
        [Description("Bollinger Band period.")]
        [Category("Parameters")]
        [DisplayName("BB period")]
        public int Bollinger_Period
        {
            get { return _bollinger_period; }
            set
            {
                _bollinger_period = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("Bollinger Band standard deviation")]
        [Category("Parameters")]
        [DisplayName("BB stddev")]
        public double Bollinger_stddev
        {
            get { return _bollinger_stddev; }
            set
            {
                _bollinger_stddev = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("Bollinger Band fast")]
        [Category("Parameters")]
        [DisplayName("MACD fast")]
        public int MACD_Fast
        {
            get { return _macd_fast; }
            set
            {
                _macd_fast = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("Bollinger Band slow")]
        [Category("Parameters")]
        [DisplayName("MACD slow")]
        public int MACD_Slow
        {
            get { return _macd_slow; }
            set
            {
                _macd_slow = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("Bollinger Band smooth")]
        [Category("Parameters")]
        [DisplayName("MACD smooth")]
        public int MACD_Smooth
        {
            get { return _macd_smooth; }
            set
            {
                _macd_smooth = value;
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
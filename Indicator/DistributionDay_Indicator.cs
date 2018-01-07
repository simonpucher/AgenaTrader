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
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace AgenaTrader.UserCode
{
    /// <summary>
    /// Version: 1.2.0
    /// -------------------------------------------------------------------------
    /// Simon Pucher 2016
    /// -------------------------------------------------------------------------
    /// http://www.optionstradingiq.com/distribution-days-can-foreshadow-a-correction/
    /// http://ratingstocks.com/index-distribution
    /// -------------------------------------------------------------------------
    /// ****** Important ******
    /// To compile this script without any error you also need access to the utility indicator to use these global source code elements.
    /// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
    /// -------------------------------------------------------------------------
    /// Namespace holds all indicators and is required. Do not change it.
    /// </summary>
    /// 
    [Description("Take your money and run when smart money start the distribution.")]
	public class DistributionDay_Indicator : UserIndicator
	{
        public enum Enum_Volume_Calucation
        {
            VolumeIsGreaterThanYesterday = 0,
            VolumeisGreaterThantheEMAOfTheLastXCandles = 1
        }

        private Queue<DateTime> _distributionlist = null;
        private int _period = 25;
        private double _percent = 0.2;
        private bool _showdistributiondayarrows = true;
        private bool _showdistributiondaylabel = true;
        private int _distributiondaycount = 4;

        private double _volumepercent = 100.0;

        private Enum_Volume_Calucation _volume_calculation = Enum_Volume_Calucation.VolumeisGreaterThantheEMAOfTheLastXCandles;

        private int _ema_period = 30;

        private Color _color_long_signal_distribution = Color.DarkViolet;
        private Color _color_long_signal_distribution_strong = Color.Violet;

        protected override void OnInit()
		{
			CalculateOnClosedBar = false;
            this.IsOverlay = true;
		}

        protected override void OnCalculate()
        {

            //Init list on startup
            if (ProcessingBarIndex == 0)
            {
                _distributionlist = new Queue<DateTime>();
            }

            //Delete all old 
            if (this._distributionlist.Count() > 0 && this._distributionlist.Peek() <= Time[0].AddDays(this.Period * (-1)))
            {
                this._distributionlist.Dequeue();
            }

            bool volumespike = false;
            //Volume Calculation
            switch (this.Volume_Calculation)
            {
                case Enum_Volume_Calucation.VolumeIsGreaterThanYesterday:
                    if (Volume[0] > (Volume[1] * (this.VolumePercent/100.0)) ) volumespike = true;
                    break;
                case Enum_Volume_Calucation.VolumeisGreaterThantheEMAOfTheLastXCandles:
                    if (Volume[0] > EMA(Volume, this.EMA_Period)[0]) volumespike = true;
                    break;
            }

            //Draw Distribution Arrow.
            if (volumespike && ((Close[1] - Close[0]) / Close[1]) > (this.Percent / 100.0))
            {
                this._distributionlist.Enqueue(Time[0]);

                //Draw the indicator
                if (ShowDistributionDayArrows)
                {
                    Color color = Color.Black;
                    if (this._distributionlist.Count > this.DistributionDayCount)
                    {
                        color = ColorLongSignalDistribution;
                        AddChartArrowDown(ProcessingBarIndex.ToString(), true, 0, High[0], ColorLongSignalDistribution);
                    }
                    else
                    {
                        color = ColorLongSignalDistributionStrong;
                        AddChartArrowDown(ProcessingBarIndex.ToString(), true, 0, High[0], ColorLongSignalDistributionStrong);
                    }

                    if (this.ShowDistributionDayLabel)
                    {
                        AddChartText("dday" + Time[0], true, "DD", Time[0], Low[0], 0, color, new Font("Arial", 8, FontStyle.Bold), StringAlignment.Far, HorizontalAlignment.Center, VerticalAlignment.Top, color, Color.White, 255);
                    }
                }
                
            }
		}

		#region Properties


        /// <summary>
        /// </summary>
        [Description("Period which will be used to count distribution days.")]
        [Category("Parameters")]
        [DisplayName("Period")]
        public int Period
        {
            get { return _period; }
            set { _period = value; }
        }

        [Description("Period which will be used to count distribution days.")]
        [Category("Parameters")]
        [DisplayName("Distribution Day Count")]
        public int DistributionDayCount
        {
            get { return _distributiondaycount; }
            set { _distributiondaycount = value; }
        }

        [Description("Percent down to count as a distribution day.")]
        [Category("Parameters")]
        [DisplayName("Percent")]
        public double Percent
        {
            get { return _percent; }
            set { _percent = value; }
        }

        
        [Description("Show all distribution day arrows.")]
        [Category("Parameters")]
        [DisplayName("Show all arrows")]
        public bool ShowDistributionDayArrows
        {
            get { return _showdistributiondayarrows; }
            set { _showdistributiondayarrows = value; }
        }

        [Description("Show all distribution day labels.")]
        [Category("Parameters")]
        [DisplayName("Show all labels")]
        public bool ShowDistributionDayLabel
        {
            get { return _showdistributiondaylabel; }
            set { _showdistributiondaylabel = value; }
        }

        /// <summary>
        /// </summary>
        [Description("Select Color for the distribution day signal.")]
        [Category("Color")]
        [DisplayName("Color Distribution Day")]
        public Color ColorLongSignalDistribution
        {
            get { return _color_long_signal_distribution; }
            set { _color_long_signal_distribution = value; }
        }


        // Serialize Color object
        [Browsable(false)]
        public string ColorLongSignalDistributionSerialize
        {
            get { return SerializableColor.ToString(_color_long_signal_distribution); }
            set { _color_long_signal_distribution = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Select Color for the distribution day signal.")]
        [Category("Color")]
        [DisplayName("Color Distribution Day")]
        public Color ColorLongSignalDistributionStrong
        {
            get { return _color_long_signal_distribution_strong; }
            set { _color_long_signal_distribution_strong = value; }
        }
        // Serialize Color object
        [Browsable(false)]
        public string ColorLongSignalDistributionStrongSerialize
        {
            get { return SerializableColor.ToString(_color_long_signal_distribution_strong); }
            set { _color_long_signal_distribution_strong = SerializableColor.FromString(value); }

        }


        [Description("Percent of yesterday volume.")]
        [Category("Parameters")]
        [DisplayName("Volume Percent")]
        public double VolumePercent
        {
            get { return _volumepercent; }
            set { _volumepercent = value; }
        }


        [Description("Select the type of volume calculation.")]
        [Category("Volume")]
        [DisplayName("Volume Calculation")]
        public Enum_Volume_Calucation Volume_Calculation
        {
            get { return _volume_calculation; }
            set { _volume_calculation = value; }
        }

        [Description("Select the period of the EMA volume calculation.")]
        [Category("Volume")]
        [DisplayName("Volume Calculation Period")]
        public int EMA_Period
        {
            get { return _ema_period; }
            set { _ema_period = value; }
        }

        

        #endregion
    }
}
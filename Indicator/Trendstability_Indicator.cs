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
/// Version: 1.2
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
	[Description("Shows the stability of the trend.")]
	public class Trendstability_Indicator : UserIndicator
	{

        private MAEnvelopesMAType _MA_1_Selected = MAEnvelopesMAType.SMA;
        private int _ma_1 = 100;

        private double GetValue(MAEnvelopesMAType matype, int period, int position)
        {
            switch (matype)
            {
                case MAEnvelopesMAType.SMA:
                    return SMA(period)[position];
                case MAEnvelopesMAType.EMA:
                    return EMA(period)[position];
                case MAEnvelopesMAType.WMA:
                    return WMA(period)[position];
                case MAEnvelopesMAType.HMA:
                    return HMA(period)[position];
                case MAEnvelopesMAType.TEMA:
                    return TEMA(period)[position];
                case MAEnvelopesMAType.TMA:
                    return TMA(period)[position];
                default:
                    throw new NotImplementedException();
            }
        }


        protected override void OnInit()
		{
			Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
            Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Gray), "MyPlot2"));
        }

		protected override void OnCalculate()
		{
            if (this.MA_1 != 0 && this.MA_1 > this.RequiredBarsCount)
            {
                AddChartTextFixed("AlertText", "Required bars must be at least as high as the moving average period.", TextPosition.Center, Color.Red, new Font("Arial", 30), Color.Red, Color.Red, 20);
            }
            
            double resulti = (this.GetValue(this.MA_1_Selected, this.MA_1, 0) - this.GetValue(this.MA_1_Selected, this.MA_1, 1)) / StdDev(this.MA_1)[0] * 100;

			MyPlot1.Set(resulti);
            MyPlot2.Set(0);
        }

		#region Properties

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries MyPlot1
		{
			get { return Outputs[0]; }
		}

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries MyPlot2
        {
            get { return Outputs[1]; }
        }


        /// <summary>
        /// </summary>
        [Description("Select the type of MA you would like to use")]
        [InputParameter]
        [DisplayName("Type of MA")]
        public MAEnvelopesMAType MA_1_Selected
        {
            get { return _MA_1_Selected; }
            set
            {
                _MA_1_Selected = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("Period for the MA")]
        [InputParameter]
        [DisplayName("Period MA")]
        public int MA_1
        {
            get { return _ma_1; }
            set
            {
                _ma_1 = value;
            }
        }

        #endregion
    }
}
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
			Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Gray), "MyPlot2"));
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
        [Category("Parameters")]
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
        [Category("Parameters")]
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
#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator
	{
		/// <summary>
		/// Shows the stability of the trend.
		/// </summary>
		public Trendstability_Indicator Trendstability_Indicator(MAEnvelopesMAType mA_1_Selected, System.Int32 mA_1)
        {
			return Trendstability_Indicator(InSeries, mA_1_Selected, mA_1);
		}

		/// <summary>
		/// Shows the stability of the trend.
		/// </summary>
		public Trendstability_Indicator Trendstability_Indicator(IDataSeries input, MAEnvelopesMAType mA_1_Selected, System.Int32 mA_1)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Trendstability_Indicator>(input, i => i.MA_1_Selected == mA_1_Selected && i.MA_1 == mA_1);

			if (indicator != null)
				return indicator;

			indicator = new Trendstability_Indicator
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input,
							MA_1_Selected = mA_1_Selected,
							MA_1 = mA_1
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
		/// Shows the stability of the trend.
		/// </summary>
		public Trendstability_Indicator Trendstability_Indicator(MAEnvelopesMAType mA_1_Selected, System.Int32 mA_1)
		{
			return LeadIndicator.Trendstability_Indicator(InSeries, mA_1_Selected, mA_1);
		}

		/// <summary>
		/// Shows the stability of the trend.
		/// </summary>
		public Trendstability_Indicator Trendstability_Indicator(IDataSeries input, MAEnvelopesMAType mA_1_Selected, System.Int32 mA_1)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.Trendstability_Indicator(input, mA_1_Selected, mA_1);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Shows the stability of the trend.
		/// </summary>
		public Trendstability_Indicator Trendstability_Indicator(MAEnvelopesMAType mA_1_Selected, System.Int32 mA_1)
		{
			return LeadIndicator.Trendstability_Indicator(InSeries, mA_1_Selected, mA_1);
		}

		/// <summary>
		/// Shows the stability of the trend.
		/// </summary>
		public Trendstability_Indicator Trendstability_Indicator(IDataSeries input, MAEnvelopesMAType mA_1_Selected, System.Int32 mA_1)
		{
			return LeadIndicator.Trendstability_Indicator(input, mA_1_Selected, mA_1);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Shows the stability of the trend.
		/// </summary>
		public Trendstability_Indicator Trendstability_Indicator(MAEnvelopesMAType mA_1_Selected, System.Int32 mA_1)
		{
			return LeadIndicator.Trendstability_Indicator(InSeries, mA_1_Selected, mA_1);
		}

		/// <summary>
		/// Shows the stability of the trend.
		/// </summary>
		public Trendstability_Indicator Trendstability_Indicator(IDataSeries input, MAEnvelopesMAType mA_1_Selected, System.Int32 mA_1)
		{
			return LeadIndicator.Trendstability_Indicator(input, mA_1_Selected, mA_1);
		}
	}

	#endregion

}

#endregion

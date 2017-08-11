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
/// Version: 1.2.1
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this indicator without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    /// <summary>
    /// Selectable MAs for this indicator.
    /// </summary>
    public enum Enum_Distance_Indicator_MA
    {
        SMA = 0,
        EMA = 1
    }

    /// <summary>
    /// Selectable the type of signal for this indicator.
    /// </summary>
    public enum Enum_Type_of_Signal
    {
        ShowPercent = 0,
        ShowDiscreteSignal = 1
    }

    [Description("Distance in percent to a mean average indicator.")]
	public class Distance_Indicator : UserIndicator
	{
        private int _Period = 200;
        private Enum_Distance_Indicator_MA _MA_1_Selected = Enum_Distance_Indicator_MA.SMA;
        private Enum_Type_of_Signal _TypeOfSignal = Enum_Type_of_Signal.ShowPercent;
        private double _PercentThreshold = 3.0;

        protected override void OnInit()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
            //Add(new Plot(Color.FromKnownColor(KnownColor.Gray), "MyPlot2"));
            IsOverlay = false;
			CalculateOnClosedBar = true;
		}

		protected override void OnCalculate()
		{
            if (this.TypeOfSignal == Enum_Type_of_Signal.ShowPercent)
            {
                switch (this.MA_1_Selected)
                {
                    case Enum_Distance_Indicator_MA.SMA:
                        SMA _sma = SMA(this.Period);
                        double result_s = (InSeries[0] / (_sma[0] / 100)) - 100;
                        MyPlot1.Set(result_s);
                        //MyPlot2.Set(0);
                        break;
                    case Enum_Distance_Indicator_MA.EMA:
                        EMA _ema = EMA(this.Period);
                        double result_e = (InSeries[0] / (_ema[0] / 100)) - 100;
                        MyPlot1.Set(result_e);
                        //MyPlot2.Set(0);
                        break;
                    default:
                        break;
                }
            }
            else if (this.TypeOfSignal == Enum_Type_of_Signal.ShowDiscreteSignal)
            {
                switch (this.MA_1_Selected)
                {
                    case Enum_Distance_Indicator_MA.SMA:
                        SMA _sma = SMA(this.Period);
                        double result_s = (InSeries[0] / (_sma[0] / 100)) - 100;
                        if (result_s < this.PercentThreshold && result_s >= 0)
                        {
                            MyPlot1.Set(1);
                        }
                        else if (result_s > (this.PercentThreshold * (-1)) && result_s < 0)
                        {
                            MyPlot1.Set(-1);
                        }
                        else
                        {
                            MyPlot1.Set(0);
                        }
                        //MyPlot2.Set(0);
                        break;
                    case Enum_Distance_Indicator_MA.EMA:
                        EMA _ema = EMA(this.Period);
                        double result_e = (InSeries[0] / (_ema[0] / 100)) - 100;
                        if (result_e < this.PercentThreshold && result_e >= 0)
                        {
                            MyPlot1.Set(1);
                        }
                        else if (result_e > (this.PercentThreshold * (-1)) && result_e < 0)
                        {
                            MyPlot1.Set(-1);
                        }
                        else
                        {
                            MyPlot1.Set(0);
                        }
                        //MyPlot2.Set(0);
                        break;
                    default:
                        break;
                }
            }
 
        }

        #region Properties

        [Browsable(false)]
		[XmlIgnore()]
		public DataSeries MyPlot1
		{
			get { return Outputs[0]; }
		}

        //[Browsable(false)]
        //[XmlIgnore()]
        //public DataSeries MyPlot2
        //{
        //    get { return Outputs[1]; }
        //}


        [Description("Period of the mean average.")]
        [Category("Parameters")]
        [DisplayName("Period")]
        public int Period
        {
            get { return _Period; }
            set { _Period = value; }
        }

        /// <summary>
        /// </summary>
        [Description("Select the type of MA you would like to use")]
        [Category("Parameters")]
        [DisplayName("Type of MA")]
        public Enum_Distance_Indicator_MA MA_1_Selected
        {
            get { return _MA_1_Selected; }
            set
            {
                _MA_1_Selected = value;
            }
        }


        [Description("Period of the mean average.")]
        [Category("Parameters")]
        [DisplayName("Period")]
        public Enum_Type_of_Signal TypeOfSignal
        {
            get { return _TypeOfSignal; }
            set { _TypeOfSignal = value; }
        }

        [Description("Threshold in percent to show a discreet signal.")]
        [Category("Parameters")]
        [DisplayName("Percent Threshold")]
        public double PercentThreshold
        {
            get { return _PercentThreshold; }
            set { _PercentThreshold = value; }
        }

        #endregion
    }
}
#region AgenaTrader Automaticaly Generated Code. Do not change it manually

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator
	{
		/// <summary>
		/// Distance in percent to a mean average indicator.
		/// </summary>
		public Distance_Indicator Distance_Indicator(System.Int32 period, Enum_Distance_Indicator_MA mA_1_Selected, Enum_Type_of_Signal typeOfSignal, System.Double percentThreshold)
        {
			return Distance_Indicator(InSeries, period, mA_1_Selected, typeOfSignal, percentThreshold);
		}

		/// <summary>
		/// Distance in percent to a mean average indicator.
		/// </summary>
		public Distance_Indicator Distance_Indicator(IDataSeries input, System.Int32 period, Enum_Distance_Indicator_MA mA_1_Selected, Enum_Type_of_Signal typeOfSignal, System.Double percentThreshold)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Distance_Indicator>(input, i => i.Period == period && i.MA_1_Selected == mA_1_Selected && i.TypeOfSignal == typeOfSignal && Math.Abs(i.PercentThreshold - percentThreshold) <= Double.Epsilon);

			if (indicator != null)
				return indicator;

			indicator = new Distance_Indicator
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input,
							Period = period,
							MA_1_Selected = mA_1_Selected,
							TypeOfSignal = typeOfSignal,
							PercentThreshold = percentThreshold
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
		/// Distance in percent to a mean average indicator.
		/// </summary>
		public Distance_Indicator Distance_Indicator(System.Int32 period, Enum_Distance_Indicator_MA mA_1_Selected, Enum_Type_of_Signal typeOfSignal, System.Double percentThreshold)
		{
			return LeadIndicator.Distance_Indicator(InSeries, period, mA_1_Selected, typeOfSignal, percentThreshold);
		}

		/// <summary>
		/// Distance in percent to a mean average indicator.
		/// </summary>
		public Distance_Indicator Distance_Indicator(IDataSeries input, System.Int32 period, Enum_Distance_Indicator_MA mA_1_Selected, Enum_Type_of_Signal typeOfSignal, System.Double percentThreshold)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.Distance_Indicator(input, period, mA_1_Selected, typeOfSignal, percentThreshold);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Distance in percent to a mean average indicator.
		/// </summary>
		public Distance_Indicator Distance_Indicator(System.Int32 period, Enum_Distance_Indicator_MA mA_1_Selected, Enum_Type_of_Signal typeOfSignal, System.Double percentThreshold)
		{
			return LeadIndicator.Distance_Indicator(InSeries, period, mA_1_Selected, typeOfSignal, percentThreshold);
		}

		/// <summary>
		/// Distance in percent to a mean average indicator.
		/// </summary>
		public Distance_Indicator Distance_Indicator(IDataSeries input, System.Int32 period, Enum_Distance_Indicator_MA mA_1_Selected, Enum_Type_of_Signal typeOfSignal, System.Double percentThreshold)
		{
			return LeadIndicator.Distance_Indicator(input, period, mA_1_Selected, typeOfSignal, percentThreshold);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Distance in percent to a mean average indicator.
		/// </summary>
		public Distance_Indicator Distance_Indicator(System.Int32 period, Enum_Distance_Indicator_MA mA_1_Selected, Enum_Type_of_Signal typeOfSignal, System.Double percentThreshold)
		{
			return LeadIndicator.Distance_Indicator(InSeries, period, mA_1_Selected, typeOfSignal, percentThreshold);
		}

		/// <summary>
		/// Distance in percent to a mean average indicator.
		/// </summary>
		public Distance_Indicator Distance_Indicator(IDataSeries input, System.Int32 period, Enum_Distance_Indicator_MA mA_1_Selected, Enum_Type_of_Signal typeOfSignal, System.Double percentThreshold)
		{
			return LeadIndicator.Distance_Indicator(input, period, mA_1_Selected, typeOfSignal, percentThreshold);
		}
	}

	#endregion

}

#endregion

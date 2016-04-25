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
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    public enum IndicatorEnum
    {
        SMA = 1,
        EMA = 2
    }

	[Description("Compare the currenty value of an indicator to latest high value of the indicator.")]
	public class HighestHighValue_Indicator : UserIndicator
	{
        //input
        private int _indicatorPeriod = 200;
        private int _comparisonPeriod = 30;
        private IndicatorEnum _indicatorenum = IndicatorEnum.SMA;

        //internal
        private DataSeries _DATA_List;

		protected override void Initialize()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
		}

        /// <summary>
        /// Calculates the indicator value(s) at the current index.
        /// </summary>
        protected override void OnStartUp()
        {
            this._DATA_List = new DataSeries(this);
        }

		protected override void OnBarUpdate()
		{
            double currentvalue = 0.0;
            switch (_indicatorenum)
            {
                case IndicatorEnum.SMA:
                    currentvalue = SMA(IndicatorPeriod)[0];
                    break;
                case IndicatorEnum.EMA:
                    currentvalue = EMA(IndicatorPeriod)[0];
                    break;
                default:
                    break;
            }
            
            double lasthighvalue = _DATA_List.Reverse().Take(this.ComparisonPeriod).Max();

            if (lasthighvalue < currentvalue)
            {
                MyPlot1.Set(1);
            }
            else
            {
                MyPlot1.Set(0);
            }

            this._DATA_List.Set(currentvalue);
		}


        public override string ToString()
        {
            return "HHV";
        }

        public override string DisplayName
        {
            get
            {
                return "HHV";
            }
        }


		#region Properties

        #region Input

        [Description("Type of the indicator")]
        [Category("Values")]
        [DisplayName("Type of the indicator")]
        public IndicatorEnum Indicator
        {
            get { return _indicatorenum; }
            set { _indicatorenum = value; }
        }


            [Description("Period for the indicator")]
            [Category("Values")]
            [DisplayName("Period for indicator")]
            public int IndicatorPeriod
            {
                get { return _indicatorPeriod; }
                set { _indicatorPeriod = value; }
            }



            [Description("Period for comparison")]
            [Category("Values")]
            [DisplayName("Period for comparison")]
            public int ComparisonPeriod
            {
                get { return _comparisonPeriod; }
                set { _comparisonPeriod = value; }
            }

        #endregion


        #region Output

            [Browsable(false)]
            [XmlIgnore()]
            public DataSeries MyPlot1
            {
                get { return Values[0]; }
            }

        #endregion

        #endregion
    }
}
#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator : Indicator
	{
		/// <summary>
		/// Compare the currenty value of an indicator to latest high value of the indicator.
		/// </summary>
		public HighestHighValue_Indicator HighestHighValue_Indicator()
        {
			return HighestHighValue_Indicator(Input);
		}

		/// <summary>
		/// Compare the currenty value of an indicator to latest high value of the indicator.
		/// </summary>
		public HighestHighValue_Indicator HighestHighValue_Indicator(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<HighestHighValue_Indicator>(input);

			if (indicator != null)
				return indicator;

			indicator = new HighestHighValue_Indicator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input
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
		/// Compare the currenty value of an indicator to latest high value of the indicator.
		/// </summary>
		public HighestHighValue_Indicator HighestHighValue_Indicator()
		{
			return LeadIndicator.HighestHighValue_Indicator(Input);
		}

		/// <summary>
		/// Compare the currenty value of an indicator to latest high value of the indicator.
		/// </summary>
		public HighestHighValue_Indicator HighestHighValue_Indicator(IDataSeries input)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.HighestHighValue_Indicator(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Compare the currenty value of an indicator to latest high value of the indicator.
		/// </summary>
		public HighestHighValue_Indicator HighestHighValue_Indicator()
		{
			return LeadIndicator.HighestHighValue_Indicator(Input);
		}

		/// <summary>
		/// Compare the currenty value of an indicator to latest high value of the indicator.
		/// </summary>
		public HighestHighValue_Indicator HighestHighValue_Indicator(IDataSeries input)
		{
			return LeadIndicator.HighestHighValue_Indicator(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Compare the currenty value of an indicator to latest high value of the indicator.
		/// </summary>
		public HighestHighValue_Indicator HighestHighValue_Indicator()
		{
			return LeadIndicator.HighestHighValue_Indicator(Input);
		}

		/// <summary>
		/// Compare the currenty value of an indicator to latest high value of the indicator.
		/// </summary>
		public HighestHighValue_Indicator HighestHighValue_Indicator(IDataSeries input)
		{
			return LeadIndicator.HighestHighValue_Indicator(input);
		}
	}

	#endregion

}

#endregion

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

namespace AgenaTrader.UserCode
{
    [Description("Indicator which shows deep correction of ongoing trend")]
    public class DeepCorrectionTrend_Indikator : UserIndicator
    {
        const double MarketPhaseDeepCorrection = 5.3d;
        int _trendSize = 1;




        protected override void Initialize()
        {
            Add(new Plot(Color.FromArgb(255, 102, 255, 51), "DeepCorrection"));
            Overlay = false;
            CalculateOnBarClose = true;
            BarsRequired = 20;
        }

        protected override void OnBarUpdate()
        {

            if (Math.Abs(MarketPhasesAdv(_trendSize)[0]) == MarketPhaseDeepCorrection)
            {
                Value.Set(MarketPhasesAdv(_trendSize)[0]);
            }
            else
            {
                Value.Set(0);
            }



            if (Bars.Count - 1 == Count)
            {
                int i = 1;
            }


        }

        public override string ToString()
        {
            return "Deep Correction Trend (S)";
        }

        public override string DisplayName
        {
            get
            {
                return "Deep Correction Trend (S)";
            }
        }

        #region Properties

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries DeepCorrection
        {
            get { return Values[0]; }
        }


        /// <summary>
        /// </summary>
        [Description("Trendsize (0-3)")]
        [Category("Parameters")]
        [DisplayName("Trendsize (0-3)")]
        public int TrendSize
        {
            get { return _trendSize; }
            set { _trendSize = value; }
        }

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
		/// Indicator which shows deep correction of ongoing trend
		/// </summary>
		public DeepCorrectionTrend_Indikator DeepCorrectionTrend_Indikator(System.Int32 trendSize)
        {
			return DeepCorrectionTrend_Indikator(Input, trendSize);
		}

		/// <summary>
		/// Indicator which shows deep correction of ongoing trend
		/// </summary>
		public DeepCorrectionTrend_Indikator DeepCorrectionTrend_Indikator(IDataSeries input, System.Int32 trendSize)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<DeepCorrectionTrend_Indikator>(input, i => i.TrendSize == trendSize);

			if (indicator != null)
				return indicator;

			indicator = new DeepCorrectionTrend_Indikator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							TrendSize = trendSize
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
		/// Indicator which shows deep correction of ongoing trend
		/// </summary>
		public DeepCorrectionTrend_Indikator DeepCorrectionTrend_Indikator(System.Int32 trendSize)
		{
			return LeadIndicator.DeepCorrectionTrend_Indikator(Input, trendSize);
		}

		/// <summary>
		/// Indicator which shows deep correction of ongoing trend
		/// </summary>
		public DeepCorrectionTrend_Indikator DeepCorrectionTrend_Indikator(IDataSeries input, System.Int32 trendSize)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.DeepCorrectionTrend_Indikator(input, trendSize);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Indicator which shows deep correction of ongoing trend
		/// </summary>
		public DeepCorrectionTrend_Indikator DeepCorrectionTrend_Indikator(System.Int32 trendSize)
		{
			return LeadIndicator.DeepCorrectionTrend_Indikator(Input, trendSize);
		}

		/// <summary>
		/// Indicator which shows deep correction of ongoing trend
		/// </summary>
		public DeepCorrectionTrend_Indikator DeepCorrectionTrend_Indikator(IDataSeries input, System.Int32 trendSize)
		{
			return LeadIndicator.DeepCorrectionTrend_Indikator(input, trendSize);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Indicator which shows deep correction of ongoing trend
		/// </summary>
		public DeepCorrectionTrend_Indikator DeepCorrectionTrend_Indikator(System.Int32 trendSize)
		{
			return LeadIndicator.DeepCorrectionTrend_Indikator(Input, trendSize);
		}

		/// <summary>
		/// Indicator which shows deep correction of ongoing trend
		/// </summary>
		public DeepCorrectionTrend_Indikator DeepCorrectionTrend_Indikator(IDataSeries input, System.Int32 trendSize)
		{
			return LeadIndicator.DeepCorrectionTrend_Indikator(input, trendSize);
		}
	}

	#endregion

}

#endregion

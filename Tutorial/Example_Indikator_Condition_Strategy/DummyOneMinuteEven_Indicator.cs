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

  

	[Description("Liefert Signal wenn es eine gerade Minute ist")]
	public class DummyOneMinuteEven_Indicator : UserIndicator
	{


        private int _myvalue = 1;



		protected override void Initialize()
		{
            Add(new Plot(Color.Orange, "EvenMinutePlot"));
            BarsRequired = 1;
			CalculateOnBarClose = false;
		}

        protected override void OnBarUpdate()
        {
            if (Bars != null && Bars.Count > 0
                         && TimeFrame.Periodicity == DatafeedHistoryPeriodicity.Minute
                         && TimeFrame.PeriodicityValue == 1)
            { }
            else
            {
                return;
            }

            if (Bars.GetTime(CurrentBar).Minute % 2 == 0 ) //Es ist eine gerade Minute
            {
                Value.Set(100);
            }
            else
            {
                Value.Set(0);
            }
        }


        #region MyRegion

        [Description("Period for RSI")]
        [Category("Values")]
        [DisplayName("Period for RSI")]
            public int Myvalue
            {
                get { return _myvalue; }
                set { _myvalue = value; }
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
		/// Liefert Signal wenn es eine gerade Minute ist
		/// </summary>
		public DummyOneMinuteEven_Indicator DummyOneMinuteEven_Indicator()
        {
			return DummyOneMinuteEven_Indicator(Input);
		}

		/// <summary>
		/// Liefert Signal wenn es eine gerade Minute ist
		/// </summary>
		public DummyOneMinuteEven_Indicator DummyOneMinuteEven_Indicator(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<DummyOneMinuteEven_Indicator>(input);

			if (indicator != null)
				return indicator;

			indicator = new DummyOneMinuteEven_Indicator
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
		/// Liefert Signal wenn es eine gerade Minute ist
		/// </summary>
		public DummyOneMinuteEven_Indicator DummyOneMinuteEven_Indicator()
		{
			return LeadIndicator.DummyOneMinuteEven_Indicator(Input);
		}

		/// <summary>
		/// Liefert Signal wenn es eine gerade Minute ist
		/// </summary>
		public DummyOneMinuteEven_Indicator DummyOneMinuteEven_Indicator(IDataSeries input)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.DummyOneMinuteEven_Indicator(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Liefert Signal wenn es eine gerade Minute ist
		/// </summary>
		public DummyOneMinuteEven_Indicator DummyOneMinuteEven_Indicator()
		{
			return LeadIndicator.DummyOneMinuteEven_Indicator(Input);
		}

		/// <summary>
		/// Liefert Signal wenn es eine gerade Minute ist
		/// </summary>
		public DummyOneMinuteEven_Indicator DummyOneMinuteEven_Indicator(IDataSeries input)
		{
			return LeadIndicator.DummyOneMinuteEven_Indicator(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Liefert Signal wenn es eine gerade Minute ist
		/// </summary>
		public DummyOneMinuteEven_Indicator DummyOneMinuteEven_Indicator()
		{
			return LeadIndicator.DummyOneMinuteEven_Indicator(Input);
		}

		/// <summary>
		/// Liefert Signal wenn es eine gerade Minute ist
		/// </summary>
		public DummyOneMinuteEven_Indicator DummyOneMinuteEven_Indicator(IDataSeries input)
		{
			return LeadIndicator.DummyOneMinuteEven_Indicator(input);
		}
	}

	#endregion

}

#endregion

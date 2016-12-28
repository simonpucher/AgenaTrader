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
    [Description("DoubleBottom")]
    public class DoubleBottom : UserIndicator
    {

        double _tolerancePercentage = 0.6;


        protected override void OnInit()
        {
            Add(new Plot(Color.Red, "DoubleBottom_DS"));
            IsOverlay = false;
            CalculateOnClosedBar = true;

            //Inhalt des OutputWindow l�schen
            ClearOutputWindow();
        }

        protected override void OnCalculate()
        {

            if (ProcessingBarIndex == Bars.Count - 1)
            {
                Print("asdf");
            }

            if (ProcessingBarIndex == 1978)
            {
                Print("asdf");
            }

            DoubleBottom_DS.Set(0);

            DateTime OneMonthAgo = Time[0].AddMonths(-1);
            double tolerance = Bars[0].Low * (TolerancePercentage / 100);

            Print("   {0}, {1}, {2}",
          Bars[0].Time.ToString(), Math.Round(Bars[0].Low + tolerance, 2), Math.Round(Bars[0].Low - tolerance), 2);


            //find previous buttom
            //Select all data and find high & low.
            IEnumerable<IBar> lastBottoms = Bars.Where(x => x.Time <= OneMonthAgo)                  //older than 1 Month
                                                 .Where(y => y.Low <= (Bars[0].Low + tolerance)     // Close <= current Close + Tolerance
                                                         && y.Low >= (Bars[0].Low - tolerance)      // Close >= current Close + Tolerance    
                                                         );
            foreach (IBar bar in lastBottoms)
            {
                double LowestLow = LowestLowPrice(Bars.GetBarsAgo(bar.Time))[0]; //search for the lowest low 
                double LowestLowBefore = LowestLowPrice(Bars.GetBarsAgo(bar.Time) + 20)[0]; //search for the lowest low before the potential bottom. this is to make sure that there is no lower price leading up to the bottom

                if (LowestLow <= (Bars[0].Low + tolerance)                  //check if that lowest low is inside tolerance levels   
                 && LowestLow >= (Bars[0].Low - tolerance)
                 && LowestLowBefore <= (Bars[0].Low + tolerance)
                 && LowestLowBefore >= (Bars[0].Low - tolerance)
                    )
                {
                    Print("DoubleBottom   {0}, {1}",
                          bar.Close, bar.Time.ToString());

                    string strdoubleBottomConnecter = "DoubleBottomConnecter_" + Bars[0].Time.ToString() + "_" + bar.Time.ToString();
                    AddChartLine(strdoubleBottomConnecter, Bars.GetBarsAgo(bar.Time), bar.Low, 0, Bars[0].Low, Color.Red);


                    double BreakThrough = HighestHighPrice(Bars.GetBarsAgo(bar.Time))[0];
                    double BreakThroughAgo = HighestHighIndex(Bars.GetBarsAgo(bar.Time))[0];

                    string strBreakThrough = strdoubleBottomConnecter + "BreakThrough";
                    string strBreakThroughVert = strdoubleBottomConnecter + "BreakThroughVert";
                    AddChartHorizontalLine(strBreakThrough, BreakThrough, Color.Green);
                    AddChartLine(strBreakThroughVert, (int)BreakThroughAgo, bar.Low, (int)BreakThroughAgo, BreakThrough, Color.Aquamarine);

                    DoubleBottom_DS.Set(1);
                    DoubleBottom_DS.Set(Bars.GetBarsAgo(bar.Time), 0.5);
                }
            }




        }

        #region Properties

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries DoubleBottom_DS
        {
            get { return Outputs[0]; }
        }

        public double TolerancePercentage
        {
            get
            {
                return _tolerancePercentage;
            }

            set
            {
                _tolerancePercentage = value;
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
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom()
        {
			return DoubleBottom(InSeries);
		}

		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<DoubleBottom>(input);

			if (indicator != null)
				return indicator;

			indicator = new DoubleBottom
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input
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
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom()
		{
			return LeadIndicator.DoubleBottom(InSeries);
		}

		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom(IDataSeries input)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.DoubleBottom(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom()
		{
			return LeadIndicator.DoubleBottom(InSeries);
		}

		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom(IDataSeries input)
		{
			return LeadIndicator.DoubleBottom(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom()
		{
			return LeadIndicator.DoubleBottom(InSeries);
		}

		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom(IDataSeries input)
		{
			return LeadIndicator.DoubleBottom(input);
		}
	}

	#endregion

}

#endregion

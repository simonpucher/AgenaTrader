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

        public struct HighLow
        {
            public string HighOrLow;
            public DateTime DateTime;
            public double Close;

            public HighLow(string highorlow, DateTime datetime, double close)
            {
                HighOrLow = highorlow;
                DateTime = datetime;
                Close = close;
            }
        }

        private double lastHigh = Double.MinValue;
        private double lastLow = Double.MaxValue;

        private Boolean SetSuccessFromEcho = false;

        public List<HighLow> HighLowList;

        private double _tolerancePercentage = 0.6;
        private int _candles = 8;


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
            if (SetSuccessFromEcho)
            {
                DoubleBottom_DS.Set(1);
            }
            else
            {
                DoubleBottom_DS.Set(0);
            }
            

            //if (!IsProcessingBarIndexLast)
            //{
            //    return;
            //    Print("LastBar-Index");
            //}
                

            //if (Bars[1]==null)
            //{
            //    HighLowList = new List<HighLow>();
            //    return;
            //}

            //if (ProcessingBarIndex == Bars.Count - 1)
            //{
            //    Print("LastBar");
            //}


            //////////////NEW BUT NOT WORKING BEGIN//////////////
            //if (Close[0] > lastHigh)
            //{
            //    //higher
            //    HighLowList.Add(new HighLow("High", Bars[0].Time, Bars[0].Close));

            //    //reset LastLow
            //    lastLow = Double.MaxValue;
            //}
            //else if (Close[0] < lastLow)
            //{
            //    //lower
            //    HighLowList.Add(new HighLow("Low", Bars[0].Time, Bars[0].Close));

            //    //reset LastHigh
            //    lastHigh = Double.MinValue;
            //}
            //////////////NEW BUT NOT WORKING END//////////////


            //////////////////OLD BUT WORKING BEGIN//////////////////



            if (ProcessingBarIndex >= ( Bars.Count - this.Candles) - 1)
            {
                Print("LastBar");
            }
            else
            {
                return;  //Just the Last Bar plus the Echo-Bars
            }

            if (ProcessingBarIndex == 269)
            {
                Print("asdf");
            }

            //DoubleBottom_DS.Set(0);

            DateTime OneMonthAgo = Time[0].AddMonths(-1);
            double tolerance = Bars[0].Low * (TolerancePercentage / 100);

            Print("   {0}, {1}, {2}",
          Bars[0].Time.ToString(), Math.Round(Bars[0].Low + tolerance, 2), Math.Round(Bars[0].Low - tolerance), 2);


            //find previous buttom
            //Select all data and find high & low.
            IEnumerable<IBar> lastBottoms = Bars.Where(x => x.Time <= OneMonthAgo)                  //older than 1 Month
                                                 .Where(y => y.Low <= (Bars[0].Low + tolerance)     // Close <= current Close + Tolerance
                                                         && y.Low >= (Bars[0].Low - tolerance)      // Close >= current Close + Tolerance    
                                                         )
                                                   //      .OrderByDescending(x => x.Time)
                                                         ;



            foreach (IBar bar in lastBottoms)
            {
                double LowestLow = LowestLowPrice(Bars.GetBarsAgo(bar.Time))[0]; //search for the lowest low 
                double LowestLowBefore = LowestLowPrice(Bars.GetBarsAgo(bar.Time) + 20)[0]; //search for the lowest low before the potential bottom. this is to make sure that there is no lower price leading up to the bottom

                if (LowestLow <= (Bars[0].Low + tolerance)                  //check if that lowest low is inside tolerance levels   
                 && LowestLow >= (Bars[0].Low - tolerance)
                 && LowestLowBefore <= (Bars[0].Low + tolerance)
                 && LowestLowBefore >= (Bars[0].Low - tolerance)
//                 && LowestLow < LowestLowBefore
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
                    SetSuccessFromEcho = true;
//                    break; only take the first found bottom
                }
            }
            //////////////////OLD BUT WORKING END//////////////////



        }

        #region Properties

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries DoubleBottom_DS
        {
            get { return Outputs[0]; }
        }

        [Description("Tolerance level in percent.")]
        [Category("Parameters")]
        [DisplayName("Tolerance")]
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


        [Description("The script shows a signal if the double bottom was reached within the last x candles.")]
        [Category("Parameters")]
        [DisplayName("Candles")]
        public int Candles
        {
            get { return _candles; }
            set { _candles = value; }
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
		public DoubleBottom DoubleBottom(System.Double tolerancePercentage, System.Int32 candles)
        {
			return DoubleBottom(InSeries, tolerancePercentage, candles);
		}

		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom(IDataSeries input, System.Double tolerancePercentage, System.Int32 candles)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<DoubleBottom>(input, i => Math.Abs(i.TolerancePercentage - tolerancePercentage) <= Double.Epsilon && i.Candles == candles);

			if (indicator != null)
				return indicator;

			indicator = new DoubleBottom
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input,
							TolerancePercentage = tolerancePercentage,
							Candles = candles
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
		public DoubleBottom DoubleBottom(System.Double tolerancePercentage, System.Int32 candles)
		{
			return LeadIndicator.DoubleBottom(InSeries, tolerancePercentage, candles);
		}

		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom(IDataSeries input, System.Double tolerancePercentage, System.Int32 candles)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.DoubleBottom(input, tolerancePercentage, candles);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom(System.Double tolerancePercentage, System.Int32 candles)
		{
			return LeadIndicator.DoubleBottom(InSeries, tolerancePercentage, candles);
		}

		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom(IDataSeries input, System.Double tolerancePercentage, System.Int32 candles)
		{
			return LeadIndicator.DoubleBottom(input, tolerancePercentage, candles);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom(System.Double tolerancePercentage, System.Int32 candles)
		{
			return LeadIndicator.DoubleBottom(InSeries, tolerancePercentage, candles);
		}

		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom(IDataSeries input, System.Double tolerancePercentage, System.Int32 candles)
		{
			return LeadIndicator.DoubleBottom(input, tolerancePercentage, candles);
		}
	}

	#endregion

}

#endregion

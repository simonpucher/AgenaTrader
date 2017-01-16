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
        private bool _drawTolerance;


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


            double LowestLowFromEchoBars;
            double LowestLowFromEchoBarsIndex;

            //Get the lowest Price/Index from our Echo-Period
            if (ProcessingBarIndex >= (Bars.Count- 1))
            {
                LowestLowFromEchoBars = LowestLowPrice(this.Candles)[0];
                LowestLowFromEchoBarsIndex = LowestLowIndex(this.Candles)[0];
            }
            else
            {
                return;  //Just the Last Bar plus the Echo-Bars
            }
                    
            DateTime OneMonthAgo = Time[0].AddMonths(-1);
            double tolerance     = LowestLowFromEchoBars * (TolerancePercentage / 100);
            double tolerance_min = LowestLowFromEchoBars - tolerance;
            double tolerance_max = LowestLowFromEchoBars + tolerance;

            Print("  Bar {0}, Tol+{1}, Tol-{2}",
            Bars[0].Time.ToString(), Math.Round(tolerance_max, 2), Math.Round(tolerance_min, 2));

            if (DrawTolerance)
            {
                //Show Tolerance
                AddChartRectangle("ToleranceRectangle", true, Bars.Count - 1 , tolerance_max, 0, tolerance_min, Color.Yellow, Color.Yellow, 50);
            }

            //find previous bottom
            //Select all data and find high & low.
            IEnumerable<IBar> lastBottoms = Bars.Where(x => x.Time <= OneMonthAgo)                 //older than 1 Month, so we have a arch in between the two low points
                                                .Where(y => y.Low <= tolerance_max                 // Low <= current Low + Tolerance
                                                         && y.Low >= tolerance_min                 // Low >= current Low + Tolerance    
                                                         )
                                                         .OrderBy(x => x.Low)
                                                         ;



            foreach (IBar bar in lastBottoms)
            {
                double LowestLow       = LowestLowPrice(Bars.GetBarsAgo(bar.Time))[0];      //calculate the lowest low between current bar and potential bottom
                double LowestLowBefore = LowestLowPrice(Bars.GetBarsAgo(bar.Time) + 20)[0]; //calculate the lowest low before the potential bottom. this is to make sure that there is no lower price leading up to the bottom

                int previous = Bars.GetBarsAgo(bar.Time) + 20;
                double LowestLowBefore2 = LowestLowPrice(previous)[0];

                //now check, if the current bar is on the same price level as the potential bottom. just to make sure, there is no lower price in that period.
                if (LowestLow       <= (tolerance_max)            //check if that lowest low is inside tolerance levels   
                 && LowestLow       >= (tolerance_min)
                 && LowestLowBefore <= (tolerance_max)            //check if the lowest low B
                 && LowestLowBefore >= (tolerance_min)
                 && ( LowestLow == LowestLowBefore                //LowestLow has to be either current bar or the current bottom from loop
                 ||   LowestLow == LowestLowFromEchoBars )
                    )
                {
                    Print("DoubleBottom  Low: {0}, Time: {1}, LowestLow: {2}, LowestLowBefore: {3}",
                          bar.Low, bar.Time.ToString(), LowestLow, LowestLowBefore);

                    string strdoubleBottomConnecter = "DoubleBottomConnecter_" + Bars[0].Time.ToString() + "_" + bar.Time.ToString();
                    //strdoubleBottomConnecter = "DoubleBottomConnecter";
                    AddChartLine(strdoubleBottomConnecter, Bars.GetBarsAgo(bar.Time), bar.Low, (int)LowestLowFromEchoBarsIndex, LowestLowFromEchoBars, Color.Red);


                    double BreakThrough    = HighestHighPrice(Bars.GetBarsAgo(bar.Time))[0];
                    double BreakThroughAgo = HighestHighIndex(Bars.GetBarsAgo(bar.Time))[0];

                    string strBreakThrough     = strdoubleBottomConnecter + "BreakThrough";
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

            if (true)
            {

            }


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

        [Description("Draw the ToleranceLevel")]
        [Category("Parameters")]
        [DisplayName("Draw Tolerance")]
        public bool DrawTolerance
        {
            get
            {
                return _drawTolerance;
            }

            set
            {
                _drawTolerance = value;
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
		public DoubleBottom DoubleBottom(System.Double tolerancePercentage, System.Int32 candles, System.Boolean drawTolerance)
        {
			return DoubleBottom(InSeries, tolerancePercentage, candles, drawTolerance);
		}

		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom(IDataSeries input, System.Double tolerancePercentage, System.Int32 candles, System.Boolean drawTolerance)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<DoubleBottom>(input, i => Math.Abs(i.TolerancePercentage - tolerancePercentage) <= Double.Epsilon && i.Candles == candles && i.DrawTolerance == drawTolerance);

			if (indicator != null)
				return indicator;

			indicator = new DoubleBottom
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input,
							TolerancePercentage = tolerancePercentage,
							Candles = candles,
							DrawTolerance = drawTolerance
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
		public DoubleBottom DoubleBottom(System.Double tolerancePercentage, System.Int32 candles, System.Boolean drawTolerance)
		{
			return LeadIndicator.DoubleBottom(InSeries, tolerancePercentage, candles, drawTolerance);
		}

		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom(IDataSeries input, System.Double tolerancePercentage, System.Int32 candles, System.Boolean drawTolerance)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.DoubleBottom(input, tolerancePercentage, candles, drawTolerance);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom(System.Double tolerancePercentage, System.Int32 candles, System.Boolean drawTolerance)
		{
			return LeadIndicator.DoubleBottom(InSeries, tolerancePercentage, candles, drawTolerance);
		}

		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom(IDataSeries input, System.Double tolerancePercentage, System.Int32 candles, System.Boolean drawTolerance)
		{
			return LeadIndicator.DoubleBottom(input, tolerancePercentage, candles, drawTolerance);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom(System.Double tolerancePercentage, System.Int32 candles, System.Boolean drawTolerance)
		{
			return LeadIndicator.DoubleBottom(InSeries, tolerancePercentage, candles, drawTolerance);
		}

		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom(IDataSeries input, System.Double tolerancePercentage, System.Int32 candles, System.Boolean drawTolerance)
		{
			return LeadIndicator.DoubleBottom(input, tolerancePercentage, candles, drawTolerance);
		}
	}

	#endregion

}

#endregion




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

        //input
        private double _tolerancePercentage = 0.6;
        private int _candles = 8;
        private bool _drawTolerance;
        private int _barsAgo = 20;


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

                DoubleBottom_DS.Set(0);



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


            //check if datafeed is providing appropriate data
            if (Bars[BarsAgo + (int)LowestLowFromEchoBarsIndex] == null)
            {
                return;
            }
            
            //Calculate the minimum distance from current low to the next low
            DateTime MinBarsAgoDateTime = Bars[BarsAgo + (int)LowestLowFromEchoBarsIndex].Time;

            //Calculate Tolerance
            double tolerance     = LowestLowFromEchoBars * (TolerancePercentage / 100);
            double tolerance_min = LowestLowFromEchoBars - tolerance;
            double tolerance_max = LowestLowFromEchoBars + tolerance;


            Print("  Bar {0}, Tol+{1}, Tol-{2}",
            Bars[0].Time.ToString(), Math.Round(tolerance_max, 2), Math.Round(tolerance_min, 2));
            

            //Check, when the chart was the last time below our current low. That period becomes irrelevant for us and gets ignored
            IEnumerable<IBar> belowLow = Bars.Where(y => y.Low <= tolerance_min).OrderBy(x => x.Low)
                                             .OrderByDescending(x => x.Time);

            //if there is no other Low and the chart is coming all the way from a higher price, than just leave this indicator
            if (!belowLow.Any())
            {
                return;
            }
            

            DateTime IgnoreFromHereOn = belowLow.FirstOrDefault().Time;

            //Draw ToleranceArea for the respected timeperiod
            if (DrawTolerance)
            {
                AddChartRectangle("ToleranceRectangle", true, Bars.GetBarsAgo(IgnoreFromHereOn), tolerance_max, 0, tolerance_min, Color.Yellow, Color.Yellow, 50);
            }


            //find previous bottom
            //Select all data and find high & low.
            IEnumerable<IBar> lastBottoms = Bars.Where(x => x.Time <= MinBarsAgoDateTime           //older than x Bars, so we have a arch in between the two low points 
                                                         && x.Time >= IgnoreFromHereOn )           //but younger than the timeperiod when the chart was below our low     
                                                .Where(y => y.Low <= tolerance_max                 // Low <= current Low + Tolerance
                                                         && y.Low >= tolerance_min                 // Low >= current Low + Tolerance    
                                                         )
                                                .OrderBy(x => x.Low)
                                                         ;

            int LowestLowBarsBefore = 5;

            foreach (IBar bar in lastBottoms)
            {
                double LowestLow       = LowestLowPrice(Bars.GetBarsAgo(bar.Time))[0];      //calculate the lowest low between current bar and potential bottom
                double LowestLowBefore = LowestLowPrice(Bars.GetBarsAgo(bar.Time) + LowestLowBarsBefore)[0]; //calculate the lowest low before the potential bottom. this is to make sure that there is no lower price leading up to the bottom

                //now check, if the current bar is on the same price level as the potential bottom. just to make sure, there is no lower price in that period.
                if (LowestLow       <= (tolerance_max)            //check if that lowest low is inside tolerance levels   
                 && LowestLow       >= (tolerance_min)
                 && LowestLowBefore <= (tolerance_max)            //check if the lowest low Before is inside tolerance levels 
                 && LowestLowBefore >= (tolerance_min)
                 && ( LowestLow == LowestLowBefore                //LowestLow has to be either current bar or the current bottom from loop
                 ||   LowestLow == LowestLowFromEchoBars )
                    )
                {
                    Print("DoubleBottom  Low: {0}, Time: {1}, LowestLow: {2}, LowestLowBefore: {3}",
                          bar.Low, bar.Time.ToString(), LowestLow, LowestLowBefore);

                    //Drawings
                    //Red Connection Line of the Bottoms
                    string strdoubleBottomConnecter = "DoubleBottomConnecter_" + Bars[0].Time.ToString() + "_" + bar.Time.ToString();
                    AddChartLine(strdoubleBottomConnecter, Bars.GetBarsAgo(bar.Time), bar.Low, (int)LowestLowFromEchoBarsIndex, LowestLowFromEchoBars, Color.Red);

                    //High and Breakthrough
                    double BreakThrough    = HighestHighPrice(Bars.GetBarsAgo(bar.Time))[0];
                    double BreakThroughAgo = HighestHighIndex(Bars.GetBarsAgo(bar.Time))[0];

                    string strBreakThrough     = strdoubleBottomConnecter + "BreakThrough";
                    string strBreakThroughVert = strdoubleBottomConnecter + "BreakThroughVert";
                    AddChartLine(strBreakThrough,     (int)BreakThroughAgo, BreakThrough, 0 , BreakThrough, Color.Green, DashStyle.Solid, 2);
                    AddChartLine(strBreakThroughVert, (int)BreakThroughAgo, bar.Low, (int)BreakThroughAgo, BreakThrough, Color.Aquamarine);

                    //Mark current low
                    DoubleBottom_DS.Set((int)LowestLowFromEchoBarsIndex,1);
                    //Mark previous low(s)
                    DoubleBottom_DS.Set(Bars.GetBarsAgo(bar.Time), 0.5);
                    SetSuccessFromEcho = true;
                }
            }
            if (SetSuccessFromEcho)
            {
                DoubleBottom_DS.Set(1);
            }
            else
            {
                DoubleBottom_DS.Set(0);
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


        [Description("Determines, how many bars the other bottom(s) should be at least away from the current low")]
        [Category("Parameters")]
        [DisplayName("Min Bars ago for last bottom")]
        public int BarsAgo
        {
            get
            {
                return _barsAgo;
            }

            set
            {
                _barsAgo = value;
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
		public DoubleBottom DoubleBottom(System.Double tolerancePercentage, System.Int32 candles, System.Boolean drawTolerance, System.Int32 barsAgo)
        {
			return DoubleBottom(InSeries, tolerancePercentage, candles, drawTolerance, barsAgo);
		}

		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom(IDataSeries input, System.Double tolerancePercentage, System.Int32 candles, System.Boolean drawTolerance, System.Int32 barsAgo)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<DoubleBottom>(input, i => Math.Abs(i.TolerancePercentage - tolerancePercentage) <= Double.Epsilon && i.Candles == candles && i.DrawTolerance == drawTolerance && i.BarsAgo == barsAgo);

			if (indicator != null)
				return indicator;

			indicator = new DoubleBottom
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input,
							TolerancePercentage = tolerancePercentage,
							Candles = candles,
							DrawTolerance = drawTolerance,
							BarsAgo = barsAgo
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
		public DoubleBottom DoubleBottom(System.Double tolerancePercentage, System.Int32 candles, System.Boolean drawTolerance, System.Int32 barsAgo)
		{
			return LeadIndicator.DoubleBottom(InSeries, tolerancePercentage, candles, drawTolerance, barsAgo);
		}

		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom(IDataSeries input, System.Double tolerancePercentage, System.Int32 candles, System.Boolean drawTolerance, System.Int32 barsAgo)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.DoubleBottom(input, tolerancePercentage, candles, drawTolerance, barsAgo);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom(System.Double tolerancePercentage, System.Int32 candles, System.Boolean drawTolerance, System.Int32 barsAgo)
		{
			return LeadIndicator.DoubleBottom(InSeries, tolerancePercentage, candles, drawTolerance, barsAgo);
		}

		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom(IDataSeries input, System.Double tolerancePercentage, System.Int32 candles, System.Boolean drawTolerance, System.Int32 barsAgo)
		{
			return LeadIndicator.DoubleBottom(input, tolerancePercentage, candles, drawTolerance, barsAgo);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom(System.Double tolerancePercentage, System.Int32 candles, System.Boolean drawTolerance, System.Int32 barsAgo)
		{
			return LeadIndicator.DoubleBottom(InSeries, tolerancePercentage, candles, drawTolerance, barsAgo);
		}

		/// <summary>
		/// DoubleBottom
		/// </summary>
		public DoubleBottom DoubleBottom(IDataSeries input, System.Double tolerancePercentage, System.Int32 candles, System.Boolean drawTolerance, System.Int32 barsAgo)
		{
			return LeadIndicator.DoubleBottom(input, tolerancePercentage, candles, drawTolerance, barsAgo);
		}
	}

	#endregion

}

#endregion




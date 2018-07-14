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
/// Version: stable
/// -------------------------------------------------------------------------
/// Christian Kovar 2017
/// Simon Pucher 2017
/// http://script-trading.com
/// -------------------------------------------------------------------------
/// inspired by the geomatrical figures of a double/tripple top, this indicator
/// is showing and highlighting highs that occur on the same level.
/// -use the _tolerancePercentage parameter to adjust your price tolerance in percentage
/// -use the _barsAgo parameter to adjust the length of the arch that should be between your
/// current and past high(s)
/// -us the _candles parameter to implement an "echo" funcionality. this feature is not only
/// checking the current/last bar but also the last 8 bars. This makes screening much easier, 
/// especially in the scanner column
/// -------------------------------------------------------------------------
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("DoubleTop")]
	public class DoubleTop : UserIndicator
	{
        private Boolean SetSuccessFromEcho = false;

        //input
        private double _tolerancePercentage = 0.6;
        private int _candles = 8;
        private bool _drawTolerance;
        private int _barsAgo = 20;


        protected override void OnInit()
        {
            Add(new OutputDescriptor(Color.Red, "DoubleTop_DS"));
            IsOverlay = false;
            CalculateOnClosedBar = false;

            //Inhalt des OutputWindow l�schen
            ClearTraceWindow();
        }

        protected override void OnCalculate()
        {

            DoubleTop_DS.Set(0);

            double HighestHighFromEchoBars;
            double HighestHighFromEchoBarsIndex;
            DateTime HighestHighFromEchoBarsDate;

            //Get the highest Price/Index from our Echo-Period
            if (ProcessingBarIndex >= (Bars.Count - 1))
            {
                HighestHighFromEchoBars = HighestHighPrice(this.Candles)[0];
                HighestHighFromEchoBarsIndex = HighestHighIndex(this.Candles)[0];
                HighestHighFromEchoBarsDate = Bars[(int)HighestHighFromEchoBarsIndex].Time;
            }
            else
            {
                return;  //Just the Last Bar plus the Echo-Bars
            }


            //check if datafeed is providing appropriate data
            if (Bars[BarsAgo + (int)HighestHighFromEchoBarsIndex] == null)
            {
                return;
            }

            //Calculate the minimum distance from current low to the next low
            DateTime MinBarsAgoDateTime = Bars[BarsAgo + (int)HighestHighFromEchoBarsIndex].Time;

            //Calculate Tolerance
            double tolerance = HighestHighFromEchoBars * (TolerancePercentage / 100);
            double tolerance_min = HighestHighFromEchoBars - tolerance;
            double tolerance_max = HighestHighFromEchoBars + tolerance;


            Print(Bars.Instrument + " Bar {0}, Tol+{1}, Tol-{2}",
            Bars[0].Time.ToString(), Math.Round(tolerance_max, 2), Math.Round(tolerance_min, 2));




            //Check, when the chart was the last time above our current high. That period becomes irrelevant for us and gets ignored
            IEnumerable<IBar> aboveHigh = Bars.Where(y => y.High >= tolerance_max)
                                              .Where(x => x.Time <  HighestHighFromEchoBarsDate)
                                              .OrderByDescending(x => x.Time);

            DateTime IgnoreFromHereOn = DateTime.MinValue.AddDays(1);
            //if there is no other High and the chart is coming all the way from a lower price, than just leave this indicator
            //if (!aboveHigh.Any())
            //{
            //    return;
            //}

            if (aboveHigh.GetEnumerator().MoveNext())
            {
                IgnoreFromHereOn = aboveHigh.FirstOrDefault().Time;
            }
            

            //Draw ToleranceArea for the respected timeperiod
            if (DrawTolerance)
            {
                //AddChartRectangle("ToleranceRectangle", true, Bars.GetBarsAgo(IgnoreFromHereOn), tolerance_max, 0, tolerance_min, Color.Yellow, Color.Yellow, 50);
                AddChartRectangle("ToleranceRectangle", true, IgnoreFromHereOn.AddDays(-1), tolerance_max, Bars[0].Time.AddDays(1), tolerance_min, Color.Yellow, Color.Yellow, 50);
            }

            //Check, if the time period between the highes Echo-Candle and the MinBarsAgo has any higher price. then we are not at a current high, we are just in strange time situations
            if (HighestHighPrice(this.Candles + BarsAgo)[0] > HighestHighFromEchoBars)
            {
                return;
            }

            //find previous highs
            //Select all data and find highs.
            IEnumerable<IBar> lastTops = Bars.Where(x => x.Time <= MinBarsAgoDateTime           //older than x Bars, so we have a arch in between the two low points 
                                                         && x.Time >= IgnoreFromHereOn)           //but younger than the timeperiod when the chart was below our low     
                                                .Where(y => y.High <= tolerance_max                 // Low <= current Low + Tolerance
                                                         && y.High >= tolerance_min                 // Low >= current Low + Tolerance    
                                                         )
                                                .OrderBy(x => x.High)
                                                         ;

            int HighestHighBarsBefore = 5;

            foreach (IBar bar in lastTops)
            {
                double HighestHigh       = HighestHighPrice(Bars.GetBarsAgo(bar.Time))[0];                         //calculate the HighestHigh between current bar and potential bottom
                double HighestHighBefore = HighestHighPrice(Bars.GetBarsAgo(bar.Time) + HighestHighBarsBefore)[0]; //calculate the HighestHigh before the potential top. this is to make sure that there is no higher price leading up to the top

                //now check, if the current bar is on the same price level as the potential top. just to make sure, there is no higher price in that period.
                if (HighestHigh       <= (tolerance_max)                  //check if that HighestHigh is inside tolerance levels   
                 && HighestHigh       >= (tolerance_min)
                 && HighestHighBefore <= (tolerance_max)                  //check if the HighestHighBefore is inside tolerance levels 
                 && HighestHighBefore >= (tolerance_min)
                && (HighestHigh       == HighestHighBefore                //HighestHigh has to be either current bar or the current bottom from loop
                 || HighestHigh       == HighestHighFromEchoBars)
                    )
                {
                    Print("DoubleTop  High: {0}, Time: {1}, HighestHigh: {2}, HighestHighBefore: {3}",
                          bar.High, bar.Time.ToString(), HighestHigh, HighestHighBefore);

                    //Drawings
                    //Red Connection Line of the Bottoms
                    string strdoubleTopConnecter = "DoubleTopConnecter_" + Bars[0].Time.ToString() + "_" + bar.Time.ToString();
                    AddChartLine(strdoubleTopConnecter, Bars.GetBarsAgo(bar.Time), bar.High, (int)HighestHighFromEchoBarsIndex, HighestHighFromEchoBars, Color.Red);

                    //High and Breakthrough
                    double BreakThrough    = LowestLowPrice(Bars.GetBarsAgo(bar.Time))[0];
                    double BreakThroughAgo = LowestLowIndex(Bars.GetBarsAgo(bar.Time))[0];

                    string strBreakThrough = strdoubleTopConnecter + "BreakThrough";
                    string strBreakThroughVert = strdoubleTopConnecter + "BreakThroughVert";
                    AddChartLine(strBreakThrough,     (int)BreakThroughAgo, BreakThrough, 0,                    BreakThrough, Color.Aquamarine, DashStyle.Solid, 2);
                    AddChartLine(strBreakThroughVert, (int)BreakThroughAgo, bar.High,     (int)BreakThroughAgo, BreakThrough, Color.Aquamarine, DashStyle.Solid, 2);

                    //Mark current High
                    DoubleTop_DS.Set((int)HighestHighFromEchoBarsIndex, 1);
                    //Mark previous High(s)
                    DoubleTop_DS.Set(Bars.GetBarsAgo(bar.Time), 0.5);
                    SetSuccessFromEcho = true;
                }
            }
            if (SetSuccessFromEcho)
            {
                DoubleTop_DS.Set(1);
            }
            else
            {
                DoubleTop_DS.Set(0);
            }
        }

        #region Properties

        [Browsable(false)]
		[XmlIgnore()]
		public DataSeries DoubleTop_DS
        {
			get { return Outputs[0]; }
		}

        [Description("Tolerance level in percent.")]
        [InputParameter]
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
        [InputParameter]
        [DisplayName("Candles")]
        public int Candles
        {
            get { return _candles; }
            set { _candles = value; }
        }

        [Description("Draw the ToleranceLevel")]
        [InputParameter]
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
        [InputParameter]
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
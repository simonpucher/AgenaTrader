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
/// inspired by the geomatrical figures of a double/tripple bottom, this indicator
/// is showing and highlighting lows that occur on the same level.
/// -use the _tolerancePercentage parameter to adjust your price tolerance in percentage
/// -use the _barsAgo parameter to adjust the length of the arch that should be between your
/// current and past low(s)
/// -us the _candles parameter to implement an "echo" funcionality. this feature is not only
/// checking the current/last bar but also the last 8 bars. This makes screening much easier, 
/// especially in the scanner column
/// -------------------------------------------------------------------------
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>

namespace AgenaTrader.UserCode
{
    [Description("DoubleBottom")]
    public class DoubleBottom : UserIndicator
    {

        private Boolean SetSuccessFromEcho = false;
        private Boolean VerboseMode = false;

        //input
        private double _tolerancePercentage = 0.6;
        private int _candles = 8;
        private bool _drawTolerance;
        private int _barsAgo = 20;
        private bool _history = false;
        private bool _filter_SMA200;

        //output
        private double _stop = double.MaxValue;
        private double _target = double.MinValue;
        

        protected override void OnInit()
        {
            Add(new OutputDescriptor(Color.LawnGreen, "DoubleBottom_DS"));
            IsOverlay = false;
            CalculateOnClosedBar = false;

            //Inhalt des OutputWindow l�schen
            if (VerboseMode)
            {
                ClearTraceWindow();
            }
            
        }

        protected override void OnCalculate()
        {

                DoubleBottom_DS.Set(0);
            SetSuccessFromEcho = false;



            double LowestLowFromEchoBars;
            double LowestLowFromEchoBarsIndex;
            DateTime LowestLowFromEchoBarsDate;

            //Get the lowest Price/Index from our Echo-Period
            if (ProcessingBarIndex >= (Bars.Count- 1) || History == true)
            {
                LowestLowFromEchoBars = LowestLowPrice(this.Candles)[0];
                LowestLowFromEchoBarsIndex = LowestLowIndex(this.Candles)[0];
                LowestLowFromEchoBarsDate = Bars[(int)LowestLowFromEchoBarsIndex].Time;
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

            double SMA_tol     = SMA(200)[0] * (TolerancePercentage / 100);
            double SMA_tol_min = SMA(200)[0] - SMA_tol;
            double SMA_tol_max = SMA(200)[0] + SMA_tol;

            if (VerboseMode)
            {
                Print("Instrument {3}  Bar {0}, Tol+{1}, Tol-{2}",
            Bars[0].Time.ToString(), Math.Round(tolerance_max, 2), Math.Round(tolerance_min, 2), Bars.Instrument);
            }
            


            //Check, when the chart was the last time below our current low. That period becomes irrelevant for us and gets ignored
            IEnumerable<IBar> belowLow = Bars.Where(y => y.Low <= tolerance_min)
                                             .Where(x => x.Time < LowestLowFromEchoBarsDate)
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
                //AddChartRectangle("ToleranceRectangle", true, Bars.GetBarsAgo(IgnoreFromHereOn), tolerance_max, 0, tolerance_min, Color.Yellow, Color.Yellow, 50);
                AddChartRectangle("ToleranceRectangle", true, IgnoreFromHereOn.AddDays(-1), tolerance_max, Bars[0].Time.AddDays(1), tolerance_min, Color.Yellow, Color.Yellow, 50);
            }


            //Check, if the time period between the highes Echo-Candle and the MinBarsAgo has any higher price. then we are not at a current high, we are just in strange time situations
            if (LowestLowPrice(this.Candles + BarsAgo)[0] < LowestLowFromEchoBars)
            {
                return;
            }


            //find previous bottom
            //Select all data and find lows.
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


                    //Drawings
                    //Red Connection Line of the Bottoms
                    string strdoubleBottomConnecter = "DoubleBottomConnecter_" + Bars[0].Time.ToString() + "_" + bar.Time.ToString();
                    AddChartLine(strdoubleBottomConnecter, Bars.GetBarsAgo(bar.Time), bar.Low, (int)LowestLowFromEchoBarsIndex, LowestLowFromEchoBars, Color.LawnGreen);

                    //High and Breakthrough
                    double BreakThrough    = HighestHighPrice(Bars.GetBarsAgo(bar.Time))[0];
                    double BreakThroughAgo = HighestHighIndex(Bars.GetBarsAgo(bar.Time))[0];

                    string strBreakThrough     = strdoubleBottomConnecter + "BreakThrough";
                    string strBreakThroughVert = strdoubleBottomConnecter + "BreakThroughVert";
                    AddChartLine(strBreakThrough,     (int)BreakThroughAgo, BreakThrough, 0 , BreakThrough, Color.Aquamarine, DashStyle.Solid, 2);
                    AddChartLine(strBreakThroughVert, (int)BreakThroughAgo, bar.Low, (int)BreakThroughAgo, BreakThrough, Color.Aquamarine, DashStyle.Solid, 2);

                    if (Filter_SMA200)
                    {
                        if (  bar.Low < SMA_tol_min
                           || bar.Low > SMA_tol_max)
                        {
                            continue;  //check for SMA200 filter and check if the current low is within the SMA-tolerances
                        }
                    }
                    //Mark current low
                    DoubleBottom_DS.Set((int)LowestLowFromEchoBarsIndex,1);
                    //Mark previous low(s)
                    DoubleBottom_DS.Set(Bars.GetBarsAgo(bar.Time), 0.5);
                    SetSuccessFromEcho = true;

                    if (VerboseMode)
                    {
                        Print("DoubleBottom  Low: {0}, Time: {1}, LowestLow: {2}, LowestLowBefore: {3}, BreakThrough:  {4}",
                          bar.Low, bar.Time.ToString(), LowestLow, LowestLowBefore, BreakThrough);
                    }

                    //set Stop and Target for Strategy
                    _stop = LowestLow * 0.99;
                    _target = BreakThrough * 0.99;
                }
            }
            if (SetSuccessFromEcho)
            {
                DoubleBottom_DS.Set(1);
                Print("Indikator Kaufsignal: " + LowestLowFromEchoBarsDate.ToString());
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

        [Description("Also calculate historic values (longer calculatio time!)")]
        [InputParameter]
        [DisplayName("History")]
        public bool History
        {
            get
            {
                return _history;
            }

            set
            {
                _history = value;
            }
        }

        [Description("Filter: SMA200)")]
        [InputParameter]
        [DisplayName("SMA200")]
        public bool Filter_SMA200
        {
            get
            {
                return _filter_SMA200;
            }

            set
            {
                _filter_SMA200 = value;
            }
        }

        public double Stop
        {
            get
            {
                return _stop;
            }

        }

        public double Target
        {
            get
            {
                return _target;
            }
        }




        #endregion
    }
}
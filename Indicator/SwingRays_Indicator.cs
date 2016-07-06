#region Using declarations
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Plots horizontal rays at swing highs and lows and removes them once broken.  
    /// </summary>
    [Description("Plots horizontal rays at swing highs and lows and removes them once broken.")]
    public class SwingRaysV1 : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int strength = 5; 				// number of bars required to left and right of the pivot high/low
			// User defined variables (add any user defined variables below)
			private Color swingHighColor = Color.DarkCyan;
			private Color swingLowColor = Color.Magenta;	
					
			private ArrayList	lastHighCache;
			private ArrayList	lastLowCache;
			private double		lastSwingHighValue	= double.MaxValue;  // used when testing for price breaks
			private double		lastSwingLowValue	= double.MinValue;
			private Stack 		swingHighRays;		//	last entry contains nearest swing high; removed when swing is broken
			private Stack 		swingLowRays;		//  track swing lows in the same manner
			private bool	 	enableAlerts 		= 	true;
			private bool		keepBrokenLines 	=	true;
			
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            DisplayInDataBox 	= false;
            CalculateOnBarClose	= true;
            Overlay				= true;
            PriceTypeSupported	= false;
			
			lastHighCache = new ArrayList();  // used to identify swing points; from default Swing indicator
			lastLowCache = new ArrayList();
			swingHighRays = new Stack();   // LIFO buffer; last entry contains the nearest swing high
			swingLowRays = new Stack();
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // build up cache of recent High and Low values
			// code devised from default Swing Indicator by marqui@BMT, 10-NOV-2010 
			lastHighCache.Add(High[0]);  
			if (lastHighCache.Count > (2 * strength) + 1)
				lastHighCache.RemoveAt(0);  // if cache is filled, drop the oldest value
			lastLowCache.Add(Low[0]);
			if (lastLowCache.Count > (2 * strength) + 1)
				lastLowCache.RemoveAt(0); 
			//
			if (lastHighCache.Count == (2 * strength) + 1)  // wait for cache of Highs to be filled
				{
					// test for swing high 
					bool isSwingHigh = true;  
					double swingHighCandidateValue = (double) lastHighCache[strength];
					for (int i=0; i < strength; i++)
						if ((double) lastHighCache[i] >= swingHighCandidateValue - double.Epsilon)
							isSwingHigh = false; // bar(s) to right of candidate were higher
					for (int i=strength+1; i < lastHighCache.Count; i++)
						if ((double) lastHighCache[i] > swingHighCandidateValue - double.Epsilon)
							isSwingHigh = false; // bar(s) to left of candidate were higher
					// end of test
						
					if (isSwingHigh)
						lastSwingHighValue = swingHighCandidateValue;
				
					if (isSwingHigh)  // if we have a new swing high then we draw a ray line on the chart
						{
							IRay newRay = DrawRay( "highRay" + (CurrentBar-strength), false, strength, lastSwingHighValue, 0, lastSwingHighValue, swingHighColor, DashStyle.Dot, 2); 
							swingHighRays.Push(newRay);  // store a reference so we can remove it from the chart later
						}
					else if (High[0] > lastSwingHighValue)  // otherwise, we test to see if price has broken through prior swing high
						{
							if (swingHighRays.Count > 0)  // just to be safe 
								{
									IRay currentRay = (IRay) swingHighRays.Pop(); // pull current ray from stack   
									if (enableAlerts) Alert("SwHiAlert", Priority.Low, "Swing High at " + currentRay.Anchor1Y + " broken", "Alert2.wav", 5, Color.White, Color.Red);
									if (keepBrokenLines)  // draw a line between swing point and break bar 
										{	
											int barsAgo = currentRay.Anchor1BarsAgo;
											ILine newLine = DrawLine("highLine"+(CurrentBar-barsAgo), false, barsAgo, currentRay.Anchor1Y, 0, currentRay.Anchor1Y, swingHighColor, DashStyle.Solid, 2);
										}
									RemoveDrawObject(currentRay.Tag);  
									if (swingHighRays.Count > 0)
										{
											IRay priorRay = (IRay) swingHighRays.Peek();
											lastSwingHighValue=priorRay.Anchor1Y;  // needed when testing the break of the next swing high
										}
									else
										lastSwingHighValue=double.MaxValue;  // there are no higher swings on the chart; reset to default					
								}
						}				
				}
			
				if (lastLowCache.Count == (2 * strength) + 1)  // repeat the above for the swing lows
				{
					// test for swing low 
					bool isSwingLow = true;  
					double swingLowCandidateValue = (double) lastLowCache[strength];
					for (int i=0; i < strength; i++)
						if ((double) lastLowCache[i] <= swingLowCandidateValue + double.Epsilon)
							isSwingLow = false; // bar(s) to right of candidate were lower

					for (int i=strength+1; i < lastLowCache.Count; i++)
						if ((double) lastLowCache[i] < swingLowCandidateValue + double.Epsilon)
							isSwingLow = false; // bar(s) to left of candidate were lower
					// end of test for low
						
					if (isSwingLow)
						lastSwingLowValue = swingLowCandidateValue;
				
					if (isSwingLow)  // found a new swing low; draw it on the chart
						{
							IRay newRay = DrawRay( "lowRay" + (CurrentBar-strength), false, strength, lastSwingLowValue, 0, lastSwingLowValue, swingLowColor, DashStyle.Dot, 2); 
							swingLowRays.Push(newRay);  
						}
					else if (Low[0] < lastSwingLowValue)  // otherwise test to see if price has broken through prior swing low
						{
							if (swingLowRays.Count > 0)
								{
									IRay currentRay = (IRay) swingLowRays.Pop();  
									if (enableAlerts) Alert("SwHiAlert", Priority.Low, "Swing Low at " + currentRay.Anchor1Y + " broken", "Alert2.wav", 5, Color.White, Color.Red);
									if (keepBrokenLines)  // draw a line between swing point and break bar 
										{	
											int barsAgo = currentRay.Anchor1BarsAgo;
											ILine newLine = DrawLine("highLine"+(CurrentBar-barsAgo), false, barsAgo, currentRay.Anchor1Y, 0, currentRay.Anchor1Y, swingLowColor, DashStyle.Solid, 2);
										}
									RemoveDrawObject(currentRay.Tag);  
							
									if (swingLowRays.Count > 0)
										{
											IRay priorRay = (IRay) swingLowRays.Peek();  
											lastSwingLowValue=priorRay.Anchor1Y;  // price level of the prior swing low  
										}
									else
										lastSwingLowValue=double.MinValue;  // no swing lows present; set this to default value 
								}
						}
				}
			}

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HighRay
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LowRay
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HighLine
        {
            get { return Values[2]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LowLine
        {
            get { return Values[3]; }
        }

        [Description("Number of bars before/after each pivot bar")]
        [GridCategory("Parameters")]
        public int Strength
        {
            get { return strength; }
            set { strength = Math.Max(2, value); }
        }

		[Description("Alert when swings are broken")]
        [GridCategory("Parameters")]
        public bool EnableAlerts
        {
            get { return enableAlerts; }
            set { enableAlerts = value; }
        }

		[Description("Show broken swing points")]
        [GridCategory("Parameters")]
        public bool KeepBrokenLines
        {
            get { return keepBrokenLines; }
            set { keepBrokenLines = value; }
        }

		[Description("Color for swing highs")]
        [GridCategory("Colors")]
        public Color SwingHighColor
        {
            get { return swingHighColor; }
            set { swingHighColor = value; }
        }
		
		[Description("Color for swing lows")]
        [GridCategory("Colors")]
        public Color SwingLowColor
        {
            get { return swingLowColor; }
            set { swingLowColor = value; }
        }
			
		#endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private SwingRaysV1[] cacheSwingRaysV1 = null;

        private static SwingRaysV1 checkSwingRaysV1 = new SwingRaysV1();

        /// <summary>
        /// Plots horizontal rays at swing highs and lows and removes them once broken.
        /// </summary>
        /// <returns></returns>
        public SwingRaysV1 SwingRaysV1(bool enableAlerts, bool keepBrokenLines, int strength, Color swingHighColor, Color swingLowColor)
        {
            return SwingRaysV1(Input, enableAlerts, keepBrokenLines, strength, swingHighColor, swingLowColor);
        }

        /// <summary>
        /// Plots horizontal rays at swing highs and lows and removes them once broken.
        /// </summary>
        /// <returns></returns>
        public SwingRaysV1 SwingRaysV1(Data.IDataSeries input, bool enableAlerts, bool keepBrokenLines, int strength, Color swingHighColor, Color swingLowColor)
        {
            if (cacheSwingRaysV1 != null)
                for (int idx = 0; idx < cacheSwingRaysV1.Length; idx++)
                    if (cacheSwingRaysV1[idx].EnableAlerts == enableAlerts && cacheSwingRaysV1[idx].KeepBrokenLines == keepBrokenLines && cacheSwingRaysV1[idx].Strength == strength && cacheSwingRaysV1[idx].SwingHighColor == swingHighColor && cacheSwingRaysV1[idx].SwingLowColor == swingLowColor && cacheSwingRaysV1[idx].EqualsInput(input))
                        return cacheSwingRaysV1[idx];

            lock (checkSwingRaysV1)
            {
                checkSwingRaysV1.EnableAlerts = enableAlerts;
                enableAlerts = checkSwingRaysV1.EnableAlerts;
                checkSwingRaysV1.KeepBrokenLines = keepBrokenLines;
                keepBrokenLines = checkSwingRaysV1.KeepBrokenLines;
                checkSwingRaysV1.Strength = strength;
                strength = checkSwingRaysV1.Strength;
                checkSwingRaysV1.SwingHighColor = swingHighColor;
                swingHighColor = checkSwingRaysV1.SwingHighColor;
                checkSwingRaysV1.SwingLowColor = swingLowColor;
                swingLowColor = checkSwingRaysV1.SwingLowColor;

                if (cacheSwingRaysV1 != null)
                    for (int idx = 0; idx < cacheSwingRaysV1.Length; idx++)
                        if (cacheSwingRaysV1[idx].EnableAlerts == enableAlerts && cacheSwingRaysV1[idx].KeepBrokenLines == keepBrokenLines && cacheSwingRaysV1[idx].Strength == strength && cacheSwingRaysV1[idx].SwingHighColor == swingHighColor && cacheSwingRaysV1[idx].SwingLowColor == swingLowColor && cacheSwingRaysV1[idx].EqualsInput(input))
                            return cacheSwingRaysV1[idx];

                SwingRaysV1 indicator = new SwingRaysV1();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.EnableAlerts = enableAlerts;
                indicator.KeepBrokenLines = keepBrokenLines;
                indicator.Strength = strength;
                indicator.SwingHighColor = swingHighColor;
                indicator.SwingLowColor = swingLowColor;
                Indicators.Add(indicator);
                indicator.SetUp();

                SwingRaysV1[] tmp = new SwingRaysV1[cacheSwingRaysV1 == null ? 1 : cacheSwingRaysV1.Length + 1];
                if (cacheSwingRaysV1 != null)
                    cacheSwingRaysV1.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSwingRaysV1 = tmp;
                return indicator;
            }
        }
    }
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
    public partial class Column : ColumnBase
    {
        /// <summary>
        /// Plots horizontal rays at swing highs and lows and removes them once broken.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SwingRaysV1 SwingRaysV1(bool enableAlerts, bool keepBrokenLines, int strength, Color swingHighColor, Color swingLowColor)
        {
            return _indicator.SwingRaysV1(Input, enableAlerts, keepBrokenLines, strength, swingHighColor, swingLowColor);
        }

        /// <summary>
        /// Plots horizontal rays at swing highs and lows and removes them once broken.
        /// </summary>
        /// <returns></returns>
        public Indicator.SwingRaysV1 SwingRaysV1(Data.IDataSeries input, bool enableAlerts, bool keepBrokenLines, int strength, Color swingHighColor, Color swingLowColor)
        {
            return _indicator.SwingRaysV1(input, enableAlerts, keepBrokenLines, strength, swingHighColor, swingLowColor);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Plots horizontal rays at swing highs and lows and removes them once broken.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SwingRaysV1 SwingRaysV1(bool enableAlerts, bool keepBrokenLines, int strength, Color swingHighColor, Color swingLowColor)
        {
            return _indicator.SwingRaysV1(Input, enableAlerts, keepBrokenLines, strength, swingHighColor, swingLowColor);
        }

        /// <summary>
        /// Plots horizontal rays at swing highs and lows and removes them once broken.
        /// </summary>
        /// <returns></returns>
        public Indicator.SwingRaysV1 SwingRaysV1(Data.IDataSeries input, bool enableAlerts, bool keepBrokenLines, int strength, Color swingHighColor, Color swingLowColor)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SwingRaysV1(input, enableAlerts, keepBrokenLines, strength, swingHighColor, swingLowColor);
        }
    }
}
#endregion

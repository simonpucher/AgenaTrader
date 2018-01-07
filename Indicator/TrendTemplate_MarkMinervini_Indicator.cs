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
/// Version: 1.1.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// This indicator provides trend template by Mark Minervini signals http://www.stockfetcher.com/forums2/Filter-Exchange/Trend-Template-by-Mark-Minervini/125969
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{

    [Description("Enter the description for the new custom indicator here")]
	public class TrendTemplate_MarkMinervini_Indicator : UserIndicator
	{

        private bool ErrorOccured = false;


        protected override void OnInit()
		{
			Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Red), "MyPlot1"));
            Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Orange), "MyPlot2"));
            Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Blue), "MyPlot3"));
            CalculateOnClosedBar = true;
            this.IsOverlay = true;
		}

        protected override void OnStart()
        {
            base.OnStart();
            
            this.ErrorOccured = false;
        }

        protected override void OnCalculate()
		{

            this.MyPlot1.Set(SMA(50)[0]);
            this.MyPlot2.Set(SMA(150)[0]);
            this.MyPlot3.Set(SMA(200)[0]);

            //Lets call the calculate method and save the result with the trade action
            ResultValue returnvalue = this.calculate(Close);

            //If the calculate method was not finished we need to stop and show an alert message to the user.
            if (returnvalue.ErrorOccured)
            {
                //Display error just one time
                if (!this.ErrorOccured)
                {
                    GlobalUtilities.DrawAlertTextOnChart(this, Const.DefaultStringErrorDuringCalculation);
                    this.ErrorOccured = true;
                }
                return;
            }


            //Entry
            if (returnvalue.Entry.HasValue)
            {
                switch (returnvalue.Entry)
                {
                    case OrderDirection.Buy:
                        //AddChartDot("ArrowLong_Entry" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, Color.LightGreen);
                        //this.MyPlot1.Set(1);
                        AddChartArrowUp("ArrowLong_Entry" + +Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].Low, Color.LightGreen);
                        break;
                }
            }
            else
            {
                //Value was null so nothing to do.
                //this.MyPlot1.Set(0);
            }

        }


        public ResultValue calculate(IDataSeries input)
        {
            //Create a return object
            ResultValue returnvalue = new ResultValue();

            //try catch block with all calculations
            try
            {
                
                /*
               The Trend Template is a set of selection criteria by Market Wizard Mark Minervini. Here are the rules: 

               1. The current stock price is above both the 150-day (30-week) and the 200-day (40-week) moving average price lines. 
               2. The 150-day moving average is above the 200-day moving average. 
               3. The 200-day moving average line is trending up for at least 1 month (preferably 4�5 months minimum in most cases). 
               4. The 50-day (10-week) moving average is above both the 150-day and 200-day moving averages. 
               5. The current stock price is trading above the 50-day moving average. 
               6. The current stock price is at least 30 percent above its 52-week low. (Many of the best selections will be 100 percent, 300 percent, or greater above their 52-week low before they emerge from a solid consolidation period and mount a large scale advance.) 
               7. The current stock price is within at least 25 percent of its 52-week high (the closer to a new high the better). 
               8. The relative strength ranking (as reported in Investor�s Business Daily) is no less than 70, and preferably in the 80s or 90s, which will generally be the case with the better selections. 

               StockFetcher Code: 

               Close is above MA(50) 
               MA(50) is above MA(150) 
               MA(150) is above MA(200) 
               MA(200) is increasing for 1 month 
               Close divided by 260 day low is above 1.3 
               Close divided by 260 day high is above 0.75 
               Relative strength(^SPX,90) is above 1.0
            */

                if (input[0] > SMA(input, 50)[0] && SMA(input, 50)[0] > SMA(input, 150)[0] && SMA(input, 150)[0] > SMA(input, 200)[0] && IsSeriesRising(SMA(input, 200))
                    && input[0] / LowestLowPrice(input, 260)[0] > 1.3 && input[0] / HighestHighPrice(input, 260)[0] > 0.75)
                {
                    returnvalue.Entry = OrderDirection.Buy;
                }
                else
                {
                    returnvalue.Exit = null;
                }

            }
            catch (Exception)
            {
                //If this method is called via a strategy or a condition we need to log the error.
                returnvalue.ErrorOccured = true;
            }


            //return the result object
            return returnvalue;
        }


        public override string ToString()
        {
            return "TT Mark Minervini (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "TT Mark Minervini (I)";
            }
        }

        #region Properties

        [Browsable(false)]
		[XmlIgnore()]
		public DataSeries MyPlot1
		{
			get { return Outputs[0]; }
		}

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries MyPlot2
        {
            get { return Outputs[1]; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries MyPlot3
        {
            get { return Outputs[2]; }
        }


        #endregion
    }
}
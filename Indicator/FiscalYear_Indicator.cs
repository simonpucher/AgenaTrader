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
using System.Runtime.CompilerServices;


/// <summary>
/// Version: 1.1.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2018
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
  


    [Description("Show seasonal trends")]
	public class FiscalYear_Indicator : UserIndicator
	{

        #region Variables

        private int _year = 0;



        #endregion


        protected override void OnInit()
		{
			//Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
			IsOverlay = true;
		}

        protected override void OnBarsRequirements()
        {
            //Print("InitRequirements");
        }
   

        protected override void OnStart()
        {
            //Print("OnStartUp");

            /*   switch (FiscalYearType)
              {
                  case FiscalYearType.SellInMay:
                      _list_sellinmayandgoaway_buy = Bars.Where(x => x.Time.Month <= 4 || x.Time.Month >= 10);
                      _list_sellinmayandgoaway_sell = Bars.Except(_list_sellinmayandgoaway_buy);
                      hashset = new HashSet<DateTime>(_list_sellinmayandgoaway_buy.Select(x => x.Time));
                      break;
                  case FiscalYearType.SantaClausRally:
                      _list_santaclausrally_buy = from b in Bars
                                                 where (b.Time.Month == 12 && b.Time.Day >= 15) || (b.Time.Month == 1 && b.Time.Day <= 9)
                                                 select b;
                      hashset = new HashSet<DateTime>(_list_santaclausrally_buy.Select(x => x.Time));
                      break;
                  case FiscalYearType.fourthofjuly:
                      _list_fourthofjuly_buy = from b in Bars
                                                  where (b.Time.Month == 7 && b.Time.Day >= 1) || (b.Time.Month == 7 && b.Time.Day <= 8)
                                                  select b;
                      hashset = new HashSet<DateTime>(_list_fourthofjuly_buy.Select(x => x.Time));
                      break;
                  case FiscalYearType.manual:
                      _list_manual_buy = from b in Bars
                                               where (b.Time.Month >= this.Start_Month  && b.Time.Month <= this.End_Month)
                                               select b;
                      hashset = new HashSet<DateTime>(_list_manual_buy.Select(x => x.Time));
                      break;
                  default:
                      break;
              } */

            

            CalculateOnClosedBar = true;
            IsOverlay = true;
        }

		protected override void OnCalculate()
		{
            if (_year == 0)
            {
                _year = Time[0].Year;
            }

            //DateTime lastDayOfYear = new DateTime(Time[0].Year + 1, 1, 1).AddDays(-1);

            if (_year < Time[0].Year)
            {
                AddChartVerticalLine(Time[0].Date.ToString(), 0, Color.Magenta, DashStyle.Dash, 2);
                //AddChartTextFixed("txt" + Time[0].Date.ToString(), Time[0].Year.ToString(), TextPosition.BottomCenter);
                AddChartText("txt" + Time[0].Date.ToString(), Time[0].Year.ToString(), 0, Low[0], Color.Magenta);
                _year = Time[0].Year;
            }

            

        }

        /// <summary>
        /// Draws a rectangle in the chart to visualize the seasonality.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="col"></param>
        private void DrawAreaRectangle(IEnumerable<IBar> list, Color color, string name)
        {
            double low = list.Min(x => x.Low);
            double high = list.Max(x => x.High);

            double difference = list.Last().Close - list.First().Open;

            //We need to get ensure that each name is unique.
            name = name + list.First().Time.Ticks;

            AddChartRectangle("Seasonal_rect" + name, true, list.First().Time, high, list.Last().Time, low, color, color, 70);
            AddChartText("Seasonal_text" + name, true, Math.Round((difference), 2).ToString(), list.First().Time, high, 7, Color.Black, new Font("Arial", 9), StringAlignment.Center, Color.Gray, color, 100);
        }


       



        public override string ToString()
        {
            return "Fiscal Year End";
        }

        public override string DisplayName
        {
            get
            {
                  return "Fiscal Year End";
            }
        }


        /// <summary>
        /// True if the periodicity of the data feed is correct for this indicator.
        /// </summary>
        /// <returns></returns>
        public bool DatafeedPeriodicityIsValid(ITimeFrame timeframe)
        {
            TimeFrame tf = (TimeFrame)timeframe;
            if (tf.Periodicity == DatafeedHistoryPeriodicity.Day && tf.PeriodicityValue == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




		#region Properties

        #region InSeries 
        
       


      
  

        //[Description("Start month of the manual seasonal type")]
        //[InputParameter]
        //[DisplayName("Start month (manual)")]
        //public int Start_Month
        //{
        //    get { return _start_month; }
        //    set { _start_month = value; }
        //}

        //[Description("End month of the manual seasonal type")]
        //[InputParameter]
        //[DisplayName("End month (manual)")]
        //public int End_Month
        //{
        //    get { return _end_month; }
        //    set { _end_month = value; }
        //}

        #endregion

        //[Browsable(false)]
        //[XmlIgnore()]
        //public DataSeries MyPlot1
        //{
        //    get { return Outputs[0]; }
        //}




        #endregion
    }
}
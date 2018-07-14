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
/// Version: 1.2.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// The initial version was inspired by this link: http://emini-watch.com/stock-market-seasonal-trades/5701/
/// -------------------------------------------------------------------------
/// todo 
/// tax time, september effect, Thanksgiving
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
  
    /// <summary>
    /// Differend types of seasonal indictors.
    /// </summary>
    public enum SeasonalDateType
    {
        SellInMay = 1,
        SantaClausRally = 2,
        fourthofjuly = 3,
        manual = 100
    }

    [Description("Show seasonal trends")]
	public class SeasonalDate_Indicator : UserIndicator
	{

        #region Variables

        //input
        private SeasonalDateType _seasonal = SeasonalDateType.SellInMay;
        private int _start_month = 2;
        private int _end_month = 10;

        //output


        //internal
        //Save data into hashset for a performance boost on the contains method
        private HashSet<DateTime> hashset = null;
        //sell in may
        private IEnumerable<IBar> _list_sellinmayandgoaway_buy = null;
        private IEnumerable<IBar> _list_sellinmayandgoaway_sell = null;
        private IBar _last_start_sellinmayandgoway = null;
        private IBar _last_end_sellinmayandgoway = null;
        //santa claus
        private IEnumerable<IBar> _list_santaclausrally_buy = null;
        private IBar _last_start_santaclausrally = null;
        //4th of july
        private IEnumerable<IBar> _list_fourthofjuly_buy = null;
        private IBar _last_start_fourthofjuly = null;
        //manual
        private IEnumerable<IBar> _list_manual_buy = null;
        private IBar _last_start_manual = null;

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

            switch (SeasonalDateType)
            {
                case SeasonalDateType.SellInMay:
                    _list_sellinmayandgoaway_buy = Bars.Where(x => x.Time.Month <= 4 || x.Time.Month >= 10);
                    _list_sellinmayandgoaway_sell = Bars.Except(_list_sellinmayandgoaway_buy);
                    hashset = new HashSet<DateTime>(_list_sellinmayandgoaway_buy.Select(x => x.Time));
                    break;
                case SeasonalDateType.SantaClausRally:
                    _list_santaclausrally_buy = from b in Bars
                                               where (b.Time.Month == 12 && b.Time.Day >= 15) || (b.Time.Month == 1 && b.Time.Day <= 9)
                                               select b;
                    hashset = new HashSet<DateTime>(_list_santaclausrally_buy.Select(x => x.Time));
                    break;
                case SeasonalDateType.fourthofjuly:
                    _list_fourthofjuly_buy = from b in Bars
                                                where (b.Time.Month == 7 && b.Time.Day >= 1) || (b.Time.Month == 7 && b.Time.Day <= 8)
                                                select b;
                    hashset = new HashSet<DateTime>(_list_fourthofjuly_buy.Select(x => x.Time));
                    break;
                case SeasonalDateType.manual:
                    _list_manual_buy = from b in Bars
                                             where (b.Time.Month >= this.Start_Month  && b.Time.Month <= this.End_Month)
                                             select b;
                    hashset = new HashSet<DateTime>(_list_manual_buy.Select(x => x.Time));
                    break;
                default:
                    break;
            }

            CalculateOnClosedBar = true;
            IsOverlay = true;
        }

		protected override void OnCalculate()
		{
            //Check if peridocity is valid for this script
            if (this.IsProcessingBarIndexLast && !DatafeedPeriodicityIsValid(Bars.TimeFrame))
            {
                GlobalUtilities.DrawWarningTextOnChart(this, Const.DefaultStringDatafeedPeriodicity);
            }

            switch (SeasonalDateType)
            {
                case SeasonalDateType.SellInMay:
                    this.calculate_Sell_in_May();
                    break;
                case SeasonalDateType.SantaClausRally:
                    this.calculate_Santa_Claus_Rally();
                    break;
                case SeasonalDateType.fourthofjuly:
                    this.calculate_4th_of_July_Rally();
                    break;
                case SeasonalDateType.manual:
                    this.calculate_manual();
                    break;
                default:
                    break;
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


        #region calculate methods

        
        /// <summary>
        /// Calculate the seasonal indicator for "manual".
        /// </summary>
        private void calculate_manual()
        {
            if (hashset.Contains(Bars[0].Time))
            {
                if (this._last_start_manual == null)
                {
                    this._last_start_manual = Bars[0];
                }
            }
            else
            {
                if (this._last_start_manual != null)
                {
                    DrawAreaRectangle(this._list_manual_buy.Where(x => x.Time >= this._last_start_manual.Time).Where(x => x.Time <= Bars[0].Time), Color.Green, "manual_buy");

                    this._last_start_manual = null;
                }
            }

        }

        /// <summary>
        /// Calculate the seasonal indicator for "4th of July".
        /// </summary>
        private void calculate_4th_of_July_Rally()
        {
            if (hashset.Contains(Bars[0].Time))
            {
                if (this._last_start_fourthofjuly == null)
                {
                    this._last_start_fourthofjuly = Bars[0];
                }
            }
            else
            {
                if (this._last_start_fourthofjuly != null)
                {
                    DrawAreaRectangle(this._list_fourthofjuly_buy.Where(x => x.Time >= this._last_start_fourthofjuly.Time).Where(x => x.Time <= Bars[0].Time), Color.Green, "fourthofjuly_buy");

                    this._last_start_fourthofjuly = null;
                }
            }

        }

        /// <summary>
        /// Calculate the seasonal indicator for "Santa Claus Rally".
        /// </summary>
        private void calculate_Santa_Claus_Rally()
        {
            if (hashset.Contains(Bars[0].Time))
            {
                if (this._last_start_santaclausrally == null)
                {
                    this._last_start_santaclausrally = Bars[0];
                }
            }
            else
            {
                if (this._last_start_santaclausrally != null)
                {
                    DrawAreaRectangle(this._list_santaclausrally_buy.Where(x => x.Time >= this._last_start_santaclausrally.Time).Where(x => x.Time <= Bars[0].Time), Color.Green, "santaclausrally_buy");

                    this._last_start_santaclausrally = null;
                }
            }
        }


        /// <summary>
        /// Calculate the seasonal indicator for "Sell in May".
        /// </summary>
        private void calculate_Sell_in_May()
        {
            if (hashset.Contains(Bars[0].Time))
            {
                if (_last_start_sellinmayandgoway == null)
                {
                    this._last_start_sellinmayandgoway = Bars[0];
                }

                if (_last_end_sellinmayandgoway != null)
                {
                    DrawAreaRectangle(_list_sellinmayandgoaway_sell.Where(x => x.Time >= _last_end_sellinmayandgoway.Time).Where(x => x.Time <= Bars[0].Time), Color.Red, "sellinmay_sell");

                    _last_end_sellinmayandgoway = null;
                }

            }
            else
            {
                if (_last_end_sellinmayandgoway == null)
                {
                    this._last_end_sellinmayandgoway = Bars[0];
                }

                if (_last_start_sellinmayandgoway != null)
                {
                    DrawAreaRectangle(_list_sellinmayandgoaway_buy.Where(x => x.Time >= _last_start_sellinmayandgoway.Time).Where(x => x.Time <= Bars[0].Time), Color.Green, "sellinmay_buy");

                    _last_start_sellinmayandgoway = null;
                }
            }
        }

        #endregion




        public override string ToString()
        {
            return "Seasonal";
        }

        public override string DisplayName
        {
            get
            {
                return "Seasonal";
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
        
            /// <summary>
            /// </summary>
            [Description("Seasonal Type")]
            [InputParameter]
            [DisplayName("Seasonal Type")]
            public SeasonalDateType SeasonalDateType
            {
                get { return _seasonal; }
                set { _seasonal = value; }
            }


      
  

        [Description("Start month of the manual seasonal type")]
        [InputParameter]
        [DisplayName("Start month (manual)")]
        public int Start_Month
        {
            get { return _start_month; }
            set { _start_month = value; }
        }

        [Description("End month of the manual seasonal type")]
        [InputParameter]
        [DisplayName("End month (manual)")]
        public int End_Month
        {
            get { return _end_month; }
            set { _end_month = value; }
        }

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
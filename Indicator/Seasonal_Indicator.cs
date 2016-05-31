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
/// Version: 1.1.1
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// The initial version was inspired by this link: http://emini-watch.com/stock-market-seasonal-trades/5701/
/// -------------------------------------------------------------------------
/// todo 
/// tax time, september effect, Thanksgiving
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
  
    /// <summary>
    /// Differend types of seasonal indictors.
    /// </summary>
    public enum SeasonalType
    {
        SellInMay = 1,
        SantaClausRally = 2,
        fourthofjuly = 3
    }

    [Description("Show seasonal trends")]
	public class Seasonal_Indicator : UserIndicator
	{

        #region Variables

        //input
        private SeasonalType _seasonal = SeasonalType.SellInMay;


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

        #endregion


		protected override void Initialize()
		{
			//Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
			Overlay = true;
		}

        protected override void InitRequirements()
        {
            //Print("InitRequirements");
        }
   

        protected override void OnStartUp()
        {
            //Print("OnStartUp");

            switch (SeasonalType)
            {
                case SeasonalType.SellInMay:
                    _list_sellinmayandgoaway_buy = Bars.Where(x => x.Time.Month <= 4 || x.Time.Month >= 10);
                    _list_sellinmayandgoaway_sell = Bars.Except(_list_sellinmayandgoaway_buy);
                    hashset = new HashSet<DateTime>(_list_sellinmayandgoaway_buy.Select(x => x.Time));
                    break;
                case SeasonalType.SantaClausRally:
                    _list_santaclausrally_buy = from b in Bars
                                               where (b.Time.Month == 12 && b.Time.Day >= 15) || (b.Time.Month == 1 && b.Time.Day <= 9)
                                               select b;
                    hashset = new HashSet<DateTime>(_list_santaclausrally_buy.Select(x => x.Time));
                    break;
                case SeasonalType.fourthofjuly:
                    _list_fourthofjuly_buy = from b in Bars
                                                where (b.Time.Month == 7 && b.Time.Day >= 1) || (b.Time.Month == 7 && b.Time.Day <= 8)
                                                select b;
                    hashset = new HashSet<DateTime>(_list_fourthofjuly_buy.Select(x => x.Time));
                    break;
                default:
                    break;
            }

            CalculateOnBarClose = true;
            Overlay = true;
        }

		protected override void OnBarUpdate()
		{
            //Check if peridocity is valid for this script
            if (this.IsCurrentBarLast && !DatafeedPeriodicityIsValid(Bars.TimeFrame))
            {
                GlobalUtilities.DrawWarningTextOnChart(this, Const.DefaultStringDatafeedPeriodicity);
            }

            switch (SeasonalType)
            {
                case SeasonalType.SellInMay:
                    this.calculate_Sell_in_May();
                    break;
                case SeasonalType.SantaClausRally:
                    this.calculate_Santa_Claus_Rally();
                    break;
                case SeasonalType.fourthofjuly:
                    this.calculate_4th_of_July_Rally();
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

            DrawRectangle("Seasonal_rect" + name, true, list.First().Time, high, list.Last().Time, low, color, color, 70);
            DrawText("Seasonal_text" + name, true, Math.Round((difference), 2).ToString(), list.First().Time, high, 7, Color.Black, new Font("Arial", 9), StringAlignment.Center, Color.Gray, color, 100);
        }


        #region calculate methods
       
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

        #region Input 
        
            /// <summary>
            /// </summary>
            [Description("Seasonal Type")]
            [Category("Parameters")]
            [DisplayName("Seasonal Type")]
            public SeasonalType SeasonalType
            {
                get { return _seasonal; }
                set { _seasonal = value; }
            }

	    #endregion

        //[Browsable(false)]
        //[XmlIgnore()]
        //public DataSeries MyPlot1
        //{
        //    get { return Values[0]; }
        //}


        

		#endregion
	}
}
#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator : Indicator
	{
		/// <summary>
		/// Show seasonal trends
		/// </summary>
		public Seasonal_Indicator Seasonal_Indicator(SeasonalType seasonalType)
        {
			return Seasonal_Indicator(Input, seasonalType);
		}

		/// <summary>
		/// Show seasonal trends
		/// </summary>
		public Seasonal_Indicator Seasonal_Indicator(IDataSeries input, SeasonalType seasonalType)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Seasonal_Indicator>(input, i => i.SeasonalType == seasonalType);

			if (indicator != null)
				return indicator;

			indicator = new Seasonal_Indicator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							SeasonalType = seasonalType
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
		/// Show seasonal trends
		/// </summary>
		public Seasonal_Indicator Seasonal_Indicator(SeasonalType seasonalType)
		{
			return LeadIndicator.Seasonal_Indicator(Input, seasonalType);
		}

		/// <summary>
		/// Show seasonal trends
		/// </summary>
		public Seasonal_Indicator Seasonal_Indicator(IDataSeries input, SeasonalType seasonalType)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.Seasonal_Indicator(input, seasonalType);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Show seasonal trends
		/// </summary>
		public Seasonal_Indicator Seasonal_Indicator(SeasonalType seasonalType)
		{
			return LeadIndicator.Seasonal_Indicator(Input, seasonalType);
		}

		/// <summary>
		/// Show seasonal trends
		/// </summary>
		public Seasonal_Indicator Seasonal_Indicator(IDataSeries input, SeasonalType seasonalType)
		{
			return LeadIndicator.Seasonal_Indicator(input, seasonalType);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Show seasonal trends
		/// </summary>
		public Seasonal_Indicator Seasonal_Indicator(SeasonalType seasonalType)
		{
			return LeadIndicator.Seasonal_Indicator(Input, seasonalType);
		}

		/// <summary>
		/// Show seasonal trends
		/// </summary>
		public Seasonal_Indicator Seasonal_Indicator(IDataSeries input, SeasonalType seasonalType)
		{
			return LeadIndicator.Seasonal_Indicator(input, seasonalType);
		}
	}

	#endregion

}

#endregion

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
/// Version: in progress
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// The initial version of this strategy was inspired by this link: http://emini-watch.com/stock-market-seasonal-trades/5701/
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this indicator without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Show seasonal trends")]
	public class Seasonal_Indicator : UserIndicator
	{
		protected override void Initialize()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
			Overlay = true;
		}

        protected override void InitRequirements()
        {
            //Print("InitRequirements");

        }

        private IEnumerable<IBar> list_sellinmayandgoaway_buy = null;
        private IEnumerable<IBar> list_sellinmayandgoaway_sell = null;
        private IBar last_start_sellinmayandgoway = null;
        private IBar last_end_sellinmayandgoway = null;

        protected override void OnStartUp()
        {
            //Print("OnStartUp");
            list_sellinmayandgoaway_buy = Bars.Where(x => x.Time.Month <= 4 || x.Time.Month >= 10);
            list_sellinmayandgoaway_sell = Bars.Where(x => x.Time.Month >= 5 && x.Time.Month <= 9);
        }

		protected override void OnBarUpdate()
		{
			MyPlot1.Set(Input[0]);

            if (list_sellinmayandgoaway_buy.Select(x => x.Time).Contains(Bars[0].Time))
            {
                if (last_start_sellinmayandgoway == null)
                {
                    this.last_start_sellinmayandgoway = Bars[0];

                }

                if (last_end_sellinmayandgoway != null)
                {
                    IEnumerable<IBar> area = list_sellinmayandgoaway_sell.Where(x => x.Time >= last_end_sellinmayandgoway.Time).Where(x => x.Time <= Bars[0].Time);
                    double low = area.Min(x => x.Low);
                    double high = area.Max(x => x.High);

                    double difference = Bars[0].Close - this.last_end_sellinmayandgoway.Open;

                    DrawRectangle("sellinmayRect_sell" + Bars[0].Time.ToString(), true, this.last_end_sellinmayandgoway.Time, high, Bars[0].Time, low, Color.Red, Color.Red, 70);
                    DrawText("sellinmayString_sell" + Bars[0].Time.ToString(), true, Math.Round((difference), 2).ToString(), this.last_end_sellinmayandgoway.Time, high, 9, Color.Black, new Font("Arial", 9), StringAlignment.Center, Color.Gray, Color.Red, 70);

                    last_end_sellinmayandgoway = null;
                }


            }
            else
            {
                if (last_end_sellinmayandgoway == null)
                {
                    this.last_end_sellinmayandgoway = Bars[0];

                }

                if (last_start_sellinmayandgoway != null)
                {
                    IEnumerable<IBar> area = list_sellinmayandgoaway_buy.Where(x => x.Time >= last_start_sellinmayandgoway.Time).Where(x => x.Time <= Bars[0].Time);
                    double low = area.Min(x=>x.Low);
                    double high = area.Max(x=>x.High);

                    double difference = Bars[0].Close - this.last_start_sellinmayandgoway.Open;

                    DrawRectangle("sellinmayRect_buy" + Bars[0].Time.ToString(), true, this.last_start_sellinmayandgoway.Time, high, Bars[0].Time, low, Color.Green, Color.Green, 70);
                    DrawText("sellinmayString_buy" + Bars[0].Time.ToString(), true, Math.Round((difference), 2).ToString(), this.last_start_sellinmayandgoway.Time, high, 9, Color.Black, new Font("Arial", 9), StringAlignment.Center, Color.Gray, Color.Green, 70);

                    last_start_sellinmayandgoway = null;
                }

            }
		}



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

		#region Properties

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries MyPlot1
		{
			get { return Values[0]; }
		}

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
		public Seasonal_Indicator Seasonal_Indicator()
        {
			return Seasonal_Indicator(Input);
		}

		/// <summary>
		/// Show seasonal trends
		/// </summary>
		public Seasonal_Indicator Seasonal_Indicator(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Seasonal_Indicator>(input);

			if (indicator != null)
				return indicator;

			indicator = new Seasonal_Indicator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input
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
		public Seasonal_Indicator Seasonal_Indicator()
		{
			return LeadIndicator.Seasonal_Indicator(Input);
		}

		/// <summary>
		/// Show seasonal trends
		/// </summary>
		public Seasonal_Indicator Seasonal_Indicator(IDataSeries input)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.Seasonal_Indicator(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Show seasonal trends
		/// </summary>
		public Seasonal_Indicator Seasonal_Indicator()
		{
			return LeadIndicator.Seasonal_Indicator(Input);
		}

		/// <summary>
		/// Show seasonal trends
		/// </summary>
		public Seasonal_Indicator Seasonal_Indicator(IDataSeries input)
		{
			return LeadIndicator.Seasonal_Indicator(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Show seasonal trends
		/// </summary>
		public Seasonal_Indicator Seasonal_Indicator()
		{
			return LeadIndicator.Seasonal_Indicator(Input);
		}

		/// <summary>
		/// Show seasonal trends
		/// </summary>
		public Seasonal_Indicator Seasonal_Indicator(IDataSeries input)
		{
			return LeadIndicator.Seasonal_Indicator(input);
		}
	}

	#endregion

}

#endregion

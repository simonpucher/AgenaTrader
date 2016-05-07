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
/// todo tax time, 4th of july, september effect, Thanksgiving
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this indicator without any error you also need access to the utility indicator to use these global source code elements.
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
        SantaClausRally = 2
    }

    [Description("Show seasonal trends")]
	public class Seasonal_Indicator : UserIndicator
	{
      

        #region Variables

        //input
        private SeasonalType _seasonal = SeasonalType.SantaClausRally;

        //output


        //internal
        private RectangleF _rect;
        private Brush _brush = Brushes.Gray;

        #endregion


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

        private IEnumerable<IBar> list_santaclausrally_buy = null;
        private IBar last_start_santaclausrally = null;

        protected override void OnStartUp()
        {
            //Print("OnStartUp");

            //// Add event listener
            //if (ChartControl != null)
            //    ChartControl.ChartPanelMouseDown += OnChartPanelMouseDown;

            //Sell in May
            list_sellinmayandgoaway_buy = Bars.Where(x => x.Time.Month <= 4 || x.Time.Month >= 10);
            list_sellinmayandgoaway_sell = Bars.Except(list_sellinmayandgoaway_buy);

            //Santa Claus Rally
            list_santaclausrally_buy = from b in Bars
                                       where (b.Time.Month == 12 && b.Time.Day >= 15) || (b.Time.Month == 1 && b.Time.Day <= 9) 
                                       select b;
 
                //Bars.Where(x => x.Time.Month == 12 && x.Time.Day >= 15).Where(x => x.Time.Month == 1 && x.Time.Day <= 9);
        }

		protected override void OnBarUpdate()
		{
			MyPlot1.Set(Input[0]);

            switch (SeasonalType)
            {
                case SeasonalType.SellInMay:
                    this.calculate_Sell_in_May();
                    break;
                case SeasonalType.SantaClausRally:
                    this.calculate_Santa_Claus_Rally();
                    break;
                default:
                    break;
            }
        
            
		}

        protected override void OnTermination()
        {
            //// Remove event listener
            //if (ChartControl != null)
            //    ChartControl.ChartPanelMouseDown -= OnChartPanelMouseDown;
        }

        /// <summary>
        /// Calculate the seasonal indicator for "Santa Claus Rally".
        /// </summary>
        private void calculate_Santa_Claus_Rally()
        {
            if (list_santaclausrally_buy.Select(x => x.Time).Contains(Bars[0].Time))
            {
                if (last_start_santaclausrally == null)
                {
                    this.last_start_santaclausrally = Bars[0];
                }
            }
            else
            {
                if (last_start_santaclausrally != null)
                {
                    IEnumerable<IBar> area = list_santaclausrally_buy.Where(x => x.Time >= last_start_santaclausrally.Time).Where(x => x.Time <= Bars[0].Time);
                    double low = area.Min(x => x.Low);
                    double high = area.Max(x => x.High);

                    double difference = Bars[0].Close - this.last_start_santaclausrally.Open;

                    DrawRectangle("sellinmayRect_buy" + Bars[0].Time.ToString(), true, this.last_start_santaclausrally.Time, high, Bars[0].Time, low, Color.Green, Color.Green, 70);
                    DrawText("sellinmayString_buy" + Bars[0].Time.ToString(), true, Math.Round((difference), 2).ToString(), this.last_start_santaclausrally.Time, high, 9, Color.Black, new Font("Arial", 9), StringAlignment.Center, Color.Gray, Color.Green, 100);

                    last_start_santaclausrally = null;
                }
            }
        }


        /// <summary>
        /// Calculate the seasonal indicator for "Sell in May".
        /// </summary>
        private void calculate_Sell_in_May() {
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
                    double low = area.Min(x => x.Low);
                    double high = area.Max(x => x.High);

                    double difference = Bars[0].Close - this.last_start_sellinmayandgoway.Open;

                    DrawRectangle("sellinmayRect_buy" + Bars[0].Time.ToString(), true, this.last_start_sellinmayandgoway.Time, high, Bars[0].Time, low, Color.Green, Color.Green, 70);
                    DrawText("sellinmayString_buy" + Bars[0].Time.ToString(), true, Math.Round((difference), 2).ToString(), this.last_start_sellinmayandgoway.Time, high, 9, Color.Black, new Font("Arial", 9), StringAlignment.Center, Color.Gray, Color.Green, 70);

                    last_start_sellinmayandgoway = null;
                }
            }
        }


        //#region Events



        //public override void Plot(Graphics g, Rectangle r, double min, double max)
        //{
        //    if (Bars == null || ChartControl == null) return;

        //    //Only draw button if parameters are available.
        //    if (this.Instrument != null)
        //    {
        //        using (Font font1 = new Font("Arial", 10, FontStyle.Bold, GraphicsUnit.Point))
        //        {
        //            StringFormat stringFormat = new StringFormat();
        //            stringFormat.Alignment = StringAlignment.Center;
        //            stringFormat.LineAlignment = StringAlignment.Center;

        //            //if (String.IsNullOrEmpty(Shortcut_list))
        //            //{
        //            //    if (this.Name_of_list.Count() >= 5)
        //            //    {
        //            //        this.Shortcut_list = this.Name_of_list.Substring(0, 5);
        //            //    }
        //            //    else
        //            //    {
        //            //        this.Shortcut_list = this.Name_of_list;
        //            //    }
        //            //}

        //            Brush tempbrush = new SolidBrush(GlobalUtilities.AdjustOpacity(((SolidBrush)_brush).Color, 0.5F));

        //            _rect = new RectangleF(r.Width - 100, 10, 86, 27);
        //            g.FillRectangle(tempbrush, _rect);
        //            g.DrawString("bubu", font1, Brushes.White, _rect, stringFormat);
        //            //g.DrawRectangle(_pen, Rectangle.Round(_rect));
        //        }
        //    }
        //}


       

        //private void OnChartPanelMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    //Print("X = {0}, Y = {1}", e.X, e.Y);
        //    //Print("X = {0}, Y = {1}", ChartControl.GetDateTimeByX(e.X), ChartControl.GetPriceByY(e.Y));

        //    Point cursorPos = new Point(e.X, e.Y);
        //    if (_rect.Contains(cursorPos))
        //    {
        //        DatafeedHistoryRequest mumu = this.ChartControl.HistoryRequest;
        //        mumu.DaysBack = 1000;
        //        //mumu.SetFromDate(DateTime.Now.AddDays(1000));
        //        //mumu.CalculateStartDate();

        //        this.Root.Core.ChartManager.FormatWindowCaption2HistoryRequest(mumu);

        //        this.Root.Core.ChartManager.ReloadCharts(this.Instrument);

        //        //this.ShowEditor(this.Root.Core.GuiManager.MainWindow);
                
       

        //        //this.Root.Core.ChartManager.Se

        //        //DatafeedHistoryRequestInfo bubu = new DatafeedHistoryRequestInfo();
        //        //DatafeedHistoryRequest mumu = bubu.Request;

        //        // mumu.BarsCount = 1000;
        //        //// this.Root.Core.HistoryManager.UpdateBar(mumu, null);

        //        //if (!_list.Contains((Instrument)this.Instrument))
        //        //{
        //        //    this.Root.Core.InstrumentManager.AddInstrument2List(this.Instrument, this.Name_of_list);
        //        //}
        //        //else
        //        //{
        //        //    this.Root.Core.InstrumentManager.RemoveInstrumentFromList(this.Name_of_list, this.Instrument);
        //        //}
        //    }
        //    else
        //    {
        //        //nothing to do
        //    }

        //    this.OnBarUpdate();

        //}

        //#endregion


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

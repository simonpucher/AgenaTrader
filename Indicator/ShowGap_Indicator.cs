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
	[Description("Prüft auf Gap und ob die nachfolgenden 15Mins Kerzen den Gap verstärken oder aufheben")]
	public class ShowGap_Indicator : UserIndicator
	{

        decimal _PunkteGapMin = 50;
        decimal _PunkteGapMax = 100;
        private Color _col_gap = Color.Turquoise;
        private Color colWin = Color.Yellow;
        private Color colFail = Color.Brown;
        bool existgap;
        bool GapTradeShort;
        bool GapTradeLong;
        bool sessionprocessed = true;
        decimal GapTradeCounterShort;
        decimal GapTradeCounterLong;
        decimal GapTradeResultTotalShort;
        decimal GapTradeResultTotalLong;
        decimal GapTradeFailCounterShort;
        decimal GapTradeFailCounterLong;
        decimal GapTradeWinCounterShort;
        decimal GapTradeWinCounterLong;
        double GapOpen;
        double GapSize;

		protected override void Initialize()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Square, "MyGap"));
			Overlay = true;
			CalculateOnBarClose = true;
            ClearOutputWindow();
            //sc = GetScriptedCondition("ShowGap_Condition") as ShowGap_Condition;
		}

        protected override void OnBarUpdate()
        {

      //      sc.process_ShowGap(Bars, CurrentBar, Time, Closes);

            process_ShowGap(Bars, CurrentBar, Time, Closes);

            MyGap.Set(Input[0]);
        }


        public void process_ShowGap(IBars iv_Bars, int iv_CurrentBar, ITimeSeries iv_Time, ISeriesCollection<IDataSeries> iv_Closes)
        {
            if (iv_Bars != null && iv_Bars.Count > 0)
            //             && TimeFrame.Periodicity == DatafeedHistoryPeriodicity.Minute
            //             && TimeFrame.PeriodicityValue == 15) 
            { }
            else
            {
                return;
            }

            if (iv_Bars.BarsSinceSession == 0)
            {
                sessionprocessed = false;
            }

            TimeSpan openingtime = GlobalUtilities.GetOfficialMarketOpeningTime(Instrument.Symbol);

            if (iv_Bars.GetTime(iv_CurrentBar).TimeOfDay > openingtime //größer 09.00 geht für 15M und 1Std (und 1Tag?)
            && sessionprocessed == false)        //Tag noch nicht verarbeitet
            {
                sessionprocessed = true;
                GapTradeLong = GapTradeShort = false;

                IBar GapOpenBar = GlobalUtilities.GetFirstBarOfCurrentSession(iv_Bars, iv_Bars[0].Time.Date);
                GapOpen = GapOpenBar.Open;

//                double LastDayClose = PriorDayOHLC(Closes[0]).PriorClose[iv_Bars.Count - iv_CurrentBar];
                double LastDayClose = PriorDayOHLC(Closes[0]).PriorClose[0];

                //Wenn es keinen LastDayClose Wert gibt -> raus (immer am Anfang des Charts)
                if (LastDayClose == 0)
                {
                    return;
                }


                GapSize = GapOpen - LastDayClose;
                DateTime LastDayCloseDate = iv_Bars.GetTime(Count - 7);
                DateTime LastPeriod = Time[1];

                if (LastDayClose != null
                && Math.Abs(GapSize) > 0)
                {
                    existgap = true;


                    //Gap markieren (08.00 - 09.15)
                    string strMyRect = "MyRect" + Count;
                    string strMyGapSize = "MyGapSize" + Count;


                    //                    if (LastDayClose - GapOpen < 0)   
                    if (IsUpwardsGap() == true)
                    {
                        //Long
                        DrawRectangle(strMyRect, true, LastDayCloseDate, LastDayClose, LastPeriod, GlobalUtilities.GetHighestHigh(iv_Bars, 5), _col_gap, _col_gap, 70);
                        DrawText(strMyGapSize, true, Math.Round(GapSize, 1).ToString(), LastDayCloseDate, LastDayClose + (500*TickSize), 9, Color.Black, new Font("Areal", 9), StringAlignment.Center, Color.Black, Color.Azure, 1);


                        if (GapIndicatesTradeLong(iv_Closes) == true)
                        {
                            //Chancenreicher SuccessTrade
                            PrepareTradeLong(iv_Bars, iv_CurrentBar);
                        }
                    }
                    else
                    {
                        //Short                 
                        DrawRectangle(strMyRect, true, LastDayCloseDate, LastDayClose, LastPeriod, GlobalUtilities.GetLowestLow(iv_Bars, 5), _col_gap, _col_gap, 70);
                        DrawText(strMyGapSize, true, Math.Round(GapSize, 1).ToString(), LastDayCloseDate, LastDayClose - (500 * TickSize), 9, Color.Black, new Font("Areal", 9), StringAlignment.Center, Color.Black, Color.Azure, 1);

                        if (GapIndicatesTradeShort(iv_Closes) == true)
                        {
                            ////Chancenreicher SuccessTrade
                            PrepareTradeShort(iv_Bars, iv_CurrentBar);
                        }
                    }

                    if (GapTradeShort == true || GapTradeLong == true)
                    {
                        PrintBasicTradeInfo();
                    }

                }
                else
                {
                    existgap = false;
                }
            }

//09.15. - 09.30 Kerze
            else if (iv_Bars.BarsSinceSession == 6 && existgap == true)
            {
                //Auswertung    
                printAuswertung(iv_Bars, iv_CurrentBar);
            }


        }



        private bool GapIndicatesTradeLong(ISeriesCollection<IDataSeries> iv_Closes)
        {
            if (LinReg(iv_Closes[0], 5)[0] > GapOpen
            && (decimal)GapSize > _PunkteGapMin
            && (decimal)GapSize < _PunkteGapMax)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private bool GapIndicatesTradeShort(ISeriesCollection<IDataSeries> iv_Closes)
        {
            if (LinReg(iv_Closes[0], 5)[0] < GapOpen
            && Math.Abs((decimal)GapSize) > _PunkteGapMin
            && Math.Abs((decimal)GapSize) < _PunkteGapMax)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private void PrepareTradeShort(IBars iv_Bars, int iv_CurrentBar)
        {
            string strArrowDown = "ArrowDown" + iv_Bars.GetTime(iv_CurrentBar);
            DrawArrowDown(strArrowDown, true, iv_Bars.GetTime(Count - 1), iv_Bars.GetOpen(iv_CurrentBar) + 300 * TickSize, Color.Red);
            GapTradeShort = true;

//            Occurred.Set(-1);
//            Entry.Set(iv_Bars.GetOpen(iv_CurrentBar));
        }


        private void PrepareTradeLong(IBars iv_Bars, int iv_CurrentBar)
        {
            string strArrowUp = "ArrowUp" + iv_Bars.GetTime(iv_CurrentBar);
            DrawArrowUp(strArrowUp, true, iv_Bars.GetTime(Count - 1), iv_Bars.GetOpen(iv_CurrentBar) - 300 * TickSize, Color.Green);
            GapTradeLong = true;

//            Occurred.Set(1);
//            Entry.Set(iv_Bars.GetOpen(iv_CurrentBar));
        }

        private bool IsUpwardsGap()
        {
            if (GapSize > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsDownwardsGap()
        {
            if (GapSize < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void PrintBasicTradeInfo()
        {
            Print("------------------" + Time[5] + "------------------");
            Print("LineReg: " + Math.Round(LinReg(5)[0]), 1);
            Print("Gap Size: " + GapSize);
        }


        private void printAuswertung(IBars iv_Bars, int iv_CurrentBar)
        {
            decimal GapTradeResult;
            Color colorTextBox;

            GapTradeResult = (decimal)iv_Bars.GetClose(iv_CurrentBar - 1) - (decimal)iv_Bars.GetOpen(iv_CurrentBar - 1);
            if (GapTradeLong == true)
            {
                //Long
                GapTradeCounterLong += 1;
                GapTradeResultTotalLong = GapTradeResultTotalLong + GapTradeResult;


                string strGapeTradeLong = "GapTradeLong" + iv_CurrentBar;
                string strTradeResultLong;

                if (GapTradeResult < 0)
                {
                    Print("Fail");
                    GapTradeFailCounterLong += 1;
                    strTradeResultLong = "Fail " + GapTradeResult.ToString();
                    colorTextBox = colFail;
                }
                else
                {
                    GapTradeWinCounterLong += 1;
                    strTradeResultLong = "Win " + GapTradeResult.ToString();
                    colorTextBox = colWin;
                }
                DrawText(strGapeTradeLong, true, strTradeResultLong, Time[1], iv_Bars.GetHigh(iv_CurrentBar - 1) + 10, 9, Color.Black, new Font("Areal", 9), StringAlignment.Center, Color.Black, colorTextBox, 70);

            }
            else if (GapTradeShort == true)
            {
                //Short
                GapTradeCounterShort += 1;
                GapTradeResultTotalShort = GapTradeResultTotalShort - GapTradeResult;


                string strGapeTradeShort = "GapTradeLong" + iv_CurrentBar;
                string strTradeResultShort;

                if (GapTradeResult > 0)
                {
                    Print("FAAAAAAAAAAAAAAAIIIIIIIIIIIIIILLLLLLLLLL");
                    GapTradeFailCounterShort += 1;
                    strTradeResultShort = "Fail " + GapTradeResult.ToString();
                    colorTextBox = colFail;
                }
                else
                {
                    GapTradeWinCounterShort += 1;
                    strTradeResultShort = "Win " + GapTradeResult.ToString();
                    colorTextBox = colWin;
                }
                DrawText(strGapeTradeShort, true, strTradeResultShort, Time[1], iv_Bars.GetLow(iv_CurrentBar - 1) - 10, 9, Color.Black, new Font("Areal", 9), StringAlignment.Center, Color.Black, colorTextBox, 70);
            }
            Print("Gap Trade Result: " + GapTradeResult);
        }


//            if (Bars != null && Bars.Count > 0 )                                           
////             && TimeFrame.Periodicity == DatafeedHistoryPeriodicity.Minute
////             && TimeFrame.PeriodicityValue == 15) 
//            {}
//            else{
//            return;
//            }

//            if (Bars.BarsSinceSession == 0) {
//                sessionprocessed = false;
//            }

////08.00, 08.15, 08.30, 08.45, 09.00 sind abgeschlossen -> es ist 09.15)
////                if(Bars.BarsSinceSession == 5)
//            if (ToTime(Bars.GetTime(CurrentBar)) > 90000 //größer 09.00 geht für 15M und 1Std (und 1Tag?)
//            && sessionprocessed == false        )        //Tag noch nicht verarbeitet
//                {
//                    sessionprocessed = true;
//                    GapTradeLong = GapTradeShort = false;

//                    IBar GapOpenBar = Bars.Where(x => x.Time.Date == Bars[0].Time.Date).FirstOrDefault(); //liefert erster kerze des tages
//                  double GapOpen = GapOpenBar.Open;    

//                double LastDayClose = PriorDayOHLC().PriorClose[0];
//                   double GapSize = GapOpen - LastDayClose;
//                   DateTime LastDayCloseDate = Bars.GetTime(Count - 7);
//                   DateTime LastPeriod = Time[1];

//                if (LastDayClose != null
//                && Math.Abs(LastDayClose - GapOpen) > _PunkteGapMin
//                && Math.Abs(LastDayClose - GapOpen) < _PunkteGapMax )
//                {  //Wenn Gap größer 50 und kleiner 100
//                    existgap = true;


////Gap markieren (08.00 - 09.15)
//                    string strMyRect = "MyRect" + Count;
//                    string strMyGapSize = "MyGapSize" + Count;


//                    if (LastDayClose - GapOpen < 0)   //Long
//                    {
////Long                        
//                        DrawRectangle(strMyRect, true, LastDayCloseDate, LastDayClose, LastPeriod, HighestHighPrice(5)[0], _col_gap, _col_gap, 70);
//                        DrawText(strMyGapSize, true, Math.Round(GapSize, 1).ToString(), LastDayCloseDate, LastDayClose + 25, 9, Color.Black, new Font("Areal", 9), StringAlignment.Center, Color.Black, Color.Azure, 1);

//                       // if (LinReg(5)[0] > GapOpen)
//                        if (LinReg(Closes[0], 5)[0] > GapOpen)
//                        {
////Chancenreicher SuccessTrade
//                            string strArrowUp = "ArrowUp" + Bars.GetTime(CurrentBar);
//                            DrawArrowUp(strArrowUp, true, Bars.GetTime(Count - 1), Bars.GetOpen(CurrentBar) - 300 * TickSize, Color.Green);
//                            GapTradeLong = true;
//                        }
//                    }
//                    else
//                    {                 
////Short                        
//                        DrawRectangle(strMyRect, true, LastDayCloseDate, LastDayClose, LastPeriod, LowestLowPrice(5)[0], Color.Pink, Color.Pink, 70);
//                        DrawText(strMyGapSize, true, Math.Round(GapSize, 1).ToString(), LastDayCloseDate, LastDayClose - 25, 9, Color.Black, new Font("Areal", 9), StringAlignment.Center, Color.Black, Color.Azure, 1);

//                        if (LinReg(Closes[0], 5)[0] < GapOpen)
//                        {
//////Chancenreicher SuccessTrade
//                            string strArrowDown = "ArrowDown" + Bars.GetTime(CurrentBar);
//                            DrawArrowDown(strArrowDown, true, Bars.GetTime(Count - 1), Bars.GetOpen(CurrentBar) + 300 * TickSize, Color.Red);
//                            GapTradeShort = true;
//                        }
//                    }

//                    if (GapTradeShort == true || GapTradeLong == true) { 
//                    Print("------------------" + Time[5] + "------------------");
//                    Print("LineReg: " + Math.Round(LinReg(5)[0]), 1);
//                    Print("Gap Open: " + GapOpen);
//                    }

//                }
//                else {
//                    existgap = false;
//                }

//                }

////09.15. - 09.30 Kerze
//                else if (Bars.BarsSinceSession == 6 && existgap == true) { 
////Auswertung
//                    decimal GapTradeResult;
//                    Color colorTextBox;
                    
//                    GapTradeResult = (decimal)Bars.GetClose(CurrentBar-1) - (decimal)Bars.GetOpen(CurrentBar-1);
//                    if (GapTradeLong == true) {
////Long
//                    GapTradeCounterLong += 1;
//                    GapTradeResultTotalLong = GapTradeResultTotalLong + GapTradeResult;

                    
//                    string strGapeTradeLong = "GapTradeLong" + CurrentBar;
//                    string strTradeResultLong;

//                    if (GapTradeResult < 0)
//                    {
//                        Print("FAAAAAAAAAAAAAAAIIIIIIIIIIIIIILLLLLLLLLL");
//                        GapTradeFailCounterLong += 1;
//                         strTradeResultLong = "Fail "+GapTradeResult.ToString();
//                         colorTextBox = colFail;
//                    }
//                    else
//                    {
//                        GapTradeWinCounterLong += 1;
//                         strTradeResultLong = "Win " + GapTradeResult.ToString();
//                         colorTextBox = colWin;
//                    }
//                    DrawText(strGapeTradeLong, true, strTradeResultLong, Time[1], Bars.GetHigh(CurrentBar - 1) + 10, 9, Color.Black, new Font("Areal", 9), StringAlignment.Center, Color.Black, colorTextBox, 70);

//                   }
//                    else if (GapTradeShort == true) {
////Short
//                        GapTradeCounterShort += 1;
//                        GapTradeResultTotalShort = GapTradeResultTotalShort - GapTradeResult;

                        
//                        string strGapeTradeShort = "GapTradeLong" + CurrentBar;
//                        string strTradeResultShort;

//                        if (GapTradeResult > 0)
//                        {
//                            Print("FAAAAAAAAAAAAAAAIIIIIIIIIIIIIILLLLLLLLLL");
//                            GapTradeFailCounterShort += 1;
//                             strTradeResultShort = "Fail " + GapTradeResult.ToString();
//                             colorTextBox = colFail;
//                        }
//                        else
//                        {
//                            GapTradeWinCounterShort += 1;
//                             strTradeResultShort = "Win " + GapTradeResult.ToString();
//                             colorTextBox = colWin;
//                        }
//                        DrawText(strGapeTradeShort, true, strTradeResultShort, Time[1], Bars.GetLow(CurrentBar - 1) - 10, 9, Color.Black, new Font("Areal", 9), StringAlignment.Center, Color.Black, colorTextBox, 70);


//                    }

//                    Print("Gap Trade Result: " + GapTradeResult);                
//                }

//                if (Count == Bars.Count-1)
//                { 
//                Print("Total Trades Long:  " + GapTradeCounterLong);
//                Print("Wins Long: " + GapTradeWinCounterLong);
//                Print("Fails Long: " + GapTradeFailCounterLong);

//                Print("Total Trades Short: " + GapTradeCounterShort);
//                Print("Wins Short: " + GapTradeWinCounterShort);
//                Print("Fails Short: " + GapTradeFailCounterShort);

//                if (GapTradeCounterLong > 0) { Print("Avg Long: " + (GapTradeResultTotalLong / GapTradeCounterLong)); } 
//                if (GapTradeCounterShort > 0) { Print("Avg Short: " + (GapTradeResultTotalShort / GapTradeCounterShort)); }

                
                
//                }


//		}

		#region Properties

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries MyGap
		{
			get { return Values[0]; }
		}


        [Description("Mind. Punkte für Gap")]
        [Category("Parameters")]
        [DisplayName("MinPunkte")]
        public decimal PunkteGapMin
        {
            get { return _PunkteGapMin; }
            set { _PunkteGapMin = value; }
        }

        [Description("Max. Punkte für Gap")]
        [Category("Parameters")]
        [DisplayName("MaxPunkte")]
        public decimal PunkteGapMax
        {
            get { return _PunkteGapMax; }
            set { _PunkteGapMax = value; }
        }



        [Description("Select Color")]
        [Category("Colors")]
        [DisplayName("Farbe Gap")]
        public Color Color_Gap
        {
            get { return _col_gap; }
            set { _col_gap = value; }
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
		/// Prüft auf Gap und ob die nachfolgenden 15Mins Kerzen den Gap verstärken oder aufheben
		/// </summary>
		public ShowGap_Indicator ShowGap_Indicator(System.Decimal punkteGapMin, System.Decimal punkteGapMax)
        {
			return ShowGap_Indicator(Input, punkteGapMin, punkteGapMax);
		}

		/// <summary>
		/// Prüft auf Gap und ob die nachfolgenden 15Mins Kerzen den Gap verstärken oder aufheben
		/// </summary>
		public ShowGap_Indicator ShowGap_Indicator(IDataSeries input, System.Decimal punkteGapMin, System.Decimal punkteGapMax)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<ShowGap_Indicator>(input, i => i.PunkteGapMin == punkteGapMin && i.PunkteGapMax == punkteGapMax);

			if (indicator != null)
				return indicator;

			indicator = new ShowGap_Indicator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							PunkteGapMin = punkteGapMin,
							PunkteGapMax = punkteGapMax
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
		/// Prüft auf Gap und ob die nachfolgenden 15Mins Kerzen den Gap verstärken oder aufheben
		/// </summary>
		public ShowGap_Indicator ShowGap_Indicator(System.Decimal punkteGapMin, System.Decimal punkteGapMax)
		{
			return LeadIndicator.ShowGap_Indicator(Input, punkteGapMin, punkteGapMax);
		}

		/// <summary>
		/// Prüft auf Gap und ob die nachfolgenden 15Mins Kerzen den Gap verstärken oder aufheben
		/// </summary>
		public ShowGap_Indicator ShowGap_Indicator(IDataSeries input, System.Decimal punkteGapMin, System.Decimal punkteGapMax)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.ShowGap_Indicator(input, punkteGapMin, punkteGapMax);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Prüft auf Gap und ob die nachfolgenden 15Mins Kerzen den Gap verstärken oder aufheben
		/// </summary>
		public ShowGap_Indicator ShowGap_Indicator(System.Decimal punkteGapMin, System.Decimal punkteGapMax)
		{
			return LeadIndicator.ShowGap_Indicator(Input, punkteGapMin, punkteGapMax);
		}

		/// <summary>
		/// Prüft auf Gap und ob die nachfolgenden 15Mins Kerzen den Gap verstärken oder aufheben
		/// </summary>
		public ShowGap_Indicator ShowGap_Indicator(IDataSeries input, System.Decimal punkteGapMin, System.Decimal punkteGapMax)
		{
			return LeadIndicator.ShowGap_Indicator(input, punkteGapMin, punkteGapMax);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Prüft auf Gap und ob die nachfolgenden 15Mins Kerzen den Gap verstärken oder aufheben
		/// </summary>
		public ShowGap_Indicator ShowGap_Indicator(System.Decimal punkteGapMin, System.Decimal punkteGapMax)
		{
			return LeadIndicator.ShowGap_Indicator(Input, punkteGapMin, punkteGapMax);
		}

		/// <summary>
		/// Prüft auf Gap und ob die nachfolgenden 15Mins Kerzen den Gap verstärken oder aufheben
		/// </summary>
		public ShowGap_Indicator ShowGap_Indicator(IDataSeries input, System.Decimal punkteGapMin, System.Decimal punkteGapMax)
		{
			return LeadIndicator.ShowGap_Indicator(input, punkteGapMin, punkteGapMax);
		}
	}

	#endregion

}

#endregion


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
/// 
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
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
        public double StopForShowGapTrade;

		protected override void Initialize()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Square, "MyGap"));
			Overlay = true;
			CalculateOnBarClose = true;
		}

        protected override void OnBarUpdate()
        {
            process_ShowGap();
        }


        public void process_ShowGap()
        {
//Prüfen, ob 15min Chart aktiviert ist, anonsten -> raus
            if (Bars != null && Bars.Count > 0
                         && TimeFrame.Periodicity == DatafeedHistoryPeriodicity.Minute
                         && TimeFrame.PeriodicityValue == 15) 
            { }
            else
            {
                return;
            }

//Prüfen, ob es sich um die erste Kerze des Tage handelt, nur mit dieser kann Gap berechnet werden
            if (Bars.BarsSinceSession == 0)
            {
                sessionprocessed = false;
            }

//Marktöffnungszeit für Instrument lesen
            TimeSpan openingtime = GlobalUtilities.GetOfficialMarketOpeningTime(Instrument.Symbol);

//Prüfen, ob aktuelle Kerze nach der offiziellen Marktöffnung ist, oder ob es eine vorbörsliche Kerze ist
            if (Bars.GetTime(CurrentBar).TimeOfDay > openingtime //größer 09.00 geht für 15M und 1Std (und 1Tag?)
            && sessionprocessed == false)        //Tag noch nicht verarbeitet
            {
                sessionprocessed = true;
                GapTradeLong = GapTradeShort = false;

//Erste Kerze des Tages besorgen und deren Errönfnungskurs lesen
                IBar GapOpenBar = GlobalUtilities.GetFirstBarOfCurrentSession(Bars, Bars[0].Time.Date);
                GapOpen = GapOpenBar.Open;

//Tagesschlusskurs von gestern besorgen
                double LastDayClose = PriorDayOHLC(Closes[0]).PriorClose[0];

                //Wenn es keinen LastDayClose Wert gibt -> raus (immer am Anfang des Charts)
                if (LastDayClose == 0)
                {
                    return;
                }

//gestrige Schlusskurs soll auch als InitialStop für etwaige Orders verwendet werden
                StopForShowGapTrade = LastDayClose;


//Gapgröße berechnen
                GapSize = GapOpen - LastDayClose;

//DateTime von gestrigen Schlusskurs
                DateTime LastDayCloseDate = Bars.GetTime(Count - 7);
                DateTime LastPeriod = Time[1];

//Prüfen ob es überhaupt ein Gap gab
                if (LastDayClose != null
                && Math.Abs(GapSize) > 0)
                {
                    existgap = true;


//Gap markieren (08.00 - 09.15)
                    string strMyRect = "MyRect" + Count;
                    string strMyGapSize = "MyGapSize" + Count;

//Gap zeigt nach oben
                    if (IsUpwardsGap() == true)
                    {
//Long
                        DrawRectangle(strMyRect, true, LastDayCloseDate, LastDayClose, LastPeriod, GlobalUtilities.GetHighestHigh(Bars, 5), _col_gap, _col_gap, 70);
                        DrawText(strMyGapSize, true, Math.Round(GapSize, 1).ToString(), GapOpenBar.Time, LastDayClose - 6, 9, Color.Black, new Font("Areal", 9), StringAlignment.Center, Color.Black, Color.Azure, 1);


                        if (GapIndicatesTradeLong() == true)
                        {
//Chancenreicher SuccessTrade Long
                            PrepareTradeLong();
                        }
                    }

//Gap zeigt nach unten
                    else
                    {
//Short                 
                        DrawRectangle(strMyRect, true, LastDayCloseDate, LastDayClose, LastPeriod, GlobalUtilities.GetLowestLow(Bars, 5), _col_gap, _col_gap, 70);
                        DrawText(strMyGapSize, true, Math.Round(GapSize, 1).ToString(), GapOpenBar.Time, LastDayClose + 6, 9, Color.Black, new Font("Areal", 9), StringAlignment.Center, Color.Black, Color.Azure, 1);

                        if (GapIndicatesTradeShort() == true)
                        {
////Chancenreicher SuccessTrade Short
                            PrepareTradeShort();
                        }
                    }

                    if (GapTradeShort == true || GapTradeLong == true)
                    {
//Auswertung
                        PrintBasicTradeInfo();
                    }

                }
                else
                {
                    existgap = false;
                }
            }

//09.15. - 09.30 Kerze
            else if (Bars.BarsSinceSession == 6 && existgap == true)
            {
//Auswertung nach ShowGap Handelszeit (=09.30)   
                printAuswertung();
            }
        }


//Ermittlung ob Trade chancenreich wäre (Long)
        private bool GapIndicatesTradeLong()
        {
            if (LinReg(Closes[0], 5)[0] > GapOpen
            && (decimal)GapSize > _PunkteGapMin
            && (decimal)GapSize < _PunkteGapMax)
            {
                Value.Set(100);
                return true;                
            }
            else
            {
                Value.Set(0);
                return false;
            }
        }

//Ermittlung ob Trade chancenreich wäre (Short)
        private bool GapIndicatesTradeShort()
        {
            if (LinReg(Closes[0], 5)[0] < GapOpen
            && Math.Abs((decimal)GapSize) > _PunkteGapMin
            && Math.Abs((decimal)GapSize) < _PunkteGapMax)
            {
                Value.Set(-100);
                return true;
            }
            else
            {
                Value.Set(0);
                return false;
            }
        }


//grafische Darstellung Einstieg Short
        private void PrepareTradeShort()
        {
            string strArrowDown = "ArrowDown" + Bars.GetTime(CurrentBar);
            DrawArrowDown(strArrowDown, true, Bars.GetTime(Count - 1), Bars.GetOpen(CurrentBar) + 300 * TickSize, Color.Red);
            GapTradeShort = true;
        }

//grafische Darstellung Einstieg Long
        private void PrepareTradeLong()
        {
            string strArrowUp = "ArrowUp" + Bars.GetTime(CurrentBar);
            DrawArrowUp(strArrowUp, true, Bars.GetTime(Count - 1), Bars.GetOpen(CurrentBar) - 300 * TickSize, Color.Green);
            GapTradeLong = true;
        }


//Prüfen, ob Gap nach oben zeigt
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

//Prüfen, ob Gap nach unten zeigt
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


        private void printAuswertung()
        {
            decimal GapTradeResult;
            Color colorTextBox;

            GapTradeResult = (decimal)Bars.GetClose(CurrentBar - 1) - (decimal)Bars.GetOpen(CurrentBar - 1);
            if (GapTradeLong == true)
            {
                //Long
                GapTradeCounterLong += 1;
                GapTradeResultTotalLong = GapTradeResultTotalLong + GapTradeResult;


                string strGapeTradeLong = "GapTradeLong" + CurrentBar;
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
                DrawText(strGapeTradeLong, true, strTradeResultLong, Time[1], Bars.GetHigh(CurrentBar - 1) + 10, 9, Color.Black, new Font("Areal", 9), StringAlignment.Center, Color.Black, colorTextBox, 70);

            }
            else if (GapTradeShort == true)
            {
                //Short
                GapTradeCounterShort += 1;
                GapTradeResultTotalShort = GapTradeResultTotalShort - GapTradeResult;


                string strGapeTradeShort = "GapTradeLong" + CurrentBar;
                string strTradeResultShort;

                if (GapTradeResult > 0)
                {
                    Print("Fail");
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
                DrawText(strGapeTradeShort, true, strTradeResultShort, Time[1], Bars.GetLow(CurrentBar - 1) - 10, 9, Color.Black, new Font("Areal", 9), StringAlignment.Center, Color.Black, colorTextBox, 70);
            }
            Print("Gap Trade Result: " + GapTradeResult);
        }


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


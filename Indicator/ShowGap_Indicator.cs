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
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// todo description
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("Prüft auf Gap und ob die nachfolgenden 15Mins Kerzen den Gap verstärken oder aufheben")]
	public class ShowGap_Indicator : UserIndicator
	{

        decimal _PunkteGapMinProz = 0.005m;
        decimal _PunkteGapMaxProz = 0.01m;
        decimal PunkteGapMin;
        decimal PunkteGapMax;
        private Color _col_gap = Color.Turquoise;
        private Color colWin = Color.Yellow;
        private Color colFail = Color.Brown;
        bool _print_info= false;
        bool _dev_mode = false;
        bool _Snapshots = false;
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
        string TradeDir;
        

		protected override void OnInit()
		{
			//Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Orange), OutputSerieDrawStyle.Square, "MyGap"));
            Add(new OutputDescriptor(Color.Orange, "MyGap"));
			IsOverlay = true;
			CalculateOnClosedBar = true;
		}

        protected override void OnCalculate()
        {
            process_ShowGap();

            if (IsProcessingBarIndexLast)
            {

            }

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
            if (Bars.BarsCountForSession == 0)
            {
                sessionprocessed = false;
            }

//Marktöffnungszeit für Instrument lesen
            TimeSpan openingtime = GlobalUtilities.GetOfficialMarketOpeningTime(Instrument.Symbol);

//Prüfen, ob aktuelle Kerze nach der offiziellen Marktöffnung ist, oder ob es eine vorbörsliche Kerze ist
            if (Bars.GetTime(ProcessingBarIndex).TimeOfDay > openingtime //größer 09.00 geht für 15M und 1Std (und 1Tag?)
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

                PunkteGapMax = (decimal)LastDayClose * _PunkteGapMaxProz;
                PunkteGapMin = (decimal)LastDayClose * _PunkteGapMinProz;

//gestrige Schlusskurs soll auch als InitialStop für etwaige Orders verwendet werden
                StopForShowGapTrade = LastDayClose;


//Gapgröße berechnen
                GapSize = GapOpen - LastDayClose;

//DateTime von gestrigen Schlusskurs

                IBar GapCloseBar = GlobalUtilities.GetLastBarOfLastSession(Bars, Bars[0].Time.Date);
                DateTime LastDayCloseDate = GapCloseBar.Time;
                DateTime LastPeriod = Time[1];

//Prüfen ob es überhaupt ein Gap gab
                if (LastDayClose > 0
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
                        AddChartRectangle(strMyRect, true, LastDayCloseDate, LastDayClose, LastPeriod, GlobalUtilities.GetHighestHigh(Bars, Bars.BarsCountForSession), _col_gap, _col_gap, 70);
                        AddChartText(strMyGapSize, true, Math.Round(GapSize, 1).ToString(), GapOpenBar.Time, LastDayClose - (60 * TickSize), 9, Color.Black, new Font("Arial", 9), StringAlignment.Center, Color.Black, Color.Azure, 1);


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
                        AddChartRectangle(strMyRect, true, LastDayCloseDate, LastDayClose, LastPeriod, GlobalUtilities.GetLowestLow(Bars, Bars.BarsCountForSession), _col_gap, _col_gap, 70);
                        AddChartText(strMyGapSize, true, Math.Round(GapSize, 1).ToString(), GapOpenBar.Time, LastDayClose + (60 * TickSize), 9, Color.Black, new Font("Arial", 9), StringAlignment.Center, Color.Black, Color.Azure, 1);

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
//            else if (Bars.BarsCountForSession == 6 && existgap == true)
            else if (Bars[0].Time.TimeOfDay == new TimeSpan(9,15,0))
            {
//Auswertung nach ShowGap Handelszeit (=09.30)   
                printAuswertung();
                GlobalUtilities.SaveSnapShot("ShowGap", Instrument.Name, this.Root.Core.ChartManager.AllCharts, Bars, TimeFrame);
            }
        }


//Ermittlung ob Trade chancenreich wäre (Long)
        private bool GapIndicatesTradeLong()
        {

            if (dev_mode == true)
            {
                OutSeries.Set(100);
                return true;
            }


            if (LinReg(Closes[0], Bars.BarsCountForSession)[0] > GapOpen
            && (decimal)GapSize > PunkteGapMin
            && (decimal)GapSize < PunkteGapMax)
            {
                OutSeries.Set(100);
                return true;                
            }
            else
            {
                OutSeries.Set(0);
                return false;
            }
        }

//Ermittlung ob Trade chancenreich wäre (Short)
        private bool GapIndicatesTradeShort()
        {

            if (dev_mode == true)
            {
                OutSeries.Set(-100);
                return true;
            }

            if (LinReg(Closes[0], Bars.BarsCountForSession)[0] < GapOpen
            && Math.Abs((decimal)GapSize) > PunkteGapMin
            && Math.Abs((decimal)GapSize) < PunkteGapMax)
            {
                OutSeries.Set(-100);
                return true;
            }
            else
            {
                OutSeries.Set(0);
                return false;
            }
        }


//grafische Darstellung Einstieg Short
        private void PrepareTradeShort()
        {
            string strArrowDown = "ArrowDown" + Bars.GetTime(ProcessingBarIndex);
            AddChartArrowDown(strArrowDown, true, Bars.GetTime(Count - 1), Bars.GetOpen(ProcessingBarIndex) + 100 * TickSize, Color.Red);
            GapTradeShort = true;
        }

//grafische Darstellung Einstieg Long
        private void PrepareTradeLong()
        {
            string strArrowUp = "ArrowUp" + Bars.GetTime(ProcessingBarIndex);
            AddChartArrowUp(strArrowUp, true, Bars.GetTime(Count - 1), Bars.GetOpen(ProcessingBarIndex) - 100 * TickSize, Color.Green);
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
//
        }


        private void printAuswertung()
        {

            decimal GapTradeResult;
            Color colorTextBox;
            string strResultDescr = "";

            GapTradeResult = (decimal)Bars.GetClose(ProcessingBarIndex - 1) - (decimal)Bars.GetOpen(ProcessingBarIndex - 1);
            if (GapTradeLong == true)
            {
                //Long
                GapTradeCounterLong += 1;
                GapTradeResultTotalLong = GapTradeResultTotalLong + GapTradeResult;


                string strGapeTradeLong = "GapTradeLong" + ProcessingBarIndex;
                string strTradeResultLong;
                TradeDir = "Long";

                if (GapTradeResult < 0)
                {
                    if (_print_info == true)
                    {
                    Print("Fail");
                    }
                    GapTradeFailCounterLong += 1;
                    strResultDescr = "Fail";
                    strTradeResultLong = "Fail " + GapTradeResult.ToString();
                    colorTextBox = colFail;
                }
                else
                {
                    GapTradeWinCounterLong += 1;
                    strResultDescr = "Win";
                    strTradeResultLong = "Win " + GapTradeResult.ToString();
                    colorTextBox = colWin;
                }
                AddChartText(strGapeTradeLong, true, strTradeResultLong, Time[1], Bars.GetHigh(ProcessingBarIndex - 1) + (100 * TickSize), 9, Color.Black, new Font("Arial", 9), StringAlignment.Center, Color.Black, colorTextBox, 70);

            }
            else if (GapTradeShort == true)
            {
                //Short
                GapTradeCounterShort += 1;
                GapTradeResultTotalShort = GapTradeResultTotalShort + GapTradeResult;


                string strGapeTradeShort = "GapTradeLong" + ProcessingBarIndex;
                string strTradeResultShort;
                TradeDir = "Short";

                if (GapTradeResult > 0)
                {
                    if (_print_info == true)
                    {
                        Print("Fail");
                    }
                    GapTradeFailCounterShort += 1;
                    strResultDescr = "Fail";
                    strTradeResultShort = "Fail " + GapTradeResult.ToString();
                    colorTextBox = colFail;
                }
                else
                {
                    GapTradeWinCounterShort += 1;
                    strResultDescr = "Win";
                    strTradeResultShort = "Win " + GapTradeResult.ToString();
                    colorTextBox = colWin;
                }
                AddChartText(strGapeTradeShort, true, strTradeResultShort, Time[1], Bars.GetLow(ProcessingBarIndex - 1) - (100 * TickSize), 9, Color.Black, new Font("Arial", 9), StringAlignment.Center, Color.Black, colorTextBox, 70);
            }
            if (_print_info == true)
            {
                Print("Gap Trade Result: " + GapTradeResult);
            }

            if (dev_mode == true)
            {
                Print(Instrument.Name + ";" + Math.Round(GapSize, 2) + ";" + GapTradeResult + ";" + strResultDescr +";"+ TradeDir);
            }
        }


		#region Properties

        //[Browsable(false)]
        //[XmlIgnore()]
        //public DataSeries MyGap
        //{
        //    get { return Outputs[0]; }
        //}


        [Description("Mind. Punkte für Gap Prozent")]
        [InputParameter]
        [DisplayName("MinPunkteProz")]
        public decimal PunkteGapMinProz
        {
            get { return _PunkteGapMinProz; }
            set { _PunkteGapMinProz = value; }
        }

        [Description("Max. Punkte für Gap Prozent")]
        [InputParameter]
        [DisplayName("MaxPunkteProz")]
        public decimal PunkteGapMaxProz
        {
            get { return _PunkteGapMaxProz; }
            set { _PunkteGapMaxProz = value; }
        }



        [Description("Select Color")]
        [Category("Colors")]
        [DisplayName("Farbe Gap")]
        public Color Color_Gap
        {
            get { return _col_gap; }
            set { _col_gap = value; }
        }


        [Description("Zusätzliche technische Daten im  Ausgabefenster anzeigen?")]
        [Category("Behaviour")]
        [DisplayName("Daten in Ausgabefenster?")]
        public bool print_info
        {
            get { return _print_info; }
            set { _print_info = value; }
        }

        [Description("Entwicklermodus: zusätzliche, noch nicht freigeschaltene, Entwicklungen")]
        [Category("Behaviour")]
        [DisplayName("Entwicklermodus")]
        public bool dev_mode
        {
            get { return _dev_mode; }
            set { _dev_mode = value; }
        }

        [Description("Snapshots speichern")]
        [Category("Behaviour")]
        [DisplayName("Snapshots")]
        public bool Snapshots
        {
            get { return _Snapshots; }
            set { _Snapshots = value; }
        }

		#endregion
	}
}
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
/// Version: 1.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// ToDo
/// 
/// 
/// 
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("Geben Sie bitte hier die Beschreibung für die neue Condition ein")]
	[IsEntryAttribute(false)]
	[IsStopAttribute(true)]
	[IsTargetAttribute(false)]
	[OverrulePreviousStopPrice(false)]
	public class ShowGap_Condition_Stop : UserScriptedCondition
	{
		#region Variables

        //private int _myCondition1 = 1;

        int _PunkteGapMin = 50;
        int _PunkteGapMax = 100;
        private Color _col_gap = Color.Turquoise;
        private Color colWin = Color.Yellow;
        private Color colFail = Color.Brown;
        bool existgap;
        bool GapTradeShort;
        bool GapTradeLong;
        bool sessionprocessed;
        decimal GapTradeCounterShort;
        decimal GapTradeCounterLong;
        decimal GapTradeResultTotalShort;
        decimal GapTradeResultTotalLong;
        decimal GapTradeFailCounterShort;
        decimal GapTradeFailCounterLong;
        decimal GapTradeWinCounterShort;
        decimal GapTradeWinCounterLong;

		#endregion

		protected override void Initialize()
		{
			IsEntry = false;
			IsStop = true;
			IsTarget = false;
			Add(new Plot(Color.FromKnownColor(KnownColor.Black), "Occurred"));
			Add(new Plot(Color.FromArgb(255, 187, 128, 238), "Entry"));
			Overlay = true;
			CalculateOnBarClose = true;
		}

        //protected override void OnBarUpdate()
        //{
        //    //TODO: Write your owner OnBarUpdate handling


        //    //DrawArrowUp("Arrowup" + CurrentBar, true, Bars.GetTime(Count - 1), Bars.GetLow(CurrentBar) - 300 * TickSize, Color.Red);
        //    //DrawArrowDown("Arrowdown" + CurrentBar, true, Bars.GetTime(Count - 1), Bars.GetHigh(CurrentBar) + 300 * TickSize, Color.Green);

        //    //Occurred.Set(-1);
        //    //Entry.Set(Close[0]);

        //}

        protected override void OnBarUpdate()
        {
            //MyGap.Set(Input[0]);

            if (Bars != null && Bars.Count > 0)
            //             && TimeFrame.Periodicity == DatafeedHistoryPeriodicity.Minute
            //             && TimeFrame.PeriodicityValue == 15) 
            { }
            else
            {
                return;
            }

            if (Bars.BarsSinceSession == 0)
            {
                sessionprocessed = false;
            }

            //08.00, 08.15, 08.30, 08.45, 09.00 sind abgeschlossen -> es ist 09.15)
            //                if(Bars.BarsSinceSession == 5)
            if (ToTime(Bars.GetTime(CurrentBar)) > 90000 //größer 09.00 geht für 15M und 1Std (und 1Tag?)
            && sessionprocessed == false)        //Tag noch nicht verarbeitet
            {
                sessionprocessed = true;
                GapTradeLong = GapTradeShort = false;

                IBar GapOpenBar = Bars.Where(x => x.Time.Date == Bars[0].Time.Date).FirstOrDefault(); //liefert erster kerze des tages
                double GapOpen = GapOpenBar.Open;

                double LastDayClose = PriorDayOHLC().PriorClose[0];
                double GapSize = GapOpen - LastDayClose;
                DateTime LastDayCloseDate = Bars.GetTime(Count - 7);
                DateTime LastPeriod = Time[1];

                if (LastDayClose != null
                && Math.Abs(LastDayClose - GapOpen) > _PunkteGapMin
                && Math.Abs(LastDayClose - GapOpen) < _PunkteGapMax)
                {  //Wenn Gap größer 50 und kleiner 100
                    existgap = true;


                    //Gap markieren (08.00 - 09.15)
                    string strMyRect = "MyRect" + Count;
                    string strMyGapSize = "MyGapSize" + Count;


                    if (LastDayClose - GapOpen < 0)   //Long
                    {
                        //Long                        
                        //DrawRectangle(strMyRect, true, LastDayCloseDate, LastDayClose, LastPeriod, HighestHighPrice(5)[0], _col_gap, _col_gap, 70);
                        DrawText(strMyGapSize, true, Math.Round(GapSize, 1).ToString(), LastDayCloseDate, LastDayClose + 25, 9, Color.Black, new Font("Areal", 9), StringAlignment.Center, Color.Black, Color.Azure, 1);

                        // if (LinReg(5)[0] > GapOpen)
                        if (LinReg(Closes[0], 5)[0] > GapOpen)
                        {
                            //Chancenreicher SuccessTrade
                            string strArrowUp = "ArrowUp" + Bars.GetTime(CurrentBar);
                            DrawArrowUp(strArrowUp, true, Bars.GetTime(Count - 1), Bars.GetOpen(CurrentBar) - 300 * TickSize, Color.Green);
                            GapTradeLong = true;

                            Occurred.Set(1);
                            Entry.Set(Bars.GetOpen(CurrentBar));
                        }
                    }
                    else
                    {
                        //Short                        
                        //DrawRectangle(strMyRect, true, LastDayCloseDate, LastDayClose, LastPeriod, LowestLowPrice(5)[0], Color.Pink, Color.Pink, 70);
                        DrawText(strMyGapSize, true, Math.Round(GapSize, 1).ToString(), LastDayCloseDate, LastDayClose - 25, 9, Color.Black, new Font("Areal", 9), StringAlignment.Center, Color.Black, Color.Azure, 1);

                        if (LinReg(Closes[0], 5)[0] < GapOpen)
                        {
                            ////Chancenreicher SuccessTrade
                            string strArrowDown = "ArrowDown" + Bars.GetTime(CurrentBar);
                            DrawArrowDown(strArrowDown, true, Bars.GetTime(Count - 1), Bars.GetOpen(CurrentBar) + 300 * TickSize, Color.Red);
                            GapTradeShort = true;

                            Occurred.Set(-1);
                            Entry.Set(Bars.GetOpen(CurrentBar));
                        }
                    }

                    if (GapTradeShort == true || GapTradeLong == true)
                    {
                        Print("------------------" + Time[5] + "------------------");
                        Print("LineReg: " + Math.Round(LinReg(5)[0]), 1);
                        Print("Gap Open: " + GapOpen);
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
                //Auswertung
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
                        Print("FAAAAAAAAAAAAAAAIIIIIIIIIIIIIILLLLLLLLLL");
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
                    DrawText(strGapeTradeShort, true, strTradeResultShort, Time[1], Bars.GetLow(CurrentBar - 1) - 10, 9, Color.Black, new Font("Areal", 9), StringAlignment.Center, Color.Black, colorTextBox, 70);


                }

                Print("Gap Trade Result: " + GapTradeResult);
            }

            if (Count == Bars.Count - 1)
            {
                Print("Total Trades Long:  " + GapTradeCounterLong);
                Print("Wins Long: " + GapTradeWinCounterLong);
                Print("Fails Long: " + GapTradeFailCounterLong);

                Print("Total Trades Short: " + GapTradeCounterShort);
                Print("Wins Short: " + GapTradeWinCounterShort);
                Print("Fails Short: " + GapTradeFailCounterShort);

                if (GapTradeCounterLong > 0) { Print("Avg Long: " + (GapTradeResultTotalLong / GapTradeCounterLong)); }
                if (GapTradeCounterShort > 0) { Print("Avg Short: " + (GapTradeResultTotalShort / GapTradeCounterShort)); }



            }


        }




        public override string ToString()
        {
            return "ShowGap";
        }

        public override string DisplayName
        {
            get
            {
                return "ShowGap";
            }
        }

		#region Properties

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Occurred
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Entry
		{
			get { return Values[1]; }
		}

		public override IList<DataSeries> GetEntries()
		{
			return new[]{Entry};
		}

        //[Description("")]
        //[Category("Parameters")]
        //public int MyCondition1
        //{
        //    get { return _myCondition1; }
        //    set { _myCondition1 = Math.Max(1, value); }
        //}

        [Description("Mind. Punkte für Gap")]
        [Category("Parameters")]
        [DisplayName("MinPunkte")]
        public int PunkteGapMin
        {
            get { return _PunkteGapMin; }
            set { _PunkteGapMin = value; }
        }

        [Description("Max. Punkte für Gap")]
        [Category("Parameters")]
        [DisplayName("MaxPunkte")]
        public int PunkteGapMax
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

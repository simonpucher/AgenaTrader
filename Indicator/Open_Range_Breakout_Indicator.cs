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
using System.Globalization;



/// <summary>
/// Version: 1.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// ToDo
/// 1)  Customzing für Börsenstart 09.00 oder 15.30
/// 2)  Drawings in Background bringen, aktuell verdecken sie andere Indikatoren wie zB SMA200 -> erledigt mit Opacity
/// 3)  automatische Ordererstellung (http://www.tradeescort.com/phpbb_de/viewtopic.php?f=19&t=2401)
/// 4)  Im 1-Stundenchart wird automatisch das High/Low von dem kompletten Bar genommen. Es wird also die OpenRange von 2 Stunden genommen (120 Mins statt 75) Noch testen!
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
   
    [Description("ORB Indicator")]
	public class ORB_Indicator : UserIndicator
	{
        
        private int _orbminutes = 75;
        private Color _col_orb = Color.Brown;
        private Color _col_target_short = Color.PaleVioletRed;                      
        private Color _col_target_long  = Color.PaleGreen;

        private TimeSpan _tim_OpenRangeStartDE = new TimeSpan(9, 0, 0);    //09:00:00   
        private TimeSpan _tim_OpenRangeEndDE = new TimeSpan(10, 15, 0);    //09:00:00   

        private TimeSpan _tim_OpenRangeStartUS = new TimeSpan(15, 30, 0);  //15:30:00   
        private TimeSpan _tim_OpenRangeEndUS = new TimeSpan(16, 45, 0);  //15:30:00   

        private DateTime DayStart;                                                                                          
        private DateTime DayEnd;
        private DateTime OpenRangeStart;
        private DateTime OpenRangeEnd;
        private double target_short;
        private double target_long;
        private bool SessionSuccessful;
        private decimal CounterShort;
        private decimal CounterLong;
        private decimal CounterSessions;
        

		protected override void Initialize()
		{
			//Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
			Overlay = true;
			CalculateOnBarClose = true;
            ClearOutputWindow();
		}
        

        protected override void InitRequirements()
        {
            //Print("InitRequirements");
        }

        protected override void OnStartUp()
        {
            //Print("OnStartUp");
        }

        protected override void OnBarUpdate()
		{
           // MyPlot1.Set(Input[0]);
           
            // if (Bars != null && Bars.Count > 0 && IsCurrentBarLast)
            if (Bars != null && Bars.Count > 0 && Bars.BarsSinceSession == 0)
            {
                SessionSuccessful = false; //zurücksetzen
                CounterSessions += 1;

                //Liefert RangeStart und RangeEnde
                CalcOpenRange(out OpenRangeStart, out OpenRangeEnd);


                //Liefert TagesStart und TagesEnde        
                Bars.GetNextBeginEnd(Bars, 0, out DayStart, out DayEnd);


                //Selektiere alle gültigen Kurse und finde low und high.
                IEnumerable<IBar> list = Bars.Where(x => x.Time >= OpenRangeStart).Where(x => x.Time <= OpenRangeEnd);

                DrawOpenRange(list, OpenRangeStart, OpenRangeEnd, DayEnd, out target_long, out target_short);
            }
            else if( SessionSuccessful == false) //Wir wollen pro Session nur 1 Success-Meldung
            {

                if (Bars.GetLow(CurrentBar) <= target_short && Bars.GetTime(Count-1) > OpenRangeEnd )
                  {
                        SessionSuccessful = true;
                        CounterShort += 1;
                        BarColor = Color.Purple;
                        string strArrowDown = "ArrowDown" + OpenRangeStart;
                        DrawArrowUp(strArrowDown, true, Bars.GetTime(Count - 1), Bars.GetLow(CurrentBar) - 300 * TickSize, Color.Red);
                        Print("Treffer Short" + DayStart.ToShortDateString());

                 } else if( Bars.GetHigh(CurrentBar) >= target_long && Bars.GetTime(Count-1) > OpenRangeEnd ) {
                        SessionSuccessful = true;
                        CounterLong += 1;
                        BarColor = Color.Turquoise;
                        string strArrowUp = "ArrowUp" + OpenRangeStart;
                        DrawArrowDown(strArrowUp, true, Bars.GetTime(Count - 1), Bars.GetHigh(CurrentBar) + 300 * TickSize, Color.Green);    
                        Print("Treffer Long" + DayStart.ToShortDateString());
                 }
            }

            //Beim letzten Bar den Status schreiben
            if (Count == Bars.Count)
            {
                    Print("Counter Erfolg Long: " + CounterLong);
                    Print("Counter Erfolg Short: " + CounterShort);
                    Print("Counter Erfolg Gesamt: " + (CounterShort + CounterLong));
                    Print("Erfolg %: " + (((CounterShort + CounterLong) / CounterSessions) * 100));
                    Print("CounterSessions: " + CounterSessions);
               }
		}


        protected override void OnTermination()
        {
            //Print("OnTermination");
        }


        private void CalcOpenRange(out DateTime OpenRangeStart, out DateTime OpenRangeEnd)
        {
            //Print("CalcOpenRange");

            OpenRangeStart = Bars.Where(x => x.Time.Date == Bars[0].Time.Date).FirstOrDefault().Time;    //liefert erste tageskerze
            OpenRangeStart = GetStartTime(OpenRangeStart);
            OpenRangeEnd = GetEndTime(OpenRangeStart);
        }


        private TimeSpan getOpenRangeStart( )
        {

            if (Bars.Instrument.Symbol.Contains("DE.30") || Bars.Instrument.Symbol.Contains("DE-XTB"))
                {
                    //return new TimeSpan(9,00,00);
                       return _tim_OpenRangeStartDE;
                }
            else if (Bars.Instrument.Symbol.Contains("US.30") || Bars.Instrument.Symbol.Contains("US-XTB"))
                {
                        //return new TimeSpan(15,30,00);
                        return _tim_OpenRangeStartUS;
                }
                else
                {
                    return _tim_OpenRangeStartDE;
                }
        }


        private DateTime GetStartTime(DateTime start)
        {
            TimeSpan tim_OpenRangeStart = getOpenRangeStart();
            start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);  //Uhrzeit auf 00:00:00 zurücksetzen, ist vorbefüllt aus SessionStart
            return start.Add(tim_OpenRangeStart);
        }

        private DateTime GetEndTime(DateTime start)
        {
            return start.AddMinutes(_orbminutes);        
        }

        private void DrawOpenRange(IEnumerable<IBar> list, DateTime OpenRangeStart, DateTime OpenRangeEnd, DateTime DayEnd, out double target_long, out double target_short)
        {
            target_long = 99999999;
            target_short = 0;
            if (list != null && !list.IsEmpty())
            {
  
                double minvalue = list.Min(y => y.Low);
                double maxvalue = list.Where(x => x.High == list.Max(y => y.High)).LastOrDefault().High;
                string strMyRect;
                string strTargetAreaLong;
                string strTargetAreaShort;
                string strMinValue;
                String strMaxValue;

                strMyRect = "MyRect" + OpenRangeStart;

                DrawRectangle(strMyRect, true, OpenRangeStart, minvalue, OpenRangeEnd, maxvalue, _col_orb, _col_orb, 70);


                //Targets
                target_long = maxvalue + (maxvalue - minvalue);
                target_short = minvalue - (maxvalue - minvalue);
                strTargetAreaLong = "TargetAreaLong" + OpenRangeStart;
                strTargetAreaShort = "TargetAreaShort" + OpenRangeStart;

                DrawRectangle(strTargetAreaLong, true, OpenRangeEnd, maxvalue, DayEnd, target_long, _col_target_long, _col_target_long, 70);
                DrawRectangle(strTargetAreaShort, true, OpenRangeEnd, minvalue, DayEnd, target_short, _col_target_short, _col_target_short, 70);

                //Text Min/Max
                string maxtext = maxvalue.ToString() + "(" + Math.Round((maxvalue - minvalue), 2) + ")";
                strMaxValue = "MaxValue" + OpenRangeStart;
                strMinValue = "MinValue" + OpenRangeStart;

                DrawText(strMaxValue, true, maxtext, OpenRangeStart, maxvalue, 9, Color.Black, new Font("Areal", 9), StringAlignment.Center, Color.Black, Color.Red, 70);
                DrawText(strMinValue, true, minvalue.ToString(), OpenRangeStart, minvalue, -9, Color.Black, new Font("Areal", 9), StringAlignment.Center, Color.Black, Color.Red, 70);
            }  
        
        }


        public override string ToString()
        {
            return "ORB";
                //return base.ToString();
        }

        public override string DisplayName
        {
            get
            {
                return "ORB";
                //return base.DisplayName;
            }
        }



        #region Properties

        #region Output

        //[Browsable(false)]
        //[XmlIgnore()]
        //public DataSeries MyPlot1
        //{
        //    get { return Values[0]; }
        //}

        #endregion


        #region Input



        /// <summary>
        /// </summary>
        [Description("Period in minutes for ORB")]
        [Category("Values")]
        [DisplayName("Minutes ORB")]
        public int ORBMinutes
        {
            get { return _orbminutes; }
            set { _orbminutes = value; }
        }

        /// <summary>
        /// </summary>
        [Description("Select Color")]
        [Category("Colors")]
        [DisplayName("Open Range")]
        public Color Color_ORB
        {
            get { return _col_orb; }
            set { _col_orb = value; }
        }
        //ADD KOVAC 20160403 begin
        /// <summary>
        /// </summary>
        [Description("Select Color TargetAreaShort")]
        [Category("Colors")]
        [DisplayName("Target Area Short")]
        public Color Color_TargetAreaShort
        {
            get { return _col_target_short; }
            set { _col_target_short = value; }
        }

        /// <summary>
        /// </summary>
        [Description("Select Color TargetAreaLong")]
        [Category("Colors")]
        [DisplayName("Target Area Long")]
        public Color Color_TargetAreaLong
        {
            get { return _col_target_long; }
            set { _col_target_long = value; }
        }

        /// <summary>
        /// </summary>
        [Description("OpenRange DE Start: Uhrzeit ab wann Range gemessen wird")]
        [Category("TimeSpan")]
        [DisplayName("1. OpenRange Start DE")]
        public TimeSpan Time_OpenRangeStartDE
        {
            get { return _tim_OpenRangeStartDE; }
            set { _tim_OpenRangeStartDE = value; }
        }

        /// <summary>
        /// </summary>
        [Description("OpenRange DE End: Uhrzeit wann Range geschlossen wird")]
        [Category("TimeSpan")]
        [DisplayName("2. OpenRange End DE")]
        public TimeSpan Time_OpenRangeEndDE
        {
            get { return _tim_OpenRangeEndDE; }
            set { _tim_OpenRangeEndDE = value; }
        }

        /// <summary>
        /// </summary>
        [Description("OpenRange US Start: Uhrzeit ab wann Range gemessen wird")]
        [Category("TimeSpan")]
        [DisplayName("3. OpenRange Start US")]
        public TimeSpan Time_OpenRangeStartUS
        {
            get { return _tim_OpenRangeStartUS; }
            set { _tim_OpenRangeStartUS = value; }
        }

        /// <summary>
        /// </summary>
        [Description("OpenRange US End: Uhrzeit wann Range geschlossen wird")]
        [Category("TimeSpan")]
        [DisplayName("4. OpenRange End US")]
        public TimeSpan Time_OpenRangeEndUS
        {
            get { return _tim_OpenRangeEndUS; }
            set { _tim_OpenRangeEndUS = value; }
        }

        //ADD KOVAC 20160403 end

        #endregion


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
		/// ORB Indicator
		/// </summary>
		public ORB_Indicator ORB_Indicator()
        {
			return ORB_Indicator(Input);
		}

		/// <summary>
		/// ORB Indicator
		/// </summary>
		public ORB_Indicator ORB_Indicator(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<ORB_Indicator>(input);

			if (indicator != null)
				return indicator;

			indicator = new ORB_Indicator
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
		/// ORB Indicator
		/// </summary>
		public ORB_Indicator ORB_Indicator()
		{
			return LeadIndicator.ORB_Indicator(Input);
		}

		/// <summary>
		/// ORB Indicator
		/// </summary>
		public ORB_Indicator ORB_Indicator(IDataSeries input)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.ORB_Indicator(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// ORB Indicator
		/// </summary>
		public ORB_Indicator ORB_Indicator()
		{
			return LeadIndicator.ORB_Indicator(Input);
		}

		/// <summary>
		/// ORB Indicator
		/// </summary>
		public ORB_Indicator ORB_Indicator(IDataSeries input)
		{
			return LeadIndicator.ORB_Indicator(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// ORB Indicator
		/// </summary>
		public ORB_Indicator ORB_Indicator()
		{
			return LeadIndicator.ORB_Indicator(Input);
		}

		/// <summary>
		/// ORB Indicator
		/// </summary>
		public ORB_Indicator ORB_Indicator(IDataSeries input)
		{
			return LeadIndicator.ORB_Indicator(input);
		}
	}

	#endregion

}

#endregion

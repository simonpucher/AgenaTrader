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
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("Open Range Breakout Condition")]
	[IsEntryAttribute(true)]
	[IsStopAttribute(false)]
	[IsTargetAttribute(false)]
	[OverrulePreviousStopPrice(false)]
	public class ORB_Condition : UserScriptedCondition
	{
		#region Variables

        //input
        private int _orbminutes = 75;
        private Color _col_orb = Color.Brown;
        private Color _col_target_short = Color.PaleVioletRed;
        private Color _col_target_long = Color.PaleGreen;

        private TimeSpan _tim_OpenRangeStartDE = new TimeSpan(9, 0, 0);    //09:00:00   
        private TimeSpan _tim_OpenRangeEndDE = new TimeSpan(10, 15, 0);    //09:00:00   

        private TimeSpan _tim_OpenRangeStartUS = new TimeSpan(15, 30, 0);  //15:30:00   
        private TimeSpan _tim_OpenRangeEndUS = new TimeSpan(16, 45, 0);  //15:30:00   

        private TimeSpan _tim_EndOfDay_DE = new TimeSpan(16, 30, 0);  //16:30:00   
        private TimeSpan _tim_EndOfDay_US = new TimeSpan(21, 30, 0);  //21:30:00

        //output
		private int _myCondition1 = 1;

        //internal


		#endregion

		protected override void Initialize()
		{
			IsEntry = true;
			IsStop = false;
			IsTarget = false;
			Add(new Plot(Color.FromKnownColor(KnownColor.Black), "Occurred"));
			Add(new Plot(Color.FromArgb(255, 187, 128, 238), "Entry"));
			Overlay = true;
			CalculateOnBarClose = true;
		}

		protected override void OnBarUpdate()
		{
	

            //Occurred.Set(-1);
            //Entry.Set(Close[0]);

		}


        public override string ToString()
        {
            return "ORB";
        }

        public override string DisplayName
        {
            get
            {
                return "ORB";
            }
        }

        #region Public Functions for usage in other ORB Indicators, Targets, Stops, and so on

      /// <summary>
      /// 
      /// </summary>
      /// <param name="time_openrangestartde"></param>
      /// <param name="time_openrangestartus"></param>
      /// <returns></returns>
        public TimeSpan getOpenRangeStart(TimeSpan time_openrangestartde, TimeSpan time_openrangestartus)
            {
                if (Bars.Instrument.Symbol.Contains("DE.30") || Bars.Instrument.Symbol.Contains("DE-XTB"))
                {
                    //return new TimeSpan(9,00,00);
                    return time_openrangestartde;
                }
                else if (Bars.Instrument.Symbol.Contains("US.30") || Bars.Instrument.Symbol.Contains("US-XTB"))
                {
                    //return new TimeSpan(15,30,00);
                    return time_openrangestartus;
                }
                else
                {
                    return time_openrangestartde;
                }
            }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time_openrangeendde"></param>
        /// <param name="time_openrangeendus"></param>
        /// <returns></returns>
            public TimeSpan getEODTime(TimeSpan time_openrangeendde, TimeSpan time_openrangeendus)
            {

                if (Bars.Instrument.Symbol.Contains("DE.30") || Bars.Instrument.Symbol.Contains("DE-XTB"))
                {
                    //return new TimeSpan(9,00,00);
                    return time_openrangeendde;
                }
                else if (Bars.Instrument.Symbol.Contains("US.30") || Bars.Instrument.Symbol.Contains("US-XTB"))
                {
                    //return new TimeSpan(15,30,00);
                    return time_openrangeendus;
                }
                else
                {
                    return time_openrangeendde;
                }
            }


        #endregion


        #region Properties


        #region Input

        /// <summary>
            /// </summary>
            [Description("Period in minutes for ORB")]
            [Category("Parameters")]
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
            [DisplayName("ORB")]
            public Color Color_ORB
            {
                get { return _col_orb; }
                set { _col_orb = value; }
            }

            /// <summary>
            /// </summary>
            [Description("Select Color TargetAreaShort")]
            [Category("Colors")]
            [DisplayName("TargetAreaShort")]
            public Color Color_TargetAreaShort
            {
                get { return _col_target_short; }
                set { _col_target_short = value; }
            }

            /// <summary>
            /// </summary>
            [Description("Select Color TargetAreaLong")]
            [Category("Colors")]
            [DisplayName("TargetAreaLong")]
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

            /// <summary>
            /// </summary>
            [Description("EndOfDay DE: Uhrzeit spätestens verkauft wird")]
            [Category("TimeSpan")]
            [DisplayName("5. EndOfDay DE")]
            public TimeSpan Time_EndOfDay_DE
            {
                get { return _tim_EndOfDay_DE; }
                set { _tim_EndOfDay_DE = value; }
            }

            /// <summary>
            /// </summary>
            [Description("EndOfDay US: Uhrzeit spätestens verkauft wird")]
            [Category("TimeSpan")]
            [DisplayName("5. EndOfDay US")]
            public TimeSpan Time_EndOfDay_US
            {
                get { return _tim_EndOfDay_US; }
                set { _tim_EndOfDay_US = value; }
            }
            #endregion


            #region Output

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
                    return new[] { Entry };
                }

                [Description("")]
                [Category("Parameters")]
                public int MyCondition1
                {
                    get { return _myCondition1; }
                    set { _myCondition1 = Math.Max(1, value); }
                }
            #endregion



		#endregion
	}
}

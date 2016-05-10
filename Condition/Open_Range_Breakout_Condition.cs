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
/// The initial version of this strategy was inspired by the work of Birger Schäfermeier: https://www.whselfinvest.at/de/Store_Birger_Schaefermeier_Trading_Strategie_Open_Range_Break_Out.php
/// Further developments are inspired by the work of Mehmet Emre Cekirdekci and Veselin Iliev from the Worcester Polytechnic Institute (2010)
/// Trading System Development: Trading the Opening Range Breakouts https://www.wpi.edu/Pubs/E-project/Available/E-project-042910-142422/unrestricted/Veselin_Iliev_IQP.pdf
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this indicator without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
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
    public class ORB_Condition : UserScriptedCondition, IORB
	{
		#region Variables

        //input
        private int _orbminutes = 75;

        private TimeSpan _tim_OpenRangeStartDE = new TimeSpan(9, 0, 0);
        //private TimeSpan _tim_OpenRangeEndDE = new TimeSpan(10, 15, 0);  

        private TimeSpan _tim_OpenRangeStartUS = new TimeSpan(15, 30, 0);
        //private TimeSpan _tim_OpenRangeEndUS = new TimeSpan(16, 45, 0);    

        private TimeSpan _tim_EndOfDay_DE = new TimeSpan(17, 30, 0);
        private TimeSpan _tim_EndOfDay_US = new TimeSpan(22, 00, 0);  

        private bool _send_email = false;


        //output
		private int _myCondition1 = 1;

        //internal
        private ORB_Indicator _orb_indicator = null;


		#endregion

		protected override void Initialize()
		{
			IsEntry = true;
			IsStop = false;
			IsTarget = false;
			Add(new Plot(Color.FromKnownColor(KnownColor.Black), "Occurred"));
			Add(new Plot(Color.FromArgb(255, 187, 128, 238), "Entry"));
			Overlay = true;
			CalculateOnBarClose = false;
		}

        protected override void InitRequirements()
        {
            base.InitRequirements();


        }


        protected override void OnStartUp()
        {
            base.OnStartUp();

            //Init our indicator to get code access
            this._orb_indicator = new ORB_Indicator();
            this._orb_indicator.SetData(this.Instrument, this.Bars);

            //Initalize Indicator parameters
            _orb_indicator.ORBMinutes = this.ORBMinutes;
            _orb_indicator.Time_OpenRangeStartDE = this.Time_OpenRangeStartDE;
            _orb_indicator.Time_OpenRangeStartUS = this.Time_OpenRangeStartUS;
            _orb_indicator.Time_EndOfDay_DE = this.Time_EndOfDay_DE;
            _orb_indicator.Time_EndOfDay_US = this.Time_EndOfDay_US;
        }

       

		protected override void OnBarUpdate()
		{


            _orb_indicator.calculate(Bars[0]);
            //Occurred.Set(returnvalue);
            //Entry.Set(Bars[0].Close);

            //If there was a breakout and the current bar is the same bar as the long/short breakout, then trigger signal.
            if (_orb_indicator.LongBreakout != null && _orb_indicator.LongBreakout.Time == Bars[0].Time)
            {
                //Long Signal
                Occurred.Set(1);
                Entry.Set(Close[0]);
            }
            else if (_orb_indicator.ShortBreakout != null && _orb_indicator.ShortBreakout.Time == Bars[0].Time)
            {
                //Short Signal
                Occurred.Set(-1);
                Entry.Set(Close[0]);
            }
            else
            {
                //No Signal
                Occurred.Set(0);
                Entry.Set(Close[0]);
            }

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

     


        #region Properties


        #region Input

        /// <summary>
            /// </summary>
            [Description("Period in minutes for ORB")]
            [Category("TimeSpan")]
            [DisplayName("Minutes ORB")]
            public int ORBMinutes
            {
                get { return _orbminutes; }
                set { _orbminutes = value; }
            }

           
            /// <summary>
            /// </summary>
            [Description("OpenRange DE Start: Uhrzeit ab wann Range gemessen wird")]
            [Category("TimeSpan")]
            [DisplayName("OpenRange Start DE")]
            public TimeSpan Time_OpenRangeStartDE
            {
                get { return _tim_OpenRangeStartDE; }
                set { _tim_OpenRangeStartDE = value; }
            }


            /// <summary>
            /// </summary>
            [Description("OpenRange US Start: Uhrzeit ab wann Range gemessen wird")]
            [Category("TimeSpan")]
            [DisplayName("OpenRange Start US")]
            public TimeSpan Time_OpenRangeStartUS
            {
                get { return _tim_OpenRangeStartUS; }
                set { _tim_OpenRangeStartUS = value; }
            }


            /// <summary>
            /// </summary>
            [Description("EndOfDay DE: Uhrzeit spätestens verkauft wird")]
            [Category("TimeSpan")]
            [DisplayName("EndOfDay DE")]
            public TimeSpan Time_EndOfDay_DE
            {
                get { return _tim_EndOfDay_DE; }
                set { _tim_EndOfDay_DE = value; }
            }

            /// <summary>
            /// </summary>
            [Description("EndOfDay US: Uhrzeit spätestens verkauft wird")]
            [Category("TimeSpan")]
            [DisplayName("EndOfDay US")]
            public TimeSpan Time_EndOfDay_US
            {
                get { return _tim_EndOfDay_US; }
                set { _tim_EndOfDay_US = value; }
            }



            [Description("If true an email will be send on open range breakout.")]
            [Category("Email")]
            [DisplayName("Send email on breakout")]
            public bool Send_email
            {
                get { return _send_email; }
                set { _send_email = value; }
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

              
            #endregion

        #region Internals


            [Browsable(false)]
            public bool IsEmailFunctionActive
            {
                get
                {
                    if (this.Send_email) // && GlobalUtilities.IsValidEmail(this.EmailAdress))
                    {
                        return true;
                    }
                    return false;
                }
            }

        #endregion


        #endregion
    }
}

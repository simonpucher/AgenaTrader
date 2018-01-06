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
/// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
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
        private int _orbminutes = Const.DefaultOpenRangeSizeinMinutes;
        private Color _plot1color = Const.DefaultIndicatorColor;
        private int _plot1width = Const.DefaultLineWidth;
        private DashStyle _plot1dashstyle = Const.DefaultIndicatorDashStyle;

        private TimeSpan _tim_OpenRangeStartDE = new TimeSpan(8, 0, 0);
        private TimeSpan _tim_OpenRangeStartUS = new TimeSpan(14, 30, 0);

        private TimeSpan _tim_EndOfDay_DE = new TimeSpan(17, 30, 0);
        private TimeSpan _tim_EndOfDay_US = new TimeSpan(22, 00, 0);
       

   


        //output
		private int _myCondition1 = 1;

        //internal
        private ORB_Indicator _orb_indicator = null;


		#endregion

		protected override void OnInit()
		{
			IsEntry = true;
			IsStop = false;
			IsTarget = false;
            Add(new OutputDescriptor(new Pen(this.Plot1Color, this.Plot0Width), OutputSerieDrawStyle.Line, "Occurred"));
            //Add(new OutputDescriptor(new Pen(this.Plot1Color, this.Plot0Width), OutputSerieDrawStyle.Line, "Entry"));
			IsOverlay = true;
			CalculateOnClosedBar = false;

            //Because of Backtesting reasons if we use the afvanced mode we need at least two bars
            this.RequiredBarsCount = 2;
		}

        protected override void OnBarsRequirements()
        {
            base.OnBarsRequirements();

        
        }


        protected override void OnStart()
        {
            base.OnStart();

            //Init our indicator to get code access
            this._orb_indicator = new ORB_Indicator();
            this._orb_indicator.SetData(this.Instrument);

            //Initalize Indicator parameters
            _orb_indicator.ORBMinutes = this.ORBMinutes;
            _orb_indicator.Time_OpenRangeStartDE = this.Time_OpenRangeStartDE;
            _orb_indicator.Time_OpenRangeStartUS = this.Time_OpenRangeStartUS;
            _orb_indicator.Time_EndOfDay_DE = this.Time_EndOfDay_DE;
            _orb_indicator.Time_EndOfDay_US = this.Time_EndOfDay_US;
        }

       

		protected override void OnCalculate()
		{

            _orb_indicator.calculate(this.Bars, this.Bars[0]);
            //Occurred.Set(returnvalue);
            //Entry.Set(Bars[0].Close);

            //If there was a breakout and the current bar is the same bar as the long/short breakout, then trigger signal.
            if (_orb_indicator.LongBreakout != null && _orb_indicator.LongBreakout.Time == Bars[0].Time)
            {
                //Long Signal
                Occurred.Set(1);
                //Entry.Set(Close[0]);
            }
            else if (_orb_indicator.ShortBreakout != null && _orb_indicator.ShortBreakout.Time == Bars[0].Time)
            {
                //Short Signal
                Occurred.Set(-1);
                //Entry.Set(Close[0]);
            }
            else
            {
                //No Signal
                Occurred.Set(0);
                //Entry.Set(Close[0]);
            }

		}


        public override string ToString()
        {
            return "ORB (C)";
        }

        public override string DisplayName
        {
            get
            {
                return "ORB (C)";
            }
        }

     


        #region Properties


        #region InSeries

        /// <summary>
        /// </summary>
        [Description("Period in minutes for ORB")]
        [Category("Minutes")]
        [DisplayName("Minutes ORB")]
        public int ORBMinutes
        {
            get { return _orbminutes; }
            set
            {
                if (value >= 1 && value <= 300)
                {
                    _orbminutes = value;
                }
                else
                {
                    _orbminutes = Const.DefaultOpenRangeSizeinMinutes;
                }
            }
        }


            /// <summary>
            /// </summary>
            [Description("Start of the open range in Germany")]
            [Category("CFD")]
            [DisplayName("OpenRange Start DE")]
            public TimeSpan Time_OpenRangeStartDE
            {
                get { return _tim_OpenRangeStartDE; }
                set { _tim_OpenRangeStartDE = value; }
            }
            [Browsable(false)]
            public long Time_OpenRangeStartDESerialize
            {
                get { return _tim_OpenRangeStartDE.Ticks; }
                set { _tim_OpenRangeStartDE = new TimeSpan(value); }
            }



            /// <summary>
            /// </summary>
            [Description("Start of the open range in America")]
            [Category("CFD")]
            [DisplayName("OpenRange Start US")]
            public TimeSpan Time_OpenRangeStartUS
            {
                get { return _tim_OpenRangeStartUS; }
                set { _tim_OpenRangeStartUS = value; }
            }
            [Browsable(false)]
            public long Time_OpenRangeStartUSSerialize
            {
                get { return _tim_OpenRangeStartUS.Ticks; }
                set { _tim_OpenRangeStartUS = new TimeSpan(value); }
            }



            /// <summary>
            /// </summary>
            [Description("End of trading day in Germany")]
            [Category("CFD")]
            [DisplayName("EndOfDay DE")]
            public TimeSpan Time_EndOfDay_DE
            {
                get { return _tim_EndOfDay_DE; }
                set { _tim_EndOfDay_DE = value; }
            }
            [Browsable(false)]
            public long Time_EndOfDay_DESerialize
            {
                get { return _tim_EndOfDay_DE.Ticks; }
                set { _tim_EndOfDay_DE = new TimeSpan(value); }
            }

            /// <summary>
            /// </summary>
            [Description("End of trading day in America")]
            [Category("CFD")]
            [DisplayName("EndOfDay US")]
            public TimeSpan Time_EndOfDay_US
            {
                get { return _tim_EndOfDay_US; }
                set { _tim_EndOfDay_US = value; }
            }
            [Browsable(false)]
            public long Time_EndOfDay_USSerialize
            {
                get { return _tim_EndOfDay_US.Ticks; }
                set { _tim_EndOfDay_US = new TimeSpan(value); }
            }



            #region Plotstyle

            [XmlIgnore()]
            [Description("Select Color")]
            [Category("Colors")]
            [DisplayName("ORB Indicator")]
            public Color Plot1Color
            {
                get { return _plot1color; }
                set { _plot1color = value; }
            }

            [Browsable(false)]
            public string Plot1ColorSerialize
            {
                get { return SerializableColor.ToString(_plot1color); }
                set { _plot1color = SerializableColor.FromString(value); }
            }

            /// <summary>
            /// </summary>
            [Description("Width for Indicator.")]
            [Category("Plots")]
            [DisplayName("Line Width Indicator")]
            public int Plot0Width
            {
                get { return _plot1width; }
                set { _plot1width = Math.Max(1, value); }
            }


            /// <summary>
            /// </summary>
            [Description("DashStyle for Indicator.")]
            [Category("Plots")]
            [DisplayName("Dash Style Indicator")]
            public DashStyle Dash0Style
            {
                get { return _plot1dashstyle; }
                set { _plot1dashstyle = value; }
            }

            #endregion

            #endregion


            #region Output

                [Browsable(false)]
                [XmlIgnore()]
                public DataSeries Occurred
                {
                    get { return Outputs[0]; }
                }

                [Browsable(false)]
                [XmlIgnore()]
                public DataSeries Entry
                {
                    get { return Outputs[1]; }
                }

                public override IList<DataSeries> GetEntries()
                {
                    return new[] { Entry };
                }

              
            #endregion



        #endregion
    }
}
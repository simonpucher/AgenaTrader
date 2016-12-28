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
/// Version: 1.2
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// todo
/// If barsrequired is smaller than the MAs print a warning message.
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://github.com/ScriptTrading/Basic-Package/blob/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    /// <summary>
    /// Selectable MAs for this indicator.
    /// </summary>
    public enum Enum_Moving_Averages_Indicator_MA
    {
        SMA = 0,
        EMA = 1
    }

    [Description("Use 5 different SMA or EMA at the same time in one indicator.")]
    public class Moving_Averages_Indicator : UserIndicator
    {

        //input 
        private Enum_Moving_Averages_Indicator_MA _MA_1_Selected = Enum_Moving_Averages_Indicator_MA.EMA;
        private Enum_Moving_Averages_Indicator_MA _MA_2_Selected = Enum_Moving_Averages_Indicator_MA.SMA;
        private Enum_Moving_Averages_Indicator_MA _MA_3_Selected = Enum_Moving_Averages_Indicator_MA.SMA;
        private Enum_Moving_Averages_Indicator_MA _MA_4_Selected = Enum_Moving_Averages_Indicator_MA.SMA;
        private Enum_Moving_Averages_Indicator_MA _MA_5_Selected = Enum_Moving_Averages_Indicator_MA.SMA;
        private Enum_Moving_Averages_Indicator_MA _MA_6_Selected = Enum_Moving_Averages_Indicator_MA.SMA;

        private int _ma_1 = 50;
        private int _ma_2 = 100;
        private int _ma_3 = 200;
        private int _ma_4 = 0;
        private int _ma_5 = 0;
        private int _ma_6 = 0;

        private int _linewidth_1 = 2;
        private DashStyle _linestyle_1 = DashStyle.Solid;
        private Color _col_1 = Color.Red;

        private int _linewidth_2 = 2;
        private DashStyle _linestyle_2 = DashStyle.Solid;
        private Color _col_2 = Color.Orange;

        private int _linewidth_3 = 2;
        private DashStyle _linestyle_3 = DashStyle.Solid;
        private Color _col_3 = Color.Blue;

        private int _linewidth_4 = 2;
        private DashStyle _linestyle_4 = DashStyle.Solid;
        private Color _col_4 = Color.Green;

        private int _linewidth_5 = 2;
        private DashStyle _linestyle_5 = DashStyle.Solid;
        private Color _col_5 = Color.DarkViolet;

        private int _linewidth_6 = 2;
        private DashStyle _linestyle_6 = DashStyle.Solid;
        private Color _col_6 = Color.DarkGoldenrod;



        protected override void OnInit()
        {
            
            Add(new Plot(new Pen(this.Color_1, this.LineWidth_1), PlotStyle.Line, "MA_1"));
            Add(new Plot(new Pen(this.Color_2, this.LineWidth_2), PlotStyle.Line, "MA_2"));
            Add(new Plot(new Pen(this.Color_3, this.LineWidth_3), PlotStyle.Line, "MA_3"));
            Add(new Plot(new Pen(this.Color_4, this.LineWidth_4), PlotStyle.Line, "MA_4"));
            Add(new Plot(new Pen(this.Color_5, this.LineWidth_5), PlotStyle.Line, "MA_5"));
            Add(new Plot(new Pen(this.Color_6, this.LineWidth_6), PlotStyle.Line, "MA_6"));

            CalculateOnClosedBar = true;
            IsOverlay = true;

            //this.GetNameOnchart();

        }


        //protected override void OnStart()
        //{
        //    //Set the color
        //    PlotColors[0][0] = this.Color_1;
        //    Plots[0].PenStyle = this.DashStyle_1;
        //    Plots[0].Pen.Width = this.LineWidth_1;
        //    PlotColors[1][0] = this.Color_2;
        //    Plots[1].PenStyle = this.DashStyle_2;
        //    Plots[1].Pen.Width = this.LineWidth_2;
        //    PlotColors[2][0] = this.Color_3;
        //    Plots[2].PenStyle = this.DashStyle_3;
        //    Plots[2].Pen.Width = this.LineWidth_3;
        //    PlotColors[3][0] = this.Color_4;
        //    Plots[3].PenStyle = this.DashStyle_4;
        //    Plots[3].Pen.Width = this.LineWidth_4;
        //    PlotColors[4][0] = this.Color_5;
        //    Plots[4].PenStyle = this.DashStyle_5;
        //    Plots[4].Pen.Width = this.LineWidth_5;
        //}





        protected override void OnCalculate()
        {
            //this.GetNameOnchart();

           

            if (this.MA_1 != 0)
            {
                switch (MA_1_Selected)
                {
                    case Enum_Moving_Averages_Indicator_MA.SMA:
                        Plot_1.Set(SMA(this.MA_1)[0]);
                        break;
                    case Enum_Moving_Averages_Indicator_MA.EMA:
                        Plot_1.Set(EMA(this.MA_1)[0]);
                        break;
                    default:
                        break;
                }
            }


            if (this.MA_2 != 0)
            {
                switch (MA_2_Selected)
                {
                    case Enum_Moving_Averages_Indicator_MA.SMA:
                        Plot_2.Set(SMA(this.MA_2)[0]);
                        break;
                    case Enum_Moving_Averages_Indicator_MA.EMA:
                        Plot_2.Set(EMA(this.MA_2)[0]);
                        break;
                    default:
                        break;
                }
            }


            if (this.MA_3 != 0)
            {
                switch (MA_3_Selected)
                {
                    case Enum_Moving_Averages_Indicator_MA.SMA:
                        Plot_3.Set(SMA(this.MA_3)[0]);
                        break;
                    case Enum_Moving_Averages_Indicator_MA.EMA:
                        Plot_3.Set(EMA(this.MA_3)[0]);
                        break;
                    default:
                        break;
                }
            }

            if (this.MA_4 != 0)
            {
                switch (MA_4_Selected)
                {
                    case Enum_Moving_Averages_Indicator_MA.SMA:
                        Plot_4.Set(SMA(this.MA_4)[0]);
                        break;
                    case Enum_Moving_Averages_Indicator_MA.EMA:
                        Plot_4.Set(EMA(this.MA_4)[0]);
                        break;
                    default:
                        break;
                }
            }

            if (this.MA_5 != 0)
            {
                switch (MA_5_Selected)
                {
                    case Enum_Moving_Averages_Indicator_MA.SMA:
                        Plot_5.Set(SMA(this.MA_5)[0]);
                        break;
                    case Enum_Moving_Averages_Indicator_MA.EMA:
                        Plot_5.Set(EMA(this.MA_5)[0]);
                        break;
                    default:
                        break;
                }
            }

            if (this.MA_6 != 0)
            {
                switch (MA_6_Selected)
                {
                    case Enum_Moving_Averages_Indicator_MA.SMA:
                        Plot_5.Set(SMA(this.MA_6)[0]);
                        break;
                    case Enum_Moving_Averages_Indicator_MA.EMA:
                        Plot_5.Set(EMA(this.MA_6)[0]);
                        break;
                    default:
                        break;
                }
            }


            //Set the color
            PlotColors[0][0] = this.Color_1;
            Plots[0].PenStyle = this.DashStyle_1;
            Plots[0].Pen.Width = this.LineWidth_1;
            PlotColors[1][0] = this.Color_2;
            Plots[1].PenStyle = this.DashStyle_2;
            Plots[1].Pen.Width = this.LineWidth_2;
            PlotColors[2][0] = this.Color_3;
            Plots[2].PenStyle = this.DashStyle_3;
            Plots[2].Pen.Width = this.LineWidth_3;
            PlotColors[3][0] = this.Color_4;
            Plots[3].PenStyle = this.DashStyle_4;
            Plots[3].Pen.Width = this.LineWidth_4;
            PlotColors[4][0] = this.Color_5;
            Plots[4].PenStyle = this.DashStyle_5;
            Plots[4].Pen.Width = this.LineWidth_5;
            PlotColors[5][0] = this.Color_6;
            Plots[5].PenStyle = this.DashStyle_6;
            Plots[5].Pen.Width = this.LineWidth_6;

        }


        private string GetNameOnchart()
        {
           string returnvalue = "Moving Averages (I) ";
            
            if (this.MA_1 != 0)
            {
                returnvalue += MA_1_Selected.ToString() + this.MA_1 + " ";
            }
            
            if (this.MA_2 != 0)
            {
                returnvalue += MA_2_Selected.ToString() + this.MA_2 + " ";
            }
            
            if (this.MA_3 != 0)
            {
                returnvalue += MA_3_Selected.ToString() + this.MA_3 + " ";
            }

            if (this.MA_4 != 0)
            {
                returnvalue += MA_4_Selected.ToString() + this.MA_4 + " ";
            }

            if (this.MA_5 != 0)
            {
                returnvalue += MA_5_Selected.ToString() + this.MA_5 + " ";
            }

            if (this.MA_6 != 0)
            {
                returnvalue += MA_6_Selected.ToString() + this.MA_6 + " ";
            }

            return returnvalue;
        }


        public override string ToString()
        {
            return GetNameOnchart();
        }

        public override string DisplayName
        {
            get
            {
                return GetNameOnchart();
            }
        }


        

		#region Properties

        #region InSeries

        
         /// <summary>
        /// </summary>
        [Description("Select the type of MA 1 you would like to use")]
        [Category("Parameters")]
        [DisplayName("1.1 Type of MA")]
        public Enum_Moving_Averages_Indicator_MA MA_1_Selected
        {
            get { return _MA_1_Selected; }
            set
            {
                _MA_1_Selected = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("Period for the MA 1")]
        [Category("Parameters")]
        [DisplayName("1.2 Period MA")]
        public int MA_1
        {
            get { return _ma_1; }
            set
            {
                _ma_1 = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("Line Width of MA 1.")]
        [Category("Plots")]
        [DisplayName("LW MA 1")]
        public int LineWidth_1
        {
            get { return _linewidth_1; }
            set { _linewidth_1 = Math.Max(1, value); }
        }


        /// <summary>
        /// </summary>
        [Description("DashStyle for MA 1.")]
        [Category("Plots")]
        [DisplayName("DS MA 1")]
        public DashStyle DashStyle_1
        {
            get { return _linestyle_1; }
            set { _linestyle_1 = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Color for MA 1")]
        [Category("Plots")]
        [DisplayName("Color MA 1")]
        public Color Color_1
        {
            get { return _col_1; }
            set { _col_1 = value; }
        }

        [Browsable(false)]
        public string Color_1_Serialize
        {
            get { return SerializableColor.ToString(_col_1); }
            set { _col_1 = SerializableColor.FromString(value); }
        }


        /// <summary>
        /// </summary>
        [Description("Select the type of MA you would like to use")]
        [Category("Parameters")]
        [DisplayName("2.1 Type of MA")]
        public Enum_Moving_Averages_Indicator_MA MA_2_Selected
        {
            get { return _MA_2_Selected; }
            set
            {
                _MA_2_Selected = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("Period for the MA 2")]
        [Category("Parameters")]
        [DisplayName("2.2 Period MA")]
        public int MA_2
        {
            get { return _ma_2; }
            set
            {
                _ma_2 = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("Line Width of MA 2.")]
        [Category("Plots")]
        [DisplayName("LW MA 2")]
        public int LineWidth_2
        {
            get { return _linewidth_2; }
            set { _linewidth_2 = Math.Max(1, value); }
        }


        /// <summary>
        /// </summary>
        [Description("DashStyle for MA 2.")]
        [Category("Plots")]
        [DisplayName("DS MA 2")]
        public DashStyle DashStyle_2
        {
            get { return _linestyle_2; }
            set { _linestyle_2 = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Color for MA 2")]
        [Category("Plots")]
        [DisplayName("Color MA 2")]
        public Color Color_2
        {
            get { return _col_2; }
            set { _col_2 = value; }
        }

        [Browsable(false)]
        public string Color_2_Serialize
        {
            get { return SerializableColor.ToString(_col_2); }
            set { _col_2 = SerializableColor.FromString(value); }
        }


        /// <summary>
        /// </summary>
        [Description("Select the type of MA you would like to use")]
        [Category("Parameters")]
        [DisplayName("3.1 Type of MA")]
        public Enum_Moving_Averages_Indicator_MA MA_3_Selected
        {
            get { return _MA_3_Selected; }
            set
            {
                _MA_3_Selected = value;
            }
        }


        /// <summary>
        /// </summary>
        [Description("Period for the MA 3")]
        [Category("Parameters")]
        [DisplayName("3.2 Period MA")]
        public int MA_3
        {
            get { return _ma_3; }
            set
            {
                _ma_3 = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("Line Width of MA 3.")]
        [Category("Plots")]
        [DisplayName("LW MA 3")]
        public int LineWidth_3
        {
            get { return _linewidth_3; }
            set { _linewidth_3 = Math.Max(1, value); }
        }


        /// <summary>
        /// </summary>
        [Description("DashStyle for MA 3.")]
        [Category("Plots")]
        [DisplayName("DS MA 3")]
        public DashStyle DashStyle_3
        {
            get { return _linestyle_3; }
            set { _linestyle_3 = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Color for MA 3")]
        [Category("Plots")]
        [DisplayName("Color MA 3")]
        public Color Color_3
        {
            get { return _col_3; }
            set { _col_3 = value; }
        }

        [Browsable(false)]
        public string Color_3_Serialize
        {
            get { return SerializableColor.ToString(_col_3); }
            set { _col_3 = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Select the type of MA you would like to use")]
        [Category("Parameters")]
        [DisplayName("4.1 Type of MA")]
        public Enum_Moving_Averages_Indicator_MA MA_4_Selected
        {
            get { return _MA_4_Selected; }
            set
            {
                _MA_4_Selected = value;
            }
        }


        /// <summary>
        /// </summary>
        [Description("Period for the MA 4")]
        [Category("Parameters")]
        [DisplayName("4.2 Period MA")]
        public int MA_4
        {
            get { return _ma_4; }
            set
            {
                _ma_4 = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("Line Width of MA 4.")]
        [Category("Plots")]
        [DisplayName("LW MA 4")]
        public int LineWidth_4
        {
            get { return _linewidth_4; }
            set { _linewidth_4 = Math.Max(1, value); }
        }


        /// <summary>
        /// </summary>
        [Description("DashStyle for MA 4.")]
        [Category("Plots")]
        [DisplayName("DS MA 4")]
        public DashStyle DashStyle_4
        {
            get { return _linestyle_4; }
            set { _linestyle_4 = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Color for MA 4")]
        [Category("Plots")]
        [DisplayName("Color MA 4")]
        public Color Color_4
        {
            get { return _col_4; }
            set { _col_4 = value; }
        }

        [Browsable(false)]
        public string Color_4_Serialize
        {
            get { return SerializableColor.ToString(_col_4); }
            set { _col_4 = SerializableColor.FromString(value); }
        }


        /// <summary>
        /// </summary>
        [Description("Select the type of MA you would like to use")]
        [Category("Parameters")]
        [DisplayName("5.1 Type of MA")]
        public Enum_Moving_Averages_Indicator_MA MA_5_Selected
        {
            get { return _MA_5_Selected; }
            set
            {
                _MA_5_Selected = value;
            }
        }


        /// <summary>
        /// </summary>
        [Description("Period for the MA 5")]
        [Category("Parameters")]
        [DisplayName("5.2 Period MA")]
        public int MA_5
        {
            get { return _ma_5; }
            set
            {
                _ma_5 = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("Line Width of MA 5.")]
        [Category("Plots")]
        [DisplayName("LW MA 5")]
        public int LineWidth_5
        {
            get { return _linewidth_5; }
            set { _linewidth_5 = Math.Max(1, value); }
        }


        /// <summary>
        /// </summary>
        [Description("DashStyle for MA 5.")]
        [Category("Plots")]
        [DisplayName("DS MA 5")]
        public DashStyle DashStyle_5
        {
            get { return _linestyle_5; }
            set { _linestyle_5 = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Color for MA 5")]
        [Category("Plots")]
        [DisplayName("Color MA 5")]
        public Color Color_5
        {
            get { return _col_5; }
            set { _col_5 = value; }
        }

        [Browsable(false)]
        public string Color_5_Serialize
        {
            get { return SerializableColor.ToString(_col_5); }
            set { _col_5 = SerializableColor.FromString(value); }
        }


        /// <summary>
        /// </summary>
        [Description("Select the type of MA 6 you would like to use")]
        [Category("Parameters")]
        [DisplayName("6.1 Type of MA")]
        public Enum_Moving_Averages_Indicator_MA MA_6_Selected
        {
            get { return _MA_6_Selected; }
            set
            {
                _MA_6_Selected = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("Period for the MA 6")]
        [Category("Parameters")]
        [DisplayName("6.2 Period MA")]
        public int MA_6
        {
            get { return _ma_6; }
            set
            {
                _ma_6 = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("Line Width of MA 6.")]
        [Category("Plots")]
        [DisplayName("LW MA 6")]
        public int LineWidth_6
        {
            get { return _linewidth_6; }
            set { _linewidth_6 = Math.Max(1, value); }
        }


        /// <summary>
        /// </summary>
        [Description("DashStyle for MA 6.")]
        [Category("Plots")]
        [DisplayName("DS MA 6")]
        public DashStyle DashStyle_6
        {
            get { return _linestyle_6; }
            set { _linestyle_6 = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Color for MA 6")]
        [Category("Plots")]
        [DisplayName("Color MA 6")]
        public Color Color_6
        {
            get { return _col_6; }
            set { _col_6 = value; }
        }

        [Browsable(false)]
        public string Color_6_Serialize
        {
            get { return SerializableColor.ToString(_col_6); }
            set { _col_6 = SerializableColor.FromString(value); }
        }

        #endregion

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Plot_1 { get { return Outputs[0]; } }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Plot_2 { get { return Outputs[1]; } }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Plot_3 { get { return Outputs[2]; } }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Plot_4 { get { return Outputs[3]; } }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Plot_5 { get { return Outputs[4]; } }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Plot_6 { get { return Outputs[5]; } }


        #endregion
    }
}


#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator
	{
		/// <summary>
		/// Use 5 different SMA or EMA at the same time in one indicator.
		/// </summary>
		public Moving_Averages_Indicator Moving_Averages_Indicator(Enum_Moving_Averages_Indicator_MA mA_1_Selected, System.Int32 mA_1, Enum_Moving_Averages_Indicator_MA mA_2_Selected, System.Int32 mA_2, Enum_Moving_Averages_Indicator_MA mA_3_Selected, System.Int32 mA_3, Enum_Moving_Averages_Indicator_MA mA_4_Selected, System.Int32 mA_4, Enum_Moving_Averages_Indicator_MA mA_5_Selected, System.Int32 mA_5, Enum_Moving_Averages_Indicator_MA mA_6_Selected, System.Int32 mA_6)
        {
			return Moving_Averages_Indicator(InSeries, mA_1_Selected, mA_1, mA_2_Selected, mA_2, mA_3_Selected, mA_3, mA_4_Selected, mA_4, mA_5_Selected, mA_5, mA_6_Selected, mA_6);
		}

		/// <summary>
		/// Use 5 different SMA or EMA at the same time in one indicator.
		/// </summary>
		public Moving_Averages_Indicator Moving_Averages_Indicator(IDataSeries input, Enum_Moving_Averages_Indicator_MA mA_1_Selected, System.Int32 mA_1, Enum_Moving_Averages_Indicator_MA mA_2_Selected, System.Int32 mA_2, Enum_Moving_Averages_Indicator_MA mA_3_Selected, System.Int32 mA_3, Enum_Moving_Averages_Indicator_MA mA_4_Selected, System.Int32 mA_4, Enum_Moving_Averages_Indicator_MA mA_5_Selected, System.Int32 mA_5, Enum_Moving_Averages_Indicator_MA mA_6_Selected, System.Int32 mA_6)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Moving_Averages_Indicator>(input, i => i.MA_1_Selected == mA_1_Selected && i.MA_1 == mA_1 && i.MA_2_Selected == mA_2_Selected && i.MA_2 == mA_2 && i.MA_3_Selected == mA_3_Selected && i.MA_3 == mA_3 && i.MA_4_Selected == mA_4_Selected && i.MA_4 == mA_4 && i.MA_5_Selected == mA_5_Selected && i.MA_5 == mA_5 && i.MA_6_Selected == mA_6_Selected && i.MA_6 == mA_6);

			if (indicator != null)
				return indicator;

			indicator = new Moving_Averages_Indicator
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input,
							MA_1_Selected = mA_1_Selected,
							MA_1 = mA_1,
							MA_2_Selected = mA_2_Selected,
							MA_2 = mA_2,
							MA_3_Selected = mA_3_Selected,
							MA_3 = mA_3,
							MA_4_Selected = mA_4_Selected,
							MA_4 = mA_4,
							MA_5_Selected = mA_5_Selected,
							MA_5 = mA_5,
							MA_6_Selected = mA_6_Selected,
							MA_6 = mA_6
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
		/// Use 5 different SMA or EMA at the same time in one indicator.
		/// </summary>
		public Moving_Averages_Indicator Moving_Averages_Indicator(Enum_Moving_Averages_Indicator_MA mA_1_Selected, System.Int32 mA_1, Enum_Moving_Averages_Indicator_MA mA_2_Selected, System.Int32 mA_2, Enum_Moving_Averages_Indicator_MA mA_3_Selected, System.Int32 mA_3, Enum_Moving_Averages_Indicator_MA mA_4_Selected, System.Int32 mA_4, Enum_Moving_Averages_Indicator_MA mA_5_Selected, System.Int32 mA_5, Enum_Moving_Averages_Indicator_MA mA_6_Selected, System.Int32 mA_6)
		{
			return LeadIndicator.Moving_Averages_Indicator(InSeries, mA_1_Selected, mA_1, mA_2_Selected, mA_2, mA_3_Selected, mA_3, mA_4_Selected, mA_4, mA_5_Selected, mA_5, mA_6_Selected, mA_6);
		}

		/// <summary>
		/// Use 5 different SMA or EMA at the same time in one indicator.
		/// </summary>
		public Moving_Averages_Indicator Moving_Averages_Indicator(IDataSeries input, Enum_Moving_Averages_Indicator_MA mA_1_Selected, System.Int32 mA_1, Enum_Moving_Averages_Indicator_MA mA_2_Selected, System.Int32 mA_2, Enum_Moving_Averages_Indicator_MA mA_3_Selected, System.Int32 mA_3, Enum_Moving_Averages_Indicator_MA mA_4_Selected, System.Int32 mA_4, Enum_Moving_Averages_Indicator_MA mA_5_Selected, System.Int32 mA_5, Enum_Moving_Averages_Indicator_MA mA_6_Selected, System.Int32 mA_6)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.Moving_Averages_Indicator(input, mA_1_Selected, mA_1, mA_2_Selected, mA_2, mA_3_Selected, mA_3, mA_4_Selected, mA_4, mA_5_Selected, mA_5, mA_6_Selected, mA_6);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Use 5 different SMA or EMA at the same time in one indicator.
		/// </summary>
		public Moving_Averages_Indicator Moving_Averages_Indicator(Enum_Moving_Averages_Indicator_MA mA_1_Selected, System.Int32 mA_1, Enum_Moving_Averages_Indicator_MA mA_2_Selected, System.Int32 mA_2, Enum_Moving_Averages_Indicator_MA mA_3_Selected, System.Int32 mA_3, Enum_Moving_Averages_Indicator_MA mA_4_Selected, System.Int32 mA_4, Enum_Moving_Averages_Indicator_MA mA_5_Selected, System.Int32 mA_5, Enum_Moving_Averages_Indicator_MA mA_6_Selected, System.Int32 mA_6)
		{
			return LeadIndicator.Moving_Averages_Indicator(InSeries, mA_1_Selected, mA_1, mA_2_Selected, mA_2, mA_3_Selected, mA_3, mA_4_Selected, mA_4, mA_5_Selected, mA_5, mA_6_Selected, mA_6);
		}

		/// <summary>
		/// Use 5 different SMA or EMA at the same time in one indicator.
		/// </summary>
		public Moving_Averages_Indicator Moving_Averages_Indicator(IDataSeries input, Enum_Moving_Averages_Indicator_MA mA_1_Selected, System.Int32 mA_1, Enum_Moving_Averages_Indicator_MA mA_2_Selected, System.Int32 mA_2, Enum_Moving_Averages_Indicator_MA mA_3_Selected, System.Int32 mA_3, Enum_Moving_Averages_Indicator_MA mA_4_Selected, System.Int32 mA_4, Enum_Moving_Averages_Indicator_MA mA_5_Selected, System.Int32 mA_5, Enum_Moving_Averages_Indicator_MA mA_6_Selected, System.Int32 mA_6)
		{
			return LeadIndicator.Moving_Averages_Indicator(input, mA_1_Selected, mA_1, mA_2_Selected, mA_2, mA_3_Selected, mA_3, mA_4_Selected, mA_4, mA_5_Selected, mA_5, mA_6_Selected, mA_6);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Use 5 different SMA or EMA at the same time in one indicator.
		/// </summary>
		public Moving_Averages_Indicator Moving_Averages_Indicator(Enum_Moving_Averages_Indicator_MA mA_1_Selected, System.Int32 mA_1, Enum_Moving_Averages_Indicator_MA mA_2_Selected, System.Int32 mA_2, Enum_Moving_Averages_Indicator_MA mA_3_Selected, System.Int32 mA_3, Enum_Moving_Averages_Indicator_MA mA_4_Selected, System.Int32 mA_4, Enum_Moving_Averages_Indicator_MA mA_5_Selected, System.Int32 mA_5, Enum_Moving_Averages_Indicator_MA mA_6_Selected, System.Int32 mA_6)
		{
			return LeadIndicator.Moving_Averages_Indicator(InSeries, mA_1_Selected, mA_1, mA_2_Selected, mA_2, mA_3_Selected, mA_3, mA_4_Selected, mA_4, mA_5_Selected, mA_5, mA_6_Selected, mA_6);
		}

		/// <summary>
		/// Use 5 different SMA or EMA at the same time in one indicator.
		/// </summary>
		public Moving_Averages_Indicator Moving_Averages_Indicator(IDataSeries input, Enum_Moving_Averages_Indicator_MA mA_1_Selected, System.Int32 mA_1, Enum_Moving_Averages_Indicator_MA mA_2_Selected, System.Int32 mA_2, Enum_Moving_Averages_Indicator_MA mA_3_Selected, System.Int32 mA_3, Enum_Moving_Averages_Indicator_MA mA_4_Selected, System.Int32 mA_4, Enum_Moving_Averages_Indicator_MA mA_5_Selected, System.Int32 mA_5, Enum_Moving_Averages_Indicator_MA mA_6_Selected, System.Int32 mA_6)
		{
			return LeadIndicator.Moving_Averages_Indicator(input, mA_1_Selected, mA_1, mA_2_Selected, mA_2, mA_3_Selected, mA_3, mA_4_Selected, mA_4, mA_5_Selected, mA_5, mA_6_Selected, mA_6);
		}
	}

	#endregion

}

#endregion

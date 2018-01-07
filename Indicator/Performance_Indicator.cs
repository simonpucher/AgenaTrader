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
/// Version: 1.2.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// This indicator can be used in the scanner column to display the yearly or daily performance.
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{

    public enum PerformanceCalculationType
    {
        BarCount = 0,
        ThisYear = 1,
        SelectedDate = 2
    }

	[Description("Show the performance of an instrument in the scanner column. Indicator calculates bars backwards so you need to configure the timeframe correctly.")]
	public class Performance_Indicator : UserIndicator
	{
        //InSeries
        private int _barscount = 365;
        private DateTime _selecteddate = DateTime.Now.Date;
        private int _opacity = Const.DefaultOpacity;
        private Color _rangecolor = Color.LightBlue;
        private PerformanceCalculationType _PerformanceCalculationType = PerformanceCalculationType.BarCount;


        protected override void OnInit()
		{
			Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Orange), "Plot_Performance_Indicator"));

            //to get the latest data into the bars object
			CalculateOnClosedBar = false;
            IsOverlay = true;

            //Because of Backtesting reasons if we use the advanced mode we need at least two bars
            this.RequiredBarsCount = 400;
        }

		protected override void OnCalculate()
		{
            if (IsProcessingBarIndexLast)
            {
                //if (!this.Instrument.IsInMarketHours(DateTime.Now))
                //{
                //    Plot_Performance_Indicator.Set(0);
                //}
                //else
                //{

                //}
                IBar b = null;

                switch (this.PerformanceCalculationType)
                {
                    case PerformanceCalculationType.BarCount:
                        b = Bars[this.BarsCount];
                        break;
                    case PerformanceCalculationType.ThisYear:
                        b = Bars.Where(x => x.Time.Year != DateTime.Now.Year).LastOrDefault();                
                        break;
                    case PerformanceCalculationType.SelectedDate:
                        b = Bars.GetBar(this.SelectedDate);
                        break;
                }

                if (b != null)
                {
                    Plot_Performance_Indicator.Set(((Close[0] - b.Close) * 100) / b.Close);
                }

                if (Chart != null)
                {
                    IEnumerable<IBar> list = Bars.Where(x => x.Time >= b.Time).Where(x => x.Time <= Time[0]);
                    AddChartRectangle("ORBRect" + b.Time.Ticks, true, b.Time, list.Where(x => x.Low == list.Min(y => y.Low)).LastOrDefault().Low, Time[0], list.Where(x => x.High == list.Max(y => y.High)).LastOrDefault().High, Color.Aquamarine, Color.Aquamarine, 50);
                }

            }
        }


        public override string ToString()
        {
            return "Performance (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "Performance (I)";
            }
        }


        #region Properties

        [Description("Performance will be calculated x bars backwards. Insert 365 for the performance of the last year. Insert 1 for the perfomance since yesterday.")]
        [Category("Parameters")]
        [DisplayName("Bars backwards")]
        public int BarsCount
        {
            get { return _barscount; }
            set { _barscount = value; }
        }


        [Description("Performance will be calculated from today to a dedicated date. The close of this day is used.")]
        [Category("Parameters")]
        [DisplayName("Selected date")]
        public DateTime SelectedDate
        {
            get { return _selecteddate; }
            set { _selecteddate = value; }
        }

        [Description("Choose the type of calculation. BarCount = The calculation will use the bars for the calculation. ThisYear = Calculation will be done for today minus the close of the last bar of last year.")]
        [Category("Parameters")]
        [DisplayName("Type")]
        public PerformanceCalculationType PerformanceCalculationType
        {
            get { return _PerformanceCalculationType; }
            set { _PerformanceCalculationType = value; }
        }

        [Browsable(false)]
		[XmlIgnore()]
		public DataSeries Plot_Performance_Indicator
        {
			get { return Outputs[0]; }
		}


        /// <summary>
        /// </summary>
        [Description("Opacity for Drawing")]
        [Category("Colors")]
        [DisplayName("Opacity")]
        public int Opacity
        {
            get { return _opacity; }
            set
            {
                if (value >= 1 && value <= 100)
                {
                    _opacity = value;
                }
                else
                {
                    _opacity = Const.DefaultOpacity;
                }
            }
        }

        /// <summary>
        /// </summary>
        [Description("Color of the range between the current date and the dedicated date.")]
        [Category("Colors")]
        [DisplayName("Range Color")]
        public Color Color_ORB
        {
            get { return _rangecolor; }
            set { _rangecolor = value; }
        }

        [Browsable(false)]
        public string Color_ORBSerialize
        {
            get { return SerializableColor.ToString(_rangecolor); }
            set { _rangecolor = SerializableColor.FromString(value); }
        }



        #endregion
    }
}
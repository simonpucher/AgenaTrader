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
/// Version: 1.1.3
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// Opens web browser by clicking on the chart.
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Opens web browser by clicking on the chart.")]
    public class OpenBrowser_Utility_Tool : UserIndicator
    {
        #region Variables


        private RectangleF _rect;
        private RectangleF _rect2;
        //private Pen _pen = Pens.Black;
        private Brush _brush = Brushes.Gray;

        private bool _opengooglefinance = true;
        private bool _openmorningstar = true;
        private bool _openyahoofinance = true;
        private bool _openearningswhispers = true;
        private bool _openzacks = true;


        #endregion

        protected override void Initialize()
        {
            //Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
            Overlay = true;
            
        }


        protected override void OnStartUp()
        {

            // Add event listener
            if (ChartControl != null)
                ChartControl.ChartPanelMouseDown += OnChartPanelMouseDown;

        }


        protected override void OnBarUpdate()
        {

            if (this.IsCurrentBarLast)
            {
              _brush = Brushes.Green;
            }

        }

        protected override void OnTermination()
        {
            // Remove event listener
            if (ChartControl != null)
                ChartControl.ChartPanelMouseDown -= OnChartPanelMouseDown;
        }


        public override string DisplayName
        {
            get
            {
                return "Open Browser (T)";
            }
        }


        public override string ToString()
        {
            return "Open Browser (T)";
        }


        #region Events



        public override void Plot(Graphics g, Rectangle r, double min, double max)
        {
            if (Bars == null || ChartControl == null) return;

        
            //Only draw button if parameters are available.
            if (this.Instrument != null)
            {
                //Only stocks are possible to lookup
                if (this.Instrument.InstrumentType == InstrumentType.Stock)
                {
                    using (Font font1 = new Font("Arial", 8, FontStyle.Bold, GraphicsUnit.Point))
                    {
                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;

                        this.Core.GetDataDirectory();

                        Brush tempbrush = new SolidBrush(GlobalUtilities.AdjustOpacity(((SolidBrush)_brush).Color, 0.5F));

                        _rect = new RectangleF(r.Width - 100, 10, 86, 27);
                        g.FillRectangle(tempbrush, _rect);
                        g.DrawString("open browser", font1, Brushes.White, _rect, stringFormat);
                        _rect2 = new RectangleF(r.Width - 100, 40, 86, 27);

                    }
                }
               
            }
        }


        private void OnChartPanelMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            Point cursorPos = new Point(e.X, e.Y);
            if (_rect.Contains(cursorPos))
            {

                string symbol = this.Instrument.Symbol;
                string isin = this.Instrument.ISIN;

                //if (this.Instrument.InstrumentType == InstrumentType.CFD)
                //{

                //}

                if (this.OpenGoogleFinance)
                {
                    GUIHelper.OpenInBrowser("https://www.google.com/finance?q=" + symbol);
                }

                if (this.OpenMorningstar)
                {
                    GUIHelper.OpenInBrowser("http://beta.morningstar.com/search.html?q=" + isin);
                }

                if (this.OpenYahooFinance)
                {
                    GUIHelper.OpenInBrowser("https://finance.yahoo.com/quote/" + symbol);
                }

                if (this.OpenEarningswhispers)
                {
                    GUIHelper.OpenInBrowser("https://earningswhispers.com/stocks/" + symbol);
                }

                if (this.OpenZacks)
                {
                    GUIHelper.OpenInBrowser("https://www.zacks.com/search.php?q=" + Name);
                }


            }
            else
            {
                //nothing to do
            }

            this.OnBarUpdate();

        }



        #endregion



        #region Properties

        #region Output

        //[Browsable(false)]
        //    [XmlIgnore()]
        //    public DataSeries MyPlot1
        //    {
        //        get { return Values[0]; }
        //    }


        #endregion

        #region Input

        

    [Description("Opens Yahoo Finance with the current symbol displayed in the chart")]
        [Category("Parameters")]
        [DisplayName("Yahoo Finance")]
        public bool OpenYahooFinance
        {
            get { return _openyahoofinance; }
            set { _openyahoofinance = value; }
        }

        [Description("Opens Google Finance with the current symbol displayed in the chart")]
        [Category("Parameters")]
        [DisplayName("Google Finance")]
        public bool OpenGoogleFinance
        {
            get { return _opengooglefinance; }
            set { _opengooglefinance = value; }
        }

        [Description("Opens Morningstar with the current symbol displayed in the chart")]
        [Category("Parameters")]
        [DisplayName("Morningstar")]
        public bool OpenMorningstar
        {
            get { return _openmorningstar; }
            set { _openmorningstar = value; }
        }

        [Description("Opens Earnings whispers with the current symbol displayed in the chart")]
        [Category("Parameters")]
        [DisplayName("Earnings whispers")]
        public bool OpenEarningswhispers
        {
            get { return _openearningswhispers; }
            set { _openearningswhispers = value; }
        }

        
  [Description("Opens Zacks with the current symbol displayed in the chart")]
        [Category("Parameters")]
        [DisplayName("Zacks")]
        public bool OpenZacks
        {
            get { return _openzacks; }
            set { _openzacks = value; }
        }

        private Color _col_positive = Color.Green;
        /// <summary>
        /// </summary>
        [Description("Color for in the list")]
        [Category("Plots")]
        [DisplayName("Color positive")]
        public Color Color_Positive
        {
            get { return _col_positive; }
            set { _col_positive = value; }
        }

        [Browsable(false)]
        public string Color_Positive_Serialize
        {
            get { return SerializableColor.ToString(_col_positive); }
            set { _col_positive = SerializableColor.FromString(value); }
        }


        private Color _col_negative = Color.Gray;
        /// <summary>
        /// </summary>
        [Description("Color for not in the list")]
        [Category("Plots")]
        [DisplayName("Color negative")]
        public Color Color_Negative
        {
            get { return _col_negative; }
            set { _col_negative = value; }
        }

        [Browsable(false)]
        public string Color_Negative_Serialize
        {
            get { return SerializableColor.ToString(_col_negative); }
            set { _col_negative = SerializableColor.FromString(value); }
        }



        #endregion



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
		/// Opens web browser by clicking on the chart.
		/// </summary>
		public OpenBrowser_Utility_Tool OpenBrowser_Utility_Tool(System.Boolean openYahooFinance, System.Boolean openGoogleFinance, System.Boolean openMorningstar, System.Boolean openEarningswhispers, System.Boolean openZacks)
        {
			return OpenBrowser_Utility_Tool(Input, openYahooFinance, openGoogleFinance, openMorningstar, openEarningswhispers, openZacks);
		}

		/// <summary>
		/// Opens web browser by clicking on the chart.
		/// </summary>
		public OpenBrowser_Utility_Tool OpenBrowser_Utility_Tool(IDataSeries input, System.Boolean openYahooFinance, System.Boolean openGoogleFinance, System.Boolean openMorningstar, System.Boolean openEarningswhispers, System.Boolean openZacks)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<OpenBrowser_Utility_Tool>(input, i => i.OpenYahooFinance == openYahooFinance && i.OpenGoogleFinance == openGoogleFinance && i.OpenMorningstar == openMorningstar && i.OpenEarningswhispers == openEarningswhispers && i.OpenZacks == openZacks);

			if (indicator != null)
				return indicator;

			indicator = new OpenBrowser_Utility_Tool
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							OpenYahooFinance = openYahooFinance,
							OpenGoogleFinance = openGoogleFinance,
							OpenMorningstar = openMorningstar,
							OpenEarningswhispers = openEarningswhispers,
							OpenZacks = openZacks
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
		/// Opens web browser by clicking on the chart.
		/// </summary>
		public OpenBrowser_Utility_Tool OpenBrowser_Utility_Tool(System.Boolean openYahooFinance, System.Boolean openGoogleFinance, System.Boolean openMorningstar, System.Boolean openEarningswhispers, System.Boolean openZacks)
		{
			return LeadIndicator.OpenBrowser_Utility_Tool(Input, openYahooFinance, openGoogleFinance, openMorningstar, openEarningswhispers, openZacks);
		}

		/// <summary>
		/// Opens web browser by clicking on the chart.
		/// </summary>
		public OpenBrowser_Utility_Tool OpenBrowser_Utility_Tool(IDataSeries input, System.Boolean openYahooFinance, System.Boolean openGoogleFinance, System.Boolean openMorningstar, System.Boolean openEarningswhispers, System.Boolean openZacks)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.OpenBrowser_Utility_Tool(input, openYahooFinance, openGoogleFinance, openMorningstar, openEarningswhispers, openZacks);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Opens web browser by clicking on the chart.
		/// </summary>
		public OpenBrowser_Utility_Tool OpenBrowser_Utility_Tool(System.Boolean openYahooFinance, System.Boolean openGoogleFinance, System.Boolean openMorningstar, System.Boolean openEarningswhispers, System.Boolean openZacks)
		{
			return LeadIndicator.OpenBrowser_Utility_Tool(Input, openYahooFinance, openGoogleFinance, openMorningstar, openEarningswhispers, openZacks);
		}

		/// <summary>
		/// Opens web browser by clicking on the chart.
		/// </summary>
		public OpenBrowser_Utility_Tool OpenBrowser_Utility_Tool(IDataSeries input, System.Boolean openYahooFinance, System.Boolean openGoogleFinance, System.Boolean openMorningstar, System.Boolean openEarningswhispers, System.Boolean openZacks)
		{
			return LeadIndicator.OpenBrowser_Utility_Tool(input, openYahooFinance, openGoogleFinance, openMorningstar, openEarningswhispers, openZacks);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Opens web browser by clicking on the chart.
		/// </summary>
		public OpenBrowser_Utility_Tool OpenBrowser_Utility_Tool(System.Boolean openYahooFinance, System.Boolean openGoogleFinance, System.Boolean openMorningstar, System.Boolean openEarningswhispers, System.Boolean openZacks)
		{
			return LeadIndicator.OpenBrowser_Utility_Tool(Input, openYahooFinance, openGoogleFinance, openMorningstar, openEarningswhispers, openZacks);
		}

		/// <summary>
		/// Opens web browser by clicking on the chart.
		/// </summary>
		public OpenBrowser_Utility_Tool OpenBrowser_Utility_Tool(IDataSeries input, System.Boolean openYahooFinance, System.Boolean openGoogleFinance, System.Boolean openMorningstar, System.Boolean openEarningswhispers, System.Boolean openZacks)
		{
			return LeadIndicator.OpenBrowser_Utility_Tool(input, openYahooFinance, openGoogleFinance, openMorningstar, openEarningswhispers, openZacks);
		}
	}

	#endregion

}

#endregion

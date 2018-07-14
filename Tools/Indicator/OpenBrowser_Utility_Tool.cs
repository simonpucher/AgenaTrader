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
/// Opens web browser by clicking on the chart.
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this indicator without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Opens web browser by clicking on a button on the chart.")]
    public class OpenBrowser_Utility_Tool : UserIndicator
    {
        #region Variables


        private RectangleF _rect;
        private RectangleF _rect2;
        //private Pen _pen = Pens.Black;
        private Brush _brush = Brushes.Gray;

        private bool _opengooglefinance = true;
        private bool _openmorningstar = true;
        private bool _openmorningstar_direct = true;
        private bool _openyahoofinance = true;
        private bool _openearningswhispers = true;
        private bool _openzacks = true;
        private bool _openzacks_direct = true;


        #endregion

        protected override void OnInit()
        {
            //Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
            IsOverlay = true;
            
        }


        protected override void OnStart()
        {

            // Add event listener
            if (Chart != null)
                Chart.ChartPanelMouseDown += OnChartPanelMouseDown;

        }


        protected override void OnCalculate()
        {

            //if (this.IsProcessingBarIndexLast && this.Instrument.InstrumentType == InstrumentType.Stock)
            //{
            //  _brush = Brushes.Green;
            //}

        }

        protected override void OnDispose()
        {
            // Remove event listener
            if (Chart != null)
                Chart.ChartPanelMouseDown -= OnChartPanelMouseDown;
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



        public override void OnPaint(Graphics g, Rectangle r, double min, double max)
        {
            if (Bars == null || Chart == null) return;

        
            //Only draw button if parameters are available.
            if (this.Instrument != null)
            {
                string strtext = "open browser";
                //Only stocks are possible to lookup
                if (this.Instrument.InstrumentType == InstrumentType.Stock)
                {
                    _brush = Brushes.Green;
                }
                else
                {
                    _brush = Brushes.Gray;
                    strtext = "not supported";
                }
                using (Font font1 = new Font("Arial", 8, FontStyle.Bold, GraphicsUnit.Point))
                    {
                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;

                        this.Core.GetDataDirectory();
                        
                        Brush tempbrush = new SolidBrush(GlobalUtilities.AdjustOpacity(((SolidBrush)_brush).Color, 0.5F));

                        _rect = new RectangleF(r.Width - 100, 10, 86, 27);
                        g.FillRectangle(tempbrush, _rect);
                        g.DrawString(strtext, font1, Brushes.White, _rect, stringFormat);
                        _rect2 = new RectangleF(r.Width - 100, 40, 86, 27);

                }
               

               
            }
        }


        private void OnChartPanelMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            Point cursorPos = new Point(e.X, e.Y);
            if (_rect.Contains(cursorPos) && this.Instrument.InstrumentType == InstrumentType.Stock)
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

                if (this.OpenMorningstar_Direct)
                {
                    //GUIHelper.OpenInBrowser("http://quote.morningstar.com/Quote.html?ticker=" + symbol);
                    GUIHelper.OpenInBrowser("http://quote.morningstar.com/Quote.html?t=" + isin);
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
                    GUIHelper.OpenInBrowser("https://www.zacks.com/search.php?q=" + symbol);
                }

                if (this.OpenZacks_Direct)
                {
                    GUIHelper.OpenInBrowser("https://www.zacks.com/stock/quote/" + symbol);
                }


            }
            else
            {
                //nothing to do
            }

            this.OnCalculate();

        }



        #endregion



        #region Properties

        #region Output

        //[Browsable(false)]
        //    [XmlIgnore()]
        //    public DataSeries MyPlot1
        //    {
        //        get { return Outputs[0]; }
        //    }


        #endregion

        #region InSeries

        

    [Description("Opens Yahoo Finance with the current symbol displayed in the chart")]
        [InputParameter]
        [DisplayName("Yahoo Finance")]
        public bool OpenYahooFinance
        {
            get { return _openyahoofinance; }
            set { _openyahoofinance = value; }
        }

        [Description("Opens Google Finance with the current symbol displayed in the chart")]
        [InputParameter]
        [DisplayName("Google Finance")]
        public bool OpenGoogleFinance
        {
            get { return _opengooglefinance; }
            set { _opengooglefinance = value; }
        }

        [Description("Opens Morningstar with the current symbol displayed in the chart")]
        [InputParameter]
        [DisplayName("Morningstar")]
        public bool OpenMorningstar
        {
            get { return _openmorningstar; }
            set { _openmorningstar = value; }
        }

        [Description("Opens Morningstar Direct with the current symbol displayed in the chart")]
        [InputParameter]
        [DisplayName("Morningstar Direct")]
        public bool OpenMorningstar_Direct
        {
            get { return _openmorningstar_direct; }
            set { _openmorningstar_direct = value; }
        }

        

        [Description("Opens Earnings whispers with the current symbol displayed in the chart")]
        [InputParameter]
        [DisplayName("Earnings whispers")]
        public bool OpenEarningswhispers
        {
            get { return _openearningswhispers; }
            set { _openearningswhispers = value; }
        }

        
  [Description("Opens Zacks with the current symbol displayed in the chart")]
        [InputParameter]
        [DisplayName("Zacks")]
        public bool OpenZacks
        {
            get { return _openzacks; }
            set { _openzacks = value; }
        }

        [Description("Opens Zacks Direct with the current symbol displayed in the chart")]
        [InputParameter]
        [DisplayName("Zacks Direct")]
        public bool OpenZacks_Direct
        {
            get { return _openzacks_direct; }
            set { _openzacks_direct = value; }
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
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
/// Version: 1.1
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

        private IInstrumentsList _list = null;
        private RectangleF _rect;
        private RectangleF _rect2;
        //private Pen _pen = Pens.Black;
        private Brush _brush = Brushes.Gray;
        

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
            //MyPlot1.Set(Input[0]);

            if (this.IsCurrentBarLast)
            {
              _brush = Brushes.Green;
//                if (_list.Contains((Instrument)this.Instrument))
//                {
//                    //_pen = Pens.Red;
//                    _brush = Brushes.Green;
//                }
//                else {
//                    //_pen = Pens.Black;
//                    _brush = Brushes.Gray;
//                }
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

             
            
                using (Font font1 = new Font("Arial", 8, FontStyle.Bold, GraphicsUnit.Point))
                {
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;

//                    if (String.IsNullOrEmpty(Shortcut_list))
//                    {
//                        if (this.Name_of_list.Count() >= 5)
//                        {
//                            this.Shortcut_list = this.Name_of_list.Substring(0, 5);
//                        }
//                        else
//                        {
//                            this.Shortcut_list = this.Name_of_list;
//                        }
//                    }

                    this.Core.GetDataDirectory();

                    Brush tempbrush = new SolidBrush(GlobalUtilities.AdjustOpacity(((SolidBrush)_brush).Color, 0.5F));

                    _rect = new RectangleF(r.Width - 100, 10, 86, 27);
                    g.FillRectangle(tempbrush, _rect);
                    g.DrawString("open browser", font1, Brushes.White, _rect, stringFormat);
                    _rect2 = new RectangleF(r.Width - 100, 40, 86, 27);
                  
                    //g.DrawRectangle(_pen, Rectangle.Round(_rect));
                }
            }
        }


        private void OnChartPanelMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //Print("X = {0}, Y = {1}", e.X, e.Y);
            //Print("X = {0}, Y = {1}", ChartControl.GetDateTimeByX(e.X), ChartControl.GetPriceByY(e.Y));

            Point cursorPos = new Point(e.X, e.Y);
            if (_rect.Contains(cursorPos))
            {
                GUIHelper.OpenInBrowser("https://www.google.com/finance?q="+this.Instrument.Symbol);
                //http://beta.morningstar.com/search.html?q=da

                //                if (!_list.Contains((Instrument)this.Instrument))
                //                {
                //                    this.Root.Core.InstrumentManager.AddInstrument2List(this.Instrument, this.Name_of_list);
                //                }
                //                else
                //                {
                //                    this.Root.Core.InstrumentManager.RemoveInstrumentFromList(this.Name_of_list, this.Instrument);
                //                }
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
		public OpenBrowser_Utility_Tool OpenBrowser_Utility_Tool()
        {
			return OpenBrowser_Utility_Tool(Input);
		}

		/// <summary>
		/// Opens web browser by clicking on the chart.
		/// </summary>
		public OpenBrowser_Utility_Tool OpenBrowser_Utility_Tool(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<OpenBrowser_Utility_Tool>(input);

			if (indicator != null)
				return indicator;

			indicator = new OpenBrowser_Utility_Tool
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
		/// Opens web browser by clicking on the chart.
		/// </summary>
		public OpenBrowser_Utility_Tool OpenBrowser_Utility_Tool()
		{
			return LeadIndicator.OpenBrowser_Utility_Tool(Input);
		}

		/// <summary>
		/// Opens web browser by clicking on the chart.
		/// </summary>
		public OpenBrowser_Utility_Tool OpenBrowser_Utility_Tool(IDataSeries input)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.OpenBrowser_Utility_Tool(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Opens web browser by clicking on the chart.
		/// </summary>
		public OpenBrowser_Utility_Tool OpenBrowser_Utility_Tool()
		{
			return LeadIndicator.OpenBrowser_Utility_Tool(Input);
		}

		/// <summary>
		/// Opens web browser by clicking on the chart.
		/// </summary>
		public OpenBrowser_Utility_Tool OpenBrowser_Utility_Tool(IDataSeries input)
		{
			return LeadIndicator.OpenBrowser_Utility_Tool(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Opens web browser by clicking on the chart.
		/// </summary>
		public OpenBrowser_Utility_Tool OpenBrowser_Utility_Tool()
		{
			return LeadIndicator.OpenBrowser_Utility_Tool(Input);
		}

		/// <summary>
		/// Opens web browser by clicking on the chart.
		/// </summary>
		public OpenBrowser_Utility_Tool OpenBrowser_Utility_Tool(IDataSeries input)
		{
			return LeadIndicator.OpenBrowser_Utility_Tool(input);
		}
	}

	#endregion

}

#endregion

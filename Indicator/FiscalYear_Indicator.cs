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
using System.Runtime.CompilerServices;


/// <summary>
/// Version: 1.1.2
/// -------------------------------------------------------------------------
/// Simon Pucher 2018
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
  
    
    [Description("Show seasonal trends")]
	public class FiscalYear_Indicator : UserIndicator
	{

        #region Variables

        private int _vertical_line_width = 1;
        private Color _color_vertical_line_FiscalEnd = Color.DarkViolet;
        private int _horizontal_line_width = 2;
        private Color _color_horizontal_line_FiscalEnd = Color.DarkViolet;
        private DashStyle _horizontal_dashstyle = DashStyle.Dash;
        private DashStyle _vertical_dashstyle = DashStyle.Dash;

        private int _year = 0;

        #endregion


        protected override void OnInit()
		{
			//Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
			IsOverlay = true;
		}

        protected override void OnBarsRequirements()
        {
            //Print("InitRequirements");
        }
   

        protected override void OnStart()
        {
            CalculateOnClosedBar = true;
            IsOverlay = true;
        }

		protected override void OnCalculate()
		{
            if (_year == 0)
            {
                _year = Time[0].Year;
            }

            //DateTime lastDayOfYear = new DateTime(Time[0].Year + 1, 1, 1).AddDays(-1);

            if (_year < Time[0].Year)
            {
                AddChartVerticalLine("vline" + Time[0].Date.ToString(), 0, this.Color_Vertical_Line_FiscalEnd, this.Vertical_DashStyle, this.Vertical_Line_Width);
                //AddChartText("txt" + Time[0].Date.ToString(), Time[0].Year.ToString(), ProcessingBarIndexes[0] - Bars.Count() + 1, Low[0], this.Color_Horizontal_Line_FiscalEnd);
                AddChartText("txt" + Time[0].Date.ToString(), Time[0].Year.ToString(), ProcessingBarIndexes[0] - Chart.LastBarVisible + 1, Close[0], this.Color_Horizontal_Line_FiscalEnd);
                AddChartLine("hline" + Time[0].ToString(), 0, Close[0], ProcessingBarIndexes[0]-Bars.Count()+1, Close[0], this.Color_Horizontal_Line_FiscalEnd);
                _year = Time[0].Year;
                
            }
            
        }

      

        public override string ToString()
        {
            return "Fiscal Year End (I)";
        }

        public override string DisplayName
        {
            get
            {
                  return "Fiscal Year End (I)";
            }
        }


        /// <summary>
        /// True if the periodicity of the data feed is correct for this indicator.
        /// </summary>
        /// <returns></returns>
        public bool DatafeedPeriodicityIsValid(ITimeFrame timeframe)
        {
            TimeFrame tf = (TimeFrame)timeframe;
            if (tf.Periodicity == DatafeedHistoryPeriodicity.Day && tf.PeriodicityValue == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        #region Properties

        /// <summary>
        /// </summary>
        [Description("Select the Dash Style for the vertical line.")]
        [Category("Color")]
        [DisplayName("Dash Style Vertical")]
        public DashStyle Vertical_DashStyle
        {
            get { return _vertical_dashstyle; }
            set { _vertical_dashstyle = value; }
        }

        /// <summary>
        /// </summary>
        [Description("Select the Dash Style for the horizontal line.")]
        [Category("Color")]
        [DisplayName("Dash Style Horizontal")]
        public DashStyle Horizontal_DashStyle
        {
            get { return _horizontal_dashstyle; }
            set { _horizontal_dashstyle = value; }
        }

        /// <summary>
        /// </summary>
        [Description("Select the width for the vertical line.")]
        [Category("Color")]
        [DisplayName("Width Vertical")]
        public int Vertical_Line_Width
        {
            get { return _vertical_line_width; }
            set { _vertical_line_width = value; }
        }

        /// <summary>
        /// </summary>
        [Description("Select the width for the horizontal line.")]
        [Category("Color")]
        [DisplayName("Width Horizontal")]
        public int Horizontal_Line_Width
        {
            get { return _horizontal_line_width; }
            set { _horizontal_line_width = value; }
        }

        /// <summary>
        /// </summary>
        [Description("Select Color for the vertical line.")]
        [Category("Color")]
        [DisplayName("Color Vertical")]
        public Color Color_Vertical_Line_FiscalEnd
        {
            get { return _color_vertical_line_FiscalEnd; }
            set { _color_vertical_line_FiscalEnd = value; }
        }


        // Serialize Color object
        [Browsable(false)]
        public string Color_Vertical_Line_FiscalEndSerialize
        {
            get { return SerializableColor.ToString(_color_vertical_line_FiscalEnd); }
            set { _color_vertical_line_FiscalEnd = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Select Color for the horizontal line.")]
        [Category("Color")]
        [DisplayName("Color horizontal")]
        public Color Color_Horizontal_Line_FiscalEnd
        {
            get { return _color_horizontal_line_FiscalEnd; }
            set { _color_horizontal_line_FiscalEnd = value; }
        }


        // Serialize Color object
        [Browsable(false)]
        public string Color_Horizontal_Line_FiscalEndSerialize
        {
            get { return SerializableColor.ToString(_color_horizontal_line_FiscalEnd); }
            set { _color_horizontal_line_FiscalEnd = SerializableColor.FromString(value); }
        }

        


        #endregion
    }
}
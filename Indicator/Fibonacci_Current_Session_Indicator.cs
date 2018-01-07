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
/// Version: 1.3.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// OutputDescriptors the Fibonacci Lines of the current session.
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
 
	[Description("Plots the Fibonacci Lines of the current session.")]
	public class Fibonacci_Current_Session : UserIndicator
	{
        //input
        private Color _Color_Fibo_Level_100 = Color.Red;
        private Color _Color_Fibo_Level_0 = Color.Green;
        private Color _Color_Fibo_Level_0_100 = Color.Gray;
        private Color _Color_Text = Color.Black;
        private int _Fibo_0_LineWidth = 3;
        private int _Fibo_0_100_LineWidth = 2;
        private int _Fibo_100_LineWidth = 3;
        private string _WhichLinesShouldWeUse = "0;23;38;50;61;76;78;100;";


        //output

        //internal

        protected override void OnBarsRequirements()
        {
            //  Print("InitRequirements");
        }

        protected override void OnStart()
        {
            // Print("OnStartUp");
        }

		protected override void OnInit()
		{
            CalculateOnClosedBar = true;
            IsOverlay = true;
		}

		protected override void OnCalculate()
		{
			//MyPlot1.Set(InSeries[0]);

            if (Bars != null && Bars.Count > 0 && IsProcessingBarIndexLast)
            {

                //Check if peridocity is valid for this script
                if (!DatafeedPeriodicityIsValid(Bars.TimeFrame))
                {
                    GlobalUtilities.DrawWarningTextOnChart(this, Const.DefaultStringDatafeedPeriodicity);
                    return;
                }
               
                DateTime start = Bars.Where(x => x.Time.Date == Bars[0].Time.Date).FirstOrDefault().Time;
                DateTime start_date = start.Date;
                DateTime end = Bars[0].Time;

                //Selektiere alle g�ltigen Kurse und finde low und high.
                IEnumerable<IBar> list = Bars.Where(x => x.Time >= start).Where(x => x.Time <= end);
                if (list != null && !list.IsEmpty())
                {
                    double minvalue = list.Where(x => x.Low == list.Min(y => y.Low)).LastOrDefault().Low;
                    double maxvalue = list.Where(x => x.High == list.Max(y => y.High)).LastOrDefault().High;
                    double range = maxvalue - minvalue;

                    DateTime enddrawing_string = end.AddSeconds(this.TimeFrame.GetSeconds() + this.TimeFrame.GetSeconds() * 0.15);
                    DateTime enddrawing_line = end.AddSeconds(this.TimeFrame.GetSeconds());

                    string[] arr_WhichLinesShouldWeUse = this.WhichLinesShouldWeUse.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    //Dictionary<string, string> dict = arr_WhichLinesShouldWeUse.ToDictionary<string, string>(x => x.ToString(), x => x.ToString());

                    if (Array.IndexOf(arr_WhichLinesShouldWeUse, "100") != -1)
                    {
                        AddChartText("Fibonacci_Session_LowLine_String", true, minvalue.ToString("N2") + " (100%)", enddrawing_string, minvalue, 0, this.Color_Text, new Font("Arial", 7.5f), StringAlignment.Far, Color.Transparent, Color.Transparent, 100);
                        AddChartLine("Fibonacci_Session_LowLine", true, start, minvalue, enddrawing_line , minvalue, this.Color_Fibo_Level_100, DashStyle.Solid, this.Fibo_100_LineWidth);
                    }

                    if (Array.IndexOf(arr_WhichLinesShouldWeUse, "0") != -1)
                    {
                        AddChartText("Fibonacci_Session_HighLine_String", true, maxvalue.ToString("N2") + " (0%)", enddrawing_string, maxvalue, 0, this.Color_Text, new Font("Arial", 7.5f), StringAlignment.Far, Color.Transparent, Color.Transparent, 100);
                        AddChartLine("Fibonacci_Session_HighLine", true, start, maxvalue, enddrawing_line, maxvalue, this.Color_Fibo_Level_0, DashStyle.Solid, this.Fibo_0_LineWidth);
                      
                    }

                    if (Array.IndexOf(arr_WhichLinesShouldWeUse, "23") != -1)
                    {
                        double _fibo_lv_2361 = maxvalue - ((range / 100) * 23.61);
                    AddChartText("Fibonacci_Session_23.61_String", true, _fibo_lv_2361.ToString("N2") + " (23.61%)", enddrawing_string, _fibo_lv_2361, 0, this.Color_Text, new Font("Arial", 7.5f), StringAlignment.Far, Color.Transparent, Color.Transparent, 100);
                    AddChartLine("Fibonacci_Session_23.61_Line", true, start, _fibo_lv_2361, enddrawing_line, _fibo_lv_2361, this.Color_Fibo_Level_0_100, DashStyle.Solid, this.Fibo_0_100_LineWidth);
 
                    }
                    if (Array.IndexOf(arr_WhichLinesShouldWeUse, "38") != -1)
                    {
                         double _fibo_lv_382 = maxvalue - ((range / 100) * 38.2);
                    AddChartText("Fibonacci_Session_38.2_String", true, _fibo_lv_382.ToString("N2") + " (38.2%)", enddrawing_string, _fibo_lv_382, 0, this.Color_Text, new Font("Arial", 7.5f), StringAlignment.Far, Color.Transparent, Color.Transparent, 100);
                    AddChartLine("Fibonacci_Session_38.2", true, start, _fibo_lv_382, enddrawing_line, _fibo_lv_382, this.Color_Fibo_Level_0_100, DashStyle.Solid, this.Fibo_0_100_LineWidth);
 
                    }
                    if (Array.IndexOf(arr_WhichLinesShouldWeUse, "50") != -1)
                    {
                         double _fibo_lv_500 = maxvalue - ((range / 100) * 50.0);
                    AddChartText("Fibonacci_Session_50.0_String", true, _fibo_lv_500.ToString("N2") + " (50.0%)", enddrawing_string, _fibo_lv_500, 0, this.Color_Text, new Font("Arial", 7.5f), StringAlignment.Far, Color.Transparent, Color.Transparent, 100);
                    AddChartLine("Fibonacci_Session_50.0", true, start, _fibo_lv_500, enddrawing_line, _fibo_lv_500, this.Color_Fibo_Level_0_100, DashStyle.Solid, this.Fibo_0_100_LineWidth);
 
                    }

                    if (Array.IndexOf(arr_WhichLinesShouldWeUse, "61") != -1)
                    {
                          double _fibo_lv_618 = maxvalue - ((range / 100) * 61.8);
                    AddChartText("Fibonacci_Session_61.8_String", true, _fibo_lv_618.ToString("N2") + " (61.8%)", enddrawing_string, _fibo_lv_618, 0, this.Color_Text, new Font("Arial", 7.5f), StringAlignment.Far, Color.Transparent, Color.Transparent, 100);
                    AddChartLine("Fibonacci_Session_61.8", true, start, _fibo_lv_618, enddrawing_line, _fibo_lv_618, this.Color_Fibo_Level_0_100, DashStyle.Solid, this.Fibo_0_100_LineWidth);

                    }
                    if (Array.IndexOf(arr_WhichLinesShouldWeUse, "76") != -1)
                    {
                        double _fibo_lv_7640 = maxvalue - ((range / 100) * 76.4);
                    AddChartText("Fibonacci_Session_76.40_String", true, _fibo_lv_7640.ToString("N2") + " (76.4%)", enddrawing_string, _fibo_lv_7640, 0, this.Color_Text, new Font("Arial", 7.5f), StringAlignment.Far, Color.Transparent, Color.Transparent, 100);
                    AddChartLine("Fibonacci_Session_76.40", true, start, _fibo_lv_7640, enddrawing_line, _fibo_lv_7640, this.Color_Fibo_Level_0_100, DashStyle.Solid, this.Fibo_0_100_LineWidth);
  
                    }
                    if (Array.IndexOf(arr_WhichLinesShouldWeUse, "78") != -1)
                    {
                       double _fibo_lv_7862 = maxvalue - ((range / 100) * 78.62);
                    AddChartText("Fibonacci_Session_78.62_String", true, _fibo_lv_7862.ToString("N2") + " (78.62%)", enddrawing_string, _fibo_lv_7862, 0, this.Color_Text, new Font("Arial", 7.5f), StringAlignment.Far, Color.Transparent, Color.Transparent, 100);
                    AddChartLine("Fibonacci_Session_78.62", true, start, _fibo_lv_7862, enddrawing_line, _fibo_lv_7862, this.Color_Fibo_Level_0_100, DashStyle.Solid, this.Fibo_0_100_LineWidth);
 
                    }
                }
            }
		}


        protected override void OnDispose()
        {
            //Print("OnTermination");

        }

        public override string ToString()
        {
            return "Fibonacci Current Session (I)";
        }

        public override string DisplayName
        {
            get
            {
                return "Fibonacci Current Session (I)";
            }
        }



        /// <summary>
        /// True if the periodicity of the data feed is correct for this indicator.
        /// </summary>
        /// <returns></returns>
        public bool DatafeedPeriodicityIsValid(ITimeFrame timeframe)
        {
            TimeFrame tf = (TimeFrame)timeframe;
            if (this.Bars.TimeFrame.Periodicity == DatafeedHistoryPeriodicity.Hour || this.Bars.TimeFrame.Periodicity == DatafeedHistoryPeriodicity.Minute
                || this.Bars.TimeFrame.Periodicity == DatafeedHistoryPeriodicity.Second || this.Bars.TimeFrame.Periodicity == DatafeedHistoryPeriodicity.Tick)
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
        [Description("Color Fibo Level 100%")]
        [Category("Drawing")]
        [DisplayName("Fibo Lvl 100%")]
        public Color Color_Fibo_Level_100
        {
            get { return _Color_Fibo_Level_100; }
            set { _Color_Fibo_Level_100 = value; }
        }

        [Browsable(false)]
        public string _Color_Fibo_Level_100_Serialize
        {
            get { return SerializableColor.ToString(_Color_Fibo_Level_100); }
            set { _Color_Fibo_Level_100 = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Color Fibo Level 0% - 100%")]
        [Category("Drawing")]
        [DisplayName("Fibo Lvls 0% - 100%")]
        public Color Color_Fibo_Level_0_100
        {
            get { return _Color_Fibo_Level_0_100; }
            set { _Color_Fibo_Level_0_100 = value; }
        }

        [Browsable(false)]
        public string Color_Fibo_Level_0_100_Serialize
        {
            get { return SerializableColor.ToString(_Color_Fibo_Level_0_100); }
            set { _Color_Fibo_Level_0_100 = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Color Fibo Level 0%")]
        [Category("Drawing")]
        [DisplayName("Fibo Lvl 0%")]
        public Color Color_Fibo_Level_0
        {
            get { return _Color_Fibo_Level_0; }
            set { _Color_Fibo_Level_0 = value; }
        }

        [Browsable(false)]
        public string Color_Fibo_Level_0_Serialize
        {
            get { return SerializableColor.ToString(_Color_Fibo_Level_0); }
            set { _Color_Fibo_Level_0 = SerializableColor.FromString(value); }
        }


        /// <summary>
        /// </summary>
        [Description("Color for the text")]
        [Category("Drawing")]
        [DisplayName("Text color")]
        public Color Color_Text
        {
            get { return _Color_Text; }
            set { _Color_Text = value; }
        }

        [Browsable(false)]
        public string Color_Text_Serialize
        {
            get { return SerializableColor.ToString(_Color_Text); }
            set { _Color_Text = SerializableColor.FromString(value); }
        }

     

        /// <summary>
        /// </summary>
        [Description("Width for the Fibo line 0%.")]
        [Category("Drawing")]
        [DisplayName("Line Width Fibo lvl 0%")]
        public int Fibo_0_LineWidth
        {
            get { return _Fibo_0_LineWidth; }
            set { _Fibo_0_LineWidth = Math.Max(1, value); }
        }

        /// <summary>
        /// </summary>
        [Description("Width for the Fibo line 0% - 100%.")]
        [Category("Drawing")]
        [DisplayName("Line Width Fibo lvl 0% - 100%")]
        public int Fibo_0_100_LineWidth
        {
            get { return _Fibo_0_100_LineWidth; }
            set { _Fibo_0_100_LineWidth = Math.Max(1, value); }
        }

        /// <summary>
        /// </summary>
        [Description("Width for the Fibo line 100%.")]
        [Category("Drawing")]
        [DisplayName("Line Width Fibo lvl 100%")]
        public int Fibo_100_LineWidth
        {
            get { return _Fibo_100_LineWidth; }
            set { _Fibo_100_LineWidth = Math.Max(1, value); }
        }

       
        /// <summary>
        /// </summary>
        [Description("Here you can activate and deactivate the fibo levels you like to see. If you want all, please use: 0;23;38;50;61;76;78;100;")]
        [Category("Drawing")]
        [DisplayName("Fibo Lvls to use")]
        public string WhichLinesShouldWeUse
        {
            get { return _WhichLinesShouldWeUse; }
            set { _WhichLinesShouldWeUse = value; }
        }


		#endregion
	}
}
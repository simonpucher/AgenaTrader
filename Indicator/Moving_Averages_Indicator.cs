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
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;


/// <summary>
/// Version: 1.3.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{

    [Description("Use 6 different MA like SMA, HMA, EMA, WMA, TEMA, TMA, WMA at the same time in one indicator.")]
    public class Moving_Averages_Indicator : UserIndicator
    {
        
        //input 
        private MAEnvelopesMAType _MA_1_Selected = MAEnvelopesMAType.SMA;
        private MAEnvelopesMAType _MA_2_Selected = MAEnvelopesMAType.SMA;
        private MAEnvelopesMAType _MA_3_Selected = MAEnvelopesMAType.SMA;
        private MAEnvelopesMAType _MA_4_Selected = MAEnvelopesMAType.EMA;
        private MAEnvelopesMAType _MA_5_Selected = MAEnvelopesMAType.EMA;
        private MAEnvelopesMAType _MA_6_Selected = MAEnvelopesMAType.EMA;

        private int _ma_1 = 50;
        private int _ma_2 = 100;
        private int _ma_3 = 200;
        private int _ma_4 = 0;
        private int _ma_5 = 0;
        private int _ma_6 = 0;

        private bool _1_over_2 = true;
        private bool _2_over_3 = true;
        private bool _3_over_4 = false;
        private bool _4_over_5 = false;
        private bool _5_over_6 = false;

        private bool _ShowSignalOnChartBackground = true;
        private bool _ShowSignalOnChartArrow = true;
        private bool _ShowSignalOnChartPercent = true;

        private int _linewidth_1 = 1;
        private DashStyle _linestyle_1 = DashStyle.Solid;
        private Color _col_1 = Color.Red;

        private int _linewidth_2 = 1;
        private DashStyle _linestyle_2 = DashStyle.Solid;
        private Color _col_2 = Color.Orange;

        private int _linewidth_3 = 1;
        private DashStyle _linestyle_3 = DashStyle.Solid;
        private Color _col_3 = Color.Blue;

        private int _linewidth_4 = 1;
        private DashStyle _linestyle_4 = DashStyle.Solid;
        private Color _col_4 = Color.Green;

        private int _linewidth_5 = 1;
        private DashStyle _linestyle_5 = DashStyle.Solid;
        private Color _col_5 = Color.DarkViolet;

        private int _linewidth_6 = 1;
        private DashStyle _linestyle_6 = DashStyle.Solid;
        private Color _col_6 = Color.DarkGoldenrod;

        private IntSeries _signals;
        private IntSeries _days;
        private DataSeries _percent;

        private Color _color_long_signal_background = Const.DefaultArrowLongColor;
        private Color _color_short_signal_background = Const.DefaultArrowShortColor;
        private Color _color_long_signal_arrow = Const.DefaultArrowLongColor;
        private Color _color_short_signal_arrow = Const.DefaultArrowShortColor;
        private int _opacity_long_signal = 25;
        private int _opacity_short_signal = 25;

        private DashStyle _plotdashstyleline = DashStyle.Dash;
        private DashStyle _plotdashstylelinelast = DashStyle.Dot;
        private int _plotwidthline = 2;
        private int _plotwidthlinelast = 2;

        protected override void OnInit()
        {
            
            Add(new OutputDescriptor(new Pen(this.Color_1, this.LineWidth_1), OutputSerieDrawStyle.Line, "MA_1"));
            Add(new OutputDescriptor(new Pen(this.Color_2, this.LineWidth_2), OutputSerieDrawStyle.Line, "MA_2"));
            Add(new OutputDescriptor(new Pen(this.Color_3, this.LineWidth_3), OutputSerieDrawStyle.Line, "MA_3"));
            Add(new OutputDescriptor(new Pen(this.Color_4, this.LineWidth_4), OutputSerieDrawStyle.Line, "MA_4"));
            Add(new OutputDescriptor(new Pen(this.Color_5, this.LineWidth_5), OutputSerieDrawStyle.Line, "MA_5"));
            Add(new OutputDescriptor(new Pen(this.Color_6, this.LineWidth_6), OutputSerieDrawStyle.Line, "MA_6"));

            CalculateOnClosedBar = false;
            IsOverlay = true;

            this.RequiredBarsCount = 200;
             
            _signals = new IntSeries(this);
            _days = new IntSeries(this);
            _percent = new DataSeries(this);

        }

        private double GetValue(MAEnvelopesMAType matype, int period)
        {
            switch (matype)
            {
                case MAEnvelopesMAType.SMA:
                    return SMA(period)[0];
                case MAEnvelopesMAType.EMA:
                    return EMA(period)[0];
                case MAEnvelopesMAType.WMA:
                    return WMA(period)[0];
                case MAEnvelopesMAType.HMA:
                    return HMA(period)[0];
                case MAEnvelopesMAType.TEMA:
                    return TEMA(period)[0];
                case MAEnvelopesMAType.TMA:
                    return TMA(period)[0];
                default:
                    throw new NotImplementedException();
            }
        }

        private double getpercent(int dayoffset) {
            int offset = Math.Abs(_days[dayoffset]) + dayoffset;
            double percent = (Close[dayoffset] - Close[offset]) / (Close[offset] / 100);
            return percent;
        }

        private void drawpercentlines(int dayoffset, DashStyle styleofline, int widthofline) {
            int offset = Math.Abs(_days[dayoffset]) + dayoffset;
            double percent = this.getpercent(dayoffset);

            Color _color = Color.Green;
            if (percent < 0) _color = Color.Red;
            int _offsetdrawingtext = 7;
            if (percent < 0) _offsetdrawingtext = _offsetdrawingtext * -3;
            string percstring = string.Format("{0:N2}% ({1})", percent, Math.Abs(_days[dayoffset]));
            AddChartText("lastsegmentpercentline" + Time[dayoffset], true, percstring, dayoffset, Close[dayoffset], _offsetdrawingtext, _color, new Font("Arial", 9, FontStyle.Bold), StringAlignment.Center, HorizontalAlignment.Right, VerticalAlignment.Bottom, _color, Color.White, 255);

            AddChartLine("drawaline" + Time[dayoffset], offset, Close[offset], dayoffset, Close[dayoffset], _color, styleofline, widthofline);
            //int offset = Math.Abs(_days[dayoffset]) + dayoffset;
            //double percent = (Close[dayoffset] - Close[offset]) / (Close[offset] / 100);
            //Color _color = Color.Green;
            //if (percent < 0) _color = Color.Red;
            //int _offsetdrawingtext = 7;
            //if (percent < 0) _offsetdrawingtext = _offsetdrawingtext * -3;
            //AddChartText("lastsegmentpercentline" + Time[dayoffset], true, string.Format("{0:N2}%", percent), dayoffset, Close[dayoffset], _offsetdrawingtext, _color, new Font("Arial", 9, FontStyle.Bold), StringAlignment.Center, HorizontalAlignment.Right, VerticalAlignment.Bottom, _color, Color.White, 255);
            //AddChartLine("drawaline" + Time[dayoffset], offset, Close[offset], dayoffset, Close[dayoffset], _color, styleofline, widthofline);
        }

        protected override void OnCalculate()
        {
            if (this.MA_1 != 0 && this.MA_1 > this.RequiredBarsCount ||
                this.MA_2 != 0 && this.MA_2 > this.RequiredBarsCount ||
                this.MA_3 != 0 && this.MA_3 > this.RequiredBarsCount ||
                this.MA_4 != 0 && this.MA_4 > this.RequiredBarsCount ||
                this.MA_5 != 0 && this.MA_5 > this.RequiredBarsCount ||
                this.MA_6 != 0 && this.MA_6 > this.RequiredBarsCount)
            {
                AddChartTextFixed("AlertText", "Required bars must be at least as high as the largest moving average period.", TextPosition.Center, Color.Red, new Font("Arial", 30), Color.Red, Color.Red, 20);
            }

            int _signal_value = 0;
            int _enabled_ifs = 0;

            if (this.MA_1 != 0) Plot_1.Set(this.GetValue(this.MA_1_Selected, this.MA_1));
            if (this.MA_2 != 0) Plot_2.Set(this.GetValue(this.MA_2_Selected, this.MA_2));
            if (this.MA_3 != 0) Plot_3.Set(this.GetValue(this.MA_3_Selected, this.MA_3));
            if (this.MA_4 != 0) Plot_4.Set(this.GetValue(this.MA_4_Selected, this.MA_4));
            if (this.MA_5 != 0) Plot_5.Set(this.GetValue(this.MA_5_Selected, this.MA_5));
            if (this.MA_6 != 0) Plot_6.Set(this.GetValue(this.MA_6_Selected, this.MA_6));

            //Signals 
            if (this.If_1_over_2)
            {
                _enabled_ifs++;
                if (this.MA_1 != 0 && this.MA_2 != 0 && Plot_1.Last() > Plot_2.Last())
                {
                    _signal_value++;
                }
                else if (this.MA_1 != 0 && this.MA_2 != 0 && Plot_1.Last() < Plot_2.Last())
                {
                    _signal_value--;
                }
            }

            if (this.If_2_over_3)
            {
                _enabled_ifs++;
                if (this.MA_2 != 0 && this.MA_3 != 0 && Plot_2.Last() > Plot_3.Last())
                {
                    _signal_value++;
                }
                else if (this.MA_2 != 0 && this.MA_3 != 0 && Plot_2.Last() < Plot_3.Last())
                {
                    _signal_value--;
                }
            }

            if (this.If_3_over_4)
            {
                _enabled_ifs++;
                if (this.MA_3 != 0 && this.MA_4 != 0 && Plot_3.Last() > Plot_4.Last())
                {
                    _signal_value++;
                }
                else if (this.MA_3 != 0 && this.MA_4 != 0 && Plot_3.Last() < Plot_4.Last())
                {
                    _signal_value--;
                }
            }

            if (this.If_4_over_5)
            {
                _enabled_ifs++;
                if (this.MA_4 != 0 && this.MA_5 != 0 && Plot_4.Last() > Plot_5.Last())
                {
                    _signal_value++;
                }
                else if (this.MA_4 != 0 && this.MA_5 != 0 && Plot_4.Last() < Plot_5.Last())
                {
                    _signal_value--;
                }
            }

            if (this.If_5_over_6)
            {
                _enabled_ifs++;
                if (this.MA_5 != 0 && this.MA_6 != 0 && Plot_5.Last() > Plot_6.Last())
                {
                    _signal_value++;
                }
                else if (this.MA_5 != 0 && this.MA_6 != 0 && Plot_5.Last() < Plot_6.Last())
                {
                    _signal_value--;
                }
            }

            
            //signal
            if (_signal_value == _enabled_ifs) _signals.Set(1);
            else if (_signal_value == _enabled_ifs * -1) _signals.Set(-1);
            else _signals.Set(0);

            //days
            if (_signals[0] == 0)
            {
                _days.Set(0);
            }
            else 
            {
                if (_signals[0] == 1 && _signals[1] == 1)
                {
                    _days.Set(_days[1] + 1);
                }
                else if (_signals[0] == -1 && _signals[1] == -1)
                {
                    _days.Set(_days[1] - 1);
                }
                else
                {
                    if (_signals[0] == 1 && _signals[1] == 0
                        || _signals[0] == 1 && _signals[1] == -1)
                    {
                        _days.Set(1);
                    }
                    else
                    {
                        _days.Set(-1);
                    }
                }

               
            }

            
            if (ShowSignalOnChartBackground)
            {
                //color an background
                if (_signals[0] == 1) this.BackColor = GlobalUtilities.AdjustOpacity(this.ColorLongSignalBackground , this.OpacityLongSignal / 100.0);
                else if (_signals[0] == -1) this.BackColor = GlobalUtilities.AdjustOpacity(this.ColorShortSignalBackground, this.OpacityShortSignal / 100.0);
            }

            //if (this.ShowSignalOnChartPercent)
            //{
            //    //percent on all signals with more _enabled_ifs
            //    if (_signals[0] == 0 && _signals[1] != 0)
            //    {
            //        this.drawpercentlines(1, this.DashStyleLine, this.PlotWidthLine);
            //    }
            //    else if (_signals[0] == 1 && _signals[1] == -1)
            //    {
            //        this.drawpercentlines(1, this.DashStyleLine, this.PlotWidthLine);
            //    }
            //    else if (_signals[0] == -1 && _signals[1] == 1)
            //    {
            //        this.drawpercentlines(1, this.DashStyleLine, this.PlotWidthLine);
            //    }

            //    //percent on last candle
            //    if (_signals[0] != 0 && IsProcessingBarIndexLast)
            //    {
            //        this.drawpercentlines(0, this.DashStyleLineLast, this.PlotWidthLineLast);
            //    }
            //}

            //percent
            //percent on all signals with more _enabled_ifs
            if (_signals[0] == 0 && _signals[1] != 0)
            {
                _percent.Set(this.getpercent(1));
                if (this.ShowSignalOnChartPercent)
                {
                    this.drawpercentlines(1, this.DashStyleLine, this.PlotWidthLine);
                }
            }
            else if (_signals[0] == 1 && _signals[1] == -1)
            {
                _percent.Set(this.getpercent(1));
                if (this.ShowSignalOnChartPercent)
                {
                    this.drawpercentlines(1, this.DashStyleLine, this.PlotWidthLine);
                }
            }
            else if (_signals[0] == -1 && _signals[1] == 1)
            {
                _percent.Set(this.getpercent(1));
                if (this.ShowSignalOnChartPercent)
                {
                    this.drawpercentlines(1, this.DashStyleLine, this.PlotWidthLine);
                }
            }

            //percent on last candle
            if (_signals[0] != 0 && IsProcessingBarIndexLast)
            {
                _percent.Set(this.getpercent(0));
                if (this.ShowSignalOnChartPercent)
                {
                    this.drawpercentlines(0, this.DashStyleLineLast, this.PlotWidthLineLast);
                }
            }


            if (this.ShowSignalOnChartArrow)
            {
                if (_signals[0] == 1 && _signals[1] != 1) AddChartArrowUp("ArrowLong_MA" + +Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].Low, this.ColorLongSignalArrow);
                else if (_signals[0] == -1 && _signals[1] != -1) AddChartArrowDown("ArrowShort_MA" + +Bars[0].Time.Ticks, this.IsAutoAdjustableScale, 0, Bars[0].High, this.ColorShortSignalArrow);
            }
            

            //Set the color
            PlotColors[0][0] = this.Color_1;
            OutputDescriptors[0].PenStyle = this.DashStyle_1;
            OutputDescriptors[0].Pen.Width = this.LineWidth_1;
            PlotColors[1][0] = this.Color_2;
            OutputDescriptors[1].PenStyle = this.DashStyle_2;
            OutputDescriptors[1].Pen.Width = this.LineWidth_2;
            PlotColors[2][0] = this.Color_3;
            OutputDescriptors[2].PenStyle = this.DashStyle_3;
            OutputDescriptors[2].Pen.Width = this.LineWidth_3;
            PlotColors[3][0] = this.Color_4;
            OutputDescriptors[3].PenStyle = this.DashStyle_4;
            OutputDescriptors[3].Pen.Width = this.LineWidth_4;
            PlotColors[4][0] = this.Color_5;
            OutputDescriptors[4].PenStyle = this.DashStyle_5;
            OutputDescriptors[4].Pen.Width = this.LineWidth_5;
            PlotColors[5][0] = this.Color_6;
            OutputDescriptors[5].PenStyle = this.DashStyle_6;
            OutputDescriptors[5].Pen.Width = this.LineWidth_6;

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
        public MAEnvelopesMAType MA_1_Selected
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
        public MAEnvelopesMAType MA_2_Selected
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
        public MAEnvelopesMAType MA_3_Selected
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
        public MAEnvelopesMAType MA_4_Selected
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
        public MAEnvelopesMAType MA_5_Selected
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
        public MAEnvelopesMAType MA_6_Selected
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

        [Browsable(false)]
        [XmlIgnore()]
        public IntSeries Signals { get { return _signals; } }

        [Browsable(false)]
        [XmlIgnore()]
        public IntSeries Days { get { return _days; } }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Percent { get { return _percent; } }

        /// <summary>
        /// </summary>
        [Description("Show signal strength on the chart (background).")]
        [Category("Background")]
        [DisplayName("Show background")]
        public bool ShowSignalOnChartBackground
        {
            get { return _ShowSignalOnChartBackground; }
            set
            {
                _ShowSignalOnChartBackground = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("Show signal strength on the chart (percent).")]
        [Category("Background")]
        [DisplayName("Show percent")]
        public bool ShowSignalOnChartPercent
        {
            get { return _ShowSignalOnChartPercent; }
            set
            {
                _ShowSignalOnChartPercent = value;
            }
        }

  

        /// <summary>
        /// </summary>
        [Description("Show signal strength on the chart (arrow).")]
        [Category("Background")]
        [DisplayName("Show arrow")]
        public bool ShowSignalOnChartArrow
        {
            get { return _ShowSignalOnChartArrow; }
            set
            {
                _ShowSignalOnChartArrow = value;
            }
        }
        

        /// <summary>
        /// </summary>
        [Description("If MA1 is larger than MA2.")]
        [Category("Conditions")]
        [DisplayName("MA1 > MA2")]
        public bool If_1_over_2
        {
            get { return _1_over_2; }
            set
            {
                _1_over_2 = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("If MA2 is larger than MA3.")]
        [Category("Conditions")]
        [DisplayName("MA2 > MA3")]
        public bool If_2_over_3
        {
            get { return _2_over_3; }
            set
            {
                _2_over_3 = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("If MA3 is larger than MA4.")]
        [Category("Conditions")]
        [DisplayName("MA3 > MA4")]
        public bool If_3_over_4
        {
            get { return _3_over_4; }
            set
            {
                _3_over_4 = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("If MA4 is larger than MA5.")]
        [Category("Conditions")]
        [DisplayName("MA4 > MA5")]
        public bool If_4_over_5
        {
            get { return _4_over_5; }
            set
            {
                _4_over_5 = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("If MA5 is larger than MA6.")]
        [Category("Conditions")]
        [DisplayName("MA5 > MA6")]
        public bool If_5_over_6
        {
            get { return _5_over_6; }
            set
            {
                _5_over_6 = value;
            }
        }


        /// <summary>
        /// </summary>
        [Description("Select opacity for the background in long setup in percent.")]
        [Category("Background")]
        [DisplayName("Opacity Background Long %")]
        public int OpacityLongSignal
        {
            get { return _opacity_long_signal; }
            set
            {
                if (value < 0) value = 0;
                if (value > 100) value = 100;
                _opacity_long_signal = value;
            }
        }

        /// <summary>
        /// </summary>
        [Description("Select opacity for the background in short setup in percent.")]
        [Category("Background")]
        [DisplayName("Opacity Background Short %")]
        public int OpacityShortSignal
        {
            get { return _opacity_short_signal; }
            set
            {
                if (value < 0) value = 0;
                if (value > 100) value = 100;
                _opacity_short_signal = value;
            }
        }

  
        /// <summary>
        /// </summary>
        [Description("Select Color for the background in long setup.")]
        [Category("Background")]
        [DisplayName("Color Background Long")]
        public Color ColorLongSignalBackground
        {
            get { return _color_long_signal_background; }
            set { _color_long_signal_background = value; }
        }

        
        // Serialize Color object
        [Browsable(false)]
        public string ColorLongSignalBackgroundSerialize
        {
            get { return SerializableColor.ToString(_color_long_signal_background); }
            set { _color_long_signal_background = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Select Color for the background in short setup.")]
        [Category("Background")]
        [DisplayName("Color Background Short")]
        public Color ColorShortSignalBackground
        {
            get { return _color_short_signal_background; }
            set { _color_short_signal_background = value; }
        }
        // Serialize Color object
        [Browsable(false)]
        public string ColorShortSignalBackgroundSerialize
        {
            get { return SerializableColor.ToString(_color_short_signal_background); }
            set { _color_short_signal_background = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Select Color for the arrow in long setup.")]
        [Category("Background")]
        [DisplayName("Color Arrow Long")]
        public Color ColorLongSignalArrow
        {
            get { return _color_long_signal_arrow; }
            set { _color_long_signal_arrow = value; }
        }


        // Serialize Color object
        [Browsable(false)]
        public string ColorLongSignalArrowSerialize
        {
            get { return SerializableColor.ToString(_color_long_signal_background); }
            set { _color_long_signal_background = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Select Color for the arrow in short setup.")]
        [Category("Background")]
        [DisplayName("Color Arrow Short")]
        public Color ColorShortSignalArrow
        {
            get { return _color_short_signal_arrow; }
            set { _color_short_signal_arrow = value; }
        }
        // Serialize Color object
        [Browsable(false)]
        public string ColorShortSignalArrowSerialize
        {
            get { return SerializableColor.ToString(_color_short_signal_arrow); }
            set { _color_short_signal_arrow = SerializableColor.FromString(value); }
        }



        /// <summary>
        /// </summary>
        [Description("DashStyle for last percent line.")]
        [Category("Plots")]
        [DisplayName("Percent Dash Style Last")]
        public DashStyle DashStyleLineLast
        {
            get { return _plotdashstylelinelast; }
            set { _plotdashstylelinelast = value; }
        }

        /// <summary>
        /// </summary>
        [Description("Width for last percent line.")]
        [Category("Plots")]
        [DisplayName("Percent Line Width")]
        public int PlotWidthLineLast
        {
            get { return _plotwidthlinelast; }
            set { _plotwidthlinelast = Math.Max(1, value); }
        }

        /// <summary>
        /// </summary>
        [Description("DashStyle for percent lines.")]
        [Category("Plots")]
        [DisplayName("Percent Dash Style")]
        public DashStyle DashStyleLine
        {
            get { return _plotdashstyleline; }
            set { _plotdashstyleline = value; }
        }
        
        /// <summary>
        /// </summary>
        [Description("Width for percent lines.")]
        [Category("Plots")]
        [DisplayName("Percent Line Width")]
        public int PlotWidthLine
        {
            get { return _plotwidthline; }
            set { _plotwidthline = Math.Max(1, value); }
        }
        #endregion
    }
}
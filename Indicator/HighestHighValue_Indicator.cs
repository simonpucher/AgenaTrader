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
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    public enum IndicatorEnum_HighestHighValue
    {
        SMA = 1,
        EMA = 2
    }

	[Description("Compare the current value of an indicator to latest high value of the indicator in a defined period of time.")]
	public class HighestHighValue_Indicator : UserIndicator
	{
        //input
        private Color _plot1color = Const.DefaultIndicatorColor;
        private int _plot1width = Const.DefaultLineWidth;
        private DashStyle _plot1dashstyle = Const.DefaultIndicatorDashStyle;
        private int _indicatorEMAPeriod = 200;
        private int _indicatorSMAPeriod = 200;
        private int _comparisonPeriod = 30;
        private IndicatorEnum_HighestHighValue _indicatorenum = IndicatorEnum_HighestHighValue.SMA;

        //output


        //internal
        private DataSeries _DATA_List;

        /// <summary>
        /// Initalizie the OutputDescriptor.
        /// </summary>
		protected override void OnInit()
		{
            Add(new OutputDescriptor(new Pen(this.Plot1Color, this.Plot0Width), OutputSerieDrawStyle.Line, "HighestHighValue_Indicator"));

            CalculateOnClosedBar = true;
            IsOverlay = false;
		}

        /// <summary>
        /// Init all variables on startup.
        /// </summary>
        protected override void OnStart()
        {
            this._DATA_List = new DataSeries(this);
        }

        /// <summary>
        /// Recalculate all data on each each bar update. 
        /// </summary>
		protected override void OnCalculate()
		{
            double currentvalue = 0.0;
            switch (IndicatorEnum)
            {
                case IndicatorEnum_HighestHighValue.SMA:
                    currentvalue = SMA(IndicatorSMAPeriod)[0];
                    break;
                case IndicatorEnum_HighestHighValue.EMA:
                    currentvalue = EMA(IndicatorEMAPeriod)[0];
                    break;
                default:
                    break;
            }
            
            double lasthighvalue = _DATA_List.Reverse().Take(this.ComparisonPeriod).Max();

            if (lasthighvalue < currentvalue)
            {
                MyPlot1.Set(1);
            }
            else
            {
                MyPlot1.Set(0);
            }

            this._DATA_List.Set(currentvalue);

            //set the color
            PlotColors[0][0] = this.Plot1Color;
            OutputDescriptors[0].PenStyle = this.Dash0Style;
            OutputDescriptors[0].Pen.Width = this.Plot0Width;

		}


        public override string ToString()
        {
            return "HHV";
        }

        public override string DisplayName
        {
            get
            {
                return "HHV";
            }
        }


		#region Properties

        #region InSeries

        [Description("Type of the indicator")]
        [InputParameter]
        [DisplayName("Type of the indicator")]
        public IndicatorEnum_HighestHighValue IndicatorEnum
        {
            get { return _indicatorenum; }
            set { _indicatorenum = value; }
        }


            [Description("Period for the SMA")]
            [InputParameter]
            [DisplayName("Period SMA")]
            public int IndicatorSMAPeriod
            {
                get { return _indicatorSMAPeriod; }
                set { _indicatorSMAPeriod = value; }
            }

            [Description("Period for the EMA")]
            [InputParameter]
            [DisplayName("Period EMA")]
            public int IndicatorEMAPeriod
            {
                get { return _indicatorEMAPeriod; }
                set { _indicatorEMAPeriod = value; }
            }



            [Description("Period for comparison")]
            [InputParameter]
            [DisplayName("Period for comparison")]
            public int ComparisonPeriod
            {
                get { return _comparisonPeriod; }
                set { _comparisonPeriod = value; }
            }

            #region Plotstyle

                [XmlIgnore()]
                [Description("Select Color")]
                [InputParameter]
                [DisplayName("Pricline")]
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
                [InputParameter]
                [DisplayName("Line Width Indicator")]
                public int Plot0Width
                {
                    get { return _plot1width; }
                    set { _plot1width = Math.Max(1, value); }
                }


                /// <summary>
                /// </summary>
                [Description("DashStyle for Indicator.")]
                [InputParameter]
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
            public DataSeries MyPlot1
            {
                get { return Outputs[0]; }
            }

        #endregion

        #endregion
    }
}
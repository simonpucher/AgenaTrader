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
/// Version: 1.2.
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// Description: https://en.wikipedia.org/wiki/Algorithmic_trading#Mean_reversion 
/// -------------------------------------------------------------------------
/// todo
/// Statistic
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this indicator without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Mean Reversion")]
	[IsEntryAttribute(true)]
	[IsStopAttribute(false)]
	[IsTargetAttribute(false)]
	[OverrulePreviousStopPrice(false)]
    public class Mean_Reversion_Condition : UserScriptedCondition, IMean_Reversion
    {
		#region Variables

        //interface 
        private bool _IsShortEnabled = false;
        private bool _IsLongEnabled = true;
        private bool _WarningOccured = false;
        private bool _ErrorOccured = false;
        private int _Bollinger_Period = 20;
        private double _Bollinger_Standard_Deviation = 2;
        private int _Momentum_Period = 100;
        private int _RSI_Period = 14;
        private int _RSI_Smooth = 3;
        private int _RSI_Level_Low = 30;
        private int _RSI_Level_High = 70;
        private int _Momentum_Level_Low = -1;
        private int _Momentum_Level_High = 1;

        //input

        //output
		
        //internal
        private Mean_Reversion_Indicator _Mean_Reversion_Indicator = null;


		#endregion

		protected override void OnInit()
		{
			IsEntry = true;
			IsStop = false;
			IsTarget = false;
			Add(new OutputDescriptor(Const.DefaultIndicatorColor, "Occurred"));
            Add(new OutputDescriptor(Const.DefaultIndicatorColor, "Entry"));

			IsOverlay = false;
			CalculateOnClosedBar = true;

            //We need at least xy bars
            this.RequiredBarsCount = 130;
		}

        protected override void OnBarsRequirements()
        {
            base.OnBarsRequirements();

        }


        protected override void OnStart()
        {
            base.OnStart();

            //Init our indicator to get code access
            this._Mean_Reversion_Indicator = new Mean_Reversion_Indicator();

            this.ErrorOccured = false;
            this.WarningOccured = false;
        }

       

		protected override void OnCalculate()
		{

            //calculate data
            ResultValue returnvalue = this._Mean_Reversion_Indicator.calculate(InSeries, Open, High, null, null, this.Bollinger_Period, this.Bollinger_Standard_Deviation, this.Momentum_Period, this.RSI_Period, this.RSI_Smooth, this.RSI_Level_Low, this.RSI_Level_High, this.Momentum_Level_Low, this.Momentum_Level_High);

            //If the calculate method was not finished we need to stop and show an alert message to the user.
            if (returnvalue.ErrorOccured)
            {
                //Display error just one time
                if (!this.ErrorOccured)
                {
                    Log(this.DisplayName + ": " + Const.DefaultStringErrorDuringCalculation, InfoLogLevel.AlertLog);
                    this.ErrorOccured = true;
                }
                return;
            }

            //Entry
            if (returnvalue.Entry.HasValue)
            {
                switch (returnvalue.Entry)
                {
                    case OrderDirection.Buy:
                        Occurred.Set(1);
                        break;
                    case OrderDirection.Sell:
                        Occurred.Set(-1);
                        break;
                }
            }

            ////Exit
            //if (returnvalue.Exit.HasValue)
            //{
            //    switch (returnvalue.Exit)
            //    {
            //        case OrderDirection.Buy:
            //            this.DoExitShort();
            //            break;
            //        case OrderDirection.Sell:
            //            this.DoExitLong();
            //            break;
            //    }
            //}


		}


        public override string ToString()
        {
            return "Mean Reversion(C)";
        }

        public override string DisplayName
        {
            get
            {
                return "Mean Reversion (C)";
            }
        }

     


        #region Properties



        #region Interface


        /// <summary>
        /// </summary>
        [Description("If true it is allowed to create long positions.")]
        [InputParameter]
        [DisplayName("Allow Long")]
        public bool IsLongEnabled
        {
            get { return _IsLongEnabled; }
            set { _IsLongEnabled = value; }
        }


        /// <summary>
        /// </summary>
        [Description("If true it is allowed to create short positions.")]
        [InputParameter]
        [DisplayName("Allow Short")]
        public bool IsShortEnabled
        {
            get { return _IsShortEnabled; }
            set { _IsShortEnabled = value; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public bool ErrorOccured
        {
            get { return _ErrorOccured; }
            set { _ErrorOccured = value; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public bool WarningOccured
        {
            get { return _WarningOccured; }
            set { _WarningOccured = value; }
        }




        [Description("Period of the Bollinger Band.")]
        [InputParameter]
        [DisplayName("BB Period")]
        public int Bollinger_Period
        {
            get { return _Bollinger_Period; }
            set { _Bollinger_Period = value; }
        }

        [Description("Standard Deviation of the Bollinger Band.")]
        [InputParameter]
        [DisplayName("BB StdDev")]
        public double Bollinger_Standard_Deviation
        {
            get { return _Bollinger_Standard_Deviation; }
            set { _Bollinger_Standard_Deviation = value; }
        }

        [Description("Period of the Momentum.")]
        [InputParameter]
        [DisplayName("MOM Period")]
        public int Momentum_Period
        {
            get { return _Momentum_Period; }
            set { _Momentum_Period = value; }
        }

        [Description("Period of the RSI.")]
        [InputParameter]
        [DisplayName("RSI Period")]
        public int RSI_Period
        {
            get { return _RSI_Period; }
            set { _RSI_Period = value; }
        }

        [Description("Smooth Period of the RSI.")]
        [InputParameter]
        [DisplayName("RSI Smooth Period")]
        public int RSI_Smooth
        {
            get { return _RSI_Smooth; }
            set { _RSI_Smooth = value; }
        }

        [Description("We trade long below this RSI level.")]
        [InputParameter]
        [DisplayName("RSI Level Low")]
        public int RSI_Level_Low
        {
            get { return _RSI_Level_Low; }
            set { _RSI_Level_Low = value; }
        }

        [Description("We trade short above this RSI level.")]
        [InputParameter]
        [DisplayName("RSI Level High")]
        public int RSI_Level_High
        {
            get { return _RSI_Level_High; }
            set { _RSI_Level_High = value; }
        }

        [Description("We trade long if momentum is above this level.")]
        [InputParameter]
        [DisplayName("MOM Level Low")]
        public int Momentum_Level_Low
        {
            get { return _Momentum_Level_Low; }
            set { _Momentum_Level_Low = value; }
        }

        [Description("We trade short if momentum is below this level.")]
        [InputParameter]
        [DisplayName("MOM Level High")]
        public int Momentum_Level_High
        {
            get { return _Momentum_Level_High; }
            set { _Momentum_Level_High = value; }
        }
        #endregion


        #region InSeries



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

        #region Internals



        #endregion


        #endregion
    }
}
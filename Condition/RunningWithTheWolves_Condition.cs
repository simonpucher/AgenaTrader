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
/// Version: in progress
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// 
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this indicator without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("RunningWithTheWolves")]
	[IsEntryAttribute(true)]
	[IsStopAttribute(false)]
	[IsTargetAttribute(false)]
	[OverrulePreviousStopPrice(false)]
    public class RunningWithTheWolves_Condition : UserScriptedCondition
	{
		#region Variables

        //input

        //output
		
        //internal
        private RunningWithTheWolves_Indicator _RunningWithTheWolves_Indicator = null;


		#endregion

		protected override void Initialize()
		{
			IsEntry = true;
			IsStop = false;
			IsTarget = false;
			Add(new Plot(Const.DefaultIndicatorColor, "Occurred"));
            Add(new Plot(Const.DefaultIndicatorColor, "Entry"));

			Overlay = false;
			CalculateOnBarClose = true;

            //For SMA200 we need at least 200 Bars.
            this.BarsRequired = 200;
		}

        protected override void InitRequirements()
        {
            base.InitRequirements();

        }


        protected override void OnStartUp()
        {
            base.OnStartUp();

            //Init our indicator to get code access
            this._RunningWithTheWolves_Indicator = new RunningWithTheWolves_Indicator();
        }

       

		protected override void OnBarUpdate()
		{

            //calculate data
            OrderAction? resultdata = this._RunningWithTheWolves_Indicator.calculate(Input);
            if (resultdata.HasValue)
            {
                switch (resultdata)
                {
                    case OrderAction.Buy:
                        Occurred.Set(1);
                        //Entry.Set(Input[0]);
                        break;
                    case OrderAction.SellShort:
                        Occurred.Set(-1);
                        //Entry.Set(Input[0]);
                        break;
                    //case OrderAction.BuyToCover:
                    //    break;
                    //case OrderAction.Sell:
                    //    break;
                    default:
                        //nothing to do
                        Occurred.Set(0);
                        //Entry.Set(Input[0]);
                        break;
                }
            }
            else
            {
                Occurred.Set(0);
            }

		}


        public override string ToString()
        {
            return "Running with the wolves (C)";
        }

        public override string DisplayName
        {
            get
            {
                return "Running with the wolves (C)";
            }
        }

     


        #region Properties


        #region Input



            #endregion


            #region Output

                [Browsable(false)]
                [XmlIgnore()]
                public DataSeries Occurred
                {
                    get { return Values[0]; }
                }

                [Browsable(false)]
                [XmlIgnore()]
                public DataSeries Entry
                {
                    get { return Values[1]; }
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

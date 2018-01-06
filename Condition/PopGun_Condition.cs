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
/// Christian Kovar 2016
/// Simon Pucher 2016
/// -------------------------------------------------------------------------
/// todo description
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use global source code elements.
/// You will find this script on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("Condition für Pop Gun Bar Pattern")]
	[IsEntryAttribute(true)]
	[IsStopAttribute(false)]
	[IsTargetAttribute(false)]
	[OverrulePreviousStopPrice(false)]
	public class PopGun_Condition : UserScriptedCondition
	{
		#region Variables
        //input
        private int _PopGunExpires = 5;
        private bool _issnapshotactive = false;
        private bool _isevaluationactive = false;
        private bool _filter_NoShortRSI = false;
        private bool _filter_NoLongRSI = false;
        private bool _filter_NoTriggerEOD = false;
        private PopGunType _PopGunType = PopGunType.ThreeBarReversal;

        //internal
        private PopGun_Indicator _popgun_indicator = null;

		#endregion

        

		protected override void OnInit()
		{
			IsEntry = true;
			IsStop = false;
			IsTarget = false;
			Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Black), "Occurred"));
			Add(new OutputDescriptor(Color.FromArgb(255, 92, 242, 57), "Entry"));
			IsOverlay = false;
			CalculateOnClosedBar = true;
            RequiredBarsCount = 3;
		}

        protected override void OnBarsRequirements()
        {
            base.OnBarsRequirements();

        }

        protected override void OnStart()
        {
            base.OnStart();

            //Init our indicator to get code access
            this._popgun_indicator = new PopGun_Indicator();
            this._popgun_indicator.SetData(this.PopGunExpires, this.IsSnapshotActive, this.IsEvaluationActive, this.Filter_NoTriggerEOD);
        }

		protected override void OnCalculate()
		{
            if (this.Bars != null && this.Bars.Count >  0 && ProcessingBarIndex > 3)
            {
                //ShowGap Indikator aufrufen. Dieser liefert 100 für Long Einstieg und -100 für Short Einstieg. Liefert 0 für kein Einstiegssignal
                double PopGun_Indicator_Value = this._popgun_indicator.calculate(this.Bars, this.ProcessingBarIndex, this.PopGunType);
                if (PopGun_Indicator_Value == 100)
                {
                    Occurred.Set(1);
                    Entry.Set(GetCurrentBid());
                }
                else if (PopGun_Indicator_Value == -100)
                {
                    Occurred.Set(-1);
                    Entry.Set(GetCurrentBid());
                }

                else
                {
                    Occurred.Set(0);
                    Entry.Set(GetCurrentBid());
                }
            }
		}

		#region Properties


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
			return new[]{Entry};
		}

        [Description("Type of PopGun Pattern you would like to use.")]
        [Category("Parameters")]
        [DisplayName("Pop Gun Type")]
        public PopGunType PopGunType
        {
            get { return _PopGunType; }
            set { _PopGunType = value; }
        }


        [Description("Wieviel Bars ist PopGunTrigger gültig?")]
        [Category("Parameters")]
        [DisplayName("PopGunExpires")]
        public int PopGunExpires
        {
            get { return _PopGunExpires; }
            set { _PopGunExpires = value; }
        }

        [Description("Creates snapshots on signals")]
        [Category("Parameters")]
        [DisplayName("Snapshot is active")]
        public bool IsSnapshotActive
        {
            get { return _issnapshotactive; }
            set { _issnapshotactive = value; }
        }

        [Description("Creates evalation (P/L) on signals")]
        [Category("Parameters")]
        [DisplayName("Evalation is active")]
        public bool IsEvaluationActive
        {
            get { return _isevaluationactive; }
            set { _isevaluationactive = value; }
        }

        [Description("No Long Trades when RSI > 70")]
        [Category("TradeFilter")]
        [DisplayName("No Long Trades when RSI > 70")]
        public bool Filter_NoLongRSI
        {
            get { return _filter_NoLongRSI; }
            set { _filter_NoLongRSI = value; }
        }

        [Description("No Short Trades when RSI < 30")]
        [Category("TradeFilter")]
        [DisplayName("No Short Trades when RSI < 30")]
        public bool Filter_NoShortRSI
        {
            get { return _filter_NoShortRSI; }
            set { _filter_NoShortRSI = value; }
        }

        [Description("No PopGun is triggered, if the expire date is targeted for the following day")]
        [Category("TradeFilter")]
        [DisplayName("No Trigger before EOD")]
        public bool Filter_NoTriggerEOD
        {
            get { return _filter_NoTriggerEOD; }
            set { _filter_NoTriggerEOD = value; }
        }

		#endregion
	}
}
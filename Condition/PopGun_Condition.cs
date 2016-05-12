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

        //internal
        private PopGun_Indicator _popgun_indicator = null;

		#endregion

        

		protected override void Initialize()
		{
			IsEntry = true;
			IsStop = false;
			IsTarget = false;
			Add(new Plot(Color.FromKnownColor(KnownColor.Black), "Occurred"));
			Add(new Plot(Color.FromArgb(255, 92, 242, 57), "Entry"));
			Overlay = true;
			CalculateOnBarClose = true;
		}

        protected override void InitRequirements()
        {
            base.InitRequirements();

        }

        protected override void OnStartUp()
        {
            base.OnStartUp();

            //Init our indicator to get code access
            this._popgun_indicator = new PopGun_Indicator();
            this._popgun_indicator.SetData(this.PopGunExpires, this.IsSnapshotActive, this.IsEvaluationActive);
        }

		protected override void OnBarUpdate()
		{
            if (this.Bars != null && this.Bars.Count > 0)
            {
                //ShowGap Indikator aufrufen. Dieser liefert 100 für Long Einstieg und -100 für Short Einstieg. Liefert 0 für kein Einstiegssignal
                double PopGun_Indicator_Value = this._popgun_indicator.calculate(this.Bars, this.CurrentBar);
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
			return new[]{Entry};
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
		#endregion
	}
}

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

        //private int _popGun = 1;
        int _PopGunExpires = 5;
        //double PopGun_Indicator_Value;

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
            this._popgun_indicator.SetData(this.PopGunExpires);

        }

		protected override void OnBarUpdate()
		{
            if (this.Bars != null && this.Bars.Count > 0)
            {
                //ShowGap Indikator aufrufen. Dieser liefert 100 für Long Einstieg und -100 für Short Einstieg. Liefert 0 für kein Einstiegssignal
               // PopGun_Indicator_Value = PopGun_Indicator(_PopGunExpires)[0];
                double PopGun_Indicator_Value = this._popgun_indicator.calculate(this.Bars, this.CurrentBar);

               //// Print(Time[0].ToString() + ": " + PopGun_Indicator_Value);

                if (PopGun_Indicator_Value == 100)
                {
                    Occurred.Set(1);
                    Entry.Set(this.Bars[0].Close);
                }
                else
                {
                    Occurred.Set(0);
                    Entry.Set(this.Bars[0].Close);
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

        //[Description("")]
        //[Category("Parameters")]
        //public int PopGun
        //{
        //    get { return _popGun; }
        //    set { _popGun = Math.Max(1, value); }
        //}
        [Description("Wieviel Bars ist PopGunTrigger gültig?")]
        [Category("Parameters")]
        [DisplayName("PopGunExpires")]
        public int PopGunExpires
        {
            get { return _PopGunExpires; }
            set { _PopGunExpires = value; }
        }

		#endregion
	}
}

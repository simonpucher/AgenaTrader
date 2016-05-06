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
    [Description("Steigt bei jeder geraden Minute mit der jeweils ausgewählten Strategie in den Markt ein. Dient nur zum Strategie-Test >>> DUMMY!!!")]
    [IsEntryAttribute(true)]
    [IsStopAttribute(false)]
    [IsTargetAttribute(false)]
    [OverrulePreviousStopPrice(false)]
    public class DummyOneMinuteEntry_Condition : UserScriptedCondition
    {

        IOrder oEnterLong;
        IOrder oExitLong;

        private int _myvalue = 1;

        protected override void Initialize()
        {
            IsEntry = true;
            IsStop = false;
            IsTarget = false;
            Add(new Plot(Color.FromKnownColor(KnownColor.Black), "Occurred"));
            Add(new Plot(Color.FromArgb(255, 153, 170, 61), "Entry"));
            Overlay = true;
            CalculateOnBarClose = false;
            ClearOutputWindow();


        }

        protected override void OnBarUpdate()
        {
            if (Bars != null && Bars.Count > 0
                && TimeFrame.Periodicity == DatafeedHistoryPeriodicity.Minute
                && TimeFrame.PeriodicityValue == 1)
            { }
            else
            {
                return;
            }

            if (DummyOneMinuteEven_Indicator()[0] == 100)
            {
                Occurred.Set(-1);
            }
            else
            {
                Occurred.Set(1);
            }

            //Entry.Set(iv_Bars.GetOpen(iv_CurrentBar));
            //Entry.Set(10390);
            //Entry.Set(GetCurrentBid());

        }

        #region Properties

        [Description("Period for RSI")]
        [Category("Parameters")]
        [DisplayName("Period for RSI")]
        public int Myvalue
        {
            get { return _myvalue; }
            set { _myvalue = value; }
        }

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
    }
}

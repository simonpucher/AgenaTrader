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
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Geben Sie bitte hier die Beschreibung für die neue Condition ein")]
    [IsEntryAttribute(true)]
    [IsStopAttribute(false)]
    [IsTargetAttribute(false)]
    [OverrulePreviousStopPrice(false)]
    public class ShowGap_Condition : UserScriptedCondition
    {
        #region Variables
        decimal _PunkteGapMin = 50;
        decimal _PunkteGapMax = 100;
        double ShowGap_Indicator_Value;
        #endregion

        protected override void Initialize()
        {
            IsEntry = true;
            IsStop = false;
            IsTarget = false;
            Add(new Plot(Color.Azure, "Occurred"));
            Add(new Plot(Color.LightCyan, "Entry"));
            Overlay = true;
            CalculateOnBarClose = true;
        }

        protected override void OnBarUpdate()
        {

//ShowGap Indikator aufrufen. Dieser liefert 100 für Long Einstieg und -100 für Short Einstieg. Liefert 0 für kein Einstiegssignal
            ShowGap_Indicator_Value = ShowGap_Indicator(PunkteGapMin, PunkteGapMax)[0];

            if ( ShowGap_Indicator_Value == 100 ) {
                Occurred.Set(1);
            }
            else if (ShowGap_Indicator_Value == -100)
            {
                Occurred.Set(-1);
            }
            else
            {
                Occurred.Set(0);
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
            return new[] { Entry };
        }

        [Description("Mind. Punkte für Gap")]
        [Category("Parameters")]
        [DisplayName("MinPunkte")]
        public decimal PunkteGapMin
        {
            get { return _PunkteGapMin; }
            set { _PunkteGapMin = value; }
        }

        [Description("Max. Punkte für Gap")]
        [Category("Parameters")]
        [DisplayName("MaxPunkte")]
        public decimal PunkteGapMax
        {
            get { return _PunkteGapMax; }
            set { _PunkteGapMax = value; }
        }
        #endregion
    }
}

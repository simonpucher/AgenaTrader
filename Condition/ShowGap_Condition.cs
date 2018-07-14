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

        protected override void OnInit()
        {
            IsEntry = true;
            IsStop = false;
            IsTarget = false;
            Add(new OutputDescriptor(Color.Azure, "Occurred"));
            Add(new OutputDescriptor(Color.LightCyan, "Entry"));
            IsOverlay = true;
            CalculateOnClosedBar = true;
        }

        protected override void OnCalculate()
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

        [Description("Mind. Punkte für Gap")]
        [InputParameter]
        [DisplayName("MinPunkte")]
        public decimal PunkteGapMin
        {
            get { return _PunkteGapMin; }
            set { _PunkteGapMin = value; }
        }

        [Description("Max. Punkte für Gap")]
        [InputParameter]
        [DisplayName("MaxPunkte")]
        public decimal PunkteGapMax
        {
            get { return _PunkteGapMax; }
            set { _PunkteGapMax = value; }
        }
        #endregion
    }
}
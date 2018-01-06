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
/// Gets latest VDAX_NEW value from OnVista via httpRequest
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Gets latest VDAX_NEW value from OnVista via httpRequest")]
    public class VDAX_NEW_OnVista_Indicator : UserIndicator
    {
        //*** input ***
        private TextPosition _TextPosition = TextPosition.BottomRight;
        private int _TextSize = 10;
        private int _CheckEveryXSeconds = 60;

        //*** internal ***



        protected override void OnInit()
        {
            IsOverlay = true;
        }


        protected override void OnStart()
        {
            decimal vdax_new = GlobalUtilities.GetCurrentVdaxNew(this.CheckEveryXSeconds);
            AddChartTextFixed("VDAX_NEW", "VDAX-NEW: " + vdax_new, this.TextPosition, Color.Black, new Font("Arial", this.TextSize), Color.Transparent, Color.Transparent);
        }


        protected override void OnCalculate()
        {

        }

        public override string ToString()
        {
            return "VDAX-NEW (OnVista)";
        }

        public override string DisplayName
        {
            get
            {
                return "VDAX-NEW (OnVista)";
            }
        }

        #region Properties

        #region InSeries


        


        /// <summary>
        /// </summary>
        [Description("Check online service each x seconds.")]
        [Category("Drawings")]
        [DisplayName("Seconds Online Check")]
        public int CheckEveryXSeconds
        {
            get { return _CheckEveryXSeconds; }
            set { _CheckEveryXSeconds = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Text Position")]
        [Category("Drawings")]
        [DisplayName("Text Position")]
        public TextPosition TextPosition
        {
            get { return _TextPosition; }
            set { _TextPosition = value; }
        }

        /// <summary>
        /// </summary>
        [Description("Text Size")]
        [Category("Drawings")]
        [DisplayName("Text Size")]
        public int TextSize
        {
            get { return _TextSize; }
            set { _TextSize = value; }
        }

        #endregion

        #endregion
    }
}
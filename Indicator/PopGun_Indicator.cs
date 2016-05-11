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
    [Description("PopGun Bar Pattern")]
    public class PopGun_Indicator : UserIndicator
    {
        //Definition Outside Bar (not to be confused with the Outside Bar Markttechnik Defition based on Michael Voigt)
        // ->An outside bar occurs when the range of a bar encompasses the previous bar
        //http://www.debeurs.nl/Forum/Upload/2015/8690735.pdf

        //Definition Inside Bar (not to be confused with the Inside Bar Markttechnik Defition based on Michael Voigt)
        // ->An inside bar is a price bar whose range is encompessed by the previous bar
        //http://www.debeurs.nl/Forum/Upload/2015/8690735.pdf

        // 1) Determine high and low of price which was 2 bars ago
        // 2) Determine, if 1 Bar ago, there was an inside bar
        // 3) Determine, if current Bar is an outside bar

        //input
        private int _PopGunExpires = 5;
        private bool _issnapshotactive = false;

        //output
        private double _popGunTriggerLong;
        private double _popGunTriggerShort;

        //internal
        bool IsPopGun;
        int PopGunTarget;
        int PopGunTriggerBar;

        /// <summary>
        /// If we use this indicator from another script we need to initalize all important data first.
        /// </summary>
        public void SetData(int popgunexpires)
        {
            this.PopGunExpires = popgunexpires;
        }


        protected override void Initialize()
        {
            Add(new Plot(Color.Orange, "PopGun"));
            Add(new Plot(Color.Green, PlotStyle.Block,"PopGunTrigger"));
            Overlay = false; //underneath the price chart in his own subchart
            DrawOnPricePanel = true;
            CalculateOnBarClose = true;
        }

        protected override void OnBarUpdate()
        {
            int returnvalue = calculate(Bars, CurrentBar);
            Value.Set(returnvalue);
            if (CurrentBar <= this.PopGunTarget)
            {
                Values[1].Set(0); //Indicates that there is a PopGun Trigger!         
            }
            drawTarget();
        }

        //todo -100 wird noch nicht zurückgegeben oder?
        public int calculate(IBars bars, int curbar)
        {
            //Values[1].Set(0);
            //We need at least three bars
            if (curbar < 2) return 0;

            double TwoBarsAgo_High = bars[2].High;
            double TwoBarsAgo_Low = bars[2].Low;

            double OneBarAgo_High = bars[1].High;
            double OneBarAgo_Low = bars[1].Low;

            double CurrentBar_High = bars[0].High;
            double CurrentBar_Low = bars[0].Low;

            // 2) Determine, if 1 Bar ago, there was an inside bar
            if (TwoBarsAgo_High > OneBarAgo_High
            && TwoBarsAgo_Low < OneBarAgo_Low)
            {
                //One Bar ago was an inside bar, so lets check if current bar is outside bar
                if (TwoBarsAgo_High < CurrentBar_High
                && TwoBarsAgo_Low > CurrentBar_Low)
                {
                    // current bar is outside bar -> lets pop the gun
                    this.PopGunTarget = curbar + this.PopGunExpires;
                    PopGunTriggerBar = CurrentBar;
                    this.PopGunTriggerLong = CurrentBar_High;
                    this.PopGunTriggerShort = CurrentBar_Low;
                }
            }

            if (curbar < this.PopGunTarget)                
            {
                if (bars[0].Close > this.PopGunTriggerLong)
                {
                    return 100; 
                }
                else if (bars[0].Close < this.PopGunTriggerShort)
                {
                    return -100; 
                }
            }
            return 0;
        }

        public void drawTarget()
        {

            if (CurrentBar <= PopGunTarget
             && CurrentBar > PopGunTriggerBar
             && CurrentBar > 0)
            {
                string strPopGunLong = "PopGunLong" + CurrentBar;
                string strPopGunShort = "PopGunShort" + CurrentBar;

                DateTime lineend = GlobalUtilities.GetTargetBar(Bars, Bars.GetByIndex(PopGunTriggerBar).Time, TimeFrame, PopGunExpires);

                DrawLine(strPopGunLong, true, Bars.GetByIndex(PopGunTriggerBar).Time, PopGunTriggerLong, lineend, PopGunTriggerLong, 
                                                        Color.Green, Const.DefaultIndicatorDashStyle, Const.DefaultLineWidth_large);

                DrawLine(strPopGunShort, true, Bars.GetByIndex(PopGunTriggerBar).Time, PopGunTriggerShort, lineend, PopGunTriggerShort, 
                                                        Color.Red, Const.DefaultIndicatorDashStyle, Const.DefaultLineWidth_large);

                if (this.IsSnapshotActive)
                {
                    GlobalUtilities.SaveSnapShot("PopGun", Instrument.Name, this.Root.Core.ChartManager.AllCharts, Bars, TimeFrame); 
                }
            }
        }

        #region Properties
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

        
        #endregion

        #region Output
        [Browsable(false)]
        [XmlIgnore()]
        public double PopGunTriggerLong
        {
            get { return _popGunTriggerLong; }
            set { _popGunTriggerLong = value; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public double PopGunTriggerShort
        {
            get { return _popGunTriggerShort; }
            set { _popGunTriggerShort = value; }
        }
        #endregion
    }
}
#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator : Indicator
	{
		/// <summary>
		/// PopGun Bar Pattern
		/// </summary>
		public PopGun_Indicator PopGun_Indicator(System.Int32 popGunExpires, System.Boolean isSnapshotActive)
        {
			return PopGun_Indicator(Input, popGunExpires, isSnapshotActive);
		}

		/// <summary>
		/// PopGun Bar Pattern
		/// </summary>
		public PopGun_Indicator PopGun_Indicator(IDataSeries input, System.Int32 popGunExpires, System.Boolean isSnapshotActive)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<PopGun_Indicator>(input, i => i.PopGunExpires == popGunExpires && i.IsSnapshotActive == isSnapshotActive);

			if (indicator != null)
				return indicator;

			indicator = new PopGun_Indicator
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							PopGunExpires = popGunExpires,
							IsSnapshotActive = isSnapshotActive
						};
			indicator.SetUp();

			CachedCalculationUnits.AddIndicator2Cache(indicator);

			return indicator;
		}
	}

	#endregion

	#region Strategy

	public partial class UserStrategy
	{
		/// <summary>
		/// PopGun Bar Pattern
		/// </summary>
		public PopGun_Indicator PopGun_Indicator(System.Int32 popGunExpires, System.Boolean isSnapshotActive)
		{
			return LeadIndicator.PopGun_Indicator(Input, popGunExpires, isSnapshotActive);
		}

		/// <summary>
		/// PopGun Bar Pattern
		/// </summary>
		public PopGun_Indicator PopGun_Indicator(IDataSeries input, System.Int32 popGunExpires, System.Boolean isSnapshotActive)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.PopGun_Indicator(input, popGunExpires, isSnapshotActive);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// PopGun Bar Pattern
		/// </summary>
		public PopGun_Indicator PopGun_Indicator(System.Int32 popGunExpires, System.Boolean isSnapshotActive)
		{
			return LeadIndicator.PopGun_Indicator(Input, popGunExpires, isSnapshotActive);
		}

		/// <summary>
		/// PopGun Bar Pattern
		/// </summary>
		public PopGun_Indicator PopGun_Indicator(IDataSeries input, System.Int32 popGunExpires, System.Boolean isSnapshotActive)
		{
			return LeadIndicator.PopGun_Indicator(input, popGunExpires, isSnapshotActive);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// PopGun Bar Pattern
		/// </summary>
		public PopGun_Indicator PopGun_Indicator(System.Int32 popGunExpires, System.Boolean isSnapshotActive)
		{
			return LeadIndicator.PopGun_Indicator(Input, popGunExpires, isSnapshotActive);
		}

		/// <summary>
		/// PopGun Bar Pattern
		/// </summary>
		public PopGun_Indicator PopGun_Indicator(IDataSeries input, System.Int32 popGunExpires, System.Boolean isSnapshotActive)
		{
			return LeadIndicator.PopGun_Indicator(input, popGunExpires, isSnapshotActive);
		}
	}

	#endregion

}

#endregion

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

        //internal
        bool IsPopGun;
        int PopGunTarget;
       


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
            Overlay = true;
            CalculateOnBarClose = true;
        }

        protected override void OnBarUpdate()
        {
            int returnvalue = calculate(Bars, CurrentBar);
            Value.Set(returnvalue);
           drawTarget();
        }

        //public void calculate()
        //{
        //    if (CurrentBar < 2) return;

        //    double TwoBarsAgo_High = Bars[2].High;
        //    double TwoBarsAgo_Low = Bars[2].Low;

        //    double OneBarAgo_High = Bars[1].High;
        //    double OneBarAgo_Low = Bars[1].Low;

        //    double CurrentBar_High = Bars[0].High;
        //    double CurrentBar_Low = Bars[0].Low;

        //    // 2) Determine, if 1 Bar ago, there was an inside bar
        //    if (TwoBarsAgo_High > OneBarAgo_High
        //    && TwoBarsAgo_Low < OneBarAgo_Low)
        //    {
        //        //One Bar ago was an inside bar, so lets check if current bar is outside bar
        //        if (TwoBarsAgo_High < CurrentBar_High
        //        && TwoBarsAgo_Low > CurrentBar_Low)
        //        {
        //            // current bar is outside bar -> lets pop the gun
        //            BarColor = Color.Turquoise;
        //            PopGunTarget = CurrentBar + _PopGunExpires;
        //            Value.Set(100);
        //            return;
        //        }
        //    }
        //    Value.Set(0);
        //}

        //todo -100 wird noch nicht zurückgegeben oder?
        public int calculate(IBars bars, int curbar)
        {
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
                    this.BarColor = Color.Turquoise;
                    this.PopGunTarget = curbar + this.PopGunExpires;
                    //Value.Set(100);
                    //return;
                    return 100;
                }
            }
            //Value.Set(0);
            return 0;
        }

        public void drawTarget()
        {

            if (CurrentBar == PopGunTarget
              && CurrentBar > 0)
            {
                string strPopGunLong = "PopGunLong" + CurrentBar;
                string strPopGunShort = "PopGunShort" + CurrentBar;

                DrawLine(strPopGunLong, 5, Bars[_PopGunExpires].High, 0, Bars[_PopGunExpires].High, Color.Green);
                DrawLine(strPopGunShort, 5, Bars[_PopGunExpires].Low, 0, Bars[_PopGunExpires].Low, Color.Red);

                if (this.IsSnapshotActive)
                {
                    GlobalUtilities.SaveSnapShot("PopGun", Instrument.Name, this.Root.Core.ChartManager.AllCharts.FirstOrDefault(), Bars, TimeFrame); 
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

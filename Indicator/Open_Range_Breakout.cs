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
using System.Globalization;

/// Version: 1.0
namespace AgenaTrader.UserCode
{
   
    [Description("ORB Indicator")]
	public class ORB : UserIndicator
	{


        private int _orbminutes = 75;
        private Color _col_orb = Color.Brown;

		protected override void Initialize()
		{
			Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
			Overlay = true;
			CalculateOnBarClose = true;

           // Print("Initialize");

		}




        protected override void InitRequirements()
        {
          //  Print("InitRequirements");
        }

        protected override void OnStartUp()
        {
           // Print("OnStartUp");
        }

    

		protected override void OnBarUpdate()
		{
            // Print("OnBarUpdate");

			MyPlot1.Set(Input[0]);

            if (Bars != null && Bars.Count > 0 && IsCurrentBarLast)
            {

         //DateTime start =  DateTime.ParseExact("24.03.2016 09:00", "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
         //DateTime end = DateTime.ParseExact("24.03.2016 10:15", "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);

                DateTime start = Bars.Where(x=>x.Time.Date == Bars[0].Time.Date).FirstOrDefault().Time;
                DateTime start_date = start.Date;
                DateTime end = start.AddMinutes(ORBMinutes);

                //Selektiere alle gültigen Kurse und finde low und high.
                IEnumerable<IBar> list = Bars.Where(x => x.Time >= start).Where(x => x.Time <= end);
                if (list != null && !list.IsEmpty())
                {
                    double minvalue = list.Where(x => x.Low == list.Min(y => y.Low)).LastOrDefault().Low;
                    double maxvalue = list.Where(x => x.High == list.Max(y => y.High)).LastOrDefault().High;

                    DrawRectangle("MyRect", true, start_date, minvalue, end, maxvalue, _col_orb, _col_orb, 70);
                    DrawHorizontalLine("LowLine", true, minvalue, _col_orb, DashStyle.Solid, 3);
                    DrawHorizontalLine("HighLine", true, maxvalue, _col_orb, DashStyle.Solid, 3);
                    DrawVerticalLine("BeginnSession", start_date, _col_orb, DashStyle.Solid, 3);

                    //Print("min: " + minvalue);
                    //Print("max: " + maxvalue);

                }    
            }
		}

        protected override void OnTermination()
        {
            //Print("OnTermination");

        }

        public override string ToString()
        {
            return "ORB";
        }


        #region Properties

        [Browsable(false)]
		[XmlIgnore()]
		public DataSeries MyPlot1
		{
			get { return Values[0]; }
		}


        /// <summary>
        /// </summary>
        [Description("Period in minutes for ORB")]
        [Category("Parameters")]
        [DisplayName("Minutes ORB")]
        public int ORBMinutes
        {
            get { return _orbminutes; }
            set { _orbminutes = value; }
        }

        /// <summary>
        /// </summary>
        [Description("Select Color")]
        [Category("Colors")]
        [DisplayName("ORB")]
        public Color Color_ORB
        {
            get { return _col_orb; }
            set { _col_orb = value; }
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
		/// ORB Indicator
		/// </summary>
		public ORB ORB(System.Int32 oRBMinutes)
        {
			return ORB(Input, oRBMinutes);
		}

		/// <summary>
		/// ORB Indicator
		/// </summary>
		public ORB ORB(IDataSeries input, System.Int32 oRBMinutes)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<ORB>(input, i => i.ORBMinutes == oRBMinutes);

			if (indicator != null)
				return indicator;

			indicator = new ORB
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							ORBMinutes = oRBMinutes
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
		/// ORB Indicator
		/// </summary>
		public ORB ORB(System.Int32 oRBMinutes)
		{
			return LeadIndicator.ORB(Input, oRBMinutes);
		}

		/// <summary>
		/// ORB Indicator
		/// </summary>
		public ORB ORB(IDataSeries input, System.Int32 oRBMinutes)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.ORB(input, oRBMinutes);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// ORB Indicator
		/// </summary>
		public ORB ORB(System.Int32 oRBMinutes)
		{
			return LeadIndicator.ORB(Input, oRBMinutes);
		}

		/// <summary>
		/// ORB Indicator
		/// </summary>
		public ORB ORB(IDataSeries input, System.Int32 oRBMinutes)
		{
			return LeadIndicator.ORB(input, oRBMinutes);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// ORB Indicator
		/// </summary>
		public ORB ORB(System.Int32 oRBMinutes)
		{
			return LeadIndicator.ORB(Input, oRBMinutes);
		}

		/// <summary>
		/// ORB Indicator
		/// </summary>
		public ORB ORB(IDataSeries input, System.Int32 oRBMinutes)
		{
			return LeadIndicator.ORB(input, oRBMinutes);
		}
	}

	#endregion

}

#endregion


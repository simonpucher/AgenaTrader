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
	[Description("This indicator finds the high and low value in a dedicated timeframe.")]
	public class FindHighLowinTimeFrame : UserIndicator
	{
        //input

        //output

        //internal
        private DateTime _currentdayofupdate = DateTime.MinValue;
        private TimeSpan _tim_start = new TimeSpan(12, 0, 0);
        private TimeSpan _tim_end = new TimeSpan(13, 0, 0);

		protected override void Initialize()
		{
			//Add(new Plot(Color.FromKnownColor(KnownColor.Orange), "MyPlot1"));
			Overlay = true;
			CalculateOnBarClose = true;
		}

		protected override void OnBarUpdate()
		{
			//MyPlot1.Set(Input[0]);

            //new day session is beginning so we need to calculate and redraw the lines
            if (_currentdayofupdate.Date < Time[0].Date)
            {
                //Print("we are going to calculate the following date: " + Time[0].Date.ToString());
                this.calculateanddrawhighlowlines();
            }


            //When finished set the last day variable
            //If we are online during the day session we do not set this variable so we are redrawing and recalculating the current session.
            if (Time[0].Date != DateTime.Now.Date)
            {
                _currentdayofupdate = Time[0].Date;
            }

		}


        /// <summary>
        /// Calculate and draw the high & low lines.
        /// </summary>
        private void calculateanddrawhighlowlines()
        {

            DateTime start = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, this.Time_Start.Hours, this.Time_Start.Minutes, this.Time_Start.Seconds);
            DateTime end = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, this.Time_End.Hours, this.Time_End.Minutes, this.Time_End.Seconds);

            //Select all data and find high & low.
            IEnumerable<IBar> list = Bars.Where(x => x.Time >= start).Where(x => x.Time <= end);

            //Check if data for open range is valid.
            //we need to ignore the first day which is normally invalid.
            bool isvalidtimeframe = false;
            if (list != null && !list.IsEmpty() && list.First().Time == start)
            {
                isvalidtimeframe = true;
            }

            if (isvalidtimeframe)
            {
                double low = list.Where(x => x.Low == list.Min(y => y.Low)).LastOrDefault().Low;
                double high = list.Where(x => x.High == list.Max(y => y.High)).LastOrDefault().High;

                DrawHorizontalLine("LowLine" + start.Ticks, true, low, Color.Brown, DashStyle.Solid, 2);
                DrawHorizontalLine("HighLine" + start.Ticks, true, high, Color.Brown, DashStyle.Solid, 2);

                DrawRectangle("HighLowRect" + start.Ticks, true, start, low, end, high, Color.Brown, Color.Brown, 70);
                
            }
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
        [Description("OpenRange DE Start: Uhrzeit ab wann Range gemessen wird")]
        [Category("TimeSpan")]
        [DisplayName("OpenRange Start DE")]
        public TimeSpan Time_Start
        {
            get { return _tim_start; }
            set { _tim_start = value; }
        }
        [Browsable(false)]
        public long Time_OpenRangeStartDESerialize
        {
            get { return _tim_start.Ticks; }
            set { _tim_start = new TimeSpan(value); }
        }

        /// <summary>
        /// </summary>
        [Description("OpenRange US Start: Uhrzeit ab wann Range gemessen wird")]
        [Category("TimeSpan")]
        [DisplayName("OpenRange Start US")]
        public TimeSpan Time_End
        {
            get { return _tim_end; }
            set { _tim_end = value; }
        }
        [Browsable(false)]
        public long Time_OpenRangeStartUSSerialize
        {
            get { return _tim_end.Ticks; }
            set { _tim_end = new TimeSpan(value); }
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
		/// This indicator finds the high and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowinTimeFrame FindHighLowinTimeFrame()
        {
			return FindHighLowinTimeFrame(Input);
		}

		/// <summary>
		/// This indicator finds the high and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowinTimeFrame FindHighLowinTimeFrame(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<FindHighLowinTimeFrame>(input);

			if (indicator != null)
				return indicator;

			indicator = new FindHighLowinTimeFrame
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input
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
		/// This indicator finds the high and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowinTimeFrame FindHighLowinTimeFrame()
		{
			return LeadIndicator.FindHighLowinTimeFrame(Input);
		}

		/// <summary>
		/// This indicator finds the high and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowinTimeFrame FindHighLowinTimeFrame(IDataSeries input)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.FindHighLowinTimeFrame(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// This indicator finds the high and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowinTimeFrame FindHighLowinTimeFrame()
		{
			return LeadIndicator.FindHighLowinTimeFrame(Input);
		}

		/// <summary>
		/// This indicator finds the high and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowinTimeFrame FindHighLowinTimeFrame(IDataSeries input)
		{
			return LeadIndicator.FindHighLowinTimeFrame(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// This indicator finds the high and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowinTimeFrame FindHighLowinTimeFrame()
		{
			return LeadIndicator.FindHighLowinTimeFrame(Input);
		}

		/// <summary>
		/// This indicator finds the high and low value in a dedicated timeframe.
		/// </summary>
		public FindHighLowinTimeFrame FindHighLowinTimeFrame(IDataSeries input)
		{
			return LeadIndicator.FindHighLowinTimeFrame(input);
		}
	}

	#endregion

}

#endregion

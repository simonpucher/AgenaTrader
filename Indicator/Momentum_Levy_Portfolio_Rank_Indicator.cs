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
/// Version: 1.5
/// -------------------------------------------------------------------------
/// Simon Pucher 2017
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("Enter the description for the new custom indicator here")]
	public class Momentum_Levy_Portfolio_Rank_Indicator : UserIndicator
	{
        public static Dictionary<string, double> rank_s = null;
        public static Dictionary<string, double> rank_v = null;

        protected override void OnInit()
		{
			Add(new OutputDescriptor(Color.FromKnownColor(KnownColor.Orange), "Momentum_Levy_Portfolio_Rank_Indicator_Plot"));
            //Print("Barscount: " + Bars.Count);
        }

        protected override void OnStart()
        {
            //Print("Barscount: " + Bars.Count, InfoLogLevel.Info);
            //foreach (var item in Bars)
            //{

            //}

            //Print("OnStart");

            if (rank_s == null)
            {
                rank_s = new Dictionary<string, double>();
            }

            if (rank_v == null)
            {
                rank_v = new Dictionary<string, double>();
            }

            RSL rsl = RSL(Closes[0], 27);
            rank_s[this.Instrument.Symbol] = rsl[0];

            Volatility_Levy_Stand_Dev_Mean_Average_Indicator vol = Volatility_Levy_Stand_Dev_Mean_Average_Indicator(Closes[0], 27);
            rank_v[this.Instrument.Symbol] = vol[0];

        }

        protected override void OnCalculate()
		{
            //Print("OnCalculate");
            int result_s = 0;
            int result_v = 0;

            //List<KeyValuePair<string, double>> sorted = (from kv in rank orderby kv.Value descending select kv).ToList();

            foreach (KeyValuePair<string, double> r in rank_s.OrderByDescending(key => key.Value))
            {
                //Print("Key: {0}, Value: {1}", author.Key, author.Value);
                result_s = result_s + 1;
                if (this.Instrument.Symbol == r.Key.ToString())
                {
                    break;
                }
            }

            foreach (KeyValuePair<string, double> r in rank_v.OrderByDescending(key => key.Value))
            {
                //Print("Key: {0}, Value: {1}", author.Key, author.Value);
                result_v = result_v + 1;
                if (this.Instrument.Symbol == r.Key.ToString())
                {
                    break;
                }
            }


            //foreach (var key in sorted)
            //{
            //    //Console.WriteLine("{0}: {1}", key, list[key]);
            //    result = result + 1;
            //    if (this.Instrument.Symbol == key.ToString())
            //    {
            //        break;
            //    }
            //}

            MyPlot1.Set((result_s + result_v)/2);
		}

		#region Properties

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries MyPlot1
		{
			get { return Outputs[0]; }
		}

		#endregion
	}
}
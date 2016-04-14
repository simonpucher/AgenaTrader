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
	[Description("Enter the description for the new strategy here")]
	public class MyStrategy : UserStrategy
	{
		protected override void Initialize()
		{
		}

		protected override void OnBarUpdate()
		{
		}
	}
}

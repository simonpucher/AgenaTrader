#Template for Indicator, Condition and Strategy
[Originally posted as a question in the Agenatrader forum](http://www.tradeescort.com/phpbb_de/viewtopic.php?f=18&t=2680&p=11739)

This tutorial will show you our basic template for indicators, conditions and strategies.

#Indicator
In many cases we are starting with indicators because indicators are the best place to start on script development. 
You will be able to get pretty quick an indication if your trading idea is working and of course you are able to screen instruments visual and verify if your trading idea will be profitable.

##OnBarUpdate
Our main logic will be inside of the OnBarUpdate() method. In our example we are using moduls operation to check if there is currently an even or an odd minute. We want to show a long signal wenn it is even and we show a short signal on odd minutes: 

```C#
        protected override void OnBarUpdate()
        {
            if (Bars[0].Time.Minute % 2 == 0)
            {
                this.Indicator_Curve_Entry.Set(1);
            }
            else if (Bars[0].Time.Minute % 2 != 0)
            {
                this.Indicator_Curve_Entry.Set(-1);
            }
            else
            {
                this.Indicator_Curve_Entry.Set(0);
            }
        }
```

##TimeFrame
In our case we need a one minute time frame to work with the indicator. Please pay attention to set this method to public because we need this method also in other scripts like conditions or strategies. 
```C#
        public bool DatafeedPeriodicityIsValid(ITimeFrame timeframe)
        {
            TimeFrame tf = (TimeFrame)timeframe;
            if (tf.Periodicity == DatafeedHistoryPeriodicity.Minute && tf.PeriodicityValue == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
```

#Condition
##Bars required
Because of Backtesting reasons if we use the advanced mode we need at least two bars.
```C#
this.BarsRequired = 2;
```

##OnBarUpdate
Also in this case the main logic is inside of the onbarupdate method. Because our main logic is inside of the indicator itself we need to create an instance of this indicator. So we are able to get the data from the indicator and set our Occured object.
```C#
            //get the indicator
            DummyOneMinuteEvenOdd_Indicator _DummyOneMinuteEvenOdd_Indicator = LeadIndicator.DummyOneMinuteEvenOdd_Indicator();

            //get the value
            double returnvalue = _DummyOneMinuteEvenOdd_Indicator[0];
            
            //set the value
            Occurred.Set(returnvalue);
```

#Strategy
##Default time frame
If you start the strategy via the strategy-escort you need to set the default time frame. In this case you override the zero default time frame in strategy escort. If you start the strategy on a chart the time frame is automatically set, this will lead to a better usability in both cases.
```C#
            if (this.TimeFrame == null || this.TimeFrame.PeriodicityValue == 0)
            {
                this.TimeFrame = new TimeFrame(DatafeedHistoryPeriodicity.Minute, 1);
            }
```
##OnBarUpdate
Same procedure as in the condition. We create a fresh instance of the indicator and save the return value into a variable and we call the methods to create orders.

```C#
            DummyOneMinuteEvenOdd_Indicator _DummyOneMinuteEvenOdd_Indicator = LeadIndicator.DummyOneMinuteEvenOdd_Indicator();
            double returnvalue = _DummyOneMinuteEvenOdd_Indicator[0];
            if (returnvalue == 1)
            {
                this.DoEnterLong();
            }
            else if (returnvalue == -1)
            {
                this.DoEnterShort();
            }
	}
```

#Miscellaneous
##Filenames and Class names
To import all scripts into AgenaTrader without any error we add _indicator, _strategy, _condition or _alert to the filename and also to the c# class name.

##DisplayName and ToString()
In each script we override the ToString() method and the DisplayName to provide a readable string in AgenaTrader. So we do see a readable string instead of the class name in AgenaTrader.
```C#

        public override string ToString()
        {
            return "Dummy even/odd (S)";
        }

        public override string DisplayName
        {
            get
            {
                return "Dummy even/odd (S)";
            }
        }
```

#Files
[Indicator](https://github.com/simonpucher/AgenaTrader/blob/master/Indicator/DummyOneMinuteEvenOdd_Indicator.cs)

[Condition](https://github.com/simonpucher/AgenaTrader/blob/master/Condition/DummyOneMinuteEvenOdd_Condition.cs)

[Strategy](https://github.com/simonpucher/AgenaTrader/blob/master/Strategy/DummyOneMinuteEvenOdd_Strategy.cs)

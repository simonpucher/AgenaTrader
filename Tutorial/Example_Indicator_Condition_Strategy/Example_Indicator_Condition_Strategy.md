#Template for Indicator, Condition and Strategy
[Originally posted as a question in the Agenatrader forum](http://www.tradeescort.com/phpbb_de/viewtopic.php?f=18&t=2680&p=11739)

This tutorial will show you our basic template for indicators, conditions and strategies.


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

[Condition](https://github.com/simonpucher/AgenaTrader/blob/master/Condition/DummyOneMinuteEntryOdd_Condition.cs)

[Strategy](https://github.com/simonpucher/AgenaTrader/blob/master/Strategy/DummyOneMinuteOdd_Strategy.cs)

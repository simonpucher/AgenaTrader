#Template for Indicator, Condition and Strategy
[Originally posted as a question in the Agenatrader forum](http://www.tradeescort.com/phpbb_de/viewtopic.php?f=18&t=2680&p=11739)

This tutorial will show you our templates for indicators, conditions, strategies and give you the ability to communicate between these scripts. This will lead you to more code transparency and reduces your programming time. 

##Why do we want this?
AgenaTrader provides you the ability to create indicators, conditions, alerts and strategies in c# and use them during trading. 
Of course you can start creating an indicator and copy the code afterwards into a condition, a strategy or an alert.
Programming by using "copy & paste" is easy but on the other hand there are many disadvantages like lack of testing reasons, no single point for bug fixing and low maintainability. 

##Where should we start?
In many cases we are starting with indicators because indicators are the best place to start on script development. 
You will be able to get pretty quick an indication if your trading idea is working and of course you are able to screen instruments visual and verify if your trading idea will be profitable.

##Indicator
##Interface
We use an interface to ensure that all scripts like indicators, conditions, strategies and alerts use the same properties and methods. 
```C#
public interface IDummyOneMinuteEvenOdd
/* Here we define all properties we need. */
}
```

##Resultvalue
The result value object will holds all result data from the calculate method so we know what to do next. In a strategy we create a long or short order, in a condition we set the Occured object, and so on.
```C#
public class ResultValueDummyOneMinuteEvenOdd
/* Here we define all properties we need as a result of the calculate method. */
}
```

###Method calculate
We want to capsulate the main logic into one main methods in the indicator. In our case we do this using the following public method in the indicator.

```C#
 public ResultValueDummyOneMinuteEvenOdd calculate(IBar data, bool islongenabled, bool isshortenabled)
/* 
* Here we do all the smart work and in the end we return our result object
* So the condition or another scripts knows what to do (e.g. a strategy will create an order in the market)
*/
}
```

So it is possible that other scripts just need to call the calculate method of the indicator and get a decision what to do. 
In our case the calculate method return an object which holds all important information what has to be done. 
If we get the OrderAction.Buy as a Entry result, we need to start a long order in a strategy or we set the condition value to 1.

#Files
[Indicator](https://github.com/simonpucher/AgenaTrader/blob/master/Indicator/DummyOneMinuteEven_Indicator.cs)

[Condition](https://github.com/simonpucher/AgenaTrader/blob/master/Condition/DummyOneMinuteEntry_Condition.cs)

[Strategy](https://github.com/simonpucher/AgenaTrader/blob/master/Strategy/DummyOneMinute_Strategy.cs)

To compile these scripts without any error you also need access to the [utility indicator] (https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs) to use global source code elements.

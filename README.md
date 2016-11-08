[![GitHub issues](https://img.shields.io/github/issues/simonpucher/AgenaTrader.svg)](https://github.com/simonpucher/AgenaTrader/issues)
[![GitHub issues](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/simonpucher/AgenaTrader/blob/master/LICENSE.md)

#AgenaTrader Scripts
This project contains scripts for the AgenaTrader like indicators, conditions and strategies. If you have any questions or feedback for us please do not hesitate to contact us via Twitter [Simon](https://twitter.com/SimonPucher), [Christian](https://twitter.com/ckovar82) or [open an issue on GitHub](https://github.com/simonpucher/AgenaTrader/issues).

##IMPORTANT
###AgenaTrader version number
These scripts are compiled against AT 1.9.0.522.

###Utility Indicator
To compile indicators, conditions and other script resources without any error your **AgenaTrader also need access to the Utility Indicator** to use global source code elements! We use this indicator to share code snippets, so we do not need to copy and paste again and again. These reduces error sources, minimze the workload, gives us better testing opportunities and a better clarity. **You need to copy the Utility Indicator into the indicators directory of your AgenaTrader.** You will find the latest version of this Utility Indicator on GitHub: [Global Utilities](https://github.com/ScriptTrading/Basic-Package/blob/master/Utilities/GlobalUtilities_Utility.cs)

When you have copied the Utility Indicator into your indicator directory, you need to click on compile in your AgenaTrader.

##Version number vs. in progress
Each script should have a summary tag below the using directives. **If there is a version number you can start using the script.** If there is no summary tag or the version number is a text ("in progress"), we are working on this script and it is not recommended to use it.
```C#
/// <summary>
/// Version: 1.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// Description: https://en.wikipedia.org/wiki/Algorithmic_trading#Mean_reversion
/// -------------------------------------------------------------------------
/// ****** Important ******
/// To compile this script without any error you also need access to the utility indicator to use these global source code elements.
/// You will find this indicator on GitHub: https://github.com/ScriptTrading/Basic-Package/blob/master/Utilities/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
```

#Documentation
A rudimentary documentation exists in form of code comments in each script.
We are working on development [tutorials](https://github.com/simonpucher/AgenaTrader/tree/master/Tutorial) with a more detailed documentation and covering the basics for AgenaTrader scripts templates. If you want to help, please feel free to [open an issue on GitHub](https://github.com/simonpucher/AgenaTrader/issues) or fork this project and create a pull request.

#Installation of AgenaTrader scripts
- Locate your UserCode directory (e.g. C:\Users\yourusername\Documents\AgenaTrader\UserCode)
- Download the [Utility Indicator from this GitHub directory](https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utility/GlobalUtilities_Utility.cs) an copy it into the \Indicators directory in your UserCode directory (e.g. C:\Users\yourusername\Documents\AgenaTrader\UserCode\Indicators)
- Download the script you want to use and copy it into the correct directory (Indicators into \Indicators, Conditions into \ScriptedConditions, Strategies into \Strategies, Alerts into \AlertHandlers)
- Now you are ready to click on "compile" in AgenaTrader (in the menu bar: Strategy Handling => Programming => Compile)

##Contact
- [Twitter Simon](https://twitter.com/SimonPucher) [![Twitter](https://img.shields.io/twitter/follow/simonpucher.svg?style=social&label=Follow)](https://twitter.com/SimonPucher)
- [Twitter Christian](https://twitter.com/ckovar82) [![Twitter](https://img.shields.io/twitter/follow/ckovar82.svg?style=social&label=Follow)](https://twitter.com/ckovar82)

##Links
- [Twitter AgenaTrader](https://twitter.com/AgenaTrader) [![Twitter AgenaTrader Software](https://img.shields.io/twitter/follow/AgenaTrader.svg?style=social&label=Follow)](https://twitter.com/AgenaTrader)
- [AgenaTrader Software](http://www.tradeescort.com)
- [AgenaTrader Support Forum](http://www.tradeescort.com/phpbb_de/)

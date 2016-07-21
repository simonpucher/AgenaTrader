[![GitHub issues](https://img.shields.io/github/issues/badges/shields.svg?maxAge=2592000)]()

#AgenaTrader Scripts 
This project contains scripts for the AgenaTrader like indicators, conditions and strategies. If you have any questions or feedback for us please do not hesitate to contact us via Twitter [Simon](https://twitter.com/SimonPucher), [Christian](https://twitter.com/ckovar82) or [open an issue on GitHub](https://github.com/simonpucher/AgenaTrader/issues).

##IMPORTANT
###AgenaTrader version number
These scripts are compiled against AT 1.9.0.377 if you are using a higher version DO NOT USE these scripts!

###Utility Indicator
To compile indicators, conditions and other script resources without any error your **AgenaTrader also need access to the Utility Indicator** to use global source code elements! We use this indicator to share code snippets, so we do not need to copy and paste again and again. These reduces error sources, minimze the workload, gives us better testing opportunities and a better clarity. **You need to copy the Utility Indicator into the indicators directory of your AgenaTrader.** You will find the latest version of this Utility Indicator on GitHub: [Global Utilities](https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs)

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
/// You will find this indicator on GitHub: https://github.com/simonpucher/AgenaTrader/blob/master/Utility/GlobalUtilities_Utility.cs
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
```

#Documentation
A rudimentary documentation exists in form of code comments in each script.
We are working on development [tutorials](https://github.com/simonpucher/AgenaTrader/tree/master/Tutorial) with a more detailed documentation and covering the basics for AgenaTrader scripts templates. If you want to help, please feel free to [open an issue on GitHub](https://github.com/simonpucher/AgenaTrader/issues) or fork this project and create a pull request.

##Contact
- [Twitter Simon](https://twitter.com/SimonPucher) [![Twitter Simon](https://img.shields.io/twitter/follow/shields_io.svg?style=social&label=Follow&maxAge=2592000)](https://twitter.com/SimonPucher)
- [Twitter Christian](https://twitter.com/ckovar82) [![Twitter Christian](https://img.shields.io/twitter/follow/shields_io.svg?style=social&label=Follow&maxAge=2592000)](https://twitter.com/ckovar82)

##Links
- [Agena Trader Software](http://www.tradeescort.com)
- [Agena Trader Support Forum](http://www.tradeescort.com/phpbb_de/)

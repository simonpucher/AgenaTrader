[![GitHub issues](https://img.shields.io/github/issues/simonpucher/AgenaTrader.svg)](https://github.com/simonpucher/AgenaTrader/issues)
[![GitHub issues](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/simonpucher/AgenaTrader/blob/master/LICENSE.md)

# AgenaTrader Scripts
This project contains scripts for the AgenaTrader like indicators, conditions and strategies. If you have any questions or feedback for us please do not hesitate to contact us via Twitter [Simon](https://twitter.com/SimonPucher), [Christian](https://twitter.com/ckovar82) or [open an issue on GitHub](https://github.com/simonpucher/AgenaTrader/issues).

## IMPORTANT
### AgenaTrader version number
These scripts are compiled against AT 2.0.1.56.
Because of the new AgenaTrader API you need at least AgenaTrader 2.0.1.56.

### Utility Indicator in our Script-Trading Basic-Package
To compile indicators, conditions and other script resources without any error your **AgenaTrader also need access to the Utility Indicator** to use global source code elements! We use this indicator to share code snippets, so we do not need to copy and paste again and again. These reduces error sources, minimze the workload, gives us better testing opportunities and a better clarity. **You need to install the Utility Indicator into your AgenaTrader.**
[You will find the latest version of this Utility Indicator in our free Script-Trading Basic-Package](http://script-trading.com/en/agenatrader/)

## Version number vs. in progress
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

# Documentation
A rudimentary documentation exists in form of code comments in each script.
We are working on development [tutorials](https://github.com/simonpucher/AgenaTrader/tree/master/Tutorial) with a more detailed documentation and covering the basics for AgenaTrader scripts templates. If you want to help, please feel free to [open an issue on GitHub](https://github.com/simonpucher/AgenaTrader/issues) or fork this project and create a pull request.

# Installation of AgenaTrader scripts
-   Download our Global Utility code bundle [GlobalUtilities_Utility](https://raw.githubusercontent.com/simonpucher/AgenaTrader/master/Utilities/GlobalUtilities_Utility.cs)
-   Locate your UserCode directory (e.g. C:\\Users\\yourusername\\Documents\\AgenaTrader\\UserCode)
-   Install the Global Utility code bundle into your local \\Indicator folder.
-   Download the script you want to use and copy it into the local directory (Indicators into folder \\Indicators, Conditions into folder \\ScriptedConditions, Strategies into folder \\Strategies, Alerts into folder \\AlertHandlers)
-   Now you are ready to **click on compile in AgenaTrader** (via menu bar: Strategy Handling => Programming => Compile)

## Contact
-   [Twitter Simon](https://twitter.com/SimonPucher) [![Twitter](https://img.shields.io/twitter/follow/simonpucher.svg?style=social&label=Follow)](https://twitter.com/SimonPucher)
-   [Twitter Christian](https://twitter.com/ckovar82) [![Twitter](https://img.shields.io/twitter/follow/ckovar82.svg?style=social&label=Follow)](https://twitter.com/ckovar82)

## Links
-   [Twitter AgenaTrader](https://twitter.com/AgenaTrader) [![Twitter AgenaTrader Software](https://img.shields.io/twitter/follow/AgenaTrader.svg?style=social&label=Follow)](https://twitter.com/AgenaTrader)
-   [AgenaTrader Software](http://www.tradeescort.com)
-   [AgenaTrader Support Forum](http://www.tradeescort.com/phpbb_de/)
-   [GitHub Markdown](https://enterprise.github.com/downloads/en/markdown-cheatsheet.pdf)

#Connect GitHub with AgenaTrader

To use scripts from this GitHub project you have two options:
- Copy files to your local directories
- Clone GitHub project and link to your AgenaTrader **(our recommendation)**

##Copy files to your local directories
Of course this is easy to do and pretty straight forward if you just want to test scripts.
The **main disadvantage** is that your **local files will not update** if we modify scripts on this GitHub project!

##Clone GitHub project and link to your AgenaTrader (our recommendation)
This is our recommendation because in this  

###Install GitHub client
If you want to stay up to date and always get the latest updates you need to download the GitHub desktop client (of course you can use any other Git client).
https://desktop.github.com
Afterwards you should clone this GitHub project to your local workspace using GitHub client. 
The GitHub client will help you to stay up to date with this project.

###Linking files directly into AgenaTrader
Now we need to link the files from your local GitHub workspace into your local AgenaTrader directory.
We can use **mklink** to this work for us.

[This is a small guide, on how to work with mklink](http://www.howtogeek.com/howto/16226/complete-guide-to-symbolic-links-symlinks-on-windows-or-linux/)

The following example will show you how to link files into AgenaTrader:
mklink /d "C:\Users\Simon\Documents\AgenaTrader\UserCode\Indicators\github_indicator" "C:\workspace\Github\AgenaTrader\Indicator"
mklink /d "C:\Users\Simon\Documents\AgenaTrader\UserCode\ScriptedConditions\github_condition" "C:\workspace\Github\AgenaTrader\Condition"
mklink /d "C:\Users\Simon\Documents\AgenaTrader\UserCode\Indicators\github_utility" "C:\workspace\Github\AgenaTrader\Utility"
mklink /d "C:\Users\Simon\Documents\AgenaTrader\UserCode\Strategies\github_strategy" "C:\workspace\Github\AgenaTrader\Strategy"

Please pay attention to change the directories (source & target) to your local directories.


